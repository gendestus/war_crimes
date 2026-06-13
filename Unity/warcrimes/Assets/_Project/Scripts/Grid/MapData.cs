using System;
using UnityEngine;

namespace WarCrimes
{
    [Serializable]
    public struct UnitPlacement
    {
        public UnitData unitData;
        public int playerIndex;
        public Vector2Int gridPosition;
    }

    [CreateAssetMenu(fileName = "NewMap", menuName = "WarCrimes/Map Data")]
    public class MapData : ScriptableObject
    {
        public int width;
        public int height;
        public TileData[] tiles;       // row-major, length == width * height
        public UnitPlacement[] startingUnits;
    }
}
