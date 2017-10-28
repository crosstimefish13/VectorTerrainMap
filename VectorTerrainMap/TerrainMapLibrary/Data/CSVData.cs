using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrainMapLibrary.Data
{
    public class CSVData
    {
        private string filePath;
        private List<List<object>> meta;

        public string FilePath
        {
            get
            {
                return filePath;
            }
            set
            {
                filePath = value;
            }
        }


        public CSVData(string filePath)
        {
            this.filePath = filePath;
            meta = new List<List<object>>() { new List<object>() };
        }

        public void LoadIntoMemory()
        {
            meta.Clear();

            var reader = new StreamReader(filePath);
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
            if (row >= meta.Count)
            {
                return null;
            }

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
            if (column >= meta.First().Count)
            {
                return null;
            }

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


        private T GetField<T>(int row, int column)
        {
            if (row >= meta.Count || column >= meta[row].Count)
            {
                return default(T);
            }

            Type t = typeof(T);
            object field = meta[row][column];
            if (t == typeof(string))
            {
                return (T)(object)Convert.ToString(field);
            }
            else if (t == typeof(short))
            {
                return (T)(object)Convert.ToInt16(field);
            }
            else if (t == typeof(int))
            {
                return (T)(object)Convert.ToInt32(field);
            }
            else if (t == typeof(long))
            {
                return (T)(object)Convert.ToInt64(field);
            }
            else if (t == typeof(decimal))
            {
                return (T)(object)Convert.ToDecimal(field);
            }
            else if (t == typeof(float))
            {
                return (T)(object)Convert.ToSingle(field);
            }
            else if (t == typeof(double))
            {
                return (T)(object)Convert.ToDouble(field);
            }
            else
            {
                throw new Exception($"{t.FullName} 是不支持的数据类型。");
            }
        }
    }
}
