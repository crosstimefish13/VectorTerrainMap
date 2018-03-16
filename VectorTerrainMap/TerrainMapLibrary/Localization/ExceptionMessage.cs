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

        public static string NotZeroDivisor
        {
            get
            {
                LoadIntoMemory();
                return messages["NotZeroDivisor"];
            }
        }

        public static string NotPositiveInteger
        {
            get
            {
                LoadIntoMemory();
                return messages["NotPositiveInteger"];
            }
        }

        public static string InvalidIterations
        {
            get
            {
                LoadIntoMemory();
                return messages["InvalidIterations"];
            }
        }

        public static string InvalidExponent
        {
            get
            {
                LoadIntoMemory();
                return messages["InvalidExponent"];
            }
        }

        public static string InvalidAntilogarithm
        {
            get
            {
                LoadIntoMemory();
                return messages["InvalidAntilogarithm"];
            }
        }

        public static string InvalidLogarithmBasis
        {
            get
            {
                LoadIntoMemory();
                return messages["InvalidLogarithmBasis"];
            }
        }

        public static string InvalidReserve
        {
            get
            {
                LoadIntoMemory();
                return messages["InvalidReserve"];
            }
        }

        public static string InvalidMatrixSize
        {
            get
            {
                LoadIntoMemory();
                return messages["InvalidMatrixSize"];
            }
        }

        public static string NullGeoNumber
        {
            get
            {
                LoadIntoMemory();
                return messages["NullGeoNumber"];
            }
        }

        public static string InvalidMatrixIndex
        {
            get
            {
                LoadIntoMemory();
                return messages["InvalidMatrixIndex"];
            }
        }

        public static string NotEqualMatrixSize
        {
            get
            {
                LoadIntoMemory();
                return messages["NotEqualMatrixSize"];
            }
        }

        public static string InvalidVectorSize
        {
            get
            {
                LoadIntoMemory();
                return messages["InvalidVectorSize"];
            }
        }

        public static string NotMultipleMatrixSize
        {
            get
            {
                LoadIntoMemory();
                return messages["NotMultipleMatrixSize"];
            }
        }

        public static string NotSquareMatrix
        {
            get
            {
                LoadIntoMemory();
                return messages["NotSquareMatrix"];
            }
        }

        public static string InvalidInverseMatrix
        {
            get
            {
                LoadIntoMemory();
                return messages["InvalidInverseMatrix"];
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
