using System.Collections.Generic;
using UnityEngine;

namespace WarCrimes
{
    public static class Pathfinding
    {
        static readonly Vector2Int[] Dirs =
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        // Returns every tile the unit can stop on, mapped to the path from origin (excluding origin).
        // Origin itself is included with an empty path (stay-in-place / Wait).
        public static Dictionary<Vector2Int, List<Vector2Int>> ComputeReachable(
            Vector2Int origin, int moveRange, MovementType moveType, int playerIndex)
        {
            // bestFuel[pos] = highest remaining fuel we've found to reach pos
            var bestFuel = new Dictionary<Vector2Int, int> { [origin] = moveRange };
            var parents = new Dictionary<Vector2Int, Vector2Int>();
            var queue = new Queue<Vector2Int>();
            queue.Enqueue(origin);

            while (queue.Count > 0)
            {
                var pos = queue.Dequeue();
                int fuel = bestFuel[pos];

                foreach (var dir in Dirs)
                {
                    var next = pos + dir;
                    if (!GridManager.Instance.IsInBounds(next)) continue;

                    var tile = GridManager.Instance.GetTile(next);
                    int cost = MoveCost(tile.Data, moveType);
                    if (cost < 0) continue; // impassable terrain

                    // Can't pass through enemy units
                    if (tile.Occupant != null && tile.Occupant.PlayerIndex != playerIndex) continue;

                    int remaining = fuel - cost;
                    if (remaining < 0) continue;

                    if (!bestFuel.TryGetValue(next, out int prev) || remaining > prev)
                    {
                        bestFuel[next] = remaining;
                        parents[next] = pos;
                        queue.Enqueue(next);
                    }
                }
            }

            var result = new Dictionary<Vector2Int, List<Vector2Int>>();

            // Stay in place (empty path)
            result[origin] = new List<Vector2Int>();

            foreach (var pos in bestFuel.Keys)
            {
                if (pos == origin) continue;
                var tile = GridManager.Instance.GetTile(pos);
                if (tile.Occupant != null) continue; // can't stop on occupied tile
                result[pos] = BuildPath(pos, origin, parents);
            }

            return result;
        }

        static List<Vector2Int> BuildPath(
            Vector2Int dest, Vector2Int origin, Dictionary<Vector2Int, Vector2Int> parents)
        {
            var path = new List<Vector2Int>();
            var cur = dest;
            while (cur != origin)
            {
                path.Add(cur);
                cur = parents[cur];
            }
            path.Reverse();
            return path;
        }

        static int MoveCost(TileData tile, MovementType type) => type switch
        {
            MovementType.Foot  => tile.footMoveCost,
            MovementType.Wheel => tile.wheelMoveCost,
            MovementType.Tread => tile.treadMoveCost,
            _                  => 1,
        };
    }
}
