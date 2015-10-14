using Microsoft.VisualStudio.TestTools.UnitTesting;
using SIC.Assembler.Utilities;

namespace Assembler.UnitTest.Providers.SymbolTable
{
    [TestClass]
    public class SymbolTableProviderTest
    {
        private const string FILE_DIR = "C:\\workspace\\CSC 354 - SIC(XE) Assembler\\Assembler.UnitTest\\Providers\\SymbolTable\\";

        [TestMethod]
        public void BuildSymbol()
        {
            var symbolTable = new SIC.Assembler.Providers.SymbolTable();
            var codeLines = HelperMethods.GetAllNonEmptyLines("symbol.txt");
            symbolTable.BuildSymbolTable(codeLines);

            Assert.IsTrue(symbolTable["Balling"].MFlag, "M Flag should be set");
        }

        [TestMethod]
        public void TestSymbolTree()
        {
            var symbolTable = new SIC.Assembler.Providers.SymbolTable();
            var codeLines = HelperMethods.GetAllNonEmptyLines("symbol.txt");
            var symbolLabels = HelperMethods.GetAllNonEmptyLines("test.txt");
            var output = "";

            symbolTable.BuildSymbolTable(codeLines);
            symbolTable.PerformLookupOnSymbolTree(symbolLabels, (symbol) =>
            {
                output += symbol + "\n";
            });
        }
    }
}