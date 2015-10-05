using SIC.Assembler.Model;
using SIC.Assembler.Utilities.Collections;
using System.Text;

namespace SIC.Assembler.Providers
{
    public class LiteralTable
    {
        private static int Count = 0;
        private NonDuplicateLinkedList<Literal> Literals = new NonDuplicateLinkedList<Literal>();

        public Literal AddLiteral(string literalExpression, bool addToTable = true)
        {
            var literal = Literal.Parse(literalExpression);

            if (addToTable)
            {
                literal.Address = GenerateAddress();
                this.Literals.Add(literal);
            }

            return literal;
        }

        public Literal FindLiteral(string expression)
        {
            return this.Literals.Find(AddLiteral(expression));
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            if (this.Literals.Count > 0)
            {
                str.AppendLine(Literal.HeaderText());
                foreach (var literal in this.Literals)
                {
                    str.AppendLine(literal.ToString());
                }
            }

            return str.ToString();
        }

        private int GenerateAddress()
        {
            Count += 1;
            return Count;
        }
    }
}