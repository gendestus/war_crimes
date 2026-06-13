using System.Collections.Generic;
using UnityEngine;

namespace WarCrimes
{
    [System.Serializable]
    public struct UnitPrefabEntry
    {
        public UnitData data;
        public GameObject prefab;
    }

    public class UnitManager : MonoBehaviour
    {
        public static UnitManager Instance { get; private set; }

        [SerializeField] private UnitPrefabEntry[] prefabMap;

        private readonly List<Unit>[] unitsByPlayer = new List<Unit>[2];

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            unitsByPlayer[0] = new List<Unit>();
            unitsByPlayer[1] = new List<Unit>();
        }

        public Unit SpawnUnit(UnitData data, int playerIndex, Vector2Int gridPos)
        {
            var prefab = FindPrefab(data);
            if (prefab == null)
            {
                Debug.LogWarning($"No prefab found for unit: {data.unitName}");
                return null;
            }

            var worldPos = GridManager.Instance.GridToWorld(gridPos);
            var go = Instantiate(prefab, worldPos, Quaternion.identity, transform);
            var unit = go.GetComponent<Unit>();
            unit.Initialize(data, playerIndex);
            unit.GridPosition = gridPos;

            unitsByPlayer[playerIndex].Add(unit);

            var tile = GridManager.Instance.GetTile(gridPos);
            if (tile != null) tile.Occupant = unit;

            return unit;
        }

        public void RemoveUnit(Unit unit)
        {
            unitsByPlayer[unit.PlayerIndex].Remove(unit);
            var tile = GridManager.Instance.GetTile(unit.GridPosition);
            if (tile?.Occupant == unit) tile.Occupant = null;
            Destroy(unit.gameObject);
        }

        public List<Unit> GetUnits(int playerIndex) => unitsByPlayer[playerIndex];

        private GameObject FindPrefab(UnitData data)
        {
            foreach (var entry in prefabMap)
                if (entry.data == data) return entry.prefab;
            return null;
        }
    }
}
