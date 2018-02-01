using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerrainMapLibrary.Data;

namespace TerrainMapLibraryTest.Data
{
    [TestClass]
    public class GeoNumberTest
    {
        [TestMethod]
        public void Initialize()
        {
            GeoNumber.Precision = 0;
            var number = new GeoNumber("1");
            Assert.AreEqual(number.ToString(), "1");

            number = new GeoNumber("-1");
            Assert.AreEqual(number.ToString(), "-1");

            GeoNumber.Precision = 1;
            number = new GeoNumber("1.1");
            Assert.AreEqual(number.ToString(), "1.1");

            number = new GeoNumber("-1.1");
            Assert.AreEqual(number.ToString(), "-1.1");

            number = new GeoNumber("1.11");
            Assert.AreEqual(number.ToString(), "1.1");

            GeoNumber.Precision = 2;
            number = new GeoNumber("01.10");
            Assert.AreEqual(number.ToString(), "1.1");

            GeoNumber.Precision = 5;
            number = new GeoNumber("0");
            Assert.AreEqual(number.ToString(), "0");

            number = new GeoNumber("123");
            Assert.AreEqual(number.ToString(), "123");

            number = new GeoNumber("+123.45");
            Assert.AreEqual(number.ToString(), "123.45");

            number = new GeoNumber("-123.0103");
            Assert.AreEqual(number.ToString(), "-123.0103");

            number = new GeoNumber("000.01230");
            Assert.AreEqual(number.ToString(), "0.0123");

            number = new GeoNumber("12.1230456");
            Assert.AreEqual(number.ToString(), "12.12304");
        }

        [TestMethod]
        public void WrongInitialize()
        {
            bool isThrow = false;
            try
            {
                isThrow = false;
                GeoNumber.Precision = 0;
                var number = new GeoNumber(null);
            }
            catch (Exception e)
            {
                isThrow = true;
                Assert.AreEqual(e.Message, "it is a null value.");
            }
            finally
            {
                Assert.AreEqual(isThrow, true);
            }

            try
            {
                isThrow = false;
                GeoNumber.Precision = 0;
                var number = new GeoNumber(string.Empty);
            }
            catch (Exception e)
            {
                isThrow = true;
                Assert.AreEqual(e.Message, "it is a null value.");
            }
            finally
            {
                Assert.AreEqual(isThrow, true);
            }

            try
            {
                isThrow = false;
                GeoNumber.Precision = -1;
            }
            catch (Exception e)
            {
                isThrow = true;
                Assert.AreEqual(e.Message, "precision must be 0 or more.");
            }
            finally
            {
                Assert.AreEqual(isThrow, true);
            }

            var value = ".1";
            try
            {
                isThrow = false;
                GeoNumber.Precision = 0;
                var number = new GeoNumber(value);
            }
            catch (Exception e)
            {
                isThrow = true;
                Assert.AreEqual(e.Message, $"{value} is not a number.");
            }
            finally
            {
                Assert.AreEqual(isThrow, true);
            }
        }

        [TestMethod]
        public void Positive()
        {
            GeoNumber.Precision = 4;
            var number = new GeoNumber("0");
            Assert.AreEqual((+number).ToString(), "0");

            number = new GeoNumber("1");
            Assert.AreEqual((+number).ToString(), "1");

            number = new GeoNumber("-1");
            Assert.AreEqual((+number).ToString(), "1");

            number = new GeoNumber("0.123");
            Assert.AreEqual((+number).ToString(), "0.123");

            number = new GeoNumber("-1.23");
            Assert.AreEqual((+number).ToString(), "1.23");
        }

        [TestMethod]
        public void Negative()
        {
            GeoNumber.Precision = 4;
            var number = new GeoNumber("0");
            Assert.AreEqual((-number).ToString(), "0");

            number = new GeoNumber("1");
            Assert.AreEqual((-number).ToString(), "-1");

            number = new GeoNumber("-1");
            Assert.AreEqual((-number).ToString(), "1");

            number = new GeoNumber("0.123");
            Assert.AreEqual((-number).ToString(), "-0.123");

            number = new GeoNumber("-1.23");
            Assert.AreEqual((-number).ToString(), "1.23");
        }

        [TestMethod]
        public void Add()
        {
            GeoNumber.Precision = 4;
            var number1 = new GeoNumber("0");
            var number2 = new GeoNumber("1");
            Assert.AreEqual((number1 + number2).ToString(), "1");

            number1 = new GeoNumber("0");
            number2 = new GeoNumber("-1");
            Assert.AreEqual((number1 + number2).ToString(), "-1");

            number1 = new GeoNumber("1");
            number2 = new GeoNumber("1");
            Assert.AreEqual((number1 + number2).ToString(), "2");

            number1 = new GeoNumber("-1");
            number2 = new GeoNumber("-1");
            Assert.AreEqual((number1 + number2).ToString(), "-2");

            number1 = new GeoNumber("-1");
            number2 = new GeoNumber("1");
            Assert.AreEqual((number1 + number2).ToString(), "0");

            number1 = new GeoNumber("1");
            number2 = new GeoNumber("-2");
            Assert.AreEqual((number1 + number2).ToString(), "-1");

            number1 = new GeoNumber("-1");
            number2 = new GeoNumber("2");
            Assert.AreEqual((number1 + number2).ToString(), "1");

            number1 = new GeoNumber("1");
            number2 = new GeoNumber("9");
            Assert.AreEqual((number1 + number2).ToString(), "10");

            number1 = new GeoNumber("-1");
            number2 = new GeoNumber("-9");
            Assert.AreEqual((number1 + number2).ToString(), "-10");

            number1 = new GeoNumber("10");
            number2 = new GeoNumber("-9");
            Assert.AreEqual((number1 + number2).ToString(), "1");

            number1 = new GeoNumber("-10");
            number2 = new GeoNumber("9");
            Assert.AreEqual((number1 + number2).ToString(), "-1");

            number1 = new GeoNumber("0.1");
            number2 = new GeoNumber("1.01");
            Assert.AreEqual((number1 + number2).ToString(), "1.11");

            number1 = new GeoNumber("-0.1");
            number2 = new GeoNumber("1.01");
            Assert.AreEqual((number1 + number2).ToString(), "0.91");

            number1 = new GeoNumber("0.1001");
            number2 = new GeoNumber("1.8999");
            Assert.AreEqual((number1 + number2).ToString(), "2");

            number1 = new GeoNumber("0.001");
            number2 = new GeoNumber("-1.001");
            Assert.AreEqual((number1 + number2).ToString(), "-1");
        }

        [TestMethod]
        public void Sub()
        {
            GeoNumber.Precision = 4;
            var number1 = new GeoNumber("0");
            var number2 = new GeoNumber("1");
            Assert.AreEqual((number1 - number2).ToString(), "-1");

            number1 = new GeoNumber("0");
            number2 = new GeoNumber("-1");
            Assert.AreEqual((number1 - number2).ToString(), "1");

            number1 = new GeoNumber("1");
            number2 = new GeoNumber("1");
            Assert.AreEqual((number1 - number2).ToString(), "0");

            number1 = new GeoNumber("-1");
            number2 = new GeoNumber("-1");
            Assert.AreEqual((number1 - number2).ToString(), "0");

            number1 = new GeoNumber("-1");
            number2 = new GeoNumber("1");
            Assert.AreEqual((number1 - number2).ToString(), "-2");

            number1 = new GeoNumber("1");
            number2 = new GeoNumber("-2");
            Assert.AreEqual((number1 - number2).ToString(), "3");

            number1 = new GeoNumber("-1");
            number2 = new GeoNumber("2");
            Assert.AreEqual((number1 - number2).ToString(), "-3");

            number1 = new GeoNumber("1");
            number2 = new GeoNumber("9");
            Assert.AreEqual((number1 - number2).ToString(), "-8");

            number1 = new GeoNumber("-1");
            number2 = new GeoNumber("-9");
            Assert.AreEqual((number1 - number2).ToString(), "8");

            number1 = new GeoNumber("10");
            number2 = new GeoNumber("-9");
            Assert.AreEqual((number1 - number2).ToString(), "19");

            number1 = new GeoNumber("-10");
            number2 = new GeoNumber("9");
            Assert.AreEqual((number1 - number2).ToString(), "-19");

            number1 = new GeoNumber("0.1");
            number2 = new GeoNumber("1.01");
            Assert.AreEqual((number1 - number2).ToString(), "-0.91");

            number1 = new GeoNumber("-0.1");
            number2 = new GeoNumber("1.01");
            Assert.AreEqual((number1 - number2).ToString(), "-1.11");

            number1 = new GeoNumber("0.1001");
            number2 = new GeoNumber("1.8999");
            Assert.AreEqual((number1 - number2).ToString(), "-1.7998");

            number1 = new GeoNumber("0.001");
            number2 = new GeoNumber("-1.001");
            Assert.AreEqual((number1 - number2).ToString(), "1.002");
        }

        [TestMethod]
        public void Mul()
        {
            GeoNumber.Precision = 4;
            var number1 = new GeoNumber("0");
            var number2 = new GeoNumber("1");
            Assert.AreEqual((number1 * number2).ToString(), "0");

            number1 = new GeoNumber("0");
            number2 = new GeoNumber("-1");
            Assert.AreEqual((number1 * number2).ToString(), "0");

            number1 = new GeoNumber("1");
            number2 = new GeoNumber("1");
            Assert.AreEqual((number1 * number2).ToString(), "1");

            number1 = new GeoNumber("-1");
            number2 = new GeoNumber("-1");
            Assert.AreEqual((number1 * number2).ToString(), "1");

            number1 = new GeoNumber("-1");
            number2 = new GeoNumber("1");
            Assert.AreEqual((number1 * number2).ToString(), "-1");

            number1 = new GeoNumber("1");
            number2 = new GeoNumber("-2");
            Assert.AreEqual((number1 * number2).ToString(), "-2");

            number1 = new GeoNumber("-1");
            number2 = new GeoNumber("-2");
            Assert.AreEqual((number1 * number2).ToString(), "2");

            number1 = new GeoNumber("10");
            number2 = new GeoNumber("9");
            Assert.AreEqual((number1 * number2).ToString(), "90");

            number1 = new GeoNumber("-10");
            number2 = new GeoNumber("9");
            Assert.AreEqual((number1 * number2).ToString(), "-90");

            number1 = new GeoNumber("101");
            number2 = new GeoNumber("-10");
            Assert.AreEqual((number1 * number2).ToString(), "-1010");

            number1 = new GeoNumber("10");
            number2 = new GeoNumber("101");
            Assert.AreEqual((number1 * number2).ToString(), "1010");

            number1 = new GeoNumber("0.1");
            number2 = new GeoNumber("1.01");
            Assert.AreEqual((number1 * number2).ToString(), "0.101");

            number1 = new GeoNumber("9.9");
            number2 = new GeoNumber("-8.88");
            Assert.AreEqual((number1 * number2).ToString(), "-87.912");

            number1 = new GeoNumber("12.34");
            number2 = new GeoNumber("5678");
            Assert.AreEqual((number1 * number2).ToString(), "70066.52");
        }

        [TestMethod]
        public void Div()
        {
            GeoNumber.Precision = 4;
            var number1 = new GeoNumber("0");
            var number2 = new GeoNumber("0");
            bool isThrow = false;
            try
            {
                isThrow = false;
                var res = number1 / number2;
            }
            catch (Exception e)
            {
                isThrow = true;
                Assert.AreEqual(e.Message, "divisor should not be 0.");
            }
            finally
            {
                Assert.AreEqual(isThrow, true);
            }

            number1 = new GeoNumber("0");
            number2 = new GeoNumber("1");
            Assert.AreEqual((number1 / number2).ToString(), "0");

            number1 = new GeoNumber("0");
            number2 = new GeoNumber("-1");
            Assert.AreEqual((number1 / number2).ToString(), "0");

            number1 = new GeoNumber("1");
            number2 = new GeoNumber("1");
            Assert.AreEqual((number1 / number2).ToString(), "1");

            number1 = new GeoNumber("-1");
            number2 = new GeoNumber("-1");
            Assert.AreEqual((number1 / number2).ToString(), "1");

            number1 = new GeoNumber("-1");
            number2 = new GeoNumber("1");
            Assert.AreEqual((number1 / number2).ToString(), "-1");

            number1 = new GeoNumber("1");
            number2 = new GeoNumber("-2");
            Assert.AreEqual((number1 / number2).ToString(), "-0.5");

            number1 = new GeoNumber("-1");
            number2 = new GeoNumber("-2");
            Assert.AreEqual((number1 / number2).ToString(), "0.5");

            number1 = new GeoNumber("10");
            number2 = new GeoNumber("9");
            Assert.AreEqual((number1 / number2).ToString(), "1.1111");

            number1 = new GeoNumber("-10");
            number2 = new GeoNumber("9");
            Assert.AreEqual((number1 / number2).ToString(), "-1.1111");

            number1 = new GeoNumber("10010");
            number2 = new GeoNumber("-10");
            Assert.AreEqual((number1 / number2).ToString(), "-1001");

            number1 = new GeoNumber("10011");
            number2 = new GeoNumber("10");
            Assert.AreEqual((number1 / number2).ToString(), "1001.1");

            number1 = new GeoNumber("0.1");
            number2 = new GeoNumber("1.01");
            Assert.AreEqual((number1 / number2).ToString(), "0.099");

            number1 = new GeoNumber("9.9");
            number2 = new GeoNumber("-8.88");
            Assert.AreEqual((number1 / number2).ToString(), "-1.1148");

            number1 = new GeoNumber("12.34");
            number2 = new GeoNumber("5678");
            Assert.AreEqual((number1 / number2).ToString(), "0.0021");
        }

        [TestMethod]
        public void Mod()
        {
            GeoNumber.Precision = 4;
            var number1 = new GeoNumber("1.1");
            var number2 = new GeoNumber("1");
            bool isThrow = false;
            try
            {
                isThrow = false;
                var res = number1 % number2;
            }
            catch (Exception e)
            {
                isThrow = true;
                Assert.AreEqual(e.Message, $"{number1} must be positive integer number.");
            }
            finally
            {
                Assert.AreEqual(isThrow, true);
            }

            number1 = new GeoNumber("-1.1");
            number2 = new GeoNumber("1");
            isThrow = false;
            try
            {
                isThrow = false;
                var res = number1 % number2;
            }
            catch (Exception e)
            {
                isThrow = true;
                Assert.AreEqual(e.Message, $"{number1} must be positive integer number.");
            }
            finally
            {
                Assert.AreEqual(isThrow, true);
            }

            number1 = new GeoNumber("1");
            number2 = new GeoNumber("1.1");
            isThrow = false;
            try
            {
                isThrow = false;
                var res = number1 % number2;
            }
            catch (Exception e)
            {
                isThrow = true;
                Assert.AreEqual(e.Message, $"{number2} must be positive integer number.");
            }
            finally
            {
                Assert.AreEqual(isThrow, true);
            }

            number1 = new GeoNumber("1");
            number2 = new GeoNumber("-1.1");
            isThrow = false;
            try
            {
                isThrow = false;
                var res = number1 % number2;
            }
            catch (Exception e)
            {
                isThrow = true;
                Assert.AreEqual(e.Message, $"{number2} must be positive integer number.");
            }
            finally
            {
                Assert.AreEqual(isThrow, true);
            }

            number1 = new GeoNumber("1");
            number2 = new GeoNumber("0");
            isThrow = false;
            try
            {
                isThrow = false;
                var res = number1 % number2;
            }
            catch (Exception e)
            {
                isThrow = true;
                Assert.AreEqual(e.Message, $"divisor should not be 0.");
            }
            finally
            {
                Assert.AreEqual(isThrow, true);
            }

            number1 = new GeoNumber("1");
            number2 = new GeoNumber("1");
            Assert.AreEqual((number1 % number2).ToString(), "0");

            number1 = new GeoNumber("0");
            number2 = new GeoNumber("1");
            Assert.AreEqual((number1 % number2).ToString(), "0");

            number1 = new GeoNumber("10");
            number2 = new GeoNumber("4");
            Assert.AreEqual((number1 % number2).ToString(), "2");

            number1 = new GeoNumber("10");
            number2 = new GeoNumber("12");
            Assert.AreEqual((number1 % number2).ToString(), "10");
        }
    }
}
