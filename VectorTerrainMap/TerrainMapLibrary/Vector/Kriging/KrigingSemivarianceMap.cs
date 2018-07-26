using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrainMapLibrary.Arithmetic;
using TerrainMapLibrary.Vector.Data;

namespace TerrainMapLibrary.Vector.Kriging
{
    public class KrigingSemivarianceMap
    {
        private List<Vector.Data.MapPoint> vectors;

        public KrigingSemivarianceMap()
        {
            vectors = new List<Vector.Data.MapPoint>();
        }

        public void AddVector(Vector.Data.MapPoint vector)
        {
            vectors.Add(vector);
        }
    }
}
