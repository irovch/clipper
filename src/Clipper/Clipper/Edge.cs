using Clipper.Custom;

namespace Clipper
{
    public class Edge
    {
        /// <summary>
        /// The edge most bottom point.
        /// </summary>
        public PointL Bottom;

        /// <summary>
        /// The edge top point.
        /// </summary>
        public PointL Top;

        /// <summary>
        /// Delta between top and bottom points.
        /// </summary>
        public PointL Delta;

        /// <summary>
        /// The current working point for the edge, updated for each scanbeam.
        /// </summary>
        internal PointL Current;

        internal double Dx;
        internal PolygonKind Kind;
        internal EdgeSide Side; // Side only refers to current side of solution poly.
        internal int WindDelta; // 1 or -1 depending on winding direction
        internal int WindCount;
        internal int WindCount2; // winding count of the opposite kind
        internal int OutIndex;
        internal Edge Next;
        internal Edge Prev;

        // For LML linking
        internal Edge NextInLml;

        // For AEL linking
        internal Edge NextInAel;
        internal Edge PrevInAel;

        // For SEL linking
        internal Edge NextInSel;
        internal Edge PrevInSel;

        internal bool IsHorizontal { get; private set; }

        internal void SetBoundaryLinks(Edge next, Edge prev)
        {
            Next = next;
            Prev = prev;
        }

        internal void InitializeGeometry()
        {
            if (Current.Y >= Next.Current.Y)
            {
                Bottom = Current;
                Top = Next.Current;
            }
            else
            {
                Top = Current;
                Bottom = Next.Current;
            }
            SetDx();
        }

        internal void SetDx()
        {
            this.Delta = new PointL(Top.X - Bottom.X, Top.Y - Bottom.Y);

            IsHorizontal = Delta.Y == 0;

            Dx = IsHorizontal
                ? ClippingHelper.Horizontal
                : (double)Delta.X / Delta.Y;
        }

        internal void ReverseHorizontal()
        {
            // swap horizontal edges' top and bottom x's so they follow the natural
            // progression of the bounds - ie so their xbots will align with the
            // adjoining lower edge. [Helpful in the ProcessHorizontal() method.]
            
            // GeometryHelper.Swap(ref Top.X, ref Bottom.X);
            var tmp = this.Top.X;
            this.Top = new PointL(this.Bottom.X, this.Top.Y);
            this.Bottom = new PointL(tmp, this.Bottom.Y);
        }

        public override string ToString()
        {
            return $"C:{Current}, B: {Bottom}, T: {Top}, {Kind}, {Side}";
        }
    }
}