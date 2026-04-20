# CITY//ZERO — Development Roadmap
**Version:** 1.0 | **Status:** Production-Ready Draft

---

## OVERVIEW

| Phase | Duration | Team Size | Primary Goal |
|-------|----------|-----------|-------------|
| 0 — Prototype | 8 weeks | 2–3 | Validate core feel |
| 1 — Pre-Production | 12 weeks | 4–6 | Systems foundation + pipeline |
| 2 — Vertical Slice | 16 weeks | 6–10 | MVP complete; demo-ready |
| 3 — Alpha | 24 weeks | 10–15 | All districts; all systems |
| 4 — Beta | 16 weeks | 10–15 | Content complete; QA focus |
| 5 — Release | 8 weeks | 8–12 | Polish; certification; launch |
| **Total** | **~84 weeks (~21 months)** | | |

*Note: Timeline assumes indie team with dedicated leads per discipline. Solo or 2-person team: multiply by 2.5–3x.*

---

## PHASE 0 — PROTOTYPE
**Duration:** 8 weeks  
**Team:** 2–3 (Lead programmer + generalist artist + optional designer)  
**Budget Focus:** Minimal; placeholder assets acceptable

### Goal
Prove the core game loop feels good. Nothing else matters yet.

### Week-by-Week Deliverables

| Week | Deliverable |
|------|-------------|
| 1–2 | Godot project setup; player movement prototype (top-down; 8-dir); placeholder character |
| 2–3 | Basic shooting mechanic; 1 weapon (pistol); hit detection; placeholder enemy |
| 3–4 | Vehicle enter/exit prototype; basic vehicle physics; 1 car driving |
| 4–5 | Basic heat system (3 levels); placeholder police car pursuit |
| 5–6 | Placeholder district (flat blocks); traffic prototype (10 cars on loop) |
| 6–7 | 1 mission prototype (reach → kill → escape); basic reward display |
| 7–8 | Playtest internal; document what feels good vs bad; go/no-go decision |

### Exit Criteria
- [ ] Movement feels responsive and readable
- [ ] Shooting feels impactful
- [ ] Car chase is exciting and fair
- [ ] Basic mission loop is satisfying in 10 minutes of play
- [ ] Team is aligned on tone and direction

---

## PHASE 1 — PRE-PRODUCTION
**Duration:** 12 weeks  
**Team:** 4–6 (Lead programmer, systems programmer, lead artist, environment artist, sound designer, game designer)  
**Budget Focus:** Pipeline investment; tool setup; no shippable content yet

### Goal
Lock all design decisions. Build production pipeline. Establish all technical foundations. Create reusable systems that content plugs into.

### Milestone Deliverables

**Weeks 1–3: Foundation**
- [ ] All core C# systems architected and stubbed (Player, Vehicle, AI, Heat, Mission, Economy)
- [ ] GameBus (event system) operational
- [ ] ServiceLocator pattern established
- [ ] JSON schema for all data types finalized and documented
- [ ] Data loader + validator operational
- [ ] Godot project structure matches folder convention (see Section 9)
- [ ] Git LFS configured; branching strategy documented; CI stub operational
- [ ] Art style guide finalized (2 pages; color palettes; reference board)

**Weeks 4–6: Systems Implementation**
- [ ] Player controller: full movement + stamina + all interactions stubbed
- [ ] Vehicle system: full physics for 2 vehicle classes; damage system
- [ ] AI FSM for civilians; behavior tree framework for faction NPCs
- [ ] Heat system: all 5 levels; police dispatch; cooldown; evasion hooks
- [ ] Mission system: lifecycle working; 1 test mission end-to-end
- [ ] Economy: wallet; shop transaction; price table loaded from JSON

**Weeks 7–9: Art Pipeline**
- [ ] Blender → Godot import workflow validated
- [ ] LOD generation pipeline documented
- [ ] First full character (Kael placeholder) rigged + basic animations in engine
- [ ] First full vehicle (KAEL Cruiser) modeled, textured, in engine with full damage model
- [ ] District tile kit started: road segments, sidewalk, 3 building shells

**Weeks 10–12: Polish + Lock**
- [ ] GDD locked (no major design changes after this point without formal change request)
- [ ] All data schemas locked
- [ ] Naming conventions enforced (lint pass on existing files)
- [ ] Audio system: AudioManager operational; placeholder SFX for all categories
- [ ] HUD prototype: health, heat, weapon display, minimap (placeholder map texture)
- [ ] Save system: functional end-to-end with test data
- [ ] Pre-production review: all leads sign off

### Exit Criteria
- [ ] All 7 core systems implemented and testable
- [ ] 1 full playable test mission with real assets
- [ ] Pipeline is documented and any new team member can follow it
- [ ] GDD is locked; TDD is complete

---

## PHASE 2 — VERTICAL SLICE
**Duration:** 16 weeks  
**Team:** 6–10 (add: 2nd environment artist, animator, 2nd programmer)  
**Budget Focus:** Full asset production for 1 district; mission content

### Goal
Ship the MVP (The Pits; 2 factions; 8 vehicles; 6 weapons; 15 missions). Demo-quality. No placeholder content in deliverable.

### Milestone Deliverables

**Weeks 1–4: District Construction**
- [ ] The Pits fully blocked out in Godot (all roads, blocks, key locations)
- [ ] Navigation mesh baked; traffic system running on Pits road graph
- [ ] Civilian population: 3 archetype models; patrol routes; all FSM states
- [ ] Ruin Syndicate NPCs: full model; faction BT operational in territory

**Weeks 5–8: Content Production**
- [ ] All 8 MVP vehicles: modeled, textured, LODs, in engine
- [ ] All 6 MVP weapons: modeled, textured, in engine; full fire mechanics
- [ ] Weapon shop scene: functional; inventory; UI
- [ ] Safehouse scene: 2 safehouses; save/rest/store vehicle functionality
- [ ] Full HUD: all elements live-connected to game systems
- [ ] Phone menu: map, contacts, intel tabs

**Weeks 9–12: Mission Production**
- [ ] All 15 missions: data JSON complete; scene setup; testing
- [ ] Mission briefing UI: contact call + text brief
- [ ] Objective tracker: HUD integration
- [ ] Mission success/failure screens
- [ ] Reward delivery: cash + rep + unlocks all verified

**Weeks 13–14: Audio Pass**
- [ ] All weapon SFX recorded/sourced and integrated
- [ ] All vehicle engine sounds: idle + drive + redline per class
- [ ] District ambient track: The Pits night + day variants
- [ ] Adaptive music system: all states; correct transitions
- [ ] UI audio: all HUD feedback sounds

**Weeks 15–16: Polish + Demo Prep**
- [ ] Full playtesting pass: all 15 missions; all vehicles; all systems
- [ ] Bug fixing sprint: target 0 P0 bugs; <10 P1 bugs
- [ ] Performance pass: 60 FPS sustained on mid-spec target hardware
- [ ] Final art pass: lighting, weather, particle effects
- [ ] Demo build: packaged for Windows; 1-click launch

### Exit Criteria
- [ ] MVP fully playable; all 15 missions completable without blockers
- [ ] 60 FPS on GTX 1060 at 1080p
- [ ] No crashes in 2-hour session
- [ ] Positive playtest reception on core loop feel

---

## PHASE 3 — ALPHA
**Duration:** 24 weeks  
**Team:** 10–15 (add: additional designers, more artists, QA lead)  
**Budget Focus:** Full world; all systems; all factions; bulk of content

### Goal
All 5 districts built. All 5 factions operational. Full mission count (~80%). All systems at full scope.

### Major Milestones

**Months 1–2: Districts 2 + 3**
- The Grid: full build; Warden Bloc AI; faction story arc missions 1–4
- Neon Flats: full build; Hollow Kings AI; faction story arc missions 1–4
- Ring highway segment connecting all 3 districts

**Months 3–4: Districts 4 + 5**
- The Waterfront: full build; Meridian Cartel AI; faction arc missions 1–4
- The Spire: full build; Axiom AI; first story incursion missions
- Full ring highway complete
- Underground sewer network traversable

**Months 5–6: Systems Completion**
- [ ] All 27 vehicles in engine
- [ ] All 16 weapons in engine; full upgrade trees
- [ ] City Influence system
- [ ] Full live events system (20+ event types)
- [ ] CCTV system; signal jammer item
- [ ] Drone item
- [ ] Contraband / fence system
- [ ] Full NPC scheduling (daily routines)
- [ ] Faction territory control system
- [ ] All remaining upgrade tree nodes
- [ ] Full save system: all fields saved and loaded correctly

### Exit Criteria (Alpha)
- [ ] All 5 districts accessible and populated
- [ ] All 5 factions operational
- [ ] ~80% of missions implemented (100+ out of 130+)
- [ ] All core systems at full scope
- [ ] No P0 crashes; moderate P1/P2 bug count acceptable
- [ ] Internal Alpha Review session: 4-hour playthrough

---

## PHASE 4 — BETA
**Duration:** 16 weeks  
**Team:** 10–15 (QA ramp up; artists on polish)  
**Budget Focus:** Content completion; QA; optimization; accessibility

### Goal
Content complete. QA-driven. All bugs triaged. Performance targets met.

### Milestone Deliverables

**Weeks 1–4: Content Completion**
- [ ] All 130+ missions complete and tested
- [ ] All faction arcs: final missions implemented
- [ ] Main story missions 25–30 complete (endgame)
- [ ] All audio: complete library; all music tracks
- [ ] All VO: recorded; implemented; subtitles synced

**Weeks 5–8: QA Pass 1**
- [ ] Full regression: all missions; all systems
- [ ] Bug database triaged: all P0 fixed; P1 fixed; P2 documented
- [ ] Performance profiling: all districts at 60 FPS target
- [ ] Memory profiling: peak RAM < 4GB on target spec
- [ ] Accessibility review: colorblind; subtitle; aim assist; remapping

**Weeks 9–12: External Beta**
- [ ] Beta build distributed to 50+ testers
- [ ] Feedback aggregated and prioritized
- [ ] Balance pass: weapon damage; economy; heat escalation
- [ ] Difficulty tuning: mission difficulty curve
- [ ] UI/UX pass based on feedback

**Weeks 13–16: Polish + Cert Prep**
- [ ] All remaining bugs fixed
- [ ] Console certification prep (TRC/TCR compliance if targeting console)
- [ ] Final performance pass
- [ ] Age rating submission (ESRB / PEGI)
- [ ] Build validation: all platforms; all languages

### Exit Criteria (Beta)
- [ ] All content complete
- [ ] 0 P0 bugs; <5 P1 bugs
- [ ] 60 FPS verified on all target platforms
- [ ] Age rating received
- [ ] Build ready for certification

---

## PHASE 5 — RELEASE
**Duration:** 8 weeks  
**Team:** 8–12 (core team; external support for marketing)  
**Budget Focus:** Launch preparation; day-1 patch; post-launch monitoring

### Milestone Deliverables

**Weeks 1–3: Gold Candidate**
- [ ] Final certification submission (console)
- [ ] PC build: Steam/GOG package; Steamworks SDK integrated (achievements; cloud save)
- [ ] Launch trailer final cut
- [ ] Press kit finalized
- [ ] Store page complete (all metadata, screenshots, description)

**Weeks 4–5: Launch Readiness**
- [ ] Gold candidate approved
- [ ] Day-1 patch prepared (known post-cert issues)
- [ ] Server/backend ready (if online features)
- [ ] Internal launch drill: validate patch pipeline

**Weeks 6–7: Launch**
- [ ] Release day: PC launch
- [ ] Monitor crash reports; player feedback
- [ ] Day-1 patch: deploy if needed
- [ ] Community management: Discord, Steam forums, socials

**Week 8: Post-Launch Stability**
- [ ] Patch 1.0.1: stability + critical bug fixes
- [ ] Post-mortem begins
- [ ] DLC/expansion planning begins

### Post-Launch Content Plan
- **Month 1:** Free update — additional street events; vehicle customization expansion
- **Month 3:** DLC Pack 1 — New district expansion (The Underbridge — black market zone)
- **Month 6:** DLC Pack 2 — Faction story expansion (Axiom Directorate backstory; new antagonist)
