using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarCrimes
{
    public class UnitMover : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 8f;

        public void Move(Unit unit, List<Vector2Int> path, Action onComplete)
        {
            if (path == null || path.Count == 0)
            {
                onComplete?.Invoke();
                return;
            }
            StartCoroutine(MoveCoroutine(unit, path, onComplete));
        }

        IEnumerator MoveCoroutine(Unit unit, List<Vector2Int> path, Action onComplete)
        {
            unit.SetVisualState(UnitVisualState.Move);

            var startTile = GridManager.Instance.GetTile(unit.GridPosition);
            if (startTile != null) startTile.Occupant = null;

            bool alt = false;
            foreach (var step in path)
            {
                unit.SetVisualState(alt ? UnitVisualState.MoveAlt : UnitVisualState.Move);
                alt = !alt;

                var target = GridManager.Instance.GridToWorld(step);
                while (Vector3.Distance(unit.transform.position, target) > 0.01f)
                {
                    unit.transform.position = Vector3.MoveTowards(
                        unit.transform.position, target, moveSpeed * Time.deltaTime);
                    yield return null;
                }
                unit.transform.position = target;
            }

            var dest = path[path.Count - 1];
            unit.GridPosition = dest;
            var destTile = GridManager.Instance.GetTile(dest);
            if (destTile != null) destTile.Occupant = unit;

            unit.SetVisualState(UnitVisualState.Idle);
            onComplete?.Invoke();
        }
    }
}
