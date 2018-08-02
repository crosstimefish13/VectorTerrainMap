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


        public class SemivarianceMapCache
        {
            private string indexFileName;

            private FileStream topIndexStream;

            private Dictionary<long, FileStream> contentsStream;

            private Dictionary<long, FileStream> indexesStream;


            public long MaxCacheFileLength { get; private set; }

            public bool AutoFlush { get; private set; }

            public long Count { get; private set; }

            public string CacheDirectory { get; private set; }

            public string Version { get; private set; }



            public static SemivarianceMapCache Initialization(string version,
                bool autoFlush = true,
                long maxCacheFileLength = 1048576,
                string cacheDirectory = null)
            {
                var cache = new SemivarianceMapCache(version, cacheDirectory);

                var directory = Path.Combine(cache.CacheDirectory, version);
                if (Directory.Exists(directory) == true) { Directory.Delete(directory, true); }
                if (Directory.Exists(directory) == false) { Directory.CreateDirectory(directory); }

                var topIndexPath = Path.Combine(directory, cache.indexFileName);
                var stream = new FileStream(topIndexPath, FileMode.Create, FileAccess.Write);
                stream.Seek(0, SeekOrigin.Begin);

                var bytes = BitConverter.GetBytes(maxCacheFileLength);
                stream.Write(bytes, 0, bytes.Length);

                bytes = BitConverter.GetBytes(autoFlush == true ? 1L : 0L);
                stream.Write(bytes, 0, bytes.Length);

                bytes = BitConverter.GetBytes(0L);
                stream.Write(bytes, 0, bytes.Length);

                stream.Flush();
                stream.Close();
                stream.Dispose();

                cache.LoadIntoMemory();
                return cache;
            }

            public static SemivarianceMapCache Load(string version, string cacheDirectory = null)
            {
                var cache = new SemivarianceMapCache(version, cacheDirectory);
                cache.LoadIntoMemory();
                return cache;
            }


            public bool AddRecord(string record)
            {
                if (contentsStream != null && contentsStream.Length >= MaxCacheFileLength)
                {
                    AddGlobalIndex(Count - 1, Convert.ToInt64(Path.GetFileName(contentsStream.Name)));

                    try
                    {
                        contentsStream.Flush();
                        contentsStream.Close();
                        indexesStream.Flush();
                        indexesStream.Close();
                    }
                    catch (Exception inner) { throw inner; }
                    finally
                    {
                        if (contentsStream != null) { contentsStream.Dispose(); }
                        if (indexesStream != null) { indexesStream.Dispose(); }

                        contentsStream = null;
                        indexesStream = null;
                    }
                }

                if (contentsStream == null)
                {
                    long fileNumber = 0;
                    if (globalIndexes.Count > 0) { fileNumber = globalIndexes.Last().Value + 1; }

                    string cacheFilePath = Path.Combine(CacheDirectory, Version, $"{fileNumber}");
                    string cacheFileIndexPath = Path.Combine(CacheDirectory, Version, $"{indexFileName}{fileNumber}");

                    try
                    {
                        contentsStream = new FileStream(cacheFilePath, FileMode.Create, FileAccess.Write);
                        indexesStream = new FileStream(cacheFileIndexPath, FileMode.Create, FileAccess.Write);
                    }
                    catch (Exception inner) { throw inner; }
                    finally
                    {
                        if (contentsStream != null) { contentsStream.Dispose(); }
                        if (indexesStream != null) { indexesStream.Dispose(); }

                        contentsStream = null;
                        indexesStream = null;
                    }
                }

                if (contentsStream == null) { return false; }
            }


            private SemivarianceMapCache(string version, string cacheDirectory)
            {
                indexFileName = "index";
                globalIndexes = new Dictionary<long, long>();
                contentsStream = new Dictionary<long, FileStream>();
                indexesStream = new Dictionary<long, FileStream>();
                MaxCacheFileLength = 0;
                AutoFlush = true;
                CacheDirectory = cacheDirectory;
                if (CacheDirectory == null)
                {
                    var assemblyLocation = Assembly.GetExecutingAssembly().Location;
                    CacheDirectory = Path.Combine(Path.GetDirectoryName(assemblyLocation),
                        Path.GetFileNameWithoutExtension(assemblyLocation));
                }

                Version = version;
                Count = 0;
            }

            private void LoadIntoMemory()
            {
                var topIndexPath = Path.Combine(CacheDirectory, Version, indexFileName);
                topIndexStream = new FileStream(topIndexPath, FileMode.Open, FileAccess.ReadWrite);
                topIndexStream.Seek(0, SeekOrigin.Begin);

                var bytes = new byte[8];
                topIndexStream.Read(bytes, 0, 8);
                MaxCacheFileLength = BitConverter.ToInt64(bytes, 0);

                bytes = new byte[8];
                topIndexStream.Read(bytes, 0, 8);
                AutoFlush = BitConverter.ToInt64(bytes, 0) == 1L ? true : false;

                bytes = new byte[8];
                topIndexStream.Read(bytes, 0, 8);
                Count = BitConverter.ToInt64(bytes, 0);

                while (topIndexStream.Position < topIndexStream.Length)
                {
                    topIndexStream.Seek(8, SeekOrigin.Current);
                    bytes = new byte[8];
                    topIndexStream.Read(bytes, 0, 8);
                    long fileNumber = BitConverter.ToInt64(bytes, 0);

                    string contentPath = Path.Combine(CacheDirectory, Version, $"{fileNumber}");
                    string indexPath = Path.Combine(CacheDirectory, Version, $"{indexFileName}{fileNumber}");

                    var contentStream = new FileStream(contentPath, FileMode.Open, FileAccess.ReadWrite);
                    var indexStream = new FileStream(indexPath, FileMode.Open, FileAccess.ReadWrite);

                    contentsStream.Add(fileNumber, contentStream);
                    indexesStream.Add(fileNumber, indexStream);
                }
            }

            private void AddGlobalIndex(long endIndex, long fileNumber)
            {
                var indexFilePath = Path.Combine(CacheDirectory, Version, indexFileName);
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
                    // return if step over the step length
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

                long refreshTicks = RefreshInterval * TimeSpan.TicksPerMillisecond;
                if (refreshTimer.ElapsedTicks >= refreshTicks)
                {
                    // need to update the ticks left
                    refreshTimer.Stop();

                    long newTicksLeft = (StepLength - Step) / stepPerRefresh * refreshTicks;

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
