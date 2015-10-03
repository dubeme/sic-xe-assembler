using Microsoft.VisualStudio.TestTools.UnitTesting;
using SIC.Assembler.Model;
using SIC.Assembler.Providers;

namespace Assembler.UnitTest.Providers.SymbolTable
{
    [TestClass]
    public class SymbolTableProviderTest
    {
        private const string FILE_DIR = "C:\\workspace\\CSC 354 - SIC(XE) Assembler\\Assembler.UnitTest\\Providers\\SymbolTable\\";

        [TestMethod]
        public void BuildSymbol()
        {
            var symbolTree = SIC.Assembler.Providers.SymbolTable.BuildSymbolTree(FILE_DIR + "symbol.txt");
            Assert.IsTrue(symbolTree.FindValue(Symbol.ParseSymbolLabel("Balling")).MFlag, "M Flag should be set");
        }

        [TestMethod]
        public void TestSymbolTree()
        {
            var symbolTree = SIC.Assembler.Providers.SymbolTable.BuildSymbolTree(FILE_DIR + "symbol.txt");
            string output = "";
            SIC.Assembler.Providers.SymbolTable.PerformLookupOnSymbolTree(symbolTree, FILE_DIR + "test.txt", (symbol) =>
            {
                output += symbol + "\n";
            });
        }
    }
}