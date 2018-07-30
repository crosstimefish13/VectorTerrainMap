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


        private static KrigingException ThrowException(string message, string paramName = null)
        {
            KrigingException exception = null;
            if (paramName == null) { exception = new KrigingException(message); }
            else { exception = new KrigingException(message, paramName); }

            throw exception;
        }
    }
}
