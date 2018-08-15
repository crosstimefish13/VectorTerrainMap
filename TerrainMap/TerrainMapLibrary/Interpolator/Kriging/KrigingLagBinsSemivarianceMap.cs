using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TerrainMapLibrary.Interpolator.Data;
using TerrainMapLibrary.Mathematics;
using TerrainMapLibrary.Utils;
using TerrainMapLibrary.Utils.Sequence;

namespace TerrainMapLibrary.Interpolator.Kriging
{
    public class KrigingLagBinsSemivarianceMap
    {
        private ListFileSequence sequence;


        public double LagBins { get; private set; }

        public long VectorCount
        {
            get { return sequence.Count; }
        }


        public void Close()
        {
            sequence.Close();
        }

        public MapPointList.MapPoint GetVector(long index)
        {
            var vector = ToVector(sequence[index]);
            return vector;
        }


        public static KrigingLagBinsSemivarianceMap BuildOriginal(MapPointList data, string root = null,
            StepCounter counter = null)
        {
            // validate the data
            if (data == null || data.Count < 2)
            { throw new Exception("the count of data must be more than or equal with 2."); }

            if (root == null) { root = GetDefaultRoot(); }

            // element size is 16 B, each file size is 4 GB and memory size is 128 MB
            var sequence = ListFileSequence.Generate(GetLagBinsPath(root, 0), 16, 67108864, 2097152);

            // do not use memory cache while adding elements
            sequence.EnableMemoryCache = false;

            if (counter != null) { counter.Reset((long)(data.Count - 1) * data.Count / 2, 0, "Calculating"); }

            // calculate each map point
            for (int i = 0; i < data.Count; i++)
            {
                var left = data[i];
                for (int j = i + 1; j < data.Count; j++)
                {
                    var right = data[j];

                    // calculate the Euclid distance and semivariance
                    double vectorX = Common.EuclidDistance(left.X, left.Y, right.X, right.Y);
                    double vectorY = Common.Semivariance(left.Z, right.Z);
                    var element = ToArray(vectorX, vectorY);
                    sequence.Add(element);

                    if (counter != null) { counter.AddStep(); }
                }
            }

            if (counter != null) { counter.Reset(counter.StepLength, counter.StepLength, "Calculating"); }

            sequence.Flush();

            // use memory cache when sorting
            sequence.EnableMemoryCache = true;

            // use heap sequencer to sort the sequence
            var sequencer = new HeapSequencer(sequence, (left, right) =>
            {
                var leftVector = ToVector(left);
                var rightVector = ToVector(right);

                // compare the Euclid distance
                if (leftVector.X > rightVector.X) { return 1; }
                else if (leftVector.X < rightVector.X) { return -1; }
                else { return 0; }
            }, counter);

            sequencer.Sort();

            sequence.Close();

            var map = Load(0, root);
            return map;
        }

        public static KrigingLagBinsSemivarianceMap Build(double lagBins, string root = null, StepCounter counter = null)
        {
            if (lagBins < 0 || Common.DoubleEqual(lagBins, 0))
            { throw new Exception("lagBins must be more than 0"); }

            if (root == null) { root = GetDefaultRoot(); }

            // load original sequence and generate new sequence by using lag bins
            var originalSequence = ListFileSequence.Load(GetLagBinsPath(root, 0));
            var sequence = ListFileSequence.Generate(GetLagBinsPath(root, lagBins),
                originalSequence.ElementLength,
                originalSequence.FileElement,
                originalSequence.MemoryElement);

            if (counter != null) { counter.Reset(originalSequence.Count, 0, "Building"); }

            // check each original elements
            double maxVectorX = 0;
            double sumVectorX = 0;
            double sumVectorY = 0;
            long elementCount = 0;
            for (long index = 0; index < originalSequence.Count; index++)
            {
                var vector = ToVector(originalSequence[index]);
                if (elementCount == 0)
                {
                    // set values when first original lement
                    maxVectorX = vector.X + lagBins;
                    sumVectorX = vector.X;
                    sumVectorY = vector.Y;
                    elementCount = 1;
                }
                else if (vector.X < maxVectorX)
                {
                    // add the vector
                    sumVectorX += vector.X;
                    sumVectorY += vector.Y;
                    elementCount += 1;
                }
                else
                {
                    // add a new element while overing lag bins
                    double avgVectorX = sumVectorX / elementCount;
                    double avgVectorY = sumVectorY / elementCount;
                    sequence.Add(ToArray(avgVectorX, avgVectorY));

                    // reset the values
                    maxVectorX += lagBins;
                    sumVectorX = vector.X;
                    sumVectorY = vector.Y;
                    elementCount = 1;
                }

                if (index == originalSequence.Count - 1)
                {
                    // add the last one
                    double avgVectorX = sumVectorX / elementCount;
                    double avgVectorY = sumVectorY / elementCount;
                    sequence.Add(ToArray(avgVectorX, avgVectorY));
                }

                if (counter != null) { counter.AddStep(); }
            }

            sequence.Flush();
            sequence.Close();

            if (counter != null) { counter.Reset(counter.StepLength, counter.StepLength, "Building"); }

            var map = Load(lagBins, root);
            return map;
        }

        public static KrigingLagBinsSemivarianceMap Load(double lagBins, string root = null)
        {
            if (root == null) { root = GetDefaultRoot(); }

            var sequence = ListFileSequence.Load(GetLagBinsPath(root, lagBins));
            var map = new KrigingLagBinsSemivarianceMap() { sequence = sequence, LagBins = lagBins };

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


        private KrigingLagBinsSemivarianceMap()
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

        private static string GetLagBinsPath(string root, double lagBins)
        {
            return Path.Combine(root, lagBins.ToString());
        }

        private static byte[] ToArray(params double[] values)
        {
            var bytes = new List<byte>();
            foreach (var value in values)
            { bytes.AddRange(BitConverter.GetBytes(value)); }

            return bytes.ToArray();
        }

        private static MapPointList.MapPoint ToVector(byte[] element)
        {
            double x = BitConverter.ToDouble(element, 0);
            double y = BitConverter.ToDouble(element, 8);
            var vector = new MapPointList.MapPoint(x, y);
            return vector;
        }
    }
}
