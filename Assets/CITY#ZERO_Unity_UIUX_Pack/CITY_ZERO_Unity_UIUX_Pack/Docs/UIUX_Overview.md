# CITY//ZERO UI/UX Overview

This document provides an overview of the user interface (UI) and user experience (UX) systems included in this pack. The goal is to give you a strong starting point for implementing a cohesive and scalable UI in Unity.

## Objectives

- Deliver a clean, readable HUD suitable for a top‑down action game.
- Provide menu and pause screen scaffolds that can be expanded into full-featured UI flows.
- Include minimap and mission display systems for situational awareness.
- Establish code structures that are easily extended with additional widgets and screens.
- Document the expected hierarchy and component setup for Unity prefabs and canvases.

## Contents

This pack includes:

- **Scripts/UI/** – C# classes for managing HUD, menus, minimap, mission display, and common UI constants.
- **Docs/Blueprints/** – Markdown documents describing recommended prefab hierarchies and layout guidelines.
- **Docs/UIUX_Overview.md** – This document.

The scripts are designed to be attached to Unity `Canvas` objects and require Unity’s UI package (UGUI) or UI Toolkit depending on your implementation preferences. The classes do not rely on any external packages and should compile in a new Unity project.

## Getting Started

1. **Review the Code** – Look at each class in `Scripts/UI/` to understand its responsibilities. The code is documented with summary comments and example usage where appropriate.
2. **Create UI Prefabs** – Follow the guidance in the blueprint documents to create prefabs for your HUD, menus, and minimap. The scripts expect specific child objects to be present (e.g. Text or Image components), but these can be renamed via inspector fields.
3. **Add Scenes** – Add `HUDManager`, `MenuController`, and `MinimapController` to your main scene. Assign references in the inspector or use `FindObjectOfType` in a bootstrap script to wire them up at runtime.
4. **Hook Up Game Systems** – Connect the HUD update methods to your player, health, ammo, heat, and mission systems. The scripts include public methods for updating UI elements.

## Next Steps

This pack focuses on scaffolding rather than finished art. You will need to:

- Replace placeholder graphics with final art assets.
- Implement actual input bindings for menu navigation and pause toggling.
- Expand the minimap to render your world (e.g. using a secondary camera or render texture).
- Build out additional panels (inventory, map, options) as needed.

Refer to the `Docs/Blueprints` directory for more detailed setup instructions and diagrams.