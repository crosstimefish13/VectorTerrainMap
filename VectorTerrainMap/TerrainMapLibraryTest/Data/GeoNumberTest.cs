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
            var number = new GeoNumber("1", 0);
            Assert.AreEqual(number.ToString(), "1");

            number = new GeoNumber("-1", 0);
            Assert.AreEqual(number.ToString(), "-1");

            number = new GeoNumber("1.1", 1);
            Assert.AreEqual(number.ToString(), "1.1");

            number = new GeoNumber("-1.1", 1);
            Assert.AreEqual(number.ToString(), "-1.1");

            number = new GeoNumber("1.11", 1);
            Assert.AreEqual(number.ToString(), "1.1");

            number = new GeoNumber("01.10", 2);
            Assert.AreEqual(number.ToString(), "1.1");


            number = new GeoNumber("0", 5);
            Assert.AreEqual(number.ToString(), "0");

            number = new GeoNumber("123", 5);
            Assert.AreEqual(number.ToString(), "123");

            number = new GeoNumber("+123.45", 5);
            Assert.AreEqual(number.ToString(), "123.45");

            number = new GeoNumber("-123.0103", 5);
            Assert.AreEqual(number.ToString(), "-123.0103");

            number = new GeoNumber("000.01230", 5);
            Assert.AreEqual(number.ToString(), "0.0123");

            number = new GeoNumber("12.1230456", 5);
            Assert.AreEqual(number.ToString(), "12.12304");
        }

        [TestMethod]
        public void WrongInitialize()
        {
            bool isThrow = false;
            try
            {
                isThrow = false;
                var number = new GeoNumber(null, 0);
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
                var number = new GeoNumber(string.Empty, 0);
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
                var number = new GeoNumber("1", -1);
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
                var number = new GeoNumber(value, 0);
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
        public void Add()
        {
            var number1 = new GeoNumber("0", 0);
            var number2 = new GeoNumber("1", 0);
            Assert.AreEqual((number1 + number2).ToString(), "1");

            number1 = new GeoNumber("0", 0);
            number2 = new GeoNumber("-1", 0);
            Assert.AreEqual((number1 + number2).ToString(), "-1");

            number1 = new GeoNumber("1", 0);
            number2 = new GeoNumber("1", 0);
            Assert.AreEqual((number1 + number2).ToString(), "2");

            number1 = new GeoNumber("-1", 0);
            number2 = new GeoNumber("-1", 0);
            Assert.AreEqual((number1 + number2).ToString(), "-2");

            number1 = new GeoNumber("-1", 0);
            number2 = new GeoNumber("1", 0);
            Assert.AreEqual((number1 + number2).ToString(), "0");

            number1 = new GeoNumber("1", 0);
            number2 = new GeoNumber("-2", 0);
            Assert.AreEqual((number1 + number2).ToString(), "-1");

            number1 = new GeoNumber("-1", 0);
            number2 = new GeoNumber("2", 0);
            Assert.AreEqual((number1 + number2).ToString(), "1");

            number1 = new GeoNumber("1", 0);
            number2 = new GeoNumber("9", 0);
            Assert.AreEqual((number1 + number2).ToString(), "10");

            number1 = new GeoNumber("-1", 0);
            number2 = new GeoNumber("-9", 0);
            Assert.AreEqual((number1 + number2).ToString(), "-10");

            number1 = new GeoNumber("10", 0);
            number2 = new GeoNumber("-9", 0);
            Assert.AreEqual((number1 + number2).ToString(), "1");

            number1 = new GeoNumber("-10", 0);
            number2 = new GeoNumber("9", 0);
            Assert.AreEqual((number1 + number2).ToString(), "-1");

            number1 = new GeoNumber("0.1", 1);
            number2 = new GeoNumber("1.01", 2);
            Assert.AreEqual((number1 + number2).ToString(), "1.11");

            number1 = new GeoNumber("-0.1", 1);
            number2 = new GeoNumber("1.01", 2);
            Assert.AreEqual((number1 + number2).ToString(), "0.91");

            number1 = new GeoNumber("0.1001", 4);
            number2 = new GeoNumber("1.8999", 4);
            Assert.AreEqual((number1 + number2).ToString(), "2");

            number1 = new GeoNumber("0.001", 4);
            number2 = new GeoNumber("-1.001", 4);
            Assert.AreEqual((number1 + number2).ToString(), "-1");
        }

        [TestMethod]
        public void Sub()
        {
            var number1 = new GeoNumber("0", 0);
            var number2 = new GeoNumber("1", 0);
            Assert.AreEqual((number1 - number2).ToString(), "-1");

            number1 = new GeoNumber("0", 0);
            number2 = new GeoNumber("-1", 0);
            Assert.AreEqual((number1 - number2).ToString(), "1");

            number1 = new GeoNumber("1", 0);
            number2 = new GeoNumber("1", 0);
            Assert.AreEqual((number1 - number2).ToString(), "0");

            number1 = new GeoNumber("-1", 0);
            number2 = new GeoNumber("-1", 0);
            Assert.AreEqual((number1 - number2).ToString(), "0");

            number1 = new GeoNumber("-1", 0);
            number2 = new GeoNumber("1", 0);
            Assert.AreEqual((number1 - number2).ToString(), "-2");

            number1 = new GeoNumber("1", 0);
            number2 = new GeoNumber("-2", 0);
            Assert.AreEqual((number1 - number2).ToString(), "3");

            number1 = new GeoNumber("-1", 0);
            number2 = new GeoNumber("2", 0);
            Assert.AreEqual((number1 - number2).ToString(), "-3");

            number1 = new GeoNumber("1", 0);
            number2 = new GeoNumber("9", 0);
            Assert.AreEqual((number1 - number2).ToString(), "-8");

            number1 = new GeoNumber("-1", 0);
            number2 = new GeoNumber("-9", 0);
            Assert.AreEqual((number1 - number2).ToString(), "8");

            number1 = new GeoNumber("10", 0);
            number2 = new GeoNumber("-9", 0);
            Assert.AreEqual((number1 - number2).ToString(), "19");

            number1 = new GeoNumber("-10", 0);
            number2 = new GeoNumber("9", 0);
            Assert.AreEqual((number1 - number2).ToString(), "-19");

            number1 = new GeoNumber("0.1", 1);
            number2 = new GeoNumber("1.01", 2);
            Assert.AreEqual((number1 - number2).ToString(), "-0.91");

            number1 = new GeoNumber("-0.1", 1);
            number2 = new GeoNumber("1.01", 2);
            Assert.AreEqual((number1 - number2).ToString(), "-1.11");

            number1 = new GeoNumber("0.1001", 4);
            number2 = new GeoNumber("1.8999", 4);
            Assert.AreEqual((number1 - number2).ToString(), "-1.7998");

            number1 = new GeoNumber("0.001", 4);
            number2 = new GeoNumber("-1.001", 4);
            Assert.AreEqual((number1 - number2).ToString(), "1.002");
        }
    }
}
