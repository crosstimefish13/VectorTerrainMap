using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TerrainMapLibrary.Localization;

namespace TerrainMapLibrary.Arithmetic
{
    public sealed class GeoNumber
    {
        private bool sign;
        private GeoUNumber number;
        private int point;
        private static GeoNumber zero;
        private static GeoNumber one;
        private static GeoNumber two;
        private static GeoNumber e;
        private static GeoNumber elog2;
        private static int precision = 100;


        private static GeoNumber Zero
        {
            get
            {
                if (zero == null)
                {
                    zero = new GeoNumber(true, new GeoUNumber(new List<byte>() { 0 }), 0);
                }

                return zero;
            }
        }

        private static GeoNumber One
        {
            get
            {
                if (one == null)
                {
                    one = new GeoNumber(true, new GeoUNumber(new List<byte>() { 1 }), 0);
                }

                return one;
            }
        }

        private static GeoNumber Two
        {
            get
            {
                if (two == null)
                {
                    two = new GeoNumber(true, new GeoUNumber(new List<byte>() { 2 }), 0);
                }

                return two;
            }
        }

        private static GeoNumber ELog2
        {
            get
            {
                if (elog2 == null)
                {
                    elog2 = GetRealLog2(E, precision + 3);
                }

                return elog2;
            }
        }


        public static GeoNumber E
        {
            get
            {
                if (e == null)
                {
                    e = GetEPowReal(new GeoNumber(true, new GeoUNumber(new List<byte>() { 1 }), 0), precision + 3);
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
                    elog2 = null;
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
            return this == number;
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

        public GeoNumber Floor(int reserve = 0)
        {
            if (reserve < 0) { throw new Exception(ExceptionMessage.InvalidReserve); }
            if (reserve >= point) { return Copy(); }

            var resNumber = number.Copy();
            resNumber.Trim(0, point - reserve);
            var res = new GeoNumber(sign, resNumber, reserve);

            if (res.sign == false && res.point < point)
            {
                var digits = new List<byte>() { 1 };
                for (int i = 0; i < res.point; i++)
                {
                    digits.Add(0);
                }

                var item = new GeoNumber(false, new GeoUNumber(digits), res.point);
                res = res + item;
            }

            res.Trim();
            return res;
        }

        public GeoNumber Ceiling(int reserve = 0)
        {
            if (reserve < 0) { throw new Exception(ExceptionMessage.InvalidReserve); }
            else if (reserve >= point || number.IsZero()) { return Copy(); }

            var res = -(-this).Floor(reserve);
            return res;
        }

        public GeoNumber Pow(GeoNumber exponent)
        {
            // 0^n=0
            if (this == Zero) { return Zero; }

            // 1^n=1, and x^0=1
            if (this == One || exponent == Zero) { return One; }

            // (-1)^n=1 (n%2=0), and (-1)^n=-1 (n%2=1)
            if (this == -One)
            {
                if (exponent % Two == Zero) { return One; }
                else { return -One; }
            }

            // real pow integer
            if (exponent.point == 0) { return GetRealPowInteger(this, exponent); }

            // exponent it's not integer, check if sign is valid
            if (sign == false && exponent.number.IsOdd()) { throw new Exception(ExceptionMessage.InvalidExponent); }

            // x^p=(e^(ln(x)))^p=e^(p*ln(x))=e^(p*(log2(x)/log2(e)))
            var dupBasis = Copy();
            dupBasis.sign = true;
            var item = exponent * GetRealLog2(dupBasis, precision + 3) / ELog2;
            var res = GetEPowReal(item, precision + 3);
            res.Trim();
            return res;
        }

        public GeoNumber Log(GeoNumber basis)
        {
            if (this <= Zero) { throw new Exception(ExceptionMessage.InvalidAntilogarithm); }

            if (basis <= Zero || basis == One) { throw new Exception(ExceptionMessage.InvalidLogarithmBasis); }

            // logx(y)=log2(y)/log2(x)
            var res = GetRealLog2(this, precision + 3) / GetRealLog2(basis, precision + 3);
            res.Trim();
            return res;
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
            var isLeftNull = left is null;
            var isRightNull = right is null;

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
            return !(left == right);
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

        private static GeoNumber GetEPowReal(GeoNumber exponent, int iteration)
        {
            // e^x = (x^0)/0! + (x^1)/1! + (x^2)/2! + ... + (x^n)/n! (0!=1, n=iteration)
            var res = Zero.Copy();
            var top = One.Copy();
            var count = Zero.Copy();
            var bottom = One.Copy();

            for (int i = 1; i <= iteration; i++)
            {
                if (i == 1)
                {
                    res = res + One;
                    continue;
                }

                // x^n=(x^(n-1))*x
                top = top * exponent;

                // n!=(n-1)!*n
                count = count + One;
                bottom = bottom * count;
                res = res + top / bottom;
            }

            return res;
        }

        private static GeoNumber GetRealPowInteger(GeoNumber basis, GeoNumber exponent)
        {
            // let exponent and basis to be positive number
            var dupE = exponent.Copy();
            dupE.sign = true;
            var dupB = basis.Copy();
            dupB.sign = true;

            // iteration to get the result
            var res = One.Copy();
            while (dupE > One)
            {
                if (dupE % Two == One) { res = res * dupB; }
                dupE = (dupE / Two).Floor();
                dupB = dupB * dupB;
            }

            res = res * dupB;

            // x^n=1/(x^(-n)) (n<0)
            if (exponent < Zero) { res = One / res; }

            // need to minus if basis < 0 and exponent is odd number
            dupE = exponent.Copy();
            dupE.sign = true;
            if (basis < Zero && dupE % Two == One) { res = -res; }

            return res;
        }

        private static GeoNumber GetRealLog2(GeoNumber antilog, int iteration)
        {
            if (antilog == One)
            {
                // log2(1)=0
                return Zero;
            }
            else if (antilog < One)
            {
                // log2(n)=m (0<n<1, m<0)
                return GetFractionLog2(antilog, iteration);
            }

            // log2(antilog)=interger+log2(fractional) (fractional=(2^(-interger))*antilog, 1<=fractional<2)
            var res = Zero.Copy();
            var fractional = antilog;
            while (fractional >= Two)
            {
                fractional = fractional / Two;
                res += One;
            }

            // let y=x^(2^m) => log2(x)=(2^m)*log2(y) => log2(y)=log2(x)/(2^m) => log2(y)=(1+log2(x/2))/(2^m)
            // => 2^(-m) + (2^(-m))*log2(x/2) where 1<=y<2, choose [m] to make [2<=x<4], then 1<=x/2<2, so 
            // [y] is equivalent to [x/2], created the iteration
            var resFractional = One.Copy();
            
            for (int i = 1; i <= iteration; i++)
            {
                if (fractional == Zero || fractional == One) { break; }
                var count = Zero.Copy();
                while (fractional < Two)
                {
                    count = count - One;
                    fractional = fractional * fractional;
                }

                resFractional = resFractional * GetRealPowInteger(Two, count);
                res = res + resFractional;
                fractional = fractional / Two;
            }

            return res;
        }

        private static GeoNumber GetFractionLog2(GeoNumber antilog, int iteration)
        {
            // log2(x/y)=log2(x) - log2(y)
            // log2(antilog)=log2(xAntilog)-log2(x10) (xAntilog=antilog*(10^xCount), xAntilog>=1, x10=10^xCount)
            // get xCount to make sure xAntilog is greater or equal than 1
            var xCount = antilog.point;
            var xAntilog = new GeoNumber(true, antilog.number.Copy(), 0);

            // enlarge 1 to get x10
            var x10Number = new GeoUNumber(new List<byte>() { 1 });
            x10Number.Enlarge10(xCount);
            var x10 = new GeoNumber(true, x10Number, 0);

            // get result
            var result = GetRealLog2(xAntilog, iteration) - GetRealLog2(x10, iteration);
            return result;
        }
    }
}
