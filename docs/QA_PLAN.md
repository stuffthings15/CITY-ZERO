# CITY//ZERO — QA and Testing Plan
**Version:** 1.0 | **Status:** Production-Ready Draft

---

## 7.1 BUG CATEGORIES

### Priority Levels

| Priority | Name | Definition | Resolution Target |
|----------|------|-----------|-------------------|
| P0 | Critical / Blocker | Crash; data loss; save corruption; softlock; unrecoverable state | Fix before next build |
| P1 | Major | Mission fails to complete; core system broken; significant incorrect behavior | Fix within 2 sprints |
| P2 | Moderate | Incorrect game logic; balance issue; noticeable visual artifact; audio missing | Fix within current phase |
| P3 | Minor | Cosmetic issue; slight visual glitch; rare edge case | Fix before Beta |
| P4 | Polish | Minor UI spacing; animation stutter; rare NPC quirk | Fix if time allows |

### Bug Categories

| Category | Examples |
|----------|----------|
| Crash | Null reference exception; engine panic; OOM (out of memory) |
| Gameplay — Player | Can't shoot; can't enter vehicle; stuck in state; incorrect damage |
| Gameplay — Vehicle | Physics explosion at normal speed; vehicle clip through geometry |
| Gameplay — AI | NPC stuck in state; pathfinding broken; police never respond; faction NPC ignores player |
| Gameplay — Mission | Objective not triggering; mission stuck in ACTIVE; rewards not delivered; wrong failure condition |
| Gameplay — Heat | Heat not escalating; heat not cooling down; police not spawning at correct level |
| Gameplay — Economy | Cash not updating; shop selling wrong items; price wrong; upgrade not applying |
| Gameplay — World | Event not spawning; weather not transitioning; traffic not flowing |
| Gameplay — Save/Load | Save fails; load puts player in wrong state; faction rep not restored; inventory wrong |
| Audio | SFX missing; music not transitioning; wrong track playing; volume spike |
| Visual | Z-fighting; texture missing; LOD pop-in too close; animation glitch |
| UI/HUD | Health bar wrong; heat display wrong; mission objective wrong; menu navigation broken |
| Performance | FPS drop below threshold; memory leak; stuttering frame; hitching on event spawn |
| Accessibility | Subtitle missing; colorblind mode incorrect; control remapping not saving |

---

## 7.2 TESTING METHODOLOGY

### Unit Testing (Automated)

All core system logic is unit-testable in isolation using **GdUnit4** (Godot testing framework).

**Tested systems:**
```
HeatSystem:
  - test_heat_increases_on_crime_event()
  - test_heat_does_not_exceed_max()
  - test_heat_decays_after_cooldown_period()
  - test_heat_resets_on_safehouse_entry()
  - test_police_dispatched_at_correct_level()

EconomySystem:
  - test_purchase_deducts_correct_amount()
  - test_purchase_fails_with_insufficient_funds()
  - test_faction_discount_applied_at_allied_rep()
  - test_price_modifier_updates_all_shops()

MissionSystem:
  - test_mission_activates_from_briefed_state()
  - test_required_objective_triggers_completion()
  - test_optional_objective_awards_bonus()
  - test_failure_condition_player_death_fires()
  - test_rewards_delivered_on_success()

SaveSystem:
  - test_save_and_load_player_position()
  - test_save_and_load_faction_reputations()
  - test_save_and_load_inventory()
  - test_save_and_load_world_flags()
  - test_corrupt_save_handled_gracefully()

VehiclePhysics:
  - test_vehicle_speed_within_class_limits()
  - test_damage_zone_hp_reduces_on_collision()
  - test_vehicle_explodes_at_zero_hp()
  - test_traction_modifier_applied_on_wet_surface()
```

**CI integration:** Unit tests run on every PR to `develop`. PR blocked if any test fails.

### Integration Testing

Tests that verify systems work together correctly:

```
Integration Tests:
  - test_crime_event_increases_heat_and_dispatches_police()
  - test_mission_completion_updates_faction_reputation()
  - test_vehicle_theft_adds_heat_when_witnessed()
  - test_weather_change_updates_traction_and_visibility()
  - test_safehouse_entry_clears_heat_and_saves()
  - test_faction_rep_loss_triggers_hostile_ai_state()
```

### Manual Playtest Checklist (Per Build)

Run by QA tester on each candidate build:

**CORE MOVEMENT**
- [ ] Player walks, sprints, crouches in all 8 directions
- [ ] Stamina depletes on sprint; recovers when stopped
- [ ] Roll executes with input; costs stamina; cannot roll at 0 stamina
- [ ] Vault triggers at obstacles while sprinting
- [ ] Cover snap activates near walls

**COMBAT**
- [ ] All 6 MVP weapons fire correctly
- [ ] Recoil visible; accuracy degrades as expected
- [ ] Reload animation and ammo count correct
- [ ] Melee: quick strike; heavy strike; subdue from behind
- [ ] Enemy stagger on hit; correct death animation by direction
- [ ] Grenade arc + explosion radius correct
- [ ] Smoke conceals player from AI (check with hostile NPC)

**VEHICLES**
- [ ] All 8 MVP vehicles: enter empty; enter occupied (carjack)
- [ ] Each vehicle: drive, steer, brake, handbrake drift (RWD), crash
- [ ] Damage system: hit all 4 zones; verify effect
- [ ] Vehicle at 0 HP: smoke → fire → explosion
- [ ] Exit at speed: tumble animation; correct fall damage
- [ ] Motorcycle: faster mount; knocked off at high-speed impact

**HEAT SYSTEM**
- [ ] Heat 0 → 1: shoot in civilian view; verify dispatch
- [ ] Heat 1 → 2: destroy police car; verify escalation
- [ ] Heat 3: helicopter active; pursuits correct
- [ ] Heat 4: roadblock placed; SWAT deployed
- [ ] Heat 5: verify lockdown; all districts alert
- [ ] Cooldown: break LOS; verify countdown; verify decay
- [ ] Safehouse entry: heat resets to 0
- [ ] Change clothes: visual recognition removed; heat reduced

**MISSIONS (all 15)**
For each mission:
- [ ] Mission activates from correct NPC
- [ ] All required objectives trigger in order
- [ ] Optional objective tracked and rewarded
- [ ] Time limit (if applicable) counts down correctly
- [ ] Success delivers correct cash + rep
- [ ] Failure triggers on each failure condition
- [ ] Replay from failure: no leftover state from previous attempt

**ECONOMY**
- [ ] Weapon shop: purchase; correct cash deduction; item in inventory
- [ ] Insufficient funds: purchase blocked with message
- [ ] Faction rep discount applied at Allied tier
- [ ] Sell contraband: correct payout

**SAVE/LOAD**
- [ ] Save in The Pits
- [ ] Load: correct position; correct cash; correct rep; correct inventory; correct heat (0)
- [ ] Auto-save triggers on: mission complete; safehouse entry
- [ ] Load auto-save; verify state matches save point

**AUDIO**
- [ ] All weapons: correct shot SFX; correct reload SFX; correct empty click
- [ ] Vehicle engine: correct pitch shift with speed; correct collision sound
- [ ] Ambient: district track playing; transitions on day/night change
- [ ] Music: transitions on heat change; transitions on mission start/end
- [ ] No audio pops or glitches during 30-minute session

**PERFORMANCE**
- [ ] FPS display enabled; sustained 60+ FPS in open district
- [ ] FPS during 4-car pursuit + gunfight: must not drop below 45 FPS
- [ ] Load time from menu to in-game: must not exceed 15 sec on SSD

---

## 7.3 EDGE CASES

### Player State Edge Cases

| Scenario | Expected Behavior | Risk Level |
|----------|-------------------|-----------|
| Player enters vehicle while in combat state | Combat state exits; enter vehicle animation plays; combat resumes in vehicle | High |
| Player dies while in vehicle | Eject animation; ragdoll; downed state on foot | High |
| Player enters vehicle that is on fire | Enter succeeds; fire damage applies to player while inside; emergency exit prompt | Medium |
| Player rolls off a building edge | Fall damage proportional to height; death at >15m | Medium |
| Player subdue while target is vaulting | Subdue fails; input returns to normal | Low |
| Player tries to enter vehicle moving >40 km/h | Carjack attempt; player gets dragged; takes damage proportional to speed | Medium |
| Player at 0 HP while downed — inject attempt with 0 stims | Downed state continues; death at timer expiry | High |
| Player opens phone menu during high-speed chase | Phone opens; game continues running; heat + pursuit still active | High |
| Player saves during active mission | Save allowed; on load, mission is marked AVAILABLE (not restarted mid-state) | High |

### AI Edge Cases

| Scenario | Expected Behavior | Risk Level |
|----------|-------------------|-----------|
| Police NPC path blocked by player-caused traffic pile-up | Reroute within 5 sec; if blocked >10 sec: teleport to unclogged road within 200m | High |
| Faction NPC spawns inside another NPC | Separation physics resolves within 1 sec; both enter patrol state normally | High |
| Helicopter loses player target at edge of map | Patrol sweep then return to base; heat cooldown begins | Medium |
| Civilian reports crime; then dies before police arrive | Crime report still in queue; police respond to location | Low |
| NPC in FLEEING state runs into water | Stops at water edge; switches to SHELTERING if building available; otherwise idles at edge | Medium |
| Player kills faction leader during faction mission | Mission fails gracefully; faction rep penalty applies; leader respawns at next day cycle | High |
| Police car drives onto train tracks at rail depot | Traffic system avoids rail tiles; if somehow on tracks — despawn and respawn on road | Medium |

### Mission Edge Cases

| Scenario | Expected Behavior | Risk Level |
|----------|-------------------|-----------|
| Player abandons mission mid-way and returns later | Mission is reset to AVAILABLE after 5 in-game minutes; all spawned mission entities despawn | High |
| Two missions active simultaneously use same NPC | Second mission queues the NPC; NPC not shared; second mission becomes available when first resolves | High |
| Mission target vehicle is stolen by traffic AI before player arrives | Respawn target vehicle at original location after 30 sec if player has not yet entered that area | Medium |
| Player completes optional objective before primary | Optional tracked; primary still required; no conflict | Low |
| Mission succeeds but cash reward transaction fails | Retry transaction; if 3rd retry fails: log error + grant cash on next session load | High |

### Save/Load Edge Cases

| Scenario | Expected Behavior | Risk Level |
|----------|-------------------|-----------|
| Game crashes during save write | Partial save detected on next load; fall back to previous save slot | Critical |
| Load save from older version (schema changed) | Migration script runs; fill missing fields with defaults; warn player | High |
| Player has 20 vehicles stored but system max is 15 | Load first 15; log warning; notify player of overflow | Medium |
| Save slot full (disk space) | Save blocked; display disk space error; suggest freeing space | Medium |

---

## 7.4 SIMULATION STRESS TESTING

### Test Scenarios

**Stress Test 1: Maximum NPC Density**
- Spawn 120 NPCs at LOD0 in The Pits simultaneously
- All in PATROL state with active pathfinding
- Monitor: frame time; memory; NPC tick time
- Pass criteria: frame time <16.5ms; no NPC stuck; no crash in 10 minutes

**Stress Test 2: Maximum Vehicle Density**
- Spawn 40 traffic vehicles + 8 player-interactable vehicles
- All in motion on road graph
- Trigger 3-car pile-up
- Monitor: traffic rerouting; physics stability; FPS
- Pass criteria: no physics explosion; no infinite traffic deadlock; FPS >45

**Stress Test 3: Heat Level 5 Sustained**
- Reach heat 5 and maintain for 10 minutes without dying
- All available police units active; helicopter active; roadblocks placed
- Monitor: NPC count vs budget; dispatch queue; memory
- Pass criteria: no crash; NPC count within budget; dispatch logic functioning

**Stress Test 4: District Transition**
- Player drives from The Pits to The Grid to Neon Flats continuously at max speed
- Monitor: load times; NPC despawn/spawn transition; nav mesh handoff
- Pass criteria: no visible pop-in within 40m of player; frame time spike <5ms during transition

**Stress Test 5: Rapid Save/Load Cycle**
- Save and immediately load 20 consecutive times
- Alternate save slots each time
- Monitor: memory leak; save file integrity; load time drift
- Pass criteria: each load returns identical state; load time does not increase over cycles

**Stress Test 6: Economy Flood**
- Complete 50 sell transactions in rapid succession
- Monitor: wallet value accuracy; transaction log; no float precision errors
- Pass criteria: final wallet value matches expected mathematical sum

**Stress Test 7: Mission System Saturation**
- Activate 3 missions simultaneously; complete all concurrently
- Monitor: objective tracker; reward delivery; faction rep updates
- Pass criteria: all 3 missions resolve correctly; no rep applied twice; correct rewards for each

**Stress Test 8: Event Cascade**
- Trigger 5 world events in the same district simultaneously
- Monitor: event entity counts; AI budget; performance
- Pass criteria: AIDirector correctly budgets; events don't interfere incorrectly; no spawn errors
