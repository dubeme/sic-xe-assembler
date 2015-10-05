using SIC.Assembler.Model;
using SIC.Assembler.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SIC.Assembler.Providers
{
    public class OperandEvaluator
    {
        private static string EXPRESSION_OPERATORS = "+-";
        private static string IMMEDIATE_VALUE_TOKEN = "#";
        private static string INDEXED_ADDRESSING_TOKEN = ",";
        private static string INDIRECT_ADDRESSING_TOKEN = "@";
        private static string LITERAL_NUMBER_TOKEN = "=x";
        private static string LITERAL_STRING_TOKEN = "=c";

        public static void ParseOperands(IList<string> operandStrings, SymbolTable symbolTable)
        {
            if (operandStrings == null || symbolTable == null)
            {
                return;
            }

            var operands = new List<Operand>();

            foreach (var operandString in operandStrings.Where(str => !string.IsNullOrWhiteSpace(str)))
            {
                Console.WriteLine(GetOperand(symbolTable, operandString.ToLower()));
            }
        }

        private static Operand CreateOperand(string expression, int numericValue, OperandType type = OperandType.Unknown, bool isRelocatable = false)
        {
            return new Operand
            {
                NumericValue = numericValue,
                Expression = expression,
                Type = type,
                Relocatable = isRelocatable
            };
        }

        private static Operand GetOperand(SymbolTable symbolTable, string operandString)
        {

            switch (GetOperandType(operandString))
            {
                case OperandType.Simple:
                    return ParseSimpleAddressing(operandString, symbolTable);

                case OperandType.Immediate:
                    return ParseImmediateValue(operandString, symbolTable);

                case OperandType.ArithmeticExpression:
                    return ParseArithmeticExpression(operandString, symbolTable);

                case OperandType.Indexed:
                    return ParseIndexedAddressing(operandString, symbolTable);

                case OperandType.Indirect:
                    return ParseIndirectAddressing(operandString, symbolTable);

                case OperandType.LiteralString:
                    return ParseLiteralString(operandString, symbolTable);

                case OperandType.LiteralNumber:
                    return ParseLiteralNumber(operandString, symbolTable);
            }

            return null;
        }

        private static OperandType GetOperandType(string operandString)
        {
            if (operandString.IndexOfAny(EXPRESSION_OPERATORS.ToCharArray()) >= 0)
            {
                return OperandType.ArithmeticExpression;
            }
            if (operandString.StartsWith(IMMEDIATE_VALUE_TOKEN))
            {
                return OperandType.Immediate;
            }
            if (operandString.Contains(INDEXED_ADDRESSING_TOKEN))
            {
                return OperandType.Indexed;
            }
            if (operandString.StartsWith(INDIRECT_ADDRESSING_TOKEN))
            {
                return OperandType.Indirect;
            }
            if (operandString.StartsWith(LITERAL_NUMBER_TOKEN))
            {
                return OperandType.LiteralNumber;
            }
            if (operandString.StartsWith(LITERAL_STRING_TOKEN))
            {
                return OperandType.LiteralString;
            }

            return OperandType.Simple;
        }

        private static int GetValue(string label, SymbolTable symbolTable)
        {
            const string TRIM_CHARS = " @#";
            var trimmedLabel = label.Trim(TRIM_CHARS.ToCharArray());
            var symbol = symbolTable.FindSymbol(trimmedLabel);
            int num;

            if (int.TryParse(trimmedLabel, out num))
            {
                return num;
            }
            if (symbol != null)
            {
                return symbol.Value;
            }

            throw SymbolNotFound(trimmedLabel);
        }

        private static Operand ParseArithmeticExpression(string expression, SymbolTable symbolTable)
        {
            var operation = expression.Replace("\\s+", "").Split(EXPRESSION_OPERATORS.ToArray());

            if (operation.Length != 2)
            {
                throw new Exception(string.Format("{0} is invalid. Expecting [operand operator operand]", expression));
            }

            var operand1 = GetValue(operation[0], symbolTable);
            var operand2 = GetValue(operation[1], symbolTable);

            if (expression.Contains("+"))
            {
                return CreateOperand(expression, operand1 + operand2, OperandType.ArithmeticExpression, true);
            }
            else
            {
                return CreateOperand(expression, operand1 - operand2, OperandType.ArithmeticExpression, true);
            }
        }

        private static Operand ParseImmediateValue(string expression, SymbolTable symbolTable)
        {
            // Todo: Why not relocatable, If label can shift.
            return CreateOperand(expression, GetValue(expression, symbolTable), OperandType.Immediate, false);
        }

        private static Operand ParseIndexedAddressing(string expression, SymbolTable symbolTable)
        {
            var operation = expression.Replace("\\s+", "").Split(INDEXED_ADDRESSING_TOKEN.ToArray());

            if (operation.Length != 2)
            {
                throw new Exception(string.Format("{0} is invalid. Expecting [operand, operand]", expression));
            }

            var operand1 = GetValue(operation[0], symbolTable);

            return CreateOperand(expression, operand1, OperandType.Indexed, true);
        }

        private static Operand ParseIndirectAddressing(string expression, SymbolTable symbolTable)
        {
            return CreateOperand(expression, GetValue(expression, symbolTable), OperandType.Indirect, true);
        }

        private static Operand ParseLiteralNumber(string operandString, SymbolTable symbolTable)
        {
            var address = Literal.Parse(operandString).Address;
            return CreateOperand(operandString, address, OperandType.LiteralNumber, true);
        }

        private static Operand ParseLiteralString(string operandString, SymbolTable symbolTable)
        {
            var address = Literal.Parse(operandString).Address;
            return CreateOperand(operandString, address, OperandType.LiteralString, true);
        }

        private static Operand ParseSimpleAddressing(string expression, SymbolTable symbolTable)
        {
            return CreateOperand(expression, GetValue(expression, symbolTable), OperandType.Simple, true);
        }

        private static Exception SymbolNotFound(string symbolName)
        {
            return new Exception(string.Format("Symbol {0} doesn't exist.", symbolName));
        }
    }
}