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
    public partial class help : Form
    {
        Manager manager;

        
        public help()
        {
            InitializeComponent();
            this.Icon = Resources.logo;
        }

        private void permission_Click(object sender, EventArgs e)
        {
            if (!Path.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SaveManager/FirstRun")))
            {
                var olgameproc = Process.GetProcessesByName("OLGame");
                foreach (var item in olgameproc)
                {
                    item.Kill();
                }
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SaveManager/FirstRun"));
                Application.Restart();
            }
            else
            {
                MessageBox.Show("If you got this message, then you managed to get smth impossible haha\nTF????!?!?!");
            }
        }

        private void btnFurtherHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Please add me on discord:\n\nUserName:   trofline_black");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string url = "https://github.com/Trofline/Tools/issues/new/choose";
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
            MessageBox.Show("Optional in discord:\n\nUserName:   trofline_black");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            /*string url = "https://github.com/Trofline/Tools/releases";
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });*/

            manager.CheckAndUpdate();
            MessageBox.Show("Already up to date");
        }

        private void help_Load(object sender, EventArgs e)
        {

        }
    }
}
