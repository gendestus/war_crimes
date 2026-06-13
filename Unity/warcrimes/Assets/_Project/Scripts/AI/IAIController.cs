using System;

namespace WarCrimes
{
    public interface IAIController
    {
        void BeginTurn(Action onComplete);
    }
}
