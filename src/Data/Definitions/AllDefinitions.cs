using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CityZero.Data.Definitions
{
    // ─── VEHICLE ─────────────────────────────────────────────────────────────────

    public class VehicleDefinition
    {
        [JsonPropertyName("vehicle_id")]      public string VehicleId { get; set; }
        [JsonPropertyName("display_name")]    public string DisplayName { get; set; }
        [JsonPropertyName("class")]           public string Class { get; set; }
        [JsonPropertyName("faction_affiliation")] public string FactionAffiliation { get; set; }
        [JsonPropertyName("stats")]           public VehicleStats Stats { get; set; }
        [JsonPropertyName("damage_zones")]    public Dictionary<string, DamageZone> DamageZones { get; set; }
        [JsonPropertyName("traction_modifiers")] public Dictionary<string, float> TractionModifiers { get; set; }
        [JsonPropertyName("seats")]           public int Seats { get; set; }
        [JsonPropertyName("can_be_stolen")]   public bool CanBeStolen { get; set; }
        [JsonPropertyName("spawn_districts")] public List<string> SpawnDistricts { get; set; }
        [JsonPropertyName("spawn_weight")]    public int SpawnWeight { get; set; }
        [JsonPropertyName("price_shop")]      public int? PriceShop { get; set; }
        [JsonPropertyName("unlock_condition")] public string UnlockCondition { get; set; }
        [JsonPropertyName("audio_engine_id")] public string AudioEngineId { get; set; }
        [JsonPropertyName("model_path")]      public string ModelPath { get; set; }
    }

    public class VehicleStats
    {
        [JsonPropertyName("top_speed_kmh")]  public float TopSpeedKmh { get; set; }
        [JsonPropertyName("acceleration")]   public float Acceleration { get; set; }
        [JsonPropertyName("braking")]        public float Braking { get; set; }
        [JsonPropertyName("handling")]       public float Handling { get; set; }
        [JsonPropertyName("durability_hp")]  public int DurabilityHp { get; set; }
        [JsonPropertyName("weight_class")]   public string WeightClass { get; set; }
        [JsonPropertyName("drive_type")]     public string DriveType { get; set; }
    }

    public class DamageZone
    {
        [JsonPropertyName("hp")]               public int Hp { get; set; }
        [JsonPropertyName("effect_on_deplete")] public string EffectOnDeplete { get; set; }
    }

    // ─── WEAPON ──────────────────────────────────────────────────────────────────

    public class WeaponDefinition
    {
        [JsonPropertyName("weapon_id")]        public string WeaponId { get; set; }
        [JsonPropertyName("display_name")]     public string DisplayName { get; set; }
        [JsonPropertyName("category")]         public string Category { get; set; }
        [JsonPropertyName("stats")]            public WeaponStats Stats { get; set; }
        [JsonPropertyName("fire_modes")]       public List<string> FireModes { get; set; }
        [JsonPropertyName("ammo_type")]        public string AmmoType { get; set; }
        [JsonPropertyName("concealable")]      public bool Concealable { get; set; }
        [JsonPropertyName("heat_modifier_per_shot")] public float HeatModifierPerShot { get; set; }
        [JsonPropertyName("can_suppress")]     public bool CanSuppress { get; set; }
        [JsonPropertyName("suppressor_slot")]  public bool SuppressorSlot { get; set; }
        [JsonPropertyName("upgrades")]         public List<string> Upgrades { get; set; }
        [JsonPropertyName("price_shop")]       public int? PriceShop { get; set; }
        [JsonPropertyName("unlock_condition")] public string UnlockCondition { get; set; }
        [JsonPropertyName("audio_shot_id")]    public string AudioShotId { get; set; }
        [JsonPropertyName("model_path")]       public string ModelPath { get; set; }
    }

    public class WeaponStats
    {
        [JsonPropertyName("damage_per_bullet")]        public float DamagePerBullet { get; set; }
        [JsonPropertyName("fire_rate_rpm")]            public float FireRateRpm { get; set; }
        [JsonPropertyName("magazine_size")]            public int MagazineSize { get; set; }
        [JsonPropertyName("max_reserve_ammo")]         public int MaxReserveAmmo { get; set; }
        [JsonPropertyName("reload_time_sec")]          public float ReloadTimeSec { get; set; }
        [JsonPropertyName("effective_range_m")]        public float EffectiveRangeM { get; set; }
        [JsonPropertyName("falloff_start_m")]          public float FalloffStartM { get; set; }
        [JsonPropertyName("falloff_end_m")]            public float FalloffEndM { get; set; }
        [JsonPropertyName("recoil_horizontal")]        public float RecoilHorizontal { get; set; }
        [JsonPropertyName("recoil_vertical")]          public float RecoilVertical { get; set; }
        [JsonPropertyName("aim_cone_degrees")]         public float AimConeDegrees { get; set; }
        [JsonPropertyName("aim_cone_sprint_modifier")] public float AimConeSprintModifier { get; set; }
    }

    // ─── FACTION ─────────────────────────────────────────────────────────────────

    public class FactionDefinition
    {
        [JsonPropertyName("faction_id")]          public string FactionId { get; set; }
        [JsonPropertyName("display_name")]        public string DisplayName { get; set; }
        [JsonPropertyName("territory_districts")] public List<string> TerritoryDistricts { get; set; }
        [JsonPropertyName("leader_npc_id")]       public string LeaderNpcId { get; set; }
        [JsonPropertyName("starting_reputation")] public float StartingReputation { get; set; }
        [JsonPropertyName("reputation_bleed")]    public List<RepBleed> ReputationBleed { get; set; }
        [JsonPropertyName("ai_profile")]          public FactionAIProfile AiProfile { get; set; }
        [JsonPropertyName("missions_available")]  public List<string> MissionsAvailable { get; set; }
    }

    public class RepBleed
    {
        [JsonPropertyName("target_faction")] public string TargetFaction { get; set; }
        [JsonPropertyName("multiplier")]     public float Multiplier { get; set; }
    }

    public class FactionAIProfile
    {
        [JsonPropertyName("aggression")]             public float Aggression { get; set; }
        [JsonPropertyName("cohesion")]               public float Cohesion { get; set; }
        [JsonPropertyName("retreat_hp_threshold")]   public float RetreatHpThreshold { get; set; }
        [JsonPropertyName("ranged_preference")]      public float RangedPreference { get; set; }
    }

    // ─── DISTRICT ────────────────────────────────────────────────────────────────

    public class DistrictDefinition
    {
        [JsonPropertyName("district_id")]          public string DistrictId { get; set; }
        [JsonPropertyName("display_name")]         public string DisplayName { get; set; }
        [JsonPropertyName("controlling_faction")]  public string ControllingFaction { get; set; }
        [JsonPropertyName("heat_escalation_modifier")] public float HeatEscalationModifier { get; set; }
        [JsonPropertyName("cctv_coverage")]        public float CctvCoverage { get; set; }
        [JsonPropertyName("unlock_condition")]     public string UnlockCondition { get; set; }
        [JsonPropertyName("gameplay_traits")]      public List<string> GameplayTraits { get; set; }
    }

    // ─── MISSION ─────────────────────────────────────────────────────────────────

    public class MissionDefinition
    {
        [JsonPropertyName("mission_id")]    public string MissionId { get; set; }
        [JsonPropertyName("title")]         public string Title { get; set; }
        [JsonPropertyName("archetype")]     public string Archetype { get; set; }
        [JsonPropertyName("giver_faction")] public string GiverFaction { get; set; }
        [JsonPropertyName("giver_npc_id")]  public string GiverNpcId { get; set; }
        [JsonPropertyName("prerequisites")] public MissionPrerequisites Prerequisites { get; set; }
        [JsonPropertyName("time_limit_seconds")] public int? TimeLimitSeconds { get; set; }
        [JsonPropertyName("heat_modifier_base")] public float HeatModifierBase { get; set; }
        [JsonPropertyName("objectives")]    public List<MissionObjective> Objectives { get; set; }
        [JsonPropertyName("rewards")]       public MissionRewards Rewards { get; set; }
    }

    public class MissionPrerequisites
    {
        [JsonPropertyName("missions_completed")] public List<string> MissionsCompleted { get; set; }
        [JsonPropertyName("story_flag")]         public string StoryFlag { get; set; }
    }

    public class MissionObjective
    {
        [JsonPropertyName("id")]       public string Id { get; set; }
        [JsonPropertyName("type")]     public string Type { get; set; }
        [JsonPropertyName("target")]   public string Target { get; set; }
        [JsonPropertyName("label")]    public string Label { get; set; }
        [JsonPropertyName("required")] public bool Required { get; set; }
        [JsonPropertyName("bonus_cash")] public int BonusCash { get; set; }
    }

    public class MissionRewards
    {
        [JsonPropertyName("cash")]          public int Cash { get; set; }
        [JsonPropertyName("reputation")]    public List<RepDelta> Reputation { get; set; }
        [JsonPropertyName("unlock_flags")]  public List<string> UnlockFlags { get; set; }
    }

    public class RepDelta
    {
        [JsonPropertyName("faction")] public string Faction { get; set; }
        [JsonPropertyName("delta")]   public int Delta { get; set; }
    }
}
