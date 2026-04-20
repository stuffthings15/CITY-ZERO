# CITY//ZERO Audio System Overview

This document describes the core audio systems provided in the audio pack. These systems are designed to give you a flexible starting point for implementing sound and music in **CITY//ZERO**. All scripts are written in C# for Unity and require the built‑in `AudioSource`, `AudioClip`, and optional `AudioMixer` classes.

## Goals

- Provide a centralized manager for sound effects (SFX) playback with support for categories and 3D/2D sound.
- Create a music management system capable of handling layered tracks, crossfading, and event‑driven transitions.
- Supply a radio/music channel system for in‑car or ambient music playback.
- Offer a set of constants and guidelines for structuring audio assets and folders.

## Contents

This pack includes the following scripts and documentation:

- **Scripts/Audio/SoundManager.cs** – Centralized SFX playback with pooling and category control.
- **Scripts/Audio/MusicManager.cs** – Manages background music playlists, fading, and transitions.
- **Scripts/Audio/RadioManager.cs** – Handles radio channels, track switching, and integration with the vehicle system.
- **Scripts/Audio/AudioConstants.cs** – Defines enums and constants for audio categories and default volumes.
- **Docs/Blueprints/Audio_Folder_Structure.md** – Recommended layout for organizing audio files and mixers.

These scripts are meant to be extended. They provide core functionality but do not cover every possible use case (e.g. dynamic reverb, spatial audio occlusion). You should integrate them with Unity’s `AudioMixer` for real mixing and volume control.

## Getting Started

1. **Organize Audio Assets** – Follow the guidelines in `Audio_Folder_Structure.md` to place audio clips in appropriate folders.
2. **Add Managers to a Scene** – Create an empty `GameObject` in your bootstrap scene and attach `SoundManager`, `MusicManager`, and `RadioManager` components. Assign audio mixer groups if desired.
3. **Register Clips and Play** – Use the public methods on each manager to play clips. For example:

   ```csharp
   SoundManager.Instance.PlaySFX("gunshot_pistol", transform.position);
   MusicManager.Instance.PlayPlaylist("Combat");
   RadioManager.Instance.TuneToChannel("80s Hits");
   ```

4. **Customize and Extend** – Adjust volume levels in `AudioConstants` and add your own categories or methods. Hook into game events to trigger music transitions or radio channel changes.

## Next Steps

To get the most out of these systems, consider:

- Creating an `AudioMixer` with exposed parameters for master, music, and SFX buses. Assign mixer groups on the managers.
- Extending `RadioManager` to support DJ commentary or dynamic playlists.
- Implementing spatialization and occlusion using Unity’s built‑in audio settings or third‑party plugins.
- Adding an options menu to allow players to adjust volume levels. The `MusicManager` and `SoundManager` already expose volume properties that can be modified at runtime.