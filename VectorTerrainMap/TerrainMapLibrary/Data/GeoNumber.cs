using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            if (string.IsNullOrEmpty(value))
            {
                throw new Exception($"值是空的。");
            }

            if (precision < 0)
            {
                throw new Exception($"精度必须大于等于 0。");
            }

            Regex regex = new Regex("^[+-]?\\d+(\\.\\d+)?$");
            if (!regex.IsMatch(value))
            {
                throw new Exception($"{value} 不是一个数字。");
            }

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

            TrimZero();
        }

        public override string ToString()
        {
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

            Align(left, right, out GeoNumber alignedLeft, out GeoNumber alignedRight);

            if (left.sign == right.sign)
            {
                result = UnsignAdd(alignedLeft, alignedRight);
                result.sign = left.sign;
            }
            else
            {
                int compareResult = UnsignCompare(alignedLeft, alignedRight);

                if (compareResult > 0)
                {
                    result = UnsignSub(alignedLeft, alignedRight);
                    result.sign = left.sign;
                }
                else if (compareResult < 0)
                {
                    result = UnsignSub(alignedRight, alignedLeft);
                    result.sign = right.sign;
                }
                else
                {
                    result = new GeoNumber(true, new List<byte>() { 0 }, new List<byte>(), alignedLeft.precision);
                }
            }

            result.TrimZero();
            return result;
        }

        protected GeoNumber(bool sign, List<byte> high, List<byte> low, int precision)
        {
            if (precision < 0)
            {
                throw new Exception($"精度必须大于等于 0。");
            }

            this.sign = sign;
            this.precision = precision;

            if (high == null)
            {
                throw new Exception($"整数值是空的。");
            }

            this.high = new List<byte>();
            foreach (var digit in high)
            {
                this.high.Add(digit);
            }

            if (low == null)
            {
                throw new Exception($"小数值是空的。");
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
            int highLength = left.high.Count > right.high.Count ? left.high.Count : right.high.Count;
            int lowLength = left.low.Count > right.low.Count ? left.low.Count : right.low.Count;
            int precision = left.precision > right.precision ? left.precision : right.precision;

            var leftHigh = new List<byte>();
            for (int i = left.high.Count; i < highLength; i++)
            {
                leftHigh.Add(0);
            }
            for (int i = 0; i < left.high.Count; i++)
            {
                leftHigh.Add(left.high[i]);
            }

            var leftLow = new List<byte>();
            for (int i = 0; i < left.low.Count; i++)
            {
                leftLow.Add(left.low[i]);
            }
            for (int i = left.low.Count; i < lowLength; i++)
            {
                leftLow.Add(0);
            }

            alignedLeft = new GeoNumber(left.sign, leftHigh, leftLow, precision);

            var rightHigh = new List<byte>();
            for (int i = right.high.Count; i < highLength; i++)
            {
                rightHigh.Add(0);
            }
            for (int i = 0; i < right.high.Count; i++)
            {
                rightHigh.Add(right.high[i]);
            }

            var rightLow = new List<byte>();
            for (int i = 0; i < right.low.Count; i++)
            {
                rightLow.Add(right.low[i]);
            }
            for (int i = right.low.Count; i < lowLength; i++)
            {
                rightLow.Add(0);
            }

            alignedRight = new GeoNumber(right.sign, rightHigh, rightLow, precision);
        }

        private static int UnsignCompare(GeoNumber alignedLeft, GeoNumber alignedRight)
        {
            for (int i = 0; i < alignedLeft.high.Count; i++)
            {
                if (alignedLeft.high[i] > alignedRight.high[i])
                {
                    return 1;
                }
                else if (alignedLeft.high[i] < alignedRight.high[i])
                {
                    return -1;
                }
            }

            for (int i = 0; i < alignedLeft.low.Count; i++)
            {
                if (alignedLeft.low[i] > alignedRight.low[i])
                {
                    return 1;
                }
                else if (alignedLeft.low[i] < alignedRight.low[i])
                {
                    return -1;
                }
            }

            return 0;
        }

        private static GeoNumber UnsignAdd(GeoNumber alignedLeft, GeoNumber alignedRight)
        {
            int carry = 0;

            var low = new List<byte>();
            for (int i = alignedLeft.low.Count - 1; i >= 0; i--)
            {
                int value = alignedLeft.low[i] + alignedRight.low[i] + carry;
                if (value >= 10)
                {
                    value -= 10;
                    carry = 1;
                }
                else
                {
                    carry = 0;
                }
                low.Insert(0, (byte)value);
            }

            var high = new List<byte>();
            for (int i = alignedLeft.high.Count - 1; i >= 0; i--)
            {
                int value = alignedLeft.high[i] + alignedRight.high[i] + carry;
                if (value >= 10)
                {
                    value -= 10;
                    carry = 1;
                }
                else
                {
                    carry = 0;
                }
                high.Insert(0, (byte)value);
            }

            if (carry > 0)
            {
                high.Insert(0, (byte)carry);
            }

            var result = new GeoNumber(true, high, low, alignedLeft.precision);
            return result;
        }

        private static GeoNumber UnsignSub(GeoNumber alignedLeft, GeoNumber alignedRight)
        {
            int borrow = 0;

            var low = new List<byte>();
            for (int i = alignedLeft.low.Count - 1; i >= 0; i--)
            {
                int value = alignedLeft.low[i] - alignedRight.low[i] - borrow;
                if (value < 0)
                {
                    value += 10;
                    borrow = 1;
                }
                else
                {
                    borrow = 0;
                }
                low.Insert(0, (byte)value);
            }

            var high = new List<byte>();
            for (int i = alignedLeft.high.Count - 1; i >= 0; i--)
            {
                int value = alignedLeft.high[i] - alignedRight.high[i] - borrow;
                if (value < 0)
                {
                    value += 10;
                    borrow = 1;
                }
                else
                {
                    borrow = 0;
                }
                high.Insert(0, (byte)value);
            }

            var result = new GeoNumber(true, high, low, alignedLeft.precision);
            return result;
        }

        //public static GeoNumber operator +(GeoNumber left, GeoNumber right)
        //{
        //    if (left.multiple != right.multiple)
        //    {
        //        throw new Exception($"{left.ToString()} 与 {right.ToString()} 的精度不同。");
        //    }

        //    var result = new GeoNumber(left.GetPrecision(left.multiple));
        //    if (left.signal == right.signal)
        //    {
        //        if (UInt64.MaxValue - left.basis < right.basis)
        //        {
        //            throw new Exception($"{left.ToString()} 与 {right.ToString()} 的计算结果超出了最大范围。");
        //        }

        //        result.basis = left.basis + right.basis;
        //        result.signal = left.signal;
        //    }
        //    else if (left.basis > right.basis)
        //    {
        //        result.basis = left.basis - right.basis;
        //        result.signal = left.signal;
        //    }
        //    else if (right.basis > left.basis)
        //    {
        //        result.basis = right.basis - left.basis;
        //        result.signal = right.signal;
        //    }
        //    else
        //    {
        //        result.basis = 0;
        //        result.signal = true;
        //    }

        //    return result;
        //}

        //public static GeoNumber operator +(GeoNumber left, Double right)
        //{
        //    var nRight = new GeoNumber(right, left.GetPrecision(left.multiple));
        //    var result = left + nRight;
        //    return result;
        //}

        //public static GeoNumber operator +(Double left, GeoNumber right)
        //{
        //    var nLeft = new GeoNumber(left, right.GetPrecision(right.multiple));
        //    var result = nLeft + right;
        //    return result;
        //}

        //public static GeoNumber operator -(GeoNumber left, GeoNumber right)
        //{
        //    if (left.multiple != right.multiple)
        //    {
        //        throw new Exception($"{left.ToString()} 与 {right.ToString()} 的精度不同。");
        //    }

        //    var result = new GeoNumber(left.GetPrecision(left.multiple));
        //    if (left.signal != right.signal)
        //    {
        //        if (UInt64.MaxValue - left.basis < right.basis)
        //        {
        //            throw new Exception($"{left.ToString()} 与 {right.ToString()} 的计算结果超出了最大范围。");
        //        }

        //        result.basis = left.basis + right.basis;
        //        result.signal = left.signal;
        //    }
        //    else if (left.basis > right.basis)
        //    {
        //        result.basis = left.basis - right.basis;
        //        result.signal = left.signal;
        //    }
        //    else if (left.basis < right.basis)
        //    {
        //        result.basis = right.basis - left.basis;
        //        result.signal = !left.signal;
        //    }
        //    else
        //    {
        //        result.basis = 0;
        //        result.signal = true;
        //    }

        //    return result;
        //}

        //public static GeoNumber operator -(GeoNumber left, Double right)
        //{
        //    var nRight = new GeoNumber(right, left.GetPrecision(left.multiple));
        //    var result = left - nRight;
        //    return result;
        //}

        //public static GeoNumber operator -(Double left, GeoNumber right)
        //{
        //    var nLeft = new GeoNumber(left, right.GetPrecision(right.multiple));
        //    var result = nLeft - right;
        //    return result;
        //}

        //private UInt64 GetMutiple(Byte precision)
        //{
        //    UInt64 multiple = 1;
        //    for (int i = 0; i < precision; i++)
        //    {
        //        if (UInt64.MaxValue / 10 < multiple)
        //        {
        //            throw new Exception($"精度 {precision} 超出了最大范围。");
        //        }
        //        multiple *= 10;
        //    }
        //    return multiple;
        //}

        //private Byte GetPrecision(UInt64 multiple)
        //{
        //    Byte precision = 0;
        //    for (UInt64 i = 1; i < multiple; i *= 10)
        //    {
        //        precision += 1;
        //    }
        //    return precision;
        //}
    }
}
