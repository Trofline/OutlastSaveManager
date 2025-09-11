using System;
using System.Drawing;
using System.Windows.Forms;

namespace OutlastSaveManager
{
    public class Overlay : Form
    {
        private string displayText;
        private Font overlayFont;

        public Overlay(string text)
        {
            displayText = text;
            overlayFont = new Font("Consolas", 12, FontStyle.Regular);

            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;
            this.BackColor = Color.Black;
            this.TransparencyKey = Color.Black;
            this.ShowInTaskbar = false;

            UpdateSizeAndPosition();
        }

        public void SetText(string text)
        {
            displayText = text;
            this.Invalidate(); // nur neu zeichnen
        }


        private void UpdateSizeAndPosition()
        {
            using (Graphics g = this.CreateGraphics())
            {
                SizeF textSize = g.MeasureString(displayText, overlayFont);
                this.Size = new Size((int)textSize.Width + 10, (int)textSize.Height + 5);

                int x = Screen.PrimaryScreen.Bounds.Width - this.Width - 20; // 20px Abstand rechts
                int y = 20; // 20px Abstand oben
                this.Location = new Point(x, y);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawString(displayText, overlayFont, Brushes.DarkGray, new PointF(0, 0));
        }
    }
}
