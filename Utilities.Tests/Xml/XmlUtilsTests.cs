using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.Xml;

namespace Utilities.Tests.Xml
{
    [TestClass]
    public class XmlUtilsTests
    {
        public class User
        {
            public int Age { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }
        }

        [TestMethod]
        public void SerializeAndDeserialize()
        {
            var user = new User
            {
                Age = 39,
                FirstName = "Alexander",
                LastName = "Illarionov"
            };

            var xml = XmlUtils.Serialize(user);

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(xml);
            writer.Flush();
            stream.Position = 0;

            var user2 = XmlUtils.Deserialize<User>(xml);
            var user3 = XmlUtils.Deserialize<User>(stream);

            Assert.AreEqual(user.Age, user2.Age);
            Assert.AreEqual(user.FirstName, user2.FirstName);
            Assert.AreEqual(user.LastName, user2.LastName);
            Assert.AreEqual(user.Age, user3.Age);
            Assert.AreEqual(user.FirstName, user3.FirstName);
            Assert.AreEqual(user.LastName, user3.LastName);
        }
    }
}
