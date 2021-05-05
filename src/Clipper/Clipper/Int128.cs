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
    public static class Int128
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Int128MulEq(long lhs1, long rhs1, long lhs2, long rhs2)
        {
            if (lhs1 == 0 || rhs1 == 0) return lhs2 == 0 || rhs2 == 0;
            
            var negate1 = lhs1 < 0 != rhs1 < 0;
            var negate2 = lhs2 < 0 != rhs2 < 0;
            if (negate1 != negate2)
            {
                return false;
            }
            
            if (lhs1 < 0) lhs1 = -lhs1;
            if (rhs1 < 0) rhs1 = -rhs1;
            if (lhs2 < 0) lhs2 = -lhs2;
            if (rhs2 < 0) rhs2 = -rhs2;

            if (lhs1 == lhs2 && rhs1 == rhs2 || lhs1 == rhs2 && lhs2 == rhs1)
            {
                return true;
            }
            
            var int1Hi1 = (ulong)lhs1 >> 32;
            var int1Lo1 = (ulong)lhs1 & 0xFFFFFFFF;
            var int2Hi1 = (ulong)rhs1 >> 32;
            var int2Lo1 = (ulong)rhs1 & 0xFFFFFFFF;
            
            var int1Hi2 = (ulong)lhs2 >> 32;
            var int1Lo2 = (ulong)lhs2 & 0xFFFFFFFF;
            var int2Hi2 = (ulong)rhs2 >> 32;
            var int2Lo2 = (ulong)rhs2 & 0xFFFFFFFF;
            
            // nb: see comments in clipper.pas
            var b1 = int1Lo1 * int2Lo1;
            var c1 = int1Hi1 * int2Lo1 + int1Lo1 * int2Hi1;
            
            ulong lo1;
            unchecked { lo1 = (c1 << 32) + b1; }
            
            // nb: see comments in clipper.pas
            var b2 = int1Lo2 * int2Lo2;
            var c2 = int1Hi2 * int2Lo2 + int1Lo2 * int2Hi2;
            
            ulong lo2;
            unchecked { lo2 = (c2 << 32) + b2; }

            if (lo1 != lo2)
            {
                return false;
            }
            
            var a1 = int1Hi1 * int2Hi1;
            var a2 = int1Hi2 * int2Hi2;

            var hi1 = (long)(a1 + (c1 >> 32));
            if (lo1 < b1) hi1++;
            
            var hi2 = (long)(a2 + (c2 >> 32));
            if (lo2 < b2) hi2++;

            return hi1 == hi2;
        }
    };
}