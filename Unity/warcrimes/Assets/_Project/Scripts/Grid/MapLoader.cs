using UnityEngine;

namespace WarCrimes
{
    public class MapLoader : MonoBehaviour
    {
        [SerializeField] private MapData mapData;
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private Transform tileParent;
        [SerializeField] private CameraController cameraController;

        void Start()
        {
            if (mapData == null) { Debug.LogError("MapLoader: no MapData assigned."); return; }
            LoadMap();
        }

        void LoadMap()
        {
            GridManager.Instance.Initialize(mapData.width, mapData.height);

            for (int y = 0; y < mapData.height; y++)
            {
                for (int x = 0; x < mapData.width; x++)
                {
                    var tileData = mapData.tiles[y * mapData.width + x];
                    GridManager.Instance.SetTile(x, y, new Tile(tileData, new Vector2Int(x, y)));
                    SpawnTileVisual(tileData, x, y);
                }
            }

            foreach (var placement in mapData.startingUnits)
                UnitManager.Instance.SpawnUnit(placement.unitData, placement.playerIndex, placement.gridPosition);

            if (cameraController != null)
            {
                cameraController.SetBounds(GridManager.Instance.MapBoundsMin, GridManager.Instance.MapBoundsMax);
                var center = (GridManager.Instance.MapBoundsMin + GridManager.Instance.MapBoundsMax) / 2f;
                cameraController.CenterOn(center);
            }
        }

        void SpawnTileVisual(TileData data, int x, int y)
        {
            var worldPos = GridManager.Instance.GridToWorld(x, y);
            var go = Instantiate(tilePrefab, worldPos, Quaternion.identity, tileParent);
            go.name = $"Tile_{x}_{y}";

            var sr = go.GetComponent<SpriteRenderer>();
            if (sr == null) return;

            if (data.sprite != null)
                sr.sprite = data.sprite;
            else
                sr.color = PlaceholderColor(data.terrainName);
        }

        static Color PlaceholderColor(string terrainName) => terrainName switch
        {
            "Plains"   => new Color(0.55f, 0.76f, 0.29f),
            "Road"     => new Color(0.60f, 0.60f, 0.60f),
            "Forest"   => new Color(0.13f, 0.45f, 0.13f),
            "Mountain" => new Color(0.50f, 0.40f, 0.30f),
            "City"     => new Color(0.85f, 0.85f, 0.55f),
            "HQ"       => new Color(0.95f, 0.85f, 0.20f),
            "Factory"  => new Color(0.75f, 0.55f, 0.75f),
            _          => Color.white,
        };
    }
}
