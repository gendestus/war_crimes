using UnityEngine;

namespace WarCrimes
{
    public class Tile
    {
        public TileData Data { get; }
        public Vector2Int GridPosition { get; }
        public Unit Occupant { get; set; }
        public int CaptureProgress { get; set; }
        public int OwnerPlayerIndex { get; set; }

        public Tile(TileData data, Vector2Int gridPosition)
        {
            Data = data;
            GridPosition = gridPosition;
            OwnerPlayerIndex = -1;
        }
    }
}
