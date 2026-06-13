using UnityEngine;

namespace WarCrimes
{
    public class TurnManager : MonoBehaviour
    {
        public static TurnManager Instance { get; private set; }

        public int ActivePlayer { get; private set; }
        public int TurnCount { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Start() => BeginTurn(0);

        public void BeginTurn(int playerIndex)
        {
            ActivePlayer = playerIndex;
            TurnCount++;

            foreach (var unit in UnitManager.Instance.GetUnits(playerIndex))
            {
                unit.CanAct = true;

                var tile = GridManager.Instance.GetTile(unit.GridPosition);
                if (tile != null && tile.OwnerPlayerIndex == playerIndex && tile.Data.isCapturable)
                    unit.ReplenishSupplies();
            }

            GameStateMachine.Instance.ChangeState(GameState.PlayerTurn);
            GameEvents.TurnBegin(playerIndex);
        }

        public void EndTurn()
        {
            foreach (var unit in UnitManager.Instance.GetUnits(ActivePlayer))
                unit.CanAct = false;

            BeginTurn((ActivePlayer + 1) % 2);
        }
    }
}
