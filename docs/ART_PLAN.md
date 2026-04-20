# CITY//ZERO — Art & Asset Production Plan
**Version:** 1.0 | **Status:** Production-Ready Draft

---

## 3.1 ART DIRECTION

### Visual Style Definition

**Style:** Stylized realism with top-down 2.5D perspective. Not photorealistic; not cartoon. Readable silhouettes at zoom. High contrast between elements for gameplay clarity.

**Key Visual Rules:**
1. **Top-down readability is sacred.** Every asset must be identifiable at 200m camera height. No detail that only reads at eye level.
2. **Faction color coding:** Every faction has a dominant color. Their territory, vehicles, and NPCs carry that color in some element.
3. **Gameplay elements must contrast against environment.** Enemies use contrast against district palette. Items glow subtly. Interactive objects have a consistent highlight rule.
4. **Light sources are diegetic.** All lighting comes from in-world sources: streetlights, neon signs, fire, vehicle headlights.
5. **Weathering is economic.** Richer districts are cleaner. Poorer districts are more worn.
6. **No bloom overload.** Neon Flats uses neon but through color contrast + emissive control, not HDR overkill.

### Color Theory

**Per-District Palette:**

| District | Dominant | Accent | Shadow | Ambient Light |
|----------|----------|--------|--------|---------------|
| The Pits | Rust brown `#8B4513` | Industrial orange `#FF6600` | Dark charcoal `#1C1A18` | Amber sodium lamp |
| The Grid | Steel blue `#4A6FA5` | Chrome white `#E8EBF0` | Cold grey `#2A2D35` | Cool fluorescent |
| Neon Flats | Near-black `#0D0D1A` | Electric violet `#8B00FF` + neon pink `#FF0080` | Deep purple `#1A0033` | Mixed neon |
| The Waterfront | Sea grey `#5F7A7A` | Crane orange `#FF8C00` | Navy `#1A2535` | Harbor floodlight yellow |
| The Spire | Gunmetal `#4A4A5A` | Axiom crimson `#C41230` | Almost black `#0A0A12` | Cold clinical white |

**Faction Color Identity:**

| Faction | Primary | Accent | Used On |
|---------|---------|--------|---------|
| Ruin Syndicate | Black `#1A1A1A` | Rust orange `#C85A00` | Clothing trim, vehicle markings, tags |
| Warden Bloc | Deep navy `#1B2A4A` | Silver `#C0C8D0` | Uniforms, vehicle paint, signs |
| Hollow Kings | White `#F5F5FF` | Electric blue `#00BFFF` | Graffiti, clothing, light strips |
| Meridian Cartel | Forest green `#2D5A27` | Gold `#D4AF37` | Suits, vehicle accents, crate markings |
| Axiom Directorate | Charcoal `#3A3A45` | Crimson `#C41230` | Insignia, drone chassis, facility markings |

### Lighting Rules

- **Day:** Sun angle fixed at 45° overhead; soft shadows; ambient occlusion on all objects
- **Night:** Streetlights every 30m on main roads; faction territory has colored ambient glow (faction accent color at 10% opacity); player area illuminated subtly from below camera
- **Interior:** Each interior has 1 dominant light source type (fluorescent, incandescent, neon, fire)
- **Weather overlay:** Rain adds wet-surface specularity; fog adds global fog volume at 30% density; storm drops global brightness 40%
- **Vehicle headlights:** Cast real-time 2D projected light cones; max 8 vehicle lights rendered at full quality simultaneously

---

## 3.2 ENVIRONMENT ASSETS

### Full Categorized Asset List (200+ items)

#### ROADS & INFRASTRUCTURE (30 items)
1. Road segment — 2-lane straight (4m, 8m, 16m variants)
2. Road segment — 4-lane straight
3. Road segment — intersection 4-way
4. Road segment — intersection T-junction
5. Road segment — curved (45°, 90°)
6. Road segment — elevated highway straight
7. Road segment — elevated highway curve
8. Road segment — highway on-ramp
9. Sidewalk tile — standard
10. Sidewalk tile — cracked/damaged
11. Crosswalk marking
12. Lane marking — solid white
13. Lane marking — dashed
14. Traffic signal — standard (3-state)
15. Traffic signal — pedestrian crosswalk
16. Streetlight — industrial post (Pits style)
17. Streetlight — modern post (Grid style)
18. Streetlight — neon tube cluster (Neon Flats)
19. Streetlight — dock lamp (Waterfront)
20. Road barrier — concrete block
21. Road barrier — metal jersey
22. Road barrier — police car barricade (dynamic)
23. Manhole cover
24. Storm drain
25. Road crack decal (set of 8 variants)
26. Pothole decal
27. Wet puddle decal (rain condition)
28. Rail track — straight
29. Rail track — curved
30. Rail track — switch

#### BUILDINGS — EXTERIOR SHELLS (25 items)
31. Residential block — low-rise (Pits)
32. Residential block — mid-rise (Neon Flats)
33. Office tower — low (Grid)
34. Office tower — high (Grid)
35. Warehouse exterior — large
36. Warehouse exterior — small
37. Factory building — derelict
38. Club/bar facade (Neon Flats)
39. Shop front — generic
40. Shop front — boarded up
41. Police precinct exterior
42. Hospital exterior
43. Church/community hall
44. Parking structure — multi-story
45. Gas station (functioning + derelict variants)
46. Diner/fast food exterior
47. Hotel exterior
48. Cartel dock office
49. Warden security post exterior
50. Axiom administrative building
51. Construction site scaffolding
52. Rooftop HVAC cluster
53. Rooftop water tower
54. Rooftop access stairwell
55. Building facade — graffiti-tagged variants (6)

#### BUILDING — INTERIORS (20 items)
56. Generic office interior kit (walls, cubicles, desks, monitors)
57. Warehouse interior — open floor
58. Warehouse interior — shelved storage
59. Apartment interior — small
60. Club interior (bar, booth, dancefloor tiles)
61. Police precinct interior (cells, desks, holding area)
62. Server room interior
63. Underground lab interior
64. Chop shop interior (lifts, tools, car bays)
65. Cartel dock warehouse interior
66. Axiom facility interior (clinical, minimal)
67. Safehouse interior — basic
68. Safehouse interior — upgraded
69. Shop interior — weapons
70. Shop interior — general
71. Sewage tunnel segment
72. Sewage junction
73. Underground parking level
74. Subway station (background)
75. Elevator lobby

#### PROPS — STREET (30 items)
76. Dumpster — standard
77. Dumpster — overflowing
78. Fire hydrant
79. Mailbox
80. Newspaper stand
81. Bench — metal (Grid style)
82. Bench — wooden (Pits style)
83. Bus stop shelter
84. Phone booth (functioning/broken)
85. Trash bag pile
86. Cardboard box pile
87. Wooden pallet stack
88. Metal barrel (empty)
89. Metal barrel (flammable — can explode)
90. Chain-link fence segment
91. Wooden fence segment
92. Brick wall segment (low)
93. Concrete wall segment (high)
94. Razor wire roll (top of fence)
95. Bollard — concrete
96. Bollard — retractable
97. Street vendor cart
98. Newspaper pile
99. Shopping cart (abandoned)
100. Torn awning
101. Graffiti mural (set of 12 unique designs)
102. Billboard frame — large
103. Billboard face — corporation ad
104. Billboard face — Hollow Kings propaganda
105. Neon sign — bar
106. Neon sign — shop

#### PROPS — DOCK/INDUSTRIAL (20 items)
107. Shipping container — standard (20ft)
108. Shipping container — open
109. Shipping container — Cartel-marked
110. Crane — dock gantry (large; interactive)
111. Forklift
112. Hand truck (pallet jack)
113. Oil drum — standard
114. Oil drum — leaking
115. Cable spool — large
116. Industrial pipe section
117. Pipe junction cluster
118. Electrical junction box
119. Fuse box panel
120. Generator — portable
121. Generator — industrial fixed
122. Fuel pump — dock
123. Wooden dock platform
124. Dock cleat and rope
125. Anchor chain segment
126. Life preserver ring

#### VEGETATION AND NATURE (15 items)
127. Street tree — mature (Grid style)
128. Street tree — scrubby (Pits)
129. Potted plant — corporate lobby
130. Weed patch — cracked concrete
131. Overgrown lot — ground cover
132. Ivy wall covering
133. Harbor seaweed (shoreline)
134. Tide pool (low tide decal)
135. Dead tree (Pits industrial area)
136. Hedge — trimmed (Grid)
137. Planter box — stone
138. Planter box — metal (Warden territory)
139. Grass patch — park
140. Puddle decal set (post-rain)
141. Mud decal (Pits / construction)

#### VEHICLES — ENVIRONMENTAL (15 items — parked/wrecked)
142. Wrecked civilian sedan (smoking)
143. Wrecked civilian compact
144. Abandoned motorcycle
145. Burned-out car shell
146. Derelict van (Pits)
147. Parked police cruiser
148. Parked Warden security SUV
149. Parked Syndicate truck
150. Cartel cargo van
151. Harbor tugboat
152. Small fishing boat
153. Docked freighter (background)
154. Speedboat at dock
155. Wrecked bicycle
156. Abandoned shopping cart

#### LIGHTING AND FX ELEMENTS (15 items)
157. Streetlight cone — sodium (warm)
158. Streetlight cone — cool (Grid)
159. Neon sign flicker effect
160. Fire barrel
161. Burning trash heap
162. Campfire (NPC gathering)
163. Muzzle flash decal
164. Bullet impact sparks
165. Smoke plume — light
166. Smoke plume — heavy (vehicle/building fire)
167. Explosion flash
168. Helicopter spotlight cone
169. CCTV camera indicator (blinking red)
170. Police light bar pulse (red/blue)
171. Axiom drone indicator light

#### WATER AND ENVIRONMENT (10 items)
172. Bay water tile (animated)
173. Canal water tile
174. Storm drain flow (rain activated)
175. Puddle tile — large
176. Flooding decal — street
177. Wake effect — boat
178. Shoreline rocks
179. Harbor dock edge
180. Pier support pillar
181. Mooring line

#### DECALS AND OVERLAYS (20 items)
182. Faction tag set — Ruin Syndicate (5 designs)
183. Faction tag set — Hollow Kings (5 designs)
184. Bullet hole decal — wall (small, medium, large)
185. Blood decal — small (tasteful; gameplay feedback)
186. Tire skid mark — straight
187. Tire skid mark — curve
188. Explosion scorch mark
189. Oil spill decal
190. Water stain decal
191. Rust streak decal
192. Police tape
193. Evidence marker
194. Crack decal — window
195. Broken glass scatter
196. Graffiti fill tag (3 variants)
197. Warning striping — industrial
198. Faction territory marker — painted
199. Faction territory marker — sign
200. Road work arrow

---

## 3.3 CHARACTERS

### Modular Character System

All human characters use a modular layered assembly:

```
BASE BODY
  ├── Body mesh (M/F variants; 3 builds: slim, average, heavy)
  ├── Skin texture (12 tones, fully parameterized)
  └── Base topology: 2,800 tris (gameplay); 8,000 tris (cutscene)

LAYERS (stacked top to bottom)
  ├── Footwear (boots, sneakers, dress shoes, work boots)
  ├── Pants/Lower (jeans, combat trousers, skirt, uniform)
  ├── Top (t-shirt, jacket, vest, uniform shirt, suit jacket)
  ├── Outerwear (jacket, coat, tactical vest — optional)
  ├── Accessories (belt, holster, backpack, satchel — optional)
  ├── Head — Base (bald/shaved, short, medium, long hair × 8 styles)
  ├── Facial hair (none, stubble, beard, mustache × 6 styles)
  └── Hat/Helmet (cap, beanie, helmet, hard hat — optional)

FACTION IDENTITY LAYER
  - Overlay tint on specific clothing pieces using faction accent color
  - Faction logo applied as decal on shoulder/chest/back
  - Police/Warden: full uniform with rank insignia
  - Axiom: standardized tactical outfit; no face coverage except elite units
```

### Animation List

**Locomotion:**
1. Idle (default stand; 3 variants with subtle shifts)
2. Walk (4-directional + diagonal)
3. Crouch walk
4. Sprint
5. Sprint start / stop transitions
6. Stamina exhaustion (hunched walk)
7. Swim (surface)

**Combat:**
8. Aim idle (pistol, SMG, rifle, shotgun — 4 sets)
9. Fire (each weapon category, single + burst + auto)
10. Reload (per weapon category)
11. Melee quick strike
12. Melee heavy strike
13. Melee grab initiate
14. Subdue hold
15. Subdue finish (snap)
16. Throw (grenade arc animation)
17. Cover press (enter/hold/exit)
18. Roll / dodge
19. Vault over obstacle
20. Climb up
21. Hit reaction — light (torso stagger)
22. Hit reaction — heavy (knocked back)
23. Downed (fall + ground loop)
24. Self-inject stim
25. Death (directional: front, back, left, right falls)

**Interaction:**
26. Open door
27. Kick door
28. Loot body
29. Hack terminal
30. Plant device
31. Pick up item
32. Enter vehicle (driver)
33. Exit vehicle (normal)
34. Exit vehicle (emergency roll)
35. Carjack (attacker)
36. Carjack reaction (victim dragged)
37. Phone use

**NPC Specific:**
38. Idle conversation gestures (set of 6)
39. Flee run (faster, panicked arm movement)
40. Civilian phone call
41. Patrol walk
42. Guard idle post (rifle rest)
43. Sit idle (bench, chair variants)
44. Injured walk (limp)
45. Surrender (hands up)

---

## 3.4 VEHICLES (25+ Original Concepts)

| # | Name | Class | Design Note | Role |
|---|------|-------|-------------|------|
| 1 | ZURO Dart | Compact | Hatchback; angular nose; civic styling | Urban escape |
| 2 | NOVO Sprint | Compact | Micro-car; bubble top; minimal profile | Gap navigation |
| 3 | KAEL Cruiser | Sedan | 4-door; wide stance; slightly battered civilian | Default transport |
| 4 | MAREN Axis | Sedan | Government-plate styling; tinted windows | Undercover |
| 5 | VORN Hammer | Muscle | Long hood; wide fenders; aggressive front | Chase domination |
| 6 | KRESS 9 | Muscle | Fastback silhouette; fat rear tires | Speed + power |
| 7 | BADER Bulk | SUV | High-clearance; blunt; utility styling | Roadblock/durability |
| 8 | SKOV Patrol | SUV/Police | Warden Bloc patrol vehicle; aggressive push bar | Faction/police |
| 9 | HELIX 7 | Sports | Low; wide; rear-engine implied; wedge nose | Top-speed |
| 10 | RAPTOR V | Sports | Mid-engine feel; side intake scoops | Handling king |
| 11 | RELO Carry | Van/Cargo | Panel van; sliding door; civilian or cargo | Mission cargo |
| 12 | DUNN Box | Van/Cargo | High-roof cargo; Cartel livery variant | Smuggling |
| 13 | TERN Blade | Motorcycle | Naked sport; exposed frame; aggressive lean | Urban gap/speed |
| 14 | AXLE Ghost | Motorcycle | Cruiser; low and long; Syndicate style | Style/durability |
| 15 | SECTOR Response | Emergency | Police interceptor based on sedan platform | Police use |
| 16 | HAZE Ambulance | Emergency | White box body; can be stolen for heat bypass | Utility |
| 17 | IRON Crawler | Military/Heavy | 6-wheel armored; Warden/Axiom issue | Late game threat |
| 18 | DECK Armored | Military/Heavy | Armored truck cab; mine-resistant profile | Escort/resistance |
| 19 | SKEL Jet | Watercraft | Aggressive bow; twin outboard; Cartel racing | Harbor escape |
| 20 | MIRE Raft | Watercraft | Flat-deck utility; slow but silent | Stealth water |
| 21 | KROVE Semi | Commercial | 18-wheel cab; jackknife physics | Environmental hazard |
| 22 | FOLD Taxi | Civilian Taxi | Distinct roof marker; spawns everywhere | Disguise/blend |
| 23 | PLEX Coupe | Luxury | Grid district elite; distinctive hood ornament | The Grid |
| 24 | VANE Buggy | Off-road | Industrial; Pits spawn; exposed roll cage | The Pits rough terrain |
| 25 | STRAKE Muscle+ | Muscle (Rare) | Legendary variant of KRESS 9; Syndicate livery | Achievement unlock |
| 26 | AXIOM Stealth | Faction Special | Black matte; no plates; silent engine mode | Axiom operatives |
| 27 | CARGO HAULER | Dock Heavy | Slow dock vehicle; can tip containers | Waterfront missions |

---

## 3.5 WEAPONS (15+ Original Designs)

**Design Philosophy:**
- Every weapon has a visual identity that communicates its mechanical role
- Top-down silhouette must be readable (distinct barrel lengths, body shapes)
- No real-world weapon names or replication of iconic real designs
- Visual grit + functional clarity

| # | Name | Category | Design Notes | Role |
|---|------|----------|--------------|------|
| 1 | KP-45 | Pistol | Compact frame; polymer grip; striker-fired look | Concealable sidearm |
| 2 | VOSS-9 | SMG | Short barrel; skeletal stock folds; 32-round box mag | Close-mid offense |
| 3 | ROKO Compact | SMG | Tiny; machine-pistol; no stock | Pocket auto |
| 4 | RK-12 | Shotgun | Pump; 6-round tube; modified grip wrap | CQB |
| 5 | BREACH AUTO | Shotgun | Semi-auto; drum mag; wide receiver | Crowd clearing |
| 6 | SABER-X | Assault Rifle | Mid-length; modular rail system; ambidextrous controls | Versatile mid-range |
| 7 | LENZ MR | Marksman Rifle | Long barrel; bipod optional; cheek weld stock | Precision |
| 8 | HOLLOW 7 | Sniper Rifle | Bolt action; suppressor-capable; heavy stock | Assassination |
| 9 | CHAIN-MG | Heavy MG | Belt-fed; deployed or carried (movement penalty) | Area suppression |
| 10 | KREX Launcher | Rocket | Single-shot; break-open reload; vehicle killing | Anti-vehicle |
| 11 | SPARK GUN | Energy (Rare) | Axiom-tech; fires conductive dart; stun or lethal mode | Axiom missions |
| 12 | FIST-9 (Suppressed) | Suppressed Pistol | Integrated suppressor; subsonic; slow fire rate | Ghost tree unlock |
| 13 | GRIP-4 | Shotgun Pistol | Sawn-off; 2-shot break action; tiny form factor | Last resort |
| 14 | FLARE Pistol | Utility/Weapon | Standard flare; can ignite fuel, signal allies | Utility |
| 15 | VOSS-9 Extended | SMG (Upgrade) | VOSS-9 with extended barrel + foregrip | Mid-range coverage |
| 16 | SABER-X Marksman | Assault Rifle (Upgrade) | SABER-X with precision barrel + scope | SABER-X late upgrade |

**Thrown Weapons:**
- FRAG grenade (4m lethal radius; 7m damage radius)
- INCEND grenade (fire spread; 8-sec burn zone)
- SMOKE canister (12-sec concealment cloud; 3m radius)
- EMP grenade (disables vehicles and CCTV in 10m for 8 sec)
- FLASH grenade (blind + deafen effect; 5m radius; 2 sec)

---

## 3.6 UI/UX ASSETS

### HUD Breakdown

```
HUD LAYOUT (top-down; minimal interference with gameplay view)

TOP LEFT:
  ┌─────────────────────────────────────┐
  │ [HEALTH BAR]  ■■■■■■■■░░  87/100   │
  │ [ARMOR BAR]   ■■■■░░░░░░  40/100   │
  │ [STAMINA]     ■■■■■■░░░░  (drain)  │
  └─────────────────────────────────────┘

TOP RIGHT:
  ┌─────────────────────────────────────┐
  │  HEAT: [●●●○○]  Level 3            │
  │  WANTED ICON (flashing at active)  │
  └─────────────────────────────────────┘

BOTTOM LEFT:
  ┌─────────────────────────────────────┐
  │ [WEAPON SLOT 1]  VOSS-9   32 | 96  │
  │ [WEAPON SLOT 2]  KP-45    12 | 48  │
  │ [THROW SLOT]     FRAG ×3           │
  └─────────────────────────────────────┘

BOTTOM RIGHT:
  ┌─────────────────────────────────────┐
  │  MINIMAP (120px circular)          │
  │  Player dot center; N up           │
  │  District label overlay            │
  │  Faction territory tint            │
  └─────────────────────────────────────┘

CENTER BOTTOM:
  ┌─────────────────────────────────────┐
  │  OBJECTIVE (current primary only)  │
  │  [→ Reach the signal box | 2:45]   │
  └─────────────────────────────────────┘

CONTEXTUAL (appears only when relevant):
  - Interaction prompt (bottom-center, floating)
  - Mission update banner (top-center, slides in/out)
  - Cash earned popup (right-center)
  - Heat change indicator (top-right, pulse)
  - Faction rep change indicator (top-right, alongside heat)
```

### Menu Flows

```
MAIN MENU
  ├── NEW GAME
  │     └── Prologue cutscene → Character intro → Tutorial
  ├── CONTINUE (loads most recent save)
  ├── LOAD GAME
  │     └── Save slot selector (3 slots + auto-save)
  ├── SETTINGS
  │     ├── Graphics (resolution, quality presets, LOD)
  │     ├── Audio (master, music, SFX, voice volumes)
  │     ├── Controls (remap all actions; aim assist; vibration)
  │     └── Accessibility (colorblind mode, subtitle size, aim assist strength)
  └── QUIT

PAUSE MENU (in-game)
  ├── RESUME
  ├── MAP (full city map; fast travel to owned safehouses)
  ├── INVENTORY (weapons, items, upgrades)
  ├── STATS (rep levels, wanted history, completion)
  ├── SAVE GAME
  └── QUIT TO MAIN MENU

PHONE MENU (in-game; accessible at any time without pausing)
  ├── CONTACTS (faction contacts; call for jobs/services)
  ├── MAP (same as pause map; game continues)
  ├── SHOP (remote purchase if networker upgrade unlocked)
  └── INTEL (active mission details; optional objective tracker)
```

---

## 3.7 AUDIO PLAN

### Music System

**Architecture:** Adaptive music using a layered stem system

```
MUSIC STATE MACHINE
  States: AMBIENT | TENSION | PURSUIT | COMBAT | MISSION_ACTIVE | STEALTH | VICTORY | FAIL

  AMBIENT:
    - District-specific base track (loops indefinitely)
    - Stems: Bass layer always on; melody optional; atmosphere on
    - Changes per district: industrial beats (Pits), jazz-noir (Grid), 
      electronic/beat (Neon Flats), dockside drone (Waterfront), clinical silence (Spire)

  TENSION:
    - Player heat 1–2 or near hostile territory
    - Adds percussion stem + adds bass intensity
    - Transitions: 2-sec crossfade from ambient

  PURSUIT (Heat 3–4):
    - Full track with driving tempo; aggressive
    - Helicopter alarm sample loops under track
    - 1-sec transition (abrupt intentional)

  COMBAT:
    - Intense; BPM 130+; distorted bass; industrial rhythm
    - Loops while combat is active; transitions to tension when last enemy down

  STEALTH:
    - Minimal; only atmosphere + very quiet synth pad
    - Audio cue when detection risk rises (subtle hi-hat intensity)

  MISSION_ACTIVE:
    - Mission-specific theme if defined in mission JSON; otherwise state-based

  VICTORY / FAIL:
    - Short stings (3–5 sec); then transition back to ambient
```

**Music Catalog (target):**
- 5 district ambient tracks (3-4 min each, loopable)
- 5 tension variants (per district feel)
- 3 pursuit tracks
- 4 combat tracks
- 2 stealth tracks
- 10 mission-specific themes (main story moments)
- 5 faction-specific tracks (heard in faction territory / missions)
- Victory / fail stings

### SFX Categories

| Category | Examples | Notes |
|----------|----------|-------|
| Weapons — Gunshots | Per weapon; indoor/outdoor variants | Distance attenuation; muffled through walls |
| Weapons — Reload | Per weapon; per reload type | Mechanical detail; satisfying click |
| Weapons — Dry fire | Universal empty-click | |
| Weapons — Impact | Bullet on concrete, metal, flesh, glass, water | 3 variants each |
| Explosions | Vehicle, grenade, fuel drum; size variants | Low-frequency rumble + sharp crack |
| Vehicle — Engine | Per class; idle, drive, redline; unique per model | Pitch-shifted by speed |
| Vehicle — Tires | Screech, roll on wet, flat tire flap | |
| Vehicle — Collision | Light tap to heavy crash (5 intensity levels) | |
| Vehicle — Explosion | Sequential: fire crackle → bang → debris |  |
| Character — Footsteps | Per surface × per character weight | Concrete, gravel, water, metal, carpet |
| Character — Exertion | Sprint breath, pain grunts, death vocalizations | |
| NPC — Barks | Faction-specific combat lines, idle chatter | |
| NPC — Civilian | Panic screams, phone calls, conversation murmur | Background; spatialized |
| Environment — Ambient | Traffic hum, wind, rain, water, machinery | Looping; per district |
| Environment — Events | Sirens, gunshots from distance, crowd, alarms | Triggered by world events |
| UI — HUD | Health low warning, heat increase sting, mission ping | Subtle; non-intrusive |
| UI — Menu | Confirm, cancel, hover, transition | Clean, modern |

### Voice Design

**Kael (Player Character):**
- Sparse dialogue; speaks primarily in missions and key interactions
- Tone: Dry, controlled, occasionally sardonic
- No constant quips during combat (preserves tension)
- Key story dialogue: fully voiced
- Radio/phone calls: always voiced

**NPCs:**
- Faction leaders: fully voiced for all mission dialogue
- Street-level faction members: barks only (short, repeated, faction-appropriate)
- Civilians: ambient barks (reactions, phone calls, greetings) — varied pool of 30+ lines per type
- Director Vale: fully voiced; menacing, precise, intellectual
- All voiced dialogue: English base; localization-ready (subtitle system built in)

**Audio Localization:**
- Subtitle system with font size scaling
- All VO stored as labeled audio clips with corresponding subtitle JSON keys
- Music and SFX: no localization required
