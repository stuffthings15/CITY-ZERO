using UnityEngine;
using CityZero.UI;
using CityZero.Audio;

/// <summary>
/// A simple test harness that demonstrates how to connect the UI and audio systems.
/// Attach this script to an empty GameObject in your demo scene. It listens
/// for key presses to modify player stats, update the UI, and play sounds or music.
/// </summary>
public class DemoGameManager : MonoBehaviour
{
    [Header("UI References")]
    public HUDManager hudManager;
    public MissionUI missionUI;
    public MinimapController minimapController;
    public MenuController menuController;

    [Header("Managers")]
    public SoundManager soundManager;
    public MusicManager musicManager;
    public RadioManager radioManager;

    [Header("Demo Stats")]
    [Range(0f, 1f)] public float health = 1f;
    [Range(0f, 1f)] public float armor = 1f;
    [Range(0f, 1f)] public float heat = 0f;
    public int ammoCurrent = 12;
    public int ammoReserve = 48;
    public long cash = 0;

    private bool _combatMusic = false;
    private void Start()
    {
        // Initialize UI
        if (hudManager != null)
        {
            hudManager.UpdateHealth(health);
            hudManager.UpdateArmor(armor);
            hudManager.UpdateHeat(heat);
            hudManager.UpdateAmmo(ammoCurrent, ammoReserve);
            hudManager.UpdateCash(cash);
            hudManager.UpdateObjective("Deliver the packages to the drop points.");
        }
        if (missionUI != null)
        {
            missionUI.SetTitle("Demo Mission");
            missionUI.SetObjective("Press H/J to change health, K/L to play SFX, M/N to change music, Esc to pause.");
            missionUI.SetProgress(0, 0);
        }
        // Start exploration music
        if (musicManager != null)
        {
            musicManager.PlayPlaylist("Exploration");
        }
    }

    private void Update()
    {
        // Health decrease
        if (Input.GetKeyDown(KeyCode.H))
        {
            health = Mathf.Max(0f, health - 0.1f);
            hudManager?.UpdateHealth(health);
            hudManager?.UpdateHeat(Mathf.Min(1f, heat += 0.05f));
        }
        // Health increase
        if (Input.GetKeyDown(KeyCode.J))
        {
            health = Mathf.Min(1f, health + 0.1f);
            hudManager?.UpdateHealth(health);
            hudManager?.UpdateHeat(Mathf.Max(0f, heat -= 0.05f));
        }
        // Play weapon SFX
        if (Input.GetKeyDown(KeyCode.K))
        {
            soundManager?.PlaySFX("gunshot_pistol", transform.position);
            ammoCurrent = Mathf.Max(0, ammoCurrent - 1);
            hudManager?.UpdateAmmo(ammoCurrent, ammoReserve);
        }
        // Play UI click SFX
        if (Input.GetKeyDown(KeyCode.L))
        {
            soundManager?.PlaySFX2D("ui_click");
            cash += 100;
            hudManager?.UpdateCash(cash);
        }
        // Toggle music playlist
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (musicManager != null)
            {
                _combatMusic = !_combatMusic;
                string playlist = _combatMusic ? "Combat" : "Exploration";
                musicManager.PlayPlaylist(playlist);
            }
        }
        // Next radio channel
        if (Input.GetKeyDown(KeyCode.N))
        {
            radioManager?.NextChannel();
        }
        // Previous radio channel
        if (Input.GetKeyDown(KeyCode.B))
        {
            radioManager?.PreviousChannel();
        }
        // Pause/Resume game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menuController?.TogglePauseMenu();
        }
    }
}