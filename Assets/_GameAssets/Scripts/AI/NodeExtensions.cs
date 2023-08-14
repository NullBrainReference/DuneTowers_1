using System;

namespace AI.A_Star
{
    internal static class NodeExtensions
    {
        private static readonly (Vector2Int position, double cost)[] NeighboursTemplate = {
            (new Vector2Int(1, 0), 1),
            (new Vector2Int(0, 1), 1),
            (new Vector2Int(-1, 0), 1),
            (new Vector2Int(0, -1), 1),
            (new Vector2Int(1, 1), Math.Sqrt(2)),
            (new Vector2Int(1, -1), Math.Sqrt(2)),
            (new Vector2Int(-1, 1), Math.Sqrt(2)),
            (new Vector2Int(-1, -1), Math.Sqrt(2))
        };

        public static void Fill(this PathNode[] buffer, PathNode parent, Vector2Int target)
        {
            int i = 0;
            foreach ((Vector2Int position, double cost) in NeighboursTemplate)
            {
                Vector2Int nodePosition = position + parent.Position;
                double traverseDistance = parent.TraverseDistance + cost;
                buffer[i++] = new PathNode(nodePosition, target, traverseDistance);
            }
        }
    }
}