using SIC.Assembler.Model;
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

        public static LiteralTable ParseExpressions(IList<string> expressions, SymbolTable symbolTable, Action<object> printFunction)
        {
            if (expressions == null || symbolTable == null)
            {
                return null;
            }

            var literalTable = new LiteralTable();

            printFunction(Operand.HeaderText());
            foreach (var operandString in expressions.Where(str => !string.IsNullOrWhiteSpace(str)))
            {
                printFunction(CreateOperand(operandString.ToLower(), symbolTable, literalTable ));
            }

            return literalTable;
        }

        private static Operand CreateOperand(string expression, SymbolTable symbolTable, LiteralTable literalTable)
        {
            var _expression = expression.Replace("\\s+", "");
            var numericValue = -1;
            var operandType = GetOperandType(_expression);
            var isRelocatable = false;

            switch (operandType)
            {
                case OperandType.Simple:
                    numericValue = GetValue(_expression, symbolTable, out isRelocatable);
                    break;

                case OperandType.Immediate:
                    // Todo: Why not relocatable, If label can shift.
                    numericValue = GetValue(_expression, symbolTable, out isRelocatable);
                    break;

                case OperandType.ArithmeticExpression:

                    var operation = _expression.Split(EXPRESSION_OPERATORS.ToArray());

                    if (operation.Length != 2)
                    {
                        throw new Exception(string.Format("{0} is invalid. Expecting [operand operator operand]", _expression));
                    }

                    var operand1Relocatable = false;
                    var operand2Relocatable = false;
                    var operand1 = GetValue(operation[0], symbolTable, out operand1Relocatable);
                    var operand2 = GetValue(operation[1], symbolTable, out operand2Relocatable);

                    numericValue = operand1 + operand2 * (_expression.Contains("+") ? 1 : -1);
                    isRelocatable = operand1Relocatable || operand2Relocatable;

                    break;

                case OperandType.Indexed:
                    operation = _expression.Split(INDEXED_ADDRESSING_TOKEN.ToArray());

                    if (operation.Length != 2)
                    {
                        throw new Exception(string.Format("{0} is invalid. Expecting [operand, operand]", _expression));
                    }
                    numericValue = GetValue(operation[0], symbolTable, out isRelocatable);
                    break;

                case OperandType.Indirect:
                    numericValue = GetValue(_expression, symbolTable, out isRelocatable);
                    break;

                case OperandType.LiteralString:
                    numericValue = literalTable.AddLiteral(_expression).Address;
                    isRelocatable = true;
                    break;

                case OperandType.LiteralNumber:
                    numericValue = literalTable.AddLiteral(_expression).Address;
                    isRelocatable = true;
                    break;
            }

            return new Operand
            {
                NumericValue = numericValue,
                Expression = expression,
                Type = operandType,
                Relocatable = isRelocatable
            };
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

        private static int GetValue(string label, SymbolTable symbolTable, out bool isRelocatable)
        {
            const string TRIM_CHARS = " @#";
            var trimmedLabel = label.Trim(TRIM_CHARS.ToCharArray());
            var symbol = symbolTable.FindSymbol(trimmedLabel);
            int num;

            isRelocatable = false;

            if (int.TryParse(trimmedLabel, out num))
            {
                return num;
            }
            if (symbol != null)
            {
                isRelocatable = symbol.RFlag;
                return symbol.Value;
            }

            throw SymbolNotFound(trimmedLabel);
        }

        private static Exception SymbolNotFound(string symbolName)
        {
            return new Exception(string.Format("Symbol {0} doesn't exist.", symbolName));
        }
    }
}