using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// </summary>
namespace SIC.Assembler.Model
{
    /// <summary>
    /// </summary>
    public class Literal : IComparable
    {
        public const string FormatString = "{0, -24}{1, 24}{2, 10}{3, 16}";
        public const int PrintMaxLength = 74;

        private const int DEFAULT_ADDRESS = int.MinValue;
        private const char LITERAL_NUMBER = 'x';
        private const char LITERAL_STRING = 'c';

        public int Address { get; set; }

        public string BytesAsciiString
        {
            get
            {
                StringBuilder str = new StringBuilder();

                if (this.Bytes != null)
                {
                    foreach (var value in this.Bytes)
                    {
                        str.Append((char)value);
                    }
                }

                return str.ToString();
            }
        }

        public int ByteLength
        {
            get
            {
                return this.Bytes.Length;
            }
        }

        public byte[] Bytes { get; private set; }

        public int NumericValue
        {
            get
            {
                return int.Parse(this.BytesHexString, NumberStyles.HexNumber);
            }
        }

        public string BytesDecimalString
        {
            get
            {
                StringBuilder str = new StringBuilder();

                if (this.Bytes != null)
                {
                    foreach (var value in this.Bytes)
                    {
                        str.Append(value);
                    }
                }

                return str.ToString();
            }
        }

        public string Expression { get; set; }

        public string BytesHexString
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

        public LiteralType Type { get; set; }

        public static IEnumerable<byte> GetBytes(string valStr)
        {
            if (string.IsNullOrWhiteSpace(valStr))
            {
                throw new ArgumentException("Can't be null or whitespace", valStr);
            }

            var chunkSize = 0;
            var type = GetLiteralType(valStr, blowUpIfUnknown: true);
            Func<string, byte> transformer = null;

            valStr = valStr.TrimStart('=');

            if (type == LiteralType.StringLiteral || type == LiteralType.ConstantString)
            {
                chunkSize = 1;
                transformer = (str) => (byte)str[0];
                valStr = valStr.TrimStart(LITERAL_STRING).Trim('\'', '"');
            }
            else
            {
                chunkSize = 2;
                transformer = (str) => byte.Parse(str, NumberStyles.HexNumber);

                if (type == LiteralType.JustNumber)
                {
                    valStr = int.Parse(valStr).ToString("x");
                }
                else
                {
                    valStr = valStr.TrimStart(LITERAL_NUMBER).Trim('\'', '"');
                }

                if (valStr.Length % 2 == 1)
                {
                    // If odd # of characters
                    valStr = "0" + valStr;
                }
            }

            return Chunkify(valStr, chunkSize).Select(transformer);
        }

        public static LiteralType GetLiteralType(string literalString, bool blowUpIfUnknown = false)
        {
            const string STRING_REGEX = @"^[=]?c'([^']|(\\'))+'$";
            const string NUMBER_REGEX = @"^[=]?x'[a-f\d]+'$";
            var expr = literalString.Trim();
            var startsWithEquals = expr.StartsWith("=");

            if (Regex.IsMatch(expr, STRING_REGEX, RegexOptions.IgnoreCase))
            {
                if (startsWithEquals)
                {
                    return LiteralType.StringLiteral;
                }
                return LiteralType.ConstantString;
            }
            else if (Regex.IsMatch(expr, NUMBER_REGEX, RegexOptions.IgnoreCase))
            {
                if (startsWithEquals)
                {
                    return LiteralType.NumberLiteral;
                }
                return LiteralType.ConstantNumber;
            }
            else
            {
                int num;
                if (int.TryParse(expr, out num))
                {
                    return LiteralType.JustNumber;
                }
            }

            if (blowUpIfUnknown)
            {
                throw new ArgumentException("Unknown literal type.", literalString);
            }

            return LiteralType.Unknown;
        }

        public static bool IsLiteral(string expr)
        {
            return GetLiteralType(expr) != LiteralType.Unknown;
        }

        public static Literal Parse(string literalString)
        {
            if (string.IsNullOrWhiteSpace(literalString))
            {
                throw new ArgumentNullException("Literal expression can't be null OR empty.", literalString);
            }

            return new Literal
            {
                Expression = literalString,
                Type = GetLiteralType(literalString, blowUpIfUnknown: true),
                Address = DEFAULT_ADDRESS,
                Bytes = GetBytes(literalString.Trim()).ToArray()
            };
        }

        public int CompareTo(object obj)
        {
            var literal = (Literal)obj;
            return this.BytesHexString.CompareTo(literal.BytesHexString);
        }

        public override string ToString()
        {
            if (this.Type == LiteralType.StringLiteral || this.Type == LiteralType.ConstantString)
            {
                return string.Format(FormatString, this.Expression, this.BytesAsciiString, this.ByteLength, this.Address);
            }
            return string.Format(FormatString, this.Expression, this.BytesHexString, this.ByteLength, this.Address);
        }

        private static IEnumerable<string> Chunkify(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }
    }
}