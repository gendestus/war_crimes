using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace WarCrimes.Editor
{
    public static class Sprint3Setup
    {
        [MenuItem("WarCrimes/Fix — Add EventSystem")]
        public static void AddEventSystem()
        {
            if (GameObject.Find("EventSystem") != null)
            {
                Debug.Log("EventSystem already exists.");
                return;
            }

            var esGo = new GameObject("EventSystem");
            esGo.AddComponent<EventSystem>();
            esGo.AddComponent<InputSystemUIInputModule>();

            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("EventSystem added. UI buttons should now work.");
        }

        [MenuItem("WarCrimes/Setup Sprint 3 — Game Loop")]
        public static void SetupSprint3()
        {
            const string scenePath = "Assets/_Project/Scenes/Game.unity";
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            if (GameObject.Find("GameManager") != null)
            {
                Debug.Log("Sprint 3 already set up.");
                return;
            }

            // ── GameManager ─────────────────────────────────────────────────────────
            var gmGo = new GameObject("GameManager");
            var gsm = gmGo.AddComponent<GameStateMachine>();
            gmGo.AddComponent<TurnManager>();
            gmGo.AddComponent<NullAI>();

            // ── InputRouter ──────────────────────────────────────────────────────────
            var irGo = new GameObject("InputRouter");
            var inputRouter = irGo.AddComponent<InputRouter>();

            // Wire InputRouter into GameStateMachine
            var gsmSo = new SerializedObject(gsm);
            gsmSo.FindProperty("inputRouter").objectReferenceValue = inputRouter;
            gsmSo.ApplyModifiedPropertiesWithoutUndo();

            // ── UI Canvas ────────────────────────────────────────────────────────────
            var canvasGo = new GameObject("HUD Canvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasGo.AddComponent<GraphicRaycaster>();

            // Banner panel (centre-top)
            var bannerGo = new GameObject("TurnBannerPanel");
            bannerGo.transform.SetParent(canvasGo.transform, false);
            var bannerRect = bannerGo.AddComponent<RectTransform>();
            bannerRect.anchorMin = new Vector2(0.25f, 0.75f);
            bannerRect.anchorMax = new Vector2(0.75f, 0.92f);
            bannerRect.offsetMin = bannerRect.offsetMax = Vector2.zero;
            var bannerBg = bannerGo.AddComponent<Image>();
            bannerBg.color = new Color(0f, 0f, 0f, 0.75f);
            var bannerCg = bannerGo.AddComponent<CanvasGroup>();

            // Turn text
            var textGo = new GameObject("TurnText");
            textGo.transform.SetParent(bannerGo.transform, false);
            var textRect = textGo.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = textRect.offsetMax = Vector2.zero;
            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = "Player 1 Turn";
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = 48;
            tmp.color = Color.white;

            // End Turn button (bottom-right)
            var btnGo = new GameObject("EndTurnButton");
            btnGo.transform.SetParent(canvasGo.transform, false);
            var btnRect = btnGo.AddComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0.78f, 0.02f);
            btnRect.anchorMax = new Vector2(0.98f, 0.1f);
            btnRect.offsetMin = btnRect.offsetMax = Vector2.zero;
            var btnImg = btnGo.AddComponent<Image>();
            btnImg.color = new Color(0.2f, 0.5f, 0.2f, 1f);
            var btn = btnGo.AddComponent<Button>();
            var colors = btn.colors;
            colors.highlightedColor = new Color(0.3f, 0.7f, 0.3f, 1f);
            colors.pressedColor = new Color(0.1f, 0.35f, 0.1f, 1f);
            btn.colors = colors;

            var btnTextGo = new GameObject("Label");
            btnTextGo.transform.SetParent(btnGo.transform, false);
            var btnTextRect = btnTextGo.AddComponent<RectTransform>();
            btnTextRect.anchorMin = Vector2.zero;
            btnTextRect.anchorMax = Vector2.one;
            btnTextRect.offsetMin = btnTextRect.offsetMax = Vector2.zero;
            var btnTmp = btnTextGo.AddComponent<TextMeshProUGUI>();
            btnTmp.text = "End Turn";
            btnTmp.alignment = TextAlignmentOptions.Center;
            btnTmp.fontSize = 32;
            btnTmp.color = Color.white;

            // TurnBanner component — wire its references
            var turnBanner = bannerGo.AddComponent<TurnBanner>();
            var tbSo = new SerializedObject(turnBanner);
            tbSo.FindProperty("bannerGroup").objectReferenceValue = bannerCg;
            tbSo.FindProperty("turnText").objectReferenceValue = tmp;
            tbSo.FindProperty("endTurnButton").objectReferenceValue = btn;
            tbSo.ApplyModifiedPropertiesWithoutUndo();

            // EventSystem is required for UI buttons to receive input
            if (GameObject.Find("EventSystem") == null)
            {
                var esGo = new GameObject("EventSystem");
                esGo.AddComponent<EventSystem>();
                esGo.AddComponent<InputSystemUIInputModule>();
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, scenePath);

            AssetDatabase.SaveAssets();
            Debug.Log("Sprint 3 setup complete. Press Play in Game.unity.");
        }
    }
}
