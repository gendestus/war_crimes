using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WarCrimes
{
    public class ActionMenu : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Button waitButton;

        private Unit currentUnit;
        private Action onComplete;

        void Start()
        {
            waitButton.onClick.AddListener(OnWait);
            panel.SetActive(false);
        }

        public void Show(Unit unit, Action onActionComplete)
        {
            currentUnit = unit;
            onComplete = onActionComplete;
            panel.SetActive(true);
        }

        void OnWait()
        {
            currentUnit.CanAct = false;
            panel.SetActive(false);
            var cb = onComplete;
            currentUnit = null;
            onComplete = null;
            cb?.Invoke();
        }
    }
}
