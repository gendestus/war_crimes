# Advance Wars Clone — MVP Sprints

Ordered by dependency. Each sprint ends with a concrete milestone — something you can run and verify.

---

## Sprint 1 — Foundation

**Goal:** All data assets exist; nothing runs yet.

- Create Unity project (Unity 6, URP 2D, new Input System package)
- Set up folder structure per architecture doc
- Define all ScriptableObject schemas:
  - `UnitData` (move range, move type, attack range, max HP/ammo/fuel, sprites)
  - `TileData` (terrain name, defense stars, move costs per type, sprite)
  - `MapData` (width, height, flat `TileData[]`, starting unit placements)
  - `DamageTable` (flat `float[]`, unitTypeCount = 4)
- Create the 4 unit assets (Infantry, Mech, Recon, Tank) with stats from architecture
- Create the 7 terrain assets (Plains, Road, Forest, Mountain, City, HQ, Factory)
- Fill the 4×4 DamageTable asset with base damage values
- Create unit prefabs (placeholder sprites, `Unit` MonoBehaviour, `HealthBar`)

**Milestone:** Inspector shows all assets with correct data. Nothing playable.

---

## Sprint 2 — Map Renders

**Goal:** A map with units on it appears on screen.

- `GridManager`: owns logical grid, maps `(x,y) → Tile`
- `Tile`: runtime data (terrain type, unit ref, capture progress)
- `MapLoader`: reads `MapData` asset, builds Tilemap + logical grid, spawns starting units via `UnitManager`
- `TestMap01` asset: at minimum a small hand-authored map (8–12 tiles wide) with starting positions for both players
- `UnitManager`: spawns unit prefabs, tracks live units per player
- Camera: orthographic, drag-to-pan, bounds clamped to map extents
- `HealthBar`: smooth fill + numeric readout rendered in world space on each unit

**Milestone:** Launch the Game scene — map renders, units appear at correct positions with health bars, camera pans.

---

## Sprint 3 — Game Loop Skeleton

**Goal:** Turns alternate; the game knows whose turn it is.

- `GameState` enum: `MainMenu`, `PlayerTurn`, `AIThinking`, `Animating`, `GameOver`
- `GameStateMachine`: drives transitions, enables/disables `InputRouter` per state
- `GameEvents`: static event bus — wire up `OnTurnBegin` at minimum
- `TurnManager`: `BeginTurn` / `EndTurn`, tracks active player and turn counter, sets `CanAct` on all units, replenishes fuel/ammo for units on friendly cities/factories
- `NullAI`: no-op `IAIController` that completes immediately (hotseat)
- `InputRouter`: click/tap → world pos → grid coords → `GameEvents.OnTilePointed`; disabled outside `PlayerTurn`
- `TurnBanner` UI: "Player 1 Turn" / "Player 2 Turn" overlay fires on `OnTurnBegin`

**Milestone:** Clicking "End Turn" (hardcoded button is fine) alternates the banner and resets `CanAct` on all units.

---

## Sprint 4 — Movement

**Goal:** Units can move.

- `SelectionStateMachine`: `Idle → UnitSelected → MoveTarget → Animating → Idle`
  - Idle: tap tile → if friendly unit with `CanAct`, enter `UnitSelected`
  - UnitSelected: show move range; tap destination → `MoveTarget`; tap elsewhere → back to `Idle`
  - MoveTarget: commit move, enter `Animating`
- `Pathfinding`: BFS movement range using terrain move costs and move type; returns reachable tiles and optimal path to each
- Move highlights: blue overlay tiles on reachable cells
- Unit movement: tween along path; `GameStateMachine` enters `Animating`, returns to `PlayerTurn` on completion
- `ActionMenu` stub: appears after move with only "Wait" functional; Wait sets `unit.CanAct = false`

**Milestone:** Select a unit, see its movement range highlighted, click a tile, watch the unit move there, confirm Wait — unit grays out.

---

## Sprint 5 — Combat

**Goal:** Units can attack and die.

- `ActionMenu` fully wired: Move / Attack / Capture (disabled if not capturable) / Wait
- `AttackTarget` selection state: show attack range in red; tap valid enemy → confirm attack
- `CombatManager`: resolves attack, triggers counter-attack if defender survives and is in range, checks for death
- `CombatMath`: pure static damage formula (see architecture); returns damage dealt to both sides
- Unit death: remove from grid and `UnitManager`, destroy GameObject
- Attack animation placeholder (flash or simple tween is fine for MVP)

**Milestone:** Two units can fight. The attacker deals damage, the defender counter-attacks if able, dead units disappear.

---

## Sprint 6 — Capture and Win Conditions

**Goal:** The game can be won.

- `CaptureSystem`: foot unit ending turn on capturable tile accumulates `captureProgress += hp`; at ≥ 200, tile changes owner; progress resets on unit departure or death
- Tile ownership visual: city/HQ/factory sprite or tint updates on capture
- Win condition — HQ captured: `CaptureSystem` checks after each capture; if enemy HQ changes owner, fire `GameEvents.OnGameOver`
- Win condition — last unit destroyed: `UnitManager` checks after each death; if a player has zero units, fire `GameEvents.OnGameOver`
- `GameOver` state: `GameStateMachine` enters it on `OnGameOver`; input locks; winner announced via simple overlay

**Milestone:** Play a game to completion — capture the enemy HQ or eliminate all their units, see the game-over screen.

---

## Sprint 7 — UI Pass

**Goal:** The UI gives players the information they need.

- `UnitInfoPanel`: shows HP, ammo, fuel for the hovered or selected unit
- `TileInfoPanel`: shows terrain name and defense stars for the hovered tile
- `ActionMenu` polish: clean layout, correct items enabled/disabled by context (e.g., Capture only on capturable tiles, Attack only if valid targets exist)
- `HealthBar` polish: color shifts (green → yellow → red) as HP drops
- Damage preview: when hovering an attack target, show estimated damage dealt and received before confirming

**Milestone:** A new player can understand what every unit and tile does without reading the design doc.

---

## Sprint 8 — Scenes and Shippable Build

**Goal:** A complete, launchable game.

- Boot scene: initializes `GameManager`, transitions to MainMenu
- MainMenu scene: "Play" launches the Game scene with `TestMap01`
- Scene transitions (simple fade)
- Audio: SFX hooks for move, attack, capture, unit death, win/lose; placeholder music track
- Second test map (optional but recommended — one more layout to verify map loading is generic)
- End-to-end playtest: full game, both players, both win conditions, all unit types and terrain interactions
- Bug-fix pass from playtest findings
- Windows build artifact

**Milestone:** A standalone Windows build that two people can sit down and play from boot to game-over.

---

## Post-MVP (not in scope above)

See architecture doc roadmap. Priority order suggestion:

1. Heuristic AI (`HeuristicAI`) — unblocks solo testing
2. LLM AI (`LlmAI` + `AnthropicClient`) — WorldView layer already designed
3. More unit types and terrain
4. Fog of war
5. CO system
6. Map editor
7. Mobile port
