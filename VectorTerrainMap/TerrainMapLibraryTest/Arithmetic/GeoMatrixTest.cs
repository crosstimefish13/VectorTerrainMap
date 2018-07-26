using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TerrainMapLibrary.Arithmetic;

namespace TerrainMapLibraryTest.Arithmetic
{
    [TestClass]
    public class GeoMatrixTest
    {
        [TestMethod]
        public void Initialize()
        {
            var matrix = new GeoMatrix(2, 2);
            Assert.AreEqual(matrix.Width, 2);
            Assert.AreEqual(matrix.Height, 2);

            matrix = new GeoMatrix(2, 3);
            Assert.AreEqual(matrix.Width, 2);
            Assert.AreEqual(matrix.Height, 3);

            matrix = new GeoMatrix(3, 2);
            Assert.AreEqual(matrix.Width, 3);
            Assert.AreEqual(matrix.Height, 2);

            bool isThrow = false;

            try
            {
                isThrow = false;
                matrix = new GeoMatrix(0, 0);
            }
            catch (Exception e)
            {
                isThrow = true;
                Assert.AreEqual(e.Message, "matrix size must be 1*1 or more.");
            }
            finally
            {
                Assert.AreEqual(isThrow, true);
            }

            try
            {
                isThrow = false;
                matrix = new GeoMatrix(1, 1, null);
            }
            catch (Exception e)
            {
                isThrow = true;
                Assert.AreEqual(e.Message, "geo number must not be null.");
            }
            finally
            {
                Assert.AreEqual(isThrow, true);
            }
        }

        [TestMethod]
        public void GetValue()
        {
            var matrix = new GeoMatrix(2, 2, new GeoNumber("1"));
            Assert.AreEqual(matrix.GetValue(0, 0), new GeoNumber("1"));

            matrix = new GeoMatrix(2, 3, new GeoNumber("1"));
            Assert.AreEqual(matrix.GetValue(2, 0), new GeoNumber("1"));

            matrix = new GeoMatrix(3, 2, new GeoNumber("1"));
            Assert.AreEqual(matrix.GetValue(0, 2), new GeoNumber("1"));

            bool isThrow = false;

            try
            {
                isThrow = false;
                matrix = new GeoMatrix(2, 2, new GeoNumber("1"));
                matrix.GetValue(-1, -1);
            }
            catch (Exception e)
            {
                isThrow = true;
                Assert.AreEqual(e.Message, "matrix index must be more then 0*0 and less then matrix size.");
            }
            finally
            {
                Assert.AreEqual(isThrow, true);
            }

            try
            {
                isThrow = false;
                matrix = new GeoMatrix(2, 2, new GeoNumber("1"));
                matrix.GetValue(2, 2);
            }
            catch (Exception e)
            {
                isThrow = true;
                Assert.AreEqual(e.Message, "matrix index must be more then 0*0 and less then matrix size.");
            }
            finally
            {
                Assert.AreEqual(isThrow, true);
            }
        }

        [TestMethod]
        public void SetValue()
        {
            var matrix = new GeoMatrix(2, 2);
            matrix.SetValue(1, 1, new GeoNumber("1"));
            Assert.AreEqual(matrix.GetValue(0, 0), new GeoNumber("0"));
            Assert.AreEqual(matrix.GetValue(1, 1), new GeoNumber("1"));

            matrix = new GeoMatrix(2, 3);
            matrix.SetValue(2, 1, new GeoNumber("1"));
            Assert.AreEqual(matrix.GetValue(0, 0), new GeoNumber("0"));
            Assert.AreEqual(matrix.GetValue(2, 1), new GeoNumber("1"));

            matrix = new GeoMatrix(3, 2);
            matrix.SetValue(1, 2, new GeoNumber("1"));
            Assert.AreEqual(matrix.GetValue(0, 0), new GeoNumber("0"));
            Assert.AreEqual(matrix.GetValue(1, 2), new GeoNumber("1"));

            bool isThrow = false;

            try
            {
                isThrow = false;
                matrix = new GeoMatrix(2, 2);
                matrix.SetValue(-1, -1, null);
            }
            catch (Exception e)
            {
                isThrow = true;
                Assert.AreEqual(e.Message, "matrix index must be more then 0*0 and less then matrix size.");
            }
            finally
            {
                Assert.AreEqual(isThrow, true);
            }

            try
            {
                isThrow = false;
                matrix = new GeoMatrix(2, 2);
                matrix.SetValue(2, 2, null);
            }
            catch (Exception e)
            {
                isThrow = true;
                Assert.AreEqual(e.Message, "matrix index must be more then 0*0 and less then matrix size.");
            }
            finally
            {
                Assert.AreEqual(isThrow, true);
            }

            try
            {
                isThrow = false;
                matrix = new GeoMatrix(2, 2);
                matrix.SetValue(1, 1, null);
            }
            catch (Exception e)
            {
                isThrow = true;
                Assert.AreEqual(e.Message, "geo number must not be null.");
            }
            finally
            {
                Assert.AreEqual(isThrow, true);
            }
        }

        [TestMethod]
        public void MultiplyScalar()
        {
            var matrix = new GeoMatrix(2, 2);
            matrix.SetValue(0, 1, new GeoNumber("1"));
            matrix.SetValue(1, 0, new GeoNumber("-1"));
            matrix.SetValue(1, 1, new GeoNumber("1.1"));
            matrix = matrix.Multiply(new GeoNumber("2"));
            Assert.AreEqual(matrix.GetValue(0, 0), new GeoNumber("0"));
            Assert.AreEqual(matrix.GetValue(0, 1), new GeoNumber("2"));
            Assert.AreEqual(matrix.GetValue(1, 0), new GeoNumber("-2"));
            Assert.AreEqual(matrix.GetValue(1, 1), new GeoNumber("2.2"));

            matrix = new GeoMatrix(1, 2);
            matrix.SetValue(1, 0, new GeoNumber("-1.1"));
            matrix = matrix.Multiply(new GeoNumber("2"));
            Assert.AreEqual(matrix.GetValue(0, 0), new GeoNumber("0"));
            Assert.AreEqual(matrix.GetValue(1, 0), new GeoNumber("-2.2"));

            matrix = new GeoMatrix(2, 1);
            matrix.SetValue(0, 1, new GeoNumber("0.1"));
            matrix = matrix.Multiply(new GeoNumber("2"));
            Assert.AreEqual(matrix.GetValue(0, 0), new GeoNumber("0"));
            Assert.AreEqual(matrix.GetValue(0, 1), new GeoNumber("0.2"));
        }

        [TestMethod]
        public void Transposition()
        {
            var matrix = new GeoMatrix(2, 2);
            matrix.SetValue(0, 1, new GeoNumber("1"));
            matrix.SetValue(1, 0, new GeoNumber("2"));
            matrix.SetValue(1, 1, new GeoNumber("3"));
            matrix = matrix.Transposition();
            Assert.AreEqual(matrix.GetValue(0, 0), new GeoNumber("0"));
            Assert.AreEqual(matrix.GetValue(0, 1), new GeoNumber("2"));
            Assert.AreEqual(matrix.GetValue(1, 0), new GeoNumber("1"));
            Assert.AreEqual(matrix.GetValue(1, 1), new GeoNumber("3"));

            matrix = new GeoMatrix(1, 2);
            matrix.SetValue(1, 0, new GeoNumber("1"));
            matrix = matrix.Transposition();
            Assert.AreEqual(matrix.GetValue(0, 0), new GeoNumber("0"));
            Assert.AreEqual(matrix.GetValue(0, 1), new GeoNumber("1"));

            matrix = new GeoMatrix(2, 1);
            matrix.SetValue(0, 1, new GeoNumber("1"));
            matrix = matrix.Transposition();
            Assert.AreEqual(matrix.GetValue(0, 0), new GeoNumber("0"));
            Assert.AreEqual(matrix.GetValue(1, 0), new GeoNumber("1"));
        }

        [TestMethod]
        public void ToUnit()
        {
            var matrix = new GeoMatrix(2, 2);
            matrix.SetValue(0, 1, new GeoNumber("1"));
            matrix.SetValue(1, 0, new GeoNumber("2"));
            matrix.SetValue(1, 1, new GeoNumber("3"));
            matrix = matrix.ToUnit();
            Assert.AreEqual(matrix.GetValue(0, 0), new GeoNumber("1"));
            Assert.AreEqual(matrix.GetValue(0, 1), new GeoNumber("0"));
            Assert.AreEqual(matrix.GetValue(1, 0), new GeoNumber("0"));
            Assert.AreEqual(matrix.GetValue(1, 1), new GeoNumber("1"));

            bool isThrow = false;

            try
            {
                isThrow = false;
                matrix = new GeoMatrix(2, 1);
                matrix = matrix.ToUnit();
            }
            catch (Exception e)
            {
                isThrow = true;
                Assert.AreEqual(e.Message, "the matrix must be a square one.");
            }
            finally
            {
                Assert.AreEqual(isThrow, true);
            }
        }

        [TestMethod]
        public void Inverse()
        {
            var matrix = new GeoMatrix(2, 2);
            matrix.SetValue(0, 1, new GeoNumber("1"));
            matrix.SetValue(1, 0, new GeoNumber("2"));
            matrix.SetValue(1, 1, new GeoNumber("3"));
            matrix = matrix.Inverse();
            Assert.AreEqual(matrix.GetValue(0, 0), new GeoNumber("-1.5"));
            Assert.AreEqual(matrix.GetValue(0, 1), new GeoNumber("0.5"));
            Assert.AreEqual(matrix.GetValue(1, 0), new GeoNumber("1"));
            Assert.AreEqual(matrix.GetValue(1, 1), new GeoNumber("0"));

            GeoNumber.Precision = 3;
            matrix = new GeoMatrix(3, 3);
            matrix.SetValue(0, 0, new GeoNumber("2"));
            matrix.SetValue(0, 1, new GeoNumber("2"));
            matrix.SetValue(0, 2, new GeoNumber("3"));
            matrix.SetValue(1, 0, new GeoNumber("4"));
            matrix.SetValue(1, 1, new GeoNumber("5"));
            matrix.SetValue(1, 2, new GeoNumber("6"));
            matrix.SetValue(2, 0, new GeoNumber("7"));
            matrix.SetValue(2, 1, new GeoNumber("8"));
            matrix.SetValue(2, 2, new GeoNumber("9"));
            matrix = matrix.Inverse();
            Assert.AreEqual(matrix.GetValue(0, 0), new GeoNumber("1.002"));
            Assert.AreEqual(matrix.GetValue(0, 1), new GeoNumber("-1.999"));
            Assert.AreEqual(matrix.GetValue(0, 2), new GeoNumber("0.999"));
            Assert.AreEqual(matrix.GetValue(1, 0), new GeoNumber("-2"));
            Assert.AreEqual(matrix.GetValue(1, 1), new GeoNumber("1"));
            Assert.AreEqual(matrix.GetValue(1, 2), new GeoNumber("0"));
            Assert.AreEqual(matrix.GetValue(2, 0), new GeoNumber("0.999"));
            Assert.AreEqual(matrix.GetValue(2, 1), new GeoNumber("0.666"));
            Assert.AreEqual(matrix.GetValue(2, 2), new GeoNumber("-0.666"));

            bool isThrow = false;

            try
            {
                isThrow = false;
                matrix = new GeoMatrix(2, 2);
                matrix.SetValue(0, 1, new GeoNumber("0"));
                matrix.SetValue(1, 0, new GeoNumber("2"));
                matrix.SetValue(1, 1, new GeoNumber("3"));
                matrix = matrix.Inverse();
            }
            catch (Exception e)
            {
                isThrow = true;
                Assert.AreEqual(e.Message, "the matrix cannot be inversed.");
            }
            finally
            {
                Assert.AreEqual(isThrow, true);
            }

            try
            {
                isThrow = false;
                matrix = new GeoMatrix(1, 2);
                matrix = matrix.Inverse();
            }
            catch (Exception e)
            {
                isThrow = true;
                Assert.AreEqual(e.Message, "the matrix must be a square one.");
            }
            finally
            {
                Assert.AreEqual(isThrow, true);
            }
        }

        [TestMethod]
        public void Add()
        {
            var matrix1 = new GeoMatrix(2, 2);
            matrix1.SetValue(0, 1, new GeoNumber("1"));
            matrix1.SetValue(1, 0, new GeoNumber("2"));
            matrix1.SetValue(1, 1, new GeoNumber("3"));
            var matrix2 = new GeoMatrix(2, 2, new GeoNumber("1"));
            matrix2.SetValue(0, 1, new GeoNumber("2"));
            matrix2.SetValue(1, 0, new GeoNumber("3"));
            matrix2.SetValue(1, 1, new GeoNumber("4"));
            var matrix = matrix1 + matrix2;
            Assert.AreEqual(matrix.GetValue(0, 0), new GeoNumber("1"));
            Assert.AreEqual(matrix.GetValue(0, 1), new GeoNumber("3"));
            Assert.AreEqual(matrix.GetValue(1, 0), new GeoNumber("5"));
            Assert.AreEqual(matrix.GetValue(1, 1), new GeoNumber("7"));

            matrix1 = new GeoMatrix(2, 3);
            matrix1.SetValue(1, 0, new GeoNumber("1"));
            matrix1.SetValue(2, 0, new GeoNumber("2"));
            matrix1.SetValue(0, 1, new GeoNumber("3"));
            matrix1.SetValue(1, 1, new GeoNumber("4"));
            matrix1.SetValue(2, 1, new GeoNumber("5"));
            matrix2 = new GeoMatrix(2, 3, new GeoNumber("1"));
            matrix2.SetValue(1, 0, new GeoNumber("2"));
            matrix2.SetValue(2, 0, new GeoNumber("3"));
            matrix2.SetValue(0, 1, new GeoNumber("4"));
            matrix2.SetValue(1, 1, new GeoNumber("5"));
            matrix2.SetValue(2, 1, new GeoNumber("6"));
            matrix = matrix1 + matrix2;
            Assert.AreEqual(matrix.GetValue(0, 0), new GeoNumber("1"));
            Assert.AreEqual(matrix.GetValue(1, 0), new GeoNumber("3"));
            Assert.AreEqual(matrix.GetValue(2, 0), new GeoNumber("5"));
            Assert.AreEqual(matrix.GetValue(0, 1), new GeoNumber("7"));
            Assert.AreEqual(matrix.GetValue(1, 1), new GeoNumber("9"));
            Assert.AreEqual(matrix.GetValue(2, 1), new GeoNumber("11"));

            matrix1 = new GeoMatrix(3, 2);
            matrix1.SetValue(0, 1, new GeoNumber("1"));
            matrix1.SetValue(0, 2, new GeoNumber("2"));
            matrix1.SetValue(1, 0, new GeoNumber("3"));
            matrix1.SetValue(1, 1, new GeoNumber("4"));
            matrix1.SetValue(1, 2, new GeoNumber("5"));
            matrix2 = new GeoMatrix(3, 2, new GeoNumber("1"));
            matrix2.SetValue(0, 1, new GeoNumber("2"));
            matrix2.SetValue(0, 2, new GeoNumber("3"));
            matrix2.SetValue(1, 0, new GeoNumber("4"));
            matrix2.SetValue(1, 1, new GeoNumber("5"));
            matrix2.SetValue(1, 2, new GeoNumber("6"));
            matrix = matrix1 + matrix2;
            Assert.AreEqual(matrix.GetValue(0, 0), new GeoNumber("1"));
            Assert.AreEqual(matrix.GetValue(0, 1), new GeoNumber("3"));
            Assert.AreEqual(matrix.GetValue(0, 2), new GeoNumber("5"));
            Assert.AreEqual(matrix.GetValue(1, 0), new GeoNumber("7"));
            Assert.AreEqual(matrix.GetValue(1, 1), new GeoNumber("9"));
            Assert.AreEqual(matrix.GetValue(1, 2), new GeoNumber("11"));

            bool isThrow = false;

            try
            {
                isThrow = false;
                matrix1 = new GeoMatrix(2, 2);
                matrix2 = new GeoMatrix(3, 2);
                matrix = matrix1 + matrix2;
            }
            catch (Exception e)
            {
                isThrow = true;
                Assert.AreEqual(e.Message, "the size of two matrix must be same with each other.");
            }
            finally
            {
                Assert.AreEqual(isThrow, true);
            }
        }

        [TestMethod]
        public void Sub()
        {
            var matrix1 = new GeoMatrix(2, 2);
            matrix1.SetValue(0, 1, new GeoNumber("1"));
            matrix1.SetValue(1, 0, new GeoNumber("2"));
            matrix1.SetValue(1, 1, new GeoNumber("3"));
            var matrix2 = new GeoMatrix(2, 2, new GeoNumber("-1"));
            matrix2.SetValue(0, 1, new GeoNumber("-2"));
            matrix2.SetValue(1, 0, new GeoNumber("-3"));
            matrix2.SetValue(1, 1, new GeoNumber("-4"));
            var matrix = matrix1 - matrix2;
            Assert.AreEqual(matrix.GetValue(0, 0), new GeoNumber("1"));
            Assert.AreEqual(matrix.GetValue(0, 1), new GeoNumber("3"));
            Assert.AreEqual(matrix.GetValue(1, 0), new GeoNumber("5"));
            Assert.AreEqual(matrix.GetValue(1, 1), new GeoNumber("7"));

            matrix1 = new GeoMatrix(2, 3);
            matrix1.SetValue(1, 0, new GeoNumber("1"));
            matrix1.SetValue(2, 0, new GeoNumber("2"));
            matrix1.SetValue(0, 1, new GeoNumber("3"));
            matrix1.SetValue(1, 1, new GeoNumber("4"));
            matrix1.SetValue(2, 1, new GeoNumber("5"));
            matrix2 = new GeoMatrix(2, 3, new GeoNumber("-1"));
            matrix2.SetValue(1, 0, new GeoNumber("-2"));
            matrix2.SetValue(2, 0, new GeoNumber("-3"));
            matrix2.SetValue(0, 1, new GeoNumber("-4"));
            matrix2.SetValue(1, 1, new GeoNumber("-5"));
            matrix2.SetValue(2, 1, new GeoNumber("-6"));
            matrix = matrix1 - matrix2;
            Assert.AreEqual(matrix.GetValue(0, 0), new GeoNumber("1"));
            Assert.AreEqual(matrix.GetValue(1, 0), new GeoNumber("3"));
            Assert.AreEqual(matrix.GetValue(2, 0), new GeoNumber("5"));
            Assert.AreEqual(matrix.GetValue(0, 1), new GeoNumber("7"));
            Assert.AreEqual(matrix.GetValue(1, 1), new GeoNumber("9"));
            Assert.AreEqual(matrix.GetValue(2, 1), new GeoNumber("11"));

            matrix1 = new GeoMatrix(3, 2);
            matrix1.SetValue(0, 1, new GeoNumber("1"));
            matrix1.SetValue(0, 2, new GeoNumber("2"));
            matrix1.SetValue(1, 0, new GeoNumber("3"));
            matrix1.SetValue(1, 1, new GeoNumber("4"));
            matrix1.SetValue(1, 2, new GeoNumber("5"));
            matrix2 = new GeoMatrix(3, 2, new GeoNumber("-1"));
            matrix2.SetValue(0, 1, new GeoNumber("-2"));
            matrix2.SetValue(0, 2, new GeoNumber("-3"));
            matrix2.SetValue(1, 0, new GeoNumber("-4"));
            matrix2.SetValue(1, 1, new GeoNumber("-5"));
            matrix2.SetValue(1, 2, new GeoNumber("-6"));
            matrix = matrix1 - matrix2;
            Assert.AreEqual(matrix.GetValue(0, 0), new GeoNumber("1"));
            Assert.AreEqual(matrix.GetValue(0, 1), new GeoNumber("3"));
            Assert.AreEqual(matrix.GetValue(0, 2), new GeoNumber("5"));
            Assert.AreEqual(matrix.GetValue(1, 0), new GeoNumber("7"));
            Assert.AreEqual(matrix.GetValue(1, 1), new GeoNumber("9"));
            Assert.AreEqual(matrix.GetValue(1, 2), new GeoNumber("11"));

            bool isThrow = false;

            try
            {
                isThrow = false;
                matrix1 = new GeoMatrix(2, 2);
                matrix2 = new GeoMatrix(3, 2);
                matrix = matrix1 - matrix2;
            }
            catch (Exception e)
            {
                isThrow = true;
                Assert.AreEqual(e.Message, "the size of two matrix must be same with each other.");
            }
            finally
            {
                Assert.AreEqual(isThrow, true);
            }
        }

        [TestMethod]
        public void Mul()
        {
            var matrix1 = new GeoMatrix(2, 2, new GeoNumber("0"));
            matrix1.SetValue(0, 1, new GeoNumber("1"));
            matrix1.SetValue(1, 0, new GeoNumber("2"));
            matrix1.SetValue(1, 1, new GeoNumber("3"));
            var matrix2 = new GeoMatrix(2, 2, new GeoNumber("1"));
            matrix2.SetValue(0, 1, new GeoNumber("2"));
            matrix2.SetValue(1, 0, new GeoNumber("3"));
            matrix2.SetValue(1, 1, new GeoNumber("4"));
            var matrix = matrix1 * matrix2;
            Assert.AreEqual(matrix.GetValue(0, 0), new GeoNumber("3"));
            Assert.AreEqual(matrix.GetValue(0, 1), new GeoNumber("4"));
            Assert.AreEqual(matrix.GetValue(1, 0), new GeoNumber("11"));
            Assert.AreEqual(matrix.GetValue(1, 1), new GeoNumber("16"));

            matrix1 = new GeoMatrix(3, 2, new GeoNumber("0"));
            matrix1.SetValue(0, 1, new GeoNumber("1"));
            matrix1.SetValue(0, 2, new GeoNumber("2"));
            matrix1.SetValue(1, 0, new GeoNumber("3"));
            matrix1.SetValue(1, 1, new GeoNumber("4"));
            matrix1.SetValue(1, 2, new GeoNumber("5"));
            matrix2 = new GeoMatrix(2, 3, new GeoNumber("1"));
            matrix2.SetValue(0, 1, new GeoNumber("2"));
            matrix2.SetValue(1, 0, new GeoNumber("3"));
            matrix2.SetValue(1, 1, new GeoNumber("4"));
            matrix2.SetValue(2, 0, new GeoNumber("5"));
            matrix2.SetValue(2, 1, new GeoNumber("6"));
            matrix = matrix1 * matrix2;
            Assert.AreEqual(matrix.GetValue(0, 0), new GeoNumber("13"));
            Assert.AreEqual(matrix.GetValue(0, 1), new GeoNumber("16"));
            Assert.AreEqual(matrix.GetValue(1, 0), new GeoNumber("40"));
            Assert.AreEqual(matrix.GetValue(1, 1), new GeoNumber("52"));

            matrix1 = new GeoMatrix(2, 3, new GeoNumber("0"));
            matrix1.SetValue(0, 1, new GeoNumber("1"));
            matrix1.SetValue(1, 0, new GeoNumber("2"));
            matrix1.SetValue(1, 1, new GeoNumber("3"));
            matrix1.SetValue(2, 0, new GeoNumber("4"));
            matrix1.SetValue(2, 1, new GeoNumber("5"));
            matrix2 = new GeoMatrix(3, 2, new GeoNumber("0"));
            matrix2.SetValue(0, 1, new GeoNumber("1"));
            matrix2.SetValue(0, 2, new GeoNumber("2"));
            matrix2.SetValue(1, 0, new GeoNumber("3"));
            matrix2.SetValue(1, 1, new GeoNumber("4"));
            matrix2.SetValue(1, 2, new GeoNumber("5"));
            matrix = matrix1 * matrix2;
            Assert.AreEqual(matrix.GetValue(0, 0), new GeoNumber("3"));
            Assert.AreEqual(matrix.GetValue(0, 1), new GeoNumber("4"));
            Assert.AreEqual(matrix.GetValue(0, 2), new GeoNumber("5"));
            Assert.AreEqual(matrix.GetValue(1, 0), new GeoNumber("9"));
            Assert.AreEqual(matrix.GetValue(1, 1), new GeoNumber("14"));
            Assert.AreEqual(matrix.GetValue(1, 2), new GeoNumber("19"));
            Assert.AreEqual(matrix.GetValue(2, 0), new GeoNumber("15"));
            Assert.AreEqual(matrix.GetValue(2, 1), new GeoNumber("24"));
            Assert.AreEqual(matrix.GetValue(2, 2), new GeoNumber("33"));

            bool isThrow = false;

            try
            {
                isThrow = false;
                matrix1 = new GeoMatrix(3, 2);
                matrix2 = new GeoMatrix(3, 2);
                matrix = matrix1 * matrix2;
            }
            catch (Exception e)
            {
                isThrow = true;
                Assert.AreEqual(e.Message, "the height of left matrix must be equal with the widht of right matrix.");
            }
            finally
            {
                Assert.AreEqual(isThrow, true);
            }
        }

        [TestMethod]
        public void Div()
        {
            var matrix1 = new GeoMatrix(2, 2);
            matrix1.SetValue(0, 1, new GeoNumber("1"));
            matrix1.SetValue(1, 0, new GeoNumber("2"));
            matrix1.SetValue(1, 1, new GeoNumber("3"));
            var matrix2 = new GeoMatrix(2, 2, new GeoNumber("1"));
            matrix2.SetValue(0, 1, new GeoNumber("2"));
            matrix2.SetValue(1, 0, new GeoNumber("3"));
            matrix2.SetValue(1, 1, new GeoNumber("4"));
            var matrix = matrix1 / matrix2;
            Assert.AreEqual(matrix.GetValue(0, 0), new GeoNumber("0"));
            Assert.AreEqual(matrix.GetValue(0, 1), new GeoNumber("-1"));
            Assert.AreEqual(matrix.GetValue(1, 0), new GeoNumber("1"));
            Assert.AreEqual(matrix.GetValue(1, 1), new GeoNumber("2"));

            bool isThrow = false;

            try
            {
                isThrow = false;
                matrix1 = new GeoMatrix(1, 2);
                matrix = matrix1 / matrix2;
            }
            catch (Exception e)
            {
                isThrow = true;
                Assert.AreEqual(e.Message, "the matrix must be a square one.");
            }
            finally
            {
                Assert.AreEqual(isThrow, true);
            }

            try
            {
                isThrow = false;
                matrix1 = new GeoMatrix(3, 3);
                matrix = matrix1 / matrix2;
            }
            catch (Exception e)
            {
                isThrow = true;
                Assert.AreEqual(e.Message, "the height of left matrix must be equal with the widht of right matrix.");
            }
            finally
            {
                Assert.AreEqual(isThrow, true);
            }
        }

        [TestMethod]
        public void Equal()
        {
            var matrix1 = new GeoMatrix(2, 2);
            var matrix2 = new GeoMatrix(2, 2);
            Assert.AreEqual(matrix1 == matrix2, true);

            matrix1 = new GeoMatrix(2, 2, new GeoNumber("1"));
            matrix2 = new GeoMatrix(2, 2);
            Assert.AreEqual(matrix1 == matrix2, false);

            matrix1 = new GeoMatrix(2, 1);
            matrix2 = new GeoMatrix(2, 2);
            Assert.AreEqual(matrix1 == matrix2, false);
        }

        [TestMethod]
        public void NotEqual()
        {
            var matrix1 = new GeoMatrix(2, 2);
            var matrix2 = new GeoMatrix(2, 2);
            Assert.AreEqual(matrix1 != matrix2, false);

            matrix1 = new GeoMatrix(2, 2, new GeoNumber("1"));
            matrix2 = new GeoMatrix(2, 2);
            Assert.AreEqual(matrix1 != matrix2, true);

            matrix1 = new GeoMatrix(2, 1);
            matrix2 = new GeoMatrix(2, 2);
            Assert.AreEqual(matrix1 != matrix2, true);
        }
    }
}
