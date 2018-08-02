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

            FileStream stream = null;
            StreamReader reader = null;

            try
            {
                // read to end, read each fields, these fields are splited with comma
                stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                reader = new StreamReader(stream);

                int rowCount = -1;
                while (reader.EndOfStream == false)
                {
                    rowCount += 1;
                    string line = reader.ReadLine();

                    // continue if need to skipt first row
                    if (indicator.RowReadMode == Indicator.RowMode.SkipFirstRow && rowCount == 0) { continue; }

                    // continue if it is invalid row index
                    if (indicator.RowReadMode == Indicator.RowMode.UseRowsIndex
                            && indicator.RowsIndex.Contains(rowCount) == false)
                    { continue; }

                    // default values
                    var mapPoint = new MapPointList.MapPoint(indicator.InvalidField,
                        indicator.InvalidField,
                        indicator.InvalidField);

                    // read each xyz
                    var fields = line.Split(',').ToList();

                    if (indicator.XColumnIndex < fields.Count
                        && double.TryParse(fields[indicator.XColumnIndex].Trim(), out double x))
                    { mapPoint.X = x; }

                    if (indicator.YColumnIndex < fields.Count
                        && double.TryParse(fields[indicator.YColumnIndex].Trim(), out double y))
                    { mapPoint.Y = y; }

                    if (indicator.ZColumnIndex < fields.Count
                        && double.TryParse(fields[indicator.ZColumnIndex].Trim(), out double z))
                    { mapPoint.Z = z; }

                    mapPoints.Add(mapPoint);
                }
            }
            catch (Exception inner) { throw inner; }
            finally
            {
                if (reader != null) { reader.Dispose(); }
                if (stream != null) { stream.Dispose(); }
            }

            return mapPoints;
        }


        public class Indicator
        {
            public int XColumnIndex { get; set; }

            public int YColumnIndex { get; set; }

            public int ZColumnIndex { get; set; }

            public RowMode RowReadMode { get; set; }

            public List<int> RowsIndex { get; private set; }

            public double InvalidField { get; set; }


            public Indicator(int xColumnIndex = 0,
                int yColumnIndex = 1,
                int zColumnIndex = 2,
                RowMode rowReadMode = RowMode.FromStartToEnd,
                double invalidField = double.NaN)
            {
                XColumnIndex = xColumnIndex;
                YColumnIndex = yColumnIndex;
                ZColumnIndex = zColumnIndex;
                RowReadMode = rowReadMode;
                RowsIndex = new List<int>();
                InvalidField = invalidField;
            }


            public enum RowMode
            {
                FromStartToEnd = 0,
                SkipFirstRow = 1,
                UseRowsIndex = 2
            }
        }
    }
}
