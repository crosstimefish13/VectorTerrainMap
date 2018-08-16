using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrainMapLibrary.Mathematics;

namespace TerrainMapLibrary.Interpolator.Kriging
{
    public static class GraphicsExtension
    {
        public static void DrawRoundedLine(this Graphics g, Color color, float width, float x1, float y1, float x2, float y2)
        {
            float radius = width / 2f;
            float startAngle = GetRoundedLineStartAngle(x1, y1, x2, y2);
            if (float.IsNaN(startAngle) == true || Common.FloatCompare(radius, 0.5f) <= 0)
            {
                var pen = new Pen(color);
                g.DrawLine(pen, x1, y1, x2, y2);
                pen.Dispose();
            }
            else
            {
                var path = new GraphicsPath();
                path.AddArc(x1 - radius, y1 - radius, radius * 2, radius * 2, startAngle, 180f);
                path.AddArc(x2 - radius, y2 - radius, radius * 2, radius * 2, startAngle + 180f, 180f);
                path.CloseAllFigures();

                var brush = new SolidBrush(color);
                g.FillPath(brush, path);
                //g.DrawPath(Pens.Black, path);

                brush.Dispose();

                path.Dispose();
            }
        }


        private static float GetRoundedLineStartAngle(float x1, float y1, float x2, float y2)
        {
            float startAngle = 0f;
            int compareX = Common.FloatCompare(x1, x2);
            int compareY = Common.FloatCompare(y1, y2);

            if (compareX == 0 && compareY == 0) { return float.NaN; }

            if (compareX == 0)
            {
                if (compareY < 0) { startAngle = 180f; }
                else { startAngle = 0f; }
            }
            else if (compareY == 0)
            {
                if (compareX < 0) { startAngle = 90f; }
                else { startAngle = 270f; }
            }
            else
            {
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
