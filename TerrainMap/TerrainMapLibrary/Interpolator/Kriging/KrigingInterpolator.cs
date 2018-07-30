using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrainMapLibrary.Interpolator.Data;
using TerrainMapLibrary.Mathematics;

namespace TerrainMapLibrary.Interpolator.Kriging
{
    public class KrigingInterpolator
    {
        public List<MapPoint> Data { get; }


        public KrigingInterpolator(List<MapPoint> data = null)
        {
            Data = data;
            if (Data == null) { Data = new List<MapPoint>(); }
        }


        public KrigingSemivarianceMap GenerateSemivarianceMap(double lagBins = 0, int parallel = 1)
        {
            if (Data.Count < 2) { KrigingException.InvalidData("Data"); }
            if (lagBins < 0) { KrigingException.InvalidLagBins("lagBins"); }
            if (parallel < 1) { KrigingException.InvalidParallel("parallel"); }

            var map = new KrigingSemivarianceMap();

            int taskCount = (Data.Count - 1) * Data.Count / 2 / parallel + 1;
            int computeCount = 0;
            var tasks = new List<Task>();


            var allMap = new List<MapPoint>();
            for (int i = 0; i < Data.Count; i++)
            {
                for (int j = i + 1; j < Data.Count; j++)
                {
                    if (computeCount == 0) { tasks = new List<Task>(); }

                    double vectorX = Common.EuclidDistance(Data[i].X, Data[i].Y, Data[j].X, Data[j].Y);
                    double vectorY = Common.Semivariance(Data[i].Z, Data[j].Z);
                    allMap.Add(new MapPoint(vectorX, vectorY));
                }
            }

            return map;
        }
    }
}
