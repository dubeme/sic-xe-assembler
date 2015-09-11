using SIC.Assembler.Utilities;
using SIC.Assembler.Utilities.BinarySearchTree;
using System;

namespace SIC.Assembler.Providers.SymbolTable
{
    public class SymbolTreeProvider
    {
        public static Tree<Symbol> BuildSymbolTree(string filePath, Action<object> printFunction = null, Action<object> errorPrintFunction = null)
        {
            var symbolTree = new Tree<Symbol>();
            var codeLines = HelperMethods.GetAllNonEmptyLines(filePath);

            printFunction = printFunction ?? Console.WriteLine;
            errorPrintFunction = errorPrintFunction ?? printFunction;

            foreach (var codeLine in codeLines)
            {
                try
                {
                    var symbol = Symbol.Parse(codeLine);
                    symbolTree.Insert(symbol, (Symbol sym) =>
                    {
                        sym.MFlag = true;
                    });
                    printFunction(string.Format("Inserted symbol - {0}", symbol.Label));
                }
                catch (Exception ex)
                {
                    errorPrintFunction(ex.Message);
                }
            }

            return symbolTree;
        }

        public static void PerformLookupOnSymbolTree(Tree<Symbol> symbolTree, string filePath, Action<object> printFunction = null, Action<object> errorPrintFunction = null)
        {
            var symbolLabels = HelperMethods.GetAllNonEmptyLines(filePath);
            printFunction = printFunction ?? Console.WriteLine;
            errorPrintFunction = errorPrintFunction ?? printFunction;

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
                        errorPrintFunction(string.Format("The symbol \"{0}\" was not found.", symbolLabel));
                    }
                }
                catch (Exception ex)
                {
                    errorPrintFunction(ex.Message);
                }
            }
        }
    }
}