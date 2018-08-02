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




        public KrigingInterpolator(MapPointList data, string cacheDirectory = null)
        {
            Data = data;
            if (Data == null) { Data = new MapPointList(); }

            CacheDirectory = cacheDirectory;
            if (CacheDirectory == null)
            {
                CacheDirectory = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            }
        }




        public KrigingSemivarianceMap GenerateSemivarianceMap(double lagBins = 0,
            int flushRecordCount = 1000,
            Counter counter = null)
        {
            if (Data.Count < 2) { KrigingException.InvalidData("Data"); }
            if (lagBins < 0) { KrigingException.InvalidLagBins("lagBins"); }

            var map = new KrigingSemivarianceMap();

            FileStream stream = null;
            StreamWriter writer = null;

            try
            {
                var filePath = GetCacheFilePath(lagBins);
                stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                writer = new StreamWriter(stream);
                int flushStep = 0;

                if (counter != null) { counter.Reset((long)(Data.Count - 1) * Data.Count); }

                for (int i = 0; i < Data.Count; i++)
                {
                    for (int j = i + 1; j < Data.Count; j++)
                    {
                        double vectorX = Common.EuclidDistance(Data[i].X, Data[i].Y, Data[j].X, Data[j].Y);
                        double vectorY = Common.Semivariance(Data[i].Z, Data[j].Z);
                        writer.WriteLine($"{vectorX},{vectorY},{Data[i].X},{Data[i].Y},{Data[i].Z},{Data[j].X},{Data[j].Y},{Data[j].Z}");

                        flushStep = flushStep >= flushRecordCount ? 0 : flushStep + 1;
                        if (flushStep == 0) { writer.Flush(); }

                        if (counter != null) { counter.AddStep(); }
                    }
                }

                if (counter != null) { counter.Reset(); }

                writer.Flush();
                writer.Close();
            }
            catch (Exception inner) { throw inner; }
            finally
            {
                if (writer != null) { writer.Dispose(); }
                if (stream != null) { stream.Dispose(); }
            }

            return map;
        }


        public class SemivarianceMapFileCache
        {
            private Dictionary<long, FileStream> cachesStream;

            private List<byte> memoryRecords;


            public string CacheRoot { get; private set; }

            public long RecordLength { get; private set; }

            public long MaxRecordPerFile { get; private set; }

            public long MaxMemoryRecord { get; private set; }

            public long Count { get; private set; }


            public void Add(List<byte> record)
            {
                if (record == null || record.Count != RecordLength) { KrigingException.InvalidRecord("record"); }

                // add record into memory
                memoryRecords.AddRange(record);

                // get cache stream by using cache number
                int cacheNumber = GetCacheCount(Count, MaxRecordPerFile) - 1;
                var cacheStream = cachesStream[cacheNumber];
                if (cacheStream.Length / RecordLength == MaxRecordPerFile)
                {
                    // cache stream length is max length, need to create a new cache file
                    cachesStream.Add(cacheNumber + 1, new FileStream(GetCachePath(CacheRoot, 0),
                        FileMode.Open,
                        FileAccess.ReadWrite));
                    cacheStream = cachesStream.Last().Value;
                }

                // write all memory records into cache file if needed
                if (memoryRecords.Count / RecordLength == MaxMemoryRecord
                    || (cacheStream.Length + memoryRecords.Count) / RecordLength == MaxRecordPerFile)
                {
                    cacheStream.Write(memoryRecords.ToArray(), 0, memoryRecords.Count);
                    cacheStream.Flush();
                    memoryRecords.Clear();
                }
            }

            public static SemivarianceMapFileCache Generate(string cacheVersion,
                string cacheDirectory = null,
                long recordLength = 100,
                long maxRecordPerFile = 10000000,
                long maxMemoryRercod = 100000)
            {
                // validate paramaters
                if (string.IsNullOrEmpty(cacheVersion.Trim()))
                { KrigingException.InvalidCacheVersion("cacheVersion"); }
                if (recordLength <= 0) { KrigingException.InvalidRecordLength("recordLength"); }
                if (maxRecordPerFile <= 0) { KrigingException.InvalidMaxRecordPerFile("maxRecordPerFile"); }
                if (maxMemoryRercod <= 0 || maxMemoryRercod > maxRecordPerFile)
                { KrigingException.InvalidMaxMemoryRercod("maxMemoryRercod"); }

                // delete all files then create cache root directory
                string cacheRoot = GetCacheRoot(cacheVersion, cacheDirectory);
                if (Directory.Exists(cacheRoot) == true) { Directory.Delete(cacheRoot, true); }
                if (Directory.Exists(cacheRoot) == false) { Directory.CreateDirectory(cacheRoot); }

                // generate title file
                var titleStream = new FileStream(GetTitlePath(cacheRoot), FileMode.Create, FileAccess.Write);
                titleStream.Seek(0, SeekOrigin.Begin);

                // 0-8, RecordLength
                var bytes = BitConverter.GetBytes(recordLength);
                titleStream.Write(bytes, 0, bytes.Length);

                // 9-16, MaxRecordPerFile
                bytes = BitConverter.GetBytes(maxRecordPerFile);
                titleStream.Write(bytes, 0, bytes.Length);

                // 17-24, AutoFlushRecord
                bytes = BitConverter.GetBytes(maxMemoryRercod);
                titleStream.Write(bytes, 0, bytes.Length);

                // 25-32, Count
                bytes = BitConverter.GetBytes(0L);
                titleStream.Write(bytes, 0, bytes.Length);

                titleStream.Flush();
                titleStream.Close();
                titleStream.Dispose();

                cachesStream.Add(0, new FileStream(GetCachePath(CacheRoot, 0),
                        FileMode.Open,
                        FileAccess.ReadWrite));

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

                // 0-8, RecordLength
                var bytes = new byte[8];
                titleStream.Read(bytes, 0, 8);
                cache.RecordLength = BitConverter.ToInt64(bytes, 0);

                // 9-16, MaxRecordPerFile
                bytes = new byte[8];
                titleStream.Read(bytes, 0, 8);
                cache.MaxRecordPerFile = BitConverter.ToInt64(bytes, 0);

                // 17-24, AutoFlushRecord
                bytes = new byte[8];
                titleStream.Read(bytes, 0, 8);
                cache.MaxMemoryRecord = BitConverter.ToInt64(bytes, 0);

                // 25-32, Count
                bytes = new byte[8];
                titleStream.Read(bytes, 0, 8);
                cache.Count = BitConverter.ToInt64(bytes, 0);

                titleStream.Close();
                titleStream.Dispose();

                // load all data files
                int cacheCount = GetCacheCount(cache.Count, cache.MaxRecordPerFile);
                for (int cacheNumber = 0; cacheNumber < cacheCount; cacheNumber++)
                {
                    var cacheStream = new FileStream(GetCachePath(cache.CacheRoot, cacheNumber),
                        FileMode.Open,
                        FileAccess.ReadWrite);

                    cache.cachesStream.Add(cacheNumber, cacheStream);
                }

                return cache;
            }


            private SemivarianceMapFileCache()
            {
                cachesStream = new Dictionary<long, FileStream>();
                memoryRecords = new List<string>();
                CacheRoot = string.Empty;
                RecordLength = 0;
                MaxRecordPerFile = 0;
                MaxMemoryRecord = 0;
                Count = 0;
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

            private static string GetCachePath(string cacheRoot, long cacheNumber)
            {
                string filePath = Path.Combine(cacheRoot, $"{cacheNumber.ToString().PadLeft(8, '0')}.data");
                return filePath;
            }

            private static int GetCacheCount(long recordCount, long maxRecordPerFile)
            {
                int cacheCount = (int)(recordCount / maxRecordPerFile);
                if (cacheCount * maxRecordPerFile < recordCount) { cacheCount += 1; }

                return cacheCount;
            }

            private static long GetCacheRecordCount(long recordLength, long cacheLength)
            {
                long count = cacheLength / recordLength;
                return count;
            }


            public bool AddRecord(string record)
            {
                if (cachesStream != null && cachesStream.Length >= MaxRecordPerFile)
                {
                    AddGlobalIndex(Count - 1, Convert.ToInt64(Path.GetFileName(cachesStream.Name)));

                    try
                    {
                        cachesStream.Flush();
                        cachesStream.Close();
                        indexesStream.Flush();
                        indexesStream.Close();
                    }
                    catch (Exception inner) { throw inner; }
                    finally
                    {
                        if (cachesStream != null) { cachesStream.Dispose(); }
                        if (indexesStream != null) { indexesStream.Dispose(); }

                        cachesStream = null;
                        indexesStream = null;
                    }
                }

                if (cachesStream == null)
                {
                    long fileNumber = 0;
                    if (globalIndexes.Count > 0) { fileNumber = globalIndexes.Last().Value + 1; }

                    string cacheFilePath = Path.Combine(CacheRoot, CacheVersion, $"{fileNumber}");
                    string cacheFileIndexPath = Path.Combine(CacheRoot, CacheVersion, $"{indexName}{fileNumber}");

                    try
                    {
                        cachesStream = new FileStream(cacheFilePath, FileMode.Create, FileAccess.Write);
                        indexesStream = new FileStream(cacheFileIndexPath, FileMode.Create, FileAccess.Write);
                    }
                    catch (Exception inner) { throw inner; }
                    finally
                    {
                        if (cachesStream != null) { cachesStream.Dispose(); }
                        if (indexesStream != null) { indexesStream.Dispose(); }

                        cachesStream = null;
                        indexesStream = null;
                    }
                }

                if (cachesStream == null) { return false; }
            }


            //private SemivarianceMapFileCache(string version, string cacheDirectory)
            //{
            //    indexName = "index";
            //    globalIndexes = new Dictionary<long, long>();
            //    contentsStream = new Dictionary<long, FileStream>();
            //    indexesStream = new Dictionary<long, FileStream>();
            //    MaxRecordPerFile = 0;
            //    AutoFlushRecord = true;
            //    CacheRoot = cacheDirectory;
            //    if (CacheRoot == null)
            //    {
            //        var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            //        CacheRoot = Path.Combine(Path.GetDirectoryName(assemblyLocation),
            //            Path.GetFileNameWithoutExtension(assemblyLocation));
            //    }

            //    CacheVersion = version;
            //    RecordCount = 0;
            //}

            private void LoadIntoMemory()
            {
                var topIndexPath = Path.Combine(CacheRoot, CacheVersion, indexName);
                topIndexStream = new FileStream(topIndexPath, FileMode.Open, FileAccess.ReadWrite);
                topIndexStream.Seek(0, SeekOrigin.Begin);

                var bytes = new byte[8];
                topIndexStream.Read(bytes, 0, 8);
                MaxRecordPerFile = BitConverter.ToInt64(bytes, 0);

                bytes = new byte[8];
                topIndexStream.Read(bytes, 0, 8);
                MaxMemoryRecord = BitConverter.ToInt64(bytes, 0) == 1L ? true : false;

                bytes = new byte[8];
                topIndexStream.Read(bytes, 0, 8);
                Count = BitConverter.ToInt64(bytes, 0);

                while (topIndexStream.Position < topIndexStream.Length)
                {
                    topIndexStream.Seek(8, SeekOrigin.Current);
                    bytes = new byte[8];
                    topIndexStream.Read(bytes, 0, 8);
                    long fileNumber = BitConverter.ToInt64(bytes, 0);

                    string contentPath = Path.Combine(CacheRoot, CacheVersion, $"{fileNumber}");
                    string indexPath = Path.Combine(CacheRoot, CacheVersion, $"{indexName}{fileNumber}");

                    var contentStream = new FileStream(contentPath, FileMode.Open, FileAccess.ReadWrite);
                    var indexStream = new FileStream(indexPath, FileMode.Open, FileAccess.ReadWrite);

                    cachesStream.Add(fileNumber, contentStream);
                    indexesStream.Add(fileNumber, indexStream);
                }
            }

            private void AddGlobalIndex(long endIndex, long fileNumber)
            {
                var indexFilePath = Path.Combine(CacheRoot, CacheVersion, indexName);
                FileStream stream = new FileStream(indexFilePath, FileMode.Create, FileAccess.Write);
                stream.Seek(0, SeekOrigin.End);

                var bytes = BitConverter.GetBytes(endIndex);
                stream.Write(bytes, 0, bytes.Length);

                bytes = BitConverter.GetBytes(fileNumber);
                stream.Write(bytes, 0, bytes.Length);

                stream.Flush();
                stream.Close();
                stream.Dispose();

                globalIndexes.Add(endIndex, fileNumber);
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
