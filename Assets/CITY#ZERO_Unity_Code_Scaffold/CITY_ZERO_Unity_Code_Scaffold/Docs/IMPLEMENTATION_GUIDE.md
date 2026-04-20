# Implementation Guide

## What is included
- DTOs matching the JSON config files
- Generic JSON loader
- ScriptableObject wrappers for authoring convenience
- Mission definitions, objective handlers, runtime and manager
- Starter player controller with move, aim, sprint, roll, interact, fire
- Starter vehicle controller using arcade top-down forces
- Basic damage and weapon interfaces
- Basic heat incident system

## First steps in Unity
1. Install packages:
   - Input System
   - Cinemachine
2. Set Active Input Handling to Input System Package
3. Create a bootstrap scene and add:
   - `GameBootstrap`
   - `ConfigDatabase`
4. Create an empty GameObject with `MissionManager`
5. Assign JSON TextAssets or use StreamingAssets path flow
6. Create a player prefab with:
   - Rigidbody
   - `PlayerController`
   - `HealthComponent`
   - optional `WeaponMotor`

## Recommended next engineering tasks
- Replace placeholder interact selection with trigger-based discovery
- Add save/load persistence for player and mission flags
- Add police dispatch listeners to the heat event flow
- Implement concrete mission objective handler subclasses
- Add AI event responses to heat changes
