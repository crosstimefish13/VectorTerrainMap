using System;
using System.Collections.Generic;
using TerrainMapLibrary.Mathematics;

namespace TerrainMapLibrary.Interpolator.Data
{
    public class MapPointList
    {
        private List<MapPoint> mapPoints;

        public MapPoint this[int index]
        {
            get
            {
                return mapPoints[index];
            }
            set
            {
                mapPoints[index] = value ?? throw new ArgumentNullException();
            }
        }

        public int Count
        {
            get
            {
                return mapPoints.Count;
            }
        }

        public MapPointList()
        {
            mapPoints = new List<MapPoint>();
        }

        public void Add(MapPoint mapPoint)
        {
            if (mapPoint == null)
            {
                throw new ArgumentNullException();
            }

            mapPoints.Add(mapPoint);
        }

        public MapPointList RemoveAll(MapPoint item)
        {
            var result = new MapPointList();
            foreach (var mapPoint in mapPoints)
            {
                if (mapPoint != item)
                {
                    result.Add(mapPoint);
                }
            }

            return result;
        }

        public class MapPoint
        {
            public double X { get; set; }

            public double Y { get; set; }

            public double Z { get; set; }

            public MapPoint(double x = 0, double y = 0, double z = 0)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public static bool operator ==(MapPoint left, MapPoint right)
            {
                // compare object reference, then compare xyz
                if (left is null && right is null)
                {
                    return true;
                }
                else if (left is null || right is null)
                {
                    return false;
                }
                else if (Common.DoubleCompare(left.X, right.X) != 0)
                {
                    return false;
                }
                else if (Common.DoubleCompare(left.Y, right.Y) != 0)
                {
                    return false;
                }
                else if (Common.DoubleCompare(left.Z, right.Z) != 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            public static bool operator !=(MapPoint left, MapPoint right)
            {
                if (left == right)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            public override bool Equals(object obj)
            {
                if (obj == null || (obj is MapPoint) == false)
                {
                    return false;
                }
                else
                {
                    return this == (obj as MapPoint);
                }
            }

            public override int GetHashCode()
            {
                return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();
            }

            public override string ToString()
            {
                return $"X:{X}, Y:{Y}, Z:{Z}";
            }
        }
    }
}
