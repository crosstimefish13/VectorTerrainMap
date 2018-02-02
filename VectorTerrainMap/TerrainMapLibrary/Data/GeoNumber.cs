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
        private static int iterations = 10;


        public static GeoNumber E
        {
            get
            {
                if (e == null)
                {
                    e = GetE();
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

        public static int Iterations
        {
            get
            {
                return iterations;
            }
            set
            {
                // ensure iterations is valid
                if (value < 1)
                {
                    throw new Exception(ExceptionMessage.InvalidIterations);
                }

                // need to reset constant
                if (iterations != value)
                {
                    e = null;
                }

                iterations = value;
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
            point = number.Trim(point);
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
                dupR.number.ShiftLeft(dupL.point - dupR.point);
            }
            else if (dupL.point < dupR.point)
            {
                resPoint = dupR.point;
                dupL.number.ShiftLeft(dupR.point - dupL.point);
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
            resPoint = resNumber.Trim(resPoint);
            var res = new GeoNumber(resSign, resNumber, resPoint);
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
                dupR.number.ShiftLeft(dupL.point - dupR.point);
            }
            else if (dupL.point < dupR.point)
            {
                resPoint = dupR.point;
                dupL.number.ShiftLeft(dupR.point - dupL.point);
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
            resPoint = resNumber.Trim(resPoint);
            var res = new GeoNumber(resSign, resNumber, resPoint);
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
            resPoint = resNumber.Trim(resPoint);
            if (resNumber.IsZero()) { resSign = true; }
            var res = new GeoNumber(resSign, resNumber, resPoint);
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
                dupR.number.ShiftLeft(dupL.point - dupR.point);
            }
            else if (dupL.point < dupR.point)
            {
                dupL.number.ShiftLeft(dupR.point - dupL.point);
            }

            // mod for integer part, and division for decimal part
            var mod = dupL.number.Mod(dupR.number);
            if (!mod.IsZero()) { mod.Div(dupR.number, precision); }

            // combine integer and decimal parts
            int resPoint = mod.Length;
            var resNumber = dupL.number;
            resNumber.ShiftLeft(resPoint);
            resNumber.Add(mod);

            // trim result
            resPoint = resNumber.Trim(resPoint);
            if (resNumber.IsZero()) { resSign = true; }
            var res = new GeoNumber(resSign, resNumber, resPoint);
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

            var resPoint = mod.Trim(0);
            var res = new GeoNumber(true, mod, resPoint);
            return res;
        }

        public static bool operator ==(GeoNumber left, GeoNumber right)
        {
            var isLeftNull = ReferenceEquals(left, null);
            var isRightNull = ReferenceEquals(right, null);

            if (isLeftNull && isRightNull) { return true; }
            else if (!isLeftNull || !isRightNull) { return false; }
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

        private static GeoNumber GetE()
        {
            var e = new GeoNumber(true, new GeoUNumber(new List<byte>() { 1 }), 0);
            var items = new List<GeoNumber>() { new GeoNumber(true, new GeoUNumber(new List<byte>() { 1 }), 0) };

            for (int i = 0; i < iterations; i++)
            {
                // calucluate 1 / n!
                var item = items[0];
                for (int j = 1; j < items.Count; j++)
                {
                    item = item * items[j];
                }

                item = items[0] / item;
                e = e + item;

                items.Add(items[items.Count - 1] + items[0]);
            }

            return e;
        }
    }
}
