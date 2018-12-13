using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TerrainMapLibrary.Mathematics;

namespace TerrainMapLibrary.Interpolator.Kriging
{
    public class SemivarianceMapChart
    {
        private RectangleF vectorsCanvasClient;

        private double vectorToCanvasScaleX;

        private double vectorToCanvasScaleY;

        private long vectorValueScaleX;

        private long vectorValueScaleY;

        private float textHeight;

        private Bitmap image;

        public SemivarianceMap.Vector MinVector { get; private set; }

        public SemivarianceMap.Vector MaxVector { get; private set; }

        public SemivarianceMap Map { get; private set; }

        public IChartStyle Style { get; private set; }

        public Bitmap DrawData()
        {
            var g = Graphics.FromImage(image);
            g.Clear(Style.BackColor);

            // draw vectors
            for (long index = 0; index < Map.VectorCount; index++)
            {
                double offsetX = (Map[index].EuclidDistance * vectorValueScaleX - MinVector.EuclidDistance) * vectorToCanvasScaleX;
                double offsetY = (Map[index].Semivariance * vectorValueScaleY - MinVector.EuclidDistance) * vectorToCanvasScaleY;
                float drawPointX = vectorsCanvasClient.Left + (float)offsetX;
                float drawPointY = vectorsCanvasClient.Bottom - (float)offsetY;
                g.DrawPoint(
                    Style.PointFillColor,
                    Style.PointOutlineColor,
                    Style.PointSize,
                    Style.PointOutlinWidth,
                    drawPointX,
                    drawPointY
                );
            }

            // draw left arrow
            g.DrawRoundedLine(
                Style.AxisLineColor,
                Style.AxisLineWidth,
                vectorsCanvasClient.Left - Style.Margin - Style.AxisLineWidth * 2f,
                vectorsCanvasClient.Bottom + Style.Margin + Style.AxisLineWidth * 2f,
                vectorsCanvasClient.Left - Style.Margin - Style.AxisLineWidth * 2f,
                vectorsCanvasClient.Top - Style.Margin
            );
            g.DrawRoundedLine(
                Style.AxisLineColor,
                Style.AxisLineWidth,
                vectorsCanvasClient.Left - Style.Margin - Style.AxisLineWidth * 2f,
                vectorsCanvasClient.Top - Style.Margin,
                vectorsCanvasClient.Left - Style.Margin - Style.AxisLineWidth * 4f,
                vectorsCanvasClient.Top - Style.Margin + Style.AxisLineWidth * 2f
            );
            g.DrawRoundedLine(
                Style.AxisLineColor,
                Style.AxisLineWidth,
                vectorsCanvasClient.Left - Style.Margin - Style.AxisLineWidth * 2f,
                vectorsCanvasClient.Top - Style.Margin,
                vectorsCanvasClient.Left - Style.Margin,
                vectorsCanvasClient.Top - Style.Margin + Style.AxisLineWidth * 2f
            );

            // drwa bottom arrow
            g.DrawRoundedLine(
                Style.AxisLineColor,
                Style.AxisLineWidth,
                vectorsCanvasClient.Left - Style.Margin - Style.AxisLineWidth * 2f,
                vectorsCanvasClient.Bottom + Style.Margin + Style.AxisLineWidth * 2f,
                vectorsCanvasClient.Right + Style.Margin,
                vectorsCanvasClient.Bottom + Style.Margin + Style.AxisLineWidth * 2f
            );
            g.DrawRoundedLine(
                Style.AxisLineColor,
                Style.AxisLineWidth,
                vectorsCanvasClient.Right + Style.Margin,
                vectorsCanvasClient.Bottom + Style.Margin + Style.AxisLineWidth * 2f,
                vectorsCanvasClient.Right + Style.Margin - Style.AxisLineWidth * 2f,
                vectorsCanvasClient.Bottom + Style.Margin
            );
            g.DrawRoundedLine(
                Style.AxisLineColor,
                Style.AxisLineWidth,
                vectorsCanvasClient.Right + Style.Margin,
                vectorsCanvasClient.Bottom + Style.Margin + Style.AxisLineWidth * 2f,
                vectorsCanvasClient.Right + Style.Margin - Style.AxisLineWidth * 2f,
                vectorsCanvasClient.Bottom + Style.Margin + Style.AxisLineWidth * 4f
            );

            // draw top text
            string topMinValue = Common.ToNumberString(MinVector.Semivariance, Style.TextDecimalDigits, true);
            string topMaxValue = Common.ToNumberString(MaxVector.Semivariance, Style.TextDecimalDigits, true);
            string topText = $"Semivariance Min={topMinValue} Max={topMaxValue} Scale={vectorValueScaleY}";
            g.DrawText(
                topText,
                Style.TextFont,
                Style.TextColor,
                vectorsCanvasClient.Left - Style.Margin - Style.AxisLineWidth * 2f,
                Style.Margin
            );

            // draw bottom text
            string bottomMinValue = Common.ToNumberString(MinVector.EuclidDistance, Style.TextDecimalDigits, true);
            string bottomMaxValue = Common.ToNumberString(MaxVector.EuclidDistance, Style.TextDecimalDigits, true);
            string lagBins = Common.ToNumberString(Map.LagBins * vectorValueScaleX, Style.TextDecimalDigits, true);
            string bottomText = $"EuclidDistance Min={bottomMinValue} Max={bottomMaxValue} Scale={vectorValueScaleX} LagBins={lagBins} VectorCount={Map.VectorCount}";
            g.DrawText(
                bottomText,
                Style.TextFont,
                Style.TextColor,
                vectorsCanvasClient.Left - Style.Margin - Style.AxisLineWidth * 2f,
                Style.Height - Style.Margin - textHeight * 2f
            );

            g.Dispose();
            var result = (Bitmap)image.Clone();
            return result;
        }

        public Bitmap DrawModelCurve(Model model)
        {
            var result = (Bitmap)image.Clone();
            var g = Graphics.FromImage(result);

            // get the curve points
            double rangeX = MaxVector.EuclidDistance - MinVector.EuclidDistance;
            var points = new List<PointF>();
            for (int i = 0; i < Style.Width; i++)
            {
                double valueX = rangeX * i / Style.Width;
                double valueY = model.Map(valueX);
                float drawX = vectorsCanvasClient.Left + (float)(valueX * vectorToCanvasScaleX);
                float drawY = vectorsCanvasClient.Bottom - (float)(valueY * vectorToCanvasScaleY);
                points.Add(new PointF(drawX, drawY));
            }

            // draw curve
            var pen = new Pen(Style.CurveLineColor, Style.CurveLineWidth);
            var path = new GraphicsPath();
            path.AddCurve(points.ToArray());
            g.DrawPath(pen, path);
            path.Dispose();
            pen.Dispose();

            // draw bottom text
            string minXValue = Common.ToNumberString(model.MinX, Style.TextDecimalDigits, true);
            string minYValue = Common.ToNumberString(model.MinY, Style.TextDecimalDigits, true);
            string maxXValue = Common.ToNumberString(model.MaxX, Style.TextDecimalDigits, true);
            string maxYValue = Common.ToNumberString(model.MaxY, Style.TextDecimalDigits, true);
            g.DrawText(
                $"{model.GetType().Name} MinX={minXValue} MinY={minYValue} MaxX={maxXValue} MaxY={maxYValue}",
                Style.TextFont,
                Style.TextColor,
                vectorsCanvasClient.Left - Style.Margin - Style.AxisLineWidth * 2f,
                Style.Height - Style.Margin - textHeight
            );

            g.Dispose();
            return result;
        }

        public void Close()
        {
            image.Dispose();
        }

        public static SemivarianceMapChart Create(SemivarianceMap map, IChartStyle style)
        {
            var chart = new SemivarianceMapChart()
            {
                Map = map,
                Style = style,
                image = new Bitmap(style.Width, style.Height)
            };

            // TextHeight
            var g = Graphics.FromImage(chart.image);
            chart.textHeight = g.MeasureString("Mesaure", style.TextFont).Height;
            g.Dispose();

            // VectorsCanvasClient
            var vectorsCanvasClientLocation = new PointF(
                style.Margin * 2f + style.AxisLineWidth * 4f,
                style.Margin * 2f + chart.textHeight
            );
            var vectorsCanvasClientSize = new SizeF(
                style.Width - vectorsCanvasClientLocation.X - style.Margin * 2f,
                style.Height - vectorsCanvasClientLocation.Y - style.Margin * 2f - chart.textHeight * 2f - style.AxisLineWidth * 4f
            );
            chart.vectorsCanvasClient = new RectangleF(
                vectorsCanvasClientLocation,
                vectorsCanvasClientSize
            );

            // MinVector MaxVector
            chart.MinVector = new SemivarianceMap.Vector(double.MaxValue, double.MaxValue);
            chart.MaxVector = new SemivarianceMap.Vector(double.MinValue, double.MinValue);
            for (long index = 0; index < map.VectorCount; index++)
            {
                if (Common.DoubleCompare(map[index].EuclidDistance, chart.MinVector.EuclidDistance) < 0)
                {
                    chart.MinVector.EuclidDistance = map[index].EuclidDistance;
                }

                if (Common.DoubleCompare(map[index].Semivariance, chart.MinVector.Semivariance) < 0)
                {
                    chart.MinVector.Semivariance = map[index].Semivariance;
                }

                if (Common.DoubleCompare(map[index].EuclidDistance, chart.MaxVector.EuclidDistance) > 0)
                {
                    chart.MaxVector.EuclidDistance = map[index].EuclidDistance;
                }

                if (Common.DoubleCompare(map[index].Semivariance, chart.MaxVector.Semivariance) > 0)
                {
                    chart.MaxVector.Semivariance = map[index].Semivariance;
                }
            }

            // VectorValueScaleX VectorValueScaleY
            chart.vectorValueScaleX = 1;
            double minValueX = Math.Abs(chart.MinVector.EuclidDistance);
            while (Common.DoubleCompare(minValueX, 1) < 0)
            {
                chart.vectorValueScaleX *= 10;
                minValueX *= 10;
            }

            chart.MinVector.EuclidDistance *= chart.vectorValueScaleX;
            chart.MaxVector.EuclidDistance *= chart.vectorValueScaleX;

            chart.vectorValueScaleY = 1;
            double minValueY = Math.Abs(chart.MinVector.Semivariance);
            while (Common.DoubleCompare(minValueY, 1) < 0)
            {
                chart.vectorValueScaleY *= 10;
                minValueY *= 10;
            }

            chart.MinVector.Semivariance *= chart.vectorValueScaleY;
            chart.MaxVector.Semivariance *= chart.vectorValueScaleY;

            // VectorToCanvasScaleX VectorToCanvasScaleY
            chart.vectorToCanvasScaleX = chart.vectorsCanvasClient.Width / (chart.MaxVector.EuclidDistance - chart.MinVector.EuclidDistance);
            chart.vectorToCanvasScaleY = chart.vectorsCanvasClient.Height / (chart.MaxVector.Semivariance - chart.MinVector.Semivariance);

            return chart;
        }

        private SemivarianceMapChart()
        { }

        public override bool Equals(object obj)
        {
            throw new NotSupportedException();
        }

        public override int GetHashCode()
        {
            int hashCode =
                vectorsCanvasClient.GetHashCode() +
                MinVector.GetHashCode() +
                MaxVector.GetHashCode() +
                vectorToCanvasScaleX.GetHashCode() +
                vectorToCanvasScaleY.GetHashCode() +
                vectorValueScaleX.GetHashCode() +
                vectorValueScaleY.GetHashCode() +
                textHeight.GetHashCode() +
                image.GetHashCode() +
                Map.GetHashCode() +
                Style.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return $"Width:{Style.Width}, Height:{Style.Height}";
        }

        public interface IChartStyle
        {
            int Width { get; }

            int Height { get; }

            float Margin { get; }

            Color BackColor { get; }

            float PointSize { get; }

            Color PointFillColor { get; }

            float PointOutlinWidth { get; }

            Color PointOutlineColor { get; }

            float AxisLineWidth { get; }

            Color AxisLineColor { get; }

            Font TextFont { get; }

            Color TextColor { get; }

            int TextDecimalDigits { get; }

            float CurveLineWidth { get; }

            Color CurveLineColor { get; }
        }
    }
}
