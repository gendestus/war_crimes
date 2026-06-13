using UnityEngine;

namespace WarCrimes
{
    [CreateAssetMenu(fileName = "DamageTable", menuName = "WarCrimes/Damage Table")]
    public class DamageTable : ScriptableObject
    {
        public int unitTypeCount = 4;

        // Row = attacker index, Col = defender index (row-major)
        // Order: 0=Infantry, 1=Mech, 2=Recon, 3=Tank
        public float[] values;

        public float Get(int attackerIndex, int defenderIndex)
        {
            return values[attackerIndex * unitTypeCount + defenderIndex];
        }
    }
}
