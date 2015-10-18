using SIC.Assembler.Providers;
using System;
using System.Linq;
using System.Text;

namespace SIC.Assembler.Model
{
    public class CodeLine
    {
        private const int WORD_SIZE = 3;
        public int Address { get; private set; }
        public string Comment { get; set; }
        public Instruction Instruction { get; set; }
        public Symbol Label { get; set; }
        public int LineNumber { get; private set; }
        public string ObjectCode { get; set; }
        public Operand Operand { get; set; }

        public static int ReserveMemory(Symbol symbol, Instruction instruction, Operand operand)
        {
            var unitSize = 0;

            switch (instruction.DirectiveType)
            {
                case AssemblerDirectiveType.Resb:
                    unitSize = 1;
                    break;

                case AssemblerDirectiveType.Resw:
                    unitSize = 3;
                    break;
            }

            return unitSize * operand.NumericValue;
        }

        public CodeLine Parse(string codeline)
        {
            if (string.IsNullOrWhiteSpace(codeline))
            {
                return null;
            }

            StringBuilder str = new StringBuilder();
            var foundOpeningLiteralQuote = false;
            var readNoMore = false;

            foreach (var ch in codeline.TrimStart())
            {
                if (ch != ' ' || foundOpeningLiteralQuote)
                {
                    str.Append(ch);

                    if (ch == '\'')
                    {
                        if (foundOpeningLiteralQuote)
                        {
                            readNoMore = true;
                        }
                    }
                    foundOpeningLiteralQuote = ch == '\'';
                }
                else
                {
                }
            }
            /*

            switch (instruction.DirectiveType)
            {
                case AssemblerDirectiveType.Start:
                    break;

                case AssemblerDirectiveType.End:
                    break;

                case AssemblerDirectiveType.Byte:
                    break;

                case AssemblerDirectiveType.Word:
                    break;

                case AssemblerDirectiveType.Resb:
                    break;

                case AssemblerDirectiveType.Resw:
                    break;

                case AssemblerDirectiveType.Base:
                    break;

                case AssemblerDirectiveType.Equ:
                    break;

                case AssemblerDirectiveType.Extdef:
                    break;

                case AssemblerDirectiveType.Extref:
                    break;

                case AssemblerDirectiveType.Unknown:
                    break;

                default:
                    break;
            }

            */
            // Trim line
            return new CodeLine(); ;
        }

        private static int AssignLiteralTableAddresses(LiteralTable literalTable, int programCounter)
        {
            return 0;
        }

        private static int CalculateProgramCounter(Symbol symbol, Instruction instruction, Operand operand)
        {
            switch (instruction.DirectiveType)
            {
                case AssemblerDirectiveType.Start: return symbol.Value;
                case AssemblerDirectiveType.End: return symbol.Value;
                case AssemblerDirectiveType.Byte: return symbol.Value + operand.Bytes.Length;
                case AssemblerDirectiveType.Word: return symbol.Value + WORD_SIZE;
                case AssemblerDirectiveType.Resb: return symbol.Value + operand.NumericValue;
                case AssemblerDirectiveType.Resw: return symbol.Value + operand.NumericValue * WORD_SIZE;
                case AssemblerDirectiveType.Base:
                    break;

                case AssemblerDirectiveType.Equ:
                    break;

                case AssemblerDirectiveType.Extdef:
                    break;

                case AssemblerDirectiveType.Extref:
                    break;
            }

            return int.MinValue;
        }

        private static CodeLine Parse1Token(string[] tokens, SymbolTable symbolTable, LiteralTable literalTable, int programCounter)
        {
            return null;
        }

        private static CodeLine Parse2Tokens(string[] tokens, SymbolTable symbolTable, LiteralTable literalTable, int programCounter)
        {
            return null;
        }

        private static CodeLine Parse3Tokens(string[] tokens, SymbolTable symbolTable, LiteralTable literalTable, int programCounter, int lineNumber)
        {
            var label = tokens[0];

            if (symbolTable.ContainsSymbol(label))
            {
                throw new Exception(string.Format("Duplicate symbol \"{0}\" on line \"{1}\".", label, lineNumber));
            }

            var instruction = Instruction.Parse(tokens[1]);
            var operand = Operand.Parse(tokens[2], programCounter, symbolTable, literalTable);

            if (instruction != null)
            {
                programCounter += instruction.ByteWeight;
            }

            return null;
        }

        private static CodeLine ParseLine(string[] line, int lineNumber, SymbolTable symbolTable, LiteralTable literalTable, int currentPC, out int newPC)
        {
            Symbol symbol = null;
            Instruction instruction = Instruction.All[line[0]]; ;
            Operand operand = null;

            if (instruction == null)
            {
                instruction = Instruction.All[line[1]];
                symbol = Symbol.Parse(line[0], currentPC, false);

                if (line.Length > 2)
                {
                    operand = Operand.Parse(line[2]);
                }
                newPC = CalculateProgramCounter(symbol, instruction, operand);
            }
            else if (line.Length > 1)
            {
                operand = Operand.Parse(line[1]);
                newPC = currentPC + (int)instruction.Format;
            }
            else
            {
                throw new Exception(string.Format("Can't determine instruction on line #{0}.", lineNumber));
            }

            return new CodeLine
            {
                Address = currentPC,
                Instruction = instruction,
                Label = symbol,
                Operand = operand,
                LineNumber = lineNumber,
            };
        }

        private static Symbol ParseSymbol(string label, Instruction instruction, SymbolTable symbolTable, int programCounter)
        {
            if (instruction != null)
            {
                return symbolTable.AddSymbol(
                    label: label,
                    value: programCounter,
                    isRelocatable: true,
                    allowDuplicates: false);
            }

            throw new Exception(string.Format("Invalid instruction"));
        }
    }
}