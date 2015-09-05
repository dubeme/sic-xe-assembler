using System;
using System.Text.RegularExpressions;

namespace SIC.Assembler.Providers.SymbolTable
{
    /// <summary>
    ///
    /// </summary>
    public class Symbol : IComparable
    {
        public const string LabelErrorMessage = "The [Symbol Label] token {0} is invalid.";
        public const string LabelPattern = "^([a-zA-Z])[\\w]{1,20}$";
        public const string RFlagErrorMessage = "The [R Flag] token {0} is invalid.";
        public const string RFlagFalsePattern = "^(false)$|^(f)$|^(0)$";
        public const string RFlagPattern = "^(true|false)$|^(t|f)$|^(1|0)$";
        public const string RFlagTruePattern = "^(true)$|^(t)$|^(1)$";
        public const string ValueErrorMessage = "The [Value] token {0} is invalid.";
        public const string ValuePattern = "^\\d+$";

        /// <summary>
        /// Gets or sets a value indicating whether this instance has multiple.
        /// </summary>
        public bool MFlag { get; set; }

        /// <summary>
        /// Gets or sets the key of this Symbol.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is relocatable.
        /// </summary>
        public bool RFlag { get; set; }

        /// <summary>
        /// Gets or sets the value of this Symbol.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Parses the specified string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>A symbol object</returns>
        /// <exception cref="ArgumentNullException">Input string can't be null OR empty.</exception>
        /// <exception cref="ArgumentException">
        /// The input doesn't have the correct amount of tokens [3].
        /// or The wrong value is provided for the Value
        /// or The wrong value is provided for the Symbol label
        /// or The wrong value is provided for the R Flag
        /// </exception>
        public static Symbol Parse(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentNullException("Input string can't be null OR empty.");
            }

            var tokens = Regex.Replace(str.Trim(), "\\s+", " ").Split(' ');

            if (tokens.Length != 3)
            {
                throw new ArgumentException("The input doesn't have the correct amount of tokens [3].");
            }
            if (!Regex.IsMatch(tokens[0], ValuePattern))
            {
                throw new ArgumentException(string.Format(ValueErrorMessage, tokens[0]));
            }
            if (!Regex.IsMatch(tokens[1], LabelPattern, RegexOptions.IgnoreCase))
            {
                // Todo: VERIFY IF THE SYMBOL HAS TO BE CASE SENSITIVE
                throw new ArgumentException(string.Format(LabelErrorMessage, tokens[1]));
            }
            if (!Regex.IsMatch(tokens[2], RFlagPattern, RegexOptions.IgnoreCase))
            {
                throw new ArgumentException(string.Format(RFlagErrorMessage, tokens[2]));
            }

            return new Symbol
            {
                Value = int.Parse(tokens[0]),
                Key = tokens[1].Substring(0, 6),
                RFlag = Regex.IsMatch(tokens[2], RFlagTruePattern, RegexOptions.IgnoreCase)
            };
        }



        /// <summary>
        /// Tries the parse.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="symbol">The symbol.</param>
        /// <returns></returns>
        public static bool TryParse(string str, out Symbol symbol)
        {
            symbol = null;
            try
            {
                symbol = Parse(str);
            }
            catch (System.Exception ex)
            {
                // Todo: Handle exception
            }

            return symbol != null;
        }

        public int CompareTo(object obj)
        {
            var other = (Symbol)obj;

            return this.Key.CompareTo(other.Key);
        }

        public override string ToString()
        {
            return string.Format("{0, -15}{1, -15}{2, -15}{3, -15}", this.Key, this.Value, this.RFlag, this.MFlag);
        }
    }
}