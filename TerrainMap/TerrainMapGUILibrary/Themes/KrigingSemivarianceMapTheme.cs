using System.Drawing;
using TerrainMapLibrary.Interpolator.Kriging;

namespace TerrainMapGUILibrary.Themes
{
    internal class KrigingSemivarianceMapTheme : SemivarianceMapChart.IChartStyle
    {
        public int Width { get; private set; }

        public int Height { get; private set; }

        public float Margin
        {
            get
            {
                return 40f;
            }
        }

        public Color BackColor
        {
            get
            {
                return Color.FromArgb(255, 255, 255);
            }
        }

        public float PointSize
        {
            get
            {
                return 4f;
            }
        }

        public Color PointFillColor
        {
            get
            {
                return Color.FromArgb(153, 170, 181);
            }
        }

        public float PointOutlinWidth
        {
            get
            {
                return 1f;
            }
        }

        public Color PointOutlineColor
        {
            get
            {
                return Color.FromArgb(44, 47, 51);
            }
        }

        public float AxisLineWidth
        {
            get
            {
                return 3f;
            }
        }

        public Color AxisLineColor
        {
            get
            {
                return Color.FromArgb(35, 39, 42);
            }
        }

        public Font TextFont
        {
            get
            {
                return FontTheme.Normal();
            }
        }

        public Color TextColor
        {
            get
            {
                return Color.FromArgb(35, 39, 42);
            }
        }

        public int TextDecimalDigits
        {
            get
            {
                return 8;
            }
        }

        public float CurveLineWidth
        {
            get
            {
                return 3f;
            }
        }

        public Color CurveLineColor
        {
            get
            {
                return Color.FromArgb(114, 137, 218);
            }
        }

        public KrigingSemivarianceMapTheme(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}
