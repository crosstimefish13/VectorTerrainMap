using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TerrainMapLibrary.Localization;

namespace TerrainMapLibrary.Data
{
    public class GeoNumber
    {
        private bool sign;
        private GeoUNumber number;
        private int point;

        private static GeoNumber e;
        private static int precision = 100;

        public static GeoNumber E
        {
            get
            {
                if (e == null)
                {
                    e = GetE(precision + 3);
                }

                return e;
            }
        }

        public static int Precision
        {
            get
            {
                return precision;
            }
            set
            {
                // ensure precision is valid
                if (value < 0)
                {
                    throw new Exception(ExceptionMessage.InvalidPrecision);
                }

                // need to reset constant
                if (precision != value)
                {
                    e = null;
                }

                precision = value;
            }
        }


        public GeoNumber(string value)
        {
            // ensure value is valid
            if (string.IsNullOrEmpty(value))
            {
                throw new Exception(ExceptionMessage.NullValue);
            }

            // ensure value is a valid number
            Regex regex = new Regex("^[+-]?\\d+(\\.\\d+)?$");
            if (!regex.IsMatch(value))
            {
                throw new Exception($"{value} {ExceptionMessage.NotNumber}");
            }

            // get sign, integer part and decimal part
            sign = true;
            point = 0;
            var digits = new List<byte>();

            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '.') { point = value.Length - i - 1; }
                else if (value[i] == '+') { sign = true; }
                else if (value[i] == '-') { sign = false; }
                else if (point == 0 || value.Length - i > point - precision)
                {
                    digits.Insert(0, (byte)(value[i] - 48));
                }
                else { break; }
            }

            if (point > precision) { point = precision; }
            number = new GeoUNumber(digits);
            Trim();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (sign == false) { sb.Append("-"); }
            sb.Append(number.ToString());
            if (point > 0) { sb.Insert(sb.Length - point, "."); }
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is GeoNumber)) { return false; }
            var number = obj as GeoNumber;
            return ToString() == number.ToString();
        }

        public override int GetHashCode()
        {
            return sign.GetHashCode() + number.GetHashCode() + point.GetHashCode();
        }

        public GeoNumber Copy()
        {
            var dup = new GeoNumber(sign, number.Copy(), point);
            return dup;
        }

        public static GeoNumber operator +(GeoNumber right)
        {
            var resNumber = right.Copy();
            resNumber.sign = true;
            return resNumber;
        }

        public static GeoNumber operator -(GeoNumber right)
        {
            var resNumber = right.Copy();
            if (!resNumber.number.IsZero())
            {
                if (right.sign == false) { resNumber.sign = true; }
                else { resNumber.sign = false; }
            }
            return resNumber;
        }

        public static GeoNumber operator +(GeoNumber left, GeoNumber right)
        {
            // duplicate left and right
            var dupL = left.Copy();
            var dupR = right.Copy();
            int resPoint = left.point;

            // align left and right
            if (dupL.point > dupR.point)
            {
                resPoint = dupL.point;
                dupR.number.Enlarge10(dupL.point - dupR.point);
            }
            else if (dupL.point < dupR.point)
            {
                resPoint = dupR.point;
                dupL.number.Enlarge10(dupR.point - dupL.point);
            }

            bool resSign = dupL.sign;
            GeoUNumber resNumber = null;

            if (dupL.sign == dupR.sign)
            {
                // same sign, unsign add
                dupL.number.Add(dupR.number);
                resSign = dupL.sign;
                resNumber = dupL.number;
            }
            else
            {
                // different sign, unsign compare left and right
                var compare = dupL.number.Compare(dupR.number);
                if (compare == CompareResult.Big)
                {
                    // left is bigger, left sub right, sign comes from left
                    dupL.number.Sub(dupR.number);
                    resSign = dupL.sign;
                    resNumber = dupL.number;
                }
                else if (compare == CompareResult.Small)
                {
                    // right is bigger, right sub left, sign comes from right
                    dupR.number.Sub(dupL.number);
                    resSign = dupR.sign;
                    resNumber = dupR.number;
                }
                else
                {
                    // left is same with right, result is 0
                    resSign = true;
                    resNumber = new GeoUNumber(new List<byte>() { 0 });
                }
            }

            // trim result
            var res = new GeoNumber(resSign, resNumber, resPoint);
            res.Trim();
            return res;
        }

        public static GeoNumber operator -(GeoNumber left, GeoNumber right)
        {
            // duplicate left and right
            var dupL = left.Copy();
            var dupR = right.Copy();
            int resPoint = left.point;

            // align left and right
            if (dupL.point > dupR.point)
            {
                resPoint = dupL.point;
                dupR.number.Enlarge10(dupL.point - dupR.point);
            }
            else if (dupL.point < dupR.point)
            {
                resPoint = dupR.point;
                dupL.number.Enlarge10(dupR.point - dupL.point);
            }

            bool resSign = dupL.sign;
            GeoUNumber resNumber = null;

            if (dupL.sign != dupR.sign)
            {
                // different sign, unsign add
                dupL.number.Add(dupR.number);
                resSign = dupL.sign;
                resNumber = dupL.number;
            }
            else
            {
                // same sign, unsign compare left and right
                var compare = dupL.number.Compare(dupR.number);
                if (compare == CompareResult.Big)
                {
                    // left is bigger, left sub right, sign comes from left
                    dupL.number.Sub(dupR.number);
                    resSign = dupL.sign;
                    resNumber = dupL.number;
                }
                else if (compare == CompareResult.Small)
                {
                    // right is bigger, right sub left, sign is different with left
                    dupR.number.Sub(dupL.number);
                    resSign = !dupL.sign;
                    resNumber = dupR.number;
                }
                else
                {
                    // left is same with right, result is 0
                    resSign = true;
                    resNumber = new GeoUNumber(new List<byte>() { 0 });
                }
            }

            // trim result
            var res = new GeoNumber(resSign, resNumber, resPoint);
            res.Trim();
            return res;
        }

        public static GeoNumber operator *(GeoNumber left, GeoNumber right)
        {
            // duplicate left and right
            var dupL = left.Copy();
            var dupR = right.Copy();

            bool resSign = left.sign == right.sign;
            int resPoint = left.point + right.point;

            dupL.number.Mul(right.number);
            var resNumber = dupL.number;

            // trim result
            var res = new GeoNumber(resSign, resNumber, resPoint);
            res.Trim();
            return res;
        }

        public static GeoNumber operator /(GeoNumber left, GeoNumber right)
        {
            if (right.number.IsZero()) { throw new Exception(ExceptionMessage.NotZeroDivisor); }

            // duplicate left and right
            var dupL = left.Copy();
            var dupR = right.Copy();
            bool resSign = left.sign == right.sign;

            // align left and right
            if (dupL.point > dupR.point)
            {
                dupR.number.Enlarge10(dupL.point - dupR.point);
            }
            else if (dupL.point < dupR.point)
            {
                dupL.number.Enlarge10(dupR.point - dupL.point);
            }

            // mod for integer part, and division for decimal part
            var mod = dupL.number.Mod(dupR.number);
            if (!mod.IsZero()) { mod.Div(dupR.number, precision); }

            // combine integer and decimal parts
            int resPoint = mod.Length;
            var resNumber = dupL.number;
            resNumber.Enlarge10(resPoint);
            resNumber.Add(mod);

            // trim result
            var res = new GeoNumber(resSign, resNumber, resPoint);
            res.Trim();
            return res;
        }

        public static GeoNumber operator %(GeoNumber left, GeoNumber right)
        {
            // ensure left and right both are positive integer
            if (left.sign == false || left.point > 0)
            {
                throw new Exception($"{left.ToString()} {ExceptionMessage.NotPositiveInteger}");
            }

            if (right.sign == false || right.point > 0)
            {
                throw new Exception($"{right.ToString()} {ExceptionMessage.NotPositiveInteger}");
            }

            // ensure right is not 0
            if (right.number.IsZero()) { throw new Exception(ExceptionMessage.NotZeroDivisor); }

            // duplicate left and right
            var dupL = left.Copy();
            var dupR = right.Copy();
            var mod = dupL.number.Mod(dupR.number);

            var res = new GeoNumber(true, mod, 0);
            res.Trim();
            return res;
        }

        public static bool operator ==(GeoNumber left, GeoNumber right)
        {
            var isLeftNull = ReferenceEquals(left, null);
            var isRightNull = ReferenceEquals(right, null);

            if (isLeftNull && isRightNull) { return true; }
            else if (isLeftNull || isRightNull) { return false; }
            else
            {
                var res = left - right;
                return res.number.IsZero();
            }
        }

        public static bool operator !=(GeoNumber left, GeoNumber right)
        {
            var res = left - right;
            return !res.number.IsZero();
        }

        public static bool operator >(GeoNumber left, GeoNumber right)
        {
            var res = left - right;
            return res.sign == true && !res.number.IsZero();
        }

        public static bool operator <(GeoNumber left, GeoNumber right)
        {
            var res = left - right;
            return res.sign == false && !res.number.IsZero();
        }

        public static bool operator >=(GeoNumber left, GeoNumber right)
        {
            var res = left - right;
            return res.sign;
        }

        public static bool operator <=(GeoNumber left, GeoNumber right)
        {
            var res = left - right;
            return res.number.IsZero() || !res.sign;
        }


        private GeoNumber(bool sign, GeoUNumber number, int point)
        {
            this.sign = sign;
            this.number = number;
            this.point = point;
        }

        private void Trim()
        {
            if (number.IsZero()) { point = 0; }

            // ensure integer part is at least 0
            int firstLength = number.FirstUntilNotEqual(0, number.Length - point);
            if (firstLength >= number.Length - point) { firstLength = number.Length - point - 1; }

            // ensure decimal part less or equal then precision
            int lastLength = number.LastUntilNotEqual(0, point);
            if (point - lastLength > precision) { lastLength = point - precision; }

            // trim number
            point = point - lastLength;
            number.Trim(firstLength, lastLength);
            if (number.IsZero()) { sign = true; }
        }

        private static GeoNumber GetE(int iteration)
        {
            // e = 1/0! + 1/1! + 1/2! + ... + 1/n! (0!=1, n=iteration)
            var e = new GeoNumber(true, new GeoUNumber(new List<byte>() { 0 }), 0);

            // item2 = item2 + item1 => item3 = item3 * item2 => e = e + item1 / item3
            var item1 = new GeoNumber(true, new GeoUNumber(new List<byte>() { 1 }), 0);
            var item2 = new GeoNumber(true, new GeoUNumber(new List<byte>() { 1 }), 0);
            var item3 = new GeoNumber(true, new GeoUNumber(new List<byte>() { 1 }), 0);

            for (int i = 1; i <= iteration; i++)
            {
                // 1/0!=1, and 1/1!=1
                if (i == 1 || i == 2)
                {
                    e = e + item3;
                    continue;
                }

                // n!=(n-1)!*n
                item2 = item2 + item1;
                item3 = item3 * item2;
                e = e + item1 / item3;
            }

            return e;
        }
    }
}
