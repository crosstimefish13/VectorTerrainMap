using TerrainMapLibrary.Arithmetic;

namespace TerrainMapLibrary.Data
{
    public class Vector
    {
        public GeoNumber X { get; set; }

        public GeoNumber Y { get; set; }

        public GeoNumber Z { get; set; }

        public Vector()
            : this(new GeoNumber("0"), new GeoNumber("0"), new GeoNumber("0"))
        { }

        public Vector(GeoNumber x, GeoNumber y)
            : this(x, y, new GeoNumber("0"))
        { }

        public Vector(GeoNumber x, GeoNumber y, GeoNumber z)
        {
            X = x.Copy();
            Y = y.Copy();
            Z = z.Copy();
        }
    }
}
