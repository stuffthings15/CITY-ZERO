# CITY//ZERO — Technical Design Document (TDD)
**Version:** 1.0 | **Status:** Production-Ready Draft

---

## 2.1 ENGINE RECOMMENDATION

### Selected Engine: **Godot 4.x (C# / GDScript Hybrid)**

**Justification:**

| Criterion | Godot 4 | Unity | Unreal 5 |
|-----------|---------|-------|----------|
| 2.5D Top-Down Rendering | Native, excellent | Good | Overkill |
| Open Source / No Royalties | ✅ Fully open | ❌ Royalty-based | ❌ 5% royalty |
| C# Support | ✅ Full .NET 10 | ✅ Full | ❌ C++ primary |
| Scene System | ✅ Node-based, modular | Prefab-based | Actor-based |
| Performance at Scale | ✅ Good for 2.5D | Good | Excellent (overkill for scope) |
| Data-Driven Pipeline | ✅ Resources + JSON | OK | OK |
| Physics 2D/3D | ✅ Jolt integration | Good | Excellent |
| Community & Tooling | Growing fast | Mature | Mature |
| License Risk | None | Medium (policy changes) | Low |
| Target Platform Flexibility | ✅ PC, Console, Mobile | ✅ All | PC/Console |

**Architecture Decision:**
- **Gameplay Logic:** C# (.NET 10) — type safety, IDE integration, LINQ, performance
- **Scripting / Prototyping:** GDScript — rapid iteration on game feel, UI, events
- **Shaders:** GLSL via Godot shader language
- **Data:** JSON resource files + Godot `.tres` / `.res` resources for runtime objects

**Rendering Mode:** Forward+ renderer (Godot 4) with custom 2.5D perspective camera

---

## 2.2 SYSTEM ARCHITECTURE

### Modular System Breakdown

The engine architecture follows a **Service Locator + Event Bus** pattern. Systems are decoupled; they communicate via a central `GameBus` (global signal dispatcher). No system holds a direct reference to another — all interaction goes through events or the service locator.

```
ARCHITECTURE LAYERS:

  [CORE SERVICES]
    GameBus          - Central signal/event dispatcher
    ServiceLocator   - Global system registry
    SaveSystem       - Persistence layer
    ConfigManager    - Loaded from data/config/*.json

  [SIMULATION LAYER]
    WorldStateManager    - Owns faction states, district control, city variables
    TimeManager          - In-game clock; broadcasts day/night events
    WeatherSystem        - Weather state machine; broadcasts changes
    TrafficSystem        - Spawns/despawns vehicles on road graph
    EventSystem          - Spawns live world events; manages lifetimes

  [GAMEPLAY LAYER]
    PlayerController     - Kael's movement, stats, actions
    VehicleSystem        - Vehicle physics, damage, enter/exit
    CombatSystem         - Damage resolution, hit detection, weapon handling
    HeatSystem           - Wanted level; police dispatch; cooldown
    MissionSystem        - Mission lifecycle; objective tracking
    EconomySystem        - Cash, prices, shops, transactions

  [AI LAYER]
    AIDirector           - Global NPC budget; spawns/despawns; LOD scaling
    CivilianAI           - FSM per civilian; uses shared behavior pool
    PoliceAI             - HTN-based; coordinated via DispatchController
    FactionAI            - Behavior tree per faction NPC; faction awareness

  [PRESENTATION LAYER]
    HUDController        - Reads from GameBus; never reads gameplay directly
    MapSystem            - Minimap; waypoints; district overlays
    CameraController     - Follows player; dynamic zoom; cinematic events
    AudioManager         - Music state machine; SFX pooling
```

---

### Player Controller Architecture

```csharp
// PlayerController.cs — Top-level orchestrator
public partial class PlayerController : CharacterBody2D
{
    // Sub-controllers (injected via ServiceLocator)
    private PlayerMovement    _movement;
    private PlayerCombat      _combat;
    private PlayerVehicleLink _vehicleLink;
    private PlayerStats       _stats;
    private PlayerInteraction _interaction;
    private PlayerInventory   _inventory;

    // State machine
    private PlayerStateMachine _stateMachine;
    // States: OnFoot | InVehicle | Downed | Cutscene | Subduing | Vaulting

    public override void _Ready()
    {
        ServiceLocator.Register<PlayerController>(this);
        _stateMachine = new PlayerStateMachine(this);
        _stateMachine.TransitionTo(PlayerState.OnFoot);
    }

    public override void _PhysicsProcess(double delta)
    {
        _stateMachine.Update(delta);
    }
}
```

**Key sub-systems:**

```
PlayerMovement:
  - Input reading (InputMap abstraction)
  - Velocity calculation (with surface modifier lookup)
  - Stamina management
  - Collision response

PlayerCombat:
  - Weapon slot management (2 active + 1 sidearm + 1 heavy)
  - Firing logic → delegates to WeaponDefinition resource
  - Melee state entry
  - Hit registration → sends event to CombatSystem

PlayerStats:
  - HP, Armor, Stamina (current/max)
  - Buff/debuff stack
  - Death handling → sends PlayerDiedEvent to GameBus

PlayerInteraction:
  - Raycast-based interaction detection
  - Builds interaction options list based on context
  - Delegates to appropriate system (VehicleSystem, PickupSystem, etc.)
```

---

### Vehicle System Architecture

```
VehicleSystem (Autoload Singleton)
  - Registry: Dictionary<int, VehicleInstance>
  - Spawner: Receives spawn requests from TrafficSystem or player
  - EnterExitController: Handles transition animations + state swap

VehicleInstance (per vehicle node)
  - VehiclePhysics2D (extends RigidBody2D)
    - Surface-aware traction calculation
    - Damage zone dictionary
    - Fire/explosion FSM
  - VehicleDefinition resource (data-only; loaded from JSON)
  - OccupantSlot[] (driver + passengers)
  - DamageModel (tracks HP per zone)
  - AudioComponent (engine pitch mapping to speed)
```

---

### AI System Architecture

```
AIDirector (Autoload Singleton)
  - NPC budget: max 120 simulated NPCs at full detail
  - Distance-based LOD:
    LOD0 (<40m):  Full AI; animations; pathfinding; all behaviors
    LOD1 (40-100m): Simplified pathfinding; reduced animation; basic state only
    LOD2 (100-300m): Position simulation only; no pathfinding; crowd sim
    LOD3 (>300m): Statistical simulation (faction territory activity only)

DispatchController (Police-specific)
  - Crime event queue (sorted by severity + time)
  - Unit pool management (patrol cars, SWAT, helicopter)
  - Issues task assignments to PoliceAI instances
  - Manages roadblock placement logic

BehaviorTreeRunner (per faction NPC)
  - Tick-based (every 0.1 sec at LOD0; 0.5 sec at LOD1)
  - Tree loaded from faction-specific BT resource
  - Blackboard: shared data context per NPC instance
```

---

### Heat System Architecture

```csharp
// HeatSystem.cs
public partial class HeatSystem : Node
{
    public float CurrentHeat { get; private set; } = 0f;   // 0.0 to 5.0 (float for smooth transitions)
    public int HeatLevel => Mathf.FloorToInt(CurrentHeat); // 0–5 int for dispatch logic

    private float _cooldownTimer = 0f;
    private bool _playerHidden = false;
    private HeatConfig _config; // loaded from data/heat_config.json

    public void AddHeat(float amount, string reason)
    {
        CurrentHeat = Mathf.Clamp(CurrentHeat + amount, 0f, 5f);
        GameBus.Instance.Emit(new HeatChangedEvent(CurrentHeat, reason));
        DispatchController.Instance.OnHeatEscalated(HeatLevel);
    }

    public override void _Process(double delta)
    {
        if (_playerHidden && CurrentHeat > 0)
        {
            _cooldownTimer += (float)delta;
            if (_cooldownTimer >= _config.CooldownDelay[HeatLevel])
                CurrentHeat = Mathf.Max(0f, CurrentHeat - _config.DecayRate * (float)delta);
        }
        else
        {
            _cooldownTimer = 0f;
        }
    }
}
```

---

### Mission System Architecture

```
MissionSystem (Autoload Singleton)
  - MissionRegistry: all mission definitions loaded from data/missions/*.json
  - ActiveMissions: List<MissionInstance> (max 3 concurrent)
  - MissionLifecycle:
    LOCKED → AVAILABLE → BRIEFED → ACTIVE → COMPLETED/FAILED → LOGGED

MissionInstance
  - Owns ObjectiveTracker[]
  - Manages timer (if timed mission)
  - Listens to GameBus for relevant events (kills, item pickups, area entries)
  - On objective complete: evaluates success conditions
  - On success/fail: triggers reward/consequence dispatch

ObjectiveTracker
  - Type: Kill | Reach | Deliver | Protect | Hack | Survive | Optional
  - Current value / Target value
  - Visible: bool (hidden optional objectives revealed on completion or discovery)
```

---

### Economy System Architecture

```
EconomySystem (Autoload Singleton)
  - PlayerWallet: float VTEK
  - PriceTable: loaded from data/economy/prices.json
  - GlobalPriceModifier: float (1.0 default; modified by faction events)
  - TransactionLog: last 50 transactions (for save system)

ShopInstance (per shop node in world)
  - ShopDefinition resource (items, prices, faction affiliation)
  - PurchaseItem(ItemDefinition, quantity) → validates funds → deducts → delivers
  - SellItem(ItemDefinition, quantity) → applies fence multiplier → credits wallet

FactionEconomy
  - Per-faction price multiplier (based on player reputation with that faction)
  - Allied: 0.85x prices; Trusted: 0.70x prices; Hostile: 1.30x or no service
```

---

## 2.3 DATA-DRIVEN DESIGN

All gameplay entities are defined in JSON and loaded at startup. The game code reads definitions — it never hardcodes gameplay values.

### Mission Schema

```json
{
  "mission_id": "mission_cold_iron",
  "title": "Cold Iron",
  "archetype": "heist",
  "giver_faction": "ruin_syndicate",
  "giver_npc_id": "npc_dame_oskar",
  "prerequisites": {
    "faction_rep_min": { "ruin_syndicate": 10 },
    "missions_completed": [],
    "story_flag": null
  },
  "location": {
    "district": "the_pits",
    "spawn_point": "rail_depot_entrance",
    "area_id": "area_vektro_rail"
  },
  "time_limit_seconds": 480,
  "heat_modifier_base": 1.0,
  "objectives": [
    {
      "id": "obj_intercept_train",
      "type": "reach",
      "target": "area_signal_box",
      "label": "Reach the signal box before the train departs",
      "required": true
    },
    {
      "id": "obj_board_train",
      "type": "reach",
      "target": "area_train_car_4",
      "label": "Board the cargo train",
      "required": true
    },
    {
      "id": "obj_secure_crate",
      "type": "interact",
      "target": "prop_biometric_crate",
      "label": "Secure the weapon crate",
      "required": true
    },
    {
      "id": "obj_extract",
      "type": "deliver",
      "target": "npc_syndicate_van",
      "payload": "item_weapon_crate",
      "label": "Deliver the crate to the Syndicate",
      "required": true
    },
    {
      "id": "obj_bonus_tracker",
      "type": "interact",
      "target": "prop_train_chassis",
      "label": "Plant a tracker on the train",
      "required": false,
      "bonus_cash": 500
    }
  ],
  "success_conditions": {
    "all_required_objectives": true
  },
  "failure_conditions": [
    { "type": "area_exit", "area": "area_waterfront_cartel", "payload_held": true },
    { "type": "player_death" }
  ],
  "rewards": {
    "cash": 3200,
    "reputation": [
      { "faction": "ruin_syndicate", "delta": 15 },
      { "faction": "meridian_cartel", "delta": -10 }
    ],
    "unlock_flags": ["service_chopshop_discount"],
    "items": []
  }
}
```

---

### Vehicle Schema

```json
{
  "vehicle_id": "vehicle_helix7",
  "display_name": "HELIX 7",
  "class": "sports",
  "faction_affiliation": null,
  "stats": {
    "top_speed_kmh": 240,
    "acceleration": 0.92,
    "braking": 0.78,
    "handling": 0.90,
    "durability_hp": 280,
    "weight_class": "light",
    "drive_type": "rear_wheel"
  },
  "damage_zones": {
    "front": { "hp": 60, "effect_on_deplete": "reduced_ram" },
    "engine": { "hp": 80, "effect_on_deplete": "speed_penalty_50pct" },
    "tires": { "hp": 40, "effect_on_deplete": "handling_penalty_severe" },
    "fuel_tank": { "hp": 50, "effect_on_deplete": "fire_trail" }
  },
  "traction_modifiers": {
    "wet": 0.75,
    "gravel": 0.85,
    "ice": 0.60
  },
  "seats": 2,
  "can_be_stolen": true,
  "spawn_districts": ["neon_flats", "the_grid"],
  "spawn_weight": 3,
  "price_shop": 22000,
  "unlock_condition": "mission_redline_win",
  "audio_engine_id": "snd_engine_sports_high",
  "model_path": "res://assets/vehicles/helix7/helix7.tscn"
}
```

---

### Weapon Schema

```json
{
  "weapon_id": "weapon_voss9_smg",
  "display_name": "VOSS-9",
  "category": "smg",
  "stats": {
    "damage_per_bullet": 18,
    "fire_rate_rpm": 720,
    "magazine_size": 32,
    "max_reserve_ammo": 160,
    "reload_time_sec": 2.1,
    "effective_range_m": 15,
    "falloff_start_m": 12,
    "falloff_end_m": 25,
    "recoil_horizontal": 0.08,
    "recoil_vertical": 0.12,
    "aim_cone_degrees": 4.0,
    "aim_cone_sprint_modifier": 2.0
  },
  "fire_modes": ["auto"],
  "ammo_type": "ammo_9mm",
  "concealable": true,
  "heat_modifier_per_shot": 0.02,
  "can_suppress": true,
  "suppressor_slot": true,
  "upgrades": ["upgrade_extended_mag", "upgrade_suppressor", "upgrade_laser_dot"],
  "price_shop": 1800,
  "unlock_condition": null,
  "audio_shot_id": "snd_shot_smg_9mm",
  "model_path": "res://assets/weapons/voss9/voss9.tscn"
}
```

---

### Faction Schema

```json
{
  "faction_id": "ruin_syndicate",
  "display_name": "Ruin Syndicate",
  "symbol_path": "res://assets/ui/factions/ruin_syndicate_icon.png",
  "colors": { "primary": "#1A1A1A", "accent": "#C85A00" },
  "territory_districts": ["the_pits"],
  "leader_npc_id": "npc_dame_oskar",
  "starting_reputation": 0,
  "reputation_bleed": [
    { "target_faction": "warden_bloc", "multiplier": -0.4 }
  ],
  "services": {
    "min_rep_for_access": -20,
    "shop_id": "shop_syndicate_black_market",
    "chop_shop_id": "chopshop_pits_main",
    "discount_at_trusted": 0.40
  },
  "ai_profile": {
    "aggression": 0.75,
    "cohesion": 0.60,
    "retreat_hp_threshold": 0.15,
    "preferred_range": "medium",
    "ranged_preference": 0.70
  },
  "patrol_density": {
    "the_pits_day": 4,
    "the_pits_night": 8
  },
  "missions_available": ["mission_cold_iron", "mission_pressure_drop", "mission_shatter_house"],
  "lore_id": "lore_ruin_syndicate"
}
```

---

### District Schema

```json
{
  "district_id": "the_pits",
  "display_name": "The Pits",
  "controlling_faction": "ruin_syndicate",
  "color_palette": { "primary": "#7A5C3A", "accent": "#FF6600", "shadow": "#1C1C1C" },
  "ambient_audio_id": "amb_industrial_south",
  "population_density": {
    "day_foot": 0.6,
    "day_vehicle": 0.3,
    "night_foot": 0.8,
    "night_vehicle": 0.5
  },
  "police_presence": {
    "patrol_cars": 2,
    "foot_units": 0,
    "response_time_multiplier": 1.8
  },
  "heat_escalation_modifier": 0.85,
  "cctv_coverage": 0.20,
  "shop_ids": ["shop_general_pits", "shop_syndicate_weapons", "shopchop_pits_main"],
  "spawn_points": ["spawn_pits_north", "spawn_pits_south", "spawn_rail_depot"],
  "unlock_condition": null,
  "gameplay_traits": ["tight_streets", "low_visibility_alleys", "faction_hotzone"],
  "weather_modifiers": { "fog_frequency_multiplier": 1.4 }
}
```

---

### Shop Schema

```json
{
  "shop_id": "shop_syndicate_weapons",
  "display_name": "Syndicate Cache",
  "faction_affiliation": "ruin_syndicate",
  "min_reputation_to_access": 10,
  "location": { "district": "the_pits", "map_marker": "marker_synweapon_shop" },
  "inventory": [
    { "item_id": "weapon_voss9_smg", "stock": -1, "price_override": null },
    { "item_id": "weapon_rk12_shotgun", "price_override": 0.85 },
    { "item_id": "weapon_heavymg_chain", "min_rep": 60, "price_override": null },
    { "item_id": "ammo_9mm", "stock": -1 },
    { "item_id": "item_armor_plate", "stock": 5, "restock_hours": 24 }
  ],
  "buy_sell_ratio": 0.55,
  "audio_ambient_id": "amb_shop_underground"
}
```

---

### World Event Schema

```json
{
  "event_id": "event_warden_incursion_pits",
  "display_name": "Warden Incursion",
  "category": "faction_conflict",
  "trigger_conditions": {
    "faction_rep_differential": { "warden_bloc_vs_ruin_syndicate": 40 },
    "district": "the_pits",
    "time_of_day": ["night"],
    "cooldown_hours": 48
  },
  "probability_per_hour": 12,
  "duration_minutes": { "min": 8, "max": 20 },
  "spawns": [
    { "faction": "warden_bloc", "unit_type": "patrol", "count": 4 },
    { "faction": "ruin_syndicate", "unit_type": "defender", "count": 6 }
  ],
  "player_notification": {
    "type": "radio_tip",
    "message": "Heavy movement spotted in The Pits — Warden Bloc pushing south."
  },
  "resolution_outcomes": [
    { "condition": "warden_wins", "effect": { "district_control_shift": 0.1, "faction_rep": { "warden_bloc": 5 } } },
    { "condition": "syndicate_wins", "effect": { "faction_rep": { "ruin_syndicate": 5 } } },
    { "condition": "player_intervenes_syndicate", "effect": { "faction_rep": { "ruin_syndicate": 12 }, "heat": 1.5 } }
  ],
  "follow_up_mission_id": "mission_aftermath_incursion"
}
```

---

## 2.4 AI IMPLEMENTATION

### Behavior Tree Implementation

Using a custom lightweight C# BT implementation (no third-party dependency):

```csharp
// Core BT nodes
public abstract class BTNode
{
    public abstract BTStatus Tick(AIBlackboard board);
}

public enum BTStatus { Success, Failure, Running }

public class BTSelector : BTNode  // OR logic — first Success wins
{
    private List<BTNode> _children;
    public override BTStatus Tick(AIBlackboard board)
    {
        foreach (var child in _children)
        {
            var status = child.Tick(board);
            if (status != BTStatus.Failure) return status;
        }
        return BTStatus.Failure;
    }
}

public class BTSequence : BTNode  // AND logic — all must Succeed
{
    private List<BTNode> _children;
    public override BTStatus Tick(AIBlackboard board)
    {
        foreach (var child in _children)
        {
            var status = child.Tick(board);
            if (status != BTStatus.Success) return status;
        }
        return BTStatus.Success;
    }
}

// Condition node example
public class IsUnderAttackCondition : BTNode
{
    public override BTStatus Tick(AIBlackboard board)
        => board.Get<bool>("is_under_attack") ? BTStatus.Success : BTStatus.Failure;
}

// Task node example
public class TakeCoverTask : BTNode
{
    public override BTStatus Tick(AIBlackboard board)
    {
        var agent = board.Get<FactionAgent>("self");
        var cover = CoverSystem.FindNearest(agent.GlobalPosition, radius: 15f);
        if (cover == null) return BTStatus.Failure;
        agent.NavigateTo(cover.Position);
        board.Set("current_cover", cover);
        return BTStatus.Running;
    }
}
```

### Pathfinding

- **Library:** Godot's built-in `NavigationServer2D` with NavigationRegion2D per district
- **Mesh:** Pre-baked navigation mesh per district; updated dynamically when obstacles destroyed
- **Agents:** Each AI registers as a NavigationAgent2D; uses local avoidance (ORCA)
- **Police Vehicles:** Use road-graph pathfinding (dedicated road node graph); not nav mesh
- **Performance:** Path recalculation capped at max 30 path requests per frame; queued and distributed

### Traffic System

```
TrafficSystem
  - Road Graph: Pre-built directed graph of road segments
    - Nodes: Intersections
    - Edges: Road segments (speed limit, lane count, direction)
  - Vehicle Pool: Pre-spawned vehicle pool (40 vehicles recycled)
  - Spawner: Spawns vehicles at district boundary spawn nodes; despawns at far boundaries
  - Flow: Vehicles follow road graph; stop at red lights (traffic signal nodes, 15-sec cycle)
  - Reaction: Vehicles brake for obstacles (raycast ahead); honk and wait; reroute after 10 sec
  - Police Impact: Police chase causes adjacent traffic to pull over (broadcast event → traffic reaction)
  - Player Impact: Player vehicle collisions → chain reactions → pile-ups possible
```

---

## 2.5 SAVE/LOAD SYSTEM

### Persisted Data

```json
{
  "save_version": "1.0.0",
  "timestamp": "2024-01-15T22:34:00Z",
  "playtime_seconds": 14523,
  "player": {
    "position": { "x": 1240.5, "y": -330.2 },
    "district": "the_pits",
    "health": 87,
    "armor": 40,
    "stamina_max": 120,
    "wallet_vtek": 12400.50,
    "upgrade_tree": {
      "operator": [1, 1, 0],
      "driver": [1, 0, 0],
      "ghost": [1, 1, 0],
      "networker": [0, 0, 0]
    },
    "skill_counters": {
      "km_driven": 127.4,
      "pistol_kills": 34,
      "stealth_missions_clean": 3
    },
    "inventory": [
      { "item_id": "weapon_voss9_smg", "ammo_loaded": 24, "ammo_reserve": 96 },
      { "item_id": "weapon_kp45_pistol", "ammo_loaded": 12, "ammo_reserve": 48 }
    ]
  },
  "world": {
    "time_of_day": 21.5,
    "weather": "light_rain",
    "heat_level": 0.0,
    "faction_reputations": {
      "ruin_syndicate": 42,
      "warden_bloc": -15,
      "hollow_kings": 28,
      "meridian_cartel": 5,
      "axiom_directorate_threat": 35
    },
    "district_control": {
      "the_pits": "ruin_syndicate",
      "the_grid": "warden_bloc",
      "neon_flats": "hollow_kings",
      "the_waterfront": "meridian_cartel",
      "the_spire": "axiom_directorate"
    },
    "safehouses_owned": ["safehouse_pits_01", "safehouse_neonflats_02"],
    "world_flags": ["flag_signal_break_completed", "flag_chemist_spared"],
    "event_cooldowns": {
      "event_warden_incursion_pits": 1706234040
    }
  },
  "missions": {
    "completed": ["mission_cold_iron", "mission_glass_eyes"],
    "failed": [],
    "available": ["mission_pressure_drop", "mission_redline"],
    "active": []
  },
  "vehicles_owned": [
    { "vehicle_id": "vehicle_helix7", "stored_at": "safehouse_pits_01", "damage_state": {} }
  ]
}
```

### Save Slots: 3 manual + 1 auto-save (written on district exit, safehouse entry, mission end)

---

## 2.6 PERFORMANCE STRATEGY

### Target Specifications

| Tier | GPU | Target FPS | Resolution |
|------|-----|-----------|-----------|
| Low | GTX 1060 / RX 580 | 60 FPS | 1080p |
| Medium | RTX 2060 / RX 6600 | 60 FPS | 1440p |
| High | RTX 3070+ | 60+ FPS | 4K |

### LOD Strategy

```
DISTANCE-BASED LOD (from player camera)
  <40m:   Full mesh + full animation + all particle effects + full AI tick
  40-80m: Reduced mesh (LOD1) + simplified animation + reduced particles + AI LOD1
  80-150m: Impostor quad sprite + no particles + AI LOD2 (position only)
  >150m:  Culled or statistical simulation only

VEHICLE LOD
  <50m:  Full model + damage mesh updates + full physics
  50-120m: LOD1 mesh + simplified physics (no per-zone damage calc)
  >120m: Billboard sprite + traffic simulation only (no physics)

BUILDING LOD
  <60m:  Full interior/exterior + interior lighting
  60-150m: Exterior only + simplified shadow
  >150m: Simplified mesh + no shadow cast
```

### Culling

- Frustum culling: All nodes outside camera frustum are visibility-culled (Godot built-in)
- Occlusion culling: Large occluder meshes (building exteriors) registered as occluders; occlude interior objects
- District streaming: Non-active districts are kept loaded but simulation-paused; swap active district on boundary cross
- Interior culling: Building interiors disabled until player within 30m of entrance

### AI Scaling

```
AIDirector budget per frame:
  Full detail NPCs (LOD0): max 30
  LOD1 NPCs:               max 50
  LOD2+ (position sim):    max 120 (no per-frame tick; updated every 10 frames)

Police units:
  Active pursuit: always LOD0 (never culled while pursuing)
  Off-duty patrol: LOD1 until player within 60m

Budget enforcement:
  When budget exceeded: pause lowest-priority LOD0 NPC tick for 1 frame
  Dynamic scaling: Reduce max counts if frame time > 16.5ms
```

### Simulation Throttling

- Traffic vehicles beyond 200m: Teleport-simulated (position advances along path without physics)
- Weather particles: Cap at 2,000 particles for rain; 500 for fog; scale dynamically
- Crowd ambience: Shared animation state machine (all idle civilians in range share same tick timer group)
- Audio: Distance-based occlusion + max simultaneous SFX sources capped at 32
