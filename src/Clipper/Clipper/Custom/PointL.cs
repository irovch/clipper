using System;

namespace Clipper.Custom
{
    public readonly struct PointL
    {
        public readonly long X;
        public readonly long Y;

        public double Length => Math.Sqrt(X * X + Y * Y);

        public PointL(long x, long y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(in PointL other)
        {
            return this.X == other.X && this.Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is PointL point && this.Equals(point);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.X, this.Y);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public static bool operator ==(in PointL a, in PointL b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(in PointL a, in PointL b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public static PointL operator -(in PointL a, in PointL b)
        {
            return new PointL(a.X - b.X, a.Y - b.Y);
        }
    }
}