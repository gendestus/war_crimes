using UnityEngine;
using UnityEditor;

namespace WarCrimes.Editor
{
    public static class Sprint1AssetCreator
    {
        [MenuItem("WarCrimes/Create Sprint 1 Assets")]
        public static void CreateAll()
        {
            CreateUnitAssets();
            CreateTerrainAssets();
            CreateDamageTable();
            CreateUnitPrefabs();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Sprint 1 assets created.");
        }

        // ── Unit assets ──────────────────────────────────────────────────────────

        static void CreateUnitAssets()
        {
            CreateUnit("Infantry", 3, MovementType.Foot,  1, true,  3, 99);
            CreateUnit("Mech",     2, MovementType.Foot,  1, true,  3, 70);
            CreateUnit("Recon",    8, MovementType.Wheel, 1, false, 6, 80);
            CreateUnit("Tank",     6, MovementType.Tread, 1, false, 9, 70);
        }

        static void CreateUnit(string unitName, int move, MovementType type, int range,
                               bool canCapture, int ammo, int fuel)
        {
            string path = $"Assets/_Project/ScriptableObjects/Units/{unitName}.asset";
            if (AssetDatabase.LoadAssetAtPath<UnitData>(path) != null) return;

            var data = ScriptableObject.CreateInstance<UnitData>();
            data.unitName   = unitName;
            data.moveRange  = move;
            data.moveType   = type;
            data.attackRange = range;
            data.maxHP      = 100;
            data.maxAmmo    = ammo;
            data.maxFuel    = fuel;
            data.canCapture = canCapture;

            AssetDatabase.CreateAsset(data, path);
        }

        // ── Terrain assets ───────────────────────────────────────────────────────

        static void CreateTerrainAssets()
        {
            // (name, defStars, footCost, wheelCost, treadCost, capturable)
            // -1 = impassable
            CreateTerrain("Plains",   1, 1, 2,  1,  false);
            CreateTerrain("Road",     0, 1, 1,  1,  false);
            CreateTerrain("Forest",   2, 1, 3,  2,  false);
            CreateTerrain("Mountain", 4, 2, -1, -1, false);
            CreateTerrain("City",     3, 1, 1,  1,  true);
            CreateTerrain("HQ",       4, 1, 1,  1,  true);
            CreateTerrain("Factory",  3, 1, 1,  1,  true);
        }

        static void CreateTerrain(string terrainName, int def, int foot, int wheel, int tread,
                                  bool capturable)
        {
            string path = $"Assets/_Project/ScriptableObjects/Terrain/{terrainName}.asset";
            if (AssetDatabase.LoadAssetAtPath<TileData>(path) != null) return;

            var data = ScriptableObject.CreateInstance<TileData>();
            data.terrainName   = terrainName;
            data.defenseStars  = def;
            data.footMoveCost  = foot;
            data.wheelMoveCost = wheel;
            data.treadMoveCost = tread;
            data.isCapturable  = capturable;

            AssetDatabase.CreateAsset(data, path);
        }

        // ── Damage table ─────────────────────────────────────────────────────────

        static void CreateDamageTable()
        {
            string path = "Assets/_Project/ScriptableObjects/DamageTable.asset";
            if (AssetDatabase.LoadAssetAtPath<DamageTable>(path) != null) return;

            // Row = attacker, Col = defender
            // Order: 0=Infantry, 1=Mech, 2=Recon, 3=Tank
            float[] values =
            {
                // vs Infantry  Mech  Recon  Tank
                /* Infantry */  55,   45,    12,    5,
                /* Mech     */  65,   55,    18,   55,
                /* Recon    */  35,   28,    35,    6,
                /* Tank     */  75,   70,    85,   55,
            };

            var table = ScriptableObject.CreateInstance<DamageTable>();
            table.unitTypeCount = 4;
            table.values = values;

            AssetDatabase.CreateAsset(table, path);
        }

        // ── Unit prefabs ─────────────────────────────────────────────────────────

        static void CreateUnitPrefabs()
        {
            string[] names = { "Infantry", "Mech", "Recon", "Tank" };
            foreach (var name in names)
                CreateUnitPrefab(name);
        }

        static void CreateUnitPrefab(string unitName)
        {
            string path = $"Assets/_Project/Prefabs/Units/{unitName}.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

            // Root: Unit MonoBehaviour + SpriteRenderer
            var root = new GameObject(unitName);
            root.AddComponent<SpriteRenderer>();
            var unit = root.AddComponent<Unit>();

            // Wire up UnitData
            var unitData = AssetDatabase.LoadAssetAtPath<UnitData>(
                $"Assets/_Project/ScriptableObjects/Units/{unitName}.asset");
            if (unitData != null) unit.data = unitData;

            // Health bar: world-space canvas with HealthBar component
            var canvasGo = new GameObject("HealthBarCanvas");
            canvasGo.transform.SetParent(root.transform);
            canvasGo.transform.localPosition = new Vector3(0, 0.6f, 0);

            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            var rt = canvasGo.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(1f, 0.15f);
            rt.localScale = Vector3.one * 0.01f;

            // Fill image
            var fillGo = new GameObject("Fill");
            fillGo.transform.SetParent(canvasGo.transform);
            var fillImg = fillGo.AddComponent<UnityEngine.UI.Image>();
            fillImg.color = Color.green;
            var fillRt = fillGo.GetComponent<RectTransform>();
            fillRt.anchorMin = Vector2.zero;
            fillRt.anchorMax = Vector2.one;
            fillRt.offsetMin = fillRt.offsetMax = Vector2.zero;
            fillImg.type = UnityEngine.UI.Image.Type.Filled;
            fillImg.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
            fillImg.fillAmount = 1f;

            // HP text
            var textGo = new GameObject("HPText");
            textGo.transform.SetParent(canvasGo.transform);
            var text = textGo.AddComponent<TMPro.TextMeshProUGUI>();
            text.text = "100";
            text.fontSize = 8;
            text.alignment = TMPro.TextAlignmentOptions.Center;
            var textRt = textGo.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = textRt.offsetMax = Vector2.zero;

            // Wire HealthBar
            var hb = canvasGo.AddComponent<HealthBar>();
            var so = new SerializedObject(hb);
            so.FindProperty("fillImage").objectReferenceValue = fillImg;
            so.FindProperty("hpText").objectReferenceValue = text;
            so.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }
    }
}
