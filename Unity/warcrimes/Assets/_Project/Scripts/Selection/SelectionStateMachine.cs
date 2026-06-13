using System.Collections.Generic;
using UnityEngine;

namespace WarCrimes
{
    public class SelectionStateMachine : MonoBehaviour
    {
        [SerializeField] private MovementHighlighter highlighter;
        [SerializeField] private UnitMover unitMover;
        [SerializeField] private ActionMenu actionMenu;

        private SelectionState state = SelectionState.Idle;
        private Unit selectedUnit;
        private Dictionary<Vector2Int, List<Vector2Int>> reachable;

        void OnEnable()  => GameEvents.OnTilePointed += HandleTilePointed;
        void OnDisable() => GameEvents.OnTilePointed -= HandleTilePointed;

        void HandleTilePointed(Vector2Int gridPos)
        {
            switch (state)
            {
                case SelectionState.Idle:         HandleIdle(gridPos);         break;
                case SelectionState.UnitSelected: HandleUnitSelected(gridPos); break;
                // MoveTarget / Animating: ignore tile input
            }
        }

        void HandleIdle(Vector2Int gridPos)
        {
            var tile = GridManager.Instance.GetTile(gridPos);
            var unit = tile?.Occupant;
            if (unit == null || !unit.CanAct || unit.PlayerIndex != TurnManager.Instance.ActivePlayer)
                return;

            selectedUnit = unit;
            reachable = Pathfinding.ComputeReachable(
                gridPos, unit.data.moveRange, unit.data.moveType, unit.PlayerIndex);
            highlighter.Show(reachable.Keys);
            state = SelectionState.UnitSelected;
        }

        void HandleUnitSelected(Vector2Int gridPos)
        {
            if (reachable.TryGetValue(gridPos, out var path))
            {
                highlighter.Clear();
                state = SelectionState.Animating;
                GameStateMachine.Instance.ChangeState(GameState.Animating);
                unitMover.Move(selectedUnit, path, () => OnMoveComplete(gridPos));
            }
            else
            {
                // Clicked outside range — deselect (re-check if clicking a different friendly unit)
                highlighter.Clear();
                selectedUnit = null;
                reachable = null;
                state = SelectionState.Idle;
                HandleIdle(gridPos); // immediately try selecting whatever was clicked
            }
        }

        void OnMoveComplete(Vector2Int destination)
        {
            state = SelectionState.MoveTarget;
            GameStateMachine.Instance.ChangeState(GameState.PlayerTurn);
            actionMenu.Show(selectedUnit, OnActionComplete);
        }

        void OnActionComplete()
        {
            selectedUnit = null;
            reachable = null;
            state = SelectionState.Idle;
        }
    }
}
