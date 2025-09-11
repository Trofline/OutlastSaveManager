using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace OutlastSaveManager
{
    // Standalone implementation (programmatic UI) - paste/replace your existing HotkeyManager with this file.
    public partial class HotkeyManager : Form
    {
        // Public dictionary visible to the Manager form
        public Dictionary<string, string> UserHotkeys { get; private set; } = new Dictionary<string, string>();

        private readonly List<string> _actions;
        private readonly Dictionary<string, TextBox> _boxes = new Dictionary<string, TextBox>();
        private readonly string _savePath;

        // UI
        private FlowLayoutPanel flowPanel;
        private Button btnOK;
        private Button btnCancel;

        public HotkeyManager(List<string> actions, Dictionary<string, string>? existingHotkeys = null)
        {
            _actions = actions ?? new List<string>();
            _savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OutlastSaveManager", "hotkeys.json");

            if (existingHotkeys != null)
            {
                // clone
                foreach (var kv in existingHotkeys)
                    UserHotkeys[kv.Key] = kv.Value;
            }

            InitializeForm();
            BuildDynamicUI();
        }

        // Optional parameterless ctor to make designer-less usage easier
        public HotkeyManager() : this(new List<string>()) { }

        private void InitializeForm()
        {
            this.Text = "Hotkey Manager";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new System.Drawing.Size(520, 420);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            flowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoScroll = true,
                WrapContents = false,
                FlowDirection = FlowDirection.TopDown,
                Height = this.ClientSize.Height - 70
            };

            this.Controls.Add(flowPanel);

            // Buttons
            btnOK = new Button { Text = "OK", Width = 90, Height = 30 }; btnOK.Click += BtnOK_Click;
            btnCancel = new Button { Text = "Cancel", Width = 90, Height = 30 }; btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(8)
            };
            btnPanel.Controls.Add(btnOK);
            btnPanel.Controls.Add(btnCancel);
            this.Controls.Add(btnPanel);
        }

        private void BuildDynamicUI()
        {
            flowPanel.Controls.Clear();
            _boxes.Clear();

            foreach (var action in _actions)
            {
                var panel = new Panel { Width = flowPanel.ClientSize.Width - 25, Height = 36, Margin = new Padding(4) };

                var lbl = new Label
                {
                    Text = action,
                    AutoSize = false,
                    Width = 160,
                    Height = 28,
                    TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                    Left = 6,
                    Top = 4
                };

                var tb = new TextBox
                {
                    Width = panel.Width - lbl.Width - 16,
                    Left = lbl.Right + 6,
                    Top = 4,
                    ReadOnly = true,
                    Tag = action
                };

                // show existing hotkey if present
                if (UserHotkeys.TryGetValue(action, out var hk) && !string.IsNullOrWhiteSpace(hk))
                    tb.Text = hk;
                else
                    tb.Text = string.Empty;

                tb.KeyDown += Tb_KeyDown;
                tb.Click += Tb_Click;

                panel.Controls.Add(lbl);
                panel.Controls.Add(tb);

                flowPanel.Controls.Add(panel);
                _boxes[action] = tb;
            }
        }

        private void Tb_Click(object? sender, EventArgs e)
        {
            if (sender is TextBox tb)
            {
                tb.Focus();
                tb.SelectAll();
            }
        }

        private void Tb_KeyDown(object? sender, KeyEventArgs e)
        {
            if (sender is not TextBox tb) return;

            e.SuppressKeyPress = true;

            var parts = new List<string>();
            if (e.Control) parts.Add("CTRL");
            if (e.Shift) parts.Add("SHIFT");
            if (e.Alt) parts.Add("ALT");

            var key = e.KeyCode;
            if (key != Keys.ControlKey && key != Keys.ShiftKey && key != Keys.Menu)
                parts.Add(key.ToString());

            string hotkey = string.Join("+", parts);

            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Escape)
                hotkey = string.Empty;

            var action = (string)tb.Tag!;

            // --- Hier wird der alte Hotkey automatisch entfernt ---
            if (UserHotkeys.TryGetValue(action, out var oldHotkey) && !string.IsNullOrEmpty(oldHotkey))
            {
                // lösche die alte Bindung, bevor neue gesetzt wird
                var otherKeys = UserHotkeys.Where(kv => kv.Value == oldHotkey && kv.Key != action).Select(kv => kv.Key).ToList();
                foreach (var k in otherKeys)
                    UserHotkeys.Remove(k); // alte Bindungen löschen, falls es Doppelungen gab
            }

            // --- Prüfen, ob der Hotkey bereits einer anderen Aktion zugeordnet ist ---
            var conflict = UserHotkeys.FirstOrDefault(kv => kv.Value == hotkey && kv.Key != action);
            if (!string.IsNullOrEmpty(conflict.Key))
            {
                // alte Bindung entfernen, damit nur der neue Hotkey aktiv ist
                UserHotkeys.Remove(conflict.Key);
            }

            tb.Text = hotkey;

            if (string.IsNullOrEmpty(hotkey))
                UserHotkeys.Remove(action);
            else
                UserHotkeys[action] = hotkey;
        }


        private void BtnOK_Click(object? sender, EventArgs e)
        {
            // Save to file
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_savePath)!);
                var json = JsonSerializer.Serialize(UserHotkeys, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_savePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not save hotkeys: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // don't close with OK
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // helper for Manager to set a hotkey programmatically before showing
        public void SetHotkey(string action, string hotkey)
        {
            if (string.IsNullOrEmpty(action)) return;
            if (string.IsNullOrEmpty(hotkey))
            {
                UserHotkeys.Remove(action);
                if (_boxes.ContainsKey(action)) _boxes[action].Text = string.Empty;
            }
            else
            {
                UserHotkeys[action] = hotkey;
                if (_boxes.ContainsKey(action)) _boxes[action].Text = hotkey;
            }
        }

        // helper for Manager to get a hotkey (reads from UserHotkeys)
        public string? GetHotkey(string action)
        {
            if (string.IsNullOrEmpty(action)) return null;
            return UserHotkeys.TryGetValue(action, out var hk) ? hk : null;
        }

        // static helper: try load hotkeys from same location (used if you want to read without showing form)
        public static Dictionary<string, string> LoadHotkeysFromDisk()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OutlastSaveManager", "hotkeys.json");
            try
            {
                if (!File.Exists(path)) return new Dictionary<string, string>();
                var json = File.ReadAllText(path);
                var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                return dict ?? new Dictionary<string, string>();
            }
            catch
            {
                return new Dictionary<string, string>();
            }
        }
    }
}
