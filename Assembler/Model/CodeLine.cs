using SIC.Assembler.Providers;
using System;
using System.Text.RegularExpressions;

namespace SIC.Assembler.Model
{
    public class CodeLine
    {
        private const int WORD_SIZE = 3;
        public int Address { get; private set; }
        public int ByteSize { get; private set; }
        public Instruction Instruction { get; set; }
        public Symbol Label { get; set; }
        public int LineNumber { get; private set; }
        public string ObjectCode { get; set; }
        public Operand Operand { get; set; }
        
        public CodeLine Create(string codeline, SymbolTable symbolTable, LiteralTable literalTable, int lineNumber, int currentPC)
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
            var line = CreateCodeLine(Regex.Split(codeline, "\\s+"), symbolTable, literalTable, lineNumber, currentPC, out newPc);

            if (line == null || !ProgramCounterValid(newPc))
            {
                return null;
            }

            line.ByteSize = newPc - currentPC;
            return line;
        }

        private static int AdvanceProgramCounter(int programCounter, Instruction instruction, Operand operand)
        {
            var displacement = 0;

            if (instruction == null)
            {
                return int.MinValue;
            }

            switch (instruction.DirectiveType)
            {
                case AssemblerDirectiveType.Start:
                    return operand.NumericValue;

                case AssemblerDirectiveType.End:
                    displacement = 0;
                    break;

                case AssemblerDirectiveType.Byte:
                    displacement = operand.ByteSize;
                    break;

                case AssemblerDirectiveType.Word:
                    displacement = Math.Max(WORD_SIZE, (int)instruction.Format);
                    break;

                case AssemblerDirectiveType.Resb:
                case AssemblerDirectiveType.Resw:
                    displacement = operand.ByteSize;
                    break;

                default:
                    displacement = Math.Max(operand.ByteSize, (int)instruction.Format);
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

            newPC = AdvanceProgramCounter(currentPC, instruction, operand);
            return new CodeLine
            {
                Address = currentPC,
                Instruction = instruction,
                Label = symbol,
                Operand = operand,
                LineNumber = lineNumber,
            };
        }

        private bool ProgramCounterValid(int newPc)
        {
            return newPc != int.MinValue;
        }
    }
}