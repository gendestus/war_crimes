using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace WarCrimes.Editor
{
    public static class Sprint4Setup
    {
        [MenuItem("WarCrimes/Setup Sprint 4 — Movement")]
        public static void SetupSprint4()
        {
            const string scenePath = "Assets/_Project/Scenes/Game.unity";
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            if (GameObject.Find("SelectionStateMachine") != null)
            {
                Debug.Log("Sprint 4 already set up.");
                return;
            }

            // ── Highlights parent ────────────────────────────────────────────────────
            var highlightsGo = new GameObject("Highlights");

            // ── SelectionStateMachine + UnitMover on their own GO ────────────────────
            var ssmGo = new GameObject("SelectionStateMachine");
            var unitMoverGo = new GameObject("UnitMover");
            var unitMover = unitMoverGo.AddComponent<UnitMover>();

            // ── MovementHighlighter ──────────────────────────────────────────────────
            var highlighter = ssmGo.AddComponent<MovementHighlighter>();
            var mhSo = new SerializedObject(highlighter);
            mhSo.FindProperty("highlightParent").objectReferenceValue = highlightsGo.transform;
            mhSo.FindProperty("highlightSprite").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/white_pixel.png");
            mhSo.ApplyModifiedPropertiesWithoutUndo();

            // ── ActionMenu UI ────────────────────────────────────────────────────────
            var canvas = GameObject.Find("HUD Canvas");
            if (canvas == null) { Debug.LogError("HUD Canvas not found — run Sprint 3 setup first."); return; }

            var panelGo = new GameObject("ActionMenuPanel");
            panelGo.transform.SetParent(canvas.transform, false);
            var panelRect = panelGo.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.38f, 0.38f);
            panelRect.anchorMax = new Vector2(0.62f, 0.62f);
            panelRect.offsetMin = panelRect.offsetMax = Vector2.zero;
            var panelBg = panelGo.AddComponent<Image>();
            panelBg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

            var waitBtnGo = new GameObject("WaitButton");
            waitBtnGo.transform.SetParent(panelGo.transform, false);
            var waitRect = waitBtnGo.AddComponent<RectTransform>();
            waitRect.anchorMin = new Vector2(0.1f, 0.3f);
            waitRect.anchorMax = new Vector2(0.9f, 0.7f);
            waitRect.offsetMin = waitRect.offsetMax = Vector2.zero;
            var waitImg = waitBtnGo.AddComponent<Image>();
            waitImg.color = new Color(0.25f, 0.45f, 0.25f, 1f);
            var waitBtn = waitBtnGo.AddComponent<Button>();

            var waitLabelGo = new GameObject("Label");
            waitLabelGo.transform.SetParent(waitBtnGo.transform, false);
            var waitLabelRect = waitLabelGo.AddComponent<RectTransform>();
            waitLabelRect.anchorMin = Vector2.zero;
            waitLabelRect.anchorMax = Vector2.one;
            waitLabelRect.offsetMin = waitLabelRect.offsetMax = Vector2.zero;
            var waitTmp = waitLabelGo.AddComponent<TextMeshProUGUI>();
            waitTmp.text = "Wait";
            waitTmp.alignment = TextAlignmentOptions.Center;
            waitTmp.fontSize = 36;
            waitTmp.color = Color.white;

            // ActionMenu component
            var actionMenuComp = panelGo.AddComponent<ActionMenu>();
            var amSo = new SerializedObject(actionMenuComp);
            amSo.FindProperty("panel").objectReferenceValue = panelGo;
            amSo.FindProperty("waitButton").objectReferenceValue = waitBtn;
            amSo.ApplyModifiedPropertiesWithoutUndo();

            // ── SelectionStateMachine wiring ─────────────────────────────────────────
            var ssm = ssmGo.AddComponent<SelectionStateMachine>();
            var ssmSo = new SerializedObject(ssm);
            ssmSo.FindProperty("highlighter").objectReferenceValue = highlighter;
            ssmSo.FindProperty("unitMover").objectReferenceValue = unitMover;
            ssmSo.FindProperty("actionMenu").objectReferenceValue = actionMenuComp;
            ssmSo.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, scenePath);
            AssetDatabase.SaveAssets();

            Debug.Log("Sprint 4 setup complete. Press Play in Game.unity.");
        }
    }
}
