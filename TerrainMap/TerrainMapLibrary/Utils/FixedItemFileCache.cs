using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TerrainMapLibrary.Mathematics.Sequencer;

namespace TerrainMapLibrary.Utils
{
    public sealed class FixedItemFileCache : ISequence<List<byte>>
    {
        private List<FileStream> streams;

        private List<List<byte>> addedItems;

        private Dictionary<long, List<byte>> updatedItems;


        public string Root { get; private set; }

        public int ItemLength { get; private set; }

        public int FileItem { get; private set; }

        public bool AutoFlush { get; private set; }

        public long Count { get; private set; }

        public List<byte> this[long index]
        {
            get { return GetItem(index); }
            set { UpdateItem(index, value); }
        }


        public override bool Equals(object obj)
        {
            throw new NotSupportedException();
        }

        public override int GetHashCode()
        {
            return streams.GetHashCode() + addedItems.GetHashCode() + updatedItems.GetHashCode() + Root.GetHashCode()
                + ItemLength.GetHashCode() + FileItem.GetHashCode() + AutoFlush.GetHashCode() + Count.GetHashCode();
        }

        public override string ToString()
        {
            return $"ItemLength: {ItemLength}, Count: {Count}";
        }


        public void Add(List<byte> item)
        {
            if (item == null || item.Count != ItemLength)
            { throw new Exception("the length of item must be equal with ItemLength."); }

            addedItems.Add(item);
            if (AutoFlush == true) { Flush(); }
        }

        public void Flush()
        {
            FlushUpdated();
            FlushAdded();
        }

        public void Close()
        {
            Flush();
            foreach (var stream in streams)
            {
                stream.Close();
                stream.Dispose();
            }

            addedItems.Clear();
            updatedItems.Clear();
        }


        public static FixedItemFileCache Generate(string root, int itemLength = 100, int fileItem = 100,
            bool autoFlush = false)
        {
            if (itemLength <= 0)
            { throw new Exception("itemLength must be more than 0."); }

            if (fileItem <= 0)
            { throw new Exception("fileItem must be more than 0."); }

            // delete all files then create root directory
            if (Directory.Exists(root) == true) { Directory.Delete(root, true); }
            if (Directory.Exists(root) == false) { Directory.CreateDirectory(root); }

            // generate head file
            var headStream = new FileStream(GetHeadPath(root), FileMode.Create, FileAccess.Write);
            headStream.Seek(0, SeekOrigin.Begin);

            // 0-3, ItemLength
            var array = BitConverter.GetBytes(itemLength);
            headStream.Write(array, 0, array.Length);

            // 4-7, FileItem
            array = BitConverter.GetBytes(fileItem);
            headStream.Write(array, 0, array.Length);

            // 8, AutoFlush
            array = BitConverter.GetBytes(autoFlush);
            headStream.Write(array, 0, array.Length);

            // 9-16, Count
            array = BitConverter.GetBytes(0L);
            headStream.Write(array, 0, array.Length);

            headStream.Flush();
            headStream.Close();
            headStream.Dispose();

            // reload cache
            var cache = Load(root);
            return cache;
        }

        public static FixedItemFileCache Load(string root)
        {
            var cache = new FixedItemFileCache(root);

            // read data from head file
            var headStream = new FileStream(GetHeadPath(cache.Root), FileMode.Open, FileAccess.Read);
            headStream.Seek(0, SeekOrigin.Begin);

            // 0-3, ItemLength
            var array = new byte[4];
            headStream.Read(array, 0, array.Length);
            cache.ItemLength = BitConverter.ToInt32(array, 0);

            // 4-7, FileItem
            array = new byte[4];
            headStream.Read(array, 0, array.Length);
            cache.FileItem = BitConverter.ToInt32(array, 0);

            // 8, AutoFlush
            array = new byte[1];
            headStream.Read(array, 0, array.Length);
            cache.AutoFlush = BitConverter.ToBoolean(array, 0);

            // 9-16, Count
            array = new byte[8];
            headStream.Read(array, 0, 8);
            cache.Count = BitConverter.ToInt64(array, 0);

            headStream.Close();
            headStream.Dispose();

            // load all data files
            int bodyCount = (int)(cache.Count / cache.FileItem);
            if ((long)bodyCount * cache.FileItem < cache.Count) { bodyCount += 1; }
            for (int bodyNumber = 0; bodyNumber < bodyCount; bodyNumber++)
            {
                var bodyStream = new FileStream(GetBodyPath(cache.Root, bodyNumber),
                    FileMode.Open, FileAccess.ReadWrite);
                cache.streams.Add(bodyStream);
            }

            return cache;
        }


        private FixedItemFileCache(string root)
        {
            streams = new List<FileStream>();
            addedItems = new List<List<byte>>();
            updatedItems = new Dictionary<long, List<byte>>();
            Root = root;
            ItemLength = 0;
            FileItem = 0;
            AutoFlush = false;
            Count = 0;
        }


        private List<byte> GetItem(long index)
        {
            // validate index
            if (index < 0 || index >= Count) { throw new IndexOutOfRangeException(); }

            // offset and stream
            int bodyNumber = (int)(index / FileItem);
            long boddyOffset = (index - (long)bodyNumber * FileItem) * ItemLength;
            var stream = streams[bodyNumber];

            // read data
            stream.Seek(boddyOffset, SeekOrigin.Begin);
            var array = new byte[ItemLength];
            stream.Read(array, 0, array.Length);

            return array.ToList();
        }

        private void UpdateItem(long index, List<byte> value)
        {
            if (index < 0 || index >= Count) { throw new IndexOutOfRangeException(); }

            if (value == null || value.Count != ItemLength)
            { throw new Exception("the length of value must be equal with ItemLength."); }

            updatedItems.Add(index, value);
            if (AutoFlush == true) { FlushUpdated(); }
        }

        private void FlushUpdated()
        {
            // a dictionary to map body number and its updated items
            var bodyNumberUpdatedItemsList = new Dictionary<int, Dictionary<long, List<byte>>>();
            foreach (var indexItem in updatedItems)
            {
                long index = indexItem.Key;
                var item = indexItem.Value;
                int bodyNumber = (int)(index / FileItem);

                if (bodyNumberUpdatedItemsList.ContainsKey(bodyNumber) == false)
                { bodyNumberUpdatedItemsList.Add(bodyNumber, new Dictionary<long, List<byte>>()); }

                bodyNumberUpdatedItemsList[bodyNumber].Add(index, item);
            }

            // write items in each cache file
            foreach (var bodyNumberUpdatedItems in bodyNumberUpdatedItemsList)
            {
                // stream
                int bodyNumber = bodyNumberUpdatedItems.Key;
                var stream = streams[bodyNumber];
                foreach (var indexItem in bodyNumberUpdatedItems.Value)
                {
                    // offset
                    long index = indexItem.Key;
                    var item = indexItem.Value;
                    long boddyOffset = (index - (long)bodyNumber * FileItem) * ItemLength;

                    // write data
                    stream.Seek(boddyOffset, SeekOrigin.Begin);
                    var array = item.ToArray();
                    stream.Write(array, 0, array.Length);
                }

                stream.Flush();
            }

            updatedItems.Clear();
        }

        private void FlushAdded()
        {
            if (addedItems.Count == 0) { return; }

            // create first cache if needed
            if (streams.Count == 0)
            { streams.Add(new FileStream(GetBodyPath(Root, 0), FileMode.Create, FileAccess.ReadWrite)); }

            var bodyStream = streams[streams.Count - 1];

            // the sum of flush count and cache stream length must less or equal than max file length
            int flushCount = addedItems.Count;
            if (bodyStream.Length + (long)flushCount * ItemLength > (long)FileItem * ItemLength)
            { flushCount = (int)(FileItem - bodyStream.Length / ItemLength); }

            // flush specified array
            var data = new List<byte>();
            for (int i = 0; i < flushCount; i++)
            { data.AddRange(addedItems[i]); }

            bodyStream.Seek(0, SeekOrigin.End);
            bodyStream.Write(data.ToArray(), 0, data.Count);
            bodyStream.Flush();

            // update count
            Count += flushCount;

            // remove the flushed items
            if (flushCount < addedItems.Count) { addedItems = addedItems.Skip(flushCount).ToList(); }
            else { addedItems.Clear(); }

            if (addedItems.Count > 0)
            {
                // need to create a new cache, then iterate flush until added items count is 0
                streams.Add(new FileStream(GetBodyPath(Root, streams.Count),
                    FileMode.Create, FileAccess.ReadWrite));
                FlushAdded();
                return;
            }

            // end of flush, need to update the record count into head file
            var headStream = new FileStream(GetHeadPath(Root), FileMode.Open, FileAccess.Write);
            headStream.Seek(9, SeekOrigin.Begin);
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


        List<byte> ISequence<List<byte>>.GetItem(long index)
        {
            return GetItem(index);
        }

        void ISequence<List<byte>>.UpdateItem(long index, List<byte> item)
        {
            UpdateItem(index, item);
        }

        long ISequence<List<byte>>.GetCount()
        {
            return Count;
        }

        void ISequence<List<byte>>.Flush()
        {
            FlushUpdated();
        }

        bool ISequence<List<byte>>.GetAutoFlush()
        {
            return AutoFlush;
        }
    }
}
