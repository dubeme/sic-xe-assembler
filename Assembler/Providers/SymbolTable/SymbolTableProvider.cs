using SIC.Assembler.Utilities.BinarySearchTree;
using System.IO;
using System;

namespace SIC.Assembler.Providers.SymbolTable
{
    public class SymbolTableProvider
    {
        public static Tree<Symbol> BuildSymbolTable(string filePath)
        {
            var symbolTree = new Tree<Symbol>();

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Must provide a non-null OR empty file path");
            }

            using (StreamReader symbolFile = File.OpenText(filePath))
            {
                string line;
                while ((line = symbolFile.ReadLine()) != null)
                {
                    try
                    {
                        var symbol = Symbol.Parse(line);
                        symbolTree.Insert(symbol, (Symbol sym) => { sym.HasMultiple = true; });
                    }
                    catch (System.Exception ex)
                    {
                        System.Console.WriteLine(ex.Message);
                    }
                }
            }
            return symbolTree;
        }
    }
}