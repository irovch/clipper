using Clipper.Custom;

namespace Clipper
{
    public class IntersectNode
    {
        internal Edge Edge1;
        internal Edge Edge2;
        internal PointL Point;

        public override string ToString()
        {
            return $"P:{Point}, E1: {Edge1}, E2: {Edge2}";
        }
    }
}