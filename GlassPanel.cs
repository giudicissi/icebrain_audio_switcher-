using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace AudioSwitcherApp
{
    public class GlassPanel : Panel
    {
        [DefaultValue(typeof(Color), "180, 255, 255, 255")]
        public Color BorderColor { get; set; } = Color.FromArgb(180, 255, 255, 255);

        [DefaultValue(1)]
        public int BorderThickness { get; set; } = 1;

        public GlassPanel()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor,
                true);

            BackColor = Color.FromArgb(160, 20, 20, 20);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using var pen = new Pen(BorderColor, BorderThickness);
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            e.Graphics.DrawRectangle(pen, rect);
            base.OnPaint(e);
        }
    }
}
