using System;
using UnityEngine;

namespace WarCrimes
{
    public static class GameEvents
    {
        public static event Action<int> OnTurnBegin;
        public static event Action<Vector2Int> OnTilePointed;

        public static void TurnBegin(int playerIndex) => OnTurnBegin?.Invoke(playerIndex);
        public static void TilePointed(Vector2Int gridPos) => OnTilePointed?.Invoke(gridPos);
    }
}
