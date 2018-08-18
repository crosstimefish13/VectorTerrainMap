using System;
using TerrainMapLibrary.Mathematics;

namespace TerrainMapLibrary.Interpolator.Kriging
{
    public class ExponentialModel : Model
    {
        public ExponentialModel(double minX = 0, double minY = 0, double maxX = 1, double maxY = 1)
            : base(minX, minY, maxX, maxY)
        { }


        public override double Map(double x)
        {
            double fx = x - MinX;
            if (Common.DoubleCompare(fx, 0) <= 0) { return MinY; }

            double y = MinY + MaxY * (1 - Math.Exp((-fx) / MaxX));
            return y;
        }
    }
}
