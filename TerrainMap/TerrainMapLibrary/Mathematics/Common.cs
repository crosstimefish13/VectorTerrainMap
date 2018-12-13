using System;
using System.Collections.Generic;
using System.Linq;

namespace TerrainMapLibrary.Mathematics
{
    public static class Common
    {
        public static int DoubleCompare(double left, double right)
        {
            if (left == right)
            {
                return 0;
            }
            else if (Math.Abs(left - right) <= double.Epsilon)
            {
                return 0;
            }
            else if (left > right)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        public static int FloatCompare(float left, float right)
        {
            if (left == right)
            {
                return 0;
            }
            else if (Math.Abs(left - right) <= float.Epsilon)
            {
                return 0;
            }
            else if (left > right)
            {
                return 1;
            }
            else
            {
                return -1;
            }
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

        public static string ToNumberString(double value, int decimalDigits = 0, bool fillZero = false)
        {
            string result = value.ToString();
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                return result;
            }

            int dotIndex = result.IndexOf('.');
            var leftParts = new List<char>();
            var rightParts = new List<char>();
            if (dotIndex < 0)
            {
                leftParts = result.ToCharArray().ToList();
            }
            else
            {
                var parts = result.Split('.');
                leftParts = parts.First().ToCharArray().ToList();
                rightParts = parts.Last().ToCharArray().ToList();
            }

            if (rightParts.Count > decimalDigits)
            {
                rightParts = rightParts.Take(decimalDigits).ToList();
            }
            if (fillZero == true)
            {
                while (rightParts.Count < decimalDigits)
                {
                    rightParts.Add('0');
                }
            }

            result = string.Join(string.Empty, leftParts.ToArray());
            if (rightParts.Count > 0)
            {
                result = $"{result}.{string.Join(string.Empty, rightParts.ToArray())}";
            }

            return result;
        }
    }
}
