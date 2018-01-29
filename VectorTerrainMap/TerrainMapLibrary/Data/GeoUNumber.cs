using System.Collections.Generic;
using System.Text;

namespace TerrainMapLibrary.Data
{
    internal class GeoUNumber
    {
        private List<byte> digits;


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

        public int Trim(int point)
        {
            if (point >= digits.Count) { return point; }

            // trim decimal trail 0
            var rightDigits = new List<byte>();
            for (int i = 0; i < point; i++)
            {
                if (rightDigits.Count > 0 || digits[i] > 0) { rightDigits.Add(digits[i]); }
            }

            // trim integer leading 0
            var leftDigits = new List<byte>();
            for (int i = digits.Count - 1; i >= point; i--)
            {
                if (leftDigits.Count > 0 || digits[i] > 0) { leftDigits.Insert(0, digits[i]); }
            }

            digits.Clear();
            for (int i = 0; i < rightDigits.Count; i++)
            {
                digits.Add(rightDigits[i]);
            }

            // ensure integer is at least 0
            if (leftDigits.Count <= 0) { leftDigits.Add(0); }
            for (int i = 0; i < leftDigits.Count; i++)
            {
                digits.Add(leftDigits[i]);
            }

            // need to adjust point location
            return rightDigits.Count;
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

        public void Multiple(GeoUNumber number)
        {
            var products = new List<GeoUNumber>();
            for (int i = 0; i < number.digits.Count; i++)
            {
                // multiple each digits and shift the result products
                var product = new GeoUNumber(digits);
                product.MultipleDigit(number.digits[i]);
                product.ShiftLeft(i);
                products.Add(product);
            }

            digits.Clear();
            foreach (var item in products)
            {
                // add each products to get result
                Add(item);
            }
        }

        public void ShiftLeft(int shift)
        {
            for (int i = 0; i < shift; i++)
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

        private byte GetDigit(int position)
        {
            // return 0 if out of position
            if (position >= digits.Count) { return 0; }
            else { return digits[position]; }
        }

        private void SetDigit(int position, byte digit)
        {
            // add a new if equals the digits count
            if (position < digits.Count) { digits[position] = digit; }
            else if (position == digits.Count) { digits.Add(digit); }
        }
    }
}
