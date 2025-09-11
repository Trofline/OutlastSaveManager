using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OutlastSaveManager
{
    public partial class mods : Form
    {
        private Manager manager;
        private bool enemyToggle = false;
        public mods(Manager existingManager)
        {
            InitializeComponent();
            manager = existingManager;
        }

        private void HitBoxNormal(object sender, EventArgs e)
        {
            hitbox(30, "Normal");
            manager.HideOverlay("VaultHitbox");
            manager.HideOverlay("DoorHitbox");
            manager.HideOverlay("ShimmyHitbox");
        }

        private void HitBoxVault(object sender, EventArgs e)
        {
            hitbox(15, "Vault");
            
            manager.HideOverlay("DoorHitbox");
            manager.HideOverlay("ShimmyHitbox");
            manager.ShowOverlay("VaultHitbox", "VaultHitbox");
        }
        private void HitBoxDoor(object sender, EventArgs e)
        {
            hitbox(5, "Door");
            manager.HideOverlay("VaultHitbox");
            manager.ShowOverlay("DoorHitbox", "DoorHitbox");
            manager.HideOverlay("ShimmyHitbox");
        }
        private void HitBoxShimmy(object sender, EventArgs e)
        {
            hitbox(2, "Shimmy");
            manager.ShowOverlay("ShimmyHitbox","ShimmyHitbox");
            manager.HideOverlay("VaultHitbox");
            manager.HideOverlay("DoorHitbox");
            
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
    }
}
