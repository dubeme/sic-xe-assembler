using Microsoft.VisualStudio.TestTools.UnitTesting;
using SIC.Assembler.Model;
using System;

namespace Assembler.UnitTest.Providers.SymbolTable
{
    [TestClass]
    public class SymbolTest
    {
        [TestMethod]
        public void Parse()
        {
            var symbol1 = Symbol.Parse("          12 label736743 False");
            var symbol2 = Symbol.Parse("            65        label736743 F             ");
            var symbol3 = Symbol.Parse("+99 label736743 f");
            var symbol4 = Symbol.Parse("-00 label736743 FalSE");
            var symbol5 = Symbol.Parse("09 label736743 0");
            var symbol6 = Symbol.Parse("09 label736743 1");

            Assert.AreEqual(symbol1.RelocatableFlag, false, "RFlag shouldn't be set");
            Assert.AreEqual(symbol2.RelocatableFlag, false, "RFlag shouldn't be set");
            Assert.AreEqual(symbol3.Label, "label7", "Label should be [label7]");
            Assert.AreEqual(symbol4.RelocatableFlag, false, "RFlag shouldn't be set");
            Assert.AreEqual(symbol4.Value, 0, "Value should be 0");
            Assert.AreEqual(symbol5.RelocatableFlag, false, "RFlag should not be set");
            Assert.AreEqual(symbol6.RelocatableFlag, true, "RFlag should be set");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_EmptyString()
        {
            Symbol.Parse(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_EmptyStringQuoted()
        {
            Symbol.Parse("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_IncorrectNumberOfTokens()
        {
            Symbol.Parse("Token1 Token2 Token3 Token4");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_null()
        {
            Symbol.Parse(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_WhiteSpaces()
        {
            Symbol.Parse("    ");
        }

        [TestMethod]
        public void ParseSymbolLabel()
        {
            Assert.AreEqual(Symbol.ParseSymbolLabel("abc"), "abc");
            Assert.AreEqual(Symbol.ParseSymbolLabel("abcdef"), "abcdef");
            Assert.AreEqual(Symbol.ParseSymbolLabel("symbollabel"), "symbol");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseSymbolLabel_InvalidArgument_EmptyString()
        {
            Symbol.ParseSymbolLabel(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseSymbolLabel_InvalidArgument_EmptyStringQuoted()
        {
            Symbol.ParseSymbolLabel("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseSymbolLabel_InvalidArgument_null()
        {
            Symbol.ParseSymbolLabel(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseSymbolLabel_InvalidArgument_NumbersOnly()
        {
            Symbol.ParseSymbolLabel("87263487623874");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseSymbolLabel_InvalidArgument_StartsWithNonAlphabetCharacter()
        {
            Symbol.ParseSymbolLabel("#$#$87263487623874");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseSymbolLabel_InvalidArgument_WithSpaces()
        {
            Symbol.ParseSymbolLabel("This is a sentence");
        }

        [TestMethod]
        public void ParseSymbolRFlag()
        {
            Assert.IsTrue(Symbol.ParseSymbolRFlag("T"));
            Assert.IsTrue(Symbol.ParseSymbolRFlag("t"));
            Assert.IsTrue(Symbol.ParseSymbolRFlag("1"));
            Assert.IsTrue(Symbol.ParseSymbolRFlag("True"));
            Assert.IsTrue(Symbol.ParseSymbolRFlag("true"));
            Assert.IsTrue(Symbol.ParseSymbolRFlag("TruE"));
            Assert.IsTrue(Symbol.ParseSymbolRFlag("truE"));

            Assert.IsFalse(Symbol.ParseSymbolRFlag("F"));
            Assert.IsFalse(Symbol.ParseSymbolRFlag("f"));
            Assert.IsFalse(Symbol.ParseSymbolRFlag("0"));
            Assert.IsFalse(Symbol.ParseSymbolRFlag("False"));
            Assert.IsFalse(Symbol.ParseSymbolRFlag("false"));
            Assert.IsFalse(Symbol.ParseSymbolRFlag("FalsE"));
            Assert.IsFalse(Symbol.ParseSymbolRFlag("FaLSe"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseSymbolRFlag_InvalidArgument_EmptyString()
        {
            Symbol.ParseSymbolRFlag(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseSymbolRFlag_InvalidArgument_EmptyStringQuoted()
        {
            Symbol.ParseSymbolRFlag("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseSymbolRFlag_InvalidArgument_null()
        {
            Symbol.ParseSymbolRFlag(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseSymbolRFlag_InvalidArgument_NumbersOnly()
        {
            Symbol.ParseSymbolRFlag("00000");
        }

        [TestMethod]
        public void ParseSymbolValue()
        {
            Assert.AreEqual(Symbol.ParseSymbolValue("-0"), 0);
            Assert.AreEqual(Symbol.ParseSymbolValue("+0"), 0);
            Assert.AreEqual(Symbol.ParseSymbolValue("            -25      "), -25);
            Assert.AreEqual(Symbol.ParseSymbolValue("+25"), 25);
            Assert.AreEqual(Symbol.ParseSymbolValue("25"), 25);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseSymbolValue_InvalidArgument_EmptyString()
        {
            Symbol.ParseSymbolValue(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseSymbolValue_InvalidArgument_EmptyStringQuoted()
        {
            Symbol.ParseSymbolValue("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseSymbolValue_InvalidArgument_NonNumericCharacters()
        {
            Symbol.ParseSymbolValue("#$#$87263487623874");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseSymbolValue_InvalidArgument_null()
        {
            Symbol.ParseSymbolValue(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseSymbolValue_InvalidArgument_OutOfBounds()
        {
            Symbol.ParseSymbolValue("87263487623874");
        }

        [TestMethod]
        public void TryParse()
        {
            Symbol symbol = null;

            Assert.IsFalse(Symbol.TryParse("", out symbol));
            Assert.IsTrue(Symbol.TryParse("4545 AEIOU false", out symbol));

            Assert.AreEqual(symbol.Label, "aeiou");
            Assert.AreEqual(symbol.Value, 4545);
            Assert.AreEqual(symbol.RelocatableFlag, false);
            Assert.AreEqual(symbol.MFlag, false);
        }
    }
}