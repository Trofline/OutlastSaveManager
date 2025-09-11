using Microsoft.Win32;
using OutlastSaveManager.Properties;
using System.Diagnostics;
using System.Drawing.Drawing2D;
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
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
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
        private const int HOTKEY_ID_CTRL_M = 9000;
        private const int HOTKEY_ID_CTRL_N = 9001;
        private const int HOTKEY_ID_CTRL_B = 9002;
        private const int HOTKEY_ID_CTRL_1 = 9003;
        private const int HOTKEY_ID_CTRL_2 = 9004;
        private const int HOTKEY_ID_CTRL_3 = 9005;
        private const int HOTKEY_ID_CTRL_4 = 9006;
        private const int HOTKEY_ID_CTRL_5 = 9007;
        private const int HOTKEY_ID_CTRL_6 = 9017;
        private const int HOTKEY_ID_CTRL_7 = 9018;
        private const int HOTKEY_ID_CTRL_8 = 9019;
        private const int HOTKEY_ID_CTRL_9 = 9020;
        private const int HOTKEY_ID_ALT_CTRL_1 = 9008;
        private const int HOTKEY_ID_ALT_CTRL_2 = 9009;
        private const int HOTKEY_ID_ALT_CTRL_3 = 9010;
        private const int HOTKEY_ID_ALT_CTRL_4 = 9011;
        private const int HOTKEY_ID_ALT_CTRL_5 = 9012;
        private const int HOTKEY_ID_ALT_CTRL_6 = 9013;
        private const int HOTKEY_ID_ALT_CTRL_7 = 9014;
        private const int HOTKEY_ID_ALT_CTRL_8 = 9015;
        private const int HOTKEY_ID_ALT_CTRL_9 = 9016;
        private const int HOTKEY_ID_ALT_Shift_V = 9021;
        private const int HOTKEY_ID_ALT_Shift_G = 9022;

        private readonly string readOnlyListFile = Path.Combine(SaveDir, "readonly_list.txt");
        private List<string> readOnlyPaths = new List<string>();
        private CancellationTokenSource duplicationFixCts;
        private double playtimeHours = 0; // Spielzeit in Stunden (dezimal)
        private static readonly string SaveDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OutlastSaveManager");
        private string playtimeFile = Path.Combine(SaveDir, "playtime.txt");

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


        // --- Fade-In / Fade-Out ---
        private Timer fadeTimer;
        private bool fadingOut = false;
        private bool fadingIn = false;
        private double fadeStep = 0.05;

        // --- Save-Folder ---
        private string gameSaveFolder = "";
        private string projectSaveFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SaveData");

        // --- TreeView Check-State Management ---
        private bool _updatingChecks = false;


        //------------------------------------------------------------------------------------------------------
        //push and upload on github,update version.txt in github
        //------------------------------------------------------------------------------------------------------

        private static string LocalVersion = "1.6.7";
        private void changelogs()
        {
            AddInfoLog("All keybinds switched from [CTRL] to [Shift]!\nAdded HitBox-Changer & Infinite NightVision with hotkey & freeze enemy with hotkey\nRead the README.md for further information\n");
        }
        public Manager()
        {
            InitializeComponent();
            AttachToProcess("OLGame");
            CheckAndUpdate();


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
            //speedrunhelper(); //TODO
            StartOverlayTimer();
            unloadInfo();
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
                info14.Visible = false;
                info15.Visible = false;
            }
        }

        private Timer overlayTimer;

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

        private void speedrunhelper()
        {
            if (checkBit64())
            {

                var procs64 = Process.GetProcessesByName("speedrunhelper64");

                foreach (var proc in procs64)
                {
                    try
                    {
                        proc.Kill();
                    }
                    catch
                    {
                        // optional: Fehlerbehandlung, falls Prozess nicht gekillt werden kann
                    }
                }

                ProcessStartInfo psi = new ProcessStartInfo()
                {
                    FileName = Path.Combine(Directory.GetCurrentDirectory(), "SaveManager-Mods", "Speedrunhelper64.exe"),
                    UseShellExecute = false, // false, damit man z.B. Standardausgabe lesen kann
                    CreateNoWindow = true   // true, wenn kein Fenster aufgehen soll
                };

                Process process = Process.Start(psi);
            }
            else
            {
                var procs32 = Process.GetProcessesByName("speedrunhelper32");

                foreach (var proc in procs32)
                {
                    try
                    {
                        proc.Kill();
                    }
                    catch
                    {
                        // optional: Fehlerbehandlung, falls Prozess nicht gekillt werden kann
                    }
                }

                ProcessStartInfo psi = new ProcessStartInfo()
                {
                    FileName = Path.Combine(Directory.GetCurrentDirectory(), "SaveManager-Mods", "Speedrunhelper32.exe"),
                    UseShellExecute = false, // false, damit man z.B. Standardausgabe lesen kann
                    CreateNoWindow = true   // true, wenn kein Fenster aufgehen soll
                };

                Process process = Process.Start(psi);


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

            string exePath = Path.Combine(Directory.GetCurrentDirectory(), "RunMe.exe");
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
                    case HOTKEY_ID_CTRL_M:

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

                    case HOTKEY_ID_CTRL_N:
                        this.BeginInvoke((Action)(() => deleteEveryCheckpointExceptReadOnly()));
                        AddLog("Removed checkpoints");
                        break;

                    case HOTKEY_ID_CTRL_B:

                        this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                        AddLog("Added checkpoints");
                        break;
                    case HOTKEY_ID_CTRL_1:

                        this.BeginInvoke((Action)(() => ApplyPreset(treeViewProject, 0)));
                        this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                        AddLog("Loaded Preset 1");
                        break;
                    case HOTKEY_ID_CTRL_2:

                        this.BeginInvoke((Action)(() => ApplyPreset(treeViewProject, 1)));
                        this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                        AddLog("Loaded Preset 2");
                        break;
                    case HOTKEY_ID_CTRL_3:

                        this.BeginInvoke((Action)(() => ApplyPreset(treeViewProject, 2)));
                        this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                        AddLog("Loaded Preset 3");
                        break;
                    case HOTKEY_ID_CTRL_4:

                        this.BeginInvoke((Action)(() => ApplyPreset(treeViewProject, 3)));
                        this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                        AddLog("Loaded Preset 4");
                        break;
                    case HOTKEY_ID_CTRL_5:

                        this.BeginInvoke((Action)(() => ApplyPreset(treeViewProject, 4)));
                        this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                        AddLog("Loaded Preset 5");
                        break;
                    case HOTKEY_ID_CTRL_6:

                        this.BeginInvoke((Action)(() => ApplyPreset(treeViewProject, 5)));
                        this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                        AddLog("Loaded Preset 6");
                        break;
                    case HOTKEY_ID_CTRL_7:

                        this.BeginInvoke((Action)(() => ApplyPreset(treeViewProject, 6)));
                        this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                        AddLog("Loaded Preset 7");
                        break;
                    case HOTKEY_ID_CTRL_8:

                        this.BeginInvoke((Action)(() => ApplyPreset(treeViewProject, 7)));
                        this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                        AddLog("Loaded Preset 8");
                        break;
                    case HOTKEY_ID_CTRL_9:

                        this.BeginInvoke((Action)(() => ApplyPreset(treeViewProject, 8)));
                        this.BeginInvoke((Action)(() => addEveryCheckpoint()));
                        AddLog("Loaded Preset 9");
                        break;
                    case HOTKEY_ID_ALT_CTRL_1:

                        this.BeginInvoke((Action)(() => SaveCurrentTreeAsPreset(treeViewProject, 0)));
                        AddLog("Saved Preset 1");
                        break;
                    case HOTKEY_ID_ALT_CTRL_2:

                        this.BeginInvoke((Action)(() => SaveCurrentTreeAsPreset(treeViewProject, 1)));
                        AddLog("Saved Preset 2");
                        break;
                    case HOTKEY_ID_ALT_CTRL_3:

                        this.BeginInvoke((Action)(() => SaveCurrentTreeAsPreset(treeViewProject, 2)));
                        AddLog("Saved Preset 3");
                        break;
                    case HOTKEY_ID_ALT_CTRL_4:

                        this.BeginInvoke((Action)(() => SaveCurrentTreeAsPreset(treeViewProject, 3)));
                        AddLog("Saved Preset 4");
                        break;
                    case HOTKEY_ID_ALT_CTRL_5:

                        this.BeginInvoke((Action)(() => SaveCurrentTreeAsPreset(treeViewProject, 4)));
                        AddLog("Saved Preset 5");
                        break;
                    case HOTKEY_ID_ALT_CTRL_6:

                        this.BeginInvoke((Action)(() => SaveCurrentTreeAsPreset(treeViewProject, 5)));
                        AddLog("Saved Preset 6");
                        break;
                    case HOTKEY_ID_ALT_CTRL_7:

                        this.BeginInvoke((Action)(() => SaveCurrentTreeAsPreset(treeViewProject, 6)));
                        AddLog("Saved Preset 7");
                        break;
                    case HOTKEY_ID_ALT_CTRL_8:

                        this.BeginInvoke((Action)(() => SaveCurrentTreeAsPreset(treeViewProject, 7)));
                        AddLog("Saved Preset 8");
                        break;
                    case HOTKEY_ID_ALT_CTRL_9:

                        this.BeginInvoke((Action)(() => SaveCurrentTreeAsPreset(treeViewProject, 8)));
                        AddLog("Saved Preset 9");
                        break;
                    case HOTKEY_ID_ALT_Shift_V:
                        nightvision();
                        break;
                    case HOTKEY_ID_ALT_Shift_G:
                        freezeEnemy();
                        break;
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
        private Dictionary<string, Overlay> activeOverlays = new Dictionary<string, Overlay>();

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
                "ShimmyHitbox" => 80,
                "DoorHitbox" => 80,
                "VaultHitbox" => 80,
                _ => 95 // alle anderen Mods //ADD MOD //MOD ADD //TODO
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
                MessageBox.Show($"Process '{processName}' doesn't run.\nPlease boot the game");
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
            var procs32 = Process.GetProcessesByName("speedrunhelper32");

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
            var procs64 = Process.GetProcessesByName("speedrunhelper64");

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
            SavePlaytime();
            SaveReadOnlyList();
            UnregisterHotKey(this.Handle, HOTKEY_ID_CTRL_M);
            UnregisterHotKey(this.Handle, HOTKEY_ID_CTRL_N);
            UnregisterHotKey(this.Handle, HOTKEY_ID_CTRL_B);
            UnregisterHotKey(this.Handle, HOTKEY_ID_CTRL_1);
            UnregisterHotKey(this.Handle, HOTKEY_ID_CTRL_2);
            UnregisterHotKey(this.Handle, HOTKEY_ID_CTRL_3);
            UnregisterHotKey(this.Handle, HOTKEY_ID_CTRL_4);
            UnregisterHotKey(this.Handle, HOTKEY_ID_CTRL_5);
            UnregisterHotKey(this.Handle, HOTKEY_ID_CTRL_6);
            UnregisterHotKey(this.Handle, HOTKEY_ID_CTRL_7);
            UnregisterHotKey(this.Handle, HOTKEY_ID_CTRL_8);
            UnregisterHotKey(this.Handle, HOTKEY_ID_CTRL_9);
            UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_1);
            UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_2);
            UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_3);
            UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_4);
            UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_5);
            UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_6);
            UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_7);
            UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_8);
            UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_9);
            UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_Shift_V);
            UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_Shift_G);
            StopDuplicationFixLoop();

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
            AddCredits("BetterOutlastLauncher done by \"HayaiNeko\"\n");
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
            RegisterHotKey(this.Handle, HOTKEY_ID_CTRL_M, KeyModifiers.Shift, Keys.M);
            RegisterHotKey(this.Handle, HOTKEY_ID_CTRL_N, KeyModifiers.Shift, Keys.N);
            RegisterHotKey(this.Handle, HOTKEY_ID_CTRL_B, KeyModifiers.Shift, Keys.B);
            RegisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_1, KeyModifiers.Shift | KeyModifiers.Alt, Keys.D1);
            RegisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_2, KeyModifiers.Shift | KeyModifiers.Alt, Keys.D2);
            RegisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_3, KeyModifiers.Shift | KeyModifiers.Alt, Keys.D3);
            RegisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_4, KeyModifiers.Shift | KeyModifiers.Alt, Keys.D4);
            RegisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_5, KeyModifiers.Shift | KeyModifiers.Alt, Keys.D5);
            RegisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_6, KeyModifiers.Shift | KeyModifiers.Alt, Keys.D6);
            RegisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_7, KeyModifiers.Shift | KeyModifiers.Alt, Keys.D7);
            RegisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_8, KeyModifiers.Shift | KeyModifiers.Alt, Keys.D8);
            RegisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_9, KeyModifiers.Shift | KeyModifiers.Alt, Keys.D9);
            RegisterHotKey(this.Handle, HOTKEY_ID_CTRL_1, KeyModifiers.Shift, Keys.D1);
            RegisterHotKey(this.Handle, HOTKEY_ID_CTRL_2, KeyModifiers.Shift, Keys.D2);
            RegisterHotKey(this.Handle, HOTKEY_ID_CTRL_3, KeyModifiers.Shift, Keys.D3);
            RegisterHotKey(this.Handle, HOTKEY_ID_CTRL_4, KeyModifiers.Shift, Keys.D4);
            RegisterHotKey(this.Handle, HOTKEY_ID_CTRL_5, KeyModifiers.Shift, Keys.D5);
            RegisterHotKey(this.Handle, HOTKEY_ID_CTRL_6, KeyModifiers.Shift, Keys.D6);
            RegisterHotKey(this.Handle, HOTKEY_ID_CTRL_7, KeyModifiers.Shift, Keys.D7);
            RegisterHotKey(this.Handle, HOTKEY_ID_CTRL_8, KeyModifiers.Shift, Keys.D8);
            RegisterHotKey(this.Handle, HOTKEY_ID_CTRL_9, KeyModifiers.Shift, Keys.D9);
            RegisterHotKey(this.Handle, HOTKEY_ID_ALT_Shift_V, KeyModifiers.Shift, Keys.V);
            RegisterHotKey(this.Handle, HOTKEY_ID_ALT_Shift_G, KeyModifiers.Shift, Keys.G);
        }
        //NEW
        protected override void OnHandleDestroyed(EventArgs e)
        {
            // Hotkeys wieder sauber freigeben
            UnregisterHotKey(this.Handle, HOTKEY_ID_CTRL_M);
            UnregisterHotKey(this.Handle, HOTKEY_ID_CTRL_N);
            UnregisterHotKey(this.Handle, HOTKEY_ID_CTRL_B);
            UnregisterHotKey(this.Handle, HOTKEY_ID_CTRL_1);
            UnregisterHotKey(this.Handle, HOTKEY_ID_CTRL_2);
            UnregisterHotKey(this.Handle, HOTKEY_ID_CTRL_3);
            UnregisterHotKey(this.Handle, HOTKEY_ID_CTRL_4);
            UnregisterHotKey(this.Handle, HOTKEY_ID_CTRL_5);
            UnregisterHotKey(this.Handle, HOTKEY_ID_CTRL_6);
            UnregisterHotKey(this.Handle, HOTKEY_ID_CTRL_7);
            UnregisterHotKey(this.Handle, HOTKEY_ID_CTRL_8);
            UnregisterHotKey(this.Handle, HOTKEY_ID_CTRL_9);
            UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_1);
            UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_2);
            UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_3);
            UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_4);
            UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_5);
            UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_6);
            UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_7);
            UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_8);
            UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_CTRL_9);
            UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_Shift_V);
            UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_Shift_G);


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
            AddCredits("Hotkeys:\n[Shift] + [B] - adds every checkpoint if nothing is selected, if some are selected it will only add those\n[Shift] + [N] - deletes every checkpoint except readonly's\n[Shift] + [V] - Toggles infinite nightvision\n[Shift] + [G] - toggles freeze enemy\n[Shift] + [ALT] + [1-9] - stores selected checkpoints in the pod as a preset\n[Shift] + [1-9] - loads and add the checkpoints in the preset");

        }

        private void info15_Click(object sender, EventArgs e)
        {
            consoleArea.Clear();
            AddCredits("Green = Successful\nYellow = Warning\nRed = Error\nI guess its pretty much self explaining");

        }
    }


    internal class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true, CallingConvention = System.Runtime.InteropServices.CallingConvention.Winapi)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);
    }
}