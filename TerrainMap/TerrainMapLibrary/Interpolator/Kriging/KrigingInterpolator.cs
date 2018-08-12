using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using TerrainMapLibrary.Interpolator.Data;
using TerrainMapLibrary.Mathematics;
using TerrainMapLibrary.Utils;
using TerrainMapLibrary.Utils.Sequence;

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
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            string cacheRoot = Path.Combine(Path.GetDirectoryName(assemblyLocation),
                Path.GetFileNameWithoutExtension(assemblyLocation));

            var cache = ListFileSequence.Generate(cacheRoot, 64, cacheFileRecord, flushRecord);

            if (counter != null) { counter.Reset((long)(Data.Count - 1) * Data.Count / 2); }

            for (int i = 0; i < Data.Count; i++)
            {
                var left = Data[i];
                for (int j = i + 1; j < Data.Count; j++)
                {
                    var right = Data[j];
                    double vectorX = Common.EuclidDistance(Data[i].X, Data[i].Y, Data[j].X, Data[j].Y);
                    double vectorY = Common.Semivariance(Data[i].Z, Data[j].Z);

                    var bytes = BitConverter.GetBytes(vectorX)
                        .Concat(BitConverter.GetBytes(vectorY))
                        .Concat(BitConverter.GetBytes(left.X))
                        .Concat(BitConverter.GetBytes(left.Y))
                        .Concat(BitConverter.GetBytes(left.Z))
                        .Concat(BitConverter.GetBytes(right.X))
                        .Concat(BitConverter.GetBytes(right.Y))
                        .Concat(BitConverter.GetBytes(right.Z))
                        .ToArray();
                    cache.Add(bytes);

                    if (counter != null) { counter.AddStep(); }
                }
            }

            if (counter != null) { counter.Reset(counter.StepLength, counter.StepLength); }

            cache.Close();
        }
    }
}
