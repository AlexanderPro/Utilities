using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Utilities.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Utilities.Tests.IO
{
    [TestClass]
    public class LineReaderTests
    {
        [TestMethod]
        public void LineReaderCommonTest()
        {
            String sourceText = @"1
22
333
4444
55555
666666
7777777
88888888
999999999
0000000000";

            Int32 lineNumber = 0;
            foreach (String line in new LineReader(() => new StringReader(sourceText)))
            {
                lineNumber++;
                switch (lineNumber)
                {
                    case 1: Assert.AreEqual("1", line); break;
                    case 2: Assert.AreEqual("22", line); break;
                    case 3: Assert.AreEqual("333", line); break;
                    case 4: Assert.AreEqual("4444", line); break;
                    case 5: Assert.AreEqual("55555", line); break;
                    case 6: Assert.AreEqual("666666", line); break;
                    case 7: Assert.AreEqual("7777777", line); break;
                    case 8: Assert.AreEqual("88888888", line); break;
                    case 9: Assert.AreEqual("999999999", line); break;
                    case 10: Assert.AreEqual("0000000000", line); break;
                }
            }
        }
    }
}
