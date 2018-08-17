using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using TerrainMapLibrary.Interpolator.Data;
using TerrainMapLibrary.Mathematics;
using TerrainMapLibrary.Utils;
using TerrainMapLibrary.Utils.Sequence;

namespace TerrainMapLibrary.Interpolator.Kriging
{
    public class SemivarianceMap
    {
        private FileSequence<Vector> sequence;


        public double LagBins { get; private set; }

        public long VectorCount
        {
            get { return sequence.Count; }
        }

        public Vector this[long index]
        {
            get { return sequence[index]; }
        }


        public void Close()
        {
            sequence.Close();
        }

        public Bitmap GenerateImage(int width, int height, float margin = 10f)
        {
            var image = new Bitmap(width, height);

            var g = Graphics.FromImage(image);
            g.Clear(Color.White);

            // draw top left and bottom right text
            var client = new RectangleF(margin, margin, width - margin * 2f, height - margin * 2f);
            g.DrawText("Euclid Distance", Color.Black, client.Right - 120f, client.Bottom - 20f);
            g.DrawText("Semivariance", Color.Black, client.Left, client.Top);

            // draw left and bottom arrow
            client = new RectangleF(client.X, client.Y + 20f, client.Width, client.Height - 40f);
            g.DrawRoundedLine(Color.Black, 3f, client.Left + 6f, client.Bottom - 6f, client.Right, client.Bottom - 6f);
            g.DrawRoundedLine(Color.Black, 3f, client.Right, client.Bottom - 6f, client.Right - 6f, client.Bottom - 12f);
            g.DrawRoundedLine(Color.Black, 3f, client.Right, client.Bottom - 6f, client.Right - 6f, client.Bottom);
            g.DrawRoundedLine(Color.Black, 3f, client.Left + 6f, client.Bottom - 6f, client.Left + 6f, client.Top);
            g.DrawRoundedLine(Color.Black, 3f, client.Left + 6f, client.Top, client.Left, client.Top + 6f);
            g.DrawRoundedLine(Color.Black, 3f, client.Left + 6f, client.Top, client.Left + 12f, client.Top + 6f);

            // draw points
            client = new RectangleF(client.X + 6f + margin, client.Y + margin, client.Width - 6f - margin * 2, client.Height - 6f - margin * 2);
            Vector minVector = new Vector(double.MaxValue, double.MaxValue);
            Vector maxVector = new Vector(double.MinValue, double.MinValue);
            for (long index = 0; index < sequence.Count; index++)
            {
                if (Common.DoubleCompare(sequence[index].EuclidDistance, minVector.EuclidDistance) < 0)
                { minVector.EuclidDistance = sequence[index].EuclidDistance; }

                if (Common.DoubleCompare(sequence[index].Semivariance, minVector.Semivariance) < 0)
                { minVector.Semivariance = sequence[index].Semivariance; }

                if (Common.DoubleCompare(sequence[index].EuclidDistance, maxVector.EuclidDistance) > 0)
                { maxVector.EuclidDistance = sequence[index].EuclidDistance; }

                if (Common.DoubleCompare(sequence[index].Semivariance, maxVector.Semivariance) > 0)
                { maxVector.Semivariance = sequence[index].Semivariance; }
            }

            double scaleWidht = client.Width / (maxVector.EuclidDistance - minVector.EuclidDistance);
            double scaleHeight = client.Height / (maxVector.Semivariance - minVector.Semivariance);
            for (long index = 0; index < sequence.Count; index++)
            {
                float drawX = client.Left + (float)((sequence[index].EuclidDistance - minVector.EuclidDistance) * scaleWidht);
                float drawY = client.Bottom - (float)((sequence[index].Semivariance - minVector.EuclidDistance) * scaleHeight);
                g.DrawPoint(Color.Red, Color.Black, 4f, 1f, drawX, drawY);
            }

            DrawExponential(g, client, Color.DarkGreen, 3f,
                maxVector.EuclidDistance - minVector.EuclidDistance, scaleWidht, scaleHeight);

            g.Dispose();

            return image;
        }

        //private GraphicsPath

        //private GraphicsPath CreateRoundedRectanglePath(Rectangle rectangle, int cornerRadius)
        //{
        //    var roundedRectangle = new GraphicsPath();
        //    roundedRectangle.AddArc(rectangle.X, rectangle.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
        //    roundedRectangle.AddLine(rectangle.X + cornerRadius, rectangle.Y, rectangle.Right - cornerRadius * 2, rectangle.Y);
        //    roundedRectangle.AddArc(rectangle.X + rectangle.Width - cornerRadius * 2, rectangle.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
        //    roundedRectangle.AddLine(rectangle.Right, rectangle.Y + cornerRadius * 2, rectangle.Right, rectangle.Y + rectangle.Height - cornerRadius * 2);
        //    roundedRectangle.AddArc(rectangle.X + rectangle.Width - cornerRadius * 2, rectangle.Y + rectangle.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
        //    roundedRectangle.AddLine(rectangle.Right - cornerRadius * 2, rectangle.Bottom, rectangle.X + cornerRadius * 2, rectangle.Bottom);
        //    roundedRectangle.AddArc(rectangle.X, rectangle.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
        //    roundedRectangle.AddLine(rectangle.X, rectangle.Bottom - cornerRadius * 2, rectangle.X, rectangle.Y + cornerRadius * 2);
        //    roundedRectangle.CloseFigure();
        //    return roundedRectangle;
        //}


        public static SemivarianceMap BuildOriginal(MapPointList data, string root = null,
            StepCounter counter = null)
        {
            // validate the data
            if (data == null || data.Count < 2)
            { throw new Exception("the count of data must be more than or equal with 2."); }

            if (root == null) { root = GetDefaultRoot(); }

            // element size is 16 B, each file size is 4 GB and memory size is 128 MB
            var vectors = FileSequence<Vector>.Generate(GetLagBinsPath(root, 0, false), 16, 67108864, 2097152);

            // do not use memory cache while adding elements
            vectors.EnableMemoryCache = false;

            if (counter != null) { counter.Reset((long)(data.Count - 1) * data.Count / 2, 0, "Calculating"); }

            // calculate each map point
            for (int i = 0; i < data.Count; i++)
            {
                var left = data[i];
                for (int j = i + 1; j < data.Count; j++)
                {
                    var right = data[j];

                    // calculate the Euclid distance and semivariance
                    double euclidDistance = Common.EuclidDistance(left.X, left.Y, right.X, right.Y);
                    double semivariance = Common.Semivariance(left.Z, right.Z);
                    var vector = new Vector(euclidDistance, semivariance);
                    vectors.Add(vector);

                    if (counter != null) { counter.AddStep(); }
                }
            }

            if (counter != null) { counter.Reset(counter.StepLength, counter.StepLength, "Calculating"); }

            vectors.Flush();

            // use memory cache when sorting
            vectors.EnableMemoryCache = true;

            // use heap sequencer to sort the sequence
            var sequencer = new HeapSequencer<Vector>(vectors, (left, right) =>
            {
                // compare the Euclid distance
                return Common.DoubleCompare(left.EuclidDistance, right.EuclidDistance);
            }, counter);

            sequencer.Sort();

            vectors.Close();

            var map = Load(0, false, root);
            return map;
        }

        public static SemivarianceMap Build(double lagBins, string root = null, StepCounter counter = null)
        {
            if (lagBins < 0 || Common.DoubleCompare(lagBins, 0) <= 0)
            { throw new Exception("lagBins must be more than 0"); }

            if (root == null) { root = GetDefaultRoot(); }

            // load original sequence and generate new sequence by using lag bins
            var originalSequence = FileSequence<Vector>.Load(GetLagBinsPath(root, 0, false));
            var sequence = FileSequence<Vector>.Generate(GetLagBinsPath(root, lagBins, false),
                originalSequence.ElementLength,
                originalSequence.FileElement,
                originalSequence.MemoryElement);

            if (counter != null) { counter.Reset(originalSequence.Count, 0, "Building"); }

            // check each original elements
            var vectorGroup = new List<Vector>();
            double maxEuclidDistance = 0;
            for (long index = 0; index < originalSequence.Count; index++)
            {
                var vector = originalSequence[index];
                if (vectorGroup.Count == 0
                    || Common.DoubleCompare(vector.EuclidDistance, maxEuclidDistance) < 0)
                {
                    vectorGroup.Add(vector);
                    maxEuclidDistance = vectorGroup[0].EuclidDistance + lagBins;
                }
                else
                {
                    // add a new element while overing lag bins
                    double avgEuclidDistance = vectorGroup.Sum(v => v.EuclidDistance) / vectorGroup.Count;
                    double avgSemivariance = vectorGroup.Sum(v => v.Semivariance) / vectorGroup.Count;
                    var avgVector = new Vector(avgEuclidDistance, avgSemivariance);
                    sequence.Add(avgVector);

                    // reset the values
                    vectorGroup.Clear();
                    vectorGroup.Add(vector);
                    maxEuclidDistance += lagBins;
                }

                if (index == originalSequence.Count - 1)
                {
                    // add the last one
                    double avgEuclidDistance = vectorGroup.Sum(v => v.EuclidDistance) / vectorGroup.Count;
                    double avgSemivariance = vectorGroup.Sum(v => v.Semivariance) / vectorGroup.Count;
                    var avgVector = new Vector(avgEuclidDistance, avgSemivariance);
                    sequence.Add(avgVector);
                }

                if (counter != null) { counter.AddStep(); }
            }

            sequence.Flush();
            sequence.Close();

            if (counter != null) { counter.Reset(counter.StepLength, counter.StepLength, "Building"); }

            var map = Load(lagBins, false, root);
            return map;
        }

        public static SemivarianceMap Normalize(double lagBins, string root = null, StepCounter counter = null)
        {
            if (lagBins < 0 || Common.DoubleCompare(lagBins, 0) < 0)
            { throw new Exception("lagBins must be more than or equal with 0"); }

            var sourceSequence = FileSequence<Vector>.Load(GetLagBinsPath(root, lagBins, false));
            var sequence = FileSequence<Vector>.Generate(GetLagBinsPath(root, lagBins, true),
                sourceSequence.ElementLength,
                sourceSequence.FileElement,
                sourceSequence.MemoryElement);

            var minVector = sourceSequence[0];

            var map = Load(lagBins, true, root);
            return map;
        }

        public static SemivarianceMap Load(double lagBins, bool normalized = true, string root = null)
        {
            if (root == null) { root = GetDefaultRoot(); }

            var sequence = FileSequence<Vector>.Load(GetLagBinsPath(root, lagBins, normalized));
            var map = new SemivarianceMap() { sequence = sequence, LagBins = lagBins };

            return map;
        }

        public static List<double> GetALlLagBins(string root = null)
        {
            if (root == null) { root = GetDefaultRoot(); }

            var lagBins = new List<double>();
            var entries = Directory.GetFileSystemEntries(root);
            foreach (var entry in entries)
            {
                if (Directory.Exists(entry) == true
                    && double.TryParse(Path.GetFileName(entry), out double lagBin) == true)
                { lagBins.Add(lagBin); }
            }

            return lagBins;
        }


        private SemivarianceMap()
        {
            sequence = null;
            LagBins = 0;
        }


        private static string GetDefaultRoot()
        {
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            string root = Path.Combine(Path.GetDirectoryName(assemblyLocation),
                Path.GetFileNameWithoutExtension(assemblyLocation));
            return root;
        }

        private static string GetLagBinsPath(string root, double lagBins, bool normalized)
        {
            string path = null;
            if (normalized == true) { path = Path.Combine(root, $"n{lagBins.ToString()}"); }
            else { path = Path.Combine(root, lagBins.ToString()); }

            return path;
        }


        private void DrawGaussian(Graphics g, RectangleF client, double minX, double maxX, double minY, double maxY, double valueWidth, double valueHeight)
        {
            double c0 = 10;
            double c = 450;
            double a = 0.009;
            double b = 0.023;

            var points = new List<PointF>();
            for (int i = 0; i < client.Width; i++)
            {
                double valueX = (maxX - minX) / client.Width * i;
                double valueY = c0;
                if (valueX > b)
                {
                    double cX = valueX - b;
                    valueY = c0 + c * (1 - Math.Exp(-(cX * cX / (a * a))));
                }

                float x = client.Left + (float)(valueX * valueWidth);
                float y = client.Bottom - (float)(valueY * valueHeight);

                points.Add(new PointF(x, y));
            }

            var path = new GraphicsPath();
            path.AddCurve(points.ToArray());
            g.DrawPath(Pens.Blue, path);
            path.Dispose();
        }

        private void DrawExponential(Graphics g, RectangleF client, Color color, float width,
            double valueWidthX, double scaleWidth, double scaleHeight)
        {
            double c0 = 10;
            double c = 550;
            double a = 0.01;
            double b = 0.0245;

            var points = new List<PointF>();
            for (int i = 0; i < client.Width; i++)
            {
                double valueX = valueWidthX * i / client.Width;
                double valueY = c0;
                if (Common.DoubleCompare(valueX, b) > 0)
                {
                    double formulaX = valueX - b;
                    valueY = c0 + c * (1 - Math.Exp((-formulaX) / a));
                }

                float x = client.Left + (float)(valueX * scaleWidth);
                float y = client.Bottom - (float)(valueY * scaleHeight);

                points.Add(new PointF(x, y));
            }

            var path = new GraphicsPath();
            path.AddCurve(points.ToArray());
            var pen = new Pen(color, width);

            g.DrawPath(pen, path);

            pen.Dispose();
            path.Dispose();
        }


        public class Vector : IElement
        {
            public double EuclidDistance { get; set; }

            public double Semivariance { get; set; }

            public int ArrayLength
            {
                get { return 16; }
            }


            public Vector()
                : this(0, 0)
            { }

            public Vector(double euclidDistance = 0, double semivariance = 0)
            {
                EuclidDistance = euclidDistance;
                Semivariance = semivariance;
            }


            public static bool operator ==(Vector left, Vector right)
            {
                // compare object reference
                if (left is null && right is null) { return true; }
                else if (left is null || right is null) { return false; }
                // compare values
                else if (Common.DoubleCompare(left.EuclidDistance, right.EuclidDistance) != 0) { return false; }
                else if (Common.DoubleCompare(left.Semivariance, right.Semivariance) != 0) { return false; }
                else { return true; }
            }

            public static bool operator !=(Vector left, Vector right)
            {
                return !(left == right);
            }


            public override bool Equals(object obj)
            {
                if (obj == null || !(obj is Vector)) { return false; }

                return this == (obj as Vector);
            }

            public override int GetHashCode()
            {
                return EuclidDistance.GetHashCode() + Semivariance.GetHashCode();
            }

            public override string ToString()
            {
                return $"EuclidDistance:{EuclidDistance}, Semivariance:{Semivariance}";
            }


            public void Initialize(byte[] array)
            {
                EuclidDistance = BitConverter.ToDouble(array, 0);
                Semivariance = BitConverter.ToDouble(array, 8);
            }

            public byte[] ToArray()
            {
                var list = new List<byte>();
                list.AddRange(BitConverter.GetBytes(EuclidDistance));
                list.AddRange(BitConverter.GetBytes(Semivariance));

                return list.ToArray();
            }
        }
    }
}
