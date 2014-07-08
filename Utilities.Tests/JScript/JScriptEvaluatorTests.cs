using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.JScript;

namespace Utilities.Tests.JScript
{
    [TestClass]
    public class JScriptEvaluatorTests
    {
        [TestMethod]
        public void TestLogicExpressions()
        {
            String expression1 = "1 == 1";
            String expression2 = "1 == 0";
            String expression3 = "Math.sqrt(Math.pow(8, 2)) == 8";
            String expression4 = "Math.max(1,2,3,4,5,6,7,8,9) == 9";
            String expression5 = "((1.2 + 1.3) < 3) && ((2 + 2) == 4)";

            Assert.IsTrue((Boolean)JScriptEvaluator.Eval(expression1));
            Assert.IsFalse((Boolean)JScriptEvaluator.Eval(expression2));
            Assert.IsTrue((Boolean)JScriptEvaluator.Eval(expression3));
            Assert.IsTrue((Boolean)JScriptEvaluator.Eval(expression4));
            Assert.IsTrue((Boolean)JScriptEvaluator.Eval(expression5));
        }

        [TestMethod]
        public void TestArithmeticalExpressions()
        {
            String expression1 = "1 + 1";
            String expression2 = "Math.pow(8, 2)";
            String expression3 = "Math.round(20.49)";
            String expression4 = "Math.round(20.5)";
            String expression5 = "Math.abs(-2)";
            String expression6 = "(1.5 + 0.5) * 2 - 4.5";

            Assert.AreEqual(2, (Int32)JScriptEvaluator.Eval(expression1));
            Assert.AreEqual(64f, (Double)JScriptEvaluator.Eval(expression2));
            Assert.AreEqual(20f, (Double)JScriptEvaluator.Eval(expression3));
            Assert.AreEqual(21f, (Double)JScriptEvaluator.Eval(expression4));
            Assert.AreEqual(2f, (Double)JScriptEvaluator.Eval(expression5));
            Assert.AreEqual(-0.5f, (Double)JScriptEvaluator.Eval(expression6));
        }

        [TestMethod]
        public void TestStringExpressions()
        {
            String expression1 = "'qwerty'.toUpperCase()";
            String expression2 = "'QWERTY'.toLowerCase()";
            String expression3 = "'abc'.replace('abc', 'def')";
            String expression4 = "'Hello world'.substring(6)";
            String expression5 = "'Hello world'.charAt(0)";

            Assert.AreEqual("QWERTY", (String)JScriptEvaluator.Eval(expression1));
            Assert.AreEqual("qwerty", (String)JScriptEvaluator.Eval(expression2));
            Assert.AreEqual("def", (String)JScriptEvaluator.Eval(expression3));
            Assert.AreEqual("world", (String)JScriptEvaluator.Eval(expression4));
            Assert.AreEqual("H", (String)JScriptEvaluator.Eval(expression5));
        }
    }
}
