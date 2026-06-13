using UnityEngine;

namespace WarCrimes
{
    [CreateAssetMenu(fileName = "NewTile", menuName = "WarCrimes/Tile Data")]
    public class TileData : ScriptableObject
    {
        public string terrainName;
        public int defenseStars;
        public int footMoveCost;
        public int wheelMoveCost;  // -1 = impassable
        public int treadMoveCost;  // -1 = impassable
        public bool isCapturable;
        public Sprite sprite;
    }
}
