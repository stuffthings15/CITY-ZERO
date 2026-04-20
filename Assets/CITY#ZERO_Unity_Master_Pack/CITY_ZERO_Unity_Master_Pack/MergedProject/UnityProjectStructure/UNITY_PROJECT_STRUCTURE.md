# Unity Project Structure

## Top-Level Folder Structure
```text
Assets/
  _Project/
    Art/
      Characters/
        Player/
        Civilians/
        Factions/
        Police/
      Environment/
        Buildings/
        Roads/
        Props/
        Decals/
        FX/
      Vehicles/
        Civilian/
        Police/
        Faction/
        Emergency/
      Weapons/
      UI/
        Icons/
        HUD/
        Menus/
      Materials/
      Shaders/
      Lighting/
    Audio/
      Music/
        District/
        Chase/
        Mission/
        UI/
      SFX/
        Weapons/
        Vehicles/
        Ambient/
        Foley/
        UI/
      Voice/
        Dispatch/
        NPC/
        Faction/
        Vendors/
    Code/
      Core/
        Bootstrap/
        ServiceLocator/
        EventBus/
        SaveLoad/
        Settings/
        Utilities/
      Gameplay/
        Player/
        Combat/
        Vehicles/
        AI/
        Heat/
        Factions/
        Missions/
        Economy/
        Inventory/
        WorldEvents/
        Districts/
        Interactables/
        Traffic/
        Crowd/
        Weather/
        TimeOfDay/
      UI/
        HUD/
        Menus/
        Map/
        Shops/
        Debug/
      Data/
        ScriptableObjects/
        RuntimeDTOs/
        Importers/
      Editor/
        Validation/
        ContentTools/
        BuildTools/
    Data/
      Config/
        districts/
        factions/
        vehicles/
        weapons/
        shops/
        missions/
        events/
        audio/
      Localization/
      Balancing/
      LootTables/
    Prefabs/
      Characters/
      Vehicles/
      Weapons/
      Props/
      UI/
      FX/
      World/
    Scenes/
      Bootstrap/
      Frontend/
      Sandbox/
      Districts/
        OldQuarter/
        IronDocks/
        GlassHeights/
        AshIndustrial/
        NeonWard/
      Interiors/
      Test/
        Combat/
        Vehicles/
        AI/
        Streaming/
    AddressableGroups/
    Gizmos/
    Resources/
    Tests/
      EditMode/
      PlayMode/
  Plugins/
  ThirdParty/
ProjectSettings/
Packages/
```

## Naming Conventions
- Classes: PascalCase
- Private fields: `_camelCase`
- Interfaces: `IName`
- ScriptableObjects: suffix `Config`
- Runtime DTOs: suffix `Data`
- Prefabs: `PF_`
- Materials: `MAT_`
- Audio clips: `SFX_`, `MUS_`, `VO_`
- Scenes: `SCN_`
- Animation controllers: `AC_`
