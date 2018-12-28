using System;
using System.Collections.Generic;
using System.Linq;

namespace TerrainMapLibrary.Mathematics
{
    // wenku.baidu.com/view/50acee72f7ec4afe04a1dfa2.html
    // baike.baidu.com/item/%E7%9F%A9%E9%98%B5%E5%8F%98%E6%8D%A2/8185976?fr=aladdin
    // baike.baidu.com/item/%E9%80%86%E7%9F%A9%E9%98%B5/10481136?fr=aladdin
    // bbs.bccn.net/thread-291334-1-1.html
    // wenku.baidu.com/view/b02ea63290c69ec3d5bb75cf.html?from=search
    // www2.edu-edu.com.cn/lesson_crs78/self/j_0022/soft/ch0605.html
    public sealed class Matrix
    {
        private List<List<double>> matrix;

        public int Width
        {
            get
            {
                return matrix[0].Count;
            }
        }

        public int Height
        {
            get
            {
                return matrix.Count;
            }
        }

        public double this[int row, int column]
        {
            get
            {
                return matrix[row][column];
            }
            set
            {
                matrix[row][column] = value;
            }
        }

        public Matrix(int width = 1, int height = 1, double fill = 0)
        {
            if (width <= 0 || height <= 0)
            {
                throw new Exception("matrix width and height must be more than or equal with 1.");
            }

            // initialize matrix
            matrix = new List<List<double>>();
            for (int row = 0; row < height; row++)
            {
                matrix.Add(new List<double>());
                for (int column = 0; column < width; column++)
                {
                    matrix.Last().Add(fill);
                }
            }
        }

        public static bool operator ==(Matrix left, Matrix right)
        {
            // compare object reference, then compare matrix size
            if (left is null && right is null)
            {
                return true;
            }
            else if (left is null || right is null)
            {
                return false;
            }
            else if (left.Width != right.Width || left.Height != right.Height)
            {
                return false;
            }
            else
            {
                // compare each fields
                for (int row = 0; row < left.Height; row++)
                {
                    for (int column = 0; column < left.Width; column++)
                    {
                        if (Common.DoubleCompare(left.matrix[row][column], right.matrix[row][column]) != 0)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        public static bool operator !=(Matrix left, Matrix right)
        {
            if (left == right)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static Matrix operator *(Matrix left, double right)
        {
            // each fields of left matrix multiple right
            var result = new Matrix(left.Width, left.Height);
            for (int row = 0; row < left.Height; row++)
            {
                for (int column = 0; column < left.Width; column++)
                {
                    result.matrix[row][column] = left.matrix[row][column] * right;
                }
            }

            return result;
        }

        public static Matrix operator +(Matrix left, Matrix right)
        {
            if (left.Width != right.Width || left.Height != right.Height)
            {
                throw new Exception("the size of left matrix must be same with right.");
            }

            // add each fields one by one
            var result = new Matrix(left.Width, left.Height);
            for (int row = 0; row < left.Height; row++)
            {
                for (int column = 0; column < left.Width; column++)
                {
                    result.matrix[row][column] = left.matrix[row][column] + right.matrix[row][column];
                }
            }

            return result;
        }

        public static Matrix operator -(Matrix left, Matrix right)
        {
            var result = left + right * (-1);
            return result;
        }

        public static Matrix operator *(Matrix left, Matrix right)
        {
            // left matrix height must be equal with right matrix width
            if (left.Height != right.Width)
            {
                throw new Exception("the height of left matrix must be equal with width of right.");
            }

            // if C = A * B, then C(ij) is row (i) of A multiply and add column (j) of B
            var result = new Matrix(right.Width, left.Height);
            for (int row = 0; row < left.Height; row++)
            {
                for (int column = 0; column < right.Width; column++)
                {
                    // row of A multiply and add column of B
                    result.matrix[row][column] = 0;
                    for (int link = 0; link < left.Width; link++)
                    {
                        result.matrix[row][column] += left.matrix[row][link] * right.matrix[link][column];
                    }
                }
            }

            return result;
        }

        public static Matrix operator /(Matrix left, Matrix right)
        {
            // get the inversed matrix of left, so left/right=inverse(left)*right, because left*inverse(left)=unit(left)
            var result = left.Inverse() * right;
            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || (obj is Matrix) == false)
            {
                return false;
            }
            else
            {
                return this == (obj as Matrix);
            }
        }

        public override int GetHashCode()
        {
            return matrix.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Width} * {Height}";
        }

        public Matrix Copy()
        {
            var result = new Matrix(Width, Height);
            for (int row = 0; row < Height; row++)
            {
                for (int column = 0; column < Width; column++)
                {
                    result.matrix[row][column] = matrix[row][column];
                }
            }

            return result;
        }

        public Matrix Transposition()
        {
            // switch each row and column fields
            var result = new Matrix(Height, Width);
            for (int row = 0; row < Height; row++)
            {
                for (int column = 0; column < Width; column++)
                {
                    result.matrix[column][row] = matrix[row][column];
                }
            }

            return result;
        }

        public Matrix ToUnit()
        {
            // matrix must be a square matrix
            if (Width != Height)
            {
                throw new Exception("the width of matrix must be equal with height.");
            }

            // the item values on diagonal from top left to right bottom are 1, other values are 0, like
            // 1 0 0
            // 0 1 0
            // 0 0 1
            var result = new Matrix(Width, Height);
            for (int row = 0; row < Height; row++)
            {
                result.matrix[row][row] = 1;
            }

            return result;
        }

        public Matrix Inverse()
        {
            // matrix1 is the original, while matrix2 is unit matrix
            var matrix1 = Copy();
            var matrix2 = ToUnit();

            // elementary row operations to make matrix1 to a top triangle one, like
            // 1 x x
            // 0 1 x
            // 0 0 1
            // meanwhile, do a same operations on the matrix2
            for (int row = 0; row < Height; row++)
            {
                var column = row;
                if (Common.DoubleCompare(matrix1.matrix[row][column], 0) == 0)
                {
                    // a special case that the value of matrix[row][column = row] is 0, but we want to make it to 1,
                    // so search items from matrix[row + 1][column = row] to matrix[height][column = row] to check if
                    // any values are not 0, then let matrix[row][column = row] add that value
                    var validRow = -1;
                    for (int otherRow = row + 1; otherRow < Height; otherRow++)
                    {
                        // find the item that the value is not 0, save the row index
                        if (Common.DoubleCompare(matrix1.matrix[otherRow][column], 0) != 0)
                        {
                            validRow = otherRow;
                            break;
                        }
                    }

                    // does not find the non 0 value, that means this matrix cannot be transformed to a top triangle 
                    // matrix, so it cannot be inversed
                    if (validRow == -1)
                    {
                        throw new Exception("matrix cannot be inversed.");
                    }

                    // update current row
                    matrix1.RowAdd(row, validRow);
                    matrix2.RowAdd(row, validRow);
                }

                // make matrix[row][column = row] (the items on diagonal) to 1
                double value = 1d / matrix1.matrix[row][row];
                matrix1.RowZoom(row, value);
                matrix2.RowZoom(row, value);

                // make items from matrix[row + 1][column = row] to matrix[height][column = row] to value of 0,
                // because matrix[row][column = row] is 1 now, so let target item to sub itself
                for (int otherRow = row + 1; otherRow < Height; otherRow++)
                {
                    value = 0d - matrix1.matrix[otherRow][column];
                    matrix1.RowAdd(otherRow, row, value);
                    matrix2.RowAdd(otherRow, row, value);
                }
            }

            // now make the top triangle matrix of matrix1 to a unit matrix, of cause, do a same operations on
            // the matrix2, let us handle each rows from bottom to top, because now the bottom row looks like
            // [0, ..., 0, 1], it is very easy to use this row to handle other rows
            for (int row = Height - 1; row >= 0; row--)
            {
                // make items from matrix[row - 1][column = row] to matrix[0][column = row] to value of 0,
                // because matrix[row][column = row] is 1 now, so let target item to sub itself
                var column = row;
                for (int otherRow = row - 1; otherRow >= 0; otherRow--)
                {
                    var value = 0d - matrix1.matrix[otherRow][column];
                    matrix1.RowAdd(otherRow, row, value);
                    matrix2.RowAdd(otherRow, row, value);
                }
            }

            // now we made the matrix1 to a unit matirx, meanwhile, matrix2 did the same operations with matrix1,
            // so matrix2 now is transformed to a new matrix, this new matrix is the inversed matrix with original 
            // matrix1
            return matrix2;
        }

        private void RowSwitch(int row1, int row2)
        {
            // a row within the matrix can be switched with another row
            for (int column = 0; column < Width; column++)
            {
                var tempValue = matrix[row1][column];
                matrix[row1][column] = matrix[row2][column];
                matrix[row2][column] = tempValue;
            }
        }

        private void RowZoom(int row, double value)
        {
            // each element in a row can be multiplied by a non-zero constant
            for (int column = 0; column < Width; column++)
            {
                matrix[row][column] = matrix[row][column] * value;
            }
        }

        private void RowAdd(int row1, int row2, double row2Scale = 1)
        {
            // A row can be replaced by the sum of that row and a multiple of another row
            for (int column = 0; column < Width; column++)
            {
                matrix[row1][column] = matrix[row1][column] + matrix[row2][column] * row2Scale;
            }
        }
    }
}
