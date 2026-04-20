# Integration Guide for UI and Audio Systems

This guide explains how to wire up the UI and audio systems in a Unity scene. It assumes you have imported the packages for UI/UX and audio systems, as well as the necessary managers.

## 1. Set Up the Scene

1. **Create an empty GameObject** and name it `AudioManager`. Attach the following scripts to it:
   - `SoundManager`
   - `MusicManager`
   - `RadioManager`
2. **Create the HUD Canvas** using the blueprint from the UI package. Attach `HUDManager` and assign references to your health, armor, heat, ammo, cash, and objective UI elements.
3. **Create the Mission UI Panel** and attach `MissionUI`. Assign the title, objective, and progress text fields.
4. **Create a Minimap Canvas** and attach `MinimapController`. Assign the player icon, POI container, and minimap background. Provide a prefab for POI icons if necessary.
5. **Add a Main Menu Canvas** and a Pause Menu Canvas, each with `MenuController`. Assign the relevant panels and options.
6. **Create a Game Manager** (e.g. `DemoGameManager`) to handle input and update the UI and audio systems. Attach it to an empty GameObject named `DemoGameManager`.
7. **Create a Player GameObject** with a simple movement script. Assign its transform to the `MinimapController`.

## 2. Register Audio Clips

The `SoundManager` requires that all audio clips be assigned in the `sfxItems` array. For each sound effect:

- Set the `name` property to a unique identifier (e.g. `gunshot_pistol`).
- Assign the `clip` property to an `AudioClip` asset in your project.
- Optionally assign a category and `AudioMixerGroup`.

For music and radio:

- Create playlists in the `MusicManager.playlists` array. Each playlist should have a name, a list of tracks, a loop flag, a music category, and optionally an `AudioMixerGroup`.
- Create radio channels in the `RadioManager.channels` array with names and track lists. Optionally assign mixer groups.

## 3. Update the UI

Use the `DemoGameManager` as an example of how to update the UI:

- Call `hudManager.UpdateHealth()` and `UpdateArmor()` whenever the player takes damage or heals.
- Call `hudManager.UpdateAmmo()` whenever the player fires or reloads.
- Call `hudManager.UpdateHeat()` when the wanted/heat level changes.
- Call `hudManager.UpdateCash()` and `UpdateObjective()` as needed.
- Call `missionUI.SetTitle()`, `SetObjective()`, and `SetProgress()` to reflect the current mission state.

## 4. Play Sounds and Music

Use the audio managers:

- Call `SoundManager.Instance.PlaySFX(name, position)` to play 3D sounds.
- Call `SoundManager.Instance.PlaySFX2D(name)` for UI or global sounds.
- Call `MusicManager.Instance.PlayPlaylist(playlistName)` to change background music.
- Call `MusicManager.Instance.StopMusic()` to stop music.
- Call `RadioManager.Instance.TuneToChannel(channelName)` to tune to a specific radio station. Use `NextChannel()` and `PreviousChannel()` to cycle channels.

## 5. Extending the Demo

This demo is intentionally simple. You can extend it by:

- Adding more UI panels (inventory, map) and hooking them to game systems.
- Expanding the `DemoGameManager` to interact with missions, factions, and vehicles.
- Incorporating audio feedback for interactions like pickups and mission completion.
- Integrating options menus to control audio and UI preferences.

Use this guide to adapt the core systems to your actual gameplay scripts.