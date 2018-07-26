using System.Collections.Generic;
using TerrainMapLibrary.Arithmetic;

namespace TerrainMapLibrary.Data
{
    public class VectorCSVData : CSVData
    {
        public VectorCSVData(string filePath)
            : base(filePath)
        { }

        public List<Vector> GetValidVectors()
        {
            var fields = Fields();
            var vectors = new List<Vector>();
            foreach (var row in fields)
            {
                if (row.Count < 3) { continue; }

                try
                {
                    var vector = new Vector();
                    vector.X = new GeoNumber(row[0]);
                    vector.Y = new GeoNumber(row[1]);
                    vector.Z = new GeoNumber(row[2]);
                    vectors.Add(vector);
                }
                catch { continue; }
            }

            return vectors;
        }
    }
}
