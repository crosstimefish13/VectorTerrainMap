using TerrainMapLibrary.Mathematics;

namespace TerrainMapLibrary.Interpolator.Data
{
    public class MapPoint
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }


        public MapPoint(double x = 0, double y = 0, double z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }


        public static bool operator ==(MapPoint left, MapPoint right)
        {
            // compare object reference
            if (left is null && right is null) { return true; }
            else if (left is null || right is null) { return false; }
            // compare xyz
            else if (Common.DoubleEqual(left.X, right.X) == false) { return false; }
            else if (Common.DoubleEqual(left.Y, right.Y) == false) { return false; }
            else if (Common.DoubleEqual(left.Z, right.Z) == false) { return false; }
            else { return true; }
        }

        public static bool operator !=(MapPoint left, MapPoint right)
        {
            return !(left == right);
        }


        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is MapPoint)) { return false; }

            return this == (obj as MapPoint);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();
        }

        public override string ToString()
        {
            return $"X:{X}, Y:{Y}, Z:{Z}";
        }
    }
}
