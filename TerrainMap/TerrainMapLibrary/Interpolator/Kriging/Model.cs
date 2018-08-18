using System;

namespace TerrainMapLibrary.Interpolator.Kriging
{
    public abstract class Model
    {
        public double MinX { get; set; }

        public double MinY { get; set; }

        public double MaxX { get; set; }

        public double MaxY { get; set; }


        public Model(double minX = 0, double minY = 0, double maxX = 1, double maxY = 1)
        {
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
        }


        public override bool Equals(object obj)
        {
            throw new NotSupportedException();
        }

        public override int GetHashCode()
        {
            return MinX.GetHashCode() + MinY.GetHashCode() + MaxX.GetHashCode() + MaxY.GetHashCode();
        }

        public override string ToString()
        {
            return $"MinX:{MinX}, MinY:{MinY}, MaxX:{MaxX}, MaxY:{MaxY}";
        }


        public abstract double Map(double x);
    }
}
