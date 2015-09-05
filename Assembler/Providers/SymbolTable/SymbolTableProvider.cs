using SIC.Assembler.Utilities;
using SIC.Assembler.Utilities.BinarySearchTree;
using System;

namespace SIC.Assembler.Providers.SymbolTable
{
    public class SymbolTableProvider
    {
        public static Tree<Symbol> BuildSymbolTable(string filePath)
        {
            var symbolTree = new Tree<Symbol>();
            var codeLines = FileIO.GetAllNonEmptyLines(filePath);

            foreach (var codeLine in codeLines)
            {
                try
                {
                    var symbol = Symbol.Parse(codeLine);
                    symbolTree.Insert(symbol, (Symbol sym) => 
                    {
                        sym.MFlag = true;
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return symbolTree;
        }

        public static void TestSymbolTree(string filePath)
        {
            var codeLines = FileIO.GetAllNonEmptyLines(filePath);
            foreach (var codeLine in codeLines)
            {

            }
        }
    }
}