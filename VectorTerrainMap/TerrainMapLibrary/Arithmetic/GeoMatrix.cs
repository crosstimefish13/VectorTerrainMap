using System;
using System.Collections.Generic;
using TerrainMapLibrary.Localization;

namespace TerrainMapLibrary.Arithmetic
{
    // wenku.baidu.com/view/50acee72f7ec4afe04a1dfa2.html
    // baike.baidu.com/item/%E7%9F%A9%E9%98%B5%E5%8F%98%E6%8D%A2/8185976?fr=aladdin
    // baike.baidu.com/item/%E9%80%86%E7%9F%A9%E9%98%B5/10481136?fr=aladdin
    // bbs.bccn.net/thread-291334-1-1.html
    // wenku.baidu.com/view/b02ea63290c69ec3d5bb75cf.html?from=search
    // www2.edu-edu.com.cn/lesson_crs78/self/j_0022/soft/ch0605.html
    public sealed class GeoMatrix
    {
        private List<List<GeoNumber>> arrays;


        public int Width
        {
            get
            {
                return arrays.Count;
            }
        }

        public int Height
        {
            get
            {
                return arrays[0].Count;
            }
        }


        public GeoMatrix(int width, int height)
            : this(width, height, new GeoNumber("0"))
        { }

        public GeoMatrix(int width, int height, GeoNumber fill)
        {
            // ensure size is valid
            if (width <= 0 || height <= 0)
            {
                throw new Exception(ExceptionMessage.InvalidMatrixSize);
            }

            // ensure fill number is valid
            if (fill == null)
            {
                throw new Exception(ExceptionMessage.NullGeoNumber);
            }

            // set the array size and fill values
            arrays = new List<List<GeoNumber>>();
            for (int row = 0; row < height; row++)
            {
                var rowArray = new List<GeoNumber>();
                for (int column = 0; column < width; column++)
                {
                    rowArray.Add(fill.Copy());
                }

                arrays.Add(rowArray);
            }
        }

        public GeoNumber GetValue(int row, int column)
        {
            // ensure index is valid
            if (row < 0 || row >= Height || column < 0 || column >= Width)
            {
                throw new Exception(ExceptionMessage.InvalidMatrixIndex);
            }

            return arrays[row][column].Copy();
        }

        public void SetValue(int row, int column, GeoNumber fill)
        {
            // ensure index is valid
            if (row < 0 || row >= Height || column < 0 || column >= Width)
            {
                throw new Exception(ExceptionMessage.InvalidMatrixIndex);
            }

            // ensure fill value is valid
            if (fill == null)
            {
                throw new Exception(ExceptionMessage.NullGeoNumber);
            }

            arrays[row][column] = fill.Copy();
        }

        public GeoMatrix Copy()
        {
            // copy each values
            var result = new GeoMatrix(Width, Height);
            for (int row = 0; row < Height; row++)
            {
                for (int column = 0; column < Width; column++)
                {
                    result.arrays[row][column] = arrays[row][column].Copy();
                }
            }

            return result;
        }

        public GeoMatrix Multiply(GeoNumber scalar)
        {
            // ensure value is valid
            if (scalar == null)
            {
                throw new Exception(ExceptionMessage.NullGeoNumber);
            }

            // scalar multiply each x-y value
            var result = new GeoMatrix(Width, Height);
            for (int row = 0; row < Height; row++)
            {
                for (int column = 0; column < Width; column++)
                {
                    result.arrays[row][column] = arrays[row][column] * scalar;
                }
            }

            return result;
        }

        public GeoMatrix Transposition()
        {
            // switch each row and column values
            var result = new GeoMatrix(Height, Width);
            for (int row = 0; row < Height; row++)
            {
                for (int column = 0; column < Width; column++)
                {
                    result.arrays[column][row] = arrays[row][column];
                }
            }

            return result;
        }

        public GeoMatrix ToUnit()
        {
            // ensure both left and right are square matrix
            if (Width != Height)
            {
                throw new Exception(ExceptionMessage.NotSquareMatrix);
            }

            // the item values on diagonal from top left to right bottom are 1, other values are 0, like
            // 1 0 0
            // 0 1 0
            // 0 0 1
            var result = new GeoMatrix(Width, Height);
            var one = new GeoNumber("1");
            for (int row = 0; row < Height; row++)
            {
                result.arrays[row][row] = one.Copy();
            }

            return result;
        }

        public GeoMatrix Inverse()
        {
            // matrix1 is the original, while matrix2 is unit matrix
            var matrix1 = Copy();
            var matrix2 = ToUnit();
            var zero = new GeoNumber("0");
            var one = new GeoNumber("1");

            // elementary row operations to make matrix1 to a top triangle one, like
            // 1 x x
            // 0 1 x
            // 0 0 1
            // meanwhile, do a same operations on the matrix2
            for (int row = 0; row < Height; row++)
            {
                var column = row;
                if (matrix1.arrays[row][column] == zero)
                {
                    // a special case that the value of matrix[row][column = row] is 0, but we want to make it to 1,
                    // so search items from matrix[row + 1][column = row] to matrix[height][column = row] to check if
                    // any values are not 0, then let matrix[row][column = row] add that value
                    var validRow = -1;
                    for (int otherRow = row + 1; otherRow < Height; otherRow++)
                    {
                        // find the item that the value is not 0, save the row index
                        if (matrix1.arrays[otherRow][column] != zero)
                        {
                            validRow = otherRow;
                            break;
                        }
                    }

                    // does not find the non 0 value, that means this matrix cannot be transformed to a top triangle 
                    // matrix, so it cannot be inversed
                    if (validRow == -1)
                    {
                        throw new Exception(ExceptionMessage.InvalidInverseMatrix);
                    }

                    // update current row
                    matrix1.RowAdd(row, validRow, one);
                    matrix2.RowAdd(row, validRow, one);
                }

                // make matrix[row][column = row] (the items on diagonal) to 1
                var value = one / matrix1.arrays[row][row];
                matrix1.RowMultiply(row, value);
                matrix2.RowMultiply(row, value);

                // make items from matrix[row + 1][column = row] to matrix[height][column = row] to value of 0,
                // because matrix[row][column = row] is 1 now, so let target item to sub itself
                for (int otherRow = row + 1; otherRow < Height; otherRow++)
                {
                    matrix1.RowAdd(otherRow, row, -matrix1.arrays[otherRow][column]);
                    matrix2.RowAdd(otherRow, row, -matrix1.arrays[otherRow][column]);
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
                    matrix1.RowAdd(otherRow, row, -matrix1.arrays[otherRow][column]);
                    matrix2.RowAdd(otherRow, row, -matrix1.arrays[otherRow][column]);
                }
            }

            // now we made the matrix1 to a unit matirx, meanwhile, matrix2 did the same operations with matrix1,
            // so matrix2 now is transformed to a new matrix, this new matrix is the inversed matrix with original 
            // matrix1
            return matrix2;
        }

        public static GeoMatrix operator +(GeoMatrix left, GeoMatrix right)
        {
            // ensure matrix size is equal
            if (left.Width != right.Width || left.Height != right.Height)
            {
                throw new Exception(ExceptionMessage.NotEqualMatrixSize);
            }

            // add each x-y value
            var result = new GeoMatrix(left.Width, left.Height);
            for (int row = 0; row < left.Height; row++)
            {
                for (int column = 0; column < left.Width; column++)
                {
                    result.arrays[row][column] = left.arrays[row][column] + right.arrays[row][column];
                }
            }

            return result;
        }

        public static GeoMatrix operator -(GeoMatrix left, GeoMatrix right)
        {
            var result = left + right.Multiply(new GeoNumber("-1"));
            return result;
        }

        public static GeoMatrix operator *(GeoMatrix left, GeoMatrix right)
        {
            // ensure left height is equal with right width
            if (left.Height != right.Width)
            {
                throw new Exception(ExceptionMessage.NotMultipleMatrixSize);
            }

            // if C = A * B, then C(ij) is row (i) of A multiply and add column (j) of B
            var result = new GeoMatrix(right.Width, left.Height);
            for (int row = 0; row < left.Height; row++)
            {
                for (int column = 0; column < right.Width; column++)
                {
                    // row of A multiply and add column of B
                    var value = new GeoNumber("0");
                    for (int link = 0; link < left.Width; link++)
                    {
                        value = value + left.arrays[row][link] * right.arrays[link][column];
                    }

                    result.arrays[row][column] = value;
                }
            }

            return result;
        }

        public static GeoMatrix operator /(GeoMatrix left, GeoMatrix right)
        {
            // ensure both left is square matrix
            if (left.Width != left.Height)
            {
                throw new Exception(ExceptionMessage.NotSquareMatrix);
            }

            // ensure left height is equal with right width
            if (left.Height != right.Width)
            {
                throw new Exception(ExceptionMessage.NotMultipleMatrixSize);
            }

            // get the inversed matrix of left, so left/right=inverse(left)*right, because left*inverse(left)=unit
            var result = left.Inverse() * right;
            return result;
        }


        private void RowSwitch(int row1, int row2)
        {
            // a row within the matrix can be switched with another row
            for (int column = 0; column < Width; column++)
            {
                var tempValue = arrays[row1][column].Copy();
                arrays[row1][column] = arrays[row2][column];
                arrays[row2][column] = tempValue;
            }
        }

        private void RowMultiply(int row, GeoNumber value)
        {
            // each element in a row can be multiplied by a non-zero constant
            for (int column = 0; column < Width; column++)
            {
                arrays[row][column] = arrays[row][column] * value;
            }
        }

        private void RowAdd(int row1, int row2, GeoNumber row2Multiply)
        {
            // A row can be replaced by the sum of that row and a multiple of another row
            for (int column = 0; column < Width; column++)
            {
                arrays[row1][column] = arrays[row1][column] + arrays[row2][column] * row2Multiply;
            }
        }
    }
}
