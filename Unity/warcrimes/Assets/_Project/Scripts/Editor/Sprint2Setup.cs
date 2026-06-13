using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WarCrimes.Editor
{
    public static class Sprint2Setup
    {
        [MenuItem("WarCrimes/Create Sprint 2 Assets & Scene")]
        public static void CreateAll()
        {
            var whiteSprite = CreateOrLoadWhiteSprite();
            var tilePrefab  = CreateOrLoadTilePrefab(whiteSprite);
            CreateTestMap01();
            CreateGameScene(tilePrefab);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Sprint 2 setup complete. Open Assets/_Project/Scenes/Game.unity and press Play.");
        }

        // ── White pixel sprite ───────────────────────────────────────────────────

        static Sprite CreateOrLoadWhiteSprite()
        {
            const string assetPath = "Assets/_Project/Art/white_pixel.png";
            var existing = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (existing != null) return existing;

            var tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
            var pixels = new Color32[16];
            for (int i = 0; i < 16; i++) pixels[i] = new Color32(255, 255, 255, 255);
            tex.SetPixels32(pixels);
            tex.Apply();
            byte[] png = tex.EncodeToPNG();
            Object.DestroyImmediate(tex);

            File.WriteAllBytes(
                Path.Combine(Application.dataPath, "_Project/Art/white_pixel.png"), png);
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);

            var importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
            importer.textureType          = TextureImporterType.Sprite;
            importer.spriteImportMode     = SpriteImportMode.Single;
            importer.spritePivot          = new Vector2(0.5f, 0.5f);
            importer.spritePixelsPerUnit  = 4f;   // 4px texture → 1 Unity unit
            importer.filterMode           = FilterMode.Point;
            importer.textureCompression   = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();

            return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        }

        // ── Tile visual prefab ───────────────────────────────────────────────────

        static GameObject CreateOrLoadTilePrefab(Sprite whiteSprite)
        {
            const string path = "Assets/_Project/Prefabs/TileVisual.prefab";
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null) return existing;

            var go = new GameObject("TileVisual");
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = whiteSprite;

            var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            return prefab;
        }

        // ── TestMap01 ─────────────────────────────────────────────────────────────

        static void CreateTestMap01()
        {
            const string path = "Assets/_Project/ScriptableObjects/Maps/TestMap01.asset";
            if (AssetDatabase.LoadAssetAtPath<MapData>(path) != null) return;

            // 0=Plains 1=Road 2=Forest 3=Mountain 4=City 5=HQ 6=Factory
            // Row-major: index = y*8 + x,  y=0 is bottom of map
            int[] layout =
            {
                /* y=0 */ 5, 0, 2, 3, 3, 2, 0, 0,
                /* y=1 */ 0, 0, 0, 0, 0, 0, 0, 0,
                /* y=2 */ 4, 0, 1, 1, 1, 1, 0, 4,
                /* y=3 */ 0, 0, 1, 2, 2, 1, 0, 0,
                /* y=4 */ 0, 0, 1, 2, 2, 1, 0, 0,
                /* y=5 */ 4, 0, 1, 1, 1, 1, 0, 4,
                /* y=6 */ 0, 0, 0, 0, 0, 0, 0, 0,
                /* y=7 */ 0, 0, 2, 3, 3, 2, 0, 5,
            };

            string[] terrainPaths =
            {
                "Assets/_Project/ScriptableObjects/Terrain/Plains.asset",
                "Assets/_Project/ScriptableObjects/Terrain/Road.asset",
                "Assets/_Project/ScriptableObjects/Terrain/Forest.asset",
                "Assets/_Project/ScriptableObjects/Terrain/Mountain.asset",
                "Assets/_Project/ScriptableObjects/Terrain/City.asset",
                "Assets/_Project/ScriptableObjects/Terrain/HQ.asset",
                "Assets/_Project/ScriptableObjects/Terrain/Factory.asset",
            };

            var terrainAssets = new TileData[terrainPaths.Length];
            for (int i = 0; i < terrainPaths.Length; i++)
                terrainAssets[i] = AssetDatabase.LoadAssetAtPath<TileData>(terrainPaths[i]);

            var tiles = new TileData[64];
            for (int i = 0; i < 64; i++)
                tiles[i] = terrainAssets[layout[i]];

            var infantryData = AssetDatabase.LoadAssetAtPath<UnitData>("Assets/_Project/ScriptableObjects/Units/Infantry.asset");
            var tankData     = AssetDatabase.LoadAssetAtPath<UnitData>("Assets/_Project/ScriptableObjects/Units/Tank.asset");

            var map = ScriptableObject.CreateInstance<MapData>();
            map.width  = 8;
            map.height = 8;
            map.tiles  = tiles;
            map.startingUnits = new[]
            {
                new UnitPlacement { unitData = infantryData, playerIndex = 0, gridPosition = new Vector2Int(1, 0) },
                new UnitPlacement { unitData = tankData,     playerIndex = 0, gridPosition = new Vector2Int(1, 1) },
                new UnitPlacement { unitData = infantryData, playerIndex = 1, gridPosition = new Vector2Int(6, 7) },
                new UnitPlacement { unitData = tankData,     playerIndex = 1, gridPosition = new Vector2Int(6, 6) },
            };

            AssetDatabase.CreateAsset(map, path);
        }

        // ── Game scene ────────────────────────────────────────────────────────────

        static void CreateGameScene(GameObject tilePrefab)
        {
            const string scenePath = "Assets/_Project/Scenes/Game.unity";
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath) != null)
            {
                Debug.Log("Game.unity already exists — skipping.");
                return;
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            EditorSceneManager.SetActiveScene(scene);

            // GridManager
            new GameObject("GridManager").AddComponent<GridManager>();

            // UnitManager + prefab map
            var unitManagerGo = new GameObject("UnitManager");
            var unitManager   = unitManagerGo.AddComponent<UnitManager>();

            var infantryData = AssetDatabase.LoadAssetAtPath<UnitData>("Assets/_Project/ScriptableObjects/Units/Infantry.asset");
            var mechData     = AssetDatabase.LoadAssetAtPath<UnitData>("Assets/_Project/ScriptableObjects/Units/Mech.asset");
            var reconData    = AssetDatabase.LoadAssetAtPath<UnitData>("Assets/_Project/ScriptableObjects/Units/Recon.asset");
            var tankData     = AssetDatabase.LoadAssetAtPath<UnitData>("Assets/_Project/ScriptableObjects/Units/Tank.asset");

            var umSo = new SerializedObject(unitManager);
            var mapProp = umSo.FindProperty("prefabMap");
            mapProp.arraySize = 4;
            SetPrefabEntry(mapProp.GetArrayElementAtIndex(0), infantryData, "Infantry");
            SetPrefabEntry(mapProp.GetArrayElementAtIndex(1), mechData,     "Mech");
            SetPrefabEntry(mapProp.GetArrayElementAtIndex(2), reconData,    "Recon");
            SetPrefabEntry(mapProp.GetArrayElementAtIndex(3), tankData,     "Tank");
            umSo.ApplyModifiedPropertiesWithoutUndo();

            // Tile parent
            var tilesGo = new GameObject("Tiles");

            // Camera
            var cameraGo = new GameObject("Main Camera");
            cameraGo.tag = "MainCamera";
            var cam = cameraGo.AddComponent<Camera>();
            cam.orthographic      = true;
            cam.orthographicSize  = 5f;
            cam.clearFlags        = CameraClearFlags.SolidColor;
            cam.backgroundColor   = new Color(0.08f, 0.08f, 0.12f);
            cameraGo.transform.position = new Vector3(4f, 4f, -10f);
            var camController = cameraGo.AddComponent<CameraController>();

            // Global Light 2D so sprites aren't black under URP 2D lighting
            var lightGo = new GameObject("Global Light 2D");
            var light2d = lightGo.AddComponent<UnityEngine.Rendering.Universal.Light2D>();
            light2d.lightType = UnityEngine.Rendering.Universal.Light2D.LightType.Global;
            light2d.intensity = 1f;

            // MapLoader
            var mapLoaderGo = new GameObject("MapLoader");
            var mapLoader   = mapLoaderGo.AddComponent<MapLoader>();

            var mlSo = new SerializedObject(mapLoader);
            mlSo.FindProperty("mapData").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<MapData>("Assets/_Project/ScriptableObjects/Maps/TestMap01.asset");
            mlSo.FindProperty("tilePrefab").objectReferenceValue = tilePrefab;
            mlSo.FindProperty("tileParent").objectReferenceValue = tilesGo.transform;
            mlSo.FindProperty("cameraController").objectReferenceValue = camController;
            mlSo.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.SaveScene(scene, scenePath);
        }

        static void SetPrefabEntry(SerializedProperty element, UnitData data, string unitName)
        {
            element.FindPropertyRelative("data").objectReferenceValue = data;
            element.FindPropertyRelative("prefab").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/_Project/Prefabs/Units/{unitName}.prefab");
        }
    }
}
