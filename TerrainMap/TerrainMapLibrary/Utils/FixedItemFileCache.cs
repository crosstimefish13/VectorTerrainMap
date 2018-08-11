using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TerrainMapLibrary.Utils
{
    public sealed class FixedItemFileCache
    {
        private List<FileStream> streams;

        private List<byte> memoryItems;


        public string Root { get; private set; }

        public int ItemLength { get; private set; }

        public int FileItem { get; private set; }

        public bool AutoFlush { get; private set; }

        public long Count { get; private set; }

        public List<byte> this[long index]
        {
            get { return GetItem(index); }
        }


        public override bool Equals(object obj)
        {
            throw new NotSupportedException();
        }

        public override int GetHashCode()
        {
            return streams.GetHashCode() + memoryItems.GetHashCode() + Root.GetHashCode()
                + ItemLength.GetHashCode() + FileItem.GetHashCode() + AutoFlush.GetHashCode() + Count.GetHashCode();
        }

        public override string ToString()
        {
            return $"ItemLength: {ItemLength}, Count: {Count}";
        }


        public void Add(List<byte> item)
        {
            // validate record and MaxMemoryRecord
            if (item == null || item.Count != ItemLength)
            { throw new Exception("the length of item must be equal with ItemLength."); }

            // add record into memory
            memoryItems.AddRange(item);

            // auto flush if needed
            if (AutoFlush == true) { Flush(); }
        }

        public void Flush()
        {
            // return if does not need to flush
            if (memoryItems.Count == 0) { return; }

            if (streams.Count == 0)
            {
                // create first cache
                streams.Add(new FileStream(GetCachePath(Root, 0),
                    FileMode.Create,
                    FileAccess.ReadWrite));
            }

            var cacheStream = streams.Last();

            // the sum of flush count and cache stream length must less or equal than max file length
            int flushCount = memoryItems.Count;
            if (cacheStream.Length + memoryItems.Count > (long)FileItem * ItemLength)
            { flushCount = (int)((long)FileItem * ItemLength - cacheStream.Length); }

            // flush by using specified flush count
            cacheStream.Seek(0, SeekOrigin.End);
            cacheStream.Write(memoryItems.ToArray(), 0, flushCount);
            cacheStream.Flush();

            // update count
            Count += flushCount / ItemLength;

            // remove the flushed bytes from memory records
            if (flushCount < memoryItems.Count) { memoryItems = memoryItems.Skip(flushCount).ToList(); }
            else { memoryItems.Clear(); }

            if (memoryItems.Count > 0)
            {
                // need to create a new cache, then iterate flush until memory records count is 0
                streams.Add(new FileStream(GetCachePath(Root, streams.Count),
                    FileMode.Create,
                    FileAccess.ReadWrite));
                Flush();
            }
            else
            {
                // end of flush, need to update the record count into head file
                var headStream = new FileStream(GetHeadPath(Root), FileMode.Open, FileAccess.Write);
                headStream.Seek(24, SeekOrigin.Begin);
                var bytes = BitConverter.GetBytes(Count);
                headStream.Write(bytes, 0, bytes.Length);
                headStream.Close();
                headStream.Dispose();
            }
        }

        public void Close()
        {
            foreach (var cacheStream in streams)
            {
                cacheStream.Close();
                cacheStream.Dispose();
            }
        }


        public static FixedItemFileCache Generate(string version, string directory = null,
            int itemLength = 1000, int fileItem = 1000, bool autoFlush = false)
        {
            if (string.IsNullOrEmpty(version.Trim()))
            { throw new Exception("version must not be null or empty."); }

            if (itemLength <= 0)
            { throw new Exception("itemLength must be more than 0."); }

            if (fileItem <= 0)
            { throw new Exception("fileItem must be more than 0."); }

            // delete all files then create cache root directory
            string root = GetRoot(version, directory);
            if (Directory.Exists(root) == true) { Directory.Delete(root, true); }
            if (Directory.Exists(root) == false) { Directory.CreateDirectory(root); }

            // generate head file
            var headStream = new FileStream(GetHeadPath(root), FileMode.Create, FileAccess.Write);
            headStream.Seek(0, SeekOrigin.Begin);

            // 0-7, ItemLength
            var bytes = BitConverter.GetBytes((long)itemLength);
            headStream.Write(bytes, 0, bytes.Length);

            // 8-15, FileItem
            bytes = BitConverter.GetBytes((long)fileItem);
            headStream.Write(bytes, 0, bytes.Length);

            // 16-23, AutoFlush
            bytes = BitConverter.GetBytes(autoFlush == true ? 1L : 0L);
            headStream.Write(bytes, 0, bytes.Length);

            // 24-31, Count
            bytes = BitConverter.GetBytes(0L);
            headStream.Write(bytes, 0, bytes.Length);

            headStream.Flush();
            headStream.Close();
            headStream.Dispose();

            // reload cache
            var cache = Load(version, directory);
            return cache;
        }

        public static FixedItemFileCache Load(string version, string directory = null)
        {
            var cache = new FixedItemFileCache();
            cache.Root = GetRoot(version, directory);

            // read data from head file
            var headStream = new FileStream(GetHeadPath(cache.Root), FileMode.Open, FileAccess.Read);
            headStream.Seek(0, SeekOrigin.Begin);

            // 0-7, ItemLength
            var bytes = new byte[8];
            headStream.Read(bytes, 0, 8);
            cache.ItemLength = (int)BitConverter.ToInt64(bytes, 0);

            // 8-15, FileItem
            bytes = new byte[8];
            headStream.Read(bytes, 0, 8);
            cache.FileItem = (int)BitConverter.ToInt64(bytes, 0);

            // 16-23, AutoFlush
            bytes = new byte[8];
            headStream.Read(bytes, 0, 8);
            cache.AutoFlush = BitConverter.ToInt64(bytes, 0) == 1 ? true : false;

            // 24-31, Count
            bytes = new byte[8];
            headStream.Read(bytes, 0, 8);
            cache.Count = BitConverter.ToInt64(bytes, 0);

            headStream.Close();
            headStream.Dispose();

            // load all data files
            int cacheCount = (int)(cache.Count / cache.FileItem);
            if ((long)cacheCount * cache.FileItem < cache.Count) { cacheCount += 1; }
            for (int cacheNumber = 0; cacheNumber < cacheCount; cacheNumber++)
            {
                var cacheStream = new FileStream(GetCachePath(cache.Root, cacheNumber),
                    FileMode.Open,
                    FileAccess.ReadWrite);
                cache.streams.Add(cacheStream);
            }

            return cache;
        }


        private FixedItemFileCache()
        {
            streams = new List<FileStream>();
            memoryItems = new List<byte>();
            Root = string.Empty;
            ItemLength = 0;
            FileItem = 0;
            AutoFlush = false;
            Count = 0;
        }

        private List<byte> GetItem(long index)
        {
            // validate index
            if (index < 0 || index >= Count) { throw new IndexOutOfRangeException(); }

            // cache offset and stream
            int cacheNumber = (int)(index / FileItem);
            long cacheOffset = (index - (long)cacheNumber * FileItem) * ItemLength;
            var cacheStream = streams[cacheNumber];

            // read data
            cacheStream.Seek(cacheOffset, SeekOrigin.Begin);
            var bytes = new byte[ItemLength];
            cacheStream.Read(bytes, 0, ItemLength);

            return bytes.ToList();
        }


        private static string GetRoot(string version, string directory)
        {
            string cacheDirectory = directory;
            if (cacheDirectory == null)
            {
                var assemblyLocation = Assembly.GetExecutingAssembly().Location;
                cacheDirectory = Path.Combine(Path.GetDirectoryName(assemblyLocation),
                    Path.GetFileNameWithoutExtension(assemblyLocation));
            }

            cacheDirectory = Path.Combine(cacheDirectory, version);
            return cacheDirectory;
        }

        private static string GetHeadPath(string root)
        {
            string filePath = Path.Combine(root, "head.data");
            return filePath;
        }

        private static string GetCachePath(string root, int cacheNumber)
        {
            string filePath = Path.Combine(root, $"{cacheNumber.ToString().PadLeft(8, '0')}.data");
            return filePath;
        }
    }
}
