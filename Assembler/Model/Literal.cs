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
        /// <summary>
        /// The format string
        /// </summary>
        public const string FormatString = "{0, -24}{1, 24}{2, 10}{3, 16}";

        /// <summary>
        /// The print maximum length
        /// </summary>
        public const int PrintMaxLength = 74;

        /// <summary>
        /// The litera l_ number
        /// </summary>
        private const char LITERAL_NUMBER = 'x';

        /// <summary>
        /// The litera l_ string
        /// </summary>
        private const char LITERAL_STRING = 'c';

        /// <summary>
        /// Gets or sets the address of this Literal{T}.
        /// </summary>
        public int Address { get; set; }

        /// <summary>
        /// Gets or sets the length of the byte.
        /// </summary>
        /// <value>The length of the byte.</value>
        public int ByteLength
        {
            get
            {
                return this.Bytes.Length;
            }
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        public byte[] Bytes { get; private set; }

        /// <summary>
        /// Gets or sets the expression of this Literal{T}.
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// Gets or sets the type of this Literal.
        /// </summary>
        public LiteralType Type { get; set; }

        /// <summary>
        /// Gets the value of this Literal{T}.
        /// </summary>
        public string ValueStr
        {
            get
            {
                StringBuilder str = new StringBuilder();

                if (this.Bytes != null)
                {
                    if (this.Type == LiteralType.NumberLiteral)
                    {
                        foreach (var value in this.Bytes)
                        {
                            str.Append(value.ToString("X2"));
                        }
                    }
                    else
                    {
                        foreach (var value in this.Bytes)
                        {
                            str.Append((char)value);
                        }
                    }
                }

                return str.ToString();
            }
        }

        /// <summary>
        /// Parses the value.
        /// </summary>
        /// <param name="valStr">The value string.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Can't be null or whitespace</exception>
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

        /// <summary>
        /// Gets the type of the literal.
        /// </summary>
        /// <param name="literalString">The literal string.</param>
        /// <returns></returns>
        public static LiteralType GetLiteralType(string literalString, bool blowUpIfUnknown = false)
        {
            const string STRING_REGEX = @"^[=]?c'([^']|(\\'))+'$";
            const string NUMBER_REGEX = @"^[=]?x'[\d]+'$";
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

        /// <summary>
        /// Parses the specified literal string.
        /// </summary>
        /// <param name="literalString">The literal string.</param>
        /// <returns></returns>
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
                Address = int.MinValue,
                Bytes = GetBytes(literalString.Trim()).ToArray()
            };
        }

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            var literal = (Literal)obj;
            return this.ValueStr.CompareTo(literal.ValueStr);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format(FormatString, this.Expression, this.ValueStr, this.ByteLength, this.Address);
        }

        /// <summary>
        /// Chunkifies the specified string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="chunkSize">Size of the chunk.</param>
        /// <returns></returns>
        private static IEnumerable<string> Chunkify(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }
    }
}