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
        public static void DrawRoundedLine(this Graphics g, Pen pen, float width, float x1, float y1, float x2, float y2)
        {
            float offsetAngle = 0f;
            int compareX = Common.FloatCompare(x1, x2);
            int compareY = Common.FloatCompare(y1, y2);

            if (compareX == 0 && compareY == 0) { return; }

            if (compareX == 0) { offsetAngle = -90f; }
            else if (compareY == 0) { offsetAngle = 0f; }
            else { offsetAngle = (float)(Math.Atan2(y2 - y1, x2 - x1) * 180d / Math.PI); }

            var path = new GraphicsPath();
            path.AddArc(x1 - width, y1 - width, width * 2, width * 2, 90f + offsetAngle, 180f);

            g.DrawPath(pen, path);

            path.Dispose();
        }
    }
}
