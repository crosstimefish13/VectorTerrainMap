using System;

namespace TerrainMapLibrary.Mathematics
{
    public static class Common
    {
        public static bool DoubleEqual(double left, double right)
        {
            if (left == right) { return true; }
            else if (Math.Abs(left - right) <= double.Epsilon) { return true; }
            else { return false; }
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
