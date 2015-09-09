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
            var codeLines = HelperMethods.GetAllNonEmptyLines(filePath);

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

        public static void TestSymbolTree(Tree<Symbol> symbolTree, string filePath)
        {
            var symbolLabels = HelperMethods.GetAllNonEmptyLines(filePath);
            foreach (var symbolLabel in symbolLabels)
            {
                try
                {
                    var symbol = symbolTree.FindValue(symbolLabel);

                    if (symbol != null)
                    {
                        Console.WriteLine(symbol);
                    }
                    else
                    {
                        Console.WriteLine(string.Format("The symbol \"{0}\" was not found.", symbolLabel));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}