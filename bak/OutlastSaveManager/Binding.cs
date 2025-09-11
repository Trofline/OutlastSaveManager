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
    public partial class Binding : Form
    {
        public class CommandBinding
        {
            public string Command { get; set; }
            public Keys Hotkey { get; set; }
            public bool Enabled { get; set; }
        }

        private string iniPath = Path.Combine(Directory.GetParent(Application.StartupPath).FullName, "OLGame", "Config", "DefaultInput.ini");
        private List<CommandBinding> bindings = new List<CommandBinding>()
        {
            new CommandBinding() { Command = "Stat FPS", Hotkey = Keys.F1, Enabled = true },
            new CommandBinding() { Command = "Show COLLISION", Hotkey = Keys.F2, Enabled = true },
            new CommandBinding() { Command = "Show POSTPROCESS", Hotkey = Keys.F3, Enabled = true },
            new CommandBinding() { Command = "Show LEVELCOLORATION", Hotkey = Keys.F4, Enabled = true },
            new CommandBinding() { Command = "Show FOG", Hotkey = Keys.F5, Enabled = true },
            new CommandBinding() { Command = "Show VOLUMES", Hotkey = Keys.F6, Enabled = true },
            new CommandBinding() { Command = "Show PATHS", Hotkey = Keys.F7, Enabled = true },
            new CommandBinding() { Command = "Gamma 10", Hotkey = Keys.F10, Enabled = true },
            new CommandBinding() { Command = "Camera Freecam", Hotkey = Keys.O, Enabled = true },
            new CommandBinding() { Command = "Camera Default", Hotkey = Keys.P, Enabled = true }
        };
        public Binding()
        {
            InitializeComponent();
            GenerateBindingsUI();

        }

        private void Binding_Load(object sender, EventArgs e)
        {

        }


        private void LoadBindingsFromIni()
        {
            if (!File.Exists(iniPath)) return;

            string[] lines = File.ReadAllLines(iniPath);
            foreach (var line in lines)
            {
                foreach (var binding in bindings)
                {
                    if (line.Contains($"Command=\"{binding.Command}\""))
                    {
                        // Name="F1" extrahieren
                        int nameStart = line.IndexOf("Name=\"") + 6;
                        int nameEnd = line.IndexOf("\"", nameStart);
                        if (nameStart > 5 && nameEnd > nameStart)
                        {
                            string keyString = line.Substring(nameStart, nameEnd - nameStart);
                            if (Enum.TryParse(keyString, out Keys key))
                                binding.Hotkey = key;
                        }
                        // Optional: Kann man Enabled aus Zeile setzen, hier einfach true
                        binding.Enabled = true;
                    }
                }
            }
        }

        private void SaveBindingsToIni()
        {
            List<string> lines = new List<string>();
            foreach (var binding in bindings)
            {
                if (binding.Enabled)
                {
                    lines.Add($".Bindings=(Name=\"{binding.Hotkey}\",Command=\"{binding.Command}\")");
                }
            }
            File.WriteAllLines(iniPath, lines);
        }

        private void GenerateBindingsUI()
        {
            int y = 10;
            foreach (var binding in bindings)
            {
                TextBox tb = new TextBox();
                tb.Text = binding.Hotkey.ToString();
                tb.Location = new System.Drawing.Point(10, y);
                tb.Width = 100;
                tb.Tag = binding;
                tb.KeyDown += Tb_KeyDown;
                this.Controls.Add(tb);

                Label lbl = new Label();
                lbl.Text = binding.Command;
                lbl.Location = new System.Drawing.Point(120, y + 3);
                lbl.Width = 150;
                this.Controls.Add(lbl);

                CheckBox cb = new CheckBox();
                cb.Checked = binding.Enabled;
                cb.Location = new System.Drawing.Point(280, y);
                cb.Tag = binding;
                cb.CheckedChanged += Cb_CheckedChanged;
                this.Controls.Add(cb);

                y += 30;
            }

            // Button zum Speichern
            Button saveBtn = new Button();
            saveBtn.Text = "Save";
            saveBtn.Location = new System.Drawing.Point(10, y + 10);
            saveBtn.Click += (s, e) => SaveBindingsToIni();
            this.Controls.Add(saveBtn);
        }

        private void Tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox tb && tb.Tag is CommandBinding binding)
            {
                binding.Hotkey = e.KeyCode;
                tb.Text = e.KeyCode.ToString();
                e.SuppressKeyPress = true;
            }
        }

        private void Cb_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox cb && cb.Tag is CommandBinding binding)
            {
                binding.Enabled = cb.Checked;
            }
        }

        private void restartGame_Click(object sender, EventArgs e)
        {
            var proc = Process.GetProcessesByName("OLGame");

            foreach (var item in proc)
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
    }
}
