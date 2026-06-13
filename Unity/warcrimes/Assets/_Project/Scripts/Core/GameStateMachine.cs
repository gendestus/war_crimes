using UnityEngine;

namespace WarCrimes
{
    public class GameStateMachine : MonoBehaviour
    {
        public static GameStateMachine Instance { get; private set; }

        public GameState State { get; private set; } = GameState.PlayerTurn;

        [SerializeField] private InputRouter inputRouter;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void ChangeState(GameState newState)
        {
            State = newState;
            if (inputRouter != null)
                inputRouter.enabled = newState == GameState.PlayerTurn;
        }
    }
}
