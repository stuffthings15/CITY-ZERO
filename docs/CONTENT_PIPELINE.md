# CITY//ZERO — Content Pipeline
**Version:** 1.0 | **Status:** Production-Ready Draft

---

## 4.1 MODELING WORKFLOW

### Software Stack

| Task | Primary Tool | Export Format |
|------|-------------|---------------|
| 3D Modeling | Blender 4.x | `.glb` (GLTF 2.0) |
| Texturing | Adobe Substance Painter / Krita (indie budget) | PNG / KTX2 |
| Rigging | Blender 4.x | Embedded in `.glb` |
| Animation | Blender 4.x | Embedded in `.glb` or `.glb` per clip |
| Sprite / UI | Aseprite + Krita | PNG spritesheets |
| VFX Particles | Godot built-in ParticleSystem2D/3D | `.tscn` |
| Audio Editing | Reaper | `.ogg` (in-game), `.wav` (source) |
| Level Design | Godot 4 Editor | `.tscn` |
| Data / Schemas | VS Code + JSON Schema | `.json` |

### Poly Budget Targets

| Asset Type | Gameplay LOD0 | LOD1 | LOD2 | Cutscene |
|------------|-------------|------|------|----------|
| Human Character | 2,800 tris | 1,200 tris | 400 tris | 8,000 tris |
| Vehicle — Compact | 3,200 tris | 1,400 tris | 600 tris | 10,000 tris |
| Vehicle — Heavy | 4,500 tris | 2,000 tris | 800 tris | 14,000 tris |
| Building — Small | 1,500 tris | 700 tris | 200 tris | N/A |
| Building — Large | 3,000 tris | 1,200 tris | 400 tris | N/A |
| Prop — Small | 200–400 tris | 100 tris | Billboard | N/A |
| Prop — Large | 600–1,200 tris | 300 tris | Billboard | N/A |
| Weapon (held) | 800 tris | 400 tris | N/A | 2,400 tris |

### Modeling Standards

1. **Origin point:** All assets with physics interaction must have origin at geometric center or logical pivot point (vehicle = center-mass; door = hinge point)
2. **Scale:** Real-world metric scale. 1 Blender unit = 1 meter. Verify before export.
3. **Rotation:** Apply all transforms (Ctrl+A in Blender) before export
4. **Normals:** All normals outward-facing. Check with face orientation overlay before export.
5. **Topology:** Quads preferred. Triangles acceptable at extremities and high-curvature areas. No N-gons.
6. **Mirror symmetry:** Finalize and apply mirror modifier before export. Symmetry is a modeling convenience, not a runtime feature.
7. **Collider mesh:** For complex shapes, create a simplified collision mesh (prefixed `COL_`) included in the GLB file as a separate object. Simple shapes use Godot's generated colliders.

---

## 4.2 TEXTURING PIPELINE

### Texture Maps Per Asset

| Map | Format | Resolution (Standard / Hero) | Usage |
|-----|--------|------------------------------|-------|
| Albedo (Base Color) | PNG / KTX2 | 1024² / 2048² | Color + transparency (alpha) |
| Normal Map | PNG / KTX2 | 1024² / 2048² | Surface detail |
| ORM (Occlusion + Roughness + Metallic) | PNG / KTX2 | 1024² / 2048² | PBR channels packed |
| Emissive | PNG (where needed) | 512² / 1024² | Neon, screens, lights |

**Packing standard for ORM:**
- R channel = Ambient Occlusion
- G channel = Roughness
- B channel = Metallic

### UV Standards

- **Primary UV (UV0):** Albedo, Normal, ORM mapping. No overlapping islands for hero assets.
- **Lightmap UV (UV1):** Auto-generated or manual; required for all static scene objects that receive baked light.
- **UV padding:** Minimum 4 texel padding between islands at 1024² resolution; 8 texel at 2048².
- **UV tile range:** 0–1 only (no UDIM tiling for base game assets; texture atlasing used instead).

### Texture Atlasing

- Environmental props share atlases by category (street props, indoor props, road markings)
- Atlas resolution: 2048² max per atlas
- Atlasing tool: Blender UV packing or custom Python script
- Vehicles: Individual textures (too many variants to atlas efficiently)

### Material Naming Convention

```
MAT_[AssetType]_[AssetName]_[Variant]
Examples:
  MAT_vehicle_helix7_base
  MAT_vehicle_helix7_damaged
  MAT_character_kael_default
  MAT_character_kael_wetweather
  MAT_prop_dumpster_rusted
  MAT_building_warehouse_exterior
```

### Color Correction and Master Palette

- All textures authored in **linear color space** in Substance Painter
- Godot project set to **linear rendering** with sRGB output
- Master palette swatches file maintained at `assets/color/master_palette.kra`
- Faction color overlays applied via **shader parameter** (not baked into texture) for easy variant generation

---

## 4.3 RIGGING AND ANIMATION

### Rig Standards

**Human Character Rig:**
```
ROOT
└── PELVIS
    ├── SPINE_01 → SPINE_02 → SPINE_03 → CHEST
    │   ├── NECK → HEAD
    │   ├── SHOULDER_L → UPPER_ARM_L → LOWER_ARM_L → HAND_L
    │   │   └── FINGER groups (simplified; 3 bones per finger)
    │   └── SHOULDER_R → UPPER_ARM_R → LOWER_ARM_R → HAND_R
    ├── HIP_L → THIGH_L → CALF_L → FOOT_L → TOE_L
    └── HIP_R → THIGH_R → CALF_R → FOOT_R → TOE_R

IK TARGETS (separate control bones):
  IK_FOOT_L, IK_FOOT_R (foot IK with pole targets)
  IK_HAND_L, IK_HAND_R (weapon hold IK)
  IK_LOOK_TARGET (head tracking)

WEAPON ATTACHMENT:
  WEAPON_HOLD_R (right hand weapon parent)
  WEAPON_HOLD_L (left hand offhand parent)
```

**Vehicle Rig (Bones needed for animation):**
- WHEEL_FL, WHEEL_FR, WHEEL_RL, WHEEL_RR (rotation + steering)
- SUSPENSION_FL/FR/RL/RR (spring travel)
- DOOR_FL, DOOR_FR (open/close animation)
- HOOD (open animation for damage state)
- TRUNK (open animation)

### Animation Workflow

1. **Block:** Rough key poses at major timing points (Blender)
2. **Spline:** Smooth curves; adjust timing with Graph Editor
3. **Polish:** Secondary motion; weight, follow-through, overlap
4. **Export per clip:** Each animation is a separate `.glb` file OR separate NLA action embedded in character `.glb`
5. **Import to Godot:** AnimationPlayer / AnimationTree; BlendTree2D for directional locomotion

### Animation State Machine (in Godot AnimationTree)

```
LOCOMOTION BLEND TREE:
  BlendSpace2D:
    X axis: Horizontal direction (-1 to 1)
    Y axis: Vertical direction (-1 to 1)
    Animations: walk_N, walk_NE, walk_E, walk_SE, walk_S, walk_SW, walk_W, walk_NW

STATE MACHINE:
  Idle ──[moving]──→ Locomotion
  Locomotion ──[sprint input]──→ Sprint
  Any ──[enter vehicle]──→ Enter Vehicle (one-shot) ──→ Drive State
  Any ──[combat]──→ Combat Blend Layer (additive)
  Any ──[downed]──→ Downed (one-shot + loop)
```

---

## 4.4 INTEGRATION STEPS

### Asset → Engine Integration Checklist

**Per 3D Asset:**
```
□ Model finalized in Blender; transforms applied; origin set
□ LOD meshes created (LOD0, LOD1, LOD2)
□ Collision mesh created (prefix COL_)
□ UV unwrapped; UV2 for lightmap (static objects only)
□ Textures exported at correct resolution; naming convention followed
□ Textures imported into Godot; compression settings applied (lossy/lossless per type)
□ Material created in Godot with correct shader (standard PBR or custom faction shader)
□ Asset imported as .glb; LODLODVariant nodes set up
□ Occluder added if asset is large building (OccluderInstance3D)
□ Physics shape assigned (CollisionShape2D/3D)
□ Asset placed in correct scene or added to instance pool
□ Named correctly per convention
□ Performance validated in-scene (no frame time spike on placement)
```

**Per Audio Asset:**
```
□ Source recorded or synthesized at 48kHz, 24-bit WAV
□ Edited in Reaper: trimmed, normalized, noise removed
□ Loop point set for looping assets (music, ambient)
□ Exported as .ogg at 192kbps (music) / 128kbps (SFX)
□ Imported to Godot in correct audio bus folder
□ AudioStream resource configured (loop, stereo/mono)
□ Assigned to AudioManager pool or direct AudioStreamPlayer
□ Tested in-engine for volume, spatialization, attenuation curve
```

**Per Mission / Data Asset:**
```
□ JSON schema validated (use JSON Schema validator; schema at data/schemas/*.schema.json)
□ Faction IDs, NPC IDs, item IDs cross-referenced against registry
□ Mission tested in-engine: all objectives trigger correctly
□ Failure conditions tested (each path individually)
□ Rewards verified to fire (cash, rep, unlock flags)
□ Optional objectives tracked and rewarded
□ Heat modifiers verified correct
```

---

## 4.5 NAMING CONVENTIONS

### Universal Rules

- **All lowercase with underscores.** No spaces, no CamelCase in file names.
- **Prefix by type, then category, then name, then variant/state.**

### File Naming

```
[type]_[category]_[name]_[variant].[ext]

MODELS:
  mdl_vehicle_helix7_base.glb
  mdl_vehicle_helix7_damaged.glb
  mdl_character_kael_default.glb
  mdl_prop_dumpster_01.glb
  mdl_building_warehouse_large.glb

TEXTURES:
  tex_vehicle_helix7_albedo.png
  tex_vehicle_helix7_normal.png
  tex_vehicle_helix7_orm.png
  tex_character_kael_albedo.png
  tex_prop_street_atlas_01_albedo.png

AUDIO:
  snd_weapon_voss9_shot_close.ogg
  snd_weapon_voss9_reload.ogg
  snd_vehicle_helix7_engine_idle.ogg
  snd_vehicle_helix7_engine_drive.ogg
  amb_district_the_pits_night.ogg
  mus_ambient_neonflats_01.ogg
  mus_combat_track_01.ogg

DATA:
  mission_cold_iron.json
  vehicle_helix7.json
  faction_ruin_syndicate.json
  district_the_pits.json
  weapon_voss9_smg.json

SCENES:
  scn_district_the_pits.tscn
  scn_vehicle_helix7.tscn
  scn_character_kael.tscn
  scn_mission_cold_iron.tscn
  scn_ui_hud_main.tscn
```

### Code Naming (C#)

```
Classes:           PascalCase          (PlayerController, HeatSystem)
Methods:           PascalCase          (AddHeat, FindNearestCover)
Private fields:    _camelCase          (_currentHeat, _playerRef)
Public properties: PascalCase          (CurrentHeat, HeatLevel)
Constants:         ALL_CAPS_SNAKE      (MAX_HEAT_LEVEL, DEFAULT_SPEED)
Interfaces:        IPascalCase         (IDamageable, IInteractable)
Enums:             PascalCase type;    PascalCase values
Events/Signals:    PascalCase + Event  (HeatChangedEvent, PlayerDiedEvent)
```

---

## 4.6 VERSION CONTROL

### Repository Structure

**Git-based** (GitHub as primary remote; matches current repo setup)

### Branching Strategy (Git Flow adapted)

```
main          - Production-stable; tagged releases only
develop       - Integration branch; all features merge here first
feature/*     - Per-feature branches (feature/heat-system, feature/vehicle-physics)
content/*     - Content branches (content/mission-cold-iron, content/district-pits)
art/*         - Art asset branches (art/character-kael, art/vehicle-helix7)
bugfix/*      - Bug fixes (bugfix/police-pathfinding-null-ref)
release/*     - Release preparation (release/0.3.0)
hotfix/*      - Emergency fixes to main (hotfix/crash-on-save)
```

### Branch Rules

- `main`: Protected. No direct push. Requires 1 reviewer approval + passing CI.
- `develop`: Protected. No direct push. Requires passing CI.
- Feature/content/art branches: Open. Merge via PR to `develop`.
- Commit messages follow **Conventional Commits**: `type(scope): description`
  - `feat(heat-system): add decay rate config per heat level`
  - `fix(vehicle): null ref on exit at 0 hp`
  - `content(mission): add cold iron mission data file`
  - `art(vehicle): add helix7 LOD1 mesh`

### Large File Handling (Git LFS)

Track binary/large files with Git LFS:
```
*.glb filter=lfs diff=lfs merge=lfs -text
*.png filter=lfs diff=lfs merge=lfs -text
*.ogg filter=lfs diff=lfs merge=lfs -text
*.wav filter=lfs diff=lfs merge=lfs -text
*.mp3 filter=lfs diff=lfs merge=lfs -text
*.tscn.import filter=lfs diff=lfs merge=lfs -text
```

### Asset Review Process

1. Artist creates `art/[asset-name]` branch
2. Imports asset to Godot; runs in-engine sanity check
3. Opens PR to `develop` with screenshots (top-down view + close-up)
4. Art Director reviews: naming, LODs, poly budget, textures
5. Approved → merge; delete branch
6. CI runs Godot headless import validation (no broken references)

### CI/CD Pipeline (GitHub Actions)

```yaml
# .github/workflows/ci.yml (generated separately)
on: [push, pull_request]
jobs:
  validate:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install Godot 4
        run: # download Godot headless binary
      - name: Run JSON schema validation
        run: python scripts/validate_data.py
      - name: Run Godot import check
        run: godot --headless --import
      - name: Run unit tests (GdUnit4)
        run: godot --headless -s addons/gdunit4/runtest.gd
```
