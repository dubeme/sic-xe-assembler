using SIC.Assembler.Model;
using SIC.Assembler.Utilities;
using SIC.Assembler.Utilities.Extensions;
using System;
using System.IO;

namespace SIC.Assembler
{
    /// <summary>
    ///
    /// </summary>
    internal class Program
    {
        private const string ENTER_TO_PROCEED = "Press enter to proceed ...";
        private static TextWriter FILE_OUT = null;
        private static TextWriter STD_OUT = Console.Out;

        private static void Main(string[] args)
        {
            Console.BufferWidth = 256;
            Console.BufferHeight = short.MaxValue - 1;
            Console.WindowHeight += 10;
            Console.WindowWidth += 32;

            if (args.Length > 0)
            {
                HelperMethods.GetAllNonEmptyLines("opcodes").ForEach(opcode =>
                {
                    var line = opcode.Split(' ');
                    Instruction.RegisterInstruction(line[0], line[1], byte.Parse(line[2]));
                });

                var filePath = args[0];
                var lines = HelperMethods.GetAllLines(filePath);

                var ostrm = new FileStream("out.osicxe", FileMode.OpenOrCreate, FileAccess.Write);
                FILE_OUT = new StreamWriter(ostrm);

                SwapPrint(() =>
                {
                    Prompt(string.Format("Loaded file - {0}", filePath), "Press enter to proceed with parsing...");
                });

                CodeLine.PerformPass1(lines, (str) =>
                {
                    SwapPrint(() =>
                    {
                        PrintWithTabPrefix(str);
                    });
                }, (str) =>
                {
                    SwapPrint(() =>
                    {
                        PrintFancyError(str);
                    });
                }, (msg, proceed) =>
                {
                    SwapPrint(() =>
                    {
                        Prompt(msg, proceed);
                    });
                });
            }
            else
            {
                PrintWithTabPrefix("Source code not specified");
            }

            SwapPrint(() =>
            {
                Prompt("\n\n", "Press Enter to terminate...");
            });
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
            var previousForground = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.WriteLine(proceed);
            Console.ForegroundColor = previousForground;

            if (Console.Out == STD_OUT)
            {
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine();
            }
        }

        private static void SwapPrint(Action printer)
        {
            Console.SetOut(FILE_OUT);
            printer();
            Console.Out.Flush();

            Console.SetOut(STD_OUT);
            printer();
            Console.Out.Flush();
        }
    }
}