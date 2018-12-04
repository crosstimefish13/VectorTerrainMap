using System.Drawing;
using System.Windows.Forms;
using TerrainMapGUILibrary.Themes;

namespace TerrainMapGUILibrary.Extensions
{
    internal class GroupBoxExtension : GroupBox
    {
        public GroupBoxExtension()
        {
            Font = FontTheme.Normal();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.Clear(BackColor);

            var textBrush = new SolidBrush(SystemColors.ControlText);
            e.Graphics.DrawString(Text, Font, textBrush, 0, 0);
            textBrush.Dispose();

            var textSize = e.Graphics.MeasureString(Text, Font);
            var borderPen = new Pen(SystemColors.ControlText, 1f);
            e.Graphics.DrawLines(borderPen, new PointF[]
            {
                new PointF(ClientRectangle.Left + textSize.Width, ClientRectangle.Top),
                new PointF(ClientRectangle.Right - 1f, ClientRectangle.Top),
                new PointF(ClientRectangle.Right - 1f, ClientRectangle.Bottom - 1f),
                new PointF(ClientRectangle.Left, ClientRectangle.Bottom - 1f),
                new PointF(ClientRectangle.Left, ClientRectangle.Top + textSize.Height)
            });
            borderPen.Dispose();
        }
    }
}
