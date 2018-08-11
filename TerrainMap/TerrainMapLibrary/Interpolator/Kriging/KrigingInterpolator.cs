using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using TerrainMapLibrary.Interpolator.Data;
using TerrainMapLibrary.Mathematics;
using TerrainMapLibrary.Utils;

namespace TerrainMapLibrary.Interpolator.Kriging
{
    public class KrigingInterpolator
    {
        public MapPointList Data { get; private set; }


        public KrigingInterpolator(MapPointList data)
        {
            if (data == null || data.Count < 2)
            { throw new Exception("the count of data must be more than or equal with 2."); }

            Data = data;
        }


        public void GenerateSemivarianceMapIndex(int cacheFileRecord = 67108864, int flushRecord = 8388608,
            StepCounter counter = null)
        {
            var cache = FixedItemFileCache.Generate("0", null, 64, cacheFileRecord, false);

            int flushStep = 0;
            if (counter != null) { counter.Reset((long)(Data.Count - 1) * Data.Count / 2); }

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

                    flushStep = flushStep >= flushRecord ? 0 : flushStep + 1;
                    if (flushStep == 0) { cache.Flush(); }

                    if (counter != null) { counter.AddStep(); }
                }
            }

            if (counter != null) { counter.Reset(counter.StepLength, counter.StepLength); }

            cache.Flush();
            cache.Close();
        }
    }
}
