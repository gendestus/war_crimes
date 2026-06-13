using UnityEngine;

namespace WarCrimes
{
    [CreateAssetMenu(fileName = "NewUnit", menuName = "WarCrimes/Unit Data")]
    public class UnitData : ScriptableObject
    {
        public string unitName;
        public int moveRange;
        public MovementType moveType;
        public int attackRange;
        public int maxHP = 100;
        public int maxAmmo = 9;
        public int maxFuel = 99;
        public bool canCapture;
        public Sprite idleSprite;
    }
}
