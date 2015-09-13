using SIC.Assembler.Providers.SymbolTable;
using SIC.Assembler.Utilities.BinarySearchTree;
using System;

namespace SIC.Assembler
{
    /// <summary>
    /// 
    /// </summary>
    internal class Program
    {
        private const string ENTER_TO_PROCEED = "Press enter to proceed ...";

        private static void Main(string[] args)
        {
            Console.BufferWidth = 256;
            Console.BufferHeight = short.MaxValue - 1;

            Prompt("Start building symbol tree.", ENTER_TO_PROCEED);
            var symbolTree = SymbolTreeProvider.BuildSymbolTree("symbols.dat", PrintWithTabPrefix, PrintFancyError);

            Prompt("\n\nPerform lookup on the symbol tree.", ENTER_TO_PROCEED);
            SymbolTreeProvider.PerformLookupOnSymbolTree(symbolTree, "test.dat", PrintWithTabPrefix, PrintFancyError);

            Prompt("\n\nPrint tree in order.", ENTER_TO_PROCEED);
            symbolTree.Print(TraverseOrder.InOrder, PrintWithTabPrefix);

            Prompt("\n\n", "Press Enter to terminate...");
        }

        private static void Print(object obj, string prefix = "")
        {
            if (prefix != null)
            {
                var str = prefix + obj.ToString().Replace("\n", "\n" + prefix);
                Console.WriteLine(str);
            }
        }

        private static void PrintFancyError(object obj)
        {
            var previousForground = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            PrintWithTabPrefix(obj);
            Console.ForegroundColor = previousForground;
        }

        private static void PrintWithTabPrefix(object obj)
        {
            Print(obj, "\t");
        }

        private static void Prompt(string message, string proceed)
        {
            Console.WriteLine(message);
            Console.WriteLine(proceed);
            Console.WriteLine();
            Console.ReadLine();
        }
    }
}