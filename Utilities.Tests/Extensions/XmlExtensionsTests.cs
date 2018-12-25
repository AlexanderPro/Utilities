using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.Extensions;

namespace Utilities.Tests.Extensions
{
    [TestClass]
    public class XmlExtensionsTests
    {
        [TestMethod]
        public void GetAttributeValueOrNull()
        {
            var xml = $@"<?xml version=""1.0"" encoding=""UTF-8""?><root id=""12345""></root>";
            var document = new XmlDocument();
            document.LoadXml(xml);

            Assert.AreEqual("12345", document.DocumentElement.GetAttributeValueOrNull("id"));
            Assert.AreEqual(null, document.DocumentElement.GetAttributeValueOrNull("name"));
        }

        [TestMethod]
        public void RemoveAllNamespaces()
        {
            var source = $@"<?xml version=""1.0""?><root xmlns:xs=""http://www.w3.org/2001/XMLSchema"" targetNamespace=""myNamespace""><xs:element name=""Line1"" type=""string"" /></root>";
            var expected = $@"<root targetNamespace=""myNamespace""><element name=""Line1"" type=""string"" /></root>";
            var document = XElement.Parse(source);
            var result = document.RemoveAllNamespaces().ToString(SaveOptions.DisableFormatting);

            Assert.AreEqual(expected, result);
        }
    }
}
