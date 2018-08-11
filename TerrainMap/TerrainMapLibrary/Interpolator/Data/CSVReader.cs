using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TerrainMapLibrary.Interpolator.Data
{
    public static class CSVReader
    {
        public static MapPointList Read(string filePath, Indicator indicator = null)
        {
            var mapPoints = new MapPointList();
            if (indicator == null) { indicator = new Indicator(); }

            // read to end, read each fields, these fields are splited with comma
            var reader = new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read));

            int rowCount = -1;
            while (reader.EndOfStream == false)
            {
                rowCount += 1;
                string line = reader.ReadLine();

                // continue if it is invalid row index
                if (indicator.ExcludeRows.Contains(rowCount) == true) { continue; }

                // default values
                var mapPoint = new MapPointList.MapPoint(indicator.InvalidField,
                    indicator.InvalidField,
                    indicator.InvalidField);

                // read each xyz
                var fields = line.Split(',').ToList();

                if (indicator.XColumn < fields.Count
                    && double.TryParse(fields[indicator.XColumn].Trim(), out double x))
                { mapPoint.X = x; }

                if (indicator.YColumn < fields.Count
                    && double.TryParse(fields[indicator.YColumn].Trim(), out double y))
                { mapPoint.Y = y; }

                if (indicator.ZColumn < fields.Count
                    && double.TryParse(fields[indicator.ZColumn].Trim(), out double z))
                { mapPoint.Z = z; }

                mapPoints.Add(mapPoint);
            }

            reader.Close();
            reader.Dispose();

            return mapPoints;
        }


        public class Indicator
        {
            private int xColumnl;

            private int yColumnl;

            private int zColumnl;


            public int XColumn
            {
                get { return xColumnl; }
                set
                {
                    if (value < 0)
                    { throw new Exception("value must be more than or equal with 0."); }

                    xColumnl = value;
                }
            }

            public int YColumn
            {
                get { return yColumnl; }
                set
                {
                    if (value < 0)
                    { throw new Exception("value must be more than or equal with 0."); }

                    yColumnl = value;
                }
            }

            public int ZColumn
            {
                get { return zColumnl; }
                set
                {
                    if (value < 0)
                    { throw new Exception("value must be more than or equal with 0."); }

                    zColumnl = value;
                }
            }

            public IndexList ExcludeRows { get; private set; }

            public double InvalidField { get; set; }


            public Indicator()
            {
                xColumnl = 0;
                yColumnl = 1;
                zColumnl = 2;
                ExcludeRows = new IndexList();
                InvalidField = double.NaN;
            }


            public override bool Equals(object obj)
            {
                throw new NotSupportedException();
            }

            public override int GetHashCode()
            {
                return XColumn.GetHashCode() + YColumn.GetHashCode() + ZColumn.GetHashCode()
                     + ExcludeRows.GetHashCode() + InvalidField.GetHashCode();
            }

            public override string ToString()
            {
                return $"XColumn: {XColumn}, XColumn: {YColumn}, XColumn: {ZColumn}, ExcludeRows: {ExcludeRows.ToString()}, InvalidField: {InvalidField}";
            }


            public class IndexList
            {
                private List<int> indexes;


                public int this[int index]
                {
                    get { return indexes[index]; }
                    set
                    {
                        if (value < 0)
                        { throw new Exception("index value must be more than or equal with 0."); }

                        indexes[index] = value;
                    }
                }


                public IndexList()
                {
                    indexes = new List<int>();
                }


                public override bool Equals(object obj)
                {
                    throw new NotSupportedException();
                }

                public override int GetHashCode()
                {
                    return indexes.GetHashCode();
                }

                public override string ToString()
                {
                    return $"Count: {indexes.Count}";
                }


                public void Add(int item)
                {
                    if (item < 0)
                    { throw new Exception("index value must be more than or equal with 0."); }

                    indexes.Add(item);
                }

                public bool Contains(int item)
                {
                    return indexes.Contains(item);
                }
            }
        }
    }
}
