using SIC.Assembler.Providers;
using SIC.Assembler.Utilities;
using SIC.Assembler.Utilities.Model;
using System;
using System.Collections.Generic;

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

            var symbolTable = new SymbolTable();
            var codeLines = HelperMethods.GetAllNonEmptyLines("symbols.dat");
            var symbolLabels = HelperMethods.GetAllNonEmptyLines("test.dat");

            Prompt("Start building symbol tree.", ENTER_TO_PROCEED);
            symbolTable.BuildSymbolTable(codeLines, PrintWithTabPrefix, PrintFancyError);

            Prompt("\n\nPerform lookup on the symbol tree.", ENTER_TO_PROCEED);
            symbolTable.PerformLookupOnSymbolTree( symbolLabels, PrintWithTabPrefix, PrintFancyError);

            IList<string> input = new List<string>
            {
                "one",
                "two+74",
                "@three",
                "#five",
                "six, x",
                "#9",
                "=x'03'",
                "=c'ABC'",
                "FOUR + 4",
                "=X'03'",
                "@one",
                "8",
                "=x'03'",
                "=c'03'",
                "",
            };
            OperandEvaluator.ParseOperands(input, symbolTable);


            Prompt("\n\nPrint tree in order.", ENTER_TO_PROCEED);
            symbolTable.Print(TraverseOrder.InOrder, PrintWithTabPrefix);

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