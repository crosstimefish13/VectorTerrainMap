using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TerrainMapLibrary.Interpolator.Data;
using TerrainMapLibrary.Mathematics;

namespace TerrainMapLibrary.Interpolator.Kriging
{
    public class KrigingInterpolator
    {
        public MapPointList Data { get; private set; }


        public KrigingInterpolator(MapPointList data)
        {
            if (data == null || data.Count < 2) { KrigingException.InvalidData("data"); }

            Data = data;
        }


        public void GenerateSemivarianceMapIndex(Counter counter = null)
        {
            var cache = SemivarianceMapFileCache.Generate("0", null, 64, 33554432, false);

            int flushStep = 0;
            if (counter != null) { counter.Reset((long)(Data.Count - 1) * Data.Count); }

            for (int i = 0; i < Data.Count; i++)
            {
                var left = Data[i];
                for (int j = i + 1; j < Data.Count; j++)
                {
                    var right = Data[j];
                    double vectorX = Common.EuclidDistance(Data[i].X, Data[i].Y, Data[j].X, Data[j].Y);
                    double vectorY = Common.Semivariance(Data[i].Z, Data[j].Z);

                    var bytes = new List<byte>();
                    bytes.AddRange(BitConverter.GetBytes(vectorX));
                    bytes.AddRange(BitConverter.GetBytes(vectorY));
                    bytes.AddRange(BitConverter.GetBytes(left.X));
                    bytes.AddRange(BitConverter.GetBytes(left.Y));
                    bytes.AddRange(BitConverter.GetBytes(left.Z));
                    bytes.AddRange(BitConverter.GetBytes(right.X));
                    bytes.AddRange(BitConverter.GetBytes(right.Y));
                    bytes.AddRange(BitConverter.GetBytes(right.Z));
                    cache.Add(bytes);

                    flushStep = flushStep >= 1638400 ? 0 : flushStep + 1;
                    if (flushStep == 0) { cache.Flush(); }

                    if (counter != null) { counter.AddStep(); }
                }
            }

            if (counter != null) { counter.Reset(); }

            cache.Close();
        }


        public class SemivarianceMapFileCache
        {
            private List<FileStream> cachesStream;

            private List<byte> memoryRecords;


            public string CacheRoot { get; private set; }

            public int RecordLength { get; private set; }

            public int MaxFileRecord { get; private set; }

            public bool AutoFlush { get; private set; }

            public long Count { get; private set; }

            public List<byte> this[long index]
            {
                get { return GetRecord(index); }
            }


            public void Add(List<byte> record)
            {
                // validate record and MaxMemoryRecord
                if (record == null || record.Count != RecordLength) { KrigingException.InvalidRecord("record"); }

                // add record into memory
                memoryRecords.AddRange(record);

                // auto flush if needed
                if (AutoFlush == true) { Flush(); }
            }

            public void Flush()
            {
                // return if does not need to flush
                if (memoryRecords.Count == 0) { return; }

                if (cachesStream.Count == 0)
                {
                    // create first cache
                    cachesStream.Add(new FileStream(GetCachePath(CacheRoot, 0),
                        FileMode.Create,
                        FileAccess.ReadWrite));
                }

                var cacheStream = cachesStream.Last();

                // the sum of flush count and cache stream length must less or equal than max file length
                int flushCount = memoryRecords.Count;
                if (cacheStream.Length + memoryRecords.Count > (long)MaxFileRecord * RecordLength)
                { flushCount = (int)((long)MaxFileRecord * RecordLength - cacheStream.Length); }

                // flush by using specified flush count
                cacheStream.Seek(0, SeekOrigin.End);
                cacheStream.Write(memoryRecords.ToArray(), 0, flushCount);
                cacheStream.Flush();

                // update count
                Count += flushCount / RecordLength;

                // remove the flushed bytes from memory records
                if (flushCount < memoryRecords.Count) { memoryRecords = memoryRecords.Skip(flushCount).ToList(); }
                else { memoryRecords.Clear(); }

                if (memoryRecords.Count > 0)
                {
                    // need to create a new cache, then iterate flush until memory records count is 0
                    cachesStream.Add(new FileStream(GetCachePath(CacheRoot, cachesStream.Count),
                        FileMode.Create,
                        FileAccess.ReadWrite));
                    Flush();
                }
                else
                {
                    // end of flush, need to update the record count into title file
                    var titleStream = new FileStream(GetTitlePath(CacheRoot), FileMode.Open, FileAccess.Write);
                    titleStream.Seek(24, SeekOrigin.Begin);
                    var bytes = BitConverter.GetBytes(Count);
                    titleStream.Write(bytes, 0, bytes.Length);
                    titleStream.Close();
                    titleStream.Dispose();
                }
            }

            public void Close()
            {
                foreach (var cacheStream in cachesStream)
                {
                    cacheStream.Close();
                    cacheStream.Dispose();
                }
            }

            public static SemivarianceMapFileCache Generate(string cacheVersion,
                string cacheDirectory = null,
                int recordLength = 100,
                int maxFileRecord = 10000000,
                bool autoFlush = false)
            {
                // validate paramaters
                if (string.IsNullOrEmpty(cacheVersion.Trim()))
                { KrigingException.InvalidCacheVersion("cacheVersion"); }
                if (recordLength <= 0) { KrigingException.InvalidRecordLength("recordLength"); }
                if (maxFileRecord <= 0) { KrigingException.InvalidMaxFileRecord("maxFileRecord"); }

                // delete all files then create cache root directory
                string cacheRoot = GetCacheRoot(cacheVersion, cacheDirectory);
                if (Directory.Exists(cacheRoot) == true) { Directory.Delete(cacheRoot, true); }
                if (Directory.Exists(cacheRoot) == false) { Directory.CreateDirectory(cacheRoot); }

                // generate title file
                var titleStream = new FileStream(GetTitlePath(cacheRoot), FileMode.Create, FileAccess.Write);
                titleStream.Seek(0, SeekOrigin.Begin);

                // 0-7, RecordLength
                var bytes = BitConverter.GetBytes((long)recordLength);
                titleStream.Write(bytes, 0, bytes.Length);

                // 8-15, MaxFileRecord
                bytes = BitConverter.GetBytes((long)maxFileRecord);
                titleStream.Write(bytes, 0, bytes.Length);

                // 16-23, AutoFlush
                bytes = BitConverter.GetBytes(autoFlush == true ? 1L : 0L);
                titleStream.Write(bytes, 0, bytes.Length);

                // 24-31, Count
                bytes = BitConverter.GetBytes(0L);
                titleStream.Write(bytes, 0, bytes.Length);

                titleStream.Flush();
                titleStream.Close();
                titleStream.Dispose();

                // reload cache
                var cache = Load(cacheVersion, cacheDirectory);
                return cache;
            }

            public static SemivarianceMapFileCache Load(string cacheVersion, string cacheDirectory = null)
            {
                var cache = new SemivarianceMapFileCache();
                cache.CacheRoot = GetCacheRoot(cacheVersion, cacheDirectory);

                // read data from title file
                var titleStream = new FileStream(GetTitlePath(cache.CacheRoot), FileMode.Open, FileAccess.Read);
                titleStream.Seek(0, SeekOrigin.Begin);

                // 0-7, RecordLength
                var bytes = new byte[8];
                titleStream.Read(bytes, 0, 8);
                cache.RecordLength = (int)BitConverter.ToInt64(bytes, 0);

                // 8-15, MaxFileRecord
                bytes = new byte[8];
                titleStream.Read(bytes, 0, 8);
                cache.MaxFileRecord = (int)BitConverter.ToInt64(bytes, 0);

                // 16-23, AutoFlush
                bytes = new byte[8];
                titleStream.Read(bytes, 0, 8);
                cache.AutoFlush = BitConverter.ToInt64(bytes, 0) == 1 ? true : false;

                // 24-31, Count
                bytes = new byte[8];
                titleStream.Read(bytes, 0, 8);
                cache.Count = BitConverter.ToInt64(bytes, 0);

                titleStream.Close();
                titleStream.Dispose();

                // load all data files
                int cacheCount = (int)(cache.Count / cache.MaxFileRecord);
                if ((long)cacheCount * cache.MaxFileRecord < cache.Count) { cacheCount += 1; }
                for (int cacheNumber = 0; cacheNumber < cacheCount; cacheNumber++)
                {
                    var cacheStream = new FileStream(GetCachePath(cache.CacheRoot, cacheNumber),
                        FileMode.Open,
                        FileAccess.ReadWrite);
                    cache.cachesStream.Add(cacheStream);
                }

                return cache;
            }


            private SemivarianceMapFileCache()
            {
                cachesStream = new List<FileStream>();
                memoryRecords = new List<byte>();
                CacheRoot = string.Empty;
                RecordLength = 0;
                MaxFileRecord = 0;
                AutoFlush = false;
                Count = 0;
            }

            private List<byte> GetRecord(long index)
            {
                // validate index
                if (index < 0 || index >= Count) { KrigingException.InvalidIndex("index"); }

                // cache offset and stream
                int cacheNumber = (int)(index / MaxFileRecord);
                long cacheOffset = (index - (long)cacheNumber * MaxFileRecord) * RecordLength;
                var cacheStream = cachesStream[cacheNumber];

                // read data
                cacheStream.Seek(cacheOffset, SeekOrigin.Begin);
                var bytes = new byte[RecordLength];
                cacheStream.Read(bytes, 0, RecordLength);

                return bytes.ToList();
            }

            private static string GetCacheRoot(string cacheVersion, string cacheDirectory)
            {
                string directory = cacheDirectory;
                if (directory == null)
                {
                    var assemblyLocation = Assembly.GetExecutingAssembly().Location;
                    directory = Path.Combine(Path.GetDirectoryName(assemblyLocation),
                        Path.GetFileNameWithoutExtension(assemblyLocation));
                }

                directory = Path.Combine(directory, cacheVersion);
                return directory;
            }

            private static string GetTitlePath(string cacheRoot)
            {
                string filePath = Path.Combine(cacheRoot, "title.data");
                return filePath;
            }

            private static string GetCachePath(string cacheRoot, int cacheNumber)
            {
                string filePath = Path.Combine(cacheRoot, $"{cacheNumber.ToString().PadLeft(8, '0')}.data");
                return filePath;
            }
        }


        public class Counter
        {
            private Stopwatch refreshTimer;

            private long stepPerRefresh;


            public long Step { get; private set; }

            public long StepLength { get; private set; }

            public long TicksLeft { get; private set; }

            public long RefreshInterval { get; private set; }

            public Action<Counter> RefreshAction { get; set; }


            public Counter(long refreshInterval = 1000, Action<Counter> refreshAction = null)
            {
                refreshTimer = new Stopwatch();
                stepPerRefresh = 0;
                Step = 0;
                StepLength = 0;
                TicksLeft = 0;
                RefreshInterval = refreshInterval;
                RefreshAction = refreshAction;
            }


            public void Reset(long stepLength = 0)
            {
                refreshTimer.Stop();
                stepPerRefresh = 0;
                Step = 0;
                StepLength = stepLength;
                TicksLeft = 0;

                if (RefreshAction != null) { RefreshAction.Invoke(this); }
            }

            public bool AddStep()
            {
                Step += 1;

                if (Step >= StepLength)
                {
                    // stop refresh timer, time left shold be 0, and step is equal with step length
                    refreshTimer.Stop();
                    stepPerRefresh = 0;
                    Step = StepLength;
                    TicksLeft = 0;

                    if (RefreshAction != null) { RefreshAction.Invoke(this); }

                    return false;
                }

                // start refresh timer if needed
                if (refreshTimer.IsRunning == false) { refreshTimer.Restart(); }

                stepPerRefresh += 1;

                // update the ticks left per refresh interval if needed
                long refreshTicks = RefreshInterval * TimeSpan.TicksPerMillisecond;
                if (refreshTimer.ElapsedTicks >= refreshTicks)
                {
                    refreshTimer.Stop();

                    long newTicksLeft = (StepLength - Step) / stepPerRefresh * refreshTicks;

                    // update ticks left if needed
                    if (TicksLeft == 0 || TicksLeft - newTicksLeft > refreshTicks) { TicksLeft = newTicksLeft; }
                    else if (TicksLeft - newTicksLeft <= refreshTicks) { TicksLeft -= refreshTicks; }

                    if (TicksLeft == 0) { TicksLeft = 0; }

                    stepPerRefresh = 0;
                    refreshTimer.Restart();
                }

                if (RefreshAction != null) { RefreshAction.Invoke(this); }

                return true;
            }


            public string TimeLeft()
            {
                var timeSpan = new TimeSpan(TicksLeft);
                string content = $"{timeSpan.Hours.ToString().PadLeft(2, '0')}:{timeSpan.Minutes.ToString().PadLeft(2, '0')}:{timeSpan.Seconds.ToString().PadLeft(2, '0')}";
                if (timeSpan.Days > 0) { content = $"{timeSpan.Days} days {content}"; }

                return content;
            }
        }
    }
}
