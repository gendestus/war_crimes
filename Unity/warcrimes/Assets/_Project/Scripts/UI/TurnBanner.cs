using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WarCrimes
{
    public class TurnBanner : MonoBehaviour
    {
        [SerializeField] private CanvasGroup bannerGroup;
        [SerializeField] private TextMeshProUGUI turnText;
        [SerializeField] private Button endTurnButton;

        private static readonly string[] PlayerNames = { "Player 1", "Player 2" };

        void OnEnable() => GameEvents.OnTurnBegin += OnTurnBegin;
        void OnDisable() => GameEvents.OnTurnBegin -= OnTurnBegin;

        void Start()
        {
            endTurnButton.onClick.AddListener(() => TurnManager.Instance.EndTurn());
            bannerGroup.alpha = 0f;
        }

        void OnTurnBegin(int playerIndex)
        {
            turnText.text = $"{PlayerNames[playerIndex]} Turn";
            StopAllCoroutines();
            StartCoroutine(ShowBanner());
        }

        IEnumerator ShowBanner()
        {
            // Fade in
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * 4f;
                bannerGroup.alpha = Mathf.Clamp01(t);
                yield return null;
            }

            yield return new WaitForSeconds(1.5f);

            // Fade out
            t = 1f;
            while (t > 0f)
            {
                t -= Time.deltaTime * 4f;
                bannerGroup.alpha = Mathf.Clamp01(t);
                yield return null;
            }
        }
    }
}
