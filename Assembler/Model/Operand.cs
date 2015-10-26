using SIC.Assembler.Providers;
using SIC.Assembler.Utilities.Extensions;
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
        private const string INDEXED_ADDRESSING_TOKEN = ",x";
        private const string INDIRECT_ADDRESSING_TOKEN = "@";
        private const int PrintMaxLength = 75;

        // BASE = 0 - 4095
        // PC = -2048 - +2047
        public byte[] Bytes { get; private set; }

        public int ByteSize
        {
            get
            {
                return this.Bytes.Length;
            }
        }

        // NIXBPE
        public string Expression { get; private set; }

        public string HexString
        {
            get
            {
                StringBuilder str = new StringBuilder();

                if (this.Bytes != null)
                {
                    foreach (var value in this.Bytes)
                    {
                        str.Append(value.ToString("X2"));
                    }
                }

                return str.ToString();
            }
        }

        public int NumericValue { get; set; }

        public bool Relocatable { get; set; }

        public OperandType Type { get; set; }

        public static Operand CreateOperand(string expression)
        {
            return new Operand
            {
                Expression = expression
            };
        }

        public static Operand CreateOperand(string expression, int programCounter, SymbolTable symbolTable, LiteralTable literalTable)
        {
            return CreateOperand(expression, GetOperandType(expression), programCounter, symbolTable, literalTable);
        }

        public static Operand CreateOperand(string expression, OperandType type, int programCounter, SymbolTable symbolTable, LiteralTable literalTable)
        {
            // Todo: Can BYTE/WORD/RESB/RESW have Literal/Symbol as operand
            var _expression = expression.Replace("\\s+", "");

            switch (type)
            {
                case OperandType.Simple: return ParseSimple(_expression, symbolTable);
                case OperandType.Immediate: return ParseImmediate(_expression, symbolTable);
                case OperandType.ArithmeticExpression: return ParseArithmeticExpression(_expression, symbolTable, literalTable);
                case OperandType.Indexed: return ParseIndexed(_expression, symbolTable);
                case OperandType.Indirect: return ParseIndirect(_expression, symbolTable);
                case OperandType.LiteralString: return ParseLiteral(_expression, literalTable, OperandType.LiteralString);
                case OperandType.LiteralNumber: return ParseLiteral(_expression, literalTable, OperandType.LiteralNumber);
                case OperandType.JustNumber: return ParseLiteral(_expression, literalTable, OperandType.JustNumber, false);
                case OperandType.ConstantNumber: return ParseLiteral(_expression, literalTable, OperandType.ConstantNumber, false);
                case OperandType.ConstantString: return ParseLiteral(_expression, literalTable, OperandType.ConstantString, false);
            }

            throw new Exception("Invalid operand - " + expression);
        }

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

        public Operand ParseAs(OperandType type, int programCounter = int.MinValue, SymbolTable symbolTable = null, LiteralTable literalTable = null)
        {
            return CreateOperand(this.Expression, type, programCounter, symbolTable, literalTable);
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
            if (operandString.EndsWith(INDEXED_ADDRESSING_TOKEN, StringComparison.CurrentCultureIgnoreCase))
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

            switch (Literal.GetLiteralType(operandString))
            {
                case LiteralType.StringLiteral:
                    return OperandType.LiteralString;

                case LiteralType.NumberLiteral:
                    return OperandType.LiteralNumber;

                case LiteralType.JustNumber:
                    return OperandType.JustNumber;

                case LiteralType.ConstantNumber:
                    return OperandType.ConstantNumber;

                case LiteralType.ConstantString:
                    return OperandType.ConstantString;
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
                isRelocatable = symbol1.RelocatableFlag;
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
                    isRelocatable = isRelocatable || symbol2.RelocatableFlag;
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

        private static Operand ParseArithmeticExpression(string expression, SymbolTable symbolTable, LiteralTable literalTable)
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

        private static Operand ParseImmediate(string expression, SymbolTable symbolTable)
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

        private static Operand ParseIndexed(string expression, SymbolTable symbolTable)
        {
            var operation = expression.Replace("\\s+", "");

            if (string.IsNullOrWhiteSpace(operation))
            {
                throw new Exception(string.Format("{0} is invalid. Expecting [operand,operand]", expression));
            }

            operation = operation.Substring(0, operation.Length - INDEXED_ADDRESSING_TOKEN.Length);
            var isRelocatable = false;
            var value = GetValue(operation, symbolTable, out isRelocatable);

            return new Operand
            {
                NumericValue = value,
                Expression = expression,
                Type = OperandType.Indexed,
                Relocatable = isRelocatable
            };
        }

        private static Operand ParseIndirect(string expression, SymbolTable symbolTable)
        {
            var symbol = GetSymbol(expression.Replace("\\s+", ""), symbolTable, "@");

            return new Operand
            {
                NumericValue = symbol.Value,
                Expression = expression,
                Type = OperandType.Indirect,
                Relocatable = symbol.RelocatableFlag
            };
        }

        private static Operand ParseLiteral(string expression, LiteralTable literalTable, OperandType operandType, bool addToLiteralTable = true)
        {
            var literal = literalTable.ParseLiteral(expression.Replace("\\s+", ""), addToLiteralTable);
            var useNumeric = literal.Type == LiteralType.JustNumber || literal.Type == LiteralType.ConstantNumber;

            var operand = new Operand
            {
                NumericValue = useNumeric ? literal.NumericValue : literal.Address,
                Expression = expression,
                Type = operandType,
                Relocatable = false,
                Bytes = literal.Bytes
            };

            return literal.Type == LiteralType.Unknown ? null : operand;
        }

        private static Operand ParseSimple(string expression, SymbolTable symbolTable)
        {
            var symbols = expression.Replace(@"\s+", "").Split(',');
            symbols.ForEach(sym =>
            {
                var value = GetSymbol(sym, symbolTable);
            });
            var symbol = GetSymbol(expression.Replace(@"\s+", ""), symbolTable);

            return new Operand
            {
                Expression = expression,
                Type = OperandType.Simple,
                Relocatable = symbol.RelocatableFlag,
                NumericValue = symbol.Value
            };
        }

        private static void ParseThatShit(string str)
        {
        }
    }
}