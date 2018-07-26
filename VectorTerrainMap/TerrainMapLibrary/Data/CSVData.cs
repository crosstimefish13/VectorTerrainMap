using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TerrainMapLibrary.Arithmetic;
using TerrainMapLibrary.Localization;

namespace TerrainMapLibrary.Data
{
    public class CSVData
    {
        private List<List<object>> meta;


        public string FilePath { get; set; }


        public CSVData(string filePath)
        {
            FilePath = filePath;
            meta = new List<List<object>>() { new List<object>() };
        }

        public void LoadIntoMemory()
        {
            meta.Clear();

            // read to end, read each fields, these fields are splited with comma
            var reader = new StreamReader(GetStream());
            while (reader.EndOfStream == false)
            {
                string line = reader.ReadLine();
                var fields = line.Split(',').Cast<object>().ToList();
                meta.Add(fields);
            }
            reader.Dispose();

            if (meta.Count <= 0)
            {
                meta.Add(new List<object>());
            }
        }

        public T Field<T>(int row, int column)
        {
            return GetField<T>(row, column);
        }

        public List<T> Line<T>(int row)
        {
            // get each fields belongs to this row
            var line = new List<T>();
            for (int column = 0; column < meta[row].Count; column++)
            {
                var field = GetField<T>(row, column);
                line.Add(field);
            }
            return line;
        }

        public List<T> Column<T>(int column)
        {
            // get each fields belongs to this column
            var col = new List<T>();
            for (int row = 0; row < meta.Count; row++)
            {
                var field = GetField<T>(row, column);
                col.Add(field);
            }
            return col;
        }

        public List<List<T>> Fields<T>()
        {
            // the first dimension is row, and the second dimension is column
            var fields = new List<List<T>>();
            for (int row = 0; row < meta.Count; row++)
            {
                fields.Add(new List<T>());
                for (int column = 0; column < meta[row].Count; column++)
                {
                    var field = GetField<T>(row, column);
                    fields.Last().Add(field);
                }
            }
            return fields;
        }


        protected virtual Stream GetStream()
        {
            var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            return stream;
        }


        private T GetField<T>(int row, int column)
        {
            // ensure row and column is valid
            if (row >= meta.Count || column >= meta[row].Count)
            {
                return default(T);
            }

            // support string and any numbers
            Type t = typeof(T);
            object field = meta[row][column];
            if (t == typeof(string))
            {
                return (T)(object)Convert.ToString(field);
            }
            else if (t == typeof(GeoNumber))
            {
                return (T)(object)(new GeoNumber(Convert.ToString(field).Trim()));
            }
            else
            {
                throw new Exception($"{t.FullName} {ExceptionMessage.NotSupportDataType}");
            }
        }
    }
}
