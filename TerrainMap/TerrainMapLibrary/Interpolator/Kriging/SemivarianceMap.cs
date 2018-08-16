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

            var client = new RectangleF(margin, margin, width - margin * 2f, height - margin * 2f);
            g.DrawRectangle(Pens.Blue, client.X, client.Y, client.Width, client.Height);

            g.DrawText("Euclid Distance", Color.Black, client.Right - 120f, client.Bottom - 20f);
            g.DrawText("Semivariance", Color.Black, client.Left, client.Top);

            client = new RectangleF(client.X, client.Y + 20f, client.Width, client.Height - 40f);
            g.DrawRectangle(Pens.Blue, client.X, client.Y, client.Width, client.Height);

            g.DrawRoundedLine(Color.Black, 3f, client.Left + 6f, client.Bottom - 6f, client.Right, client.Bottom - 6f);
            g.DrawRoundedLine(Color.Black, 3f, client.Right, client.Bottom - 6f, client.Right - 6f, client.Bottom - 12f);
            g.DrawRoundedLine(Color.Black, 3f, client.Right, client.Bottom - 6f, client.Right - 6f, client.Bottom);

            g.DrawRoundedLine(Color.Black, 3f, client.Left + 6f, client.Bottom - 6f, client.Left + 6f, client.Top);
            g.DrawRoundedLine(Color.Black, 3f, client.Left + 6f, client.Top, client.Left, client.Top + 6f);
            g.DrawRoundedLine(Color.Black, 3f, client.Left + 6f, client.Top, client.Left + 12f, client.Top + 6f);

            //g.DrawRoundedLine(Color.Black, 3f, margin, height - margin, width - margin, height - margin);
            //g.DrawRoundedLine(Color.Black, 3f, width - margin, height - margin, width - margin - 6f, height - margin - 6f);
            //g.DrawRoundedLine(Color.Black, 3f, width - margin, height - margin, width - margin - 6f, height - margin + 6f);

            //g.DrawRoundedLine(Color.Black, 3f, margin, height - margin, margin, margin);
            //g.DrawRoundedLine(Color.Black, 3f, margin, margin, margin - 6f, margin + 6f);
            //g.DrawRoundedLine(Color.Black, 3f, margin, margin, margin + 6f, margin + 6f);

            float scaleWidth = (width - margin * 2f) / 20f;
            float scaleHeight = (height - margin * 2f) / 20f;
            float scopeWidth = scaleWidth * (20f - 2f);
            float scopeHeight = scaleHeight * (20f - 2f);

            var brush = new SolidBrush(Color.Red);
            var pen = new Pen(Color.Black);
            double valueWidth = sequence[sequence.Count - 1].EuclidDistance - sequence[0].EuclidDistance;
            double valueHeight = sequence[sequence.Count - 1].Semivariance - sequence[0].Semivariance;
            for (long index = 0; index < sequence.Count; index++)
            {
                var vector = sequence[index];
                float valueX = scaleWidth + (float)((vector.EuclidDistance - sequence[0].EuclidDistance) * scopeWidth / valueWidth);
                float valueY = scaleHeight + (float)((vector.Semivariance - sequence[0].Semivariance) * scopeHeight / valueHeight);
                g.FillEllipse(brush, margin + valueX - 2f, height - margin - valueY - 2f, 4f, 4f);
                g.DrawEllipse(pen, margin + valueX - 2f, height - margin - valueY - 2f, 4f, 4f);
            }

            brush.Dispose();
            pen.Dispose();

            //brush = new SolidBrush(Color.Black);
            //var font = new Font("Arial", 12f, FontStyle.Regular);
            //g.DrawString("Euclid Distance", font, brush, width - margin - 120f, height - margin - 16f);
            //font.Dispose();
            //brush.Dispose();

            //g.DrawRectangle(Pens.Blue, margin, margin, width - margin * 2, height - margin * 2);


            //for (int i = 1; i < 20; i++)
            //{
            //    g.DrawRoundedLine(Color.Black, 3f, margin + scaleWidth * i, height - margin, margin + scaleWidth * i, height - margin - 3f);
            //}

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

            var map = Load(0, root);
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
