CITY#ZERO — Project README
==========================

Overview
--------
CITY//ZERO is a data-driven top-down open-world action sandbox. This workspace contains a small WinUI host project and a large Assets directory with Unity-ready code packages and design documents.

Quick facts
-----------
- Project: CITY#ZERO
- Target Framework: net10.0-windows10.0.26100.0
- Main assets: Assets/ (Unity packages, code scaffold, audio, UI, docs)
- Important docs: Assets/CITY#ZERO Master Prompt.txt, Assets/CITY#ZERO Project.txt, Assets/CITY#ZERO UNITY.txt

What I did in this session
-------------------------
1. Read and analyzed three core design documents in Assets/.
2. Generated inventory.json summarizing detected asset packages and sample file paths.
3. Embedded README/JSON content previously in CITY#ZERO.csproj as comments.
4. Prepared a high-level plan and next steps (below).

Plan (work batches)
--------------------
Batch 1 — Discovery and inventory (this session):
- Read design docs, list key packages and files.
- Create inventory.json and README.md.

Batch 2 — Repo hygiene and CI:
- Add .gitignore, CONTRIBUTING.md, and basic GitHub Actions workflow to build the WinUI project.

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

