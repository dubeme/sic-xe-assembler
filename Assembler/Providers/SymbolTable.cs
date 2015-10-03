using SIC.Assembler.Model;
using SIC.Assembler.Utilities;
using SIC.Assembler.Utilities.Collections;
using System;

namespace SIC.Assembler.Providers
{
    /// <summary>
    ///
    /// </summary>
    public class SymbolTable
    {
        /// <summary>
        /// Builds the symbol tree.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="printFunction">The print function.</param>
        /// <param name="errorPrintFunction">The error print function.</param>
        /// <returns></returns>
        public static BinarySearchTree<Symbol> BuildSymbolTree(string filePath, Action<object> printFunction = null, Action<object> errorPrintFunction = null)
        {
            var symbolTree = new BinarySearchTree<Symbol>();
            var codeLines = HelperMethods.GetAllNonEmptyLines(filePath);

            printFunction = printFunction ?? Console.WriteLine;
            errorPrintFunction = errorPrintFunction ?? printFunction;

            foreach (var codeLine in codeLines)
            {
                try
                {
                    var duplicateSymbol = false;
                    var symbol = Symbol.Parse(codeLine);
                    symbolTree.Insert(symbol, (Symbol sym) =>
                    {
                        sym.MFlag = true;
                        duplicateSymbol = true;
                    });

                    if (duplicateSymbol)
                    {
                        printFunction(string.Format("Symbol {{{0}}} already exists. M Flag set to true", symbol.Label));
                    }
                    else
                    {
                        printFunction(string.Format("Inserted symbol {{{0}}}", symbol.Label));
                    }
                }
                catch (Exception ex)
                {
                    errorPrintFunction(ex.Message + "\n");
                }
            }

            return symbolTree;
        }

        /// <summary>
        /// Performs the lookup on symbol tree.
        /// </summary>
        /// <param name="symbolTree">The symbol tree.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="printFunction">The print function.</param>
        /// <param name="errorPrintFunction">The error print function.</param>
        public static void PerformLookupOnSymbolTree(BinarySearchTree<Symbol> symbolTree, string filePath, Action<object> printFunction = null, Action<object> errorPrintFunction = null)
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