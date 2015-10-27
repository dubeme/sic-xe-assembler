using SIC.Assembler.Providers;
using SIC.Assembler.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace SIC.Assembler.Model
{
    public class CodeLine
    {
        private const string CODELINE_FORMAT_STRING = "{0, -10}{1, -15:X5}{2, -21}{3, -15}{4}";
        private const int WORD_SIZE = 3;
        private static string TOP_BAR = new string('-', 70);

        public int Address { get; private set; }
        public int ByteSize { get; private set; }
        public string Expression { get; private set; }
        public Instruction Instruction { get; set; }
        public Symbol Label { get; set; }
        public int LineNumber { get; private set; }
        public string ObjectCode { get; set; }
        public Operand Operand { get; set; }
        public int ProgramCounter { get; private set; }

        public static CodeLine Create(string codeline, SymbolTable symbolTable, LiteralTable literalTable, int lineNumber, int currentPC)
        {
            if (string.IsNullOrWhiteSpace(codeline))
            {
                return null;
            }

            if (codeline.StartsWith("."))
            {
                return null;
            }
            int newPc;
            var line = CreateCodeLine(Regex.Split(codeline.TrimStart(), "\\s+"), symbolTable, literalTable, lineNumber, currentPC, out newPc);

            if (line == null || !ProgramCounterValid(newPc))
            {
                return null;
            }

            line.ByteSize = newPc - currentPC;
            line.Expression = codeline.TrimStart();
            return line;
        }

        public static void PerformPass1(string[] codeLines, Action<object> printFunction, Action<object> errorPrintFunction, Action<string, string> promptFunction)
        {
            if (codeLines == null || codeLines.Length < 1)
            {
                printFunction("No code to parse");
                return;
            }

            var symbolTable = new SymbolTable();
            var literalTable = new LiteralTable();
            var result = new List<CodeLine>();
            var lineNumber = 1;
            var programCounter = 0;
            var ARTIFICIAL_DELAY_MILLISECONDS = Math.Min(64, 2048/codeLines.Length);

            foreach (var lineStr in codeLines)
            {
                try
                {
                    Console.Write(string.Format("\rParsing line {0} of {1}", lineNumber, codeLines.Length));
                    var line = Create(lineStr, symbolTable, literalTable, lineNumber, programCounter);

                    if (line != null)
                    {
                        programCounter += line.ByteSize;
                    }

                    result.Add(line);
                    lineNumber++;
                }
                catch (Exception ex)
                {
                    errorPrintFunction(string.Format("\n\rError parsing line {0} of {1}", lineNumber, codeLines.Length));
                    errorPrintFunction(string.Format("{0}", ex.Message));
                    return;
                }

                Thread.Sleep(ARTIFICIAL_DELAY_MILLISECONDS);
            }

            promptFunction("\n\nDone parsing file.", "Press enter to proceed with output...");
            PrintCodelines(result, printFunction, promptFunction);
            
            promptFunction("", "Press enter to proceed with dumping symbol table...");
            symbolTable.Print(Utilities.Model.TraverseOrder.InOrder, printFunction);
            
            promptFunction("", "Press enter to proceed with dumping literal table...");
            printFunction(literalTable);
        }

        public static void PrintCodelines(IEnumerable<CodeLine> lines, Action<object> printFunction, Action<string, string> promptFunction)
        {
            if (lines == null || !lines.Any())
            {
                return;
            }

            printFunction = printFunction ?? Console.WriteLine;

            printFunction(string.Format(CODELINE_FORMAT_STRING,
                "Line #",
                "Prog Count",
                "Label",
                "Instruction",
                "Operand"));
            printFunction(TOP_BAR);

            lines.ForEach(line =>
            {
                if (line != null)
                {
                    printFunction(line);
                }
            });
        }

        public override string ToString()
        {
            return string.Format(CODELINE_FORMAT_STRING,
                this.LineNumber,
                this.ProgramCounter,
                this.Label == null ? "" : this.Label.LongLabel,
                this.Instruction == null ? "" : this.Instruction.Mnemonic,
                this.Operand == null ? "" : this.Operand.Expression);
        }

        private static int AdvanceProgramCounter(int programCounter, Symbol symbol, Instruction instruction, Operand operand, SymbolTable symbolTable, LiteralTable literalTable)
        {
            var defaultDisplacement = 3;
            var displacement = 0;

            if (instruction == null)
            {
                return int.MinValue;
            }

            switch (instruction.DirectiveType)
            {
                case AssemblerDirectiveType.Start:
                    return operand.Evaluate(symbolTable, literalTable, programCounter).NumericValue;

                case AssemblerDirectiveType.End:
                    displacement = 0;
                    break;

                case AssemblerDirectiveType.Base:
                    displacement = 0;
                    break;

                case AssemblerDirectiveType.Equ:
                    symbol.Value = operand.Evaluate(symbolTable, literalTable, programCounter).NumericValue;
                    displacement = defaultDisplacement;
                    break;

                case AssemblerDirectiveType.Extdef:
                case AssemblerDirectiveType.Extref:
                    displacement = defaultDisplacement;
                    break;

                case AssemblerDirectiveType.Byte:
                    displacement = operand.Evaluate(symbolTable, literalTable, programCounter).ByteSize;
                    break;

                case AssemblerDirectiveType.Word:
                    displacement = Math.Max(WORD_SIZE, (int)instruction.Format);
                    break;

                case AssemblerDirectiveType.Resb:
                    displacement = operand.Evaluate(symbolTable, literalTable, programCounter).NumericValue;
                    break;

                case AssemblerDirectiveType.Resw:
                    displacement = operand.Evaluate(symbolTable, literalTable, programCounter).NumericValue * WORD_SIZE;
                    break;

                default:
                    displacement = (int)instruction.Format;
                    break;
            }

            return programCounter + displacement;
        }

        private static CodeLine CreateCodeLine(string[] columns, SymbolTable symbolTable, LiteralTable literalTable, int lineNumber, int currentPC, out int newPC)
        {
            Symbol symbol = null;
            Operand operand = null;
            Instruction instruction = Instruction.All[columns[0]];

            // Todo: Check to see if labels can be reserved words
            if (instruction == null)
            {
                instruction = Instruction.All[columns[1]];

                if (instruction == null)
                {
                    throw new Exception(string.Format("Can't determine instruction on line #{0}.", lineNumber));
                }

                symbol = symbolTable.AddSymbol(columns[0], currentPC, true, false);

                if (columns.Length > 2)
                {
                    //operand = Operand.CreateOperand(columns[2], currentPC, symbolTable, literalTable);
                    operand = Operand.CreateOperand(columns[2]);
                }
            }
            else if (columns.Length > 1)
            {
                //operand = Operand.CreateOperand(columns[1], currentPC, symbolTable, literalTable);
                operand = Operand.CreateOperand(columns[1]);
            }
            else
            {
                throw new Exception(string.Format("Can't determine instruction on line #{0}.", lineNumber));
            }

            newPC = AdvanceProgramCounter(currentPC, symbol, instruction, operand, symbolTable, literalTable);
            return new CodeLine
            {
                Address = currentPC,
                Instruction = instruction,
                Label = symbol,
                Operand = operand,
                LineNumber = lineNumber,
                ProgramCounter = currentPC
            };
        }

        private static bool ProgramCounterValid(int newPc)
        {
            return newPc != int.MinValue;
        }
    }
}