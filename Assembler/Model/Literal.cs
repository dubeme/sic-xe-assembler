using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

/// <summary>
///
/// </summary>
namespace SIC.Assembler.Model
{
    /// <summary>
    ///
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
        private const string LITERAL_NUMBER = "=x";

        /// <summary>
        /// The litera l_ string
        /// </summary>
        private const string LITERAL_STRING = "=c";

        /// <summary>
        /// Gets or sets the address of this Literal{T}.
        /// </summary>
        public int Address { get; set; }

        /// <summary>
        /// Gets or sets the length of the byte.
        /// </summary>
        /// <value>
        /// The length of the byte.
        /// </value>
        public int ByteLength { get; set; }

        /// <summary>
        /// Gets or sets the expression of this Literal{T}.
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// Gets or sets the type of this Literal.
        /// </summary>
        public LiteralType Type { get; set; }

        /// <summary>
        /// Gets the values.
        /// </summary>
        public int[] Values { get; private set; }

        /// <summary>
        /// Gets the value of this Literal{T}.
        /// </summary>
        public string ValueStr
        {
            get
            {
                StringBuilder str = new StringBuilder();

                if (this.Values != null)
                {
                    if (this.Type == LiteralType.Number)
                    {
                        foreach (var value in this.Values)
                        {
                            str.Append(value.ToString("X2"));
                        }
                    }
                    else
                    {
                        foreach (var value in this.Values)
                        {
                            str.Append((char)value);
                        }
                    }
                }

                return str.ToString();
            }
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
                return null;
            }

            var address = int.MinValue;
            var type = LiteralType.Unknown;
            var expression = literalString;

            var values = new List<int>();
            if (literalString.StartsWith(LITERAL_STRING))
            {
                type = LiteralType.String;
                literalString = literalString.Replace(LITERAL_STRING, "").Replace("'", "");
                values = Chunkify(literalString, 1)
                    .Select(str => (int)str[0])
                    .ToList();
            }
            else if (literalString.StartsWith(LITERAL_NUMBER))
            {
                type = LiteralType.Number;
                literalString = literalString.Replace(LITERAL_NUMBER, "").Replace("'", "");

                if (literalString.Length%2 == 1)
                {
                    // If odd # of characters
                    literalString = "0" + literalString;
                }

                values = Chunkify(literalString, 2)
                    .Select(str => int.Parse(str, NumberStyles.HexNumber))
                    .ToList();
            }
            return new Literal
            {
                Expression = expression,
                Type = type,
                Address = address,
                ByteLength = values.Count,
                Values = values.ToArray()
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
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
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