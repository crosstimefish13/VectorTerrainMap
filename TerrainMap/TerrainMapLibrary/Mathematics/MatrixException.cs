using System;

namespace TerrainMapLibrary.Mathematics
{
    public class MatrixException : ArgumentException
    {
        public MatrixException()
            : base()
        { }

        internal MatrixException(string message, string paramName = null)
            : base(message, paramName)
        { }


        internal static MatrixException InvalidSize(string paramName = null)
        {
            string message = "matrix size must be 1 * 1 or more.";
            var exception = new MatrixException(message, paramName);
            throw exception;
        }

        internal static MatrixException NullObject(string paramName = null)
        {
            string message = "object reference must not be null.";
            var exception = new MatrixException(message, paramName);
            throw exception;
        }

        internal static MatrixException NotSameSize(string paramName = null)
        {
            string message = "these matrix size must be same with each other.";
            var exception = new MatrixException(message, paramName);
            throw exception;
        }

        internal static MatrixException InvalidMultiple(string paramName = null)
        {
            string message = "left matrix height must be equal with right matrix width.";
            var exception = new MatrixException(message, paramName);
            throw exception;
        }
    }
}
