using UnityEngine;

namespace WarCrimes
{
    public class Unit : MonoBehaviour
    {
        public UnitData data;

        public int PlayerIndex { get; private set; }
        public int HP { get; private set; }
        public int Ammo { get; private set; }
        public int Fuel { get; private set; }
        public Vector2Int GridPosition { get; set; }

        private bool canAct;
        public bool CanAct
        {
            get => canAct;
            set { canAct = value; UpdateActedVisual(); }
        }

        private SpriteRenderer sr;
        private HealthBar healthBar;

        void Awake() => sr = GetComponent<SpriteRenderer>();

        public void Initialize(UnitData unitData, int playerIndex)
        {
            data = unitData;
            PlayerIndex = playerIndex;
            HP = unitData.maxHP;
            Ammo = unitData.maxAmmo;
            Fuel = unitData.maxFuel;
            healthBar = GetComponentInChildren<HealthBar>();
            healthBar?.SetHP(HP, unitData.maxHP);
            if (sr != null) sr.sortingOrder = 2;
            CanAct = true; // sets color via UpdateActedVisual
        }

        public void TakeDamage(int damage)
        {
            HP = Mathf.Max(0, HP - damage);
            healthBar?.SetHP(HP, data.maxHP);
        }

        public void ReplenishSupplies()
        {
            Ammo = data.maxAmmo;
            Fuel = data.maxFuel;
        }

        public void SetVisualState(UnitVisualState state)
        {
            if (sr == null) return;
            sr.sprite = state switch
            {
                UnitVisualState.Move    => data.spriteMove    != null ? data.spriteMove    : data.spriteIdle,
                UnitVisualState.MoveAlt => data.spriteMoveAlt != null ? data.spriteMoveAlt : data.spriteMove,
                UnitVisualState.Attack  => data.spriteAttack  != null ? data.spriteAttack  : data.spriteIdle,
                UnitVisualState.Hit     => data.spriteHit     != null ? data.spriteHit     : data.spriteIdle,
                _                       => data.spriteIdle,
            };
        }

        void UpdateActedVisual()
        {
            if (sr == null) return;
            var baseColor = PlayerIndex == 0 ? new Color(1f, 0.55f, 0.1f) : new Color(0.2f, 0.55f, 1f);
            sr.color = canAct ? baseColor : baseColor * 0.45f;
        }
    }
}
