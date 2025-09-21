using Microsoft.Win32;
using OutlastSaveManager.Properties;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO.Compression;
using System.Media;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Timer = System.Windows.Forms.Timer;
using TreeView = System.Windows.Forms.TreeView;

namespace OutlastSaveManager
{
    public partial class Manager : Form
    {
        // --- Windows API für Drag & Minimize ---
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, KeyModifiers fsModifiers, Keys vk);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool IsWow64Process(IntPtr hProcess, out bool lpSystemInfo);
        [DllImport("user32.dll")]
        internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        [DllImport("user32.dll")] private static extern bool ReleaseCapture();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
        [DllImport("user32.dll")] private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;
        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_MINIMIZE = 0xF020;
        private bool enemyToggle = false;
        private System.Threading.Timer timer;
        private help helpInstance;
        private mods modsInstance;
        private mods mod;
        private coffee coffeeInstance;
        private settings settingsInstance;
        private Process watchedProcess;
        private Overlay Overlay = null;
        private Timer overlayTimer;
        private bool isDebugledge;
        private bool gamespeedValue = false;
        private bool hitboxnormalBool = true;

        // -> Insert after your HOTKEY_ID_* constants
        // ---------------- Hotkey-Management ----------------
        private readonly string hotkeyFile = Path.Combine(SaveDir, "hotkeys.json");

        // action -> hotkey string (z. B. "CTRL+SHIFT+S")
        private Dictionary<string, string> currentHotkeys = new Dictionary<string, string>();

        // action -> hotkey id (muss zu deinen vorhandenen switch-cases passen)
        private readonly Dictionary<string, int> actionToId = new Dictionary<string, int>
{
    {"Set Speedrun start", HOTKEY_saveCheckpoint},
    {"Load Speedrun start", HOTKEY_loadCheckpoint},
    {"PlayerInfo", HOTKEY_playerinfo},
    {"Show GameMarkers", HOTKEY_gamemarkers},
    {"ReloadCheckpoint", HOTKEY_reloadcheckpoint},
    {"Reset Collactibles", HOTKEY_removecollactibles},
    {"Reset World", HOTKEY_rsw},
    {"Gamespeed", HOTKEY_gamespeed},
    {"STAT FPS", HOTKEY_StatFPS},
    {"Freecam", HOTKEY_Freecam},
    {"TPFreecam", HOTKEY_TPFreecam},
    {"Gamma10", HOTKEY_Gamma10},
    {"ShowDebug", HOTKEY_ShowDebug},
    {"Show Paths", HOTKEY_Paths},
    {"StatLevels", HOTKEY_StatLevels},
    {"ShowCollision", HOTKEY_ShowCollision},
    {"SetInsaneDifficulty", HOTKEY_SetInsaneDifficulty},
    {"MeshEdges", HOTKEY_meshedges},
    {"Show FOG", HOTKEY_fog},
    {"StaticMeshes", HOTKEY_staticmeshes},
    {"ai", HOTKEY_ai},
    {"Bounds", HOTKEY_bounds},
    {"ShowVolumes", HOTKEY_volumes},
    {"LevelColoration", HOTKEY_levelcoloration},
    {"PostProcess", HOTKEY_postprocess},
    {"ToggleWindow", HOTKEY_Menu},
    {"RemoveCheckpoints", HOTKEY_DeleteExcept},
    {"AddCheckpoints", HOTKEY_AddSelected},
    {"NightVisionToggle", HOTKEY_InfiniteNightVision},
    {"FreezeEnemy", HOTKEY_FreezeEnemy},
    {"NoDamage", HOTKEY_NoDamage},
    {"Rage Quit", HOTKEY_Exit},
    {"Normal Hitbox", HOTKEY_Normal},
    {"Vault Hitbox", HOTKEY_Vault},
    {"Door Hitbox", HOTKEY_Door},
    {"Shimmy Hitbox", HOTKEY_Shimmy},
    {"CheckpointSave1", HOTKEY_CTRL_F1},
    {"CheckpointSave2", HOTKEY_CTRL_F2},
    {"CheckpointSave3", HOTKEY_CTRL_F3},
    {"CheckpointSave4", HOTKEY_CTRL_F4},
    {"CheckpointLoad1", HOTKEY_F1},
    {"CheckpointLoad2", HOTKEY_F2},
    {"CheckpointLoad3", HOTKEY_F3},
    {"CheckpointLoad4", HOTKEY_F4},
    {"PresetLoad1", HOTKEY_LoadPreset_1},
    {"PresetLoad2", HOTKEY_LoadPreset_2},
    {"PresetLoad3", HOTKEY_LoadPreset_3},
    {"PresetLoad4", HOTKEY_LoadPreset_4},
    {"PresetLoad5", HOTKEY_LoadPreset_5},
    {"PresetLoad6", HOTKEY_LoadPreset_6},
    {"PresetLoad7", HOTKEY_LoadPreset_7},
    {"PresetLoad8", HOTKEY_LoadPreset_8},
    {"PresetLoad9", HOTKEY_LoadPreset_9},
    {"PresetSave1", HOTKEY_SavePreset_1},
    {"PresetSave2", HOTKEY_SavePreset_2},
    {"PresetSave3", HOTKEY_SavePreset_3},
    {"PresetSave4", HOTKEY_SavePreset_4},
    {"PresetSave5", HOTKEY_SavePreset_5},
    {"PresetSave6", HOTKEY_SavePreset_6},
    {"PresetSave7", HOTKEY_SavePreset_7},
    {"PresetSave8", HOTKEY_SavePreset_8},
    {"PresetSave9", HOTKEY_SavePreset_9}
};

        // track which ids are currently registered so we can unregister them later
        private readonly HashSet<int> registeredHotkeyIds = new HashSet<int>();

        public const int HOTKEY_Menu = 9000;
        public const int HOTKEY_DeleteExcept = 9001;
        public const int HOTKEY_AddSelected = 9002;
        public const int HOTKEY_LoadPreset_1 = 9003;
        public const int HOTKEY_LoadPreset_2 = 9004;
        public const int HOTKEY_LoadPreset_3 = 9005;
        public const int HOTKEY_LoadPreset_4 = 9006;
        public const int HOTKEY_LoadPreset_5 = 9007;
        public const int HOTKEY_LoadPreset_6 = 9017;
        public const int HOTKEY_LoadPreset_7 = 9018;
        public const int HOTKEY_LoadPreset_8 = 9019;
        public const int HOTKEY_LoadPreset_9 = 9020;
        public const int HOTKEY_SavePreset_1 = 9008;
        public const int HOTKEY_SavePreset_2 = 9009;
        public const int HOTKEY_SavePreset_3 = 9010;
        public const int HOTKEY_SavePreset_4 = 9011;
        public const int HOTKEY_SavePreset_5 = 9012;
        public const int HOTKEY_SavePreset_6 = 9013;
        public const int HOTKEY_SavePreset_7 = 9014;
        public const int HOTKEY_SavePreset_8 = 9015;
        public const int HOTKEY_SavePreset_9 = 9016;
        public const int HOTKEY_InfiniteNightVision = 9021;
        public const int HOTKEY_FreezeEnemy = 9022;
        public const int HOTKEY_NoDamage = 9023;
        public const int HOTKEY_Freecam = 9024;
        public const int HOTKEY_StatFPS = 9025;
        public const int HOTKEY_Gamma10 = 9026;
        public const int HOTKEY_StatLevels = 9027;
        public const int HOTKEY_ShowCollision = 9028;
        public const int HOTKEY_SetInsaneDifficulty = 9029;
        public const int HOTKEY_TPFreecam = 9030;
        public const int HOTKEY_volumes = 9031;
        public const int HOTKEY_levelcoloration = 9033;
        public const int HOTKEY_postprocess = 9034;
        public const int HOTKEY_CTRL_F1 = 9035;
        public const int HOTKEY_CTRL_F2 = 9036;
        public const int HOTKEY_CTRL_F3 = 9037;
        public const int HOTKEY_CTRL_F4 = 9038;
        public const int HOTKEY_F1 = 9039;
        public const int HOTKEY_F2 = 9040;
        public const int HOTKEY_F3 = 9041;
        public const int HOTKEY_F4  = 9042;
        public const int HOTKEY_Exit = 9043;
        public const int HOTKEY_ShowDebug = 9044;
        public const int HOTKEY_Paths = 9045;
        public const int HOTKEY_playerinfo = 9046;
        public const int HOTKEY_gamemarkers = 9047;
        public const int HOTKEY_meshedges= 9048;
        public const int HOTKEY_bounds= 9049;
        public const int HOTKEY_staticmeshes= 9050;
        public const int HOTKEY_ai= 9051;
        public const int HOTKEY_gamespeed = 9052;
        public const int HOTKEY_reloadcheckpoint = 9053;
        public const int HOTKEY_fog = 9054;
        public const int HOTKEY_removedocuments = 9055;
        public const int HOTKEY_removerecordings = 9056;
        public const int HOTKEY_removecollactibles = 9057;
        public const int HOTKEY_rsw = 9058;
        public const int HOTKEY_loadCheckpoint = 9059;
        public const int HOTKEY_Normal = 9060;
        public const int HOTKEY_Vault = 9061;
        public const int HOTKEY_Door = 9062;
        public const int HOTKEY_Shimmy = 9063;
        public const int HOTKEY_saveCheckpoint = 9064;


        private bool debugActive = false;
        private bool isGamma10 = false;
        private bool isFreecam = false;
        private Dictionary<string, Overlay> activeOverlays = new Dictionary<string, Overlay>();
        private bool isNoDamage = false;
        private readonly string readOnlyListFile = Path.Combine(SaveDir, "readonly_list.txt");
        private List<string> readOnlyPaths = new List<string>();
        private CancellationTokenSource duplicationFixCts;
        private double playtimeHours = 0; // Spielzeit in Stunden (dezimal)
        private static readonly string SaveDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OutlastSaveManager");
        private string playtimeFile = Path.Combine(SaveDir, "playtime.txt");
        private static string startupPath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
        private string commandsCFG = Path.Combine(startupPath,"Binaries", "commands.cfg");
        private string presetFile = Path.Combine(SaveDir, "presets.json"); private string ordnerPfad = Path.Combine(Application.StartupPath, "Boot");
        private System.Threading.Timer playtimeTimer;
        private System.Threading.Timer duplicationFixTimer;
        //List<List<string>> presets = new List<List<string>> { new(), new(), new(), new(), new() };

        List<List<string>> presets = new List<List<string>>
        {
            new(), new(), new(), new(), new(), new(), new(), new(), new()
        };
        private bool nightVisionActive = false;


        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int X, int Y, int cx, int cy, uint uFlags);

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_SHOWWINDOW = 0x0040;

        // macht die laufende Manager-Instanz global erreichbar
        public static Manager Instance { get; private set; }

        // öffentliche read-only Views auf die Daten
        public IReadOnlyDictionary<string, string> PublicCurrentHotkeys => currentHotkeys;
        public IReadOnlyDictionary<string, int> PublicActionToId => actionToId;




        // --- Fade-In / Fade-Out ---
        private Timer fadeTimer;
        private bool fadingOut = false;
        private bool fadingIn = false;
        private double fadeStep = 0.05;
        private string nopping = "unnop";
        // --- Save-Folder ---
        private string gameSaveFolder = "";
        private string projectSaveFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SaveData");

        // --- TreeView Check-State Management ---
        private bool _updatingChecks = false;
        private Timer _focusTimer; // Feld speichern

        //------------------------------------------------------------------------------------------------------
        //push and upload on github,update version.txt in github
        //------------------------------------------------------------------------------------------------------

        private static string LocalVersion = "2.3.0";
        private void changelogs()
        {
            AddInfoLog("Most of the new features are only useable via Hotkeys!\nIf you got more ideas or any bug, just text me on discord\nRead the README.md for further information\n");
        }
        public Manager()
        {
            InitializeComponent();
            AttachToProcess("OLGame");
            CheckAndUpdate();
            LoadHotkeysFromFile();
            RegisterAllHotkeys();
            Instance = this;

            this.Icon = Resources.logo;
            this.Paint += (s, e) => e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;


            try
            {
                LoadProjectSaves();
                btnLoadLiveSaves_Click(null, null);
            }
            catch (Exception ex)
            {
                AddErrorLog("Error while loading the project saves:\n" + ex.Message);
            }

            EnsureSaveFolderAccess();
            ApplyRoundedCorners(10);

            Program.communicator.MessageReceived += MessageFired;

        }

        private void MessageFired(string msg)
        {
            switch (msg)
            {
                

                default:
                    break;
            }
        }

        // -------------------------
        // Form Load & UI Setup
        // -------------------------
        private void Manager_Load(object sender, EventArgs e)
        {
            timer = new System.Threading.Timer(_ => updateNeeded(), null, Timeout.Infinite, Timeout.Infinite);
            fadeTimer = new System.Windows.Forms.Timer();
            fadeTimer.Interval = 15; // ~60 FPS
            fadeTimer.Tick += FadeTimer_Tick;


            panelLive.Paint += panelLive_Paint;
            panelProject.Paint += panelProject_Paint;
            //panelBtnAdds.Paint += panelBtnAdd_Paint;
            //panelBtnRemoveRO.Paint += panelBtnAdd_Paint;
            //panelBtnSetROS.Paint += panelBtnAdd_Paint;
            btnRemoveChk.Paint += panelBtnRemoveChk_Paint;
            btnRemoveRO.Paint += panelBtnRemoveChk_Paint;
            //panelBtnUL.Paint += panelBtnAdd_Paint;
            panelBar.Paint += panelBar_Paint;


            // Fenster-Grundsetup
            FormBorderStyle = FormBorderStyle.None;
            MinimizeBox = true;
            BackColor = Color.FromArgb(0x0A, 0x0A, 0x0A);
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(900, 615);

            // Runde Ecken anwenden
            ApplyRoundedCorners(20);

            // Titlebar
            var titleBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 34,
                BackColor = Color.FromArgb(0x0A, 0x0A, 0x0A)
            };
            titleBar.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
                }
            };
            Controls.Add(titleBar);
            // Setze das Icon korrekt im Konstruktor des PictureBox-Objekts
            var picLogo = new PictureBox
            {
                //Image = Resources., // Korrekt: Property "Image" setzen, nicht "picLogo.Image = ..."
                Image = Properties.Resources.Frame_1, // Dein Icon hier
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(25, 25),
                Location = new Point(10, 9), // Abstand links/oben
                BackColor = Color.Transparent
            };
            titleBar.Controls.Add(picLogo);

            // Name hinzufügen
            var lblAppName = new Label
            {
                Text = "SaveManager", // Dein Name
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 13, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(picLogo.Right + 8, 7), // rechts vom Icon
                BackColor = Color.Transparent
            };
            titleBar.Controls.Add(lblAppName);
            // Minimize (–)
            var btnMin = new System.Windows.Forms.Button
            {
                Text = "–",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                Dock = DockStyle.Right,
                Width = 50,
                Height = titleBar.Height,
                Cursor = Cursors.Hand
            };
            btnMin.FlatAppearance.BorderSize = 0;
            btnMin.MouseEnter += (s, e) => btnMin.BackColor = Color.FromArgb(10, 20, 20, 20);
            btnMin.MouseLeave += (s, e) => btnMin.BackColor = Color.Transparent;
            btnMin.Click += (s, e) => FadeOutAndMinimize();
            titleBar.Controls.Add(btnMin);

            // Close (X)
            var btnClose = new System.Windows.Forms.Button
            {
                Text = "X",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                Dock = DockStyle.Right,
                Width = 50,
                Height = titleBar.Height,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.MouseEnter += (s, e) => btnClose.BackColor = Color.FromArgb(128, 232, 17, 35);
            btnClose.MouseLeave += (s, e) => btnClose.BackColor = Color.Transparent;
            btnClose.Click += (s, e) => Close();
            titleBar.Controls.Add(btnClose);

            // Paint-Effekt für AntiAlias (glattere Kanten)
            this.Paint += (s, e) => e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            treeViewLive.BorderStyle = BorderStyle.None;
            treeViewProject.BorderStyle = BorderStyle.None;
            treeViewProject.Paint += treeViewProject_Paint;

            treeViewProject.BackColor = Color.FromArgb(10, 10, 10); // oder Color.FromArgb(0x0A,0x0A,0x0A)
            treeViewProject.MouseWheel += (s, e) =>
            {
                int scrollAmount = e.Delta > 0 ? 20 : -20; // Pixel pro Scroll
                Rectangle rect = treeViewProject.DisplayRectangle;
                rect.Y += scrollAmount;

                // Grenzen prüfen
                if (rect.Y > 0) rect.Y = 0;
                int minY = Math.Min(treeViewProject.Height - treeViewProject.DisplayRectangle.Height, 0);
                if (rect.Y < minY) rect.Y = minY;

                treeViewProject.Invalidate();
            };

            StartTimer();
            consoleArea.AppendText("");
            bootminized();

            if (prop.Default.duplicationFix)
            {
                duplicationFix();
            }

            if (!IsProcessRunning("OLGame"))//change on realse to OLGame
            {
                MessageBox.Show("couldnt find process OLGame.exe\nDid you really start Outlast?"); //delete on realse
                Application.Exit();
                //Process.GetCurrentProcess().Kill();
            }
            getVersion();

            if (prop.Default.duplicationFix)
            {
                StartDuplicationFixLoop();
            }
            LoadPlaytime();
            StartPlaytimeTimer();
            labelHours.Text = playtimeHours.ToString("F1"); // Anzeige beim Start

            this.Activated += Manager_Activated;     // Fenster bekommt Fokus
            this.Deactivate += Manager_Deactivate;

            if (File.Exists(presetFile))
            {
                var json = File.ReadAllText(presetFile);
                presets = JsonSerializer.Deserialize<List<List<string>>>(json) ?? presets;
            }

            changelogs();
            bootFolderCheck();
            if (prop.Default.sh && prop.Default.externalModPackage)
            {
                UnregisterHotKey(this.Handle, HOTKEY_CTRL_F1);
                UnregisterHotKey(this.Handle, HOTKEY_CTRL_F2);
                UnregisterHotKey(this.Handle, HOTKEY_CTRL_F3);
                UnregisterHotKey(this.Handle, HOTKEY_CTRL_F4);
                UnregisterHotKey(this.Handle, HOTKEY_F1);
                UnregisterHotKey(this.Handle, HOTKEY_F2);
                UnregisterHotKey(this.Handle, HOTKEY_F3);
                UnregisterHotKey(this.Handle, HOTKEY_F4);
                speedrunhelper();

            }
            StartOverlayTimer();
            unloadInfo();
            ManageHotkeysFocus();
        }
        // in Manager.cs
        public void ManageHotkeysFocus()
        {
            bool hotkeysRegistered = false;
            string targetProcess = "OLGame"; // Outlast.exe ohne .exe

            _focusTimer = new Timer();
            _focusTimer.Interval = 1000; // alle 1000ms prüfen
            _focusTimer.Tick += (s, e) =>
            {
                IntPtr hwnd = GetForegroundWindow();
                GetWindowThreadProcessId(hwnd, out int pid);

                bool focused = false;
                try
                {
                    Process proc = Process.GetProcessById((int)pid);
                    focused = proc.ProcessName.Equals(targetProcess, StringComparison.OrdinalIgnoreCase);
                    
                    if (!focused && Form.ActiveForm != null)
                    {
                        focused = Form.ActiveForm.Handle == hwnd;
                    }
                }
                catch { focused = false; }

                if (focused && !hotkeysRegistered)
                {
                    if (prop.Default.speedrun)
                    {
                        DisableHotkeys();
                    }
                    else
                    {
                        RegisterAllHotkeys();
                    }
                    hotkeysRegistered = true;
                }
                else if (!focused && hotkeysRegistered)
                {
                    UnregisterAllHotkeys();
                    hotkeysRegistered = false;
                }
            };
            _focusTimer.Start();
        }

        private void externalwarning()
        {
            if (prop.Default.externalModPackage)
            { 
                    MessageBox.Show("This option is only available if you turn OFF external mod packages in the settings");
            }
        }
        public void OverrideHotkeysFrom(Dictionary<string, string> newHotkeys)
        {
            currentHotkeys = new Dictionary<string, string>(newHotkeys ?? new Dictionary<string, string>());
            SaveHotkeysToFile();
            RegisterAllHotkeys();
        }

        private void LoadHotkeysFromFile()
        {
            currentHotkeys.Clear();
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(hotkeyFile));
                if (File.Exists(hotkeyFile))
                {
                    string json = File.ReadAllText(hotkeyFile);
                    var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                    if (dict != null)
                    {
                        currentHotkeys = dict;
                    }
                }
                // If file doesn't exist, leave currentHotkeys empty (no defaults).
            }
            catch (Exception ex)
            {
                AddErrorLog("Could not load hotkeys: " + ex.Message);
                currentHotkeys = new Dictionary<string, string>();
            }
        }
        private void OpenHotkeyManager()
        {
            var actions = actionToId.Keys.ToList(); // <- sehr wichtig, das ist die Liste der Aktionen
            using (HotkeyManager hkManager = new HotkeyManager(actions, currentHotkeys))
            {
                if (hkManager.ShowDialog() == DialogResult.OK)
                {
                    currentHotkeys = new Dictionary<string, string>(hkManager.UserHotkeys ?? new Dictionary<string, string>());
                    SaveHotkeysToFile();
                    RegisterAllHotkeys();
                }
            }
        }


        private void SaveHotkeysToFile()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(hotkeyFile));
                string json = JsonSerializer.Serialize(currentHotkeys, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(hotkeyFile, json);
            }
            catch (Exception ex)
            {
                AddErrorLog("Could not save hotkeys: " + ex.Message);
            }
        }

        private void UnregisterAllHotkeys()
        {
            try
            {
                foreach (int id in registeredHotkeyIds.ToList())
                {
                    try { UnregisterHotKey(this.Handle, id); } catch { }
                }
                registeredHotkeyIds.Clear();
            }
            catch { }
        }

        private void RegisterAllHotkeys()
        {
            UnregisterAllHotkeys();

            foreach (var kv in currentHotkeys)
            {
                string action = kv.Key;
                string hk = kv.Value?.Trim();

                if (string.IsNullOrEmpty(hk)) continue; // skip empty entries (no default!)

                if (!actionToId.TryGetValue(action, out int id)) continue; // unknown action -> skip

                var (mods, key) = ParseHotkey(hk);
                if (key == Keys.None) continue; // invalid parse -> skip

                if (prop.Default.externalModPackage && id >= 9060 && id <= 9063)
                    continue;

                try
                {
                    bool ok = RegisterHotKey(this.Handle, id, mods, key);
                    if (ok)
                        registeredHotkeyIds.Add(id);
                   
                        
                }
                catch (Exception ex)
                {
                    AddErrorLog($"RegisterHotKey failed for {action}: {ex.Message}");
                }
            }
        }



        
        /// <summary>
        /// Parses "CTRL+SHIFT+S" style string into KeyModifiers + Keys.
        /// falls das enum KeyModifiers in deinem Projekt andere Werte hat, passt die Zahlen hier an.
        /// </summary>
        private (KeyModifiers, Keys) ParseHotkey(string hotkey)
        {
            KeyModifiers mods = 0;
            Keys key = Keys.None;
            try
            {
                var parts = hotkey.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries)
                                  .Select(p => p.Trim().ToUpperInvariant()).ToList();

                foreach (var p in parts)
                {
                    if (p == "CTRL" || p == "CONTROL")
                    {
                        mods |= (KeyModifiers)0x0002; // MOD_CONTROL
                    }
                    else if (p == "ALT" || p == "MENU")
                    {
                        mods |= (KeyModifiers)0x0001; // MOD_ALT
                    }
                    else if (p == "SHIFT")
                    {
                        mods |= (KeyModifiers)0x0004; // MOD_SHIFT
                    }
                    else if (p == "WIN" || p == "WINDOWS")
                    {
                        mods |= (KeyModifiers)0x0008; // MOD_WIN (falls genutzt)
                    }
                    else
                    {
                        // try parse key name to Keys
                        if (Enum.TryParse<Keys>(p, true, out Keys parsed))
                        {
                            key = parsed;
                        }
                        else
                        {
                            // handle common names like "PLUS", "MINUS" or "OemPlus" if needed
                            // fallback: ignore
                        }
                    }
                }
            }
            catch { }

            return (mods, key);
        }


        private void unloadInfo()
        {
            if (!prop.Default.info)
            {
                info1.Visible = false;
                info2.Visible = false;
                info3.Visible = false;
                info4.Visible = false;
                info5.Visible = false;
                info6.Visible = false;
                info7.Visible = false;
                info8.Visible = false;
                info9.Visible = false;
                info10.Visible = false;
                info11.Visible = false;
                info12.Visible = false;
                info13.Visible = false;
                //info14.Visible = false;
                //info15.Visible = false;
            }
        }

  
        private void StartOverlayTimer()
        {
            overlayTimer = new Timer();
            overlayTimer.Interval = 300; // alle 750 ms prüfen
            overlayTimer.Tick += OverlayTimer_Tick;
            overlayTimer.Start();
        }

        private void OverlayTimer_Tick(object sender, EventArgs e)
        {
            bool outlastIsActive = false;
            IntPtr foreground = GetForegroundWindow();
            if (foreground != IntPtr.Zero)
            {
                try
                {
                    GetWindowThreadProcessId(foreground, out int pid); // int
                    var proc = Process.GetProcessById(pid);

                    if (proc.ProcessName.ToLower().Contains("olgame"))
                    {
                        outlastIsActive = true;
                    }
                }
                catch { }
            }

            foreach (var overlay in activeOverlays.Values)
            {
                if (!overlay.IsDisposed)
                    overlay.Visible = prop.Default.borderless && outlastIsActive;
            }
        }

        public void speedrunhelper()
        {
            if (prop.Default.externalModPackage)
            {
                UnregisterHotKey(this.Handle, HOTKEY_CTRL_F1);
                UnregisterHotKey(this.Handle, HOTKEY_CTRL_F2);
                UnregisterHotKey(this.Handle, HOTKEY_CTRL_F3);
                UnregisterHotKey(this.Handle, HOTKEY_CTRL_F4);
                UnregisterHotKey(this.Handle, HOTKEY_F1);
                UnregisterHotKey(this.Handle, HOTKEY_F2);
                UnregisterHotKey(this.Handle, HOTKEY_F3);
                UnregisterHotKey(this.Handle, HOTKEY_F4);

                if (checkBit64())
                {
                    ProcessStartInfo psi = new ProcessStartInfo()
                    {
                        FileName = Path.Combine(Directory.GetCurrentDirectory(), "SaveManager-Mods", "checkpointHandler64.exe"),
                        UseShellExecute = false, // false, damit man z.B. Standardausgabe lesen kann
                        CreateNoWindow = true   // true, wenn kein Fenster aufgehen soll
                    };

                    Process process = Process.Start(psi);
                }
                else
                {
                    ProcessStartInfo psi = new ProcessStartInfo()
                    {
                        FileName = Path.Combine(Directory.GetCurrentDirectory(), "SaveManager-Mods", "checkpointHandler32.exe"),
                        UseShellExecute = false, // false, damit man z.B. Standardausgabe lesen kann
                        CreateNoWindow = true   // true, wenn kein Fenster aufgehen soll
                    };

                    Process process = Process.Start(psi);
                }
            }
            else
            {
                RegisterHotKey(this.Handle, HOTKEY_F1, KeyModifiers.Shift, Keys.F1);
                RegisterHotKey(this.Handle, HOTKEY_F2, KeyModifiers.Shift, Keys.F2);
                RegisterHotKey(this.Handle, HOTKEY_F3, KeyModifiers.Shift, Keys.F3);
                RegisterHotKey(this.Handle, HOTKEY_F4, KeyModifiers.Shift, Keys.F4);
                RegisterHotKey(this.Handle, HOTKEY_CTRL_F1, KeyModifiers.Shift, Keys.F1);
                RegisterHotKey(this.Handle, HOTKEY_CTRL_F2, KeyModifiers.Shift, Keys.F2);
                RegisterHotKey(this.Handle, HOTKEY_CTRL_F3, KeyModifiers.Shift, Keys.F3);
                RegisterHotKey(this.Handle, HOTKEY_CTRL_F4, KeyModifiers.Shift, Keys.F4);
            }
        }

        private void bootFolderCheck()
        {
            if (!Directory.Exists(ordnerPfad))
            {
                Console.WriteLine("Boot-Folder not found: " + ordnerPfad);
                Directory.CreateDirectory(ordnerPfad);
            }
        }

        private void bootFolders()
        {
            if (!Directory.Exists(ordnerPfad))
            {
                Console.WriteLine("Ordner nicht gefunden: " + ordnerPfad);
                Directory.CreateDirectory(ordnerPfad);
                return;
            }

            string[] dateien = Directory.GetFiles(ordnerPfad, "*.*");
            

            foreach (string datei in dateien)
            {
                try
                {
                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = datei,
                        UseShellExecute = true // wichtig, damit auch Nicht-EXE (z.B. TXT, PDF, MP3) mit Standardprogramm geöffnet werden
                    });

                    Console.WriteLine("Gestartet: " + datei);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fehler bei " + datei + ": " + ex.Message);
                }
            }
        }

        private void Manager_Activated(object sender, EventArgs e)
        {
            duplicationFixTimer?.Change(Timeout.Infinite, Timeout.Infinite);
        }

        // Starten, wenn Fenster nicht Fokus hat
        private void Manager_Deactivate(object sender, EventArgs e)
        {
            duplicationFixTimer?.Change(TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }
        private void SavePlaytime()
        {
            File.WriteAllText(playtimeFile, playtimeHours.ToString("F1"));
        }

        private void StartPlaytimeTimer()
        {
            playtimeTimer = new System.Threading.Timer(_ =>
            {
                playtimeHours += 1.0 / 60.0; // 1 Minute = 1/60 Stunde
                SavePlaytime();

                // UI aktualisieren
                this.BeginInvoke((Action)(() =>
                {
                    labelHours.Text = playtimeHours.ToString("F1");
                }));

            }, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }
        private void LoadPlaytime()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(playtimeFile));
            if (File.Exists(playtimeFile))
            {
                string content = File.ReadAllText(playtimeFile);
                if (double.TryParse(content, out double hours))
                    playtimeHours = hours;
            }
            else
            {
                playtimeHours = 0; // first time
                File.WriteAllText(playtimeFile, playtimeHours.ToString("F1"));
            }

            labelHours.Text = playtimeHours.ToString("F1");
        }
        /*private void StartDuplicationFixLoop()
        {
            duplicationFixTimer = new System.Threading.Timer(_ =>
            {
                duplicationFix(); // deine existierende Methode
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(3)); // alle 5 Sekunden
        }*/

        private void StartDuplicationFixLoop()
        {
            duplicationFixTimer = new System.Threading.Timer(_ =>
            {
                this.BeginInvoke((Action)(() =>
                {
                    duplicationFix(); // läuft jetzt auf dem UI-Thread
                }));
            }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(650));
        }


        private void StopDuplicationFixLoop()
        {
            duplicationFixCts?.Cancel();
            duplicationFixCts = null;
        }


        private void getVersion()
        {
            versionLabel.Text = $"Version: {LocalVersion}";
        }

        public void CheckAndUpdate()
        {
            try
            {
                string url = "https://raw.githubusercontent.com/Trofline/Tools/main/version.txt";

                // Synchronously download the version file
                string versionFile;
                using (WebClient wc = new WebClient())
                {
                    versionFile = wc.DownloadString(url);
                }

                string[] lines = versionFile.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length < 3)
                    return; // version.txt nicht korrekt

                string latestVersion = lines[0].Trim();
                string updateUrl = lines[1].Trim();
                string updaterUrl = lines[2].Trim();

                if (!LocalVersion.Equals(latestVersion, StringComparison.OrdinalIgnoreCase))
                {
                    if (latestVersion.Equals("0.0.0",StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }
                    // Update gefunden
                    DialogResult result = MessageBox.Show($"Update available ({latestVersion}). Upgrade now?",
                                                          "Update",
                                                          MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        DownloadAndStartUpdater(updateUrl, updaterUrl);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update-Check could not be done: " + ex.Message);
            }
        }

        private void DownloadAndStartUpdater(string url, string updater)
        {
            //UPDATE

            displayUpdate();

            string exePath = Path.Combine(Directory.GetCurrentDirectory(), "OutlastSaveManager.exe");
            string exeDir = Path.GetDirectoryName(exePath);
            string zipPath = Path.Combine(exeDir, "update.zip");
            string updaterPath = Path.Combine(exeDir, "UpdaterSaveManager.zip");

            using (WebClient wc = new WebClient())
            {
                wc.DownloadFile(url, zipPath);
                wc.DownloadFile(updater, updaterPath);
            }


            string updaterExe = Path.Combine(exeDir, "UpdaterSaveManager.exe");
            if (File.Exists(updaterExe))
            {
                File.Delete(updaterExe);
            }

            ZipFile.ExtractToDirectory(updaterPath, exeDir);


            File.Delete(updaterPath);

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = updaterExe,
                Arguments = $"\"{zipPath}\" \"{exePath}\"",
                UseShellExecute = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal
            });

            Application.Exit();
        }

        private void displayUpdate()
        {
            btnAdd.Enabled = false;
            btnSetRO.Enabled = false;
            btnRemoveChk.Enabled = false;
            btnRemoveRO.Enabled = false;
            treeViewProject.Enabled = false;
            treeViewLive.Enabled = false;
            versionLabel.Text = "Version Update";
            help.Visible = false;
            linkLabel1.Visible = false;
            linkLabel2.Visible = false;
            linkLabel3.Visible = false;
            linkLabel4.Visible = false;
            AddWarningLog("SaveManager is updating, please wait...");

        }

        public bool IsProcessRunning(string namePart)
        {
            foreach (Process p in Process.GetProcesses())
            {
                try
                {
                    if (p.ProcessName.IndexOf(namePart, System.StringComparison.OrdinalIgnoreCase) >= 0)
                        return true;
                }
                catch { } // falls auf manche Prozesse kein Zugriff möglich ist
            }
            return false;
        }


        private void duplicationFix()
        {
            updateNeeded();
            var nodes = GetAllCheckpoints(treeViewLive);

            // gruppieren nach Text
            var duplicates = nodes.GroupBy(n => n.Text)
                                  .Where(g => g.Count() > 1);

            foreach (var group in duplicates)
            {
                // alle außer dem ersten löschen, sodass nur eines übrig bleibt
                var toRemove = group.Skip(1).ToList();

                foreach (var node in toRemove)
                {
                    try
                    {
                        // Datei löschen, wenn vorhanden
                        if (node.Tag is string filePath && File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }

                        // Node aus TreeView entfernen
                        node.Remove();
                    }
                    catch (Exception ex)
                    {
                        AddErrorLog($"Error deleting {node.Text}: {ex.Message}");
                    }
                }
            }
        }

        private void bootminized()
        {
            if (prop.Default.minimizedBoot)
            {
                FadeOutAndMinimize();
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            const int WM_HOTKEY = 0x0312;
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();



                switch (id)
                {
                    case HOTKEY_Menu:
                        if (this.WindowState == FormWindowState.Minimized)
                        {
                            this.WindowState = FormWindowState.Normal;
                            this.Show();
                            this.Activate();
                            AddLog("Changed WindowState");
                            ToggleMods();
                        }
                        else
                        {
                            FadeOutAndMinimize();
                            AddLog("Changed WindowState");
                        }
                        break;

                    case HOTKEY_DeleteExcept:
                        this.BeginInvoke((Action)(() => deleteEveryCheckpointExceptReadOnly()));
                        AddLog("Removed checkpoints");
                        break;

                    case HOTKEY_AddSelected:
                        this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                        AddLog("Added checkpoints");
                        break;

                    case HOTKEY_LoadPreset_1:
                        this.BeginInvoke((Action)(() => deleteEveryCheckpointExceptReadOnly()));
                        this.BeginInvoke((Action)(() => ApplyPreset(treeViewProject, 0)));
                        this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                        AddLog("Loaded Preset 1");
                        break;

                    case HOTKEY_LoadPreset_2:
                        this.BeginInvoke((Action)(() => deleteEveryCheckpointExceptReadOnly()));
                        this.BeginInvoke((Action)(() => ApplyPreset(treeViewProject, 1)));
                        this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                        AddLog("Loaded Preset 2");
                        break;

                    case HOTKEY_LoadPreset_3:
                        this.BeginInvoke((Action)(() => deleteEveryCheckpointExceptReadOnly()));
                        this.BeginInvoke((Action)(() => ApplyPreset(treeViewProject, 2)));
                        this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                        AddLog("Loaded Preset 3");
                        break;

                    case HOTKEY_LoadPreset_4:
                        this.BeginInvoke((Action)(() => deleteEveryCheckpointExceptReadOnly()));
                        this.BeginInvoke((Action)(() => ApplyPreset(treeViewProject, 3)));
                        this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                        AddLog("Loaded Preset 4");
                        break;

                    case HOTKEY_LoadPreset_5:
                        this.BeginInvoke((Action)(() => deleteEveryCheckpointExceptReadOnly()));
                        this.BeginInvoke((Action)(() => ApplyPreset(treeViewProject, 4)));
                        this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                        AddLog("Loaded Preset 5");
                        break;

                    case HOTKEY_LoadPreset_6:
                        this.BeginInvoke((Action)(() => deleteEveryCheckpointExceptReadOnly()));
                        this.BeginInvoke((Action)(() => ApplyPreset(treeViewProject, 5)));
                        this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                        AddLog("Loaded Preset 6");
                        break;

                    case HOTKEY_LoadPreset_7:
                        this.BeginInvoke((Action)(() => deleteEveryCheckpointExceptReadOnly()));
                        this.BeginInvoke((Action)(() => ApplyPreset(treeViewProject, 6)));
                        this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                        AddLog("Loaded Preset 7");
                        break;

                    case HOTKEY_LoadPreset_8:
                        this.BeginInvoke((Action)(() => deleteEveryCheckpointExceptReadOnly()));
                        this.BeginInvoke((Action)(() => ApplyPreset(treeViewProject, 7)));
                        this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                        AddLog("Loaded Preset 8");
                        break;

                    case HOTKEY_LoadPreset_9:
                        this.BeginInvoke((Action)(() => deleteEveryCheckpointExceptReadOnly()));
                        this.BeginInvoke((Action)(() => ApplyPreset(treeViewProject, 8)));
                        this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                        AddLog("Loaded Preset 9");
                        break;

                    case HOTKEY_SavePreset_1:
                        this.BeginInvoke((Action)(() => SaveCurrentTreeAsPreset(treeViewProject, 0)));
                        AddLog("Saved Preset 1");
                        break;

                    case HOTKEY_SavePreset_2:
                        this.BeginInvoke((Action)(() => SaveCurrentTreeAsPreset(treeViewProject, 1)));
                        AddLog("Saved Preset 2");
                        break;

                    case HOTKEY_SavePreset_3:
                        this.BeginInvoke((Action)(() => SaveCurrentTreeAsPreset(treeViewProject, 2)));
                        AddLog("Saved Preset 3");
                        break;

                    case HOTKEY_SavePreset_4:
                        this.BeginInvoke((Action)(() => SaveCurrentTreeAsPreset(treeViewProject, 3)));
                        AddLog("Saved Preset 4");
                        break;

                    case HOTKEY_SavePreset_5:
                        this.BeginInvoke((Action)(() => SaveCurrentTreeAsPreset(treeViewProject, 4)));
                        AddLog("Saved Preset 5");
                        break;

                    case HOTKEY_SavePreset_6:
                        this.BeginInvoke((Action)(() => SaveCurrentTreeAsPreset(treeViewProject, 5)));
                        AddLog("Saved Preset 6");
                        break;

                    case HOTKEY_SavePreset_7:
                        this.BeginInvoke((Action)(() => SaveCurrentTreeAsPreset(treeViewProject, 6)));
                        AddLog("Saved Preset 7");
                        break;

                    case HOTKEY_SavePreset_8:
                        this.BeginInvoke((Action)(() => SaveCurrentTreeAsPreset(treeViewProject, 7)));
                        AddLog("Saved Preset 8");
                        break;

                    case HOTKEY_SavePreset_9:
                        this.BeginInvoke((Action)(() => SaveCurrentTreeAsPreset(treeViewProject, 8)));
                        AddLog("Saved Preset 9");
                        break;

                    case HOTKEY_InfiniteNightVision:
                        nightvision();
                        break;

                    case HOTKEY_FreezeEnemy:
                        freezeEnemy();
                        break;

                    case HOTKEY_NoDamage:
                        noDamage();
                        break;
                    case HOTKEY_Freecam:
                        isFreecam = !isFreecam;

                        if (isFreecam)
                        {
                            File.WriteAllText(commandsCFG, "Camera Freecam");
                        }
                        else
                        {
                            File.WriteAllText(commandsCFG, "Camera Default");
                        }
                        SimulateKey();
                        break;
                    case HOTKEY_StatFPS:
                        File.WriteAllText(commandsCFG, "Stat FPS");
                        SimulateKey();
                        break;
                    case HOTKEY_ShowCollision:
                        File.WriteAllText(commandsCFG, "Show COLLISION");
                        SimulateKey();
                        break;
                    case HOTKEY_Gamma10:
                        isGamma10 = !isGamma10;
                        if (isGamma10)
                        {
                            File.WriteAllText(commandsCFG, "Gamma 10");
                        }
                        else
                        {
                            File.WriteAllText(commandsCFG, "Gamma 3");//TODO CHECK IF WORKING
                        }
                        SimulateKey();
                        break;
                    case HOTKEY_StatLevels:
                        File.WriteAllText(commandsCFG, "Stat LEVELS");
                        SimulateKey();
                        break;
                    case HOTKEY_SetInsaneDifficulty:
                        File.WriteAllText(commandsCFG, "set OLGame DifficultyMode EDM_Insane");
                        SimulateKey();
                        break;
                    case HOTKEY_volumes:
                        File.WriteAllText(commandsCFG, "Show VOLUMES");
                        SimulateKey();
                        break;
                    case HOTKEY_levelcoloration:
                        File.WriteAllText(commandsCFG, "Show LEVELCOLORATION");
                        SimulateKey();
                        break;
                    case HOTKEY_postprocess:
                        File.WriteAllText(commandsCFG, "Show POSTPROCESS");
                        SimulateKey();
                        break;
                    case HOTKEY_TPFreecam:
                        tptofreecam();
                        File.WriteAllText(commandsCFG, "Camera Default");
                        isFreecam = false;
                        SimulateKey();
                        break;
                    case HOTKEY_CTRL_F1:
                        externalwarning();
                        if (prop.Default.externalModPackage) break;
                        File.WriteAllText(commandsCFG, "SavePos 1");//TODO
                        SimulateKey();
                        break;
                    case HOTKEY_CTRL_F2:
                        externalwarning();
                        if (prop.Default.externalModPackage) break;
                        File.WriteAllText(commandsCFG, "SavePos 2");//TODO
                        SimulateKey();
                        break;
                    case HOTKEY_CTRL_F3:
                        externalwarning();
                        if (prop.Default.externalModPackage) break;
                        File.WriteAllText(commandsCFG, "SavePos 3");//TODO
                        SimulateKey();
                        break;
                    case HOTKEY_CTRL_F4:
                        externalwarning();
                        if (prop.Default.externalModPackage) break;
                        File.WriteAllText(commandsCFG, "SavePos 4");//TODO
                        SimulateKey();
                        break;
                    case HOTKEY_F1:
                        externalwarning();
                        if (prop.Default.externalModPackage) break;
                        File.WriteAllText(commandsCFG, $"LoadPos 1 {prop.Default.shCamera.ToString().ToLower()}");//TODO
                        SimulateKey();
                        break;
                    case HOTKEY_F2:
                        externalwarning();
                        if (prop.Default.externalModPackage) break;
                        File.WriteAllText(commandsCFG, $"LoadPos 2 {prop.Default.shCamera.ToString().ToLower()}");
                        SimulateKey();
                        break;
                    case HOTKEY_F3:
                        externalwarning();
                        if (prop.Default.externalModPackage) break;
                        File.WriteAllText(commandsCFG, $"LoadPos 3 {prop.Default.shCamera.ToString().ToLower()}");
                        SimulateKey();
                        break;
                    case HOTKEY_F4:
                        externalwarning();
                        if (prop.Default.externalModPackage) break;
                        File.WriteAllText(commandsCFG, $"LoadPos 4 {prop.Default.shCamera.ToString().ToLower()}");
                        SimulateKey();
                        break;
                    case HOTKEY_Exit:
                        var OLProc = Process.GetProcessesByName("OLGame");
                        foreach (var item in OLProc)
                        {
                            item.Kill();
                            item.WaitForExit();
                        }
                        break;
                    case HOTKEY_ShowDebug:
                        File.WriteAllText(commandsCFG, "ShowDebug");
                        SimulateKey();
                        break;
                    case HOTKEY_Paths:
                        File.WriteAllText(commandsCFG, "Show Paths");
                        SimulateKey();
                        break;
                    case HOTKEY_playerinfo:
                        File.WriteAllText(commandsCFG, "playerinfo");
                        SimulateKey();
                        break;
                    case HOTKEY_staticmeshes:
                        File.WriteAllText(commandsCFG, "Show STATICMESHES");
                        SimulateKey();
                        break;
                    case HOTKEY_bounds:
                        File.WriteAllText(commandsCFG, "Show BOUNDS");
                        SimulateKey();
                        break;
                    case HOTKEY_ai:
                        File.WriteAllText(commandsCFG, "ToggleAIDebug");
                        SimulateKey();
                        break;
                    case HOTKEY_meshedges:
                        File.WriteAllText(commandsCFG, "Show MESHEDGES");
                        SimulateKey();
                        break;
                    case HOTKEY_gamemarkers:
                        externalwarning();
                        isDebugledge = !isDebugledge;
                        File.WriteAllText(commandsCFG, $"DebugMarkers {isDebugledge.ToString().ToLower()}");
                        SimulateKey();
                        break;
                    case HOTKEY_gamespeed:
                        externalwarning();
                        gamespeedValue = !gamespeedValue;
                        if (gamespeedValue)
                        {
                            File.WriteAllText(commandsCFG, "gamespeed 10");

                        }
                        else
                        {
                            File.WriteAllText(commandsCFG, "gamespeed 1");

                        }
                        SimulateKey();
                        break;
                    case HOTKEY_reloadcheckpoint:
                        externalwarning();
                        File.WriteAllText(commandsCFG, "ReloadCheckpoint");
                        SimulateKey();
                        break;
                    case HOTKEY_fog:
                        File.WriteAllText(commandsCFG,"Show FOG");
                        SimulateKey();
                        break;
                    case HOTKEY_removecollactibles:
                        externalwarning();
                        File.WriteAllText(commandsCFG, "removerecordings || removedocuments");
                        SimulateKey();
                        break;
                    case HOTKEY_rsw:
                        externalwarning();
                        File.WriteAllText(commandsCFG, "rsw");
                        SimulateKey();
                        break;
                    case HOTKEY_loadCheckpoint:
                        deleteEveryCheckpointExceptReadOnly();
                        File.WriteAllText(commandsCFG,"loadcheckpoint");
                        SimulateKey();
                        break;
                    case HOTKEY_saveCheckpoint:
                        File.WriteAllText(commandsCFG, "savecheckpoint");
                        SimulateKey();
                        break;
                    case HOTKEY_Normal:
                        hitboxnormal();
                        break;
                    case HOTKEY_Vault:
                        hiboxvault();
                        break;
                    case HOTKEY_Door:
                        hitboxdoor();
                        break;
                    case HOTKEY_Shimmy:
                        hitboxshimmy();
                        break;
                }
            }
        }

        public void TriggerHotkey(int hotkeyId)
        {
            forehook();
            // Der Switch-Case aus WndProc, aber ohne Message-Objekt
            switch (hotkeyId)
            {
                case HOTKEY_Menu:
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Normal;
                        this.Show();
                        this.Activate();
                        AddLog("Changed WindowState");
                        ToggleMods();
                    }
                    else
                    {
                        FadeOutAndMinimize();
                        AddLog("Changed WindowState");
                    }
                    break;

                case HOTKEY_DeleteExcept:
                    this.BeginInvoke((Action)(() => deleteEveryCheckpointExceptReadOnly()));
                    AddLog("Removed checkpoints");
                    break;

                case HOTKEY_AddSelected:
                    this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                    AddLog("Added checkpoints");
                    break;

                case HOTKEY_LoadPreset_1:
                    this.BeginInvoke((Action)(() => deleteEveryCheckpointExceptReadOnly()));
                    this.BeginInvoke((Action)(() => ApplyPreset(treeViewProject, 0)));
                    this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                    AddLog("Loaded Preset 1");
                    break;

                case HOTKEY_LoadPreset_2:
                    this.BeginInvoke((Action)(() => deleteEveryCheckpointExceptReadOnly()));
                    this.BeginInvoke((Action)(() => ApplyPreset(treeViewProject, 1)));
                    this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                    AddLog("Loaded Preset 2");
                    break;

                case HOTKEY_LoadPreset_3:
                    this.BeginInvoke((Action)(() => deleteEveryCheckpointExceptReadOnly()));
                    this.BeginInvoke((Action)(() => ApplyPreset(treeViewProject, 2)));
                    this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                    AddLog("Loaded Preset 3");
                    break;

                case HOTKEY_LoadPreset_4:
                    this.BeginInvoke((Action)(() => deleteEveryCheckpointExceptReadOnly()));
                    this.BeginInvoke((Action)(() => ApplyPreset(treeViewProject, 3)));
                    this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                    AddLog("Loaded Preset 4");
                    break;

                case HOTKEY_LoadPreset_5:
                    this.BeginInvoke((Action)(() => deleteEveryCheckpointExceptReadOnly()));
                    this.BeginInvoke((Action)(() => ApplyPreset(treeViewProject, 4)));
                    this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                    AddLog("Loaded Preset 5");
                    break;

                case HOTKEY_LoadPreset_6:
                    this.BeginInvoke((Action)(() => deleteEveryCheckpointExceptReadOnly()));
                    this.BeginInvoke((Action)(() => ApplyPreset(treeViewProject, 5)));
                    this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                    AddLog("Loaded Preset 6");
                    break;

                case HOTKEY_LoadPreset_7:
                    this.BeginInvoke((Action)(() => deleteEveryCheckpointExceptReadOnly()));
                    this.BeginInvoke((Action)(() => ApplyPreset(treeViewProject, 6)));
                    this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                    AddLog("Loaded Preset 7");
                    break;

                case HOTKEY_LoadPreset_8:
                    this.BeginInvoke((Action)(() => deleteEveryCheckpointExceptReadOnly()));
                    this.BeginInvoke((Action)(() => ApplyPreset(treeViewProject, 7)));
                    this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                    AddLog("Loaded Preset 8");
                    break;

                case HOTKEY_LoadPreset_9:
                    this.BeginInvoke((Action)(() => deleteEveryCheckpointExceptReadOnly()));
                    this.BeginInvoke((Action)(() => ApplyPreset(treeViewProject, 8)));
                    this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                    AddLog("Loaded Preset 9");
                    break;

                case HOTKEY_SavePreset_1:
                    this.BeginInvoke((Action)(() => SaveCurrentTreeAsPreset(treeViewProject, 0)));
                    AddLog("Saved Preset 1");
                    break;

                case HOTKEY_SavePreset_2:
                    this.BeginInvoke((Action)(() => SaveCurrentTreeAsPreset(treeViewProject, 1)));
                    AddLog("Saved Preset 2");
                    break;

                case HOTKEY_SavePreset_3:
                    this.BeginInvoke((Action)(() => SaveCurrentTreeAsPreset(treeViewProject, 2)));
                    AddLog("Saved Preset 3");
                    break;

                case HOTKEY_SavePreset_4:
                    this.BeginInvoke((Action)(() => SaveCurrentTreeAsPreset(treeViewProject, 3)));
                    AddLog("Saved Preset 4");
                    break;

                case HOTKEY_SavePreset_5:
                    this.BeginInvoke((Action)(() => SaveCurrentTreeAsPreset(treeViewProject, 4)));
                    AddLog("Saved Preset 5");
                    break;

                case HOTKEY_SavePreset_6:
                    this.BeginInvoke((Action)(() => SaveCurrentTreeAsPreset(treeViewProject, 5)));
                    AddLog("Saved Preset 6");
                    break;

                case HOTKEY_SavePreset_7:
                    this.BeginInvoke((Action)(() => SaveCurrentTreeAsPreset(treeViewProject, 6)));
                    AddLog("Saved Preset 7");
                    break;

                case HOTKEY_SavePreset_8:
                    this.BeginInvoke((Action)(() => SaveCurrentTreeAsPreset(treeViewProject, 7)));
                    AddLog("Saved Preset 8");
                    break;

                case HOTKEY_SavePreset_9:
                    this.BeginInvoke((Action)(() => SaveCurrentTreeAsPreset(treeViewProject, 8)));
                    AddLog("Saved Preset 9");
                    break;

                case HOTKEY_InfiniteNightVision:
                    nightvision();
                    break;

                case HOTKEY_FreezeEnemy:
                    freezeEnemy();
                    break;

                case HOTKEY_NoDamage:
                    noDamage();
                    break;
                case HOTKEY_Freecam:
                    isFreecam = !isFreecam;

                    if (isFreecam)
                    {
                        File.WriteAllText(commandsCFG, "Camera Freecam");
                    }
                    else
                    {
                        File.WriteAllText(commandsCFG, "Camera Default");
                    }
                    SimulateKey();
                    break;
                case HOTKEY_StatFPS:
                    File.WriteAllText(commandsCFG, "Stat FPS");
                    SimulateKey();
                    break;
                case HOTKEY_ShowCollision:
                    File.WriteAllText(commandsCFG, "Show COLLISION");
                    SimulateKey();
                    break;
                case HOTKEY_Gamma10:
                    isGamma10 = !isGamma10;
                    if (isGamma10)
                    {
                        File.WriteAllText(commandsCFG, "Gamma 10");
                    }
                    else
                    {
                        File.WriteAllText(commandsCFG, "Gamma 3");//TODO CHECK IF WORKING
                    }
                    SimulateKey();
                    break;
                case HOTKEY_StatLevels:
                    File.WriteAllText(commandsCFG, "Stat LEVELS");
                    SimulateKey();
                    break;
                case HOTKEY_SetInsaneDifficulty:
                    File.WriteAllText(commandsCFG, "set OLGame DifficultyMode EDM_Insane");
                    SimulateKey();
                    break;
                case HOTKEY_volumes:
                    File.WriteAllText(commandsCFG, "Show VOLUMES");
                    SimulateKey();
                    break;
                case HOTKEY_levelcoloration:
                    File.WriteAllText(commandsCFG, "Show LEVELCOLORATION");
                    SimulateKey();
                    break;
                case HOTKEY_postprocess:
                    File.WriteAllText(commandsCFG, "Show POSTPROCESS");
                    SimulateKey();
                    break;
                case HOTKEY_TPFreecam:
                    tptofreecam();
                    File.WriteAllText(commandsCFG, "Camera Default");
                    isFreecam = false;
                    SimulateKey();
                    break;
                case HOTKEY_CTRL_F1:
                    externalwarning();
                    if (prop.Default.externalModPackage) break;
                    File.WriteAllText(commandsCFG, "SavePos 1");//TODO
                    SimulateKey();
                    break;
                case HOTKEY_CTRL_F2:
                    externalwarning();
                    if (prop.Default.externalModPackage) break;
                    File.WriteAllText(commandsCFG, "SavePos 2");//TODO
                    SimulateKey();
                    break;
                case HOTKEY_CTRL_F3:
                    externalwarning();
                    if (prop.Default.externalModPackage) break;
                    File.WriteAllText(commandsCFG, "SavePos 3");//TODO
                    SimulateKey();
                    break;
                case HOTKEY_CTRL_F4:
                    externalwarning();
                    if (prop.Default.externalModPackage) break;
                    File.WriteAllText(commandsCFG, "SavePos 4");//TODO
                    SimulateKey();
                    break;
                case HOTKEY_F1:
                    externalwarning();
                    if (prop.Default.externalModPackage) break;
                    File.WriteAllText(commandsCFG, $"LoadPos 1 {prop.Default.shCamera.ToString().ToLower()}");//TODO
                    SimulateKey();
                    break;
                case HOTKEY_F2:
                    externalwarning();
                    if (prop.Default.externalModPackage) break;
                    File.WriteAllText(commandsCFG, $"LoadPos 2 {prop.Default.shCamera.ToString().ToLower()}");
                    SimulateKey();
                    break;
                case HOTKEY_F3:
                    externalwarning();
                    if (prop.Default.externalModPackage) break;
                    File.WriteAllText(commandsCFG, $"LoadPos 3 {prop.Default.shCamera.ToString().ToLower()}");
                    SimulateKey();
                    break;
                case HOTKEY_F4:
                    externalwarning();
                    if (prop.Default.externalModPackage) break;
                    File.WriteAllText(commandsCFG, $"LoadPos 4 {prop.Default.shCamera.ToString().ToLower()}");
                    SimulateKey();
                    break;
                case HOTKEY_Exit:
                    var OLProc = Process.GetProcessesByName("OLGame");
                    foreach (var item in OLProc)
                    {
                        item.Kill();
                        item.WaitForExit();
                    }
                    break;
                case HOTKEY_ShowDebug:
                    File.WriteAllText(commandsCFG, "ShowDebug");
                    SimulateKey();
                    break;
                case HOTKEY_Paths:
                    File.WriteAllText(commandsCFG, "Show Paths");
                    SimulateKey();
                    break;
                case HOTKEY_playerinfo:
                    File.WriteAllText(commandsCFG, "playerinfo");
                    SimulateKey();
                    break;
                case HOTKEY_staticmeshes:
                    File.WriteAllText(commandsCFG, "Show STATICMESHES");
                    SimulateKey();
                    break;
                case HOTKEY_bounds:
                    File.WriteAllText(commandsCFG, "Show BOUNDS");
                    SimulateKey();
                    break;
                case HOTKEY_ai:
                    File.WriteAllText(commandsCFG, "ToggleAIDebug");
                    SimulateKey();
                    break;
                case HOTKEY_meshedges:
                    File.WriteAllText(commandsCFG, "Show MESHEDGES");
                    SimulateKey();
                    break;
                case HOTKEY_gamemarkers:
                    externalwarning();
                    isDebugledge = !isDebugledge;
                    File.WriteAllText(commandsCFG, $"DebugMarkers {isDebugledge.ToString().ToLower()}");
                    SimulateKey();
                    break;
                case HOTKEY_gamespeed:
                    externalwarning();
                    gamespeedValue = !gamespeedValue;
                    if (gamespeedValue)
                    {
                        File.WriteAllText(commandsCFG, "gamespeed 10");

                    }
                    else
                    {
                        File.WriteAllText(commandsCFG, "gamespeed 1");

                    }
                    SimulateKey();
                    break;
                case HOTKEY_reloadcheckpoint:
                    externalwarning();
                    File.WriteAllText(commandsCFG, "ReloadCheckpoint");
                    SimulateKey();
                    break;
                case HOTKEY_fog:
                    File.WriteAllText(commandsCFG, "Show FOG");
                    SimulateKey();
                    break;
                case HOTKEY_removecollactibles:
                    externalwarning();
                    File.WriteAllText(commandsCFG, "removerecordings || removedocuments");
                    SimulateKey();
                    break;
                case HOTKEY_rsw:
                    externalwarning();
                    File.WriteAllText(commandsCFG, "rsw");
                    SimulateKey();
                    break;
                case HOTKEY_loadCheckpoint:
                    deleteEveryCheckpointExceptReadOnly();
                    File.WriteAllText(commandsCFG, "loadcheckpoint");
                    SimulateKey();
                    break;
                case HOTKEY_saveCheckpoint:
                    File.WriteAllText(commandsCFG, "savecheckpoint");
                    SimulateKey();
                    break;
                case HOTKEY_Normal:
                    hitboxnormal();
                    break;
                case HOTKEY_Vault:
                    hiboxvault();
                    break;
                case HOTKEY_Door:
                    hitboxdoor();
                    break;
                case HOTKEY_Shimmy:
                    hitboxshimmy();
                    break;
            }

        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        private const int KEYEVENTF_KEYDOWN = 0x0000;
        private const int KEYEVENTF_KEYUP = 0x0002;
        public static void SimulateKey()
        {

            if (prop.Default.speedrun)
                return;
            
            // 0-Taste auf der Haupttastatur = 0x30
            byte key = 0xA1;

            // Taste drücken
            keybd_event(key, 0, KEYEVENTF_KEYDOWN, 0);

            // Taste loslassen
            keybd_event(key, 0, KEYEVENTF_KEYUP, 0);
        }

        public void noDamage()
        {
            isNoDamage = !isNoDamage;

            if (prop.Default.externalModPackage)
            {


                if (checkBit64())
                {
                    try
                    {

                        ProcessStartInfo psi = new ProcessStartInfo()
                        {
                            FileName = Path.Combine(Directory.GetCurrentDirectory(), "SaveManager-Mods", "nodamage64.exe"),
                            Arguments = isNoDamage.ToString(),
                            UseShellExecute = false, // false, damit man z.B. Standardausgabe lesen kann
                            CreateNoWindow = true   // true, wenn kein Fenster aufgehen soll
                        };

                        Process process = Process.Start(psi);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Error: " + e);
                        throw;
                    }
                }
                else
                {
                    try
                    {

                        ProcessStartInfo psi = new ProcessStartInfo()
                        {
                            FileName = Path.Combine(Directory.GetCurrentDirectory(), "SaveManager-Mods", "nodamage32.exe"),
                            Arguments = isNoDamage.ToString(),
                            UseShellExecute = false, // false, damit man z.B. Standardausgabe lesen kann
                            CreateNoWindow = true   // true, wenn kein Fenster aufgehen soll
                        };

                        Process process = Process.Start(psi);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Error: " + e);
                        throw;
                    }
                }

                if (prop.Default.borderless)
                {
                    if (isNoDamage)
                    {
                        ShowOverlay("NoDamage", "NoDamage");
                    }
                    else
                    {
                        HideOverlay("NoDamage");
                    }
                }

            }
            else
            {
                File.WriteAllText(commandsCFG, "NoDamage");
                forehook();
                SimulateKey();
                if (isNoDamage)
                {
                    ShowOverlay("NoDamage", "NoDamage");
                }
                else
                {
                    HideOverlay("NoDamage");
                }
            }
        }

        public void nightvision()
        {
            nightVisionActive = !nightVisionActive;

            if (checkBit64())
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo()
                    {
                        FileName = Path.Combine(Directory.GetCurrentDirectory(), "SaveManager-Mods", "NightVision64.exe"),
                        Arguments = nightVisionActive.ToString(),
                        UseShellExecute = false, // false, damit man z.B. Standardausgabe lesen kann
                        CreateNoWindow = true   // true, wenn kein Fenster aufgehen soll
                    };

                    Process process = Process.Start(psi);
                    AddLog($"Infinite NightVision: {nightVisionActive}");
                    nightVisionActive = !nightVisionActive;

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            else
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo()
                    {
                        FileName = Path.Combine(Directory.GetCurrentDirectory(), "SaveManager-Mods", "NightVision32.exe"),
                        Arguments = nightVisionActive.ToString(),
                        UseShellExecute = false, // false, damit man z.B. Standardausgabe lesen kann
                        CreateNoWindow = true   // true, wenn kein Fenster aufgehen soll
                    };

                    Process process = Process.Start(psi);
                    AddLog($"Infinite NightVision: {nightVisionActive}");


                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            if (prop.Default.borderless)
            {
                if (nightVisionActive)
                {
                    ShowOverlay("NightVision", "NightVision");
                }
                else
                {
                    HideOverlay("NightVision");
                }
            }
        }
       

        // Dictionary für alle Overlays


        public void ShowOverlay(string modName, string text)
        {
            if (!prop.Default.borderless)
                return;

            // Overlay vorher entfernen, falls es existiert
            if (activeOverlays.ContainsKey(modName))
            {
                HideOverlay(modName);
            }

            // Neues Overlay erstellen
            Overlay overlay = new Overlay(text);

            // Feste Y-Position je nach Mod
            int yOffset = modName switch
            {
                "NightVision" => 50,
                "FreezeEnemy" => 65,
                "NoDamage" => 95,
                "ShimmyHitbox"   => 110,
                "DoorHitbox"     => 110,
                "VaultHitbox"    => 110,
                "PlayerInfo" => 125,
                _ => 140,               // alle anderen Mods //ADD MOD //MOD ADD //TODO
            };

            overlay.Location = new Point(Screen.PrimaryScreen.Bounds.Width - 140, yOffset);

            overlay.Show();
            activeOverlays.Add(modName, overlay);
        }





        public void HideOverlay(string modName)
        {
            if (activeOverlays.ContainsKey(modName))
            {
                var overlay = activeOverlays[modName];
                if (!overlay.IsDisposed)
                {
                    overlay.Close();    // Fenster schließen
                    overlay.Dispose();  // Ressourcen freigeben
                }
                activeOverlays.Remove(modName);
            }
        }




        public bool checkBit64()
        {
            Process[] processes = Process.GetProcessesByName("OLGame"); // Name ohne .exe

            Process p = processes[0];
            bool isWow64;
            if (!IsWow64Process(p.Handle, out isWow64)) { }

            if (isWow64)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        void SavePresets()
        {
            var json = JsonSerializer.Serialize(presets, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(presetFile, json);
        }


        void SaveCurrentTreeAsPreset(TreeView tree, int index)
        {
            var list = new List<string>();
            CollectCheckedNodes(tree.Nodes, list);

            // Sicherstellen, dass die Liste groß genug ist
            while (index >= presets.Count)
            {
                presets.Add(new List<string>());
            }

            presets[index] = list;
            SavePresets();
        }


        // Hilfsmethode zum Sammeln gecheckter Nodes
        void CollectCheckedNodes(TreeNodeCollection nodes, List<string> list)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Checked)
                    list.Add(node.Text); // oder node.Name
                if (node.Nodes.Count > 0)
                    CollectCheckedNodes(node.Nodes, list);
            }
        }

        // Preset anwenden
        /*void ApplyPreset(TreeView tree, int index)
        {
            if (index < 0 || index >= presets.Count) return;
            var list = presets[index];
            ApplyToNode(tree.Nodes, list);
        }*/

        void ApplyPreset(TreeView tree, int index)
        {
            // Sicherstellen, dass der Index gültig ist
            if (index < 0) return;

            // Wenn der Index größer ist als die aktuelle Liste, erweitern wir sie
            while (index >= presets.Count)
            {
                presets.Add(new List<string>());
            }

            var list = presets[index];
            ApplyToNode(tree.Nodes, list);
        }



        // Hilfsmethode zum Setzen der Checkboxen
        void ApplyToNode(TreeNodeCollection nodes, List<string> list)
        {
            foreach (TreeNode node in nodes)
            {
                node.Checked = list.Contains(node.Text); // oder node.Name
                if (node.Nodes.Count > 0)
                    ApplyToNode(node.Nodes, list);
            }
        }

        private void addEveryCheckpoint()
        {
            var node = GetCheckedCheckpoints(treeViewProject);

            if (node.Count == 0)
            {
                CheckAllNodes(treeViewProject);
            }

            btnAddCheckpoints_Click2(null, null);
        }

        private void deleteEveryCheckpointExceptReadOnly()
        {
            updateNeeded();
            CheckAllNodes(treeViewLive);
            if (checkIfOnlyParentExists(false))
            {
                AddWarningLog("No checkpoints selected to remove");
                return;
            }

            var selectedNodes = GetCheckedCheckpoints(treeViewLive);

            if (selectedNodes.Count == 0)
            {
                AddWarningLog("No checkpoints selected to remove");
                return;
            }

            if (string.IsNullOrEmpty(gameSaveFolder) || !Directory.Exists(gameSaveFolder))
            {
                AddErrorLog("Save-Folder from game not found. Please start the game.");
                return;
            }

            // Prüfen, ob unter den Ausgewählten ReadOnly-Dateien sind
            bool hasReadOnly = selectedNodes.Any(node =>
            {
                if (node.Nodes.Count > 0) return false; // ✅ Parent überspringen
                string filePath = node.Tag as string;
                return !string.IsNullOrEmpty(filePath) &&
                       File.Exists(filePath) &&
                       (File.GetAttributes(filePath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
            });

            bool allReadOnly = selectedNodes
                .Where(node => node.Nodes.Count == 0) // ✅ nur Child-Nodes berücksichtigen
                .All(node =>
                {
                    string filePath = node.Tag as string;
                    return !string.IsNullOrEmpty(filePath) &&
                         File.Exists(filePath) &&
                           (File.GetAttributes(filePath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
                });

            bool deleteReadOnly = false;


            int deleted = 0;
            int skippedReadOnly = 0;

            foreach (TreeNode node in selectedNodes)
            {
                string filePath = node.Tag as string;
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                    continue;

                try
                {
                    var attrs = File.GetAttributes(filePath);
                    bool isRO = (attrs & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;

                    // Wenn Nein: ReadOnly-Dateien überspringen (nicht ändern, nicht löschen)
                    if (isRO && !deleteReadOnly)
                    {
                        skippedReadOnly++;
                        continue;
                    }

                    // Wenn Ja: ReadOnly-Attribut entfernen, dann löschen
                    if (isRO && deleteReadOnly)
                    {
                        File.SetAttributes(filePath, attrs & ~FileAttributes.ReadOnly);
                    }

                    File.Delete(filePath);
                    node.Remove(); // entfernt den Node unabhängig davon, ob Root oder Child
                    deleted++;

                    if (readOnlyPaths.Contains(filePath))
                    {
                        readOnlyPaths.Remove(filePath);
                        SaveReadOnlyList(); // 💾 Liste sofort aktualisieren
                        //AddLog($"Removed {Path.GetFileName(filePath)} from ReadOnly list.");
                    }

                }
                catch (Exception ex)
                {
                    AddErrorLog($"\nError while deleting {Path.GetFileName(filePath)}: {ex.Message}");
                }
            }

            UncheckAllNodes(treeViewLive);
            if (skippedReadOnly == 0)
            {
                AddLog($"Removed {deleted} checkpoints from live saves.");
            }
            else
            {
                AddLog($"Removed {deleted} checkpoints from live saves.\nSkipped ReadOnly: {skippedReadOnly}.");
            }
        }

        [Flags]
        public enum KeyModifiers
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            Windows = 8
        }

        private void AttachToProcess(string processName)
        {
            // Alle Prozesse mit diesem Namen suchen
            var processes = Process.GetProcessesByName(processName);

            if (processes.Length > 0)
            {
                watchedProcess = processes.First();

                // Events aktivieren
                watchedProcess.EnableRaisingEvents = true;
                watchedProcess.Exited += WatchedProcess_Exited;
            }
            else
            {
                MessageBox.Show($"Process '{processName}' is not running.\nPlease boot the game\nIf you booted the game for the first time,\ngo one folder back in your explorer and start trough these shortcuts");
                Process.GetCurrentProcess().Kill();

            }
        }

        private void WatchedProcess_Exited(object sender, EventArgs e)
        {
            // Dein Programm beenden, wenn das beobachtete Programm geschlossen wird
            if (InvokeRequired)
            {
                Invoke(new Action(() => Application.Exit()));
            }
            else
            {
                Application.Exit();
            }
        }
        private void updateNeeded()
        {
            if (IsDisposed || Disposing)
                return;
            if (InvokeRequired)
            {
                Invoke(new Action(updateNeeded));
                return;
            }
            if (!CompareTreeViewWithDirectory(treeViewLive, gameSaveFolder))
            {
                btnLoadLiveSaves_Click(null, null);
            }
        }

        private void UncheckAllNodes(TreeView tree)
        {
            foreach (TreeNode node in tree.Nodes)
            {
                UncheckNodeRecursive(node);
            }
        }

        private void UncheckNodeRecursive(TreeNode node)
        {
            node.Checked = false;
            foreach (TreeNode child in node.Nodes)
            {
                UncheckNodeRecursive(child);
            }
        }
        private void StopTimer()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
        private void StartTimer()
        {
            timer.Change(0, 1000);
        }

        private void treeViewProject_MouseWheel(object sender, MouseEventArgs e)
        {
            TreeView tree = sender as TreeView;
            int scrollAmount = e.Delta > 0 ? 20 : -20; // Pixel pro Scroll
            Rectangle rect = tree.DisplayRectangle;
            rect.Y += scrollAmount;
            tree.Invalidate();
        }

        private void treeViewProject_Paint(object sender, PaintEventArgs e)
        {
            TreeView tree = sender as TreeView;
            int clientHeight = tree.ClientSize.Height;

            if (tree.Nodes.Count > 0 && tree.DisplayRectangle.Height > clientHeight)
            {
                float viewRatio = clientHeight / (float)tree.DisplayRectangle.Height;
                int thumbHeight = Math.Max(6, (int)(clientHeight * viewRatio));
                int maxScroll = Math.Max(1, tree.DisplayRectangle.Height - clientHeight);
                int thumbPos = (int)(-tree.DisplayRectangle.Y / (float)maxScroll * (clientHeight - thumbHeight));

                Rectangle thumb = new Rectangle(tree.Width - 6, thumbPos, 3, thumbHeight);
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 200, 200, 200))) // hellgrau
                {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    e.Graphics.FillRectangle(brush, thumb);
                }
            }
        }

        private void panelLive_Paint(object sender, PaintEventArgs e)
        {
            int radius = 12;
            int borderWidth = 2;
            Color borderColor = Color.FromArgb(255, 119, 188, 136);

            Rectangle rect = panelProject.ClientRectangle;
            rect.Width -= borderWidth;  // Korrektur nach innen
            rect.Height -= borderWidth; // Korrektur nach innen

            using (GraphicsPath path = RoundedRect(rect, radius))
            using (Pen pen = new Pen(borderColor, borderWidth))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawPath(pen, path);
            }
        }

        private void panelProject_Paint(object sender, PaintEventArgs e)
        {
            int radius = 12;
            int borderWidth = 2;
            Color borderColor = Color.FromArgb(255, 119, 188, 136);

            Rectangle rect = panelProject.ClientRectangle;
            rect.Width -= borderWidth;  // Korrektur nach innen
            rect.Height -= borderWidth; // Korrektur nach innen

            using (GraphicsPath path = RoundedRect(rect, radius))
            using (Pen pen = new Pen(borderColor, borderWidth))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawPath(pen, path);
            }
        }
        /*private void panelBtnAdd_Paint(object sender, PaintEventArgs e)
        {
            int radius = 12;
            int borderWidth = 2;
            Color borderColor = Color.FromArgb(255, 119, 188, 136);

            Rectangle rect = panelBtnAdds.ClientRectangle;
            rect.Width -= borderWidth;  // Korrektur nach innen
            rect.Height -= borderWidth; // Korrektur nach innen

            using (GraphicsPath path = RoundedRect(rect, radius))
            using (Pen pen = new Pen(borderColor, borderWidth))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawPath(pen, path);
            }
        }*/
        private void panelBtnRemoveChk_Paint(object sender, PaintEventArgs e)
        {
            int radius = 12;
            int borderWidth = 2;
            Color borderColor = Color.FromArgb(255, 119, 188, 136);

            Rectangle rect = btnRemoveChk.ClientRectangle;
            rect.Width -= borderWidth;  // Korrektur nach innen
            rect.Height -= borderWidth; // Korrektur nach innen

            using (GraphicsPath path = RoundedRect(rect, radius))
            using (Pen pen = new Pen(borderColor, borderWidth))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawPath(pen, path);
            }
        }
        private void panelBtnRemoveChk_Paint2(object sender, PaintEventArgs e)
        {
            int radius = 12;
            int borderWidth = 2;
            //Color borderColor = Color.FromArgb(10, 119, 188, 136);
            Color borderColor = Color.FromArgb(200, 255, 255, 255);

            Rectangle rect = btnRemoveChk.ClientRectangle;
            rect.Width -= borderWidth;  // Korrektur nach innen
            rect.Height -= borderWidth; // Korrektur nach innen

            using (GraphicsPath path = RoundedRect(rect, radius))
            using (Pen pen = new Pen(borderColor, borderWidth))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawPath(pen, path);
            }
        }
        private void panelBar_Paint(object sender, PaintEventArgs e)
        {
            int radius = 12;
            int borderWidth = 2;
            Color borderColor = Color.FromArgb(255, 119, 188, 136);

            Rectangle rect = panelBar.ClientRectangle;
            rect.Width -= borderWidth;  // Korrektur nach innen
            rect.Height -= borderWidth; // Korrektur nach innen

            using (GraphicsPath path = RoundedRect(rect, radius))
            using (Pen pen = new Pen(borderColor, borderWidth))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawPath(pen, path);
            }
        }

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            GraphicsPath path = new GraphicsPath();

            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }


        /// <summary>
        /// Setzt die Form auf runde, glatte Ecken.
        /// </summary>
        private void ApplyRoundedCorners(int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90); // oben links
            path.AddArc(Width - radius, 0, radius, radius, 270, 90); // oben rechts
            path.AddArc(Width - radius, Height - radius, radius, radius, 0, 90); // unten rechts
            path.AddArc(0, Height - radius, radius, radius, 90, 90); // unten links
            path.CloseFigure();
            this.Region = new Region(path);
        }

        /// <summary>
        /// Fade-Out und minimieren
        /// </summary>
        private void FadeOutAndMinimize()
        {
            StopTimer(); // Timer stoppen, um unnötige Aufrufe zu vermeiden
            fadingOut = true;
            fadeTimer.Start();

            ToggleMods();


        }
        private void ToggleMods()
        {
            if (modsInstance != null && !modsInstance.IsDisposed)
            {
                if (modsInstance.WindowState == FormWindowState.Minimized)
                {
                    // Wenn minimiert → wieder normal
                    modsInstance.WindowState = FormWindowState.Normal;
                    modsInstance.Show();
                    modsInstance.BringToFront();
                }
                else
                {
                    // Wenn normal oder maximiert → minimieren
                    modsInstance.WindowState = FormWindowState.Minimized;
                }
            }
        }

        // FadeTimer_Tick bleibt wie gehabt

        private void SetRoundedCorners(Form form, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            Rectangle rect = new Rectangle(0, 0, form.Width, form.Height);

            // Oben links
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            // Oben rechts
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            // Unten rechts
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            // Unten links
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);

            path.CloseAllFigures();
            form.Region = new Region(path);
        }


        // -------------------------
        // Fade Logic
        // -------------------------
        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            if (fadingOut)
            {
                if (this.Opacity > 0)
                    this.Opacity -= fadeStep;
                else
                {
                    fadeTimer.Stop();
                    fadingOut = false;
                    this.WindowState = FormWindowState.Minimized;
                    this.Opacity = 0;
                }
            }
            else if (fadingIn)
            {
                StartTimer(); // Timer wieder starten, wenn wir einblenden
                if (this.Opacity < 1)
                    this.Opacity += fadeStep;
                else
                {
                    fadeTimer.Stop();
                    fadingIn = false;
                    this.Opacity = 1;
                }
            }
        }

        private void btnMin_Click(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                fadingOut = true;
                fadeTimer.Start();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (this.WindowState == FormWindowState.Normal && this.Opacity == 0)
            {
                fadingIn = true;
                fadeTimer.Start();
            }
        }

        // -------------------------
        // Titlebar Drag
        // -------------------------
        private void titleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        // -------------------------
        // Live Save & Project Load
        // -------------------------

        public bool CompareTreeViewWithDirectory(TreeView tree, string path)
        {
            int nodeCount = CountNodes(tree.Nodes);
            int dirCount = CountDirectoryItems(path);

            return nodeCount == dirCount;
        }

        // Zählt rekursiv alle Nodes in der TreeView
        private int CountNodes(TreeNodeCollection nodes)
        {
            int count = 0;
            foreach (TreeNode node in nodes)
            {
                count++;
                count += CountNodes(node.Nodes); // Kinder mitzählen
            }
            return count;
        }

        // Zählt rekursiv alle Dateien + Ordner im Pfad
        private int CountDirectoryItems(string path)
        {
            int count = 0;

            try
            {
                // Dateien zählen
                count += Directory.GetFiles(path).Length;

                // Unterordner zählen + darin weitergehen
                foreach (var dir in Directory.GetDirectories(path))
                {
                    count++; // den Ordner selbst mitzählen
                    count += CountDirectoryItems(dir);
                }
            }
            catch
            {
                // Fehler abfangen (z.B. kein Zugriff)
            }

            return count;
        }
        private void btnLoadLiveSaves_Click(object sender, EventArgs e)
        {

            if (InvokeRequired)
            {
                Invoke(new Action(() => btnLoadLiveSaves_Click(sender, e)));
                return;
            }
            var processes = Process.GetProcessesByName("OLGame");
            if (processes.Length == 0)
            {
                AddErrorLog("Outlast is not running!");
                return;
            }

            string exePath = processes[0].MainModule.FileName;
            string gameDir = Path.GetDirectoryName(exePath);

            gameSaveFolder = Path.Combine(gameDir, "..", "..", "OLGame", "SaveData");
            gameSaveFolder = Path.GetFullPath(gameSaveFolder);

            if (!Directory.Exists(gameSaveFolder))
            {
                AddErrorLog("Save folder not found!");
                return;
            }

            treeViewLive.Nodes.Clear();
            LoadTreeFromFolder(treeViewLive, gameSaveFolder);
            MarkReadOnlyNodes(treeViewLive);
        }

        private void LoadProjectSaves()
        {
            treeViewProject.Nodes.Clear();
            if (Directory.Exists(projectSaveFolder))
                LoadTreeFromFolder(treeViewProject, projectSaveFolder);
        }

        private void LoadTreeFromFolder(TreeView treeView, string folder)
        {
            treeView.Nodes.Clear();
            var root = new TreeNode(Path.GetFileName(folder)) { Tag = folder };
            AddFolderNodes(root);
            treeView.Nodes.Add(root);
            root.Expand();
        }

        private void AddFolderNodes(TreeNode node)
        {
            string path = node.Tag.ToString();

            foreach (var dir in Directory.GetDirectories(path))
            {
                var dirNode = new TreeNode(Path.GetFileName(dir)) { Tag = dir };
                AddFolderNodes(dirNode);
                node.Nodes.Add(dirNode);
            }

            foreach (var file in Directory.GetFiles(path, "*.sav"))
            {
                string checkpointName = ExtractCheckpointName(file);
                var fileNode = new TreeNode(checkpointName) { Tag = file };
                node.Nodes.Add(fileNode);
            }
        }

        // Füge am Anfang der Klasse hinzu:

        // Lade die ReadOnly-Liste beim Start
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadReadOnlyList();
            MarkReadOnlyNodes(treeViewLive);
            MarkReadOnlyNodes(treeViewProject);
        }

        // Speichere die ReadOnly-Liste beim Schließen
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            var procread = Process.GetProcessesByName("readValues32");
            var procread64 = Process.GetProcessesByName("readValues64");
            foreach (var p in procread)
            {
                p.Kill();
                p.WaitForExit();
            }
            foreach (var p in procread64)
            {
                p.Kill();
                p.WaitForExit();
            }



            var proc = Process.GetProcessesByName("HitBoxChanger32");

            foreach (var p in proc)
            {
                p.Kill();
                p.WaitForExit();
            }
            var proc64 = Process.GetProcessesByName("HitBoxChanger64");

            foreach (var p in proc64)
            {
                p.Kill();
                p.WaitForExit();
            }
            var proc32ET = Process.GetProcessesByName("EnemyToggle32");

            foreach (var p in proc32ET)
            {
                p.Kill();
                p.WaitForExit();
            }
            var proc64ET = Process.GetProcessesByName("EnemyToggle64");

            foreach (var p in proc64ET)
            {
                p.Kill();
                p.WaitForExit();
            }
            var procs32 = Process.GetProcessesByName("checkpointHandler32");

            foreach (var proce in procs32)
            {
                try
                {
                    proce.Kill();
                }
                catch
                {
                    // optional: Fehlerbehandlung, falls Prozess nicht gekillt werden kann
                }
            }
            var procs64 = Process.GetProcessesByName("checkpointHandler64");

            foreach (var proc3 in procs64)
            {
                try
                {
                    proc3.Kill();
                }
                catch
                {
                    // optional: Fehlerbehandlung, falls Prozess nicht gekillt werden kann
                }
            }
            var procsh = Process.GetProcessesByName("outlast-speedrun-helper.exe");

            foreach (var p in procsh)
            {
                p.Kill();
                p.WaitForExit();
            }
            SavePlaytime();
            SaveReadOnlyList();
            UnregisterHotKey(this.Handle, HOTKEY_Menu);
            UnregisterHotKey(this.Handle, HOTKEY_DeleteExcept);
            UnregisterHotKey(this.Handle, HOTKEY_AddSelected);
            UnregisterHotKey(this.Handle, HOTKEY_LoadPreset_1);
            UnregisterHotKey(this.Handle, HOTKEY_LoadPreset_2);
            UnregisterHotKey(this.Handle, HOTKEY_LoadPreset_3);
            UnregisterHotKey(this.Handle, HOTKEY_LoadPreset_4);
            UnregisterHotKey(this.Handle, HOTKEY_LoadPreset_5);
            UnregisterHotKey(this.Handle, HOTKEY_LoadPreset_6);
            UnregisterHotKey(this.Handle, HOTKEY_LoadPreset_7);
            UnregisterHotKey(this.Handle, HOTKEY_LoadPreset_8);
            UnregisterHotKey(this.Handle, HOTKEY_LoadPreset_9);
            UnregisterHotKey(this.Handle, HOTKEY_SavePreset_1);
            UnregisterHotKey(this.Handle, HOTKEY_SavePreset_2);
            UnregisterHotKey(this.Handle, HOTKEY_SavePreset_3);
            UnregisterHotKey(this.Handle, HOTKEY_SavePreset_4);
            UnregisterHotKey(this.Handle, HOTKEY_SavePreset_5);
            UnregisterHotKey(this.Handle, HOTKEY_SavePreset_6);
            UnregisterHotKey(this.Handle, HOTKEY_SavePreset_7);
            UnregisterHotKey(this.Handle, HOTKEY_SavePreset_8);
            UnregisterHotKey(this.Handle, HOTKEY_SavePreset_9);
            UnregisterHotKey(this.Handle, HOTKEY_InfiniteNightVision);
            UnregisterHotKey(this.Handle, HOTKEY_FreezeEnemy);
            UnregisterHotKey(this.Handle, HOTKEY_Freecam);
            UnregisterHotKey(this.Handle, HOTKEY_StatFPS);
            UnregisterHotKey(this.Handle, HOTKEY_Gamma10);
            UnregisterHotKey(this.Handle, HOTKEY_StatLevels);
            UnregisterHotKey(this.Handle, HOTKEY_ShowCollision);
            UnregisterHotKey(this.Handle, HOTKEY_SetInsaneDifficulty);
            UnregisterHotKey(this.Handle, HOTKEY_volumes);
            UnregisterHotKey(this.Handle, HOTKEY_levelcoloration);
            UnregisterHotKey(this.Handle, HOTKEY_postprocess);
            UnregisterHotKey(this.Handle, HOTKEY_NoDamage);
            UnregisterHotKey(this.Handle, HOTKEY_CTRL_F1);
            UnregisterHotKey(this.Handle, HOTKEY_CTRL_F2);
            UnregisterHotKey(this.Handle, HOTKEY_CTRL_F3);
            UnregisterHotKey(this.Handle, HOTKEY_CTRL_F4);
            UnregisterHotKey(this.Handle, HOTKEY_F1);
            UnregisterHotKey(this.Handle, HOTKEY_F2);
            UnregisterHotKey(this.Handle, HOTKEY_F3);
            UnregisterHotKey(this.Handle, HOTKEY_F4);
            UnregisterHotKey(this.Handle, HOTKEY_ShowCollision);
            UnregisterHotKey(this.Handle, HOTKEY_Paths); 
            UnregisterHotKey(this.Handle, HOTKEY_playerinfo);
            UnregisterHotKey(this.Handle, HOTKEY_gamemarkers);
            UnregisterHotKey(this.Handle, HOTKEY_meshedges);
            UnregisterHotKey(this.Handle, HOTKEY_bounds);
            UnregisterHotKey(this.Handle, HOTKEY_staticmeshes);
            UnregisterHotKey(this.Handle, HOTKEY_ai);
            UnregisterHotKey(this.Handle, HOTKEY_gamespeed);
            UnregisterHotKey(this.Handle, HOTKEY_reloadcheckpoint);
            UnregisterHotKey(this.Handle, HOTKEY_fog);
            UnregisterHotKey(this.Handle, HOTKEY_removedocuments);
            UnregisterHotKey(this.Handle, HOTKEY_removerecordings);
            UnregisterHotKey(this.Handle, HOTKEY_removecollactibles);
            UnregisterHotKey(this.Handle, HOTKEY_rsw);
            UnregisterHotKey(this.Handle, HOTKEY_loadCheckpoint);
            UnregisterHotKey(this.Handle, HOTKEY_Normal);
            UnregisterHotKey(this.Handle, HOTKEY_Vault);
            UnregisterHotKey(this.Handle, HOTKEY_Door);
            UnregisterHotKey(this.Handle, HOTKEY_Shimmy);
            UnregisterHotKey(this.Handle, HOTKEY_saveCheckpoint);
            StopDuplicationFixLoop();

            timer?.Change(Timeout.Infinite, Timeout.Infinite);
            timer?.Dispose();
            timer = null;

            base.OnFormClosing(e);

            base.OnFormClosing(e);


        }

        // Hilfsmethoden zum Speichern/Laden
        private void SaveReadOnlyList()
        {
            try
            {
                File.WriteAllLines(readOnlyListFile, readOnlyPaths);
            }
            catch { }
        }

        private void LoadReadOnlyList()
        {
            readOnlyPaths.Clear();
            if (File.Exists(readOnlyListFile))
            {
                readOnlyPaths.AddRange(File.ReadAllLines(readOnlyListFile));
            }
        }

        // Setze die Farbe für alle Nodes, die in der ReadOnly-Liste sind
        private void MarkReadOnlyNodes(TreeView treeView)
        {
            void MarkNodes(TreeNodeCollection nodes)
            {
                foreach (TreeNode node in nodes)
                {
                    if (node.Tag is string path && readOnlyPaths.Contains(path))
                    {
                        node.ForeColor = Color.FromArgb(25, 49, 124, 203); //Blue
                    }
                    if (node.Nodes.Count > 0)
                        MarkNodes(node.Nodes);
                }
            }
            MarkNodes(treeView.Nodes);
        }

        private void SetReadOnly(TreeView treeView, bool readOnly)
        {
            if (checkIfOnlyParentExists(readOnly))
            {
                AddWarningLog("No checkpoints selected to set");
                return;
            }


            var selectedNodes = GetCheckedCheckpoints(treeView);
            var selectedProjectNodes = GetCheckedCheckpoints(treeViewProject);



            if (selectedNodes.Count == 0)
            {
                if (selectedProjectNodes.Count != 0)
                {
                    btnAddCheckpoints_Click(null, null);
                    CheckAllNodes(treeView);

                    selectedNodes = GetCheckedCheckpoints(treeViewLive);
                    goto setLogic;
                }
                AddWarningLog("No checkpoints selected to set");
                return;
            }

        setLogic:

            foreach (var node in selectedNodes)
            {
                if (node.Tag is string path && File.Exists(path))
                {
                    try
                    {
                        var attr = File.GetAttributes(path);
                        if (readOnly)
                        {
                            File.SetAttributes(path, attr | FileAttributes.ReadOnly);
                            node.ForeColor = Color.FromArgb(255, 15, 45, 75);
                            if (!readOnlyPaths.Contains(path))
                                readOnlyPaths.Add(path);
                        }
                        else
                        {
                            File.SetAttributes(path, attr & ~FileAttributes.ReadOnly);
                            node.ForeColor = Color.White;
                            readOnlyPaths.Remove(path);
                        }
                    }
                    catch
                    {
                        AddErrorLog($"No authorization for: {path}");
                    }
                }
            }
            btnLoadLiveSaves_Click(null, null);
            UncheckAllNodes(treeViewLive);
            UncheckAllNodes(treeViewProject);
            SaveReadOnlyList(); // Speichere die Liste nach Änderung

            if (readOnly)
            {
                AddLog("Set ReadOnly");
            }
            else
            {
                AddLog("Removed ReadOnly");
            }
        }

        private bool checkIfOnlyParentExists(bool value)
        {
            if (value)
            {
                var selectedNodes = GetCheckedCheckpoints(treeViewLive);
                var selectedNodes2 = GetCheckedCheckpoints(treeViewProject);

                if (selectedNodes.Count == 1 && selectedNodes2.Count > 0)
                {
                    UncheckAllNodes(treeViewLive);
                    selectedNodes = GetCheckedCheckpoints(treeViewLive);
                    return false;
                }

                if (selectedNodes.Count == 1 && selectedNodes[0].Text == "SaveData" && selectedNodes2.Count == 0)
                {
                    UncheckAllNodes(treeViewLive);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                var selectedNodes = GetCheckedCheckpoints(treeViewLive);

                if (selectedNodes.Count == 1 && selectedNodes[0].Text == "SaveData")
                {
                    UncheckAllNodes(treeViewLive);
                    UncheckAllNodes(treeViewProject);
                    return true;
                }
                else
                {
                    if (selectedNodes.Count != 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }

        public void AddLog(string text)
        {
            consoleArea.ForeColor = Color.FromArgb(100, 97, 209, 125);
            text = "" + text;
            consoleArea.AppendText(text + Environment.NewLine);
            consoleArea.SelectionStart = consoleArea.Text.Length;
            consoleArea.ScrollToCaret();
        }

        private void AddInfoLog(string text)
        {
            consoleArea.ForeColor = Color.FromArgb(100, 180, 240, 190);
            text = "" + text;
            consoleArea.AppendText(text + Environment.NewLine);
            consoleArea.SelectionStart = consoleArea.Text.Length;
            consoleArea.ScrollToCaret();
        }
        private void AddCredits(string text)
        {
            consoleArea.ForeColor = Color.FromArgb(255, 150, 150, 150);
            text = "" + text;
            consoleArea.AppendText(text + Environment.NewLine);
            consoleArea.SelectionStart = consoleArea.Text.Length;
            consoleArea.ScrollToCaret();
        }
        private void AddErrorLog(string text)
        {
            consoleArea.ForeColor = Color.FromArgb(255, 254, 43, 44);
            text = "\n" + text + "\n";
            consoleArea.AppendText(text + Environment.NewLine);
            consoleArea.SelectionStart = consoleArea.Text.Length;
            consoleArea.ScrollToCaret();
        }
        private void AddWarningLog(string text)
        {
            consoleArea.ForeColor = Color.FromArgb(255, 237, 190, 94);
            text = "" + text;
            consoleArea.AppendText(text + Environment.NewLine);
            consoleArea.SelectionStart = consoleArea.Text.Length;
            consoleArea.ScrollToCaret();
        }

        private void SetReadOnlyRecursive(TreeNode node, bool readOnly)
        {
            if (node.Tag is string path && File.Exists(path))
            {
                try
                {
                    var attr = File.GetAttributes(path);
                    if (readOnly)
                    {
                        File.SetAttributes(path, attr | FileAttributes.ReadOnly);
                    }
                    else
                    {
                        File.SetAttributes(path, attr & ~FileAttributes.ReadOnly);
                    }
                }
                catch
                {
                    AddErrorLog($"No authorization for: {path}");
                }
            }

            foreach (TreeNode child in node.Nodes)
                SetReadOnlyRecursive(child, readOnly);
        }

        private void btnSetReadOnly_Click(object sender, EventArgs e) => SetReadOnly(treeViewLive, true);
        private void btnRemoveReadOnly_Click(object sender, EventArgs e) => SetReadOnly(treeViewLive, false);

        private string ExtractCheckpointName(string filePath)
        {
            try
            {
                byte[] data = new byte[512];
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    fs.Read(data, 0, data.Length);

                string text = Encoding.UTF8.GetString(data);
                var words = text.Split(new char[] { '\0', ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var w in words)
                    if (w.Length > 2 && w.All(c => char.IsLetterOrDigit(c) || c == '_'))
                        return w;
            }
            catch { }
            return Path.GetFileName(filePath);
        }

        private void EnsureSaveFolderAccess()
        {
            try
            {
                if (Directory.Exists(gameSaveFolder))
                {
                    string testFile = Path.Combine(gameSaveFolder, "test.tmp");
                    File.WriteAllText(testFile, "test");
                    File.Delete(testFile);
                }
            }
            catch
            {
                AddErrorLog("No access to the save folder.\nHave you done the tutorial?\nRewatch the tutorial in the settings if so...");
            }
        }

        // -------------------------
        // Add / Remove Checkpoints
        // -------------------------
        private void btnAddCheckpoints_Click(object sender, EventArgs e)
        {
            var selectedNodes = GetCheckedCheckpoints(treeViewProject);
            if (selectedNodes.Count == 0)
            {
                AddWarningLog("No checkpoints selected to add");
                return;
            }

            if (string.IsNullOrEmpty(gameSaveFolder) || !Directory.Exists(gameSaveFolder))
            {
                AddErrorLog("Save-Folder from game not found. Please start the game.");
                return;
            }

            foreach (var node in selectedNodes)
            {
                string sourceFile = node.Tag as string;
                if (string.IsNullOrEmpty(sourceFile) || !File.Exists(sourceFile))
                    continue;

                string destFile = Path.Combine(gameSaveFolder, Path.GetFileName(sourceFile));

                try
                {
                    File.Copy(sourceFile, destFile, true);

                    bool exists = false;
                    foreach (TreeNode liveNode in treeViewLive.Nodes)
                        if ((liveNode.Tag as string) == destFile) exists = true;

                    if (!exists)
                        treeViewLive.Nodes.Add(new TreeNode(Path.GetFileNameWithoutExtension(destFile)) { Tag = destFile });
                }
                catch (Exception ex)
                {
                    AddErrorLog($"Error while copying {Path.GetFileName(sourceFile)}: {ex.Message}");
                }
            }

            btnLoadLiveSaves_Click(null, null);
            UncheckAllNodes(treeViewLive);
            UncheckAllNodes(treeViewProject);
            AddLog($"Added {selectedNodes.Count} checkpoints");
        }
        private void btnAddCheckpoints_Click2(object sender, EventArgs e)
        {
            var selectedNodes = GetCheckedCheckpoints(treeViewProject);
            if (selectedNodes.Count == 0)
            {
                AddWarningLog("No checkpoints selected to add");
                return;
            }

            if (string.IsNullOrEmpty(gameSaveFolder) || !Directory.Exists(gameSaveFolder))
            {
                AddErrorLog("Save-Folder from game not found. Please start the game.");
                return;
            }

            foreach (var node in selectedNodes)
            {
                string sourceFile = node.Tag as string;
                if (string.IsNullOrEmpty(sourceFile) || !File.Exists(sourceFile))
                    continue;

                string destFile = Path.Combine(gameSaveFolder, Path.GetFileName(sourceFile));

                try
                {
                    File.Copy(sourceFile, destFile, true);

                    bool exists = false;
                    foreach (TreeNode liveNode in treeViewLive.Nodes)
                        if ((liveNode.Tag as string) == destFile) exists = true;

                    if (!exists)
                        treeViewLive.Nodes.Add(new TreeNode(Path.GetFileNameWithoutExtension(destFile)) { Tag = destFile });
                }
                catch (Exception ex)
                {
                    AddErrorLog($"Error while copying {Path.GetFileName(sourceFile)}: {ex.Message}");
                }
            }

            btnLoadLiveSaves_Click(null, null);
            AddLog($"Added {selectedNodes.Count} checkpoints");
        }

        private void CheckAllNodes(TreeView treeView)
        {
            void CheckNodes(TreeNodeCollection nodes)
            {
                foreach (TreeNode node in nodes)
                {
                    node.Checked = true;   // ✅ Haken setzen
                    if (node.Nodes.Count > 0)
                        CheckNodes(node.Nodes); // rekursiv für Kind-Nodes
                }
            }

            CheckNodes(treeView.Nodes);
        }
        private void btnRemoveCheckpoints_Click(object sender, EventArgs e)
        {
            if (checkIfOnlyParentExists(false))
            {
                AddWarningLog("No checkpoints selected to remove");
                return;
            }

            var selectedNodes = GetCheckedCheckpoints(treeViewLive);

            if (selectedNodes.Count == 0)
            {
                AddWarningLog("No checkpoints selected to remove");
                return;
            }

            if (string.IsNullOrEmpty(gameSaveFolder) || !Directory.Exists(gameSaveFolder))
            {
                AddErrorLog("Save-Folder from game not found. Please start the game.");
                return;
            }

            // Prüfen, ob unter den Ausgewählten ReadOnly-Dateien sind
            bool hasReadOnly = selectedNodes.Any(node =>
            {
                if (node.Nodes.Count > 0) return false; // ✅ Parent überspringen
                string filePath = node.Tag as string;
                return !string.IsNullOrEmpty(filePath) &&
                       File.Exists(filePath) &&
                       (File.GetAttributes(filePath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
            });

            bool allReadOnly = selectedNodes
                .Where(node => node.Nodes.Count == 0) // ✅ nur Child-Nodes berücksichtigen
                .All(node =>
                {
                    string filePath = node.Tag as string;
                    return !string.IsNullOrEmpty(filePath) &&
                         File.Exists(filePath) &&
                           (File.GetAttributes(filePath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
                });

            bool deleteReadOnly = true;

            if (hasReadOnly && !allReadOnly)
            {
                var result = MessageBox.Show(
                    "Do you want to delete ReadOnly checkpoints too?",
                    "Confirm",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                deleteReadOnly = (result == DialogResult.Yes);
            }

            int deleted = 0;
            int skippedReadOnly = 0;

            foreach (TreeNode node in selectedNodes)
            {
                string filePath = node.Tag as string;
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                    continue;

                try
                {
                    var attrs = File.GetAttributes(filePath);
                    bool isRO = (attrs & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;

                    // Wenn Nein: ReadOnly-Dateien überspringen (nicht ändern, nicht löschen)
                    if (isRO && !deleteReadOnly)
                    {
                        skippedReadOnly++;
                        continue;
                    }

                    // Wenn Ja: ReadOnly-Attribut entfernen, dann löschen
                    if (isRO && deleteReadOnly)
                    {
                        File.SetAttributes(filePath, attrs & ~FileAttributes.ReadOnly);
                    }

                    File.Delete(filePath);
                    node.Remove(); // entfernt den Node unabhängig davon, ob Root oder Child
                    deleted++;

                    if (readOnlyPaths.Contains(filePath))
                    {
                        readOnlyPaths.Remove(filePath);
                        SaveReadOnlyList(); // 💾 Liste sofort aktualisieren
                        //AddLog($"Removed {Path.GetFileName(filePath)} from ReadOnly list.");
                    }

                }
                catch (Exception ex)
                {
                    AddErrorLog($"\nError while deleting {Path.GetFileName(filePath)}: {ex.Message}");
                }
            }

            UncheckAllNodes(treeViewLive);
            UncheckAllNodes(treeViewProject);
            if (skippedReadOnly == 0)
            {
                AddLog($"Removed {deleted} checkpoints from live saves.");
            }
            else
            {
                AddLog($"Removed {deleted} checkpoints from live saves.\nSkipped ReadOnly: {skippedReadOnly}.");
            }
        }

        // -------------------------
        // TreeView Check Handling
        // -------------------------
        private List<TreeNode> GetCheckedCheckpoints(TreeView tree)
        {
            List<TreeNode> checkedNodes = new List<TreeNode>();

            void Traverse(TreeNodeCollection nodes)
            {
                foreach (TreeNode node in nodes)
                {
                    if (node.Checked)
                        checkedNodes.Add(node);

                    if (node.Nodes.Count > 0)
                        Traverse(node.Nodes);
                }
            }

            Traverse(tree.Nodes);
            return checkedNodes;
        }
        private List<TreeNode> GetAllCheckpoints(TreeView tree)
        {
            List<TreeNode> checkedNodes = new List<TreeNode>();

            void Traverse(TreeNodeCollection nodes)
            {
                foreach (TreeNode node in nodes)
                {
                    checkedNodes.Add(node);

                    if (node.Nodes.Count > 0)
                        Traverse(node.Nodes);
                }
            }

            Traverse(tree.Nodes);
            return checkedNodes;
        }

        private void treeViewProject_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (_updatingChecks) return;
            _updatingChecks = true;
            try
            {
                SetChildrenChecked(e.Node, e.Node.Checked);
                UpdateParents(e.Node);
            }
            finally { _updatingChecks = false; }
        }

        private void SetChildrenChecked(TreeNode node, bool isChecked)
        {
            foreach (TreeNode child in node.Nodes)
            {
                child.Checked = isChecked;
                SetChildrenChecked(child, isChecked);
            }
        }

        private void UpdateParents(TreeNode node)
        {
            TreeNode parent = node.Parent;
            while (parent != null)
            {
                parent.Checked = parent.Nodes.Cast<TreeNode>().All(n => n.Checked);
                parent = parent.Parent;
            }
        }

        private void treeViewLive_AfterCheck(object sender, TreeViewEventArgs e)
        {
            TreeView tree = sender as TreeView;
            tree.AfterCheck -= treeViewLive_AfterCheck;
            try
            {
                void CheckAllChildren(TreeNode node, bool isChecked)
                {
                    foreach (TreeNode child in node.Nodes)
                    {
                        if (child.Checked != isChecked)
                            child.Checked = isChecked;
                        if (child.Nodes.Count > 0)
                            CheckAllChildren(child, isChecked);
                    }
                }
                CheckAllChildren(e.Node, e.Node.Checked);

                void UpdateParent(TreeNode node)
                {
                    if (node.Parent != null)
                    {
                        node.Parent.Checked = node.Parent.Nodes.Cast<TreeNode>().All(n => n.Checked);
                        UpdateParent(node.Parent);
                    }
                }
                UpdateParent(e.Node);
            }
            finally { tree.AfterCheck += treeViewLive_AfterCheck; }
        }

        private void Tree_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            if (_updatingChecks) { e.Cancel = true; return; }

            e.Cancel = true;
            _updatingChecks = true;
            var tree = (TreeView)sender;
            tree.BeginUpdate();
            try
            {
                bool newState = !e.Node.Checked;
                SetChildrenChecked(e.Node, newState);
                UpdateParents(e.Node);
            }
            finally
            {
                tree.EndUpdate();
                _updatingChecks = false;
            }
        }

        // ------------
        // ButtonDesign
        // ------------



        private void btnRemoveChkColorSwitch(object sender, EventArgs e)
        {
            btnRemoveChk.ForeColor = Color.PaleGreen;
        }

        private void btnRemoveChkColorSwitchBack(object sender, EventArgs e)
        {
            btnRemoveChk.ForeColor = Color.White;
        }


        private void panelBtnRemoveChk_MouseEnter(object sender, EventArgs e)
        {
            btnRemoveChk.Paint -= panelBtnRemoveChk_Paint;
            btnRemoveChk.Paint += panelBtnRemoveChk_Paint2;
        }
        private void panelBtnRemoveChk_MouseLeave(object sender, EventArgs e)
        {
            btnRemoveChk.Paint += panelBtnRemoveChk_Paint;
            btnRemoveChk.Paint -= panelBtnRemoveChk_Paint2;
        }

        private void btnSet_MouseEnter(object sender, EventArgs e)
        {
            btnSetRO.ForeColor = Color.White;
            panelBtnSetRO.BackgroundImage = Properties.Resources.Frame12;

        }

        private void btnSet_MouseLeave(object sender, EventArgs e)
        {
            btnSetRO.ForeColor = Color.Black;
            panelBtnSetRO.BackgroundImage = Properties.Resources.Frame11;
        }

        private void btnAdd_MouseEnter(object sender, EventArgs e)
        {
            btnAdd.ForeColor = Color.White;
            panelBtnAdd.BackgroundImage = Properties.Resources.Frame12;
        }

        private void btnAdd_MouseLeave(object sender, EventArgs e)
        {
            btnAdd.ForeColor = Color.Black;
            panelBtnAdd.BackgroundImage = Properties.Resources.Frame11;
        }

        private void btnRemoveRO_MouseEnter(object sender, EventArgs e)
        {
            btnRemoveRO.Paint -= panelBtnRemoveChk_Paint;
            btnRemoveRO.Paint += panelBtnRemoveChk_Paint2;
        }

        private void btnRemoveRO_MouseLeave(object sender, EventArgs e)
        {
            btnRemoveRO.Paint += panelBtnRemoveChk_Paint;
            btnRemoveRO.Paint -= panelBtnRemoveChk_Paint2;
        }

        private void help_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (helpInstance == null || helpInstance.IsDisposed)
            {
                helpInstance = new help();
                helpInstance.Show();

                helpInstance.FormClosed += (s, args) => helpInstance = null;
            }
            else
            {
                // Falls Form schon existiert, einfach in den Vordergrund holen
                helpInstance.BringToFront();
            }
        }

        private void easteregg(object sender, EventArgs e)
        {
            MessageBox.Show("Random facts: Estimates suggest that annually more people die from cows than from shark attacks.");
        }

        private void credit_MouseClick(object sender, LinkLabelLinkClickedEventArgs e)
        {
            consoleArea.ForeColor = Color.DarkGray;
            AddCredits("\nDesign concept by \"Jative Design\" ");
            AddCredits("Assistance with scripting \"Rafy\"");
            AddCredits("BorderlessWindow done by \"lanylow\"\n");
        }

        private void settings(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (settingsInstance == null || settingsInstance.IsDisposed)
            {
                // Neue Instanz erstellen
                settingsInstance = new settings();

                // Eventhandler für FormClosed, damit Instance zurückgesetzt wird
                settingsInstance.FormClosed += (s, args) => settingsInstance = null;

                settingsInstance.Show();
            }
            else
            {
                // Fenster existiert schon → nur hervorheben
                if (settingsInstance.WindowState == FormWindowState.Minimized)
                    settingsInstance.WindowState = FormWindowState.Normal;

                settingsInstance.BringToFront();
                settingsInstance.Activate();
            }
        }

        private void buymeacoffee(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (coffeeInstance == null || coffeeInstance.IsDisposed)
            {
                coffeeInstance = new coffee();
                coffeeInstance.Show();

                coffeeInstance.FormClosed += (s, args) => coffeeInstance = null;
            }
            else
            {
                // Falls Form schon existiert, einfach in den Vordergrund holen
                coffeeInstance.BringToFront();
            }
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // Hotkeys hier erneut sicher registrieren
            //RegisterHotKey(this.Handle, HOTKEY_ID_CTRL_M, KeyModifiers.Shift, Keys.M);
            //RegisterHotKey(this.Handle, HOTKEY_ID_CTRL_N, KeyModifiers.Shift, Keys.N);
            //RegisterHotKey(this.Handle, HOTKEY_ID_CTRL_B, KeyModifiers.Shift, Keys.B);
            //RegisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_1, KeyModifiers.Shift | KeyModifiers.Alt, Keys.D1);
            //RegisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_2, KeyModifiers.Shift | KeyModifiers.Alt, Keys.D2);
            //RegisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_3, KeyModifiers.Shift | KeyModifiers.Alt, Keys.D3);
            //RegisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_4, KeyModifiers.Shift | KeyModifiers.Alt, Keys.D4);
            //RegisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_5, KeyModifiers.Shift | KeyModifiers.Alt, Keys.D5);
            //RegisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_6, KeyModifiers.Shift | KeyModifiers.Alt, Keys.D6);
            //RegisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_7, KeyModifiers.Shift | KeyModifiers.Alt, Keys.D7);
            //RegisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_8, KeyModifiers.Shift | KeyModifiers.Alt, Keys.D8);
            //RegisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_9, KeyModifiers.Shift | KeyModifiers.Alt, Keys.D9);
            //RegisterHotKey(this.Handle, HOTKEY_ID_CTRL_1, KeyModifiers.Shift, Keys.D1);
            //RegisterHotKey(this.Handle, HOTKEY_ID_CTRL_2, KeyModifiers.Shift, Keys.D2);
            //RegisterHotKey(this.Handle, HOTKEY_ID_CTRL_3, KeyModifiers.Shift, Keys.D3);
            //RegisterHotKey(this.Handle, HOTKEY_ID_CTRL_4, KeyModifiers.Shift, Keys.D4);
            //RegisterHotKey(this.Handle, HOTKEY_ID_CTRL_5, KeyModifiers.Shift, Keys.D5);
            //RegisterHotKey(this.Handle, HOTKEY_ID_CTRL_6, KeyModifiers.Shift, Keys.D6);
            //RegisterHotKey(this.Handle, HOTKEY_ID_CTRL_7, KeyModifiers.Shift, Keys.D7);
            //RegisterHotKey(this.Handle, HOTKEY_ID_CTRL_8, KeyModifiers.Shift, Keys.D8);
            //RegisterHotKey(this.Handle, HOTKEY_ID_CTRL_9, KeyModifiers.Shift, Keys.D9);
            //RegisterHotKey(this.Handle, HOTKEY_ID_ALT_Shift_V, KeyModifiers.Shift, Keys.V);
            //RegisterHotKey(this.Handle, HOTKEY_ID_ALT_Shift_G, KeyModifiers.Shift, Keys.G);
            //RegisterHotKey(this.Handle, HOTKEY_ID_ALT_Shift_T, KeyModifiers.Shift, Keys.T);
            //RegisterHotKey(this.Handle, HOTKEY_ID_ALT_Shift_F1, KeyModifiers.Shift, Keys.F1);
            //RegisterHotKey(this.Handle, HOTKEY_ID_ALT_Shift_F2, KeyModifiers.Shift, Keys.F2);
            //RegisterHotKey(this.Handle, HOTKEY_ID_ALT_Shift_F3, KeyModifiers.Shift, Keys.F3);
            //RegisterHotKey(this.Handle, HOTKEY_ID_ALT_Shift_F4, KeyModifiers.Shift, Keys.F4);
            
            //RegisterHotKey(this.Handle, HOTKEY_ID_ALT_F1, KeyModifiers.Shift, Keys.F1);
            //RegisterHotKey(this.Handle, HOTKEY_ID_ALT_F2, KeyModifiers.Shift, Keys.F2);
            //RegisterHotKey(this.Handle, HOTKEY_ID_ALT_F3, KeyModifiers.Shift, Keys.F3);
            //RegisterHotKey(this.Handle, HOTKEY_ID_ALT_F4, KeyModifiers.Shift, Keys.F4);
            //            RegisterHotKey(this.Handle, HOTKEY_ID_ALT_Shift_T, KeyModifiers.Shift,Keys.F);
        }
        
        protected override void OnHandleDestroyed(EventArgs e)
        {
            // Hotkeys wieder sauber freigeben
            UnregisterHotKey(this.Handle, HOTKEY_Menu);
            UnregisterHotKey(this.Handle, HOTKEY_DeleteExcept);
            UnregisterHotKey(this.Handle, HOTKEY_AddSelected);
            UnregisterHotKey(this.Handle, HOTKEY_LoadPreset_1);
            UnregisterHotKey(this.Handle, HOTKEY_LoadPreset_2);
            UnregisterHotKey(this.Handle, HOTKEY_LoadPreset_3);
            UnregisterHotKey(this.Handle, HOTKEY_LoadPreset_4);
            UnregisterHotKey(this.Handle, HOTKEY_LoadPreset_5);
            UnregisterHotKey(this.Handle, HOTKEY_LoadPreset_6);
            UnregisterHotKey(this.Handle, HOTKEY_LoadPreset_7);
            UnregisterHotKey(this.Handle, HOTKEY_LoadPreset_8);
            UnregisterHotKey(this.Handle, HOTKEY_LoadPreset_9);
            UnregisterHotKey(this.Handle, HOTKEY_SavePreset_1);
            UnregisterHotKey(this.Handle, HOTKEY_SavePreset_2);
            UnregisterHotKey(this.Handle, HOTKEY_SavePreset_3);
            UnregisterHotKey(this.Handle, HOTKEY_SavePreset_4);
            UnregisterHotKey(this.Handle, HOTKEY_SavePreset_5);
            UnregisterHotKey(this.Handle, HOTKEY_SavePreset_6);
            UnregisterHotKey(this.Handle, HOTKEY_SavePreset_7);
            UnregisterHotKey(this.Handle, HOTKEY_SavePreset_8);
            UnregisterHotKey(this.Handle, HOTKEY_SavePreset_9);
            UnregisterHotKey(this.Handle, HOTKEY_InfiniteNightVision);
            UnregisterHotKey(this.Handle, HOTKEY_FreezeEnemy);
            UnregisterHotKey(this.Handle, HOTKEY_NoDamage);
            UnregisterHotKey(this.Handle, HOTKEY_Freecam);

            base.OnHandleDestroyed(e);
        }

        private void labelHours_Click(object sender, EventArgs e)
        {
            using (Form inputForm = new Form())
            {
                inputForm.Text = "Edit Playtime";
                inputForm.TopMost = true;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.StartPosition = FormStartPosition.CenterParent;
                inputForm.Width = 300;
                inputForm.Height = 150;
                inputForm.MaximizeBox = false;
                inputForm.MinimizeBox = false;

                Label lbl = new Label() { Text = "Enter new playtime in hours (e.g. 2000.1):", AutoSize = false, Width = 280, Height = 40, Location = new Point(10, 10) };
                TextBox txt = new TextBox() { Text = playtimeHours.ToString("F1"), Width = 260, Location = new Point(10, 50) };

                Button btnOk = new Button() { Text = "OK", DialogResult = DialogResult.OK, Location = new Point(70, 80) };
                Button btnCancel = new Button() { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(150, 80) };

                inputForm.Controls.Add(lbl);
                inputForm.Controls.Add(txt);
                inputForm.Controls.Add(btnOk);
                inputForm.Controls.Add(btnCancel);

                inputForm.AcceptButton = btnOk;
                inputForm.CancelButton = btnCancel;

                if (inputForm.ShowDialog() == DialogResult.OK)
                {
                    if (double.TryParse(txt.Text, out double hours))
                    {
                        if (hours < 0) hours = 0; // prevent negative
                        playtimeHours = hours;
                        SavePlaytime();
                        labelHours.Text = playtimeHours.ToString("F1");
                    }
                    else
                    {
                        MessageBox.Show("Invalid format. Please enter hours as a number (e.g. 2000.1).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void boot_Clicked(object sender, EventArgs e)
        {
            if (Directory.Exists(ordnerPfad))
            {
                var result = MessageBox.Show(
    $"Do you want to start all files in the Boot folder?",
    "Start Boot Files",
    MessageBoxButtons.YesNo,
    MessageBoxIcon.Question,
    MessageBoxDefaultButton.Button1,
    MessageBoxOptions.ServiceNotification
);

                if (result == DialogResult.Yes)
                {
                    bootFolders();
                }
            }

        }

        private void mods_Clicked(object sender, EventArgs e)
        {
            if (modsInstance == null || modsInstance.IsDisposed)
            {
                // Übergibt die aktuelle Manager-Instanz an mods
                modsInstance = new mods(this);
                modsInstance.Show();

                modsInstance.FormClosed += (s, args) => modsInstance = null;
            }
            else
            {
                modsInstance.BringToFront();
            }
        }

        public void freezeEnemy()
        {
            enemyToggle = !enemyToggle;

            if (prop.Default.externalModPackage)
            {

                if (checkBit64())
                {
                    if (Process.GetProcessesByName("EnemyToggle64") != null)
                    {
                        var proc = Process.GetProcessesByName("EnemyToggle64");
                        foreach (var p in proc)
                        {
                            p.Kill();
                        }
                    }
                    try
                    {
                        ProcessStartInfo psi = new ProcessStartInfo()
                        {
                            FileName = Path.Combine(Directory.GetCurrentDirectory(), "SaveManager-Mods", "EnemyToggle64.exe"),
                            Arguments = enemyToggle.ToString(),
                            UseShellExecute = false, // false, damit man z.B. Standardausgabe lesen kann
                            CreateNoWindow = true   // true, wenn kein Fenster aufgehen soll
                        };

                        Process process = Process.Start(psi);
                        AddLog($"Enemy toggled to: {enemyToggle}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }

                }
                else
                {
                    if (Process.GetProcessesByName("EnemyToggle32") != null)
                    {
                        var proc = Process.GetProcessesByName("EnemyToggle32");
                        foreach (var p in proc)
                        {
                            p.Kill();
                        }
                    }
                    try
                    {
                        ProcessStartInfo psi = new ProcessStartInfo()
                        {
                            FileName = Path.Combine(Directory.GetCurrentDirectory(), "SaveManager-Mods", "EnemyToggle32.exe"),
                            Arguments = enemyToggle.ToString(),
                            UseShellExecute = false, // false, damit man z.B. Standardausgabe lesen kann
                            CreateNoWindow = true   // true, wenn kein Fenster aufgehen soll
                        };

                        Process process = Process.Start(psi);
                        AddLog($"Enemy toggled to: {enemyToggle}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
                if (prop.Default.borderless)
                {
                    if (enemyToggle)
                    {
                        ShowOverlay("FreezeEnemy", "FreezeEnemy");
                    }
                    else
                    {
                        HideOverlay("FreezeEnemy");
                    }
                }
            }
            else
            {
                if (enemyToggle)
                {
                    ShowOverlay("FreezeEnemy", "FreezeEnemy");
                }
                else
                {
                    HideOverlay("FreezeEnemy");
                }
                File.WriteAllText(commandsCFG, $"FreezeEnemy {enemyToggle.ToString().ToLower()}");
                forehook();
                SimulateKey();
            }
            
        }

        private void info3_Click(object sender, EventArgs e)
        {
            consoleArea.Clear();
            AddCredits("Here you can add checkpoints into your game by checking them\nin the [Checkpoint Pod]!\nThis button is only beeing used by [Checkpoint Pod]\nand from the [Shift] + [B] hotkey\ncheck hotkey icon do see all hotkeys");
        }


        private void info1_Click(object sender, EventArgs e)
        {
            consoleArea.Clear();
            AddCredits("Here you can select Checkpoints (or SaveData to select every Checkpoint)\nand then press these buttons below.\nPress the info icon on the buttons to know what they are doing");
        }

        private void info2_Click(object sender, EventArgs e)
        {
            consoleArea.Clear();
            AddCredits("Here you can select checkpoints (or the parent to select entire chapter/game)\nand add them with the button [Set Checkpoint] or either [Set ReadOnly]\n to add them (and set readonly too)\nPress the icon on the buttons to know what they are doing");
        }

        private void info4_Click(object sender, EventArgs e)
        {
            consoleArea.Clear();
            AddCredits("Here you can delete checkpoints from your game\nby checking them in the [Loaded Checkpoint]\nThis button is only beeing used by [Loaded Checkpoint]");

        }

        private void info5_Click(object sender, EventArgs e)
        {
            consoleArea.Clear();
            AddCredits("Here you can check checkpoints in the [Loaded Checkpoints] section\nand with this you cant delete checkpoints ingame\nit prevents automatic deletion\nThis button is only beeing used by [Loaded Checkpoints]");
        }

        private void info6_Click(object sender, EventArgs e)
        {
            consoleArea.Clear();
            AddCredits("Here you can check checkpoints in the [Loaded Checkpoints] section\nand with this you can delete checkpoints ingame again\nThis button is only beeing used by [Loaded Checkpoints]");

        }

        private void info7_Click(object sender, EventArgs e)
        {
            consoleArea.Clear();
            AddCredits("By clicking on the GameHours label, you can edit your gamehours\nIt will automaticaly track your gamehours btw.");
        }

        private void info8_Click(object sender, EventArgs e)
        {
            consoleArea.Clear();
            AddCredits("Here you got some mods I have created like freezing enemies,\nchanging hitbox size or infinite nightvision\nYou might wanna hover them, some you can toggle by a hotkey");
        }

        private void info9_Click(object sender, EventArgs e)
        {
            consoleArea.Clear();
            AddCredits("If you click here it asks you to boot the [Boot] folder\nwhich should be now located in the outlast root directory\nLets say you practised and want to speedrun\nby doing this it starts every program you need\nlike obs,livesplit,nohboard...\nmake sure to create .ink files of them, which are simple shortcuts\nto create shortcuts, go on the program you want to put in the folder\nright click it and create shortcut, now drag and drop it");

        }

        private void info10_Click(object sender, EventArgs e)
        {
            consoleArea.Clear();
            AddCredits("Here you can change some settings\nhover over them to see what they do\nHere you can also disable this info icons everywhere...");
        }

        private void info11_Click(object sender, EventArgs e)
        {
            consoleArea.Clear();
            AddCredits("Since the SaveManager cost me a lot of time\n(around 20+ hours and 4.000-5.000 lines of code)\nI would really appreciate even 1€ for a daily coffee");

        }

        private void info12_Click(object sender, EventArgs e)
        {
            consoleArea.Clear();
            AddCredits("Displays credits in the console");
        }

        private void info13_Click(object sender, EventArgs e)
        {
            consoleArea.Clear();
            AddCredits("If for some reason you need troubleshooting\nlike savemanager not working (press activate permission), autoupdate fails\nyou found a bug or just need further help/questions\nyou can just contact me here or watch a guide...");

        }

        private void info14_Click(object sender, EventArgs e)
        {
            consoleArea.Clear();
            AddCredits("Hotkeys:\n[Shift] + [B] - adds every checkpoint if nothing is selected, if some are selected it will only add those\n[Shift] + [N] - deletes every checkpoint except readonly's\n[Shift] + [V] - Toggles infinite nightvision\n[Shift] + [G] - toggles freeze enemy\n[Shift] + [ALT] + [1-9] - stores selected checkpoints in the pod as a preset\n[Shift] + [1-9] - loads and add the checkpoints in the preset\n[Shift] + [T] - activates nodamage\n");

        }

        private void info15_Click(object sender, EventArgs e)
        {
            consoleArea.Clear();
            AddCredits("Green = Successful\nYellow = Warning\nRed = Error\nI guess its pretty much self explaining");

        }

        public void tptofreecam()
        {
            if (checkBit64())
            {
                ProcessStartInfo psi = new ProcessStartInfo()
                {
                    FileName = Path.Combine(Directory.GetCurrentDirectory(), "SaveManager-Mods", "tptofreecam64.exe"),
                    Arguments = enemyToggle.ToString(),
                    UseShellExecute = false, // false, damit man z.B. Standardausgabe lesen kann
                    CreateNoWindow = true   // true, wenn kein Fenster aufgehen soll
                };

                Process process = Process.Start(psi);
            }
            else
            {
                ProcessStartInfo psi = new ProcessStartInfo()
                {
                    FileName = Path.Combine(Directory.GetCurrentDirectory(), "SaveManager-Mods", "tptofreecam32.exe"),
                    Arguments = enemyToggle.ToString(),
                    UseShellExecute = false, // false, damit man z.B. Standardausgabe lesen kann
                    CreateNoWindow = true   // true, wenn kein Fenster aufgehen soll
                };

                Process process = Process.Start(psi);
            }
        }

        public void hitboxnormal()
        {
            File.WriteAllText(commandsCFG, "SetPlayerCollisionType CT_Normal");
            forehook();
            SimulateKey();
            nopping = "nop";
            nopHitbox();

            hitboxnormalBool = true;

            HideOverlay("VaultHitbox");
            HideOverlay("DoorHitbox");
            HideOverlay("ShimmyHitbox");
            
        }
        public void hiboxvault()
        {
            File.WriteAllText(commandsCFG, "SetPlayerCollisionType CT_Vault");
            forehook();
            SimulateKey();
            nopHitbox();
            hitboxnormalBool = false;

            HideOverlay("DoorHitbox");
            HideOverlay("ShimmyHitbox");
            ShowOverlay("VaultHitbox","VaultHitbox");
        }
        public void hitboxdoor()
        {
            File.WriteAllText(commandsCFG, "SetPlayerCollisionType CT_Door");
            forehook();
            SimulateKey();
            nopHitbox();
            hitboxnormalBool = false;

            HideOverlay("VaultHitbox");
            HideOverlay("ShimmyHitbox");
            ShowOverlay("DoorHitbox", "DoorHitbox");
        }
        public void hitboxshimmy()
        {
            File.WriteAllText(commandsCFG, "SetPlayerCollisionType CT_Shimmy");
            forehook();
            SimulateKey();
            nopHitbox();
            
            hitboxnormalBool = false;

            HideOverlay("DoorHitbox");
            HideOverlay("VaultHitbox");
            ShowOverlay("ShimmyHitbox", "ShimmyHitbox");
        }
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        Process[] procs = Process.GetProcessesByName("OLGame"); // Outlast heißt "OLGame.exe"
        private void forehook()
        {
            if (procs.Length > 0)
            {
                SetForegroundWindow(procs[0].MainWindowHandle);
            }
        }

        private void nopHitbox()
        {
            if (prop.Default.hitboxChange)
            {
                if (nopping == "nop")
                {
                    nopping = "unnop";
                }
                else
                {
                    nopping = "nop";
                }
                if (checkBit64())
                {
                    ProcessStartInfo psi = new ProcessStartInfo()
                    {
                        FileName = Path.Combine(Directory.GetCurrentDirectory(), "SaveManager-Mods", "NopHitbox64.exe"),
                        UseShellExecute = false, // false, damit man z.B. Standardausgabe lesen kann
                        Arguments = nopping,
                        CreateNoWindow = true   // true, wenn kein Fenster aufgehen soll
                    };

                    Process process = Process.Start(psi);
                }
                else
                {
                    ProcessStartInfo psi = new ProcessStartInfo()
                    {
                        FileName = Path.Combine(Directory.GetCurrentDirectory(), "SaveManager-Mods", "NopHitbox32.exe"),
                        UseShellExecute = false, // false, damit man z.B. Standardausgabe lesen kann
                        Arguments = nopping,
                        CreateNoWindow = true   // true, wenn kein Fenster aufgehen soll
                    };

                    Process process = Process.Start(psi);
                }
            }
        }

        internal void DisableHotkeys()
        {
            if (prop.Default.speedrun)
            {
                UnregisterAllHotkeys();
                foreach (var kv in currentHotkeys)
                {
                    string action = kv.Key;
                    string hk = kv.Value?.Trim();

                    if (string.IsNullOrEmpty(hk)) continue; // skip empty entries (no default!)

                    if (!actionToId.TryGetValue(action, out int id)) continue; // unknown action -> skip

                    if (id != 9000 && id != 9064 && id != 9059 && id != 0x0000 && id != 0x0002)
                    {
                        continue;
                    }
                    var (mods, key) = ParseHotkey(hk);
                    if (key == Keys.None) continue; // invalid parse -> skip

                    try
                    {
                        bool ok = RegisterHotKey(this.Handle, id, mods, key);
                        if (ok)
                            registeredHotkeyIds.Add(id);


                    }
                    catch (Exception ex)
                    {
                        AddErrorLog($"RegisterHotKey failed for {action}: {ex.Message}");
                    }
                    
                }
            }
            else
            {
                UnregisterAllHotkeys();
                RegisterAllHotkeys();
            }
        }
    }


    internal class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true, CallingConvention = System.Runtime.InteropServices.CallingConvention.Winapi)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);
    }
}