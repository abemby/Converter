using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using ConverterClasses;
using ConverterInterface;
using System;
using System.Collections.Generic;

namespace ConsoleApp1.Tests
{
    [TestClass()]
    public class DocConverterTests
    {
        [TestMethod()]
        public void CsvToXmlTest()
        {
                        
            var basePath = Directory.GetCurrentDirectory().ToLower().Replace("\\bin\\debug", "").Replace("\\bin\\release", "");
            var csvPath = string.Format("{0}\\{1}", basePath, "1.csv");
            var xmlPath = string.Format("{0}\\{1}", basePath, "V1.xml");
            var logPath = string.Format("{0}", basePath);
            
            var logger = new Logger(LogDevice.Console, logPath);

            var success = new Transform(logger).CSVToXML(csvPath, xmlPath, DateTime.Now.ToFileTime().ToString());                        

            // Verify
            var expected = File.Exists(xmlPath) && success;
            Assert.AreEqual(true, expected);

            logger = null;
        }

        //[TestMethod()]
        //public void CsvToXmlV2Test()
        //{
        //    // Setup
        //    var basePath = Directory.GetCurrentDirectory().ToLower().Replace("\\bin\\debug", "").Replace("\\bin\\release", "");
        //    var csvPath = string.Format("{0}\\{1}", basePath, "1.csv");
        //    var xmlPath = string.Format("{0}\\{1}", basePath, "V2.xml");


        //    // Test
        //    var success = DocumentConverter.Instance.CsvToXmlV2(csvPath, xmlPath);

        //    // Verify
        //    var expected = File.Exists(xmlPath) && success;
        //    Assert.AreEqual(true, expected);
        //}

        //[TestMethod()]
        //public void CsvToXmlV3Test()
        //{
        //    // Setup
        //    var basePath = Directory.GetCurrentDirectory().ToLower().Replace("\\bin\\debug", "").Replace("\\bin\\release", "");
        //    var csvPath = string.Format("{0}\\{1}", basePath, "1.csv");
        //    var xmlPath = string.Format("{0}\\{1}", basePath, "V3.xml");


        //    // Test
        //    var success = DocumentConverter.Instance.CsvToXmlV3(csvPath, xmlPath);

        //    // Verify
        //    var expected = File.Exists(xmlPath) && success;
        //    Assert.AreEqual(true, expected);
        //}
    }
}