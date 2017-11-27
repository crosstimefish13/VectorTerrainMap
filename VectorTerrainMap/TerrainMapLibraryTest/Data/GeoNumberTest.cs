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
    }
}
