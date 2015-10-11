using System;
using System.Text;
using System.Text.RegularExpressions;

namespace SIC.Assembler.Model
{
    /// <summary>
    ///
    /// </summary>
    public class Symbol : IComparable
    {
        /// <summary>
        /// The labe l_ pattern
        /// </summary>
        public const string LABEL_PATTERN = "^([a-z])[\\w]{0,20}$";

        /// <summary>
        /// The rfla g_ fals e_ pattern
        /// </summary>
        public const string RFLAG_FALSE_PATTERN = "^(false)$|^(f)$|^(0)$";

        /// <summary>
        /// The rfla g_ pattern
        /// </summary>
        public const string RFLAG_PATTERN = "^(true|false)$|^(t|f)$|^(1|0)$";

        /// <summary>
        /// The rfla g_ tru e_ pattern
        /// </summary>
        public const string RFLAG_TRUE_PATTERN = "^(true)$|^(t)$|^(1)$";

        /// <summary>
        /// The valu e_ pattern
        /// </summary>
        public const string VALUE_PATTERN = "^(\\+|-)?\\d+$";

        private const int LABEL_MAX_LENGTH = 21;
        private const int LABEL_MIN_LENGTH = 1;
        private const int LABEL_TRIM_LENGTH = 21;

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
        /// <param name="codeLine">The string.</param>
        /// <returns>
        /// A symbol object
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// </exception>
        /// <exception cref="ArgumentNullException">Input string can't be null OR empty.</exception>
        /// <exception cref="ArgumentException">The input doesn't have the correct amount of tokens [3].
        /// or The wrong value is provided for the Value
        /// or The wrong value is provided for the Symbol label
        /// or The wrong value is provided for the R Flag</exception>
        public static Symbol Parse(string codeLine)
        {
            if (string.IsNullOrWhiteSpace(codeLine))
            {
                var err = InvalidSymbolMessage("", "Empty OR Null", "3 tokens [Value Label RFlag]", codeLine);
                throw new ArgumentException(err);
            }

            var tokens = Regex.Replace(codeLine.Trim(), "\\s+", " ").Split(' ');

            if (tokens.Length != 3)
            {
                var err = InvalidSymbolMessage("", "Invalid number of tokens", "3 tokens [Value Label RFlag]", codeLine);
                throw new ArgumentException(err);
            }

            return new Symbol
            {
                Value = ParseSymbolValue(tokens[0]),
                Label = ParseSymbolLabel(tokens[1]),
                RFlag = ParseSymbolRFlag(tokens[2])
            };
        }

        /// <summary>
        /// Parses the symbol label.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// </exception>
        /// <exception cref="System.ArgumentException"></exception>
        public static string ParseSymbolLabel(string label)
        {
            string reason = "";
            string expected = "";
            string actual = label;

            try
            {
                label = label.Trim().ToLower();

                if (!(label.Length >= LABEL_MIN_LENGTH && label.Length <= LABEL_MAX_LENGTH))
                {
                    reason = "Out of range";
                    expected = string.Format("[{0},{1}] Aphabets, Numbers and Underscore", LABEL_MIN_LENGTH, LABEL_MAX_LENGTH);
                    throw new Exception();
                }

                if (!Regex.IsMatch(label, "^[a-z]"))
                {
                    reason = "Invalid first character";
                    expected = "Alphabets";
                    throw new Exception();
                }

                if (!Regex.IsMatch(label, LABEL_PATTERN))
                {
                    reason = "Invalid symbol";
                    expected = "Aphabets, Numbers and Underscore";
                    throw new Exception();
                }

                return label.Length > LABEL_TRIM_LENGTH ? label.Substring(0, LABEL_TRIM_LENGTH) : label;
            }
            catch (Exception)
            {
                var err = InvalidSymbolMessage("Label", reason, expected, actual);
                throw new ArgumentException(err);
            }
        }

        /// <summary>
        /// Parses the symbol r flag.
        /// </summary>
        /// <param name="rFlag">The r flag.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public static bool ParseSymbolRFlag(string rFlag)
        {
            var actual = rFlag;
            try
            {
                rFlag = rFlag.Trim().ToLower();

                if (!Regex.IsMatch(rFlag, RFLAG_PATTERN))
                {
                    throw new Exception();
                }

                return Regex.IsMatch(rFlag, RFLAG_TRUE_PATTERN);
            }
            catch (Exception)
            {
                var err = InvalidSymbolMessage("R Flag", "Not a valid boolean value", "[true\\false, t\\f, 1\\0]", actual);
                throw new ArgumentException(err);
            }
        }

        /// <summary>
        /// Parses the symbol value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public static int ParseSymbolValue(string value)
        {
            var actual = value;
            try
            {
                value = value.Trim();
                if (!Regex.IsMatch(value, VALUE_PATTERN))
                {
                    throw new Exception();
                }
                return int.Parse(value);
            }
            catch (Exception)
            {
                var err = InvalidSymbolMessage("Value", "Not a valid integer", "32 bit integer", actual);
                throw new ArgumentException(err);
            }
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
            catch (Exception ex)
            {
                // Todo: Handle exception
            }

            return symbol != null;
        }

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj is string )
            {
                // This is the scenario where we
                // compare a Symbol object to a string(Symbol label)
                var symbolLabel = obj.ToString();
                return this.Label.CompareTo(Symbol.ParseSymbolLabel(symbolLabel));
            }

            var other = (Symbol)obj;
            return this.Label.CompareTo(other.Label);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0, -15}{1, -15}{2, -15}{3, -15}", this.Label, this.Value, this.RFlag, this.MFlag);
        }

        /// <summary>
        /// Invalids the symbol message.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="reason">The reason.</param>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        /// <returns></returns>
        private static string InvalidSymbolMessage(string property, string reason, string expected, string actual)
        {
            if (string.IsNullOrWhiteSpace(actual))
            {
                if (actual == null)
                {
                    actual = "NULL";
                }
                else
                {
                    actual = "EMPTY_STRING";
                }
            }

            StringBuilder errMsg = new StringBuilder();
            errMsg.AppendLine("Invalid Symbol " + property);
            errMsg.Append("\t" + string.Format("{0, -10}{1}\n", "Reason:", reason));
            errMsg.Append("\t" + string.Format("{0, -10}{1}\n", "Expected:", expected));
            errMsg.Append("\t" + string.Format("{0, -10}{1}", "Actual:", actual));

            return errMsg.ToString();
        }
    }
}