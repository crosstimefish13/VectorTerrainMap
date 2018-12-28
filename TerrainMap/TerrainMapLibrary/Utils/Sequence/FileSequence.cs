using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TerrainMapLibrary.Utils.Sequence
{
    public sealed class FileSequence<T> : ISequence<T> where T : IElement
    {
        private long fileElementCount;

        private List<FileStream> bodyStreams;

        private List<T> addedElements;

        private Dictionary<long, T> updatedElements;

        private Dictionary<long, T> cacheElements;

        public string Root { get; private set; }

        public int ElementLength { get; private set; }

        public int FileElement { get; private set; }

        public int MemoryElement { get; private set; }

        public long Count { get; private set; }

        public T this[long index]
        {
            get
            {
                return GetElement(index);
            }
            set
            {
                Update(index, value);
            }
        }

        public bool EnableMemoryCache { get; set; }

        public override bool Equals(object obj)
        {
            throw new NotSupportedException();
        }

        public override int GetHashCode()
        {
            int hashCode =
                fileElementCount.GetHashCode() +
                bodyStreams.GetHashCode() +
                addedElements.GetHashCode() +
                updatedElements.GetHashCode() +
                cacheElements.GetHashCode() +
                Root.GetHashCode() +
                ElementLength.GetHashCode() +
                FileElement.GetHashCode() +
                MemoryElement.GetHashCode() +
                Count.GetHashCode() +
                EnableMemoryCache.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return $"ItemLength: {ElementLength}, Count: {Count}";
        }

        public T GetElement(long index)
        {
            // validate index
            if (index < 0 || index >= Count)
            {
                throw new IndexOutOfRangeException();
            }

            // get element from cache, added or updated if needed
            if (TryGetCache(index, out T element) == false)
            {
                if (index >= fileElementCount)
                {
                    element = addedElements[(int)(index - fileElementCount)];
                }
                else if (updatedElements.ContainsKey(index) == true)
                {
                    element = updatedElements[index];
                }
                else
                {
                    // get element from file
                    int bodyNumber = (int)(index / FileElement);
                    long boddyOffset = (index - (long)bodyNumber * FileElement) * ElementLength;
                    var bodyStream = bodyStreams[bodyNumber];

                    // initialize element by reading data
                    element = Activator.CreateInstance<T>();
                    var array = new byte[element.ArrayLength];
                    bodyStream.Seek(boddyOffset, SeekOrigin.Begin);
                    bodyStream.Read(array, 0, array.Length);
                    element.Initialize(array);
                }

                // update cache
                UpdateCache(index, element);
            }

            return element;
        }

        public void Add(T element)
        {
            // flush added elements if needed
            if (addedElements.Count >= MemoryElement)
            {
                FlushAdded();
            }

            // add element into memeory and cache
            addedElements.Add(element);
            UpdateCache(Count, element);
            Count += 1;
        }

        public void Update(long index, T element)
        {
            // validate index
            if (index < 0 || index >= Count)
            {
                throw new IndexOutOfRangeException();
            }


            if (index >= fileElementCount)
            {
                // update added memory list if needed
                addedElements[(int)(index - fileElementCount)] = element;
            }
            else
            {
                if (updatedElements.ContainsKey(index))
                {
                    // update elements into memroy
                    updatedElements[index] = element;
                }
                else
                {
                    // flush updated elements if needed
                    if (updatedElements.Count >= MemoryElement)
                    {
                        FlushUpdated();
                    }

                    // add new update elements into memroy
                    updatedElements.Add(index, element);
                }
            }

            // update cache
            UpdateCache(index, element);
        }

        public void Flush()
        {
            FlushUpdated();
            FlushAdded();
        }

        public void Close()
        {
            Flush();
            foreach (var stream in bodyStreams)
            {
                stream.Close();
                stream.Dispose();
            }

            addedElements.Clear();
            updatedElements.Clear();
            cacheElements.Clear();
        }

        public static FileSequence<T> Generate(string root, int elementLength = 100, int fileElement = 100, int memoryElement = 80)
        {
            if (elementLength <= 0)
            {
                throw new Exception("elementLength must be more than 0.");
            }

            if (fileElement <= 0)
            {
                throw new Exception("fileElement must be more than 0.");
            }

            if (memoryElement < 0)
            {
                throw new Exception("memoryElement must be more than or equal with 0.");
            }

            // delete all files then create root directory
            if (Directory.Exists(root) == true)
            {
                Directory.Delete(root, true);
            }

            if (Directory.Exists(root) == false)
            {
                Directory.CreateDirectory(root);
            }

            // generate head file
            var headStream = new FileStream(GetHeadPath(root), FileMode.Create, FileAccess.Write);
            headStream.Seek(0, SeekOrigin.Begin);

            // 0-3, elementLength
            var array = BitConverter.GetBytes(elementLength);
            headStream.Write(array, 0, array.Length);

            // 4-7, fileElement
            array = BitConverter.GetBytes(fileElement);
            headStream.Write(array, 0, array.Length);

            // 8-11, memoryElement
            array = BitConverter.GetBytes(memoryElement);
            headStream.Write(array, 0, array.Length);

            // 12-19, Count
            array = BitConverter.GetBytes(0L);
            headStream.Write(array, 0, array.Length);

            headStream.Flush();
            headStream.Close();
            headStream.Dispose();

            // reload cache
            var cache = Load(root);
            return cache;
        }

        public static FileSequence<T> Load(string root)
        {
            var cache = new FileSequence<T>(root);

            // read data from head file
            var headStream = new FileStream(GetHeadPath(cache.Root), FileMode.Open, FileAccess.Read);
            headStream.Seek(0, SeekOrigin.Begin);

            // 0-3, ElementLength
            var array = new byte[4];
            headStream.Read(array, 0, array.Length);
            cache.ElementLength = BitConverter.ToInt32(array, 0);

            // 4-7, FileElement
            array = new byte[4];
            headStream.Read(array, 0, array.Length);
            cache.FileElement = BitConverter.ToInt32(array, 0);

            // 8-11, MemoryElement
            array = new byte[4];
            headStream.Read(array, 0, array.Length);
            cache.MemoryElement = BitConverter.ToInt32(array, 0);

            // 12-19, Count
            array = new byte[8];
            headStream.Read(array, 0, 8);
            cache.Count = BitConverter.ToInt64(array, 0);
            cache.fileElementCount = cache.Count;

            headStream.Close();
            headStream.Dispose();

            // load all data files
            int bodyCount = (int)(cache.Count / cache.FileElement);
            if ((long)bodyCount * cache.FileElement < cache.Count)
            {
                bodyCount += 1;
            }
            for (int bodyNumber = 0; bodyNumber < bodyCount; bodyNumber++)
            {
                var bodyStream = new FileStream(
                    GetBodyPath(cache.Root, bodyNumber),
                    FileMode.Open, 
                    FileAccess.ReadWrite
                );
                cache.bodyStreams.Add(bodyStream);
            }

            return cache;
        }

        private FileSequence(string root)
        {
            fileElementCount = 0;
            bodyStreams = new List<FileStream>();
            addedElements = new List<T>();
            updatedElements = new Dictionary<long, T>();
            cacheElements = new Dictionary<long, T>();
            Root = root;
            ElementLength = 0;
            FileElement = 0;
            MemoryElement = 0;
            Count = 0;
            EnableMemoryCache = false;
        }

        private void UpdateCache(long index, T element)
        {
            // return if disabled memeory cache
            if (EnableMemoryCache == false)
            {
                return;
            }

            if (cacheElements.ContainsKey(index))
            {
                cacheElements[index] = element;
            }
            else
            {
                if (cacheElements.Count >= MemoryElement)
                {
                    cacheElements.Remove(cacheElements.First().Key);
                }

                cacheElements.Add(index, element);
            }
        }

        private bool TryGetCache(long index, out T element)
        {
            // return if disabled memeory cache
            if (EnableMemoryCache == false)
            {
                element = default(T);
                return false;
            }

            return cacheElements.TryGetValue(index, out element);
        }

        private void FlushUpdated()
        {
            if (updatedElements.Count == 0)
            {
                return;
            }

            // a dictionary to map body number and its updated elements
            var bodyNumbersUpdatedElements = new Dictionary<int, Dictionary<long, T>>();
            foreach (var indexElement in updatedElements)
            {
                long index = indexElement.Key;
                var element = indexElement.Value;
                int bodyNumber = (int)(index / FileElement);

                if (bodyNumbersUpdatedElements.ContainsKey(bodyNumber) == false)
                {
                    bodyNumbersUpdatedElements.Add(bodyNumber, new Dictionary<long, T>());
                }

                bodyNumbersUpdatedElements[bodyNumber].Add(index, element);
            }

            // write elements in each cache file
            foreach (var bodyNumberUpdatedElements in bodyNumbersUpdatedElements)
            {
                // body stream
                int bodyNumber = bodyNumberUpdatedElements.Key;
                var bodyStream = bodyStreams[bodyNumber];
                foreach (var indexElement in bodyNumberUpdatedElements.Value)
                {
                    // offset
                    long index = indexElement.Key;
                    var element = indexElement.Value;
                    long boddyOffset = (index - (long)bodyNumber * FileElement) * ElementLength;

                    // write data
                    var array = element.ToArray();
                    bodyStream.Seek(boddyOffset, SeekOrigin.Begin);
                    bodyStream.Write(array, 0, array.Length);
                }

                bodyStream.Flush();
            }

            updatedElements.Clear();
        }

        private void FlushAdded()
        {
            if (addedElements.Count == 0)
            {
                return;
            }

            // create first cache if needed
            if (bodyStreams.Count == 0)
            {
                bodyStreams.Add(new FileStream(GetBodyPath(Root, 0), FileMode.Create, FileAccess.ReadWrite));
            }

            var bodyStream = bodyStreams[bodyStreams.Count - 1];

            // the sum of flush count and cache stream length must less or equal than max file length
            int flushCount = addedElements.Count;
            if (bodyStream.Length + (long)flushCount * ElementLength > (long)FileElement * ElementLength)
            { flushCount = (int)(FileElement - bodyStream.Length / ElementLength); }

            // flush specified array
            var list = new List<byte>();
            for (int i = 0; i < flushCount; i++)
            { list.AddRange(addedElements[i].ToArray()); }

            bodyStream.Seek(0, SeekOrigin.End);
            bodyStream.Write(list.ToArray(), 0, list.Count);
            bodyStream.Flush();

            // update count
            fileElementCount += flushCount;

            // remove the flushed items
            if (flushCount < addedElements.Count)
            {
                addedElements = addedElements.Skip(flushCount).ToList();
            }
            else
            {
                addedElements.Clear();
            }

            if (addedElements.Count > 0)
            {
                // need to create a new cache, then iterate flush until added items count is 0
                bodyStreams.Add(
                    new FileStream(GetBodyPath(Root, bodyStreams.Count),
                    FileMode.Create,
                    FileAccess.ReadWrite)
                );
                FlushAdded();
                return;
            }

            // end of flush, need to update the record count into head file
            var headStream = new FileStream(GetHeadPath(Root), FileMode.Open, FileAccess.Write);
            headStream.Seek(12, SeekOrigin.Begin);
            var array = BitConverter.GetBytes(Count);
            headStream.Write(array, 0, array.Length);
            headStream.Close();
            headStream.Dispose();
        }

        private static string GetHeadPath(string root)
        {
            string filePath = Path.Combine(root, "head.data");
            return filePath;
        }

        private static string GetBodyPath(string root, int bodyNumber)
        {
            string filePath = Path.Combine(root, $"{bodyNumber.ToString().PadLeft(8, '0')}.data");
            return filePath;
        }
    }
}
