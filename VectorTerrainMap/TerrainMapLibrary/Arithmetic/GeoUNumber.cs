using System.Collections.Generic;
using System.Text;

namespace TerrainMapLibrary.Arithmetic
{
    internal class GeoUNumber
    {
        private List<byte> digits;


        public int Length
        {
            get { return digits.Count; }
        }


        public GeoUNumber(List<byte> digits)
        {
            this.digits = new List<byte>();

            if (digits != null)
            {
                foreach (var digit in digits)
                {
                    this.digits.Add(digit);
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = digits.Count - 1; i >= 0; i--)
            {
                sb.Append(digits[i]);
            }

            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is GeoUNumber)) { return false; }
            var number = obj as GeoUNumber;
            return ToString() == number.ToString();
        }

        public override int GetHashCode()
        {
            return digits.GetHashCode();
        }

        public GeoUNumber Copy()
        {
            var dup = new GeoUNumber(digits);
            return dup;
        }

        public CompareResult Compare(GeoUNumber number)
        {
            // compare each dights, if it's over length, then digit is 0
            int length = digits.Count >= number.digits.Count ? digits.Count : number.digits.Count;
            var result = CompareResult.Equal;

            for (int i = length - 1; i >= 0; i--)
            {
                byte left = GetDigit(i);
                byte right = number.GetDigit(i);
                if (left > right)
                {
                    result = CompareResult.Big;
                    break;
                }
                else if (left < right)
                {
                    result = CompareResult.Small;
                    break;
                }
            }

            return result;
        }

        public int FirstUntilNotEqual(byte value, int maxCheckLength = 0)
        {
            int count = 0;
            int terminate = digits.Count - maxCheckLength;
            if (terminate < 0) { terminate = 0; }

            for (int i = digits.Count - 1; i >= terminate; i--)
            {
                // match until not equal from first to last
                if (digits[i] == value) { count += 1; }
                else { break; }
            }

            return count;
        }

        public int LastUntilNotEqual(byte value, int maxCheckLength = 0)
        {
            int count = 0;
            int terminate = maxCheckLength;
            if (terminate > digits.Count) { terminate = digits.Count; }

            for (int i = 0; i < terminate; i++)
            {
                // match until not equal from last to first
                if (digits[i] == value) { count += 1; }
                else { break; }
            }

            return count;
        }

        public void Trim(int firstLength, int lastLength)
        {
            if (firstLength + lastLength >= digits.Count)
            {
                // it's zero
                digits = new List<byte>() { 0 };
                return;
            }

            // trim first and last
            var newDigits = new List<byte>();
            for (int i = lastLength; i < digits.Count - firstLength; i++)
            {
                newDigits.Add(digits[i]);
            }

            digits = newDigits;
        }

        public bool IsZero()
        {
            bool isZero = true;
            foreach (var digit in digits)
            {
                if (digit != 0)
                {
                    isZero = false;
                    break;
                }
            }

            return isZero;
        }

        public bool IsOdd()
        {
            bool isOdd = digits[0] % 2 != 0;
            return isOdd;
        }

        public void Add(GeoUNumber number)
        {
            int length = digits.Count >= number.digits.Count ? digits.Count : number.digits.Count;
            int carry = 0;

            for (int i = 0; i < length; i++)
            {
                // add each digits and previous carry
                int value = GetDigit(i) + number.GetDigit(i) + carry;
                if (value >= 10)
                {
                    // need to carry in next
                    carry = 1;
                    SetDigit(i, (byte)(value - 10));
                }
                else
                {
                    carry = 0;
                    SetDigit(i, (byte)value);
                }
            }

            if (carry > 0)
            {
                // need to add 1 in next digit
                SetDigit(digits.Count, 1);
            }
        }

        public void Sub(GeoUNumber number)
        {
            int length = digits.Count >= number.digits.Count ? digits.Count : number.digits.Count;
            int borrow = 0;

            for (int i = 0; i < digits.Count; i++)
            {
                // sub each digits and previous borrow
                int value = GetDigit(i) - number.GetDigit(i) - borrow;
                if (value < 0)
                {
                    // need to borrow from next
                    borrow = 1;
                    SetDigit(i, (byte)(value + 10));
                }
                else
                {
                    borrow = 0;
                    SetDigit(i, (byte)value);
                }
            }
        }

        public void Mul(GeoUNumber number)
        {
            var products = new List<GeoUNumber>();
            for (int i = 0; i < number.digits.Count; i++)
            {
                // multiple each digits and shift the result products
                var product = new GeoUNumber(digits);
                product.MultipleDigit(number.digits[i]);
                product.Enlarge10(i);
                products.Add(product);
            }

            digits.Clear();
            foreach (var item in products)
            {
                // add each products to get result
                Add(item);
            }
        }

        public GeoUNumber Mod(GeoUNumber number)
        {
            var divisors = GetDivisors(number);
            var basis = new GeoUNumber(new List<byte>() { 0 });
            var division = new List<byte>();

            for (int i = digits.Count - 1; i >= 0; i--)
            {
                // try division each digits, the next basis is current basis * 10, then add current digit
                basis.digits.Insert(0, GetDigit(i));
                division.Insert(0, basis.DivisionDivisors(divisors));
            }

            // set the division as result value, then return basis as mod value
            digits = division;
            return basis;
        }

        public void Div(GeoUNumber number, int precision)
        {
            var divisors = GetDivisors(number);
            var basis = new GeoUNumber(digits);
            var division = new List<byte>();

            for (int i = 0; i < precision; i++)
            {
                // try division until over the precision, the next basis is current basis * 10
                basis.digits.Insert(0, 0);
                division.Insert(0, basis.DivisionDivisors(divisors));

                // break if mode is 0
                if (basis.IsZero()) { break; }
            }

            // set the division as result value
            digits = division;
        }

        public void Enlarge10(int count)
        {
            for (int i = 0; i < count; i++)
            {
                digits.Insert(0, 0);
            }
        }


        private void MultipleDigit(byte digit)
        {
            int carry = 0;

            for (int i = 0; i < digits.Count; i++)
            {
                // multiple each digits and add the previous carry
                int value = GetDigit(i) * digit + carry;
                if (value >= 10)
                {
                    // need to carry in next
                    carry = value / 10;
                    SetDigit(i, (byte)(value - carry * 10));
                }
                else
                {
                    carry = 0;
                    SetDigit(i, (byte)value);
                }
            }

            int remain = carry;
            while (remain > 0)
            {
                // need to add all carries
                remain = carry / 10;
                SetDigit(digits.Count, (byte)(carry - remain * 10));
            }
        }

        private byte DivisionDivisors(List<GeoUNumber> divisors)
        {
            int digit = 0;
            for (int i = 0; i < divisors.Count; i++)
            {
                // compare current value with each divisiors from 0 to 9
                var compare = Compare(divisors[i]);
                if (compare == CompareResult.Small)
                {
                    // it means the previous digit is the division, sub divisor to get mod
                    digit = i - 1;
                    Sub(divisors[i - 1]);
                    break;
                }
                else if (compare == CompareResult.Equal)
                {
                    // it means mod is 0, division is current digit
                    digit = i;
                    digits.Clear();
                    digits.Add(0);
                    break;
                }
                else if (i == divisors.Count - 1)
                {
                    // it means division is 9, and sub divisor to get mod
                    digit = i;
                    Sub(divisors[i]);
                    break;
                }
            }

            return (byte)digit;
        }

        private byte GetDigit(int position)
        {
            // return 0 if out of position
            if (position >= digits.Count || position < 0) { return 0; }
            else { return digits[position]; }
        }

        private void SetDigit(int position, byte digit)
        {
            // add a new if equals the digits count
            if (position < digits.Count) { digits[position] = digit; }
            else if (position == digits.Count) { digits.Add(digit); }
        }

        private List<GeoUNumber> GetDivisors(GeoUNumber number)
        {
            // divisors from 0 to 9
            var divisors = new List<GeoUNumber>();
            var divisor = new GeoUNumber(new List<byte>() { 0 });
            for (int i = 0; i < 10; i++)
            {
                divisors.Add(new GeoUNumber(divisor.digits));
                divisor.Add(number);
            }

            return divisors;
        }
    }
}
