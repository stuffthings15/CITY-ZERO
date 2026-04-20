# Vertical Slice Build Checklist

## Goal
Deliver one polished district with complete loop validation: steal, fight, drive, evade, earn, upgrade, repeat.

## Slice Scope
- District: Old Quarter
- Factions: Blue Saints, Razor Union
- Vehicles: 8
- Weapons: 6
- Main missions: 6
- Side activities: 4
- Heat system: levels 0 to 3
- Safehouse: 1
- Shop types: garage, black market, clinic
- Day/night cycle: yes
- Weather: clear and rain

## Must-Have Gameplay Systems
- Player movement
- Shooting and melee
- Vehicle enter/exit and theft
- Traffic spawning and pathing
- Civilian reactions
- Police heat response
- Mission framework
- Basic faction reputation
- Save/load
- HUD and minimap
- Shop purchase flow

## Content Checklist

### World
- [ ] Blockout Old Quarter complete
- [ ] Traffic roads spline-authored
- [ ] Sidewalk nav areas baked
- [ ] 3 landmark intersections
- [ ] 2 alleys with shortcuts
- [ ] 1 rooftop route network
- [ ] 1 garage, 1 clinic, 1 black market, 1 safehouse

### Vehicles
- [ ] Scooter
- [ ] Compact
- [ ] Sedan
- [ ] Taxi
- [ ] Delivery van
- [ ] Muscle car
- [ ] Faction cruiser
- [ ] Police interceptor

### Weapons
- [ ] Fists
- [ ] Pipe melee
- [ ] Pistol
- [ ] Revolver
- [ ] SMG
- [ ] Shotgun

### AI
- [ ] Civilian idle, flee, report states
- [ ] Police investigate, chase, arrest, combat states
- [ ] Faction patrol, engage, call backup states
- [ ] Vehicle traffic obeys lanes and basic avoidance

### Missions
- [ ] Static Spark
- [ ] Boosted Steel
- [ ] Dead Drop Dive
- [ ] Shopfront Warning
- [ ] Loose Change Convoy
- [ ] Siren Bait

### UI
- [ ] Health and armor HUD
- [ ] Current weapon and ammo
- [ ] Heat meter
- [ ] Objective tracker
- [ ] Minimap
- [ ] Shop UI
- [ ] Pause and settings menu

## Technical Checklist
- [ ] Addressables configured
- [ ] Bootstrap scene works
- [ ] Additive scene loading works
- [ ] Save/load captures inventory, vehicles, mission flags, reputation, time of day
- [ ] Data validation pipeline catches malformed JSON
- [ ] Debug overlays for AI, heat, mission state, and spawn zones
- [ ] Input rebinding functional

## Art Checklist
- [ ] Environment material set for district
- [ ] Modular building kit
- [ ] Street prop set
- [ ] Faction clothing set for Blue Saints and Razor Union
- [ ] Police uniform set
- [ ] Vehicle damage decals
- [ ] HUD icon pack for slice
- [ ] Muzzle flashes and impact FX

## Audio Checklist
- [ ] One district music loop day and night
- [ ] Chase music layer
- [ ] 50+ core SFX implemented
- [ ] Dispatch radio bark set
- [ ] Civilian panic bark set
- [ ] Garage and shop interaction sounds

## Acceptance Criteria
- Player can complete at least 20 minutes of varied play without blockers
- Heat escalation creates readable intensity jumps
- Driving feels fun within the first two minutes
- Missions can be completed with more than one tactic
- Average frame rate target met on min-spec dev target
- Save/load survives a full mission and active heat state without corruption
