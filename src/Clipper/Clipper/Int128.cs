using System;
using System.Runtime.CompilerServices;

namespace Clipper
{
    /// <summary>
    /// Int128 struct (enables safe math on signed 64bit integers)
    /// eg Int128 val1((Int64)9223372036854775807); //ie 2^63 -1
    ///    Int128 val2((Int64)9223372036854775807);
    ///    Int128 val3 = val1 * val2;
    ///    val3.ToString => "85070591730234615847396907784232501249" (8.5e+37)
    /// </summary>
    public readonly struct Int128
    {
        private readonly long _hi;
        private readonly ulong _lo;

        public Int128(in long hi, in ulong lo)
        {
            _lo = lo;
            _hi = hi;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in Int128 val1, in Int128 val2)
        {
            return val1._hi == val2._hi && val1._lo == val2._lo;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in Int128 val1, in Int128 val2)
        {
            return val1._hi != val2._hi || val1._lo != val2._lo;
        }

        public override bool Equals(object obj)
        {
            return obj is Int128 i128 && i128._hi == this._hi && i128._lo == this._lo;
        }

        public bool Equals(in Int128 other)
        {
            return this._hi == other._hi && this._lo == other._lo;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this._hi, this._lo);
        }

        public static bool operator >(in Int128 val1, in Int128 val2)
        {
            if (val1._hi != val2._hi)
            {
                return val1._hi > val2._hi;
            }

            return val1._lo > val2._lo;
        }

        public static bool operator <(in Int128 val1, in Int128 val2)
        {
            if (val1._hi != val2._hi)
            {
                return val1._hi < val2._hi;
            }

            return val1._lo < val2._lo;
        }

        public static Int128 operator +(in Int128 lhs, in Int128 rhs)
        {
            var hi = lhs._hi + rhs._hi;
            var lo = lhs._lo + rhs._lo;
            if (lo < rhs._lo)
            {
                hi++;
            }

            return new Int128(hi, lo);
        }

        public static Int128 operator -(in Int128 lhs, in Int128 rhs)
        {
            return lhs + -rhs;
        }

        public static Int128 operator -(in Int128 val)
        {
            return val._lo == 0
                ? new Int128(-val._hi, 0)
                : new Int128(~val._hi, ~val._lo + 1);
        }

        public static explicit operator double(in Int128 val)
        {
            const double shift64 = 18446744073709551616.0; // 2^64

            if (val._hi >= 0) return val._lo + val._hi * shift64;

            if (val._lo == 0)
            {
                return val._hi * shift64;
            }

            return -(~val._lo + ~val._hi * shift64);
        }

        // nb: Constructing two new Int128 objects every time we want to multiply longs  
        // is slow. So, although calling the Int128Mul method doesn't look as clean, the 
        // code runs significantly faster than if we'd used the * operator.

        public static Int128 Int128Mul(long lhs, long rhs)
        {
            var negate = lhs < 0 != rhs < 0;
            if (lhs < 0) lhs = -lhs;
            if (rhs < 0) rhs = -rhs;
            var int1Hi = (ulong)lhs >> 32;
            var int1Lo = (ulong)lhs & 0xFFFFFFFF;
            var int2Hi = (ulong)rhs >> 32;
            var int2Lo = (ulong)rhs & 0xFFFFFFFF;

            // nb: see comments in clipper.pas
            var a = int1Hi * int2Hi;
            var b = int1Lo * int2Lo;
            var c = int1Hi * int2Lo + int1Lo * int2Hi;

            ulong lo;
            var hi = (long)(a + (c >> 32));

            unchecked { lo = (c << 32) + b; }
            if (lo < b) hi++;
            var result = new Int128(hi, lo);
            return negate ? -result : result;
        }
    };
}