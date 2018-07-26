using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TerrainMapLibrary.Arithmetic;
using TerrainMapLibrary.Localization;

namespace TerrainMapLibrary.Vector.Data
{
    public class CSVData
    {
        private List<List<string>> meta;


        public string FilePath { get; set; }

        public int Width
        {
            get { return meta[0].Count; }
        }

        public int Height
        {
            get { return meta.Count; }
        }


        public CSVData(string filePath)
        {
            FilePath = filePath;
            meta = new List<List<string>>() { new List<string>() };
        }

        public void LoadIntoMemory()
        {
            meta.Clear();

            // read to end, read each fields, these fields are splited with comma
            var reader = new StreamReader(GetStream());
            while (reader.EndOfStream == false)
            {
                string line = reader.ReadLine();
                var fields = line.Split(',').ToList();
                meta.Add(fields);
            }
            reader.Dispose();

            if (meta.Count <= 0)
            {
                meta.Add(new List<string>());
            }
        }

        public string Field(int row, int column)
        {
            return meta[row][column];
        }

        public List<string> Line(int row)
        {
            // get each fields belongs to this row
            var fields = new List<string>();
            for (int column = 0; column < meta[row].Count; column++)
            {
                fields.Add(meta[row][column]);
            }

            return fields;
        }

        public List<string> Column(int column)
        {
            // get each fields belongs to this column
            var fields = new List<string>();
            for (int row = 0; row < meta.Count; row++)
            {
                fields.Add(meta[row][column]);
            }

            return fields;
        }

        public List<List<string>> Fields()
        {
            // the first dimension is row, and the second dimension is column
            var fields = new List<List<string>>();
            for (int row = 0; row < meta.Count; row++)
            {
                fields.Add(new List<string>());
                for (int column = 0; column < meta[row].Count; column++)
                {
                    fields.Last().Add(meta[row][column]);
                }
            }

            return fields;
        }


        protected virtual Stream GetStream()
        {
            var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            return stream;
        }
    }
}
