using UnityEngine;
using UnityEngine.InputSystem;

namespace WarCrimes
{
    // Converts mouse clicks to grid events. Drag detection prevents firing during camera pan.
    public class InputRouter : MonoBehaviour
    {
        [SerializeField] private float clickMaxPixelDrift = 5f;

        private Vector2 pressScreenPos;

        void Update()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;

            if (mouse.leftButton.wasPressedThisFrame)
                pressScreenPos = mouse.position.ReadValue();

            if (mouse.leftButton.wasReleasedThisFrame)
            {
                Vector2 releasePos = mouse.position.ReadValue();
                if (Vector2.Distance(pressScreenPos, releasePos) <= clickMaxPixelDrift)
                    HandleClick(releasePos);
            }
        }

        void HandleClick(Vector2 screenPos)
        {
            var cam = Camera.main;
            if (cam == null) return;

            var worldPos = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -cam.transform.position.z));
            var gridPos = GridManager.Instance.WorldToGrid(worldPos);

            if (GridManager.Instance.IsInBounds(gridPos))
                GameEvents.TilePointed(gridPos);
        }
    }
}
