using Microsoft.VisualStudio.TestTools.UnitTesting;
using SIC.Assembler.Providers.SymbolTable;
using System;

namespace Assembler.UnitTest.Providers.SymbolTable
{
    [TestClass]
    public class SymbolTableProviderTest
    {
        const string FILE_DIR = "C:\\workspace\\CSC 354 - SIC(XE) Assembler\\Assembler.UnitTest\\Providers\\SymbolTable\\";
        [TestMethod]
        public void BuildSymbol()
        {
            var symbolTree = SymbolTableProvider.BuildSymbolTree(FILE_DIR + "symbol.txt");
            Assert.IsTrue(symbolTree.FindValue(Symbol.ParseSymbolLabel("Balling")).MFlag, "M Flag should be set");
        }

        [TestMethod]
        public void TestSymbolTree()
        {
            var symbolTree = SymbolTableProvider.BuildSymbolTree(FILE_DIR + "symbol.txt");
            string output = "";
            SymbolTableProvider.PerformLookupOnSymbolTree(symbolTree, FILE_DIR + "test.txt", (symbol) => {
                output += symbol + "\n";
            });
        }
    }
}