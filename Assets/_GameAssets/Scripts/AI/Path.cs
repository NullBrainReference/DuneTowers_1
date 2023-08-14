using System;
using System.Collections.Generic;
//https://bitbucket.org/Xamtos/randomstuff/src/master/Projects/AI/A_Star/Path.cs
//https://habr.com/ru/post/513158/

namespace AI.A_Star
{
    /// <summary>
    /// Reusable A* path finder.
    /// </summary>
    public class Path : IPath
    {
        private const int MaxNeighbours = 8;
        private readonly PathNode[] neighbours = new PathNode[MaxNeighbours];

        private readonly int maxSteps;
        private readonly IBinaryHeap<Vector2Int, PathNode> frontier;
        private readonly HashSet<Vector2Int> ignoredPositions;
        private readonly List<Vector2Int> output;
        private readonly IDictionary<Vector2Int, Vector2Int> links;

        /// <summary>
        /// Creation of new path finder.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Path(int maxSteps = int.MaxValue, int initialCapacity = 0)
        {
            if (maxSteps <= 0) throw new ArgumentOutOfRangeException(nameof(maxSteps));
            if (initialCapacity < 0) throw new ArgumentOutOfRangeException(nameof(initialCapacity));

            this.maxSteps = maxSteps;
            frontier = new BinaryHeap<Vector2Int, PathNode>(a => a.Position, initialCapacity);
            ignoredPositions = new HashSet<Vector2Int>(initialCapacity);
            output = new List<Vector2Int>(initialCapacity);
            links = new Dictionary<Vector2Int, Vector2Int>(initialCapacity);
        }

        /// <summary>
        /// Calculate a new path between two points.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public bool Calculate(Vector2Int start, Vector2Int target, IReadOnlyCollection<Vector2Int> obstacles, out IReadOnlyCollection<Vector2Int> path)
        {
            if (obstacles == null) throw new ArgumentNullException(nameof(obstacles));

            if (!GenerateNodes(start, target, obstacles))
            {
                path = Array.Empty<Vector2Int>();
                return false;
            }

            output.Clear();
            output.Add(target);

            while (links.TryGetValue(target, out target)) output.Add(target);
            path = output;
            return true;
        }

        private bool GenerateNodes(Vector2Int start, Vector2Int target, IReadOnlyCollection<Vector2Int> obstacles)
        {
            frontier.Clear();
            ignoredPositions.Clear();
            links.Clear();

            frontier.Enqueue(new PathNode(start, target, 0));
            ignoredPositions.UnionWith(obstacles);
            var step = 0;
            while (frontier.Count > 0 && step++ <= maxSteps)
            {
                PathNode current = frontier.Dequeue();
                ignoredPositions.Add(current.Position);

                if (current.Position.Equals(target)) return true;

                GenerateFrontierNodes(current, target);
            }

            // All nodes analyzed - no path detected.
            return false;
        }

        private void GenerateFrontierNodes(PathNode parent, Vector2Int target)
        {
            neighbours.Fill(parent, target);
            foreach (PathNode newNode in neighbours)
            {
                // Position is already checked or occupied by an obstacle.
                if (ignoredPositions.Contains(newNode.Position)) continue;

                // Node is not present in queue.
                if (!frontier.TryGet(newNode.Position, out PathNode existingNode))
                {
                    frontier.Enqueue(newNode);
                    links[newNode.Position] = parent.Position;
                }

                // Node is present in queue and new optimal path is detected.
                else if (newNode.TraverseDistance < existingNode.TraverseDistance)
                {
                    frontier.Modify(newNode);
                    links[newNode.Position] = parent.Position;
                }
            }
        }
    }
}