using SIC.Assembler.Model;
using SIC.Assembler.Utilities.Collections;
using SIC.Assembler.Utilities.Model;
using System;

namespace SIC.Assembler.Providers
{
    /// <summary>
    ///
    /// </summary>
    public class SymbolTable
    {
        /// <summary>
        /// The _ symbol table
        /// </summary>
        private BinarySearchTree<Symbol> _SymbolTable = new BinarySearchTree<Symbol>();

        public Symbol this[string label]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(label))
                {
                    return null;
                }

                label = Symbol.ParseSymbolLabel(label).ToLower();
                return this._SymbolTable.FindValue(label);
            }
        }

        public Symbol AddSymbol(string label, int value, bool isRelocatable, bool allowDuplicates = true)
        {
            if (!allowDuplicates && this.ContainsSymbol(label))
            {
                throw new Exception(string.Format("Symbol {{{0}}} already exists.", label));
            }

            var symbol = Symbol.CreateSymbol(label, value, isRelocatable);
            this._SymbolTable.Insert(symbol, (Symbol sym) =>
            {
                sym.MFlag = true;
            });

            return symbol;
        }

        /// <summary>
        /// Builds the symbol table.
        /// </summary>
        /// <param name="codeLines">The code lines.</param>
        /// <param name="printFunction">The print function.</param>
        /// <param name="errorPrintFunction">The error print function.</param>
        public void BuildSymbolTable(string[] codeLines, Action<object> printFunction = null, Action<object> errorPrintFunction = null)
        {
            this._SymbolTable = new BinarySearchTree<Symbol>();
            printFunction = printFunction ?? Console.WriteLine;
            errorPrintFunction = errorPrintFunction ?? printFunction;

            foreach (var codeLine in codeLines)
            {
                try
                {
                    var duplicateSymbol = false;
                    var symbol = Symbol.Parse(codeLine);
                    this._SymbolTable.Insert(symbol, (Symbol sym) =>
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
        }

        public bool ContainsSymbol(string label)
        {
            return this[label] != null;
        }

        /// <summary>
        /// Performs the lookup on symbol tree.
        /// </summary>
        /// <param name="symbolLabels">A collection of symbol labels.</param>
        /// <param name="printFunction">The print function.</param>
        /// <param name="errorPrintFunction">The error print function.</param>
        public void PerformLookupOnSymbolTree(string[] symbolLabels, Action<object> printFunction = null, Action<object> errorPrintFunction = null)
        {
            printFunction = printFunction ?? Console.WriteLine;
            errorPrintFunction = errorPrintFunction ?? printFunction;

            if (this._SymbolTable != null)
            {
                foreach (var symbolLabel in symbolLabels)
                {
                    try
                    {
                        var symbol = this._SymbolTable.FindValue(symbolLabel);

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
            else
            {
                errorPrintFunction("Symbol table not created");
            }
        }

        public void Print(TraverseOrder order, Action<object> printFunction)
        {
            if (this._SymbolTable != null && !this._SymbolTable.IsEmpty())
            {
                printFunction(Symbol.GetSymbolHeader());
                this._SymbolTable.Print(order, printFunction);
            }
            else
            {
                printFunction("Symbol table empty");
            }
        }
    }
}