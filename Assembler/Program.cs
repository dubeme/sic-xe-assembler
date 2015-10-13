using SIC.Assembler.Providers;
using SIC.Assembler.Utilities;
using SIC.Assembler.Utilities.Model;
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

            var symbolTable = new SymbolTable();
            var codeLines = HelperMethods.GetAllNonEmptyLines("symbols.dat");
            var symbolLabels = HelperMethods.GetAllNonEmptyLines("test.dat");
            var expressions = HelperMethods.GetAllNonEmptyLines("expr.dat");

            Prompt("Start building Symbol Table.", ENTER_TO_PROCEED);
            symbolTable.BuildSymbolTable(codeLines, PrintWithTabPrefix, PrintFancyError);

            Prompt("\n\nPrint Tree[In Order].", ENTER_TO_PROCEED);
            symbolTable.Print(TraverseOrder.InOrder, PrintWithTabPrefix);

            Prompt("\n\nProcess Expressions.", ENTER_TO_PROCEED);
            var literalTable = OperandEvaluator.ParseExpressions(expressions, symbolTable, PrintWithTabPrefix, PrintFancyError);

            Prompt("\n\nPrint Literal Table.", ENTER_TO_PROCEED);
            PrintWithTabPrefix(literalTable);

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