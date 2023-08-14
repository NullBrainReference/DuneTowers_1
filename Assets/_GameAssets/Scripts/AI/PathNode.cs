using System;

namespace AI.A_Star
{
    internal readonly struct PathNode : IComparable<PathNode>
    {
        private readonly double estimatedTotalCost;
        private readonly double heuristicDistance;

        public PathNode(Vector2Int position, Vector2Int target, double traverseDistance)
        {
            Position = position;
            TraverseDistance = traverseDistance;
            heuristicDistance = (position - target).DistanceEstimate();
            estimatedTotalCost = traverseDistance + heuristicDistance;
        }

        public Vector2Int Position { get; }
        public double TraverseDistance { get; }

        public int CompareTo(PathNode other)
            => estimatedTotalCost.CompareTo(other.estimatedTotalCost);
    }
}