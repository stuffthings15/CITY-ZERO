# Audio Folder Structure Blueprint

This blueprint outlines a recommended directory layout for organizing audio assets, mixers, and related resources in your Unity project. Keeping a consistent structure helps your team find and manage audio content easily.

## Recommended Directory Tree

```
Assets/
└── Audio/
    ├── Clips/
    │   ├── SFX/
    │   │   ├── Weapons/
    │   │   ├── Vehicles/
    │   │   ├── UI/
    │   │   ├── Footsteps/
    │   │   └── Ambience/
    │   ├── Music/
    │   │   ├── Exploration/
    │   │   ├── Combat/
    │   │   ├── Radio/
    │   │   │   ├── Channel1/
    │   │   │   ├── Channel2/
    │   │   │   └── Channel3/
    │   │   └── Stingers/
    │   └── VO/
    │       ├── NPC/
    │       ├── RadioDJ/
    │       └── Misc/
    ├── Mixers/
    │   ├── Master.mixer
    │   ├── Music.mixer
    │   ├── SFX.mixer
    │   └── Radio.mixer
    ├── ScriptableObjects/
    │   ├── Playlists/
    │   └── AudioSettings/
    └── Resources/
        ├── AudioEvents/
        └── RadioChannels/
```

### Notes

- **Clips/** – Store raw `AudioClip` assets here. Organize by category to simplify referencing. For example, place all weapon sound effects in `Clips/SFX/Weapons/`.
- **Music/** – Divide music into thematic groups (e.g. combat, exploration) and further by usage (radio channels, stingers). Stingers are short cues for mission success or failure.
- **VO/** – Store voice overs separated by speaker or usage type. Use subfolders for different characters or contexts.
- **Mixers/** – Create separate mixer assets for master, music, SFX, and radio if you plan to mix them independently. Assign these mixers to your `AudioSource` components.
- **ScriptableObjects/** – Use custom ScriptableObjects to define playlists, radio channels, or audio settings. These can be edited in the Unity inspector and referenced at runtime.
- **Resources/** – Include runtime‑loaded data such as audio event definitions or serialized radio channel configurations.

Maintaining a clear directory structure early will save time as your audio library grows. Feel free to adapt this blueprint to fit your team’s workflow and naming conventions.