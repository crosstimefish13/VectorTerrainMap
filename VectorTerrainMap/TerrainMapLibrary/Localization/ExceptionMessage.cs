using System.Collections.Generic;
using TerrainMapLibrary.Data;

namespace TerrainMapLibrary.Localization
{
    internal static class ExceptionMessage
    {
        private static Dictionary<string, string> messages;


        public static string NotSupportDataType
        {
            get
            {
                LoadIntoMemory();
                return messages["NotSupportDataType"];
            }
        }

        public static string NullValue
        {
            get
            {
                LoadIntoMemory();
                return messages["NullValue"];
            }
        }

        public static string InvalidPrecision
        {
            get
            {
                LoadIntoMemory();
                return messages["InvalidPrecision"];
            }
        }

        public static string NotNumber
        {
            get
            {
                LoadIntoMemory();
                return messages["NotNumber"];
            }
        }

        public static string NullInteger
        {
            get
            {
                LoadIntoMemory();
                return messages["NullInteger"];
            }
        }

        public static string NullDecimal
        {
            get
            {
                LoadIntoMemory();
                return messages["NullDecimal"];
            }
        }


        private static void LoadIntoMemory()
        {
            if (messages == null)
            {
                var data = new EmbeddedCSVData("TerrainMapLibrary.Localization.Exception.csv");
                data.LoadIntoMemory();
                var fields = data.Fields<string>();

                messages = new Dictionary<string, string>();
                foreach (var row in fields)
                {
                    messages.Add(row[0].Trim(), row[1].Trim());
                }
            }
        }
    }
}
