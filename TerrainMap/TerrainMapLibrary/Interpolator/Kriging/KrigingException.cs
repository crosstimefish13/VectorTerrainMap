using System;

namespace TerrainMapLibrary.Interpolator.Kriging
{
    public class KrigingException : ArgumentException
    {
        public KrigingException()
            : base()
        { }

        internal KrigingException(string message, string paramName = null)
            : base(message, paramName)
        { }


        internal static KrigingException InvalidLagBins(string paramName = null)
        {
            string message = "lat bins must be 0 or more.";
            return ThrowException(message, paramName);
        }

        internal static KrigingException InvalidParallel(string paramName = null)
        {
            string message = "parallel must be 1 or more.";
            return ThrowException(message, paramName);
        }

        internal static KrigingException InvalidData(string paramName = null)
        {
            string message = "data item count must be 2 or more.";
            return ThrowException(message, paramName);
        }

        internal static KrigingException InvalidCacheVersion(string paramName = null)
        {
            string message = "cache version must not be null or empty.";
            return ThrowException(message, paramName);
        }

        internal static KrigingException InvalidRecordLength(string paramName = null)
        {
            string message = "record length must be more than 0.";
            return ThrowException(message, paramName);
        }

        internal static KrigingException InvalidMaxFileRecord(string paramName = null)
        {
            string message = "max file record must be more than 0.";
            return ThrowException(message, paramName);
        }

        internal static KrigingException InvalidMaxMemoryRercod(string paramName = null)
        {
            string message = "max memory record must be more than 0 and less than or equal with max file record.";
            return ThrowException(message, paramName);
        }

        internal static KrigingException InvalidRecord(string paramName = null)
        {
            string message = "record length must be same with spcified record length.";
            return ThrowException(message, paramName);
        }

        internal static KrigingException InvalidAdd(string paramName = null)
        {
            string message = "memory records are max, please flush memory records before adding new record.";
            return ThrowException(message, paramName);
        }


        private static KrigingException ThrowException(string message, string paramName = null)
        {
            KrigingException exception = null;
            if (paramName == null) { exception = new KrigingException(message); }
            else { exception = new KrigingException(message, paramName); }

            throw exception;
        }
    }
}
