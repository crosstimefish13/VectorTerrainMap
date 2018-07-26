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
        private List<Data.Vector> data;

        public KrigingInterpolator(List<Data.Vector> data)
        {
            this.data = data;
        }

        public KrigingSemivarianceMap GenerateSemivarianceMap(GeoNumber lagBins)
        {
            var map = new KrigingSemivarianceMap();

            var allMap = new List<Data.Vector>();
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = i + 1; j < data.Count; j++)
                {
                    var vector = new Data.Vector();
                    vector.X = GetEuclidDidstance(data[i], data[j]);
                    vector.Y = GetSemivariance(data[i], data[j]);
                    allMap.Add(vector);
                }
            }

            return map;
        }

        private GeoNumber GetEuclidDidstance(Data.Vector v1, Data.Vector v2)
        {
            var vl = (v1.X - v2.X) * (v1.X - v2.X) + (v1.Y - v2.Y) * (v1.Y - v2.Y);
            var distance = vl.Pow(new GeoNumber("0.5"));
            return distance;
        }

        private GeoNumber GetSemivariance(Data.Vector v1, Data.Vector v2)
        {
            var semivariance = (v1.Z - v2.Z) * (v1.Z - v2.Z) * new GeoNumber("0.5");
            return semivariance;
        }
    }
}
