using System.Collections.Generic;
using UnityEngine;

namespace WarCrimes
{
    public class MovementHighlighter : MonoBehaviour
    {
        [SerializeField] private Sprite highlightSprite;
        [SerializeField] private Transform highlightParent;

        private readonly List<GameObject> active = new();

        public void Show(IEnumerable<Vector2Int> tiles)
        {
            Clear();
            foreach (var pos in tiles)
            {
                var go = new GameObject("Highlight");
                go.transform.SetParent(highlightParent, false);
                go.transform.position = GridManager.Instance.GridToWorld(pos);

                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = highlightSprite;
                sr.color = new Color(0.2f, 0.5f, 1f, 0.45f);
                sr.sortingOrder = 1; // above tiles (0), below units (2)

                active.Add(go);
            }
        }

        public void Clear()
        {
            foreach (var go in active)
                if (go != null) Destroy(go);
            active.Clear();
        }
    }
}
