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
            return ThrowException(message, paramName);
        }

        internal static MatrixException NullObject(string paramName = null)
        {
            string message = "object reference must not be null.";
            return ThrowException(message, paramName);
        }

        internal static MatrixException NotSameSize(string paramName = null)
        {
            string message = "these matrix size must be same with each other.";
            return ThrowException(message, paramName);
        }

        internal static MatrixException InvalidMultiple(string paramName = null)
        {
            string message = "left matrix height must be equal with right matrix width.";
            return ThrowException(message, paramName);
        }

        internal static MatrixException NotSquare(string paramName = null)
        {
            string message = "matrix width must be same with height.";
            return ThrowException(message, paramName);
        }

        internal static MatrixException InvalidIndex(string paramName = null)
        {
            string message = "index must be 0 or more, and must be matrix width or height or more.";
            return ThrowException(message, paramName);
        }

        internal static MatrixException NotInverse(string paramName = null)
        {
            string message = "matrix cannot be inversed.";
            return ThrowException(message, paramName);
        }


        private static MatrixException ThrowException(string message, string paramName = null)
        {
            MatrixException exception = null;
            if (paramName == null) { exception = new MatrixException(message); }
            else { exception = new MatrixException(message, paramName); }

            throw exception;
        }
    }
}
