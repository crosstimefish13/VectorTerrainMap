using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TerrainMapLibrary.Mathematics;

namespace TerrainMapLibrary.Interpolator.Kriging
{
    public static class GraphicsExtension
    {
        public static void DrawRoundedLine(this Graphics g, Color color, float width,
            float x1, float y1, float x2, float y2)
        {
            // title of line head
            float radius = width / 2f;
            float startAngle = GetRoundedLineStartAngle(x1, y1, x2, y2);
            if (float.IsNaN(startAngle) == true || Common.FloatCompare(radius, 0.5f) <= 0)
            {
                // width less than or equal with 1, just draw a line
                var pen = new Pen(color);
                g.DrawLine(pen, x1, y1, x2, y2);
                pen.Dispose();
            }
            else
            {
                // need to draw line with rounded head
                var path = new GraphicsPath();
                path.AddArc(x1 - radius, y1 - radius, radius * 2, radius * 2, startAngle, 180f);
                path.AddArc(x2 - radius, y2 - radius, radius * 2, radius * 2, startAngle + 180f, 180f);
                path.CloseAllFigures();

                var brush = new SolidBrush(color);
                g.FillPath(brush, path);

                brush.Dispose();

                path.Dispose();
            }
        }

        public static void DrawPoint(this Graphics g, Color fill, Color outline, float size, float outlineWidth,
            float x, float y)
        {
            var brush = new SolidBrush(fill);
            var pen = new Pen(outline, outlineWidth);

            float radius = size / 2f;
            g.FillEllipse(brush, x - radius, y - radius, size, size);
            g.DrawEllipse(pen, x - radius, y -radius, size, size);

            pen.Dispose();
            brush.Dispose();
        }

        public static void DrawText(this Graphics g, string text, Color color, float x, float y)
        {
            var brush = new SolidBrush(color);
            var font = new Font("Arial", 12f, FontStyle.Regular);
            g.DrawString(text, font, brush, x, y);
            font.Dispose();
            brush.Dispose();
        }


        private static float GetRoundedLineStartAngle(float x1, float y1, float x2, float y2)
        {
            float startAngle = 0f;
            int compareX = Common.FloatCompare(x1, x2);
            int compareY = Common.FloatCompare(y1, y2);

            // not need to draw a line
            if (compareX == 0 && compareY == 0) { return float.NaN; }

            if (compareX == 0)
            {
                // vertical line
                if (compareY < 0) { startAngle = 180f; }
                else { startAngle = 0f; }
            }
            else if (compareY == 0)
            {
                // horizontal line
                if (compareX < 0) { startAngle = 90f; }
                else { startAngle = 270f; }
            }
            else
            {
                // calcualte the start angle
                startAngle = (float)(Math.Atan2(y2 - y1, x2 - x1) * 180d / Math.PI);
                if (compareX < 0 && compareY > 0) { startAngle = 90f + startAngle; }
                else if (compareX > 0 && compareY > 0) { startAngle = 180f - startAngle; }
                else if (compareX > 0 && compareY < 0) { startAngle = 90f + startAngle; }
                else { startAngle = 90f + startAngle; }
            }

            return startAngle;
        }
    }
}
