# CITY//ZERO

> **Top-down open-world crime-action sandbox** | Original IP | Godot 4 + .NET 10

---

## What Is CITY//ZERO?

CITY//ZERO is a systemic top-down open-world crime-action game set in **Vektor**, a fictional coastal mega-city fractured between five warring factions. You play as **Kael Rison** — a former military logistics specialist turned criminal operator who returns to a city tearing itself apart.

This repository contains the **complete production package**: game design, technical architecture, art direction, content pipeline, roadmap, QA plan, risk analysis, and implementation-ready source code.

---

## Documentation

| Document | Description |
|----------|-------------|
| [`docs/GDD.md`](docs/GDD.md) | Full Game Design Document — all systems, factions, 10 missions |
| [`docs/TDD.md`](docs/TDD.md) | Technical Design Document — architecture, data schemas, AI, performance |
| [`docs/ART_PLAN.md`](docs/ART_PLAN.md) | Art Direction + 200+ asset list + vehicle/weapon design catalog |
| [`docs/CONTENT_PIPELINE.md`](docs/CONTENT_PIPELINE.md) | Modeling, texturing, rigging, integration, naming conventions |
| [`docs/SCOPE_PLAN.md`](docs/SCOPE_PLAN.md) | MVP Vertical Slice + Full Game scope targets |
| [`docs/ROADMAP.md`](docs/ROADMAP.md) | 6-phase development roadmap with deliverables per phase |
| [`docs/QA_PLAN.md`](docs/QA_PLAN.md) | Bug categories, test methodology, edge cases, stress tests |
| [`docs/RISK_ANALYSIS.md`](docs/RISK_ANALYSIS.md) | Technical, scope, design, and production risk register |
| [`docs/FOLDER_STRUCTURE.md`](docs/FOLDER_STRUCTURE.md) | Full production folder tree |

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Engine | **Godot 4.x** |
| Gameplay logic | **C# / .NET 10** |
| Scripting / UI | **GDScript** |
| Data format | **JSON** (schema-validated) |
| Version control | **Git + Git LFS** |
| Testing | **GdUnit4** |
| CI | **GitHub Actions** |

---

## Repository Structure

```
CITY#ZERO/
├── src/          ← C# source (Core, Player, Vehicle, AI, Systems, UI, Data)
├── scenes/       ← Godot .tscn scene files
├── assets/       ← 3D models, textures, fonts, UI graphics
├── audio/        ← Music, SFX, ambient, voice over
├── data/         ← JSON data (missions, vehicles, weapons, factions, districts)
├── docs/         ← All production documentation
├── scripts/      ← Build + validation utility scripts
└── tests/        ← Automated tests (GdUnit4)
```

---

## Quick Start

```bash
# Clone
git clone https://github.com/stuffthings15/CITY-ZERO.git && cd CITY-ZERO

# Validate data files
pip install jsonschema && python scripts/validate_data.py

# Open project.godot in Godot 4.3+, then Build → Build Solution
```

---

## Core Design Pillars

| Pillar | Description |
|--------|-------------|
| Top-down readability | Every asset reads clearly from above |
| Systemic emergence | 10+ interacting systems; unscripted scenarios |
| Arcade feel, deep simulation | Fast to pick up; rewards mastery |
| Living city | Factions, economy, weather evolve independently |
| Original IP | All content is wholly original |
| Data-driven | All entities in JSON; zero hardcoded gameplay values |

---

## Faction Overview

| Faction | Territory | Tone | Leader |
|---------|-----------|------|--------|
| Ruin Syndicate | The Pits | Angry working class | Dame Oskar |
| Warden Bloc | The Grid | Corporate authoritarian | Cmdr. Strak |
| Hollow Kings | Neon Flats | Anarchist hackers | The Seven Seats |
| Meridian Cartel | The Waterfront | Pragmatic capitalists | Tres Morande |
| Axiom Directorate | The Spire | Shadow government | Director Vale |

Batch 3 — Data manifest and hash generation:
- Produce a full recursive inventory (JSON) with file sizes and SHA256 hashes for verification.

Batch 4 — Unity project scaffold verification:
- Validate asmdef boundaries, package versions, and recommended Unity settings. Produce a file that maps Unity asmdef to code namespaces.

Batch 5 — Feature implementation (example: Inventory system):
- Implement a SimpleInventorySystem adapter and unit tests inside the Assets code scaffold.

Batch 6 — CI integration for Unity (optional):
- Add CI steps for Unity project validation (using Unity CI or GitHub Actions with unity-builder).

Estimated completion: 6 batches. Progress after this session: 1/6 (~17%).

Next immediate actions I can take
--------------------------------
- Create .gitignore and CONTRIBUTING.md now.
- Produce a full recursive inventory.json with file sizes and hashes (may take time for large repo).
- Begin implementation of the in-game Inventory system (SimpleInventorySystem.cs already exists in assets—ask to wire into game bootstrap or create unit tests).

Questions for you
-----------------
1. Confirm GitHub repository name and visibility (public/private) and owner (user/org) so I can prepare a push and CI workflow.
2. Do you want full file-hash manifest included in inventory.json? (Yes/No)
3. Which batch should I start next? (.gitignore and CONTRIBUTING.md recommended)


Progress estimate
-----------------
- Total batches: 6
- Current completion: ~17%

