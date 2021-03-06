﻿using System;
using System.Text;
using System.Text.RegularExpressions;

namespace SIC.Assembler.Model
{
    /// <summary>
    ///
    /// </summary>
    public class Symbol : IComparable
    {
        public const string LABEL_FIRST_CHAR_PATTERN = "^[a-zA-Z]";
        public const string LABEL_PATTERN = "^([a-zA-Z])[\\w]{0,20}$";
        public const string RFLAG_FALSE_PATTERN = "^(false)$|^(f)$|^(0)$";
        public const string RFLAG_PATTERN = "^(true|false)$|^(t|f)$|^(1|0)$";
        public const string RFLAG_TRUE_PATTERN = "^(true)$|^(t)$|^(1)$";
        public const string VALUE_PATTERN = "^(\\+|-)?\\d+$";
        private const int LABEL_MAX_LENGTH = 21;
        private const int LABEL_MIN_LENGTH = 1;
        private const int LABEL_TRIM_LENGTH = 6;
        private const string FORMAT_STRING = "{0, -21}{1, -15:X5}{2, -15}{3, -15}";
        private static string TOP_BAR = new string('-', 64);
        private string _label;
        private string _longLabel;

        protected Symbol()
        {
        }

        public bool ExternalFlag { get; set; }

        public string Label
        {
            get
            {
                this._label = this._label ?? Symbol.ParseSymbolLabel(this._longLabel);
                return this._label;
            }
            set
            {
                this._label = Symbol.ParseSymbolLabel(value);
            }
        }

        public string LongLabel
        {
            get
            {
                this._longLabel = this._longLabel ?? this._label;
                return this._longLabel;
            }
            set
            {
                this._longLabel = Symbol.ParseSymbolLabel(value, false, false);
            }
        }

        public bool MFlag { get; set; }
        public bool RelocatableFlag { get; set; }
        public int Value { get; set; }

        public static Symbol CreateSymbol(string label, int value, bool isRelocatable)
        {
            return new Symbol
            {
                Value = value,
                Label = label,
                RelocatableFlag = isRelocatable,
                LongLabel = label
            };
        }

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

            return CreateSymbol(ParseSymbolLabel(tokens[1], false, false), ParseSymbolValue(tokens[0]), ParseSymbolRFlag(tokens[2]));
        }

        public static string ParseSymbolLabel(string label, bool toLower = true, bool shorten = true)
        {
            string reason = "";
            string expected = "";
            string actual = label;

            try
            {
                label = label.Trim();

                if (toLower)
                {
                    label = label.ToLower();
                }

                if (!(label.Length >= LABEL_MIN_LENGTH && label.Length <= LABEL_MAX_LENGTH))
                {
                    reason = "Out of range";
                    expected = string.Format("[{0},{1}] Alphabets, Numbers and Underscore", LABEL_MIN_LENGTH, LABEL_MAX_LENGTH);
                    throw new Exception();
                }

                if (!Regex.IsMatch(label, LABEL_FIRST_CHAR_PATTERN))
                {
                    reason = "Invalid first character";
                    expected = "Alphabets";
                    throw new Exception();
                }

                if (!Regex.IsMatch(label, LABEL_PATTERN))
                {
                    reason = "Invalid symbol";
                    expected = "Alphabets, Numbers and Underscore";
                    throw new Exception();
                }

                if (shorten)
                {
                    return label.Length > LABEL_TRIM_LENGTH ? label.Substring(0, LABEL_TRIM_LENGTH) : label;
                }

                return label;
            }
            catch (Exception)
            {
                var err = InvalidSymbolMessage("Label", reason, expected, actual);
                throw new ArgumentException(err);
            }
        }

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

        public int CompareTo(object obj)
        {
            if (obj is string)
            {
                // This is the scenario where we
                // compare a Symbol object to a string(Symbol label)
                var symbolLabel = obj.ToString();
                return this.Label.CompareTo(Symbol.ParseSymbolLabel(symbolLabel));
            }

            var other = (Symbol)obj;
            return this.Label.CompareTo(other.Label);
        }

        public override string ToString()
        {
            return string.Format(FORMAT_STRING, this.LongLabel, this.Value, this.RelocatableFlag, this.MFlag);
        }

        public static string GetSymbolHeader()
        {
            return string.Format("{0}\n{1}", string.Format(FORMAT_STRING, "Label", "Value", "R Flag", "M Flag"), TOP_BAR);;
        }

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