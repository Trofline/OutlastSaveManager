using OutlastSaveManager.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OutlastSaveManager
{
    public partial class settings : Form
    {
        bool userChecked = false;
        public string bootParameter;
        private Binding bindingInstance;
        private HotkeyManager smhkInstance;
        private Manager manager;
        public settings()
        {
            InitializeComponent();
            this.Icon = Resources.logo;

            // Manager-Instanz initialisieren
            manager = Manager.Instance;

            // Events kurz abmelden, damit CheckedChanged nicht feuert
            minimizeChk.CheckedChanged -= minimizeChk_CheckedChanged;
            duplicationChk.CheckedChanged -= duplicationChk_CheckedChanged;
            bitChk.CheckedChanged -= Bit_CheckedChanged;
            borderlessChk.CheckedChanged -= borderless_CheckedChanged;
            infoChk.CheckedChanged -= infoChk_CheckedChanged;
            fpsChk.CheckedChanged -= fps_CheckedChanged;
            shChk.CheckedChanged -= speedrunhelper_CheckedChanged;
            smoothmouseChk.CheckedChanged -= smoothmouseChk_CheckedChanged;
            shortcutsChk.CheckedChanged -= shortcuts_CheckedChanged;
            internalModPackageChk.CheckedChanged -= internalModPackageChk_CheckedChanged;
            shCameraChk.CheckedChanged -= shCameraChk_CheckedChanged;
            lockHitboxChk.CheckedChanged -= lockHitboxChk_CheckedChanged;
            disKey.CheckedChanged -= disKey_CheckedChanged;


            minimizeChk.Checked = prop.Default.minimizedBoot;
            duplicationChk.Checked = prop.Default.duplicationFix;
            bitChk.Checked = prop.Default.bit;
            borderlessChk.Checked = prop.Default.borderless;
            infoChk.Checked = prop.Default.info;
            fpsChk.Checked = prop.Default.fps;
            shChk.Checked = prop.Default.sh;
            smoothmouseChk.Checked = prop.Default.smoothmouse;
            shortcutsChk.Checked = prop.Default.shortcuts;
            internalModPackageChk.Checked = prop.Default.externalModPackage;
            shCameraChk.Checked = prop.Default.shCamera;
            lockHitboxChk.Checked = prop.Default.hitboxChange;
            disKey.Checked = prop.Default.speedrun;


            // Events wieder anmelden
            minimizeChk.CheckedChanged += minimizeChk_CheckedChanged;
            duplicationChk.CheckedChanged += duplicationChk_CheckedChanged;
            bitChk.CheckedChanged += Bit_CheckedChanged;
            borderlessChk.CheckedChanged += borderless_CheckedChanged;
            infoChk.CheckedChanged += infoChk_CheckedChanged;
            fpsChk.CheckedChanged += fps_CheckedChanged;
            shChk.CheckedChanged += speedrunhelper_CheckedChanged;
            smoothmouseChk.CheckedChanged += smoothmouseChk_CheckedChanged;
            shortcutsChk.CheckedChanged += shortcuts_CheckedChanged;
            internalModPackageChk.CheckedChanged += internalModPackageChk_CheckedChanged;
            shCameraChk.CheckedChanged += shCameraChk_CheckedChanged;
            lockHitboxChk.CheckedChanged += lockHitboxChk_CheckedChanged;
            disKey.CheckedChanged += disKey_CheckedChanged;
        }

        private void settings_Load(object sender, EventArgs e)
        {
            userChecked = true;
            duplicationChk_CheckedChanged(duplicationChk, EventArgs.Empty);
            userChecked = true;
            minimizeChk_CheckedChanged(minimizeChk, EventArgs.Empty);
            userChecked = false;

            if (prop.Default.externalModPackage)
            {
                lockHitboxChk.Enabled = false;
                shCameraChk.Enabled = false;
            }


        }

        private void RestartApplication()
        {
            userChecked = !userChecked;
            if (userChecked)
            {
                DialogResult result = MessageBox.Show("Apply settings now?", "Restart required", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    RestartWithArgs("");
                }
            }
        }
        private void RestartApplication2()
        {
            var proc = Process.GetProcessesByName("OLGame");

            userChecked = !userChecked;
            if (userChecked)
            {
                DialogResult result = MessageBox.Show("Apply settings now?", "Restart required", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    var proc3 = Process.GetProcessesByName("OLGame");

                    foreach (var item in proc3)
                    {
                        item.Kill();
                    }

                    if (Program.argumentStartedWith != "")
                    {
                        RestartWithArgs(Program.argumentStartedWith);
                    }
                    else
                    {
                        RestartWithArgs("vanilla");
                    }
                }
            }
            else
            {
                var proc3 = Process.GetProcessesByName("OLGame");

                foreach (var item in proc3)
                {
                    item.Kill();
                }

                if (Program.argumentStartedWith != "")
                {
                    RestartWithArgs(Program.argumentStartedWith);
                }
                else
                {
                    RestartWithArgs("vanilla");
                }
            }
        }
        public static void RestartWithArgs(string args)
        {
            // Pfad zur aktuellen exe
            string exePath = Application.ExecutablePath;

            // Argumente als String zusammenbauen
            string argString = string.Join(" ", args);

            // Neues Programm starten
            Process.Start(exePath, argString);

            // Aktuelles Programm schließen
            Application.Exit();
        }
        private void minimizeChk_CheckedChanged(object sender, EventArgs e)
        {
            prop.Default.minimizedBoot = minimizeChk.Checked;
            prop.Default.Save();
        }

        private void duplicationChk_CheckedChanged(object sender, EventArgs e)
        {
            prop.Default.duplicationFix = duplicationChk.Checked;
            prop.Default.Save();
            RestartApplication();
        }

        private void Bit_CheckedChanged(object sender, EventArgs e)
        {
            prop.Default.bit = bitChk.Checked;
            prop.Default.Save();

            RestartApplication2();

        }

        private void borderless_CheckedChanged(object sender, EventArgs e)
        {
            prop.Default.borderless = borderlessChk.Checked;
            prop.Default.Save();

            RestartApplication2();
        }

        private void infoChk_CheckedChanged(object sender, EventArgs e)
        {
            prop.Default.info = infoChk.Checked;
            prop.Default.Save();
            RestartApplication();
        }

        private void fps_CheckedChanged(object sender, EventArgs e)
        {
            prop.Default.fps = fpsChk.Checked;
            prop.Default.Save();
            RestartApplication2();
        }



        private void speedrunhelper_CheckedChanged(object sender, EventArgs e) //TODO
        {
            prop.Default.sh = shChk.Checked;
            prop.Default.Save();

            if (!prop.Default.sh)
            {
                var proc = Process.GetProcessesByName("checkpointHandler64");
                foreach (var item in proc)
                {
                    item.Kill();
                }
                var proc32 = Process.GetProcessesByName("checkpointHandler32");
                foreach (var item in proc32)
                {
                    item.Kill();
                }
            }
            else
            {
                manager.speedrunhelper();
            }


        }

        private void smoothmouseChk_CheckedChanged(object sender, EventArgs e)
        {
            prop.Default.smoothmouse = smoothmouseChk.Checked;
            prop.Default.Save();
            RestartApplication2();
        }

        private void shortcuts_CheckedChanged(object sender, EventArgs e)
        {
            prop.Default.shortcuts = shortcutsChk.Checked;
            prop.Default.Save();
        }

        private void smhkBtn_Click(object sender, EventArgs e)
        {
            var mgr = Manager.Instance;
            if (mgr == null)
            {
                MessageBox.Show("Main Manager not initialized yet.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var actions = mgr.PublicActionToId.Keys.ToList();
            var existingHotkeys = mgr.PublicCurrentHotkeys.ToDictionary(kv => kv.Key, kv => kv.Value);

            using (var hkManager = new HotkeyManager(actions, existingHotkeys))
            {
                hkManager.TopMost = true;
                var result = hkManager.ShowDialog(this); // <-- modal
                if (result == DialogResult.OK)
                {
                    // Übernehme und registriere neu
                    mgr.OverrideHotkeysFrom(hkManager.UserHotkeys); // Methode siehe unten
                }
            }
        }

        private void bootBtn_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Path.Combine(Program.startupPath, "SaveManager", "Boot"));
        }

        private void internalModPackageChk_CheckedChanged(object sender, EventArgs e)
        {
            MessageBox.Show("This option could potentionaly kill outlast after some time of use.\nI would recommend leaving it off, this is just if you want to load vanilla and speedrun,\nbut sometimes still use mods for a MINUTE OR TWO.\nThe longer you have the mods running, the higher is the risk of outlast crashing randomly later on.\nUsualy Memory Leaks(what crashes Outlast) happen after like 15 minutes+ of having the mods enabled\nIn settings is a restart game button, if you have used it more than 10-15 minutes...", "WARNING!!!");
            prop.Default.externalModPackage = internalModPackageChk.Checked;
            prop.Default.Save();

            RestartApplication2();
        }

        private void shCameraChk_CheckedChanged(object sender, EventArgs e)
        {
            prop.Default.shCamera = shCameraChk.Checked;
            prop.Default.Save();
            //TODO NOT WORKING TO DISABLE IT
        }

        private void lockHitboxChk_CheckedChanged(object sender, EventArgs e)
        {
            prop.Default.hitboxChange = lockHitboxChk.Checked;
            prop.Default.Save();
        }

        private void restartgame_click(object sender, EventArgs e)
        {
            userChecked = true;
            RestartApplication2();
        }

        private void disKey_CheckedChanged(object sender, EventArgs e)
        {
            prop.Default.speedrun = disKey.Checked;
            prop.Default.Save();

            manager.DisableHotkeys();
        }
    }

}
