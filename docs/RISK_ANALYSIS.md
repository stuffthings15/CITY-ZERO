# CITY//ZERO — Risk Analysis
**Version:** 1.0 | **Status:** Production-Ready Draft

---

## RISK REGISTER

Each risk is rated by:
- **Probability:** Low / Medium / High
- **Impact:** Low / Medium / High / Critical
- **Risk Score:** Probability × Impact (1–9 scale)
- **Status:** Unmitigated / Mitigated / Accepted

---

## 8.1 TECHNICAL RISKS

| # | Risk | Probability | Impact | Score | Mitigation |
|---|------|-------------|--------|-------|-----------|
| T1 | Godot 4 performance insufficient for 120+ NPCs + full simulation | Medium | High | 6 | Build stress tests in Phase 0; profile early; fallback: reduce max NPC count; Godot 4 Jolt physics is fast for 2.5D |
| T2 | Navigation mesh breaks on dynamic obstacle changes (destroyed vehicles, placed barriers) | High | Medium | 6 | Rebuild nav mesh in affected zone asynchronously; cache last valid mesh; test with stress test 1 |
| T3 | Vehicle physics instability at high speeds or complex collisions | High | Medium | 6 | Tune physics parameters in dedicated vehicle prototype; cap collision response forces; use discrete collision detection above 150 km/h |
| T4 | Save system data corruption on crash during write | Low | Critical | 6 | Atomic write pattern (write to temp file; rename on success); validate save on load; auto-backup previous save |
| T5 | Memory leak in long play sessions (NPC pool, audio pool) | Medium | High | 6 | Object pooling for all frequently spawned entities; memory profiling every Alpha sprint; enforce pool size hard caps |
| T6 | GDScript ↔ C# interop performance overhead at high call frequency | Medium | Medium | 4 | Profile interop boundaries early; move high-frequency logic to C#; limit GDScript to UI and non-critical scripting |
| T7 | JSON data loading time causing long initial load screen | Low | Medium | 2 | Async loading pipeline; show loading progress; pre-cache critical data at launch; binary serialize data at build time |
| T8 | Godot 4 console export limitations (PS5/Xbox) | Medium | Medium | 4 | PC is primary target; console as Phase 2; monitor Godot console export maturity; maintain clean C# codebase for potential Unity port fallback |
| T9 | AI pathfinding request backlog during heat 4–5 (too many units recalculating) | High | Medium | 6 | Path request queue with frame-distributed processing (max 30 requests/frame); cache police paths on road graph |
| T10 | Float precision errors in economy calculations over long sessions | Low | Medium | 2 | Use `decimal` type for wallet math; round to 2 decimal places on all transactions; unit test extensively |

---

## 8.2 SCOPE RISKS

| # | Risk | Probability | Impact | Score | Mitigation |
|---|------|-------------|--------|-------|-----------|
| S1 | Mission content (130+) takes longer than estimated | High | High | 9 | **Most critical risk.** Parallel mission production using data-driven pipeline; hire or contract additional designers; cut optional objectives before cutting missions; Phase 3 buffer built into timeline |
| S2 | Full 5-district world build exceeds art team capacity | High | High | 9 | Modular tile kit approach (pre-built; re-used across districts with palette swaps); hire additional environment artist for Alpha phase |
| S3 | Full voice recording budget overrun | Medium | Medium | 4 | Scope VO to main story + faction leaders only (full VO); street NPCs: barks only; plan VO budget in pre-production; use SAG-AFTRA indie agreement |
| S4 | Vehicle customization system (paint; armor; weapons) pushes Alpha | Medium | Medium | 4 | Defer cosmetic customization to post-MVP; core vehicle identity works without it; add as Beta feature |
| S5 | Multiplayer/co-op feature requests from community post-announcement | Low | High | 3 | Design decision: single-player only; stated publicly in announcement; architecture doesn't support multiplayer; hold firm |
| S6 | Post-launch DLC content not ready on announced schedule | Medium | Medium | 4 | DLC planning begins at Beta; no public schedule until content is 50% complete; maintain buffer sprint at end of each DLC cycle |
| S7 | Open-world systemic content creates exponentially more QA surface | High | Medium | 6 | Invest in automated integration tests; dedicated systems QA tester; emergent scenario database maintained during development |

---

## 8.3 DESIGN RISKS

| # | Risk | Probability | Impact | Score | Mitigation |
|---|------|-------------|--------|-------|-----------|
| D1 | Core game loop feels too similar to existing titles despite original IP | Medium | High | 6 | Regular design critique sessions (monthly); outside playtesters who are not fans of the genre; focus on differentiating systems (emergent events; faction interplay; systemic depth) |
| D2 | Heat system escalation feels arbitrary or unfair | High | Medium | 6 | Extensive playtesting of heat escalation curve; player transparency (show heat gain reason in UI); difficulty settings for heat sensitivity |
| D3 | Faction reputation system becomes too complex to track mentally | Medium | Medium | 4 | Clear UI: faction rep screen with visual bar + icon + text; reputation change notifications on screen; simplify to max 2 bleeding relationships |
| D4 | Mission design quality inconsistent across 130+ missions | High | High | 9 | Mission design template enforced; all missions reviewed by lead designer; playtest every mission individually; cut over patch rather than ship poor-quality content |
| D5 | Open-world pacing: players may feel directionless | Medium | High | 6 | Strong early game tutorial flow; ambient intel tips; radio calls from faction contacts suggest content; mini-map pins for discovered content |
| D6 | Player character Kael lacks strong enough identity | Medium | High | 6 | Strong voice direction in VO sessions; well-written faction interactions; clear character motivation stated early; player choices reflect on Kael's reputation (not blank-slate but not fully prescribed) |
| D7 | Violence tone tips from "stylized action" into gratuitous | Low | High | 3 | Editorial review of all content; no gore slider; age rating compliance check at Beta; no torture mechanics; all violence is consequential, not pornographic |
| D8 | Economy balance: player gets too rich too fast or too poor | High | Medium | 6 | Economy balancing spreadsheet; price/reward calibration pass at Beta; difficulty settings affect cash gain rate; price inflation tied to city control |
| D9 | AI behavior too predictable; emergent gameplay feels scripted | Medium | High | 6 | Randomize patrol routes per session; behavior tree has random weight selection at decision nodes; weather + time modifiers add unpredictability |
| D10 | Player finds working with all factions required (anti-choice) | Medium | Medium | 4 | Design ensures multiple paths to endgame (burn 2 factions; still completable); no faction is mandatory; Axiom is the only required confrontation |

---

## 8.4 PRODUCTION RISKS

| # | Risk | Probability | Impact | Score | Mitigation |
|---|------|-------------|--------|-------|-----------|
| P1 | Key team member leaves during development | Medium | High | 6 | Documentation-first culture; all systems documented before shipping; no single-point-of-knowledge; pair programming on critical systems |
| P2 | Publisher/funding pressure to change core design | Medium | High | 6 | Maintain clear creative vision document; agree on creative controls before signing; MVP demonstrates vision without compromise |
| P3 | Budget runs out before Alpha complete | Medium | Critical | 8 | Conservative budget planning with 20% contingency; MVP as potential release candidate if funding fails; Kickstarter/Early Access as fallback |
| P4 | Legal challenge on IP similarity claims | Low | Critical | 4 | All content documented as original; legal review of faction designs, character designs, world; IP attorney review before announcement |
| P5 | Crunch culture emergence due to scope | High | Medium | 6 | Scope is the variable, not hours; explicit no-crunch policy; cut scope on time, not people; producer owns schedule discipline |
| P6 | Asset pipeline bottleneck (one artist; many assets) | High | Medium | 6 | Modular tile kit reduces unique asset count significantly; outsource secondary props to freelancers; asset prioritization list maintained |

---

## 8.5 RISK MITIGATION SUMMARY TABLE

**Top 5 Risks by Score:**

| Risk | Score | Primary Mitigation | Owner |
|------|-------|--------------------|-------|
| S1 — Mission content overrun | 9 | Parallel data-driven pipeline + contractor designers | Lead Designer |
| S2 — Art team capacity | 9 | Modular tile kit + additional hire at Alpha | Art Director |
| D4 — Mission quality inconsistency | 9 | Design template + mandatory review | Lead Designer |
| P3 — Budget runs out | 8 | 20% contingency + MVP as fallback | Producer |
| T1 — NPC performance | 6 | Early stress testing + LOD system | Lead Programmer |

**Risk Review Cadence:**
- Phase 0–1: Monthly review
- Phase 2–3: Bi-weekly review
- Phase 4–5: Weekly review
- Any P0 risk materialization: Emergency review within 24 hours
