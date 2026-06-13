using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WarCrimes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private TextMeshProUGUI hpText;

        public void SetHP(int current, int max)
        {
            float t = (float)current / max;
            fillImage.fillAmount = t;
            if (hpText != null)
                hpText.text = current.ToString();
        }
    }
}
