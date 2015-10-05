using System.Text;

namespace SIC.Assembler.Model
{
    public class Operand
    {
        public const string FormatString = "{0, -20}{1, -10}{2, -15}{3, -10}{4, -10}{5, -10}";
        private const int PrintMaxLength = 75;

        // NIXBPE
        public string Expression { get; set; }

        public int NumericValue { get; set; }
        public bool Relocatable { get; set; }
        public OperandType Type { get; set; }

        public static string HeaderText()
        {
            StringBuilder str = new StringBuilder();
            str.AppendFormat(FormatString, "Input Expr", "Value", "Relocatable", "Index", "Indirect", "Immediate");
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
            return string.Format(FormatString,
                this.Expression,
                this.NumericValue,
                this.Relocatable,
                this.Type == OperandType.Indexed,
                this.Type == OperandType.Indirect,
                this.Type == OperandType.Immediate);
        }
    }
}