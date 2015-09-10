using SIC.Assembler.Utilities;
using SIC.Assembler.Utilities.BinarySearchTree;
using System;

namespace SIC.Assembler.Providers.SymbolTable
{
    public class SymbolTableProvider
    {
        public static Tree<Symbol> BuildSymbolTree(string filePath, Action<object> printFunction = null)
        {
            var symbolTree = new Tree<Symbol>();
            var codeLines = HelperMethods.GetAllNonEmptyLines(filePath);
            printFunction = printFunction ?? Console.WriteLine;

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
                    printFunction(ex.Message);
                }
            }

            return symbolTree;
        }

        public static void PerformLookupOnSymbolTree(Tree<Symbol> symbolTree, string filePath, Action<object> printFunction = null)
        {
            var symbolLabels = HelperMethods.GetAllNonEmptyLines(filePath);
            printFunction = printFunction ?? Console.WriteLine;

            foreach (var symbolLabel in symbolLabels)
            {
                try
                {
                    var symbol = symbolTree.FindValue(Symbol.ParseSymbolLabel(symbolLabel));

                    if (symbol != null)
                    {
                        printFunction(symbol);
                    }
                    else
                    {
                        printFunction(string.Format("The symbol \"{0}\" was not found.", symbolLabel));
                    }
                }
                catch (Exception ex)
                {
                    printFunction(ex.Message);
                }
            }
        }
    }
}