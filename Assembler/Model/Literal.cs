using System.Text;

namespace SIC.Assembler.Model
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Literal
    {
        public const string FormatString = "{0, -20}{1, -20}{2, -20}{3, -20}";
        private const int PrintMaxLength = 80;

        /// <summary>
        /// Gets or sets the address of this Literal{T}.
        /// </summary>
        public int Address { get; set; }

        /// <summary>
        /// Gets or sets the length of the byte.
        /// </summary>
        public byte ByteLength { get; set; }

        /// <summary>
        /// Gets or sets the name of this Literal{T}.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of this Literal{T}.
        /// </summary>
        public string Value { get; set; }

        public static Literal Parse(string literalString)
        {
            return new Literal();
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

            str.AppendLine();

            return str.ToString();
        }

        public override string ToString()
        {
            return string.Format(FormatString, this.Name, this.Value, this.ByteLength, this.Address);
        }
    }
}