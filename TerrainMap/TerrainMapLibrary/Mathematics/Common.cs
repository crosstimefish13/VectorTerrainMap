using System;

namespace TerrainMapLibrary.Mathematics
{
    public static class Common
    {
        public static int DoubleCompare(double left, double right)
        {
            if (left == right) { return 0; }
            else if (Math.Abs(left - right) <= double.Epsilon) { return 0; }
            else if (left > right) { return 1; }
            else { return -1; }
        }

        public static int FloatCompare(float left, float right)
        {
            if (left == right) { return 0; }
            else if (Math.Abs(left - right) <= float.Epsilon) { return 0; }
            else if (left > right) { return 1; }
            else { return -1; }
        }

        public static double EuclidDistance(double leftX, double leftY, double rightX, double rightY)
        {
            double diffX = (leftX - rightX);
            double diffProdX = diffX * diffX;
            double diffY = (leftY - rightY);
            double diffProdY = diffY * diffY;
            double distance = Math.Sqrt(diffProdX + diffProdY);
            return distance;
        }

        public static double Semivariance(double value1, double value2)
        {
            double diff = value1 - value2;
            double semivariance = diff * diff * 0.5;
            return semivariance;
        }
    }
}
