using SIC.Assembler.Providers;
using System;
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
                
                symbol = symbolTable.AddSymbol(columns[0], currentPC, false);

                if (columns.Length > 2)
                {
                    operand = Operand.CreateOperand(columns[2], currentPC, symbolTable, literalTable);
                }
            }
            else if (columns.Length > 1)
            {
                operand = Operand.CreateOperand(columns[1], currentPC, symbolTable, literalTable);
            }
            else
            {
                throw new Exception(string.Format("Can't determine instruction on line #{0}.", lineNumber));
            }

            newPC = CalculateProgramCounter(symbol, instruction, operand);
            return new CodeLine
            {
                Address = currentPC,
                Instruction = instruction,
                Label = symbol,
                Operand = operand,
                LineNumber = lineNumber,
            };
        }
    }
}