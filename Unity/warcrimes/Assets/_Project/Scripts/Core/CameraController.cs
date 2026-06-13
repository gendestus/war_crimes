using UnityEngine;
using UnityEngine.InputSystem;

namespace WarCrimes
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float padding = 1f;

        private Camera cam;
        private Vector2 boundsMin;
        private Vector2 boundsMax;
        private bool boundsSet;

        private bool dragging;
        private Vector2 dragScreenOrigin;
        private Vector3 camPositionAtDragStart;

        void Awake() => cam = GetComponent<Camera>();

        public void SetBounds(Vector2 min, Vector2 max)
        {
            boundsMin = min;
            boundsMax = max;
            boundsSet = true;
        }

        public void CenterOn(Vector2 worldCenter)
        {
            transform.position = new Vector3(worldCenter.x, worldCenter.y, transform.position.z);
            if (boundsSet) ClampPosition();
        }

        void Update()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;

            if (mouse.leftButton.wasPressedThisFrame)
            {
                dragScreenOrigin = mouse.position.ReadValue();
                camPositionAtDragStart = transform.position;
                dragging = true;
            }

            if (mouse.leftButton.wasReleasedThisFrame)
                dragging = false;

            if (dragging)
            {
                Vector2 screenDelta = (Vector2)mouse.position.ReadValue() - dragScreenOrigin;
                float unitsPerPixel = 2f * cam.orthographicSize / Screen.height;
                transform.position = camPositionAtDragStart + new Vector3(-screenDelta.x, -screenDelta.y, 0f) * unitsPerPixel;
            }

            if (boundsSet) ClampPosition();
        }

        void ClampPosition()
        {
            float halfH = cam.orthographicSize;
            float halfW = cam.orthographicSize * cam.aspect;

            float minX = boundsMin.x - padding + halfW;
            float maxX = boundsMax.x + padding - halfW;
            float minY = boundsMin.y - padding + halfH;
            float maxY = boundsMax.y + padding - halfH;

            // If the map is smaller than the viewport, center it instead of clamping
            float x = minX < maxX ? Mathf.Clamp(transform.position.x, minX, maxX) : (boundsMin.x + boundsMax.x) * 0.5f;
            float y = minY < maxY ? Mathf.Clamp(transform.position.y, minY, maxY) : (boundsMin.y + boundsMax.y) * 0.5f;

            transform.position = new Vector3(x, y, transform.position.z);
        }
    }
}
