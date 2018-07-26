using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrainMapLibrary.Arithmetic;
using TerrainMapLibrary.Data;

namespace TerrainMapLibrary.Vector.Kriging
{
    public class KrigingSemivarianceMap
    {
        private List<Data.Vector> vectors;

        public KrigingSemivarianceMap()
        {
            vectors = new List<Data.Vector>();
        }

        public void AddVector(Data.Vector vector)
        {
            vectors.Add(vector);
        }
    }
}
