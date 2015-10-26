using SIC.Assembler.Model;
using SIC.Assembler.Providers;
using SIC.Assembler.Utilities;
using SIC.Assembler.Utilities.Extensions;
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
            Console.WindowHeight += 10;
            Console.WindowWidth += 10;

            if (args.Length > 0)
            {
                var sourceCode = args[0];
                var symbolTable = new SymbolTable();
                var literalTable = new LiteralTable();
                var opcodes = HelperMethods.GetAllNonEmptyLines("opcodes");
                var codeLines = HelperMethods.GetAllLines(sourceCode);

                opcodes.ForEach(opcode =>
                {
                    var line = opcode.Split(' ');
                    Instruction.RegisterInstruction(line[0], line[1], byte.Parse(line[2]));
                });

                var lines = CodeLine.PerformPass1(codeLines, symbolTable, literalTable);


                PrintWithTabPrefix("\n\n");
                lines.ForEach(line => {
                    if (line != null)
                    {
                        PrintWithTabPrefix(line);
                    }
                });
                PrintWithTabPrefix("\n\n");

                symbolTable.Print(Utilities.Model.TraverseOrder.InOrder, PrintWithTabPrefix);
            }
            else
            {
                PrintWithTabPrefix("Source code not specified");
            }

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