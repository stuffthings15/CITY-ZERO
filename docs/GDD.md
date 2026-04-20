# CITY//ZERO — Game Design Document (GDD)
**Version:** 1.0 | **Status:** Production-Ready Draft | **IP:** Original — All content is wholly original.

---

## 1.1 HIGH CONCEPT

### Elevator Pitch
CITY//ZERO is a top-down open-world crime-action sandbox set in the fictional mega-city of **Vektor**, a sprawling coastal metropolis rotting from the inside. You play as **KAEL RISON** — a fixer, former military logistics specialist turned criminal operator — who returns to Vektor after a five-year disappearance to find every power structure that once protected him has collapsed, fractured, or turned predatory. To survive, you'll need to navigate five warring factions, build underground influence, and tear down the corrupt infrastructure before it consumes what's left of the city.

### Player Fantasy
- **Power through systems mastery** — outmaneuvering enemies using the city's own logic rather than pure reflexes
- **Criminal empire building** — from street-level fixer to untouchable operator
- **Urban dominance** — owning turf, corrupting institutions, and reshaping who controls what
- **Emergent storytelling** — your choices ripple through a living city that reacts to your behavior

### Tone and Setting
- **Tone:** Neo-noir with visceral arcade energy. Dark humor undercuts the violence. Moral ambiguity without pretension.
- **Setting:** Vektor City, year 203X. A coastal mega-city built on reclaimed industrial land. Heavy rain half the year. Neon advertising competes with decay. Prosperity exists only in the upper districts; everything below is contested territory.
- **Aesthetic influences (thematic, NOT visual copies):** Cold War espionage paranoia + 90s crime cinema grit + near-future surveillance state anxiety
- **Music tone:** Aggressive electronic, dark jazz, low-fidelity hip-hop, industrial ambient

---

## 1.2 CORE GAMEPLAY LOOP

### Step-by-Step Loop

```
[ENTER WORLD]
     |
     v
[RECEIVE INTEL / DISCOVER OPPORTUNITY]
  - Mission call from faction contact
  - Ambient world event discovered organically
  - Player-initiated target identification
     |
     v
[PLAN / PREPARE]
  - Purchase weapons, items, vehicle upgrades
  - Scout location (on foot or via drone item)
  - Bribe or recruit NPCs
  - Identify escape routes
     |
     v
[EXECUTE]
  - Driving, shooting, stealth, manipulation
  - Systemic chaos management
  - Improvisation when plans fail
     |
     v
[ESCAPE / RESOLVE]
  - Evade heat response
  - Return to safe zone or safehouse
  - Assess collateral damage
     |
     v
[REWARD / CONSEQUENCE]
  - Cash payout
  - Faction reputation change (±)
  - City state mutation (territory shifts, NPC reactions)
  - Unlock gates open
     |
     v
[RE-INVEST]
  - Upgrade character, weapons, vehicles
  - Expand safe zone network
  - Level up faction relationships
     |
     v
[REPEAT with increased system complexity]
```

### Short-Term Goals (Session Level)
- Complete a mission or job
- Earn enough cash for a specific upgrade
- Clear heat before time expires
- Survive an ambush or respond to a world event
- Scout a new district

### Long-Term Goals (Campaign Arc)
- Rise through faction ranks to reach Tier 5 Operator status
- Unlock and control all 5 districts
- Complete faction story arcs (each 8–12 missions long)
- Reach wanted level 0 with maximum city influence
- Expose and destroy the Axiom Directorate (main antagonist organization)

---

## 1.3 PLAYER SYSTEMS

### Movement (On Foot)

| Action | Mechanic Detail |
|--------|----------------|
| Walk | Standard 4-directional + analog with variable speed |
| Sprint | Hold sprint; drains stamina at 12 units/sec; auto-stops at 0 |
| Crouch | Toggle; reduces visibility to enemies by 60%; movement speed halved |
| Cover Snap | Press cover button near wall/object; character presses flush against surface |
| Vault | Auto-triggered when sprinting into low obstacle (fence, car hood, counter) |
| Roll/Dodge | Tap direction during combat; brief i-frames (6 frames); costs 20 stamina |
| Climb | Triggered at climbable surfaces (marked or learned); costs stamina per second |
| Swim | Reduced speed; no weapon use while swimming; stamina drains 2x |

**Stamina System:**
- Max stamina: 100 units (upgradeable to 150)
- Sprint: -12/sec
- Roll: -20 flat
- Climb: -8/sec
- Regen: +15/sec when not exerting; delayed by 1.5 sec after exertion stops
- At 0 stamina: forced walk, no dodge, no vault

### Combat

**Core Philosophy:** Fast read, fast reaction. Weapons feel distinct and purposeful. Every encounter should be solvable multiple ways. No weapon is strictly "best" — context determines effectiveness.

**Aiming System:**
- Top-down perspective; player aims with right stick / mouse
- Soft aim assist: gentle magnetism toward visible enemy center-mass (not head)
- Aim assist strength configurable in accessibility settings (0–100%)
- Aim cone widens under suppression fire or when sprinting

**Shooting Mechanics:**
- Recoil: visual kick + bullet spread increase; recovers over 0.3 sec
- Bullet travel: near-instant at short range; slight travel at 20m+
- Hit feedback: enemy stagger animation on torso hit; ragdoll on kill
- Friendly fire: active; civilians flee or panic; faction NPCs react

**Melee:**
- Quick strike: short-range, fast, low damage
- Heavy strike: windup, knocks enemy down, high damage
- Grab/choke: hold button near enemy; transition to throw, snap, or subdue
- Subdue: quiet takedown; enemy unconscious 90 sec; available from behind only

**Balance Philosophy:**
- Pistols: fast, concealable, low ammo cost, reliable
- SMGs: high fire rate, poor accuracy beyond 12m, best in vehicles
- Shotguns: devastating close, useless at range, crowd control king
- Rifles: precision at range, loud, heavy police attention modifier
- Heavy weapons: rare, destructive, instant heat escalation, cannot be concealed
- Thrown: grenades, molotovs, EMP grenades, smoke canisters

### Health and Armor

| Layer | Description | Regen Logic |
|-------|-------------|-------------|
| Armor | Absorbs first 40% of incoming damage | No natural regen; buy/pickup to restore |
| Health | Base 100 HP (upgradeable to 140) | Slow regen after 6 sec out of combat; 2HP/sec |
| Trauma State | Below 25 HP: movement slowed, aim shakes | Removed when health exceeds 25 |
| Downed State | 0 HP: 8-second bleed-out window; can self-inject to recover at 10 HP |
| Death | If downed with no stim or not rescued in 8 sec: respawn at last safehouse |

**Consumables:**
- Med-Kit: +40 HP instant; weight 1 slot
- Combat Stim: +25 HP + clears trauma; instant; weight 1 slot
- Armor Plate: +25 armor; weight 1 slot
- Adrenaline Shot: 30-sec speed/damage buff; weight 1 slot

### Interaction System

| Interaction | Button / Trigger | Description |
|-------------|-----------------|-------------|
| Enter Vehicle | Approach + button | Smooth transition animation; 0.8 sec |
| Carjack | Approach occupied vehicle + hold | Forces driver out; 1.5 sec; raises heat +1 if witnessed |
| Pickup Item | Walk over / press button | Context-sensitive; items glow when in range |
| Talk to NPC | Approach + interact | Dialogue wheel with 3–4 options |
| Bribe NPC | Dialogue → bribe option | Costs cash; changes NPC behavior temporarily |
| Hack Terminal | Approach + hold | 2–5 sec depending on security tier; mini-game optional |
| Plant Device | Crouch + interact near target | Explosive, tracker, or surveillance bug |
| Loot Body | Approach downed enemy + hold | Yields cash, ammo, occasionally keycard or intel |
| Open/Force Door | Interact; kick if locked | Kick: loud, alerts nearby NPCs |
| Use Phone | Hold phone button | Access map, contacts, shop, stats |

---

## 1.4 VEHICLE SYSTEM

### Vehicle Classes

| Class | Examples | Top Speed | Handling | Durability | Role |
|-------|----------|-----------|----------|------------|------|
| Compact | ZURO Dart, NOVO Sprint | 140 km/h | Excellent | Low | Urban chases, escape |
| Sedan | KAEL Cruiser, MAREN Axis | 175 km/h | Good | Medium | Standard transport |
| Muscle | VORN Hammer, KRESS 9 | 210 km/h | Fair | Medium-High | Chase dominance |
| SUV/Truck | BADER Bulk, SKOV Patrol | 160 km/h | Poor | High | Roadblocks, cargo |
| Sports | HELIX 7, RAPTOR V | 240 km/h | Excellent | Low | Long straights |
| Van/Cargo | RELO Carry, DUNN Box | 130 km/h | Poor | High | Cargo missions, mobile cover |
| Motorcycle | TERN Blade, AXLE Ghost | 220 km/h | Excellent | Very Low | Gap navigation |
| Emergency | SECTOR Response, HAZE Ambulance | 180 km/h | Good | High | Stolen for police bypass |
| Military/Heavy | IRON Crawler, DECK Armored | 110 km/h | Very Poor | Very High | Late game, faction assets |
| Watercraft | SKEL Jet, MIRE Raft | 90 km/h (water) | Fair | Medium | Harbor district missions |

### Handling Model

**Physics Approach:** Arcade-simulation hybrid. Understeer and oversteer exist but are forgiving. Momentum matters. Handbrake turns are satisfying. Not simulation-grade.

- **Traction:** Surface-dependent. Wet roads reduce grip 25%. Dirt/gravel -15%. Ice -40%.
- **Weight:** Affects braking distance and collision response. Heavy vehicles push smaller ones.
- **Drift:** Rear-wheel-drive vehicles drift on handbrake. Front-wheel vehicles understeer.
- **Air Time:** Vehicles catch air off ramps; physics-based landing with suspension compression.
- **Speed Zones:** All vehicles have: Idle → City speed → Chase speed → Redline (risky, engine damage chance)

### Damage System

| Zone | Visual Effect | Gameplay Effect |
|------|--------------|-----------------|
| Front bumper | Crumple, paint damage | Reduced ram effectiveness |
| Hood | Buckle, smoke | Engine damage; top speed -20% |
| Engine | Fire possible | Top speed -50%; risk of explosion |
| Tires | Flat animation | Handling penalty; can blow at high speed |
| Windows | Crack → shatter | Driver visibility unaffected (top-down); cosmetic |
| Doors | Dent, hang | Cosmetic + occupant exposed |
| Fuel tank | Rear impact | Leak → fire trail; explosion if ignited |
| Frame (total) | Smoke + sparks | Vehicle at <10% HP; imminent destruction |

**Destruction:** Vehicle explodes when HP reaches 0 or fire reaches engine. Explosion radius: 6m. Deals 60 damage to nearby entities.

### Enter/Exit Logic

- **On foot to vehicle (empty):** Walk within 1.5m + interact → 0.8 sec animation → in vehicle
- **On foot to vehicle (occupied / carjack):** Approach occupied side + hold → Kael opens door, drags occupant out → 1.5 sec → in vehicle → heat escalation if witnessed
- **Exit moving vehicle:** Allowed above 0 km/h; auto-roll and tumble animation; damage based on speed
- **Exit at stop:** Standard open-door animation; 0.6 sec
- **Emergency exit:** Button mash while vehicle on fire; priority vault animation
- **Motorcycles:** Faster mount/dismount; can be knocked off at impact >80 km/h

### Theft Mechanics

| Scenario | Heat Modifier | Time |
|----------|--------------|------|
| Empty parked vehicle | +0 if unwitnessed; +1 if witnessed | 0.8 sec |
| Occupied vehicle (civilian) | +1 always | 1.5 sec |
| Police vehicle | +2 | 2.0 sec |
| Faction vehicle | Triggers faction alert + heat +1 | 2.0 sec |
| Armored/military vehicle | +3; immediate armed response | 3.0 sec |

---

## 1.5 WANTED / HEAT SYSTEM

### Heat Levels

| Level | Name | Active Response | Pursuit Type | Search Behavior |
|-------|------|----------------|--------------|-----------------|
| 0 | Clean | None | None | None |
| 1 | Noticed | Single patrol car dispatched | Passive patrol near incident | 60-sec radius scan |
| 2 | Flagged | 2 patrol cars + foot units | Active pursuit if spotted | Block radius cordon |
| 3 | Pursuit | 4 cars + helicopter | Aggressive pursuit; PIT maneuvers | City quarter search |
| 4 | Manhunt | Cars + helicopter + SWAT foot | Roadblocks placed; spike strips | Half-city alert |
| 5 | LOCKDOWN | Military-tier assets; armored units | All districts alerted; asset saturation | Full city; no safe zones except registered safehouses |

### Escalation Logic

Heat increases when:
- Crime committed in view of police NPC (+1)
- Crime committed in view of civilian who calls in (+0.5 → rounds up)
- Shooting in public (+1 per 3 seconds)
- Police unit destroyed (+1)
- High-value target attacked (+1–2 depending on target)
- CCTV footage captured (check system; delay of 30–60 sec before heat increase)

Heat decreases when:
- Player breaks line of sight and stays hidden for cooldown duration
- Player reaches a registered safehouse
- Player pays off corrupt officer contact (costs scale with heat level)
- Player uses signal jammer item (blocks CCTV; reduces escalation speed)

### Cooldown Durations by Level

| Level | Time to Begin Cooldown (after LOS break) | Full Cooldown Duration |
|-------|------------------------------------------|------------------------|
| 1 | 15 sec | 30 sec |
| 2 | 30 sec | 60 sec |
| 3 | 45 sec | 120 sec |
| 4 | 60 sec | 240 sec |
| 5 | 90 sec | 600 sec (or safehouse required) |

### Police Behaviors Per Level

- **Level 1:** Single patrol car drives to incident area; NPC officers exit and investigate on foot; will engage if player spotted committing crime
- **Level 2:** Dispatch audio heard citywide in zone; patrol cars circle area; foot units establish a soft perimeter; will engage on sight
- **Level 3:** Helicopter sweeps with spotlight; patrol cars perform pursuit logic (PIT maneuver at speed, cut-off maneuvers); foot SWAT deploying at key intersections
- **Level 4:** Roadblocks at district entrances; spike strips on main roads; SWAT teams breach suspected locations; helicopter has two searchlights; armored transport deployed
- **Level 5:** Military-class armored vehicles enter world; all faction territories suspend normal activity; any civilian who sees player immediately calls in; safehouse locations broadcast if compromised; airstrike marker ability available to Axiom Directorate

### Evasion Mechanics

| Method | Effectiveness | Cost / Risk |
|--------|--------------|-------------|
| Line of sight break | Level 1–3 effective | Requires environment knowledge |
| Safehouse entry | Resets heat completely | Exposes safehouse location |
| Change appearance (clothing shop) | Removes visual recognition; lowers heat 1 | Requires accessible shop; time |
| Bribe corrupt officer contact | Instant -1 heat | Costs cash per level; contact must be unlocked |
| Signal jammer item | Slows heat escalation; disables CCTV reporting 60 sec | Consumable |
| Swap vehicle | Removes vehicle description flag | Must steal clean vehicle |
| Sewer/tunnel system | Breaks helicopter tracking | Must know entry point |
| Waterway escape | Helicopter loses track over water | Requires watercraft or swimming |

---

## 1.6 AI SYSTEMS

### Civilian Behavior Model

Civilians operate on a **Layered Awareness FSM (Finite State Machine)**:

```
STATES:
  IDLE         → Routine loop (walk path, stand, sit, shop, converse)
  AWARE        → Noticed stimulus; slow down, look at source
  ALARMED      → Witnessed crime; flee OR freeze (personality-based)
  REPORTING    → Stationary, phone animation, calling police (heat trigger)
  FLEEING      → Running away from threat source
  SHELTERING   → Enters nearest building; stays until threat timer expires
  PANICKING    → Full sprint, erratic; triggered by explosions or mass shooting

TRANSITIONS:
  IDLE → AWARE:     Stimulus within 12m (gunshot, shout, vehicle crash)
  AWARE → ALARMED:  Witnessed crime directly within FOV
  ALARMED → FLEEING/REPORTING: Personality roll (coward 70% flee, civic 30% report)
  ANY → PANICKING:  Explosion within 15m OR mass casualty event
  ANY → IDLE:       Threat clear for 120 sec AND NPC not REPORTING
```

**Civilian Archetypes (personality affects state transitions):**
- Coward: flee immediately, never report
- Civic: report first, then flee
- Hostile: shout, throw objects, may attack at heat 0
- Neutral: standard behavior
- Connected (faction civilian): reports to faction, not police

### Police AI Architecture

Police use a **Hierarchical Task Network (HTN)** style architecture with global dispatch integration:

```
DISPATCH LAYER (Global)
  - Receives crime reports from civilian AI or CCTV system
  - Assigns units from precinct pool
  - Escalates heat tier based on crime severity + unit losses
  - Issues pursuit orders; updates suspect description (vehicle, clothing)

UNIT LAYER (Individual)
  States: PATROL → RESPONDING → INVESTIGATING → PURSUING → ENGAGING → CALLING_BACKUP → RETREATING

  PATROL:       Follow district patrol route; scan for flagged vehicle/suspect
  RESPONDING:   Navigate to reported crime scene at max speed
  INVESTIGATING: On foot search in radius; check points of interest
  PURSUING:     Vehicle pursuit logic; attempt PIT, cut-off, spike strip placement
  ENGAGING:     Take cover; advance under fire; suppressive fire; flank if 2+ units
  CALLING_BACKUP: Broadcast on radio when health < 40% or outnumbered
  RETREATING:   Back off if health < 20%; still maintains LOS and reports position

TACTICAL BEHAVIORS:
  - PIT Maneuver: Intercept vehicle at angle; initiate speed match; sharp turn
  - Roadblock: Two vehicles angled across road; officers take cover behind them
  - Surround: 4+ officers spread to cardinal points; advance simultaneously
  - Helicopter: Spotlight; marks player position on dispatch map; cannot engage directly
  - SWAT: Breach doors; use flashbangs; two-by-two room clearing
```

### Faction AI Behavior Trees

Each faction NPC uses a **Behavior Tree** with faction-specific modifiers on decision weights:

```
ROOT
├── SELECTOR: High Priority
│   ├── SEQUENCE: Under Attack
│   │   ├── Condition: IsUnderAttack
│   │   ├── Task: TakeCover
│   │   ├── Task: ReturnFire
│   │   └── Task: CallReinforcements (if health < 50%)
│   ├── SEQUENCE: Player Hostile (reputation < -50)
│   │   ├── Condition: PlayerInRange(15m)
│   │   ├── Task: Threaten (verbal + weapon draw)
│   │   └── Task: Attack if player doesn't retreat in 3 sec
│   └── SEQUENCE: Patrol Disrupted
│       ├── Condition: PatrolPathBlocked OR AllyDowned
│       └── Task: InvestigateDisturbance
│
└── SELECTOR: Routine
    ├── Task: PatrolRoute (faction territory waypoints)
    ├── Task: Guard (static post; random idle animations)
    ├── Task: Converse (approach fellow faction NPC; dialogue bark loop)
    └── Task: React to Environment (sirens → heighten alertness)
```

**Faction-specific modifiers:**

| Faction | Aggression | Cohesion | Retreat Threshold | Ranged vs Melee Preference |
|---------|-----------|----------|-------------------|---------------------------|
| Ruin Syndicate | High | Medium | 15% HP | 70% Ranged |
| Warden Bloc | Very High | Very High | 5% HP (disciplined) | 90% Ranged |
| Hollow Kings | Medium | Low | 30% HP | 60% Ranged / 40% Melee |
| Meridian Cartel | Medium | High | 25% HP | 80% Ranged |
| Axiom Directorate | Low (calculated) | Very High | 10% HP | 100% Tactical ranged |

---

## 1.7 FACTION SYSTEM

### Faction 1: RUIN SYNDICATE

**Symbol:** Fractured crown  
**Colors:** Black + rust orange  
**Territory:** The Pits (industrial south district)  
**Leadership:** DAME OSKAR — scarred ex-refinery foreman turned crime lord  

**Motivation:** The Ruin Syndicate believes Vektor's elite deliberately engineered the industrial collapse that destroyed working-class livelihoods. They steal, extort, and smuggle with unapologetic fury — not for luxury, but to fund a decentralized war against the upper city.  

**Economy:** Chop shops, stolen freight, protection rackets, underground boxing  

**Gameplay Impact:**
- Control chop shops (allows vehicle upgrades at 40% discount)
- Provide access to black market heavy weapons
- High-value escort and heist missions
- Hostile to police; neutral to player by default

**Reputation Effects:**
- +Rep: Complete jobs for Dame Oskar; steal from corporations; destroy police assets
- -Rep: Work for Warden Bloc; destroy their chop shops; kill Syndicate members
- Max Rep: Unlock Syndicate safehouses in The Pits; access Syndicate vehicle pool; heavy weapon cache drops

---

### Faction 2: WARDEN BLOC

**Symbol:** Vertical bar with slash  
**Colors:** Deep blue + silver  
**Territory:** The Grid (financial/administrative district)  
**Leadership:** COMMANDER ELIA STRAK — former military, now private security empire owner  

**Motivation:** The Warden Bloc believes Vektor is ungovernable without total enforcement. They operate a for-profit security company that has effectively replaced municipal police in the upper city. They are authoritarian, disciplined, and contemptuous of the lower districts.  

**Economy:** Security contracts, prisoner labor (gray market), information brokering, weapons export  

**Gameplay Impact:**
- Control The Grid means restricted movement for player if hostile
- Can hire Warden units as temporary escorts (if allied)
- Intercept missions (protecting or raiding Warden convoys)
- Actively hunt player at heat 3+ in their territory; function like super-police

**Reputation Effects:**
- +Rep: Protect their assets; complete extraction contracts; eliminate rivals
- -Rep: Steal from The Grid; destroy Warden patrols; ally with Ruin Syndicate
- Max Rep: Unlock player bounty suppression in Grid; Warden patrol ignores heat 1–2

---

### Faction 3: HOLLOW KINGS

**Symbol:** Crown with empty eyes  
**Colors:** White + electric blue  
**Territory:** Neon Flats (entertainment/nightlife district)  
**Leadership:** No single leader — ruled by a rotating council of 7 (The Seven Seats)  

**Motivation:** The Hollow Kings emerged from underground rave culture, hacking collectives, and anarchist street art. They reject all hierarchy but have built a powerful distributed network of informants, hackers, and street operators. Their goal: expose and humiliate power — Warden Bloc especially.  

**Economy:** Data theft, blackmail, counterfeit goods, drug distribution, music venues as fronts  

**Gameplay Impact:**
- Hack services: purchase intel on enemy positions, security systems, patrol routes
- Provide drone item upgrades
- Their territory is the best place to fence stolen goods (best prices)
- Random world events in their district (raves that attract police attention)

**Reputation Effects:**
- +Rep: Leak Warden intel; steal corporate data; graffiti assignments; humiliate rivals publicly
- -Rep: Work for Axiom; destroy their network nodes; kill their members
- Max Rep: Access to Hollow Kings hacker support (real-time enemy map update during missions); free fence service

---

### Faction 4: MERIDIAN CARTEL

**Symbol:** Compass rose  
**Colors:** Gold + forest green  
**Territory:** The Waterfront (harbor/import-export district)  
**Leadership:** TRES MORANDE — third-generation smuggler, immaculately dressed, deeply dangerous  

**Motivation:** The Meridian Cartel controls Vektor's import-export lifeline. They are pragmatic capitalists — they don't care about ideology, only margins. They will work with anyone as long as the deal is profitable. They are the most economically powerful faction but the least ideologically committed.  

**Economy:** Drug smuggling, weapons import, human trafficking (player can disrupt this), luxury goods black market, port authority bribes  

**Gameplay Impact:**
- Best source of vehicles (including military-grade via import)
- Run the most lucrative (and dangerous) missions
- Can purchase police heat reduction services from them
- Control of The Waterfront affects city economy (impacts shop prices globally)

**Reputation Effects:**
- +Rep: Complete smuggling runs; protect shipments; eliminate rival operations
- -Rep: Raid shipments; ally with Hollow Kings (who leak their manifests); destroy dock operations
- Max Rep: Access to Meridian import catalog (exotic vehicles + rare weapons); port tunnel safe routes

---

### Faction 5: AXIOM DIRECTORATE

**Symbol:** Eye inside a hexagon  
**Colors:** Charcoal + crimson  
**Territory:** The Spire (government/surveillance district — largely inaccessible early game)  
**Leadership:** DIRECTOR NORA VALE — architect of Vektor's surveillance infrastructure; main antagonist  

**Motivation:** The Axiom Directorate is a shadow government agency that has embedded itself in every institution in Vektor. They believe the city is a controlled experiment in behavioral suppression — monitor everyone, destabilize organized resistance, and profit from managed chaos. They are behind the original event that forced Kael out of Vektor.  

**Economy:** Black budget government contracts, data harvesting, political manipulation, arms to multiple factions simultaneously  

**Gameplay Impact:**
- The meta-antagonist — cannot be allied with; reputation is only a threat metric
- As player rises in influence, Axiom sends asset teams (elite operatives, not police)
- Later game introduces Axiom surveillance events (blackouts, false flag incidents)
- Final act requires dismantling Axiom infrastructure across all districts

**Reputation Effects (Threat Meter, not alliance):**
- Threat 0–20: Axiom ignores player
- Threat 21–50: Axiom begins surveillance; occasional operative tails
- Threat 51–75: Axiom actively disrupts player operations; counter-missions appear
- Threat 76–100: Full Axiom mobilization; Director Vale personally hunts Kael

---

### Reputation System

**Scale:** -100 to +100 per faction  
**Tiers:**

| Score | Tier | Effects |
|-------|------|---------|
| -100 to -61 | ENEMY | Shoot on sight; bounty placed on player |
| -60 to -21 | HOSTILE | Aggression if approached; no services |
| -20 to +20 | NEUTRAL | Ignores player; basic services available |
| +21 to +60 | ALLIED | Reduced prices; missions unlocked; safe passage |
| +61 to +100 | TRUSTED | Full mission chain; unique rewards; territory protection |

**Cross-faction dynamics:**
- Ruin Syndicate ↔ Warden Bloc: Enemies; rep with one penalizes other (-15 bleed)
- Hollow Kings ↔ Axiom: Enemies; Hollow Kings rep gain automatically increases Axiom Threat
- Meridian Cartel: Independent; rep changes don't bleed to others unless directly targeted
- All factions vs Axiom: Axiom cannot be allied; Threat only rises from player actions

---

## 1.8 MISSION SYSTEM

### Mission Structure

Every mission is defined by:
1. **Type** (archetype — see below)
2. **Giver** (faction contact or world discovery)
3. **Objective Tree** (primary + 2–3 optional objectives)
4. **Heat Modifier** (base heat change from mission activities)
5. **Success Condition** (clear, variable, or tiered)
6. **Failure Conditions** (specific, not just "die")
7. **Reward** (cash + rep + unlock flag)
8. **Systemic Variables** (time of day, weather, faction state affect mission)

### Mission Archetypes (15)

| # | Archetype | Description |
|---|-----------|-------------|
| 1 | Heist | Multi-phase target acquisition; plan → execute → escape |
| 2 | Assassination | Reach and eliminate target; method determines reward bonus |
| 3 | Escort | Protect VIP or asset from A to B against opposition |
| 4 | Extraction | Rescue a captive from fortified location |
| 5 | Courier | Deliver package against time and opposition |
| 6 | Sabotage | Destroy or disable infrastructure targets |
| 7 | Surveillance | Tail target without detection; gather evidence |
| 8 | Ambush | Set up and execute ambush on convoy or group |
| 9 | Territory War | Assault and capture a contested location |
| 10 | Undercover | Infiltrate under false identity; complete embedded objective |
| 11 | Racing | Street race with combat-optional overlay |
| 12 | Debt Collection | Find targets, apply pressure, retrieve payment |
| 13 | Riot Control (Inverse) | Player instigates or manipulates a civil event |
| 14 | Data Breach | Physical + digital intrusion; secure and extract data |
| 15 | Elimination Run | Clear a series of targets across the city on a timer |

---

### Example Missions (10 Full Write-Ups)

---

#### MISSION 1: "COLD IRON" (Ruin Syndicate — Heist)
**Giver:** Dame Oskar  
**Location:** The Pits — Vektro Rail Freight Depot  
**Setup:** A Meridian Cartel shipment of military-grade weapons passes through The Pits on an armored freight train. Dame Oskar wants it hit — not for the weapons, but to humiliate the Cartel and arm the Syndicate's southern operation.

**Phase 1 — Intercept**
- Player must reach the rail depot before the train departs (8-min timer)
- Option A: Drive to signal box and manually trigger delay (stealth/hack approach)
- Option B: Block the tracks with a stolen heavy truck (loud, heat +1)

**Phase 2 — Board and Breach**
- Train is moving slowly (20 km/h) at departure
- Player can jump aboard from motorcycle or slow vehicle alongside
- 3 Cartel guards per car; 2 cars
- Target car: cargo car 4 (marked); locked with biometric crate

**Phase 3 — Crack the Crate**
- Eliminate or subdue guard with keycard
- Use keycard on biometric lock; 5-sec interaction
- Optional: Plant tracker on train first (bonus objective; reveals Cartel route intel)

**Phase 4 — Extract**
- Train will enter Cartel-controlled waterfront zone in 4 minutes
- Player must jump off with cargo or signal Syndicate van alongside tracks
- Syndicate van drives parallel — throw crate, or hand off while running

**Success:** Delivered weapons to Syndicate cache
**Optional Bonus:** +500 cash for zero civilian witnesses  
**Failure Conditions:** Train enters Cartel zone with cargo; player death  
**Rewards:** 3,200 cash + Ruin Syndicate +15 rep + Meridian Cartel -10 rep + chop shop discount unlock

---

#### MISSION 2: "GLASS EYES" (Hollow Kings — Surveillance)
**Giver:** Seat 3 (Hollow Kings council contact "PRISM")  
**Location:** Neon Flats → The Grid  
**Setup:** The Hollow Kings believe Warden Bloc is running a black-site data center in The Grid disguised as a server maintenance company. They need evidence — but can't risk their people in Warden territory. Kael goes in.

**Phase 1 — Preparation**
- Visit disguise shop; purchase business-casual clothing (removes visual flag in Grid)
- Acquire fake maintenance ID from Hollow Kings forger (delivered via drop box)

**Phase 2 — Infiltrate**
- Enter SOVIK Data Systems building (3-story office)
- Cannot use weapons unless forced; going loud fails mission
- Warden security NPCs on each floor; 2 patrol routes; 1 static post per floor
- Use stolen keycard from floor 1 (loot reception desk) to access server room on floor 3

**Phase 3 — Download**
- Connect Hollow Kings data device to server terminal
- 90-second download; player must manage 3 guard patrol routes to avoid detection
- Optional: Plant secondary bug (deeper intel; unlocks follow-up mission)

**Phase 4 — Exit**
- Leave building; once outside, change clothes at nearby restroom
- If alerted during download: fight out; download continues if device connected; extract with heat 3

**Success:** Data delivered to Prism  
**Failure Conditions:** Detected before download completes without data device  
**Rewards:** 2,800 cash + Hollow Kings +18 rep + Warden intel (permanent enemy route reveal in Grid) + Axiom Threat +5

---

#### MISSION 3: "THE DRAIN" (Meridian Cartel — Courier)
**Giver:** Tres Morande  
**Location:** The Waterfront → Neon Flats  
**Setup:** A Cartel chemist needs to be moved to a new lab in Neon Flats. Rival faction (Hollow Kings cells who know the chemist's value) is watching the docks. Kael must move him clean.

**Phase 1 — Pickup**
- Retrieve chemist from dock warehouse; he's nervous and on foot
- 2 Hollow Kings spotters nearby; must be avoided or neutralized quietly
- Chemist follows player; if Hollow Kings see him, pursuit begins

**Phase 2 — Transit**
- Drive chemist to Neon Flats lab; 3-km route
- Hollow Kings patrol cars watching main routes (randomized each attempt)
- Chemist panics if combat starts (health depletes at 2x normal rate from stress)
- Optional: Use secondary route through sewer entrance (bonus: Hollow Kings never engage)

**Phase 3 — Delivery**
- Arrive at lab; chemist exits; receive payment
- Optional: Chemist offers side tip — he knows a Warden arms cache location

**Success:** Chemist delivered alive  
**Failure Conditions:** Chemist killed; player captured by Hollow Kings  
**Rewards:** 2,400 cash + Meridian Cartel +12 rep + arms cache intel (optional; activates side content)

---

#### MISSION 4: "PRESSURE DROP" (Ruin Syndicate — Debt Collection)
**Giver:** Syndicate lieutenant BURNS  
**Location:** The Pits — 3 targets spread across district  
**Setup:** Three local operators owe Syndicate protection money. They've gone quiet. Burns wants the money or a message. Player decides the method.

**Structure:** 3 sequential mini-encounters

**Target 1 — Yev the Mechanic**
- Hiding in his garage; 2 personal guards
- Payment option: Knock guards unconscious; talk to Yev; receive 600 cash
- Message option: Destroy his garage equipment; he pays in fear

**Target 2 — MAKO Club (front business)**
- Hostile; owner is inside, refusing contact; 4 armed bouncers outside
- Payment option: Threaten owner in private after clearing bouncers
- Message option: Torch the supply storage out back

**Target 3 — Rooftop Gambler (moving target)**
- Last target is on a moving rooftop gathering; location changes every 90 sec
- Must track via informant tip or just follow the money trail
- Single bodyguard; quick confrontation

**Completion:** Return collected cash to Burns  
**Success Tier:** Full payment (all 3) → Maximum reward; 2 of 3 → Standard; 1 of 3 → Partial  
**Rewards:** Up to 2,000 cash + Ruin Syndicate +10 rep

---

#### MISSION 5: "SIGNAL BREAK" (Hollow Kings — Sabotage)
**Giver:** Seat 7 "ZERO" (Hollow Kings tech arm)  
**Location:** 3 Warden communications towers across the city  
**Setup:** Warden Bloc is deploying a city-wide civilian monitoring network. Hollow Kings want the relay towers destroyed before the system goes live. 72-in-game-hour window (real time: 12 mins).

**Structure:** 3 simultaneous optional targets → player chooses order

**Tower 1 (The Grid — high security)**
- Warden guards; restricted zone; must use EMP device on tower base; 10-sec plant + 30-sec detonate
- Alternative: Climb tower; manually cut relay cable (longer; no heat increase)

**Tower 2 (Neon Flats — moderate security)**
- Hollow Kings have partially disabled guard route; easier approach
- Blow tower with explosive charge; crowd nearby will scatter (heat +1)

**Tower 3 (Elevated highway — exposed)**
- No guards but exposed; helicopter patrol passes every 45 sec
- Must time approach with patrol gap; quick plant + depart

**Success:** 2 or more towers destroyed within window  
**Full Success:** All 3 towers + optional: upload Hollow Kings broadcast (city-wide NPC dialogue changes for 24 in-game hours)  
**Rewards:** 3,000 cash + Hollow Kings +20 rep + Warden Bloc -15 rep + Axiom Threat +8

---

#### MISSION 6: "SHATTER HOUSE" (Warden Bloc — Elimination Run)
**Giver:** Commander Strak (accessible only at rep +40 or higher)  
**Location:** Multiple Ruin Syndicate sites across The Pits  
**Setup:** Warden Bloc wants to fracture Syndicate leadership to push into The Pits economically. Kael is given a kill list of 4 lieutenants.

**Structure:** 4 targets; 15-minute window; Syndicate raises alert after 2nd kill

**Target 1 — Refinery guard post leader (armed; 3 guards)**
**Target 2 — Chop shop foreman (inside building; civilian staff present)**
**Target 3 — Syndicate recruiter at outdoor market (among civilians; must isolate)**
**Target 4 — Senior lieutenant BURNS at a moving vehicle convoy (mobile target)**

**Systemic Complication:** Completing this mission drops Ruin Syndicate rep by -30 regardless  
**Moral Tension:** Civilian bystanders; Syndicate members are working-class; player is aware  
**Optional:** Fake the death of Targets 3 & 4 (spare them; collect false confirmation for Warden; they owe Kael a favor later)

**Rewards:** 5,000 cash + Warden Bloc +25 rep + Ruin Syndicate -30 rep

---

#### MISSION 7: "GHOST FREIGHT" (Meridian Cartel — Undercover)
**Giver:** Tres Morande  
**Location:** The Waterfront  
**Setup:** A Meridian shipment was intercepted by Axiom agents masquerading as customs officers. The cargo is in a holding bay. Kael must pose as a legitimate customs inspector to get inside and recover the cargo.

**Phase 1:** Receive forged credentials from Cartel contact  
**Phase 2:** Enter customs facility during day shift; blending in requires no weapons drawn  
**Phase 3:** Find the cargo among 30 identical containers (Hollow Kings hack helps narrow it to 3)  
**Phase 4:** Identify Axiom agents inside (undercover; no uniform; behavioral tells — earpiece, weapon bulge)  
**Phase 5:** Extract cargo via loading dock; Kael signals Cartel truck  
**Complication:** One Axiom agent recognizes Kael on exit; player must decide — fight (loud) or bluff (dialogue check based on Charisma upgrade level)

**Rewards:** 4,200 cash + Meridian Cartel +20 rep + Axiom Threat +12 + intel piece for main story arc

---

#### MISSION 8: "REDLINE" (Open World Discovery — Racing)
**Discovered:** Found by winning 3 street races in Neon Flats  
**Location:** Full circuit route through 4 districts  
**Setup:** A legendary underground race — REDLINE — happens once per in-game week. No weapons officially. 8 AI competitors with unique vehicles and driving behaviors. Prize: 10,000 cash + unique vehicle unlock.

**Mechanics:**
- 3-lap circuit; 12 km total; through tunnels, elevated highway, and dockside
- Each lap introduces 1 new hazard (oil spill, police barricade, storm flood on road)
- Competitors race dirty — sideswipe, brake check, draft disruption
- Player may use weapons (heat penalty) or keep it clean

**Rewards:** 10,000 cash (winner) + HELIX 7 prototype vehicle unlock + street racing reputation tier

---

#### MISSION 9: "THE LONG DARK" (Main Story — Extraction)
**Giver:** Kael's contact SABLE  
**Location:** Axiom black site — The Spire subdistrict  
**Setup:** Kael's former military handler has been taken by Axiom. He holds critical evidence against Director Vale. This is the first mission inside Axiom territory — a test run before the final act.

**Phase 1 — Reconnaissance:** Survey the facility using drone item from adjacent rooftop  
**Phase 2 — Disable Grid:** Cut power to wing B (hack terminal in service corridor)  
**Phase 3 — Breach Under Blackout:** 90-second window before emergency power restores  
**Phase 4 — Locate Contact:** 3 possible cells; must check each quickly  
**Phase 5 — Extract:** Fight out; escort handler to extraction vehicle  
**Complication:** Axiom scrambles asset team (elite 4-NPC squad) on extraction; must fight through or outwit  

**Rewards:** Main story progression + director intel + unique SMG schematic + Axiom Threat jumps to 60

---

#### MISSION 10: "BURN THE LEDGER" (Finale-tier — Heist/Sabotage)
**Giver:** All faction leaders (requires all 4 playable factions at +40 rep minimum)  
**Location:** The Spire — Axiom Directorate headquarters  
**Setup:** Kael has assembled evidence against Director Vale. The Axiom financial ledger — containing every corrupt official, every funded faction, every paid hit — is stored on a physically air-gapped server inside the Directorate. Getting it out means going in. Every faction provides a specific piece of the plan.

**Phase 1 — Ruin Syndicate Role:** Create city-wide distraction event (coordinated riot in The Pits + Neon Flats)  
**Phase 2 — Hollow Kings Role:** Disable Axiom surveillance network (15-minute window)  
**Phase 3 — Meridian Cartel Role:** Provide extraction route via harbor submarine dock  
**Phase 4 — Kael Enters The Spire:** Full combat assault or stealth infiltration (player choice)  
**Phase 5 — Server Room:** 4-minute extract under combat pressure  
**Phase 6 — Director Vale confrontation:** Choice-based — capture (maximum ending) or eliminate (good ending) or expose remotely (variable ending)  

**Rewards:** Campaign conclusion + city ownership percentage shift + unique legendary weapon + post-game state unlocked

---

## 1.9 WORLD DESIGN

### VEKTOR CITY — Overview

Vektor is built on a peninsula of reclaimed industrial land. The northern shore faces a bay; the southern edge backs onto dead refineries and railroad infrastructure. Five distinct districts are separated by geography, architecture, and economics. A ring highway connects all districts; tunnels and overpasses create interior flow.

**Population (simulated):** ~12,000 active NPCs at any time (density scaled per district)  
**Size:** Approximately 4.5 km × 3 km navigable area  
**Time cycle:** 24-minute real-time day/night cycle  
**Weather:** 6 states (clear, overcast, light rain, heavy rain, fog, storm)

---

### District 1: THE PITS (Southern Industrial)

| Attribute | Value |
|-----------|-------|
| Controller | Ruin Syndicate |
| GDP feel | Working poor to destitute |
| Population density | High foot, low vehicle |
| Time activity peak | Night |
| Police presence | Low (2 patrol cars; foot absent) |
| Color palette | Rust, concrete grey, amber lamplight |
| Ambient sounds | Heavy machinery, distant trains, dripping water |

**Features:**
- Rail freight depot (mission hub)
- Multiple chop shops (Syndicate-controlled)
- Underground fighting rings (income source; reputation events)
- Abandoned refinery complex (raid site; faction stronghold)
- Narrow alleys and dead ends (motorcycle advantage; car trap risk)

**Gameplay Traits:**
- Low police presence means player has freedom to operate
- Tight streets punish large vehicles
- Best zone for grinding early cash and Syndicate rep
- Nighttime brings out Syndicate patrol density; daytime is calmer

**Risk:** Open faction warfare between Syndicate and Warden incursion teams

---

### District 2: THE GRID (Financial/Administrative Center)

| Attribute | Value |
|-----------|-------|
| Controller | Warden Bloc |
| GDP feel | Affluent, corporate |
| Population density | Medium foot (business-suited), heavy vehicle |
| Time activity peak | Daytime (business hours) |
| Police presence | Very High (Warden + minimal VCPD) |
| Color palette | Steel blue, white, glass reflection |
| Ambient sounds | Traffic, phone calls, corporate PA systems |

**Features:**
- Warden Bloc HQ tower (late-game infiltration target)
- Banks and financial institutions (heist sites)
- Elevated sky-bridges between buildings
- Restricted parking zones (abandoned vehicles towed in 2 minutes)
- Corporate server rooms (hacking mission sites)

**Gameplay Traits:**
- Most dangerous district for combat — Warden response is brutal and fast
- High value targets; highest cash mission rewards available here
- Surveillance cameras on every block (heat escalation accelerated)
- Disguise utility highest here (Warden uniform enables movement freedom)

**Risk:** Warden Bloc shoots first at Heat 2+; player cannot sustain combat without preparation

---

### District 3: NEON FLATS (Entertainment/Nightlife)

| Attribute | Value |
|-----------|-------|
| Controller | Hollow Kings (informal) |
| GDP feel | Mixed; creative class + underground economy |
| Population density | Very High nighttime; moderate daytime |
| Time activity peak | Night (peak 10PM–4AM) |
| Police presence | Moderate; stretched thin by event coverage |
| Color palette | Neon pink, electric blue, deep violet, black |
| Ambient sounds | Music from clubs, crowd noise, hover-boards, rain on neon |

**Features:**
- Club district (faction meeting points, undercover missions)
- Hollow Kings network nodes (hackable / destroyable)
- Street art galleries (marking territory via graffiti mechanic)
- Underground race circuit
- Open plaza events (live music, festivals — organic NPC gatherings)

**Gameplay Traits:**
- Best zone for stealth/social missions
- Fence prices highest here; best black market access
- Night events create chaos cover (easier to operate, harder to get clean line of sight)
- Hollow Kings provide real-time intelligence if allied

**Risk:** Dense pedestrian population means civilian casualties escalate heat rapidly

---

### District 4: THE WATERFRONT (Harbor/Industrial Commerce)

| Attribute | Value |
|-----------|-------|
| Controller | Meridian Cartel |
| GDP feel | Blue-collar mixed with organized crime wealth |
| Population density | Medium; high at docks |
| Time activity peak | Dusk/Dawn (shift changes; shipment windows) |
| Police presence | Medium; Cartel has port authority bribed |
| Color palette | Sea green, dark grey, orange crane lighting |
| Ambient sounds | Ship horns, crane operations, water, seagulls |

**Features:**
- Active shipping docks (cargo missions, import vehicle spawns)
- Submarine dock (late game extraction route)
- Warehouse district (shootout venues; heist targets)
- Fish market (civilian daytime density; night becomes empty)
- Cartel-owned import shop (unique vehicle and weapon availability)

**Gameplay Traits:**
- Best vehicle variety and exotic car spawns
- Water access enables boat-based escape routes
- Dock cranes are interactable (can drop containers; physics hazard)
- Fog at night reduces visibility; AI patrol spacing wider

**Risk:** Meridian Cartel convoy traffic is heavily armed; wrong vehicle can trigger armed escort

---

### District 5: THE SPIRE (Government/Surveillance)

| Attribute | Value |
|-----------|-------|
| Controller | Axiom Directorate |
| GDP feel | Sterile, official, no organic life |
| Population density | Very Low; mostly Axiom operatives and government workers |
| Time activity peak | Always active (24/7 surveillance) |
| Police presence | N/A — Axiom runs its own security apparatus |
| Color palette | Gunmetal, red accent lighting, clinical white |
| Ambient sounds | Low hum of servers, drone patrols, sparse footsteps |

**Features:**
- Axiom HQ (endgame; massive vertical building)
- Drone surveillance station (destroyable; reduces Axiom Threat)
- Communications blackout zones (no map, no mini-map inside)
- Hidden underground tunnel network (discovered via Hollow Kings intel)
- Director Vale's personal transport route (ambush site in final acts)

**Gameplay Traits:**
- Inaccessible early game without disguise or Hollow Kings surveillance disable
- Every action inside is permanent — kills here escalate Threat irreversibly
- No civilian population; no witnesses except Axiom assets
- High reward; extreme risk

**Risk:** Axiom operatives are elite AI; fastest response time; no heat system applies (Axiom ignores city police entirely)

---

### Environmental Systems

**Traffic:**
- Vehicles populate roads based on district + time of day
- Peak traffic: daytime in The Grid; nighttime in Neon Flats
- Traffic reacts to player actions (accidents create backups; police pursuit clears roads)
- Parked vehicles distributed by neighborhood wealth (luxury in The Grid; junk in The Pits)

**Weather:**
- Clear: standard behavior
- Overcast: -10% NPC visibility range
- Light rain: road traction -15%; Hollow Kings activity increases
- Heavy rain: -30% traction; visibility reduced 40%; helicopter patrol grounded
- Fog: -50% NPC visibility; player map range reduced 30%
- Storm: Catastrophic visibility; no helicopter; all outdoor civilians flee indoors; power grid flickers

**Time of Day:**
- 6 AM–9 AM: Morning commute; high civilian density; low criminal activity
- 9 AM–5 PM: Business hours; The Grid at peak activity
- 5 PM–9 PM: Transition; rising Neon Flats activity
- 9 PM–3 AM: Night peak; Neon Flats full; Pits active; Waterfront shift change
- 3 AM–6 AM: Dead hours; low population; police patrols increase proportionally

---

## 1.10 ECONOMY AND PROGRESSION

### Currency System

| Currency | How Earned | How Spent |
|----------|-----------|-----------|
| VTEK (primary) | Mission payouts, robbery, racing, jobs | Weapons, ammo, vehicles, upgrades, bribes |
| Heat Tokens | Completing missions while wanted | Heat reduction services, corrupt officer bribes |
| Faction Favors | Completing faction tasks | Faction-specific services (intel, vehicles, muscle) |
| Contraband | Looted from enemies/sites | Fenced for VTEK at Hollow Kings or Meridian |

### Upgrade Trees

**Character Upgrades (4 trees):**

```
OPERATOR TREE (Combat)
  Tier 1: +10 max HP | Faster reload | Reduced recoil
  Tier 2: +20 max HP | Dual-wield pistols | Suppressed weapon proficiency
  Tier 3: Armor penetration rounds | Combat roll i-frames +4 | Weapon sling (3rd weapon slot)

DRIVER TREE (Vehicle)
  Tier 1: +10 vehicle HP threshold before damage effects | Faster entry 0.6s | Ram +20% force
  Tier 2: Drift control +15% | Motorcycle speed +15% | Vehicle repair kit (consumable slot)
  Tier 3: Spike strip immunity | Stunt multiplier | Vehicle EMP (active ability)

GHOST TREE (Stealth/Infiltration)
  Tier 1: Crouch speed +20% | Subdue range +0.5m | CCTV awareness UI
  Tier 2: Disguise duration +60s | Pickpocket unlock | Hack speed -1s
  Tier 3: Subdue through cover | Silent sprint | Enemy comms intercept (map marks enemies for 30s)

NETWORKER TREE (Social/Economy)
  Tier 1: Bribe cost -20% | Fence bonus +15% | Extra faction mission slot
  Tier 2: Black market prices -15% | NPC informant unlock | Reputation bleed reduction -50%
  Tier 3: Double cash missions | Faction favor passive income | Heat decay speed +25%
```

### Skill Systems

Skills are **unlocked through action repetition**, not just purchase:
- Drive 50 km: unlock Tire Grip upgrade
- Kill 25 enemies with pistol: unlock dual-wield
- Complete 10 stealth missions without alert: unlock Silent Sprint
- Win 5 street races: unlock Drift Control Tier 2
- Bribe 10 officers: unlock reduced bribe costs

### Unlock Loops

1. **Cash** → Buy upgrades, vehicles, safehouses, consumables
2. **Reputation** → Unlock mission chains, services, territory benefits
3. **Skill Actions** → Unlock skill tree nodes
4. **Story Progress** → New districts, late-game items, narrative content
5. **Faction Favors** → Access exclusive items not available in shops

---

## 1.11 EMERGENT GAMEPLAY SYSTEMS

The following systems interact without scripting to create emergent scenarios:

**Core Systems:**
1. Heat/wanted system
2. Faction reputation and territory
3. Traffic simulation
4. Weather
5. Time of day
6. AI behavior states
7. Economy (shop prices, faction services)
8. Vehicle damage
9. NPC scheduling (civilians have daily routines)
10. World event triggers

### Emergent Scenario Examples

1. **Caught in the Crossfire:** Player is mid-mission in The Pits when a Warden incursion triggers a Syndicate-Warden street battle. Player must navigate through active faction combat not scripted for their presence.

2. **Storm Advantage:** Player is being chased at Heat 4 during a storm — helicopter grounds, civilian witnesses flee indoors, police AI sight range halved. Suddenly viable escape that was impossible in clear weather.

3. **Civilian Calls In:** Player steals a car quietly at Heat 0 — one background civilian who was in "REPORTING" state on an unrelated crime spots the theft and adds a 0.5 heat increment, pushing the player into active police attention unexpectedly.

4. **Faction Ripple:** Player destroys a Meridian Cartel shipment for Hollow Kings. Cartel raises import prices city-wide. All shops now show +15% cost on weapons for 2 in-game days. Player's ammo cost rises.

5. **Race Turns Combat:** During an open-world street race, a Warden Bloc patrol car tries to stop participants. Rival racers attack the patrol car. Race turns into a gang brawl. Player can exploit the chaos or be caught in it.

6. **Domino Crash:** Player causes a multi-car accident on the ring highway. Traffic backs up. A Meridian cargo convoy stuck in traffic becomes an unplanned ambush opportunity. Not scripted — purely systemic.

7. **NPC Revenge:** Player eliminated a Syndicate lieutenant during a mission. His crew (NPCs with social connections) now actively patrol the player's last-known location seeking revenge — spontaneous, persistent until killed or appeased.

8. **Safehouse Compromised:** Player returns to safehouse at Heat 5 — Axiom operatives have been tracking movements; the safehouse is staked out. Player walks into an ambush because they relied on routine.

9. **Weather Changes the Plan:** Player planned an outdoor assassination during clear weather — fog rolls in mid-approach. Guards' visual range drops 50%. Completely changes difficulty and approach (now easier to approach, harder to confirm target visually).

10. **Faction Loyalty Test:** Hollow Kings rep is high; Meridian Cartel rep is also high. Player is caught by a Meridian contact while carrying a Hollow Kings device. Meridian contact demands an explanation. Player's dialogue options affect both factions' rep — no scripted "correct" answer.

---

## 1.12 LIVE EVENTS SYSTEM

### Dynamic World Events

Live events are **procedurally triggered** based on world state, time, and player activity. They are not scripted cutscenes — they spawn into the open world and the player encounters them organically or receives a radio tip.

**Event Categories:**

| Category | Examples | Trigger Logic |
|----------|----------|--------------|
| Faction Conflict | Territory skirmish, assassination attempt, convoy ambush | Faction reputation differential > 40; faction in conflict state |
| Police Operations | Sting operation, precinct raid, undercover sweep | Heat level average above 2 in district for 5+ in-game hours |
| Natural/City Events | Storm surge flooding port roads, power grid failure, fire | Weather extreme + time + RNG seeded every in-game day |
| Economic Events | Black market price war, smuggling surge, shop robbery | Player inaction in economy for 3 in-game days |
| Street Events | Underground fight night, pop-up race, mob gathering | NPC scheduler + time of day + district |
| Axiom Events | Surveillance drone deployment, asset extraction, false flag | Axiom Threat > 50 |

### Event Trigger Logic (Pseudocode)

```
every IN_GAME_HOUR:
  for each EVENT_TEMPLATE in WORLD_EVENT_POOL:
    if EVENT_TEMPLATE.conditions_met(world_state):
      if random(0,100) < EVENT_TEMPLATE.probability:
        spawn_event(EVENT_TEMPLATE, location=district_valid_spawn)
        notify_player_if_nearby(radius=500m)  // radio tip or visual cue only
```

### Player Impact

- Participating in events earns bonus cash and reputation
- Ignoring events allows them to resolve on their own (faction wins, loses territory, etc.)
- Some events time-out and change world state permanently
- Player can trigger events deliberately (e.g., kill a faction leader to trigger succession crisis event)
- Events feed back into the mission pipeline — an event the player resolved spawns a follow-up mission from affected faction
