using System;
using UnityEngine;

namespace WarCrimes
{
    public class NullAI : MonoBehaviour, IAIController
    {
        public void BeginTurn(Action onComplete) => onComplete?.Invoke();
    }
}
