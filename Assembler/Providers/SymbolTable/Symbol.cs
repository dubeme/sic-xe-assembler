using System;
using System.Text.RegularExpressions;

namespace SIC.Assembler.Providers.SymbolTable
{
    /// <summary>
    ///
    /// </summary>
    public class Symbol : IComparable
    {
        public const string LabelErrorMessage = "The [Symbol Label] token \"{0}\" is invalid.";
        public const string LabelPattern = "^([a-zA-Z])[\\w]{1,20}$";
        public const string RFlagErrorMessage = "The [R Flag] token \"{0}\" is invalid.";
        public const string RFlagFalsePattern = "^(false)$|^(f)$|^(0)$";
        public const string RFlagPattern = "^(true|false)$|^(t|f)$|^(1|0)$";
        public const string RFlagTruePattern = "^(true)$|^(t)$|^(1)$";
        public const string ValueErrorMessage = "The [Value] token \"{0}\" is invalid.";
        public const string ValuePattern = "^\\d+$";

        /// <summary>
        /// Gets or sets the label of this Symbol.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has multiple.
        /// </summary>
        public bool MFlag { get; set; }

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

            return new Symbol
            {
                Value = ParseSymbolValue(tokens[0]),
                Label = ParseSymbolLabel(tokens[1]),
                RFlag = ParseSymbolRFlag(tokens[2])
            };
        }

        public static string ParseSymbolLabel(string label)
        {
            if (!Regex.IsMatch(label, LabelPattern, RegexOptions.IgnoreCase))
            {
                // Todo: VERIFY IF THE SYMBOL LABEL HAS TO BE CASE SENSITIVE
                throw new ArgumentException(string.Format(LabelErrorMessage, label));
            }

            return label.Substring(0, 6);
        }

        public static bool ParseSymbolRFlag(string rFlag)
        {
            if (!Regex.IsMatch(rFlag, RFlagPattern, RegexOptions.IgnoreCase))
            {
                throw new ArgumentException(string.Format(RFlagErrorMessage, rFlag));
            }

            return Regex.IsMatch(rFlag, RFlagTruePattern, RegexOptions.IgnoreCase);
        }

        public static int ParseSymbolValue(string value)
        {
            if (!Regex.IsMatch(value, ValuePattern))
            {
                throw new ArgumentException(string.Format(ValueErrorMessage, value));
            }

            return int.Parse(value);
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
            if (obj is string)
            {
                // This is the scenario where we
                // compare a Symbol object to a string(Symbol label)
                return this.Label.CompareTo(obj);
            }

            var other = (Symbol)obj;
            return this.Label.CompareTo(other.Label);
        }

        public override string ToString()
        {
            return string.Format("{0, -15}{1, -15}{2, -15}{3, -15}", this.Label, this.Value, this.RFlag, this.MFlag);
        }
    }
}