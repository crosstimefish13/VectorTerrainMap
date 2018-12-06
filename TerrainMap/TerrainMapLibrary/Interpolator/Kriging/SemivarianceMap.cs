﻿using System;
using System.Collections.Generic;
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
        private FileSequence<Vector> vectors;


        public double LagBins { get; private set; }

        public long VectorCount
        {
            get { return vectors.Count; }
        }

        public Vector this[long index]
        {
            get { return vectors[index]; }
        }


        public void Close()
        {
            vectors.Close();
        }


        public static SemivarianceMap BuildOriginal(MapPointList data, string root = null, StepCounter counter = null)
        {
            // validate the data
            if (data == null || data.Count < 2)
            { throw new Exception("the count of data must be more than or equal with 2."); }

            if (root == null) { root = GetDefaultRoot(); }

            // element size is 16 B, each file size is 4 GB and memory size is 16 MB
            var vectors = FileSequence<Vector>.Generate(GetLagBinsPath(root, 0), 16, 67108864, 262144);

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
            var originalVectors = FileSequence<Vector>.Load(GetLagBinsPath(root, 0));
            var vectors = FileSequence<Vector>.Generate(GetLagBinsPath(root, lagBins),
                originalVectors.ElementLength,
                originalVectors.FileElement,
                originalVectors.MemoryElement);

            if (counter != null) { counter.Reset(originalVectors.Count, 0, "Building"); }

            // check each original elements
            var vectorGroup = new List<Vector>();
            double maxEuclidDistance = 0;
            for (long index = 0; index < originalVectors.Count; index++)
            {
                var vector = originalVectors[index];
                if (vectorGroup.Count == 0 || Common.DoubleCompare(vector.EuclidDistance, maxEuclidDistance) < 0)
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
                    vectors.Add(avgVector);

                    // reset the values
                    vectorGroup.Clear();
                    vectorGroup.Add(vector);
                    maxEuclidDistance += lagBins;
                }

                if (index == originalVectors.Count - 1)
                {
                    // add the last one
                    double avgEuclidDistance = vectorGroup.Sum(v => v.EuclidDistance) / vectorGroup.Count;
                    double avgSemivariance = vectorGroup.Sum(v => v.Semivariance) / vectorGroup.Count;
                    var avgVector = new Vector(avgEuclidDistance, avgSemivariance);
                    vectors.Add(avgVector);
                }

                if (counter != null) { counter.AddStep(); }
            }

            vectors.Flush();
            vectors.Close();

            if (counter != null) { counter.Reset(counter.StepLength, counter.StepLength, "Building"); }

            var map = Load(lagBins, root);
            return map;
        }

        public static SemivarianceMap Load(double lagBins, string root = null)
        {
            if (root == null) { root = GetDefaultRoot(); }

            var vectors = FileSequence<Vector>.Load(GetLagBinsPath(root, lagBins));
            var map = new SemivarianceMap() { vectors = vectors, LagBins = lagBins };

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
            vectors = null;
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
            string path = Path.Combine(root, lagBins.ToString());
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
