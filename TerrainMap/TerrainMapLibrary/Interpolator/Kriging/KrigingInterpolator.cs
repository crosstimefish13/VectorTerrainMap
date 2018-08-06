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
            private List<FileStream> cachesStream;

            private List<byte> memoryRecords;


            public string CacheRoot { get; private set; }

            public long RecordLength { get; private set; }

            public long MaxFileRecord { get; private set; }

            public long MaxMemoryRecord { get; private set; }

            public long Count { get; private set; }


            public void Add(List<byte> record)
            {
                // validate record and MaxMemoryRecord
                if (record == null || record.Count != RecordLength) { KrigingException.InvalidRecord("record"); }
                if (memoryRecords.Count + record.Count > MaxMemoryRecord * RecordLength)
                { KrigingException.InvalidAdd("record"); }

                // add record into memory
                memoryRecords.AddRange(record);
            }

            public void Flush()
            {
                // return if does not need to flush
                if (memoryRecords.Count == 0) { return; }

                int cacheCount = GetCacheCount(Count, MaxFileRecord);
                if (cacheCount == 0)
                {
                    // create first cache
                    cachesStream.Add(new FileStream(GetCachePath(CacheRoot, 0),
                        FileMode.Create,
                        FileAccess.ReadWrite));
                    cacheCount = 1;
                }

                var cacheStream = cachesStream[cacheCount - 1];

                // the sum of flush count and cache stream length must less or equal than max file length
                int flushCount = memoryRecords.Count;
                if (cacheStream.Length + memoryRecords.Count > MaxFileRecord * RecordLength)
                { flushCount = (int)(MaxFileRecord * RecordLength - cacheStream.Length); }

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
                    cachesStream.Add(new FileStream(GetCachePath(CacheRoot, cacheCount),
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
                }
            }

            public static SemivarianceMapFileCache Generate(string cacheVersion,
                string cacheDirectory = null,
                long recordLength = 100,
                long maxFileRecord = 10000000,
                long maxMemoryRercod = 100000)
            {
                // validate paramaters
                if (string.IsNullOrEmpty(cacheVersion.Trim()))
                { KrigingException.InvalidCacheVersion("cacheVersion"); }
                if (recordLength <= 0) { KrigingException.InvalidRecordLength("recordLength"); }
                if (maxFileRecord <= 0) { KrigingException.InvalidMaxFileRecord("maxFileRecord"); }
                if (maxMemoryRercod <= 0) { KrigingException.InvalidMaxMemoryRercod("maxMemoryRercod"); }

                // delete all files then create cache root directory
                string cacheRoot = GetCacheRoot(cacheVersion, cacheDirectory);
                if (Directory.Exists(cacheRoot) == true) { Directory.Delete(cacheRoot, true); }
                if (Directory.Exists(cacheRoot) == false) { Directory.CreateDirectory(cacheRoot); }

                // generate title file
                var titleStream = new FileStream(GetTitlePath(cacheRoot), FileMode.Create, FileAccess.Write);
                titleStream.Seek(0, SeekOrigin.Begin);

                // 0-7, RecordLength
                var bytes = BitConverter.GetBytes(recordLength);
                titleStream.Write(bytes, 0, bytes.Length);

                // 8-15, MaxFileRecord
                bytes = BitConverter.GetBytes(maxFileRecord);
                titleStream.Write(bytes, 0, bytes.Length);

                // 16-23, MaxMemoryRercod
                bytes = BitConverter.GetBytes(maxMemoryRercod);
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
                cache.RecordLength = BitConverter.ToInt64(bytes, 0);

                // 8-15, MaxFileRecord
                bytes = new byte[8];
                titleStream.Read(bytes, 0, 8);
                cache.MaxFileRecord = BitConverter.ToInt64(bytes, 0);

                // 16-23, MaxMemoryRercod
                bytes = new byte[8];
                titleStream.Read(bytes, 0, 8);
                cache.MaxMemoryRecord = BitConverter.ToInt64(bytes, 0);

                // 24-31, Count
                bytes = new byte[8];
                titleStream.Read(bytes, 0, 8);
                cache.Count = BitConverter.ToInt64(bytes, 0);

                titleStream.Close();
                titleStream.Dispose();

                // load all data files
                int cacheCount = GetCacheCount(cache.Count, cache.MaxFileRecord);
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

            private static int GetCacheCount(long recordCount, long maxFileRecord)
            {
                int cacheCount = (int)(recordCount / maxFileRecord);
                if (cacheCount * maxFileRecord < recordCount) { cacheCount += 1; }

                return cacheCount;
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
