using TerrainMapLibrary.Arithmetic;

namespace TerrainMapLibrary.Vector.Data
{
    public class MapPoint
    {
        public GeoNumber X { get; set; }

        public GeoNumber Y { get; set; }

        public GeoNumber Z { get; set; }

        public MapPoint()
            : this(new GeoNumber("0"), new GeoNumber("0"), new GeoNumber("0"))
        { }

        public MapPoint(GeoNumber x, GeoNumber y)
            : this(x, y, new GeoNumber("0"))
        { }

        public MapPoint(GeoNumber x, GeoNumber y, GeoNumber z)
        {
            X = x.Copy();
            Y = y.Copy();
            Z = z.Copy();
        }
    }
}
