using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OutlastSaveManager
{
    public partial class mods : MaterialForm
    {
        private Manager manager;
        private bool enemyToggle = false;
        public mods(Manager existingManager)
        {
            InitializeComponent();
            manager = existingManager;

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(

            Primary.Grey900,   // Primary → Balken
            Primary.Grey900,   // DarkPrimary
            Primary.Pink800,    // LightPrimary
            Accent.Pink700,
            TextShade.WHITE
            );
            buttonReSize();

            this.Size = new Size(782, 573);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false; // Maximieren-Button weg
            this.MinimizeBox = false; // Minimieren-Button weg
            this.Sizable = false;
            label1.BackColor = Color.FromArgb(33, 33, 33);
            panel1.Size = new Size(800, 549);

            CenterFormOnScreen();

            checkAndDisable();


        }

        private void checkAndDisable()
        {
            if (prop.Default.externalModPackage)
            {
                materialButton15.Enabled = false;
                materialButton14.Enabled = false;
                materialButton13.Enabled = false;
                materialButton18.Enabled = false;
                materialButton24.Enabled = false;
                materialButton26.Enabled = false;
                materialButton21.Enabled = false;
                materialButton19.Enabled = false;
            }
        }

        private void CenterFormOnScreen()
        {
            // Berechne Position basierend auf Arbeitsfläche des Primärbildschirms
            int x = (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2;
            int y = (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2;
            this.Location = new Point(x, y);
        }
        private void buttonReSize()
        {
            materialButton1.AutoSize = false;
            materialButton2.AutoSize = false;
            materialButton3.AutoSize = false;
            materialButton4.AutoSize = false;
            materialButton5.AutoSize = false;
            materialButton6.AutoSize = false;
            loadCheckpoint.AutoSize = false;
            saveCheckpoint.AutoSize = false;
            materialButton9.AutoSize = false;
            materialButton10.AutoSize = false;
            materialButton11.AutoSize = false;
            materialButton12.AutoSize = false;
            materialButton13.AutoSize = false;
            materialButton14.AutoSize = false;
            materialButton15.AutoSize = false;
            materialButton16.AutoSize = false;
            materialButton17.AutoSize = false;
            materialButton18.AutoSize = false;
            materialButton19.AutoSize = false;
            materialButton20.AutoSize = false;
            materialButton21.AutoSize = false;
            materialButton22.AutoSize = false;
            materialButton23.AutoSize = false;
            materialButton24.AutoSize = false;
            materialButton25.AutoSize = false;
            materialButton26.AutoSize = false;
            materialButton29.AutoSize = false;
            materialButton30.AutoSize = false;
            materialButton31.AutoSize = false;
            materialButton33.AutoSize = false;
            materialButton34.AutoSize = false;
            materialButton35.AutoSize = false;
            materialButton37.AutoSize = false;
            materialButton38.AutoSize = false;
            materialButton39.AutoSize = false;
            materialButton40.AutoSize = false;
            materialButton41.AutoSize = false;
            materialButton42.AutoSize = false;
            materialButton43.AutoSize = false;
            materialButton1.Size = new Size(157, 36);
            materialButton2.Size = new Size(157, 36);
            materialButton3.Size = new Size(157, 36);
            materialButton4.Size = new Size(157, 36);
            materialButton5.Size = new Size(157, 36);
            materialButton6.Size = new Size(157, 36);
            loadCheckpoint.Size = new Size(157, 36);
            saveCheckpoint.Size = new Size(157, 36);
            materialButton9.Size = new Size(157, 36);
            materialButton10.Size = new Size(157, 36);
            materialButton11.Size = new Size(157, 36);
            materialButton12.Size = new Size(157, 36);
            materialButton13.Size = new Size(157, 36);
            materialButton14.Size = new Size(157, 36);
            materialButton15.Size = new Size(157, 36);
            materialButton16.Size = new Size(157, 36);
            materialButton17.Size = new Size(157, 36);
            materialButton18.Size = new Size(157, 36);
            materialButton19.Size = new Size(157, 36);
            materialButton20.Size = new Size(157, 36);
            materialButton21.Size = new Size(157, 36);
            materialButton22.Size = new Size(157, 36);
            materialButton23.Size = new Size(157, 36);
            materialButton24.Size = new Size(157, 36);
            materialButton25.Size = new Size(157, 36);
            materialButton26.Size = new Size(157, 36);
            materialButton29.Size = new Size(157, 36);
            materialButton30.Size = new Size(157, 36);
            materialButton31.Size = new Size(157, 36);
            materialButton33.Size = new Size(157, 36);
            materialButton34.Size = new Size(157, 36);
            materialButton35.Size = new Size(157, 36);
            materialButton37.Size = new Size(157, 36);
            materialButton38.Size = new Size(157, 36);
            materialButton39.Size = new Size(157, 36);
            materialButton40.Size = new Size(157, 36);
            materialButton41.Size = new Size(157, 36);
            materialButton42.Size = new Size(157, 36);
            materialButton43.Size = new Size(157, 36);
        }

        private void HitBoxNormal(object sender, EventArgs e)
        {
            if (prop.Default.externalModPackage)
            {
                hitbox(30, "Normal");
                manager.HideOverlay("VaultHitbox");
                manager.HideOverlay("DoorHitbox");
                manager.HideOverlay("ShimmyHitbox");
            }
            else
            {
                manager.hitboxnormal();
            }

        }

        private void HitBoxVault(object sender, EventArgs e)
        {
            if (prop.Default.externalModPackage)
            {


                hitbox(15, "Vault");

                manager.HideOverlay("DoorHitbox");
                manager.HideOverlay("ShimmyHitbox");
                manager.ShowOverlay("VaultHitbox", "VaultHitbox");
            }
            else
            {
                manager.hiboxvault();
            }
        }
        private void HitBoxDoor(object sender, EventArgs e)
        {
            if (prop.Default.externalModPackage)
            {
                hitbox(5, "Door");
                manager.HideOverlay("VaultHitbox");
                manager.ShowOverlay("DoorHitbox", "DoorHitbox");
                manager.HideOverlay("ShimmyHitbox");
            }
            else
            {
                manager.hitboxdoor();
            }
        }
        private void HitBoxShimmy(object sender, EventArgs e)
        {
            if (prop.Default.externalModPackage)
            {
                hitbox(2, "Shimmy");
                manager.ShowOverlay("ShimmyHitbox", "ShimmyHitbox");
                manager.HideOverlay("VaultHitbox");
                manager.HideOverlay("DoorHitbox");
            }
            else
            {
                manager.hitboxshimmy();
            }
        }

        private void hitbox(float value, string type)
        {
            if (manager.checkBit64())
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo()
                    {
                        FileName = Path.Combine(Directory.GetCurrentDirectory(), "SaveManager-Mods", "HitBoxChanger64.exe"),
                        Arguments = value.ToString(),
                        UseShellExecute = false, // false, damit man z.B. Standardausgabe lesen kann
                        CreateNoWindow = true   // true, wenn kein Fenster aufgehen soll
                    };

                    Process process = Process.Start(psi);
                    manager.AddLog($"Hitbox changed to: {type}");
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
                        FileName = Path.Combine(Directory.GetCurrentDirectory(), "SaveManager-Mods", "HitBoxChanger32.exe"),
                        Arguments = value.ToString(),
                        UseShellExecute = false, // false, damit man z.B. Standardausgabe lesen kann
                        CreateNoWindow = true   // true, wenn kein Fenster aufgehen soll
                    };

                    Process process = Process.Start(psi);
                    manager.AddLog($"Hitbox changed to: {type}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        private void nightVisionToggle(object sender, EventArgs e)
        {
            manager.nightvision();
        }

        public void toggleEnemy_Click(object sender, EventArgs e)
        {
            manager.freezeEnemy();
        }

        private void nodamageBtn_Click(object sender, EventArgs e)
        {
            manager.noDamage();
        }

        private void tptofreecamBtn_Click(object sender, EventArgs e)
        {
            manager.tptofreecam();
        }

        private void menu_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_Menu);
        }

        private void deleteExcept_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_DeleteExcept);
        }

        private void addSelected_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_AddSelected);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_LoadPreset_1);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_LoadPreset_2);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_LoadPreset_3);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_LoadPreset_4);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_LoadPreset_5);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_LoadPreset_6);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_LoadPreset_7);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_LoadPreset_8);
        }

        private void button22_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_LoadPreset_9);
        }

        private void button23_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_SavePreset_1);
        }

        private void button24_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_SavePreset_2);
        }

        private void button25_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_SavePreset_3);
        }

        private void button26_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_SavePreset_4);
        }

        private void button27_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_SavePreset_5);
        }

        private void button28_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_SavePreset_6);
        }

        private void button29_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_SavePreset_7);
        }

        private void button30_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_SavePreset_8);
        }

        private void button31_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_SavePreset_9);
        }

        private void button57_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_F1);
        }

        private void button58_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_F2);
        }

        private void button59_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_F3);
        }

        private void button60_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_F4);
        }

        private void button53_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_CTRL_F1);
        }

        private void button54_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_CTRL_F2);
        }

        private void button55_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_CTRL_F3);
        }

        private void button56_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_CTRL_F4);
        }

        private void gameSpeed_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_gamespeed);
        }

        private void playerInfo_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_playerinfo);
        }

        private void gameMarkers_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_gamemarkers);
        }

        private void exit_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_Exit);
        }

        private void insaneDifficulty_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_SetInsaneDifficulty);
        }

        private void maxGamma_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_Gamma10);
        }

        private void rsw_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_rsw);
        }

        private void removeCollactibles_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_removecollactibles);
        }

        private void freeCam_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_Freecam);
        }

        private void saveCheckpoint_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_saveCheckpoint);
        }

        private void loadCheckpoint_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_loadCheckpoint);
        }

        private void reloadCheckpoint_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_reloadcheckpoint);
        }

        private void statFPS_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_StatFPS);
        }

        private void showCollision_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_ShowCollision);
        }

        private void statLevels_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_StatLevels);
        }

        private void showDebug_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_ShowDebug);
        }

        private void showVolumes_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_volumes);
        }

        private void showFog_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_fog);
        }

        private void meshEdges_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_meshedges);
        }

        private void ai_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_ai);
        }

        private void showBounds_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_bounds);
        }

        private void staticMeshes_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_staticmeshes);
        }

        private void showPaths_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_Paths);
        }

        private void postProcess_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_postprocess);
        }

        private void levelColoration_Click(object sender, EventArgs e)
        {
            manager.TriggerHotkey(Manager.HOTKEY_levelcoloration);
        }

        private void mods_Load(object sender, EventArgs e)
        {

        }

        private void loadPreset_Click(object sender, EventArgs e)
        {
            switch (loadPresetPreset.SelectedItem?.ToString())
            {
                case "Preset 1":
                    manager.TriggerHotkey(Manager.HOTKEY_LoadPreset_1);
                    break;
                case "Preset 2":
                    manager.TriggerHotkey(Manager.HOTKEY_LoadPreset_2);
                    break;
                case "Preset 3":
                    manager.TriggerHotkey(Manager.HOTKEY_LoadPreset_3);
                    break;
                case "Preset 4":
                    manager.TriggerHotkey(Manager.HOTKEY_LoadPreset_4);
                    break;
                case "Preset 5":
                    manager.TriggerHotkey(Manager.HOTKEY_LoadPreset_5);
                    break;
                case "Preset 6":
                    manager.TriggerHotkey(Manager.HOTKEY_LoadPreset_6);
                    break;
                case "Preset 7":
                    manager.TriggerHotkey(Manager.HOTKEY_LoadPreset_7);
                    break;
                case "Preset 8":
                    manager.TriggerHotkey(Manager.HOTKEY_LoadPreset_8);
                    break;
                case "Preset 9":
                    manager.TriggerHotkey(Manager.HOTKEY_LoadPreset_9);
                    break;

                default:
                    MessageBox.Show("Please select a Preset");
                    break;
            }
        }

        private void savePreset_Click(object sender, EventArgs e)
        {
            switch (savePresetPreset.SelectedItem?.ToString())
            {
                case "Preset 1":
                    manager.TriggerHotkey(Manager.HOTKEY_SavePreset_1);
                    break;
                case "Preset 2":
                    manager.TriggerHotkey(Manager.HOTKEY_SavePreset_2);
                    break;
                case "Preset 3":
                    manager.TriggerHotkey(Manager.HOTKEY_SavePreset_3);
                    break;
                case "Preset 4":
                    manager.TriggerHotkey(Manager.HOTKEY_SavePreset_4);
                    break;
                case "Preset 5":
                    manager.TriggerHotkey(Manager.HOTKEY_SavePreset_5);
                    break;
                case "Preset 6":
                    manager.TriggerHotkey(Manager.HOTKEY_SavePreset_6);
                    break;
                case "Preset 7":
                    manager.TriggerHotkey(Manager.HOTKEY_SavePreset_7);
                    break;
                case "Preset 8":
                    manager.TriggerHotkey(Manager.HOTKEY_SavePreset_8);
                    break;
                case "Preset 9":
                    manager.TriggerHotkey(Manager.HOTKEY_SavePreset_9);
                    break;

                default:
                    MessageBox.Show("Please select a Preset");
                    break;
            }
        }

        private void loadCheckpoint_Click_1(object sender, EventArgs e)
        {
            switch (loadCheckpointSave.SelectedItem?.ToString())
            {
                case "Save 1":
                    manager.TriggerHotkey(Manager.HOTKEY_F1);
                    break;
                case "Save 2":
                    manager.TriggerHotkey(Manager.HOTKEY_F2);
                    break;
                case "Save 3":
                    manager.TriggerHotkey(Manager.HOTKEY_F3);
                    break;
                case "Save 4":
                    manager.TriggerHotkey(Manager.HOTKEY_F4);
                    break;

                default:
                    MessageBox.Show("Please select a Save");
                    break;
            }
        }

        private void saveCheckpoint_Click_1(object sender, EventArgs e)
        {
            switch (saveCheckpointSave.SelectedItem?.ToString())
            {
                case "Save 1":
                    manager.TriggerHotkey(Manager.HOTKEY_CTRL_F1);
                    break;
                case "Save 2":
                    manager.TriggerHotkey(Manager.HOTKEY_CTRL_F2);
                    break;
                case "Save 3":
                    manager.TriggerHotkey(Manager.HOTKEY_CTRL_F3);
                    break;
                case "Save 4":
                    manager.TriggerHotkey(Manager.HOTKEY_CTRL_F4);
                    break;

                default:
                    MessageBox.Show("Please select a Save");
                    break;
            }
        }
    }
}