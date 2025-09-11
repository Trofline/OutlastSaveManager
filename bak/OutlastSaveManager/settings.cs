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
    public partial class settings : Form
    {
        bool userChecked = false;
        public string bootParameter;
        private Binding bindingInstance;
        public settings()
        {
            InitializeComponent();
            this.Icon = Resources.logo;

            // Events kurz abmelden, damit CheckedChanged nicht feuert
            minimizeChk.CheckedChanged -= minimizeChk_CheckedChanged;
            duplicationChk.CheckedChanged -= duplicationChk_CheckedChanged;
            bitChk.CheckedChanged -= Bit_CheckedChanged;
            borderlessChk.CheckedChanged -= borderless_CheckedChanged;
            infoChk.CheckedChanged -= infoChk_CheckedChanged;
            fpsChk.CheckedChanged -= fps_CheckedChanged;



            minimizeChk.Checked = prop.Default.minimizedBoot;
            duplicationChk.Checked = prop.Default.duplicationFix;
            bitChk.Checked = prop.Default.bit;
            borderlessChk.Checked = prop.Default.borderless;
            infoChk.Checked = prop.Default.info;
            fpsChk.Checked = prop.Default.fps;


            // Events wieder anmelden
            minimizeChk.CheckedChanged += minimizeChk_CheckedChanged;
            duplicationChk.CheckedChanged += duplicationChk_CheckedChanged;
            bitChk.CheckedChanged += Bit_CheckedChanged;
            borderlessChk.CheckedChanged += borderless_CheckedChanged;
            infoChk.CheckedChanged += infoChk_CheckedChanged;
            fpsChk.CheckedChanged += fps_CheckedChanged;
        }

        private void settings_Load(object sender, EventArgs e)
        {
            userChecked = true;
            duplicationChk_CheckedChanged(duplicationChk, EventArgs.Empty);
            userChecked = true;
            minimizeChk_CheckedChanged(minimizeChk, EventArgs.Empty);
            userChecked = false;
        }

        private void RestartApplication()
        {
            userChecked = !userChecked;
            if (userChecked)
            {
                DialogResult result = MessageBox.Show("Apply settings now?", "Restart required", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    Application.Restart();
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

        private void manageBindingsBtn_Click(object sender, EventArgs e)
        {
            if (bindingInstance == null || bindingInstance.IsDisposed)
            {
                // Korrigiert: Binding benötigt keinen Parameter im Konstruktor
                bindingInstance = new Binding();
                bindingInstance.Show();

                bindingInstance.FormClosed += (s, args) => bindingInstance = null;
            }
            else
            {
                bindingInstance.BringToFront();
            }
        }
    }

}
