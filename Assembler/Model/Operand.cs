using SIC.Assembler.Providers;
using System;
using System.Linq;
using System.Text;

namespace SIC.Assembler.Model
{
    public class Operand
    {
        public const string FormatString = "{0, -20}{1, -10}{2, -15}{3, -10}{4, -10}{5, -10}";
        private const string EXPRESSION_OPERATORS = "+-";
        private const string IMMEDIATE_VALUE_TOKEN = "#";
        private const string INDEXED_ADDRESSING_TOKEN = ",";
        private const string INDIRECT_ADDRESSING_TOKEN = "@";
        private const string LITERAL_NUMBER_TOKEN = "=x";
        private const string LITERAL_STRING_TOKEN = "=c";
        private const int PrintMaxLength = 75;

        // NIXBPE
        public string Expression { get; set; }

        public int ByteSize { get; set; }

        public byte[] Bytes { get; private set; }

        public int NumericValue { get; set; }
        public bool Relocatable { get; set; }
        public OperandType Type { get; set; }

        public static string HeaderText()
        {
            StringBuilder str = new StringBuilder();
            str.AppendFormat(FormatString, "Input Expression", "Value", "Relocatable", "Index", "Indirect", "Immediate");
            str.AppendLine();

            for (int i = 0; i < PrintMaxLength; i++)
            {
                str.Append("-");
            }

            return str.ToString();
        }

        public static Operand Parse(string expression, int programCounter, SymbolTable symbolTable, LiteralTable literalTable)
        {
            var _expression = expression.Replace("\\s+", "");
            var operandType = GetOperandType(_expression);

            switch (operandType)
            {
                case OperandType.Simple: return ParseSimple(_expression, symbolTable);
                case OperandType.Immediate: return ParseImmediate(_expression, symbolTable);
                case OperandType.ArithmeticExpression: return ParseArithmeticExpression(_expression, symbolTable, literalTable);
                case OperandType.Indexed: return ParseIndexed(_expression, symbolTable);
                case OperandType.Indirect: return ParseIndirect(_expression, symbolTable);
                case OperandType.LiteralString: return ParseLiteralString(_expression, literalTable);
                case OperandType.LiteralNumber: return ParseLiteralNumber(_expression, literalTable);
            }

            throw new Exception("Invalid operand - " + expression);
        }

        public static Operand ParseArithmeticExpression(string expression, SymbolTable symbolTable, LiteralTable literalTable)
        {
            // Todo: Validate expression syntax
            // Assuming plain Symbol and Numbers [No funny shit like @#]

            var operation = expression.Replace("\\s+", "").Split(EXPRESSION_OPERATORS.ToArray());

            if (operation.Length != 2)
            {
                throw new Exception(string.Format("{0} is invalid. Expecting [operand operator operand]", expression));
            }

            var isRelocatable = false;
            var operand1Relocatable = false;
            var operand2Relocatable = false;
            var operand1 = GetValue(operation[0], symbolTable, out operand1Relocatable);
            var operand2 = GetValue(operation[1], symbolTable, out operand2Relocatable);
            var numericValue = operand1 + operand2 * (expression.Contains("+") ? 1 : -1);

            isRelocatable = operand1Relocatable || operand2Relocatable;

            return new Operand
            {
                NumericValue = numericValue,
                Expression = expression,
                Type = OperandType.ArithmeticExpression,
                Relocatable = isRelocatable
            };
        }

        public static Operand ParseImmediate(string expression, SymbolTable symbolTable)
        {
            var _expression = expression.Replace("\\s+", "").Trim("#".ToCharArray());
            var isRelocatable = false;
            var numericValue = GetValue(_expression, symbolTable, out isRelocatable);

            return new Operand
            {
                NumericValue = numericValue,
                Expression = expression,
                Type = OperandType.Immediate,
                Relocatable = isRelocatable
            };
        }

        public static Operand ParseIndexed(string expression, SymbolTable symbolTable)
        {
            var operation = expression.Replace("\\s+", "").Split(INDEXED_ADDRESSING_TOKEN.ToArray());

            if (operation.Length != 2)
            {
                throw new Exception(string.Format("{0} is invalid. Expecting [operand,operand]", expression));
            }

            var isRelocatable = false;
            var value = GetValue(operation[0], symbolTable,out isRelocatable);

            return new Operand
            {
                NumericValue = value,
                Expression = expression,
                Type = OperandType.Indexed,
                Relocatable = isRelocatable
            };
        }

        public static Operand ParseIndirect(string expression, SymbolTable symbolTable)
        {
            var symbol = GetSymbol(expression.Replace("\\s+", ""), symbolTable, "@");

            return new Operand
            {
                NumericValue = symbol.Value,
                Expression = expression,
                Type = OperandType.Indirect,
                Relocatable = symbol.RFlag
            };
        }

        public static Operand ParseLiteralNumber(string expression, LiteralTable literalTable)
        {
            var literal = literalTable.ParseLiteral(expression.Replace("\\s+", ""));

            return new Operand
            {
                NumericValue = literal.Address,
                Expression = expression,
                Type = OperandType.LiteralNumber,
                Relocatable = false
            };
        }

        public static Operand ParseLiteralString(string expression, LiteralTable literalTable)
        {
            var literal = literalTable.ParseLiteral(expression.Replace("\\s+", ""));

            return new Operand
            {
                NumericValue = literal.Address,
                Expression = expression,
                Type = OperandType.LiteralString,
                Relocatable = false
            };
        }

        public static Operand Parse(string expression)
        {
            throw new NotImplementedException();
        }

        public static Operand ParseSimple(string expression, SymbolTable symbolTable)
        {
            var symbol = GetSymbol(expression.Replace("\\s+", ""), symbolTable);

            return new Operand
            {
                Expression = expression,
                Type = OperandType.Simple,
                Relocatable = symbol.RFlag,
                NumericValue = symbol.Value
            };
        }

        public override string ToString()
        {
            return string.Format(FormatString,
                this.Expression,
                this.NumericValue,
                this.Relocatable,
                this.Type == OperandType.Indexed,
                this.Type == OperandType.Indirect,
                this.Type == OperandType.Immediate);
        }

        private static OperandType GetOperandType(string operandString)
        {
            if (operandString.Contains(INDEXED_ADDRESSING_TOKEN))
            {
                return OperandType.Indexed;
            }
            if (operandString.IndexOfAny(EXPRESSION_OPERATORS.ToCharArray()) >= 0)
            {
                return OperandType.ArithmeticExpression;
            }
            if (operandString.StartsWith(IMMEDIATE_VALUE_TOKEN, StringComparison.CurrentCultureIgnoreCase))
            {
                return OperandType.Immediate;
            }
            if (operandString.StartsWith(INDIRECT_ADDRESSING_TOKEN, StringComparison.CurrentCultureIgnoreCase))
            {
                return OperandType.Indirect;
            }
            if (operandString.StartsWith(LITERAL_NUMBER_TOKEN, StringComparison.CurrentCultureIgnoreCase))
            {
                return OperandType.LiteralNumber;
            }
            if (operandString.StartsWith(LITERAL_STRING_TOKEN, StringComparison.CurrentCultureIgnoreCase))
            {
                return OperandType.LiteralString;
            }

            return OperandType.Simple;
        }

        private static Symbol GetSymbol(string symbolExpression, SymbolTable symbolTable, string trimChars = "")
        {
            var symbol = symbolTable[symbolExpression.Trim(trimChars.ToCharArray())];
            if (symbol != null)
            {
                return symbol;
            }

            throw new Exception("Symbol not found - " + symbolExpression);
        }

        private static int GetValue(string symbolLabel, SymbolTable symbolTable, out bool isRelocatable)
        {
            const string TRIM_CHARS = " @#";
            var trimmedLabel = symbolLabel.Trim(TRIM_CHARS.ToCharArray());
            var operation = trimmedLabel.Split(EXPRESSION_OPERATORS.ToCharArray());
            var operand1 = operation.Length > 0 ? operation[0] : string.Empty;
            var operand2 = operation.Length > 1 ? operation[1] : string.Empty;
            var isExpression = !string.IsNullOrWhiteSpace(operand2);
            var num1 = int.MinValue;
            var num2 = int.MinValue;

            isRelocatable = false;

            try
            {
                // Try getting symbol
                var symbol1 = GetSymbol(operand1, symbolTable);
                num1 = symbol1.Value;
                isRelocatable = symbol1.RFlag;
            }
            catch (Exception)
            {
                // Try parsing
                if (!int.TryParse(operand1, out num1))
                {
                    throw;
                }
            }

            if (isExpression)
            {
                try
                {
                    // Try getting symbol
                    var symbol2 = GetSymbol(operand2, symbolTable);
                    num2 = symbol2.Value;
                    isRelocatable = isRelocatable || symbol2.RFlag;
                }
                catch (Exception)
                {
                    // Try parsing
                    if (!int.TryParse(operand2, out num2))
                    {
                        throw;
                    }
                }

                return num1 + num2 * (symbolLabel.Contains("+") ? 1 : -1);
            }

            return num1;
        }
    }
}