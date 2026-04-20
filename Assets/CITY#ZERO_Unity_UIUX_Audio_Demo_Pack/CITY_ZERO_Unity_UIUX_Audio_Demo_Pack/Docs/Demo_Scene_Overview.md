# Demo Scene Overview

This pack provides a simple demonstration of how to integrate the UI and audio systems into a working scene for **CITY//ZERO**. The goal is to show how the various managers and UI components interact and to serve as a template for your own test scenes.

## Scene Description

The demo scene, `DemoBootstrapScene`, contains:

- A ground plane or simple environment for testing 3D sound positioning.
- A `DemoGameManager` GameObject with the `DemoGameManager` script attached to handle key inputs and drive UI updates.
- A HUD canvas using `HUDManager` configured with dummy bars and text fields.
- A mission UI panel using `MissionUI` to display the current objective and progress.
- A minimap canvas using `MinimapController`. For the demo, it simply tracks the player’s transform.
- A main menu and pause menu controlled by `MenuController`.
- Audio managers (`SoundManager`, `MusicManager`, `RadioManager`) attached to a persistent `AudioManager` GameObject.
- A `Player` capsule with a simple movement script (not provided here) to allow roaming around the scene and triggering sounds.

## Key Bindings

The `DemoGameManager` script uses basic keyboard inputs to demonstrate functionality:

- **H** – Decrease player health.
- **J** – Increase player health.
- **K** – Play a weapon sound effect (example: pistol shot). Ensure you have a clip named `gunshot_pistol` in the `SoundManager` library.
- **L** – Play a UI click sound effect (example: menu click). Ensure you have `ui_click` clip defined.
- **M** – Toggle between the “Exploration” and “Combat” music playlists.
- **N** – Tune to the next radio channel.
- **Esc** – Toggle the pause menu.

You can modify or extend these bindings in `DemoGameManager.cs` to suit your needs.

## How to Use

1. Import all required packages (production, code scaffold, next packs, compile-hardening, scene & cleanup, vertical slice wiring, editor workflow, art assets placeholder, UI/UX, audio) into a new Unity project.
2. Import the contents of this demo pack into your project.
3. Create a new scene and set it up according to the hierarchy described above. Alternatively, use the provided `DemoBootstrapScene` if supplied.
4. Assign the UI components (HUD, MissionUI, Minimap) and audio managers to their respective scripts in the inspector.
5. Enter Play mode and press the keys listed above to see UI updates and hear audio playback.

This demo should help you validate that the core systems are working together and guide your next steps in building the game.