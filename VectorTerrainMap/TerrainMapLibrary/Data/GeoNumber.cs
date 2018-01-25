using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TerrainMapLibrary.Localization;

namespace TerrainMapLibrary.Data
{
    public class GeoNumber
    {
        private bool sign;
        private List<byte> high;
        private List<byte> low;
        private int precision;

        public GeoNumber(string value, int precision)
        {
            // ensure value is valid
            if (string.IsNullOrEmpty(value))
            {
                throw new Exception(ExceptionMessage.NullValue);
            }

            // ensure precision is valid
            if (precision < 0)
            {
                throw new Exception(ExceptionMessage.InvalidPrecision);
            }

            // ensure value is a valid number
            Regex regex = new Regex("^[+-]?\\d+(\\.\\d+)?$");
            if (!regex.IsMatch(value))
            {
                throw new Exception($"{value} {ExceptionMessage.NotNumber}");
            }

            // get sign, integer part and decimal part
            sign = true;
            high = new List<byte>();
            low = new List<byte>();
            this.precision = precision;

            bool isHigh = true;
            foreach (var charactor in value)
            {
                if (charactor == '+')
                {
                    sign = true;
                }
                else if (charactor == '-')
                {
                    sign = false;
                }
                else if (charactor == '.')
                {
                    isHigh = false;
                }
                else
                {
                    byte digit = (byte)(charactor - 48);
                    if (isHigh)
                    {
                        high.Add(digit);
                    }
                    else if (low.Count < precision)
                    {
                        low.Add(digit);
                    }
                }
            }

            // trim leading and tail
            TrimZero();
        }

        public override string ToString()
        {
            // combine sign, integer part and decimal part
            StringBuilder sb = new StringBuilder();

            if (sign == false)
            {
                sb.Append("-");
            }

            foreach (byte digit in high)
            {
                sb.Append(digit);
            }

            if (low.Count > 0)
            {
                sb.Append(".");
            }

            foreach (byte digit in low)
            {
                sb.Append(digit);
            }

            return sb.ToString();
        }

        public static GeoNumber operator +(GeoNumber left, GeoNumber right)
        {
            GeoNumber result = null;

            // align both left and right number
            Align(left, right, out GeoNumber alignedLeft, out GeoNumber alignedRight);
            if (left.sign == right.sign)
            {
                // same sign, unsign add
                result = UnsignAdd(alignedLeft, alignedRight);
                result.sign = left.sign;
            }
            else
            {
                // different sign, unsign compare left and right number
                int compareResult = UnsignCompare(alignedLeft, alignedRight);
                if (compareResult > 0)
                {
                    // left is bigger, left sub right, sign comes from left
                    result = UnsignSub(alignedLeft, alignedRight);
                    result.sign = left.sign;
                }
                else if (compareResult < 0)
                {
                    // right is bigger, right sub left, sign comes from right
                    result = UnsignSub(alignedRight, alignedLeft);
                    result.sign = right.sign;
                }
                else
                {
                    // left is same with right, result is 0
                    result = new GeoNumber(true, new List<byte>() { 0 }, new List<byte>(), alignedLeft.precision);
                }
            }

            result.TrimZero();
            return result;
        }

        public static GeoNumber operator -(GeoNumber left, GeoNumber right)
        {
            GeoNumber result = null;

            // align both left and right number
            Align(left, right, out GeoNumber alignedLeft, out GeoNumber alignedRight);
            if (left.sign != right.sign)
            {
                // different sign, unsign add
                result = UnsignAdd(alignedLeft, alignedRight);
                result.sign = left.sign;
            }
            else
            {
                // same sign, unsign compare left and right number
                int compareResult = UnsignCompare(alignedLeft, alignedRight);
                if (compareResult > 0)
                {
                    // left is bigger, left sub right, sign comes from left
                    result = UnsignSub(alignedLeft, alignedRight);
                    result.sign = left.sign;
                }
                else if (compareResult < 0)
                {
                    // right is bigger, right sub left, sign is different with left
                    result = UnsignSub(alignedRight, alignedLeft);
                    result.sign = !left.sign;
                }
                else
                {
                    // left is same with right, result is 0
                    result = new GeoNumber(true, new List<byte>() { 0 }, new List<byte>(), alignedLeft.precision);
                }
            }

            result.TrimZero();
            return result;
        }


        protected GeoNumber(bool sign, List<byte> high, List<byte> low, int precision)
        {
            // ensure precision is valid
            if (precision < 0)
            {
                throw new Exception(ExceptionMessage.InvalidPrecision);
            }

            this.sign = sign;
            this.precision = precision;

            // ensure integer part is valid
            if (high == null)
            {
                throw new Exception(ExceptionMessage.NullInteger);
            }

            this.high = new List<byte>();
            foreach (var digit in high)
            {
                this.high.Add(digit);
            }

            // ensure decimal part is valid
            if (low == null)
            {
                throw new Exception(ExceptionMessage.NullDecimal);
            }

            this.low = new List<byte>();
            foreach (var digit in low)
            {
                if (this.low.Count < precision)
                {
                    this.low.Add(digit);
                }
            }
        }

        protected void TrimZero()
        {
            // trim leading on integer part
            var trimedHigh = new List<byte>();
            for (int i = 0; i < high.Count; i++)
            {
                if (high[i] != 0 || trimedHigh.Count > 0)
                {
                    trimedHigh.Add(high[i]);
                }
            }
            if (trimedHigh.Count == 0)
            {
                trimedHigh.Add(0);
            }

            high = trimedHigh;

            // trim tail on decimal part
            var trimedLow = new List<byte>();
            for (int i = low.Count - 1; i >= 0; i--)
            {
                if (low[i] != 0 || trimedLow.Count > 0)
                {
                    trimedLow.Insert(0, low[i]);
                }
            }

            low = trimedLow;
        }


        private static void Align(
            GeoNumber left,
            GeoNumber right,
            out GeoNumber alignedLeft,
            out GeoNumber alignedRight)
        {
            // choose bigger one
            int highLength = left.high.Count > right.high.Count ? left.high.Count : right.high.Count;
            int lowLength = left.low.Count > right.low.Count ? left.low.Count : right.low.Count;
            int precision = left.precision > right.precision ? left.precision : right.precision;

            // fill leading on integer part of left number
            var leftHigh = new List<byte>();
            for (int i = left.high.Count; i < highLength; i++)
            {
                leftHigh.Add(0);
            }
            for (int i = 0; i < left.high.Count; i++)
            {
                leftHigh.Add(left.high[i]);
            }

            // fill tail on decimal part of left number
            var leftLow = new List<byte>();
            for (int i = 0; i < left.low.Count; i++)
            {
                leftLow.Add(left.low[i]);
            }
            for (int i = left.low.Count; i < lowLength; i++)
            {
                leftLow.Add(0);
            }

            // generate aligned left number
            alignedLeft = new GeoNumber(left.sign, leftHigh, leftLow, precision);

            // fill leading on integer part of right number
            var rightHigh = new List<byte>();
            for (int i = right.high.Count; i < highLength; i++)
            {
                rightHigh.Add(0);
            }
            for (int i = 0; i < right.high.Count; i++)
            {
                rightHigh.Add(right.high[i]);
            }

            // fill tail on decimal part of right number
            var rightLow = new List<byte>();
            for (int i = 0; i < right.low.Count; i++)
            {
                rightLow.Add(right.low[i]);
            }
            for (int i = right.low.Count; i < lowLength; i++)
            {
                rightLow.Add(0);
            }

            // generate aligned right number
            alignedRight = new GeoNumber(right.sign, rightHigh, rightLow, precision);
        }

        private static int UnsignCompare(GeoNumber alignedLeft, GeoNumber alignedRight)
        {
            // compare each position of integer part
            for (int i = 0; i < alignedLeft.high.Count; i++)
            {
                if (alignedLeft.high[i] > alignedRight.high[i])
                {
                    // left is bigger
                    return 1;
                }
                else if (alignedLeft.high[i] < alignedRight.high[i])
                {
                    // right is bigger
                    return -1;
                }
            }

            // integer part is same, compare each position of decimal part
            for (int i = 0; i < alignedLeft.low.Count; i++)
            {
                if (alignedLeft.low[i] > alignedRight.low[i])
                {
                    // left is bigger
                    return 1;
                }
                else if (alignedLeft.low[i] < alignedRight.low[i])
                {
                    // right is bigger
                    return -1;
                }
            }

            // both integer part and decimal part are same, so left is same with right
            return 0;
        }

        private static GeoNumber UnsignAdd(GeoNumber alignedLeft, GeoNumber alignedRight)
        {
            int carry = 0;

            // add each position of decimal part
            var low = new List<byte>();
            for (int i = alignedLeft.low.Count - 1; i >= 0; i--)
            {
                // add carry from previous position
                int value = alignedLeft.low[i] + alignedRight.low[i] + carry;
                if (value >= 10)
                {
                    // carry to next position
                    value -= 10;
                    carry = 1;
                }
                else
                {
                    // do not carry
                    carry = 0;
                }

                low.Insert(0, (byte)value);
            }

            // add each position of integer part
            var high = new List<byte>();
            for (int i = alignedLeft.high.Count - 1; i >= 0; i--)
            {
                // add carry from previous position
                int value = alignedLeft.high[i] + alignedRight.high[i] + carry;
                if (value >= 10)
                {
                    // carry to next position
                    value -= 10;
                    carry = 1;
                }
                else
                {
                    // do not carry
                    carry = 0;
                }

                high.Insert(0, (byte)value);
            }

            if (carry > 0)
            {
                // carry to next new position
                high.Insert(0, (byte)carry);
            }

            var result = new GeoNumber(true, high, low, alignedLeft.precision);
            return result;
        }

        private static GeoNumber UnsignSub(GeoNumber alignedLeft, GeoNumber alignedRight)
        {
            int borrow = 0;

            // sub each position of decimal part
            var low = new List<byte>();
            for (int i = alignedLeft.low.Count - 1; i >= 0; i--)
            {
                // sub borrow from previous position
                int value = alignedLeft.low[i] - alignedRight.low[i] - borrow;
                if (value < 0)
                {
                    // borrow from next position
                    value += 10;
                    borrow = 1;
                }
                else
                {
                    // do not borrow
                    borrow = 0;
                }

                low.Insert(0, (byte)value);
            }

            // sub each position of integer part
            var high = new List<byte>();
            for (int i = alignedLeft.high.Count - 1; i >= 0; i--)
            {
                // sub borrow from previous position
                int value = alignedLeft.high[i] - alignedRight.high[i] - borrow;
                if (value < 0)
                {
                    // borrow from next position
                    value += 10;
                    borrow = 1;
                }
                else
                {
                    // do not borrow
                    borrow = 0;
                }

                high.Insert(0, (byte)value);
            }

            var result = new GeoNumber(true, high, low, alignedLeft.precision);
            return result;
        }
    }
}
