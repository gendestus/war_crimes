# Advance Wars Clone — Architecture

## Overview

A turn-based strategy game built in Unity 2D, targeting desktop for MVP with touch-portable controls. Inspired by Advance Wars (GBA/DS series).

**Unity version:** 6000.x LTS (Unity 6)  
**Render pipeline:** URP 2D  
**Input:** Unity Input System (new) — pointer-based, works for both mouse and touch with no branching logic  
**Target platform (MVP):** Windows/Mac desktop  
**Target platform (post-MVP):** iOS / Android  

---

## MVP Scope

### Units
| Unit        | Move | Move Type | Attack Range | Can Capture |
|-------------|------|-----------|--------------|-------------|
| Infantry    | 3    | Foot      | 1            | Yes         |
| Mech        | 2    | Foot      | 1            | Yes         |
| Recon       | 8    | Wheel     | 1            | No          |
| Tank        | 6    | Tread     | 1            | No          |

### Terrain Types
| Terrain  | Defense Stars | Foot Move Cost | Wheel Move Cost | Tread Move Cost |
|----------|--------------|----------------|-----------------|-----------------|
| Plains   | 1            | 1              | 2               | 1               |
| Road     | 0            | 1              | 1               | 1               |
| Forest   | 2            | 1              | 3               | 2               |
| Mountain | 4            | 2              | —               | —               |
| City     | 3            | 1              | 1               | 1               |
| HQ       | 4            | 1              | 1               | 1               |
| Factory  | 3            | 1              | 1               | 1               |

`—` = impassable

### Win Conditions
- Capture the enemy HQ, OR
- Destroy all enemy units

### Players
- 2 players (human vs. human hotseat for MVP; AI for post-MVP)
- Each player has a faction color (Orange Star / Blue Moon)

---

## Architecture

### Design Principles

1. **Data-driven via ScriptableObjects** — unit stats, terrain costs, and damage tables are assets, not hardcoded values. Iteration happens in the Inspector.
2. **Pointer-first input** — all interaction is `position → tile → action`. No button-specific paths, so mouse and touch are identical.
3. **Event bus for loose coupling** — systems communicate via a lightweight `GameEvents` static class rather than direct references.
4. **State machine for game flow** — a `GameStateMachine` owns the current phase (PlayerTurn → SelectUnit → SelectTarget → Animate → EnemyTurn, etc.).
5. **Modern resolution, not GBA-ported** — mechanics that existed solely to fit a 240×160 screen are removed. HP is a single 1–100 integer (no display-alias scale). Health bars are smooth fills with a numeric readout. UI panels use actual readable text rather than icon-only compression.

---

## Project Folder Structure

```
Assets/
├── _Project/                        # All game-specific code and assets (underscored to sort first)
│   ├── Scripts/
│   │   ├── Core/
│   │   │   ├── GameManager.cs       # Singleton entry point; owns StateMachine + references
│   │   │   ├── GameStateMachine.cs  # Drives game phase transitions
│   │   │   ├── GameState.cs         # Enum: MainMenu, PlayerTurn, Animating, GameOver
│   │   │   ├── TurnManager.cs       # Tracks active player, turn number, begins/ends turns
│   │   │   └── GameEvents.cs        # Static event bus (OnUnitMoved, OnTileSelected, etc.)
│   │   │
│   │   ├── Grid/
│   │   │   ├── GridManager.cs       # Owns the logical grid; maps (x,y) → Tile
│   │   │   ├── Tile.cs              # Runtime tile: terrain type, unit ref, capture progress
│   │   │   ├── TileData.cs          # ScriptableObject: terrain name, move costs, defense stars, sprite
│   │   │   ├── MapLoader.cs         # Reads a MapData SO and populates the Tilemap + logical grid
│   │   │   ├── MapData.cs           # ScriptableObject: flat TileData[] (row-major, width×height) + starting unit placements
│   │   │   └── Pathfinding.cs       # BFS/Dijkstra movement range; returns reachable tiles + paths
│   │   │
│   │   ├── Units/
│   │   │   ├── Unit.cs              # MonoBehaviour on each unit prefab; holds UnitData, current HP/fuel/ammo
│   │   │   ├── UnitData.cs          # ScriptableObject: move range, move type, attack range, cost, sprites
│   │   │   ├── UnitManager.cs       # Spawns units, tracks all live units per player
│   │   │   └── MovementType.cs      # Enum: Foot, Wheel, Tread
│   │   │
│   │   ├── Combat/
│   │   │   ├── CombatManager.cs     # Orchestrates an attack: resolves damage, triggers counter, checks death
│   │   │   ├── DamageTable.cs       # ScriptableObject: flat float[] (row-major, unitTypeCount²); attacker × defender → base damage %
│   │   │   └── CombatMath.cs        # Pure static: damage formula, terrain defense, HP rounding
│   │   │
│   │   ├── Capture/
│   │   │   └── CaptureSystem.cs     # Handles capture-property action; checks win via HQ capture
│   │   │
│   │   ├── Input/
│   │   │   ├── InputRouter.cs       # Listens to InputAction (pointer click/tap); converts to world pos → tile; disabled by GameStateMachine during Animating and GameOver
│   │   │   └── SelectionStateMachine.cs  # Idle → UnitSelected → MoveTarget → AttackTarget
│   │   │
│   │   ├── UI/
│   │   │   ├── UIManager.cs         # Shows/hides panels; subscribes to GameEvents
│   │   │   ├── ActionMenuUI.cs      # Contextual popup: Move / Attack / Capture / Wait
│   │   │   ├── UnitInfoPanel.cs     # HP, ammo, fuel display for hovered/selected unit
│   │   │   ├── TileInfoPanel.cs     # Terrain name + defense stars
│   │   │   ├── TurnBanner.cs        # "Player 1 Turn" overlay between turns
│   │   │   └── HealthBar.cs         # Per-unit world-space HP bar: smooth fill + numeric readout (1–100)
│   │   │
│   │   └── AI/                      # Post-MVP stub
│   │       └── AIController.cs      # Interface; NullAI (hotseat) and SimpleAI (greedy) implementations
│   │
│   ├── ScriptableObjects/
│   │   ├── Units/
│   │   │   ├── Infantry.asset
│   │   │   ├── Mech.asset
│   │   │   ├── Recon.asset
│   │   │   └── Tank.asset
│   │   ├── Terrain/
│   │   │   ├── Plains.asset
│   │   │   ├── Road.asset
│   │   │   ├── Forest.asset
│   │   │   ├── Mountain.asset
│   │   │   ├── City.asset
│   │   │   ├── HQ.asset
│   │   │   └── Factory.asset
│   │   ├── Maps/
│   │   │   └── TestMap01.asset
│   │   └── DamageTable.asset
│   │
│   ├── Prefabs/
│   │   ├── Units/
│   │   │   ├── Infantry.prefab
│   │   │   ├── Mech.prefab
│   │   │   ├── Recon.prefab
│   │   │   └── Tank.prefab
│   │   ├── UI/
│   │   │   ├── ActionMenu.prefab
│   │   │   ├── UnitInfoPanel.prefab
│   │   │   └── TurnBanner.prefab
│   │   └── Highlight/
│   │       ├── MoveHighlight.prefab    # Blue overlay tile
│   │       ├── AttackHighlight.prefab  # Red overlay tile
│   │       └── SelectionCursor.prefab  # Animated selection box
│   │
│   ├── Scenes/
│   │   ├── Boot.unity       # Loads GameManager, then loads Game scene
│   │   ├── MainMenu.unity
│   │   └── Game.unity       # Map + gameplay
│   │
│   ├── Art/
│   │   ├── Units/           # Spritesheets per unit, per faction color
│   │   ├── Terrain/         # Tile sprites
│   │   └── UI/              # Icons, panels, fonts
│   │
│   └── Audio/
│       ├── Music/
│       └── SFX/
│
└── Settings/
    ├── InputActions.inputactions   # Pointer, Cancel, Confirm, CameraMove bindings
    └── URPAsset.asset
```

---

## Key Systems Detail

### Game State Machine

```
Boot
 └─► MainMenu
      └─► Game (load map)
           ├─► PlayerTurn
           │    ├─► Idle            (waiting for tile tap)
           │    ├─► UnitSelected    (show move range)
           │    ├─► MoveTarget      (player chose destination)
           │    ├─► ActionMenu      (Move/Attack/Capture/Wait popup)
           │    ├─► AttackTarget    (show attack range, wait for target tap)
           │    └─► Animating       (unit moves/attacks; lock input)
           └─► GameOver
```

Transitions fire events on `GameEvents`; UI and audio subscribe without the state machine knowing about them.

### Input Router

Uses a single `InputAction` of type `Value<Vector2>` (pointer position) plus a `Button` (pointer click/tap). On click:

1. Convert screen position → world position → grid coordinates
2. Raise `GameEvents.OnTilePointed(gridPos)`
3. `SelectionStateMachine` reacts based on its current state

`GameStateMachine` enables `InputRouter` on entering `PlayerTurn` and disables it on entering `Animating` or `GameOver`, so clicks during animations or after game-end are silently dropped.

No special-casing for touch vs. mouse — Unity's Input System handles it.

### Damage Formula

Based on Advance Wars GBA rules, adapted for base-100 HP:

```
baseDamage  = DamageTable[attacker][defender]            // 0–120 (base damage %)
attackPower = baseDamage * (attackerHP / 100.0)          // attacker HP: 1–100
defense     = terrainStars * 10 * (defenderHP / 100.0)   // defender HP: 1–100
finalDamage = max(0, round(attackPower - defense))
```

HP is a single 1–100 integer. No display alias. A full-HP attacker applies 100% of baseDamage; a 1-HP attacker applies 1%.

### Capture System

Each turn a foot-unit ends its turn on a capturable tile:

```
captureProgress += hp          // 1–100 per turn (full-HP unit contributes 100)
if captureProgress >= 200:     // 2 full-HP turns to capture; weakened units take longer
    tile.owner = capturingPlayer
```

Progress resets if the unit leaves or is destroyed.

### Turn Flow

```
TurnManager.BeginTurn(player)
  → Replenish fuel/ammo for units on friendly cities/factories
  → Set all units to CanAct = true
  → GameEvents.OnTurnBegin(player)

TurnManager.EndTurn()
  → All units: CanAct = false
  → Switch active player
  → Increment turn counter if back to player 1
  → BeginTurn(nextPlayer)
```

---

## Camera

- Orthographic 2D camera
- Pan: drag (pointer hold + move) — same gesture for mouse and touch
- Post-MVP: pinch-to-zoom on mobile
- Camera bounds clamped to map extents

---

## Touch Portability Checklist

- [ ] All UI hit targets ≥ 48×48 px (comfortable finger tap)
- [ ] No hover-only affordances — everything discoverable by tap
- [ ] Action menu is a radial or vertical stack (not tiny dropdown)
- [ ] Camera pan via drag works with single finger
- [ ] No right-click functionality — map to long-press or back button
- [ ] Test on 375×667 (iPhone SE) and 390×844 (iPhone 14) logical resolutions

---

## Post-MVP Roadmap

- AI opponent (greedy heuristic)
- Fog of war
- More unit types: Artillery, APC, Battlecopter, Lander
- CO system with day powers
- More terrain: Sea, Shoal, Reef, Bridge, Pipe
- Naval and air unit movement
- Map editor
- Multiplayer (async or same-device pass-and-play)
- Animated unit sprites (walk/attack cycles)
- CO artwork and dialogue
