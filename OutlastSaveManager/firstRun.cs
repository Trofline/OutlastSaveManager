using OutlastSaveManager.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OutlastSaveManager
{
    public partial class firstRun : Form
    {
        bool workedDone = false;
        public firstRun()
        {
            InitializeComponent();
            this.Icon = Resources.logo;
        }

        private void firstRun_Load(object sender, EventArgs e)
        {
            string youtubeEmbed = "https://www.youtube.com/watch?v=ZPobPOSLUtE";
            webView21.Source = new Uri(youtubeEmbed);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (!workedDone)
            {
                MessageBox.Show("Please watch the tutorial first!!!");
            }
            else
            {
                var olgamrproc = Process.GetProcessesByName("OLGame");
                foreach (var item in olgamrproc)
                {
                    item.Kill();
                }
                Program.firstRunBool = false;
                Application.Restart();
                this.Close();
            }
        }
        private void webView21_Click(object sender, EventArgs e)
        {
            workedDone = true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (gameRunning())
            {
                var processes = Process.GetProcessesByName("OLGame");
                string exePath = processes[0].MainModule.FileName;
                string gameDir = Path.GetDirectoryName(exePath);

                // Zwei Ordner zurück, dann OLGame\SaveData
                var gameSaveFolder = Path.Combine(gameDir, "..", "..", "OLGame");
                Process.Start("explorer.exe", gameSaveFolder);
                workedDone = true;
            }
        }
        private bool gameRunning()
        {
            var processes = Process.GetProcessesByName("OLGame");
            if (processes.Length == 0)
            {
                MessageBox.Show("Outlast is not running!\nPlease run the game and press me again");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}