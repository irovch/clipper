using System;

namespace Clipper
{
    public readonly struct IntPoint
    {
        public readonly long X;
        public readonly long Y;

        public double Length => Math.Sqrt(X * X + Y * Y);

        public IntPoint(long x, long y)
        {
            X = x;
            Y = y;
        }

        public IntPoint(double x, double y)
        {
            X = (long)x;
            Y = (long)y;
        }

        public IntPoint(DoublePoint point) : this(point.X, point.Y)
        {            
        }

        public bool Equals(IntPoint other)
        {
            return this.X == other.X && this.Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is IntPoint point && this.Equals(point);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.X, this.Y);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public static bool operator ==(IntPoint a, IntPoint b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(IntPoint a, IntPoint b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public static DoublePoint operator +(IntPoint a, IntPoint b)
        {
            return new DoublePoint(a.X + b.X, a.Y + b.Y);
        }

        public static IntPoint operator -(IntPoint a, IntPoint b)
        {
            return new IntPoint(a.X - b.X, a.Y - b.Y);
        }

        public static IntPoint operator *(IntPoint a, IntPoint b)
        {
            return new IntPoint(a.X * b.X, a.Y * b.Y);
        }

        public static IntPoint operator *(IntPoint a, long b)
        {
            return new IntPoint(a.X * b, a.Y * b);
        }

        public static IntPoint operator *(IntPoint a, double b)
        {
            return new IntPoint(a.X * b, a.Y * b);
        }

        public static IntPoint operator /(IntPoint a, IntPoint b)
        {
            return new IntPoint(a.X / b.X, a.Y / b.Y);
        }

        public static IntPoint operator /(IntPoint a, long b)
        {
            var scale = 1.0 / b;
            return a * scale;
        }

        public static IntPoint operator /(IntPoint a, double b)
        {
            var scale = 1.0 / b;
            return a * scale;
        }
    }
}