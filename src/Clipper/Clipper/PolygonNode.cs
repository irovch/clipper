using System.Collections.Generic;

namespace Clipper
{
    public class PolygonNode
    {
        /// <summary>
        /// The polygon points for this node in the tree.
        /// </summary>
        public Polygon Polygon { get; } = new Polygon();

        /// <summary>
        /// This children polygons contained within this node polygon.
        /// </summary>
        public List<PolygonNode> Children { get; } = new List<PolygonNode>();

        /// <summary>
        /// True is this node polygon is an open (not closed) polygon.
        /// </summary>
        public bool IsOpen { get; set; }

        public JoinType JoinType { get; set; }

        public EndType EndType { get; set; }

        internal void AddChild(PolygonNode child)
        {
            Children.Add(child);
        }
    }
}