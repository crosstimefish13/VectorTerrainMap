using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrainMapLibrary.Arithmetic;

namespace TerrainMapLibrary.Vector.Kriging
{
    public class KrigingInterpolator
    {
        private List<Vector.Data.MapPoint> data;

        public KrigingInterpolator(List<Vector.Data.MapPoint> data)
        {
            this.data = data;
        }

        public KrigingSemivarianceMap GenerateSemivarianceMap(GeoNumber lagBins)
        {
            var map = new KrigingSemivarianceMap();

            var allMap = new List<Vector.Data.MapPoint>();
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = i + 1; j < data.Count; j++)
                {
                    var vector = new Vector.Data.MapPoint();
                    vector.X = GetEuclidDidstance(data[i], data[j]);
                    vector.Y = GetSemivariance(data[i], data[j]);
                    allMap.Add(vector);
                }
            }

            return map;
        }

        private GeoNumber GetEuclidDidstance(Vector.Data.MapPoint v1, Vector.Data.MapPoint v2)
        {
            var vl = (v1.X - v2.X) * (v1.X - v2.X) + (v1.Y - v2.Y) * (v1.Y - v2.Y);
            var distance = vl.Pow(new GeoNumber("0.5"));
            return distance;
        }

        private GeoNumber GetSemivariance(Vector.Data.MapPoint v1, Vector.Data.MapPoint v2)
        {
            var semivariance = (v1.Z - v2.Z) * (v1.Z - v2.Z) * new GeoNumber("0.5");
            return semivariance;
        }
    }
}
