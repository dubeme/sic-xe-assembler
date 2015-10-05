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
    /// <typeparam name="T"></typeparam>
    public class Literal : IComparable
    {
        public const string FormatString = "{0, -20}{1, -20}{2, -20}{3, -20}";
        private const string LITERAL_NUMBER = "=x";
        private const string LITERAL_STRING = "=c";
        private const int PrintMaxLength = 80;

        /// <summary>
        /// Gets or sets the address of this Literal{T}.
        /// </summary>
        public int Address { get; set; }

        /// <summary>
        /// Gets or sets the length of the byte.
        /// </summary>
        public int ByteLength { get; set; }

        /// <summary>
        /// Gets or sets the name of this Literal{T}.
        /// </summary>
        public string Name { get; set; }

        public LiteralType Type { get; set; }

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
                            str.Append(value.ToString("X"));
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

        public static string HeaderText()
        {
            StringBuilder str = new StringBuilder();
            str.AppendFormat(FormatString, "Name", "Value", "Length", "Address");
            str.AppendLine();

            for (int i = 0; i < PrintMaxLength; i++)
            {
                str.Append("-");
            }

            return str.ToString();
        }

        public static Literal Parse(string literalString)
        {
            if (string.IsNullOrWhiteSpace(literalString))
            {
                return null;
            }

            var address = int.MinValue;
            var type = LiteralType.Unknown;
            var name = literalString;

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
                values = Chunkify(literalString, 2)
                    .Select(str => int.Parse(str, NumberStyles.HexNumber))
                    .ToList();
            }
            return new Literal
            {
                Name = name,
                Type = type,
                Address = address,
                ByteLength = values.Count,
                Values = values.ToArray()
            };
        }

        public int CompareTo(object obj)
        {
            var literal = (Literal)obj;
            return this.ValueStr.CompareTo(literal.ValueStr);
        }

        public override string ToString()
        {
            return string.Format(FormatString, this.Name, this.ValueStr, this.ByteLength, this.Address);
        }

        private static IEnumerable<string> Chunkify(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }
    }
}