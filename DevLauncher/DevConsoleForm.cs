using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CityZero.Core;
using CityZero.Core.Events;
using CityZero.Systems;
using CityZero.AI;

namespace CityZero.DevLauncher
{
    /// <summary>
    /// Interactive developer console. Boots all game systems and provides a REPL-style
    /// command interface so you can drive the simulation without a running Godot instance.
    ///
    /// Available commands:  help | heat | economy | faction | mission | save | time | ai | clear
    /// </summary>
    public partial class DevConsoleForm : Form
    {
        // ── Systems ───────────────────────────────────────────────────────────────
        private GameBus       _bus      = null!;
        private HeatSystem    _heat     = null!;
        private EconomySystem _economy  = null!;
        private FactionSystem _factions = null!;
        private MissionSystem _missions = null!;
        private SaveSystem    _save     = null!;
        private TimeManager   _time     = null!;

        // ── UI ────────────────────────────────────────────────────────────────────
        private RichTextBox _output  = null!;
        private TextBox     _input   = null!;
        private Button      _sendBtn = null!;
        private Label       _hud     = null!;
        private System.Windows.Forms.Timer _ticker = null!;

        private readonly List<string> _history = new();
        private int _histIdx = -1;

        public DevConsoleForm()
        {
            InitializeComponent();
            BootSystems();
            WireEventLog();
            PrintBanner();
            _input.Focus();
        }

        // ── Boot ──────────────────────────────────────────────────────────────────

        private void BootSystems()
        {
            ServiceLocator.Clear();

            _bus = new GameBus();
            _bus._Ready();

            _heat     = new HeatSystem();    _heat._Ready();
            _economy  = new EconomySystem(); _economy._Ready();
            _factions = new FactionSystem(); _factions._Ready();
            _missions = new MissionSystem(); _missions._Ready();
            _save     = new SaveSystem();    _save._Ready();
            _time     = new TimeManager();   _time._Ready();

            // Tick systems at ~30 fps
            _ticker = new System.Windows.Forms.Timer { Interval = 33 };
            _ticker.Tick += (_, _) =>
            {
                double delta = 0.033;
                _heat._Process(delta);
                _time._Process(delta);
                UpdateHud();
            };
            _ticker.Start();

            Log("[BOOT] All systems ready.", Color.Lime);
        }

        private void WireEventLog()
        {
            _bus.Subscribe<HeatChangedEvent>(e =>
                Log($"  [HEAT] {e.NewHeat:F2} → level {e.NewLevel}  ({e.Reason})", Color.OrangeRed));

            _bus.Subscribe<EconomyTransactionEvent>(e =>
                Log($"  [ECON] {(e.Amount >= 0 ? "+" : "")}{e.Amount:F0} ₸  balance={e.NewBalance:F0}  ({e.Reason})", Color.Gold));

            _bus.Subscribe<FactionRepChangedEvent>(e =>
                Log($"  [FACT] {e.FactionId}  {e.OldRep:+0;-0}→{e.NewRep:+0;-0}  ({e.Reason})", Color.Cyan));

            _bus.Subscribe<MissionStateChangedEvent>(e =>
                Log($"  [MISS] {e.MissionId}  {e.OldState}→{e.NewState}", Color.MediumPurple));

            _bus.Subscribe<ObjectiveCompletedEvent>(e =>
                Log($"  [OBJ]  {e.MissionId}/{e.ObjectiveId}  optional={e.IsOptional}", Color.Plum));
        }

        // ── Command dispatch ──────────────────────────────────────────────────────

        private void Execute(string raw)
        {
            raw = raw.Trim();
            if (string.IsNullOrEmpty(raw)) return;

            _history.Insert(0, raw);
            _histIdx = -1;
            Log($"\n> {raw}", Color.White);

            var parts = raw.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string cmd = parts[0].ToLowerInvariant();
            string[] args = parts.Skip(1).ToArray();

            switch (cmd)
            {
                case "help":    CmdHelp();              break;
                case "status":  CmdStatus();            break;
                case "clear":   _output.Clear(); PrintBanner(); break;

                case "heat":    CmdHeat(args);          break;
                case "economy":
                case "econ":    CmdEcon(args);          break;
                case "faction":
                case "fac":     CmdFaction(args);       break;
                case "mission":
                case "miss":    CmdMission(args);       break;
                case "save":    CmdSave(args);          break;
                case "time":    CmdTime(args);          break;
                case "ai":      CmdAI(args);            break;
                case "die":     _bus.Emit(new PlayerDiedEvent(0f)); break;

                default:
                    Log($"  Unknown command '{cmd}'. Type 'help'.", Color.Gray);
                    break;
            }
        }

        // ── Commands ──────────────────────────────────────────────────────────────

        private void CmdHelp()
        {
            Log(@"
  ┌─────────────────────────────────────────────────────────────────────┐
  │  CITY//ZERO  Dev Console                                            │
  ├───────────────────┬─────────────────────────────────────────────────┤
  │  status           │  Print all system states                        │
  │  clear            │  Clear output                                   │
  ├───────────────────┼─────────────────────────────────────────────────┤
  │  heat add <n>     │  Add n heat                                     │
  │  heat reduce <n>  │  Reduce n heat                                  │
  │  heat clear       │  Reset heat to 0                                │
  │  heat hide        │  Toggle player hidden                           │
  │  heat safe        │  Toggle safehouse                               │
  ├───────────────────┼─────────────────────────────────────────────────┤
  │  econ add <n>     │  Add n ₸ to wallet                              │
  │  econ spend <n>   │  Spend n ₸                                      │
  │  econ wallet      │  Print wallet balance                           │
  ├───────────────────┼─────────────────────────────────────────────────┤
  │  fac <id> <n>     │  Modify rep for faction by n                    │
  │  fac list         │  List all faction reps                          │
  ├───────────────────┼─────────────────────────────────────────────────┤
  │  miss start <id>  │  Activate mission                               │
  │  miss obj <id> <o>│  Complete objective o in mission id             │
  │  miss list        │  List active missions                           │
  ├───────────────────┼─────────────────────────────────────────────────┤
  │  save <slot>      │  Save game to slot (0-3)                        │
  │  save load <slot> │  Load game from slot                            │
  ├───────────────────┼─────────────────────────────────────────────────┤
  │  time speed <x>   │  Set time scale (e.g. 10)                       │
  │  time hour <h>    │  Jump to hour 0-24                              │
  ├───────────────────┼─────────────────────────────────────────────────┤
  │  ai demo          │  Run BT composite tree demo                     │
  ├───────────────────┼─────────────────────────────────────────────────┤
  │  die              │  Emit PlayerDiedEvent (fails all missions)       │
  └───────────────────┴─────────────────────────────────────────────────┘", Color.LightGray);
        }

        private void CmdStatus()
        {
            Log($"\n  HEAT     {_heat.CurrentHeat:F2} / 5.00  (level {_heat.HeatLevel})  wanted={_heat.IsWanted}", Color.OrangeRed);
            Log($"  WALLET   {_economy.Wallet:F0} ₸", Color.Gold);
            Log($"  FACTIONS ruin={_factions.GetRep("ruin_syndicate"):+0.#;-0.#;0}  " +
                          $"warden={_factions.GetRep("warden_bloc"):+0.#;-0.#;0}  " +
                          $"kings={_factions.GetRep("hollow_kings"):+0.#;-0.#;0}", Color.Cyan);
            Log($"  MISSIONS active={_missions.ActiveCount}", Color.MediumPurple);
            Log($"  TIME     {_time.TimeOfDay:F2}h  {(_time.IsNight ? "night" : "day")}", Color.SkyBlue);
        }

        private void CmdHeat(string[] args)
        {
            if (args.Length == 0) { Log($"  heat={_heat.CurrentHeat:F2}", Color.OrangeRed); return; }
            switch (args[0])
            {
                case "add"    when args.Length > 1 && float.TryParse(args[1], out float a):
                    _heat.AddHeat(a, "dev_console"); break;
                case "reduce" when args.Length > 1 && float.TryParse(args[1], out float r):
                    _heat.ReduceHeat(r, "dev_console"); break;
                case "clear":
                    _heat.ClearHeat("dev_console"); break;
                case "hide":
                    bool hidden = !_heat.IsWanted; // toggle trick — read field via property
                    _heat.SetPlayerHidden(true);
                    Log("  Player hidden=true (decay timer started)", Color.OrangeRed); break;
                case "safe":
                    _heat.SetInSafehouse(true);
                    Log("  In safehouse — heat will clear on next tick", Color.OrangeRed); break;
                default:
                    Log("  Usage: heat add|reduce|clear|hide|safe [value]", Color.Gray); break;
            }
        }

        private void CmdEcon(string[] args)
        {
            if (args.Length == 0) { Log($"  wallet={_economy.Wallet:F0} ₸", Color.Gold); return; }
            switch (args[0])
            {
                case "add" when args.Length > 1 && float.TryParse(args[1], out float a):
                    _economy.AddCash(a, "dev_console"); break;
                case "spend" when args.Length > 1 && float.TryParse(args[1], out float s):
                    bool ok = _economy.TrySpend(s, "dev_console");
                    if (!ok) Log("  Insufficient funds.", Color.Red);
                    break;
                case "wallet":
                    Log($"  wallet={_economy.Wallet:F0} ₸", Color.Gold); break;
                default:
                    Log("  Usage: econ add|spend|wallet [value]", Color.Gray); break;
            }
        }

        private void CmdFaction(string[] args)
        {
            if (args.Length == 0 || args[0] == "list")
            {
                foreach (var id in new[] { "ruin_syndicate", "warden_bloc", "hollow_kings", "meridian_cartel", "axiom_threat" })
                    Log($"  {id,-22} rep={_factions.GetRep(id):+0.#;-0.#;0}  tier={_factions.GetTier(id)}", Color.Cyan);
                return;
            }
            if (args.Length >= 2 && float.TryParse(args[1], out float delta))
                _factions.ModifyRep(args[0], delta, "dev_console");
            else
                Log("  Usage: fac <faction_id> <delta>  |  fac list", Color.Gray);
        }

        private void CmdMission(string[] args)
        {
            if (args.Length == 0) { Log("  Usage: miss start|obj|list ...", Color.Gray); return; }
            switch (args[0])
            {
                case "start" when args.Length > 1:
                    _missions.ActivateMission(args[1]); break;
                case "obj" when args.Length > 2:
                    if (_missions.TryGetActive(args[1], out var inst))
                        inst.CompleteObjective(args[2]);
                    else
                        Log($"  Mission '{args[1]}' is not active.", Color.Gray);
                    break;
                case "list":
                    Log($"  active={_missions.ActiveCount}", Color.MediumPurple); break;
                default:
                    Log("  Usage: miss start <id>  |  miss obj <mission> <obj>  |  miss list", Color.Gray); break;
            }
        }

        private void CmdSave(string[] args)
        {
            if (args.Length == 0) { Log("  Usage: save <slot>  |  save load <slot>", Color.Gray); return; }
            if (args[0] == "load" && args.Length > 1 && int.TryParse(args[1], out int ls))
            {
                var d = _save.LoadGame(ls);
                if (d != null) Log($"  Loaded slot {ls}: wallet={d.Player?.WalletVtek:F0} ver={d.SaveVersion}", Color.LightGreen);
                else           Log($"  No save in slot {ls}.", Color.Gray);
                return;
            }
            if (int.TryParse(args[0], out int slot))
            {
                var data = new SaveData
                {
                    SaveVersion     = "dev",
                    Timestamp       = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    PlaytimeSeconds = 0,
                    Player          = new PlayerSaveData { WalletVtek = _economy.Wallet, Health = 100f, Inventory = new() },
                    World           = new WorldSaveData
                    {
                        HeatLevel          = _heat.CurrentHeat,
                        TimeOfDay          = _time.TimeOfDay,
                        Weather            = "Clear",
                        FactionReputations = _factions.GetAllReputations(),
                        DistrictControl    = new(),
                        SafehousesOwned    = new(),
                        WorldFlags         = new(),
                        EventCooldowns     = new()
                    },
                    Missions = new MissionSaveData { Completed = new(), Failed = new(), Available = new() }
                };
                bool ok = _save.SaveGame(slot, data);
                Log(ok ? $"  Saved to slot {slot}." : $"  Save failed.", ok ? Color.LightGreen : Color.Red);
            }
            else Log("  Usage: save <0-3>  |  save load <0-3>", Color.Gray);
        }

        private void CmdTime(string[] args)
        {
            if (args.Length == 0) { Log($"  time={_time.TimeOfDay:F2}h", Color.SkyBlue); return; }
            switch (args[0])
            {
                case "speed" when args.Length > 1 && float.TryParse(args[1], out float spd):
                    _time.RealSecondsPerInGameMinute = Math.Max(0.01f, 1f / Math.Max(0.1f, spd));
                    Log($"  Time speed → {spd:F1}x  (RealSecondsPerInGameMinute={_time.RealSecondsPerInGameMinute:F3})", Color.SkyBlue); break;
                case "hour" when args.Length > 1 && float.TryParse(args[1], out float hr):
                    _time.SetTime(hr);
                    Log($"  Time of day → {hr:F1}h", Color.SkyBlue); break;
                default:
                    Log("  Usage: time speed <multiplier>  |  time hour <0-24>", Color.Gray); break;
            }
        }

        private void CmdAI(string[] args)
        {
            if (args.Length == 0 || args[0] != "demo") { Log("  Usage: ai demo", Color.Gray); return; }

            Log("\n  [AI] Running BT composite demo...", Color.Yellow);
            var board = new AIBlackboard();

            // Scenario 1 — under attack → take cover
            board.Set(BB.IsUnderAttack, true);
            board.Set(BB.AlertLevel, 3);

            bool tookCover = false;
            var tree = new BTSelector(
                new BTSequence(
                    new BTCondition(b => b.Get<bool>(BB.IsUnderAttack), "IsUnderAttack"),
                    new BTAction(b => { tookCover = true; return BTStatus.Success; }, "TakeCover")
                ),
                new BTAction(b => BTStatus.Success, "Patrol")
            );

            var r1 = tree.Tick(board);
            Log($"  Scenario 1 (under attack):  result={r1}  tookCover={tookCover}", Color.Yellow);

            // Scenario 2 — not under attack → patrol
            board.Set(BB.IsUnderAttack, false);
            tookCover = false;
            bool patrolled = false;
            var tree2 = new BTSelector(
                new BTSequence(
                    new BTCondition(b => b.Get<bool>(BB.IsUnderAttack)),
                    new BTAction(b => { tookCover = true; return BTStatus.Success; })
                ),
                new BTAction(b => { patrolled = true; return BTStatus.Success; })
            );

            var r2 = tree2.Tick(board);
            Log($"  Scenario 2 (patrol):        result={r2}  patrolled={patrolled}", Color.Yellow);
        }

        // ── HUD ticker ────────────────────────────────────────────────────────────

        private void UpdateHud()
        {
            if (_hud.IsDisposed) return;
            _hud.Text =
                $"HEAT {_heat.CurrentHeat:F2} (L{_heat.HeatLevel})  " +
                $"WALLET {_economy.Wallet:F0} ₸  " +
                $"TIME {_time.TimeOfDay:F2}h ({(_time.IsNight ? "NIGHT" : "DAY")})  " +
                $"MISSIONS {_missions.ActiveCount}";
        }

        // ── Logging ───────────────────────────────────────────────────────────────

        private void Log(string text, Color? color = null)
        {
            if (_output.IsDisposed) return;
            if (_output.InvokeRequired) { _output.Invoke(() => Log(text, color)); return; }
            _output.SelectionStart  = _output.TextLength;
            _output.SelectionLength = 0;
            _output.SelectionColor  = color ?? Color.LightGray;
            _output.AppendText(text + "\n");
            _output.SelectionColor  = _output.ForeColor;
            _output.ScrollToCaret();
        }

        private void PrintBanner()
        {
            Log("  ██████╗██╗████████╗██╗   ██╗   ██╗  ██╗███████╗██████╗  ██████╗ ", Color.OrangeRed);
            Log("  ██╔════╝██║╚══██╔══╝╚██╗ ██╔╝   ╚████╔╝ ██╔════╝██╔══██╗██╔═══██╗", Color.OrangeRed);
            Log("  ██║     ██║   ██║    ╚████╔╝     ╚██╔╝  █████╗  ██████╔╝██║   ██║", Color.OrangeRed);
            Log("  ██║     ██║   ██║     ╚██╔╝       ██║   ██╔══╝  ██╔══██╗██║   ██║", Color.OrangeRed);
            Log("  ╚██████╗██║   ██║      ██║        ██║   ███████╗██║  ██║╚██████╔╝", Color.OrangeRed);
            Log("   ╚═════╝╚═╝   ╚═╝      ╚═╝        ╚═╝   ╚══════╝╚═╝  ╚═╝ ╚═════╝ ", Color.OrangeRed);
            Log("  DEV CONSOLE — all game systems live  |  type 'help' for commands\n", Color.White);
        }
    }
}
