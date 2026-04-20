# CITY//ZERO Unity Code Scaffold

This package provides Unity-ready C# scaffolding for:
- runtime data schemas
- JSON loading/import pipeline
- mission runtime architecture
- starter player controller
- starter vehicle controller
- basic combat hooks
- heat system scaffold
- bootstrap wiring

## Notes
- Namespace root: `CityZero`
- Built for Unity 6 style workflows
- Uses Unity Input System APIs in the player controller
- Intended as a production starter, not a drop-in finished game

## Suggested import order
1. Import into a Unity project
2. Install Input System package
3. Create input actions and hook references on `PlayerController`
4. Create prefabs and bind data files from `Assets/_Project/Data/Config`
5. Extend mission objective handlers for game-specific logic
