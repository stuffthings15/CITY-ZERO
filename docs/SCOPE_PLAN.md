# CITY//ZERO — MVP + Full Scope Plan
**Version:** 1.0 | **Status:** Production-Ready Draft

---

## 5.1 MVP — VERTICAL SLICE

**Goal:** A fully playable, polished vertical slice demonstrating all core systems in one district. Suitable for internal review, publisher demo, and crowdfunding proof-of-concept.

**Target Play Time:** 45–90 minutes of content; infinitely replayable in free-roam mode

---

### MVP Scope

#### WORLD
- **1 District:** The Pits (full build; all streets, buildings, interiors, ambient population)
- **1 Ring Highway segment** connecting to blocked-off district boundaries (implying the rest of the world)
- Day/night cycle functional
- Weather system: clear + rain states only (fog + storm post-MVP)
- Traffic simulation active

#### FACTIONS (2)
- **Ruin Syndicate** — fully functional; all services; full reputation system
- **Warden Bloc** — present as antagonist/external threat; not fully playable ally
- Faction AI in The Pits active and reactive

#### VEHICLES (8)
| # | Name | Class | Availability |
|---|------|-------|-------------|
| 1 | ZURO Dart | Compact | Spawn + steal |
| 2 | KAEL Cruiser | Sedan | Spawn + steal |
| 3 | VORN Hammer | Muscle | Steal |
| 4 | BADER Bulk | SUV | Steal / Warden faction |
| 5 | RELO Carry | Van | Spawn + mission use |
| 6 | TERN Blade | Motorcycle | Spawn + steal |
| 7 | SECTOR Response | Police | Police faction; steal-able |
| 8 | VANE Buggy | Off-road | Pits spawn; steal |

Full handling model; damage system; enter/exit logic active for all 8

#### WEAPONS (6)
| # | Name | Category |
|---|------|----------|
| 1 | KP-45 | Pistol |
| 2 | VOSS-9 | SMG |
| 3 | RK-12 | Shotgun |
| 4 | SABER-X | Assault Rifle |
| 5 | FRAG grenade | Thrown |
| 6 | SMOKE canister | Thrown |

Full fire mechanics; ammo economy; weapon shop functional

#### MISSIONS (15)

| # | Mission | Type | Giver |
|---|---------|------|-------|
| 1 | Cold Iron | Heist | Ruin Syndicate |
| 2 | Pressure Drop | Debt Collection | Ruin Syndicate |
| 3 | Signal Break | Sabotage | Hollow Kings contact (cameo) |
| 4 | First Contact | Tutorial/Intro | SABLE |
| 5 | Wheels Up | Courier | Ruin Syndicate |
| 6 | The Squeeze | Escort | Ruin Syndicate |
| 7 | Dead Drop | Surveillance | SABLE |
| 8 | Garage Wars | Territory War | Ruin Syndicate |
| 9 | Night Run | Racing | Open World Discovery |
| 10 | Burn Notice | Ambush | Ruin Syndicate |
| 11 | Bad Debt | Assassination | Independent job board |
| 12 | Under the Radar | Undercover | Ruin Syndicate |
| 13 | Red Freight | Heist | Ruin Syndicate |
| 14 | The Warden Comes | Story (reactive to events) | SABLE |
| 15 | Hold the Line | Territory War (defense) | Ruin Syndicate |

All 15 missions fully playable; optional objectives on all; tiered rewards

#### FULL GAMEPLAY LOOP VALIDATION
- ✅ Cash economy working: earn → spend → upgrade
- ✅ Reputation working: Ruin Syndicate +/- reacts to player choices
- ✅ Heat system: all 5 levels; police spawn/pursue; cooldown/evasion
- ✅ Upgrade tree: all 4 trees accessible (Tier 1 + Tier 2 only in MVP)
- ✅ Safehouse system: 2 safehouses in district; rest/save/store vehicle
- ✅ Save/Load: 3 slots + auto-save
- ✅ Emergent events: at least 4 event types active (Warden incursion, police sting, race event, faction fight)
- ✅ HUD: complete
- ✅ Phone menu: map, contacts, intel
- ✅ Audio: full SFX + district ambient + adaptive music

---

## 5.2 FULL GAME SCOPE

### World
- 5 districts (all fully built)
- Ring highway complete
- Sewer/tunnel network (traversable; connects 3 districts underground)
- Harbor waterway (navigable by boat; connects Waterfront to Pits)
- 4 purchasable safehouses per district (20 total)
- All weather states: clear, overcast, light rain, heavy rain, fog, storm

### Factions (All 5)
- Ruin Syndicate: Full story arc (8 missions); all services
- Warden Bloc: Full story arc (8 missions); Kael can ally cautiously
- Hollow Kings: Full story arc (8 missions); hacker services full
- Meridian Cartel: Full story arc (8 missions); import catalog full
- Axiom Directorate: Threat system; 5 counter-missions; final arc (4 missions)

### Vehicles (27 full roster)
- All 27 from design list
- Per-vehicle customization: paint color (16 options per vehicle); weapon mount (late unlock); armor plating (increases durability)

### Weapons (16 + upgrades)
- All 16 weapons
- Full upgrade tree per weapon (suppressor, extended mag, sight, grip)
- Weapon customization shop in each district

### Missions (Full Count)
| Category | Count |
|----------|-------|
| Main Story Missions | 30 |
| Faction Arc Missions (4 factions × 8) | 32 |
| Side Missions (jobs, open world) | 40+ |
| Racing Events | 10 |
| Street Events (emergent; repeatable) | 20+ |
| **Total Designed Content** | **130+** |

### Progression
- All 4 upgrade trees; all 3 tiers each
- All skill unlock triggers active
- Full faction reputation system across all 5
- City Influence system: player can own territory (affects income, safety, events)

### Systems (Full)
- All 12 emergent systems active
- Full live event pool (20+ event types)
- Dynamic economy (faction territory affects prices)
- Civilian NPC scheduling (daily routines; 6 time slots)
- Full CCTV system (cameras per district; hackable; destroyable)
- Drone item (player reconnaissance tool; range upgradeable)
- Contraband system (fence stolen goods; market price fluctuates)
- Underground network (buy illegal services; expand with rep)

### Technical Full Scope
- Full 5-district navigation mesh
- Full road graph for all districts
- 120+ NPC active at full LOD budget
- All audio: full music catalog + SFX library
- Accessibility: colorblind mode, subtitle system, aim assist, controller remap, dyslexia font option
- Platform targets: PC (Windows/Linux/Mac), Console (PS5/Xbox Series via Godot export — Phase 2)
- Languages: English (full); subtitle localization for 8 languages (Phase 2)

### Full Content Counts Summary

| Category | MVP | Full Game |
|----------|-----|-----------|
| Districts | 1 | 5 |
| Playable Factions | 1 | 4 (+1 antagonist) |
| Vehicles | 8 | 27 |
| Weapons | 6 | 16 + upgrades |
| Missions | 15 | 130+ |
| Safehouses | 2 | 20 |
| Upgrade Tiers | 1–2 | 1–3 (all trees) |
| World Events | 4 types | 20+ types |
| Weather States | 2 | 6 |
| Music Tracks | 8 | 35+ |
| NPC Types | 12 | 40+ |
| Building Interiors | 8 | 50+ |
