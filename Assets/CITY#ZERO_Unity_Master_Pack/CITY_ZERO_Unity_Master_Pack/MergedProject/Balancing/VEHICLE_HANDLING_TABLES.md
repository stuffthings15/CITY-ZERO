# Vehicle Handling Tables

## Tuning Notes
- `MaxSpeed` is in gameplay meters per second
- `Acceleration` and `BrakeForce` are abstracted tuning coefficients
- `SteeringLow` is steering response at low speed
- `SteeringHigh` is steering response at top speed
- `Traction` governs grip
- `Durability` is normalized 0.0 to 1.0
- `HeatSignature` raises police interest if the vehicle is rare or memorable

## Vehicle Handling Table

| Class | Example Role | MaxSpeed | Acceleration | BrakeForce | SteeringLow | SteeringHigh | Traction | Mass | Durability | HeatSignature |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| Scooter | courier | 28 | 18.5 | 20.0 | 1.65 | 0.95 | 0.82 | 180 | 0.18 | 0.10 |
| Compact | city theft | 42 | 15.5 | 19.0 | 1.20 | 0.72 | 0.86 | 980 | 0.45 | 0.25 |
| Sedan | general civilian | 46 | 13.0 | 18.0 | 1.05 | 0.64 | 0.84 | 1300 | 0.56 | 0.22 |
| Taxi | durable civilian | 44 | 11.5 | 17.5 | 1.00 | 0.61 | 0.82 | 1450 | 0.62 | 0.28 |
| Muscle | chase car | 54 | 17.0 | 16.5 | 0.92 | 0.50 | 0.72 | 1550 | 0.60 | 0.44 |
| Coupe Performance | police bait | 58 | 18.2 | 17.0 | 0.98 | 0.53 | 0.76 | 1480 | 0.52 | 0.56 |
| Delivery Van | smuggling | 38 | 9.5 | 16.0 | 0.78 | 0.46 | 0.74 | 2100 | 0.74 | 0.30 |
| Cargo Van Heavy | transport | 34 | 7.8 | 15.5 | 0.70 | 0.42 | 0.70 | 2600 | 0.82 | 0.34 |
| Pickup Utility | industrial | 41 | 11.2 | 17.5 | 0.88 | 0.55 | 0.80 | 1900 | 0.68 | 0.26 |
| Faction Cruiser | gang pursuit | 50 | 14.5 | 17.0 | 0.96 | 0.58 | 0.78 | 1650 | 0.61 | 0.46 |
| Police Interceptor | pursuit | 56 | 16.8 | 18.2 | 1.02 | 0.60 | 0.81 | 1700 | 0.72 | 0.62 |
| Armored Security | convoy | 40 | 8.4 | 16.8 | 0.66 | 0.40 | 0.79 | 3200 | 0.95 | 0.71 |
| Ambulance | emergency | 45 | 12.0 | 17.8 | 0.94 | 0.58 | 0.80 | 2200 | 0.76 | 0.50 |
| Fire Van | emergency utility | 39 | 9.0 | 16.5 | 0.74 | 0.44 | 0.77 | 2900 | 0.88 | 0.55 |
| Prototype Escape Car | late-game reward | 62 | 20.5 | 18.8 | 1.10 | 0.64 | 0.83 | 1380 | 0.58 | 0.80 |

## Surface Modifiers

| Surface | Speed Multiplier | Traction Modifier | Notes |
|---|---:|---:|---|
| Dry Asphalt | 1.00 | 1.00 | baseline |
| Wet Asphalt | 0.96 | 0.86 | longer slides |
| Dirt Lot | 0.88 | 0.78 | visible dust trails |
| Metal Dock Plate | 0.93 | 0.72 | abrupt traction loss |
| Oil Spill | 0.82 | 0.48 | near-drift state |
| Grass Verge | 0.85 | 0.70 | offroad slowdown |

## Damage Threshold Suggestions

| Vehicle Class | Smoke Threshold | Engine Failure Threshold | Fire Threshold | Explosion Delay |
|---|---:|---:|---:|---:|
| Light | 35% | 15% | 5% | 6s |
| Medium | 30% | 12% | 4% | 7s |
| Heavy | 25% | 10% | 3% | 9s |
| Armored | 20% | 7% | 2% | 12s |
