# CITY//ZERO — Production Folder Structure
**Version:** 1.0 | **Status:** Production-Ready Draft

---

## 9.1 FULL REPOSITORY STRUCTURE

```
CITY#ZERO/                                  ← Repository root (git)
│
├── .github/
│   ├── workflows/
│   │   ├── ci.yml                          ← CI: test + lint + import validation
│   │   └── release.yml                     ← CD: build + package for release
│   ├── ISSUE_TEMPLATE/
│   │   ├── bug_report.md
│   │   └── feature_request.md
│   └── PULL_REQUEST_TEMPLATE.md
│
├── .gitattributes                           ← Git LFS rules for binary files
├── .gitignore                              ← Godot .godot/ + OS artifacts
├── README.md
├── CONTRIBUTING.md
│
├── docs/                                   ← Production documentation
│   ├── GDD.md                             ← Game Design Document
│   ├── TDD.md                             ← Technical Design Document
│   ├── ART_PLAN.md                        ← Art & Asset Production Plan
│   ├── CONTENT_PIPELINE.md               ← Pipeline documentation
│   ├── SCOPE_PLAN.md                      ← MVP + Full Scope
│   ├── ROADMAP.md                         ← Development Roadmap
│   ├── QA_PLAN.md                         ← QA and Testing Plan
│   ├── RISK_ANALYSIS.md                   ← Risk Analysis
│   ├── FOLDER_STRUCTURE.md               ← This file
│   ├── schemas/
│   │   ├── mission.schema.json
│   │   ├── vehicle.schema.json
│   │   ├── weapon.schema.json
│   │   ├── faction.schema.json
│   │   ├── district.schema.json
│   │   ├── shop.schema.json
│   │   └── event.schema.json
│   └── art/
│       ├── style_guide.md
│       ├── color_palettes.md
│       └── reference_board/               ← Reference images (PNG; non-copyrighted)
│
├── project.godot                           ← Godot 4 project file
├── export_presets.cfg                      ← Godot export configurations
│
├── src/                                    ← All C# source code
│   ├── Core/
│   │   ├── GameBus.cs                     ← Global event dispatcher
│   │   ├── ServiceLocator.cs              ← Global service registry
│   │   ├── ConfigManager.cs              ← Loads config JSON at startup
│   │   └── Events/
│   │       ├── HeatChangedEvent.cs
│   │       ├── PlayerDiedEvent.cs
│   │       ├── MissionStateChangedEvent.cs
│   │       ├── FactionRepChangedEvent.cs
│   │       ├── WorldEventSpawnedEvent.cs
│   │       └── EconomyTransactionEvent.cs
│   │
│   ├── Player/
│   │   ├── PlayerController.cs            ← Top-level orchestrator
│   │   ├── PlayerMovement.cs
│   │   ├── PlayerCombat.cs
│   │   ├── PlayerStats.cs
│   │   ├── PlayerInteraction.cs
│   │   ├── PlayerInventory.cs
│   │   ├── PlayerVehicleLink.cs
│   │   └── StateMachine/
│   │       ├── PlayerStateMachine.cs
│   │       ├── PlayerState.cs             ← Enum
│   │       ├── States/
│   │       │   ├── OnFootState.cs
│   │       │   ├── InVehicleState.cs
│   │       │   ├── DownedState.cs
│   │       │   ├── SubduingState.cs
│   │       │   └── CutsceneState.cs
│   │
│   ├── Vehicle/
│   │   ├── VehicleSystem.cs               ← Singleton; registry + spawner
│   │   ├── VehicleInstance.cs             ← Per-vehicle node
│   │   ├── VehiclePhysics.cs              ← RigidBody2D extension
│   │   ├── VehicleDamageModel.cs
│   │   ├── VehicleOccupantManager.cs
│   │   ├── VehicleEnterExitController.cs
│   │   └── TrafficSystem.cs
│   │
│   ├── AI/
│   │   ├── AIDirector.cs                  ← NPC budget + LOD manager
│   │   ├── BehaviorTree/
│   │   │   ├── BTNode.cs                  ← Abstract base
│   │   │   ├── BTSelector.cs
│   │   │   ├── BTSequence.cs
│   │   │   ├── BTDecorators/
│   │   │   │   ├── BTInverter.cs
│   │   │   │   └── BTRepeater.cs
│   │   │   ├── Conditions/
│   │   │   │   ├── IsUnderAttackCondition.cs
│   │   │   │   ├── PlayerInRangeCondition.cs
│   │   │   │   ├── HealthBelowThresholdCondition.cs
│   │   │   │   └── IsPlayerHostileCondition.cs
│   │   │   └── Tasks/
│   │   │       ├── TakeCoverTask.cs
│   │   │       ├── ReturnFireTask.cs
│   │   │       ├── PatrolRouteTask.cs
│   │   │       ├── InvestigateTask.cs
│   │   │       ├── CallReinforcementsTask.cs
│   │   │       └── FleeTask.cs
│   │   ├── Civilian/
│   │   │   ├── CivilianAgent.cs
│   │   │   ├── CivilianFSM.cs
│   │   │   └── CivilianPersonality.cs
│   │   ├── Police/
│   │   │   ├── PoliceAgent.cs
│   │   │   ├── DispatchController.cs
│   │   │   ├── PolicePursuitController.cs
│   │   │   └── HelicopterAgent.cs
│   │   └── Faction/
│   │       ├── FactionAgent.cs
│   │       ├── FactionBTRunner.cs
│   │       └── FactionAIBlackboard.cs
│   │
│   ├── Systems/
│   │   ├── HeatSystem.cs
│   │   ├── MissionSystem.cs
│   │   ├── MissionInstance.cs
│   │   ├── ObjectiveTracker.cs
│   │   ├── EconomySystem.cs
│   │   ├── FactionSystem.cs
│   │   ├── WorldEventSystem.cs
│   │   ├── WeatherSystem.cs
│   │   ├── TimeManager.cs
│   │   ├── WorldStateManager.cs
│   │   ├── CCTVSystem.cs
│   │   ├── CoverSystem.cs
│   │   └── SaveSystem.cs
│   │
│   ├── Weapons/
│   │   ├── WeaponBase.cs
│   │   ├── WeaponFirearm.cs
│   │   ├── WeaponMelee.cs
│   │   ├── WeaponThrown.cs
│   │   ├── ProjectileBase.cs
│   │   ├── BulletProjectile.cs
│   │   └── ExplosiveProjectile.cs
│   │
│   ├── UI/
│   │   ├── HUDController.cs
│   │   ├── MinimapController.cs
│   │   ├── ObjectiveDisplay.cs
│   │   ├── HeatDisplay.cs
│   │   ├── HealthArmorDisplay.cs
│   │   ├── WeaponDisplay.cs
│   │   ├── PhoneMenuController.cs
│   │   ├── MapScreen.cs
│   │   ├── ShopUI.cs
│   │   ├── MissionBriefUI.cs
│   │   ├── PauseMenuController.cs
│   │   └── MainMenuController.cs
│   │
│   ├── Audio/
│   │   ├── AudioManager.cs                ← Singleton; SFX pool + music state machine
│   │   ├── MusicStateMachine.cs
│   │   ├── SFXPool.cs
│   │   └── AudioOcclusion.cs
│   │
│   ├── Data/
│   │   ├── Definitions/
│   │   │   ├── MissionDefinition.cs       ← C# class matching mission JSON schema
│   │   │   ├── VehicleDefinition.cs
│   │   │   ├── WeaponDefinition.cs
│   │   │   ├── FactionDefinition.cs
│   │   │   ├── DistrictDefinition.cs
│   │   │   ├── ShopDefinition.cs
│   │   │   └── WorldEventDefinition.cs
│   │   └── DataRegistry.cs               ← Loads + caches all definitions at startup
│   │
│   └── Utils/
│       ├── ExtensionMethods.cs
│       ├── MathUtils.cs
│       ├── DebugDraw.cs
│       └── Logger.cs
│
├── scenes/                                ← Godot .tscn scene files
│   ├── main/
│   │   ├── main.tscn                      ← Game entry point
│   │   ├── game_world.tscn                ← World container
│   │   └── loading_screen.tscn
│   │
│   ├── player/
│   │   ├── player.tscn
│   │   └── player_camera.tscn
│   │
│   ├── vehicles/
│   │   ├── vehicle_zuro_dart.tscn
│   │   ├── vehicle_kael_cruiser.tscn
│   │   ├── vehicle_vorn_hammer.tscn
│   │   ├── vehicle_helix7.tscn
│   │   └── ... (one .tscn per vehicle)
│   │
│   ├── characters/
│   │   ├── kael_rison.tscn               ← Player character
│   │   ├── civilian_base.tscn
│   │   ├── police_officer.tscn
│   │   ├── faction_ruin_syndicate.tscn
│   │   ├── faction_warden_bloc.tscn
│   │   ├── faction_hollow_kings.tscn
│   │   ├── faction_meridian_cartel.tscn
│   │   └── faction_axiom_directorate.tscn
│   │
│   ├── districts/
│   │   ├── district_the_pits.tscn
│   │   ├── district_the_grid.tscn
│   │   ├── district_neon_flats.tscn
│   │   ├── district_the_waterfront.tscn
│   │   └── district_the_spire.tscn
│   │
│   ├── interiors/
│   │   ├── int_warehouse_generic.tscn
│   │   ├── int_office_generic.tscn
│   │   ├── int_safehouse_basic.tscn
│   │   ├── int_club_generic.tscn
│   │   └── ... (one .tscn per interior type)
│   │
│   ├── missions/
│   │   ├── mission_cold_iron.tscn
│   │   ├── mission_glass_eyes.tscn
│   │   └── ... (one .tscn per mission with unique scene setup)
│   │
│   ├── ui/
│   │   ├── hud_main.tscn
│   │   ├── hud_vehicle.tscn
│   │   ├── phone_menu.tscn
│   │   ├── pause_menu.tscn
│   │   ├── main_menu.tscn
│   │   ├── load_game_menu.tscn
│   │   ├── shop_ui.tscn
│   │   ├── mission_brief_ui.tscn
│   │   ├── map_screen.tscn
│   │   └── faction_rep_screen.tscn
│   │
│   └── fx/
│       ├── fx_muzzle_flash.tscn
│       ├── fx_blood_hit.tscn
│       ├── fx_explosion.tscn
│       ├── fx_smoke_plume.tscn
│       ├── fx_fire_barrel.tscn
│       └── fx_vehicle_fire.tscn
│
├── assets/                                ← Raw imported assets
│   ├── characters/
│   │   ├── kael/
│   │   │   ├── mdl_character_kael_default.glb
│   │   │   ├── tex_character_kael_albedo.png
│   │   │   ├── tex_character_kael_normal.png
│   │   │   └── tex_character_kael_orm.png
│   │   ├── civilians/
│   │   │   ├── mdl_civilian_male_avg.glb
│   │   │   └── ... (per archetype)
│   │   └── factions/
│   │       ├── ruin_syndicate/
│   │       ├── warden_bloc/
│   │       ├── hollow_kings/
│   │       ├── meridian_cartel/
│   │       └── axiom_directorate/
│   │
│   ├── vehicles/
│   │   ├── zuro_dart/
│   │   │   ├── mdl_vehicle_zuro_dart_lod0.glb
│   │   │   ├── mdl_vehicle_zuro_dart_lod1.glb
│   │   │   ├── tex_vehicle_zuro_dart_albedo.png
│   │   │   ├── tex_vehicle_zuro_dart_normal.png
│   │   │   └── tex_vehicle_zuro_dart_orm.png
│   │   └── ... (one folder per vehicle)
│   │
│   ├── weapons/
│   │   ├── kp45/
│   │   ├── voss9/
│   │   ├── rk12/
│   │   └── ... (one folder per weapon)
│   │
│   ├── environment/
│   │   ├── roads/
│   │   │   ├── mdl_road_2lane_straight_8m.glb
│   │   │   └── ...
│   │   ├── buildings/
│   │   │   ├── pits/
│   │   │   ├── grid/
│   │   │   ├── neon_flats/
│   │   │   ├── waterfront/
│   │   │   └── spire/
│   │   ├── props/
│   │   │   ├── street/
│   │   │   ├── dock/
│   │   │   ├── industrial/
│   │   │   └── interior/
│   │   ├── atlases/
│   │   │   ├── tex_props_street_atlas_01_albedo.png
│   │   │   └── ...
│   │   └── decals/
│   │
│   ├── ui/
│   │   ├── hud/
│   │   │   ├── health_bar.png
│   │   │   ├── armor_bar.png
│   │   │   ├── heat_dot_active.png
│   │   │   ├── heat_dot_inactive.png
│   │   │   └── ...
│   │   ├── icons/
│   │   │   ├── icon_weapon_pistol.png
│   │   │   ├── icon_faction_ruin_syndicate.png
│   │   │   └── ...
│   │   ├── fonts/
│   │   │   ├── font_ui_primary.ttf
│   │   │   ├── font_ui_mono.ttf
│   │   │   └── font_dyslexia.ttf
│   │   └── backgrounds/
│   │
│   └── color/
│       └── master_palette.kra             ← Krita master palette file
│
├── audio/                                 ← All audio assets
│   ├── music/
│   │   ├── ambient/
│   │   │   ├── mus_ambient_the_pits_day.ogg
│   │   │   ├── mus_ambient_the_pits_night.ogg
│   │   │   ├── mus_ambient_the_grid_day.ogg
│   │   │   ├── mus_ambient_neonflats_night.ogg
│   │   │   ├── mus_ambient_waterfront_dusk.ogg
│   │   │   └── mus_ambient_the_spire.ogg
│   │   ├── tension/
│   │   │   ├── mus_tension_pits.ogg
│   │   │   └── ...
│   │   ├── pursuit/
│   │   │   ├── mus_pursuit_01.ogg
│   │   │   ├── mus_pursuit_02.ogg
│   │   │   └── mus_pursuit_03.ogg
│   │   ├── combat/
│   │   │   ├── mus_combat_01.ogg
│   │   │   └── ...
│   │   ├── stealth/
│   │   │   └── mus_stealth_01.ogg
│   │   ├── mission/
│   │   │   ├── mus_mission_cold_iron.ogg
│   │   │   └── ...
│   │   └── stings/
│   │       ├── mus_sting_victory.ogg
│   │       └── mus_sting_fail.ogg
│   │
│   ├── sfx/
│   │   ├── weapons/
│   │   │   ├── kp45/
│   │   │   │   ├── snd_weapon_kp45_shot_close.ogg
│   │   │   │   ├── snd_weapon_kp45_shot_distant.ogg
│   │   │   │   ├── snd_weapon_kp45_reload.ogg
│   │   │   │   └── snd_weapon_kp45_empty.ogg
│   │   │   └── ... (per weapon)
│   │   ├── vehicles/
│   │   │   ├── generic/
│   │   │   │   ├── snd_vehicle_tire_screech.ogg
│   │   │   │   ├── snd_vehicle_collision_light.ogg
│   │   │   │   ├── snd_vehicle_collision_heavy.ogg
│   │   │   │   └── snd_vehicle_explosion.ogg
│   │   │   ├── compact/
│   │   │   │   ├── snd_vehicle_compact_idle.ogg
│   │   │   │   └── snd_vehicle_compact_drive.ogg
│   │   │   └── ... (per class)
│   │   ├── characters/
│   │   │   ├── footsteps/
│   │   │   │   ├── snd_step_concrete_light.ogg
│   │   │   │   ├── snd_step_concrete_heavy.ogg
│   │   │   │   ├── snd_step_gravel.ogg
│   │   │   │   ├── snd_step_metal.ogg
│   │   │   │   └── snd_step_water.ogg
│   │   │   ├── kael/
│   │   │   │   ├── snd_kael_sprint_breath.ogg
│   │   │   │   ├── snd_kael_pain_01.ogg
│   │   │   │   └── snd_kael_death.ogg
│   │   │   └── npc/
│   │   │       ├── snd_civilian_scream.ogg
│   │   │       ├── snd_civilian_phone_call.ogg
│   │   │       └── ...
│   │   ├── environment/
│   │   │   ├── snd_rain_light.ogg
│   │   │   ├── snd_rain_heavy.ogg
│   │   │   ├── snd_thunder.ogg
│   │   │   ├── snd_wind.ogg
│   │   │   └── snd_fire_crackle.ogg
│   │   ├── ui/
│   │   │   ├── snd_ui_confirm.ogg
│   │   │   ├── snd_ui_cancel.ogg
│   │   │   ├── snd_ui_hover.ogg
│   │   │   ├── snd_heat_increase.ogg
│   │   │   └── snd_health_low.ogg
│   │   └── explosions/
│   │       ├── snd_explosion_vehicle.ogg
│   │       ├── snd_explosion_grenade.ogg
│   │       └── snd_explosion_barrel.ogg
│   │
│   ├── ambient/
│   │   ├── amb_district_pits_day.ogg
│   │   ├── amb_district_pits_night.ogg
│   │   ├── amb_district_grid_day.ogg
│   │   ├── amb_district_neonflats_night.ogg
│   │   ├── amb_district_waterfront.ogg
│   │   └── amb_district_spire.ogg
│   │
│   └── vo/                                ← Voice over (organized by character)
│       ├── kael/
│       │   ├── vo_kael_mission_coldironbriefing.ogg
│       │   └── ...
│       ├── dame_oskar/
│       ├── tres_morande/
│       ├── commander_strak/
│       ├── director_vale/
│       └── sable/
│
├── data/                                  ← All JSON game data
│   ├── config/
│   │   ├── game_config.json               ← Global settings (physics, timings)
│   │   ├── heat_config.json               ← Heat levels, decay rates, dispatch
│   │   ├── economy_config.json            ← Base prices, faction multipliers
│   │   └── ai_config.json                 ← NPC budgets, LOD thresholds
│   │
│   ├── missions/
│   │   ├── mission_cold_iron.json
│   │   ├── mission_glass_eyes.json
│   │   ├── mission_the_drain.json
│   │   └── ... (one JSON per mission)
│   │
│   ├── vehicles/
│   │   ├── vehicle_zuro_dart.json
│   │   ├── vehicle_helix7.json
│   │   └── ... (one JSON per vehicle)
│   │
│   ├── weapons/
│   │   ├── weapon_kp45_pistol.json
│   │   ├── weapon_voss9_smg.json
│   │   └── ...
│   │
│   ├── factions/
│   │   ├── faction_ruin_syndicate.json
│   │   ├── faction_warden_bloc.json
│   │   ├── faction_hollow_kings.json
│   │   ├── faction_meridian_cartel.json
│   │   └── faction_axiom_directorate.json
│   │
│   ├── districts/
│   │   ├── district_the_pits.json
│   │   ├── district_the_grid.json
│   │   ├── district_neon_flats.json
│   │   ├── district_the_waterfront.json
│   │   └── district_the_spire.json
│   │
│   ├── shops/
│   │   ├── shop_syndicate_weapons.json
│   │   ├── shop_general_pits.json
│   │   └── ...
│   │
│   ├── events/
│   │   ├── event_warden_incursion_pits.json
│   │   ├── event_police_sting.json
│   │   └── ...
│   │
│   ├── npcs/
│   │   ├── npc_dame_oskar.json
│   │   ├── npc_tres_morande.json
│   │   ├── npc_commander_strak.json
│   │   ├── npc_director_vale.json
│   │   └── npc_sable.json
│   │
│   └── items/
│       ├── item_medkit.json
│       ├── item_combat_stim.json
│       ├── item_armor_plate.json
│       ├── item_signal_jammer.json
│       └── item_drone.json
│
├── scripts/                               ← Build + utility scripts
│   ├── validate_data.py                   ← JSON schema validation script
│   ├── build.py                           ← Automated Godot build script
│   ├── export_atlas.py                    ← Texture atlas packing helper
│   └── check_naming.py                   ← Asset naming convention linter
│
├── tests/                                 ← Automated tests (GdUnit4)
│   ├── unit/
│   │   ├── test_heat_system.gd
│   │   ├── test_economy_system.gd
│   │   ├── test_mission_system.gd
│   │   ├── test_save_system.gd
│   │   └── test_vehicle_physics.gd
│   └── integration/
│       ├── test_crime_to_police_response.gd
│       ├── test_mission_faction_reward.gd
│       └── test_district_transition.gd
│
└── builds/                                ← Exported game builds (gitignored except manifests)
    ├── .gitkeep
    ├── windows/
    ├── linux/
    ├── macos/
    └── manifests/
        └── build_manifest_latest.json     ← Build version + hash; tracked in git
```

---

## 9.2 GITIGNORE ENTRIES

```gitignore
# Godot generated
.godot/
*.import
*.uid

# Build output
builds/windows/
builds/linux/
builds/macos/

# OS artifacts
.DS_Store
Thumbs.db
desktop.ini

# IDE
.vs/
.vscode/settings.json
*.user
*.suo

# Python cache
__pycache__/
*.pyc

# Logs
*.log

# Temp
*.tmp
*.bak
```

---

## 9.3 GIT LFS TRACKING

```gitattributes
# 3D Models
*.glb filter=lfs diff=lfs merge=lfs -text
*.fbx filter=lfs diff=lfs merge=lfs -text
*.blend filter=lfs diff=lfs merge=lfs -text

# Textures
*.png filter=lfs diff=lfs merge=lfs -text
*.jpg filter=lfs diff=lfs merge=lfs -text
*.tga filter=lfs diff=lfs merge=lfs -text
*.exr filter=lfs diff=lfs merge=lfs -text
*.ktx2 filter=lfs diff=lfs merge=lfs -text

# Audio
*.ogg filter=lfs diff=lfs merge=lfs -text
*.wav filter=lfs diff=lfs merge=lfs -text
*.mp3 filter=lfs diff=lfs merge=lfs -text

# Fonts
*.ttf filter=lfs diff=lfs merge=lfs -text
*.otf filter=lfs diff=lfs merge=lfs -text

# Compiled builds
*.exe filter=lfs diff=lfs merge=lfs -text
*.pck filter=lfs diff=lfs merge=lfs -text
*.zip filter=lfs diff=lfs merge=lfs -text
```
