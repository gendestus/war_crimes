using UnityEngine;

namespace WarCrimes
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }

        public float TileSize { get; private set; } = 1f;
        public int Width { get; private set; }
        public int Height { get; private set; }

        private Tile[,] grid;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void Initialize(int width, int height, float tileSize = 1f)
        {
            Width = width;
            Height = height;
            TileSize = tileSize;
            grid = new Tile[width, height];
        }

        public void SetTile(int x, int y, Tile tile) => grid[x, y] = tile;
        public Tile GetTile(int x, int y) => IsInBounds(x, y) ? grid[x, y] : null;
        public Tile GetTile(Vector2Int pos) => GetTile(pos.x, pos.y);
        public bool IsInBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;
        public bool IsInBounds(Vector2Int pos) => IsInBounds(pos.x, pos.y);

        // Tiles are centered: (0,0) → world (0.5, 0.5), so sprites with center pivot sit flush
        public Vector3 GridToWorld(int x, int y) =>
            new Vector3((x + 0.5f) * TileSize, (y + 0.5f) * TileSize, 0f);
        public Vector3 GridToWorld(Vector2Int pos) => GridToWorld(pos.x, pos.y);

        public Vector2Int WorldToGrid(Vector3 worldPos) =>
            new Vector2Int(
                Mathf.FloorToInt(worldPos.x / TileSize),
                Mathf.FloorToInt(worldPos.y / TileSize));

        public Vector2 MapBoundsMin => Vector2.zero;
        public Vector2 MapBoundsMax => new Vector2(Width * TileSize, Height * TileSize);
    }
}
