using SIC.Assembler.Model;
using SIC.Assembler.Utilities.Collections;
using System;
using System.Text;

/// <summary>
///
/// </summary>
namespace SIC.Assembler.Providers
{
    /// <summary>
    ///
    /// </summary>
    public class LiteralTable
    {
        /// <summary>
        /// The count
        /// </summary>
        private static int Count = 0;

        /// <summary>
        /// The literals
        /// </summary>
        private NonDuplicateLinkedList<Literal> Literals = new NonDuplicateLinkedList<Literal>();
        
        public Literal ParseLiteral(string literalExpression, bool addToTable = true)
        {
            var literal = Literal.Parse(literalExpression);

            if (addToTable)
            {
                var foundLiteral = this.Literals.Find(literal, AreLiteralsEqual);

                if (foundLiteral == null)
                {
                    literal.Address = GenerateAddress();
                    this.Literals.Add(literal);
                    return literal;
                }

                return foundLiteral;
            }

            return literal;
        }

        /// <summary>
        /// Finds the literal.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public Literal FindLiteral(string expression)
        {
            return this.Literals.Find(Literal.Parse(expression), AreLiteralsEqual);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            if (this.Literals.Count > 0)
            {
                str.AppendLine(string.Format(Literal.FormatString, "Expression", "Value", "Length", "Address"));
                str.AppendLine(new string('-', Literal.PrintMaxLength));

                foreach (var literal in this.Literals)
                {
                    str.AppendLine(literal.ToString());
                }
            }

            return str.ToString();
        }

        /// <summary>
        /// Ares the literals equal.
        /// </summary>
        /// <param name="literalA">The literal a.</param>
        /// <param name="LiteralB">The literal b.</param>
        /// <returns></returns>
        private bool AreLiteralsEqual(Literal literalA, Literal LiteralB)
        {
            return literalA.CompareTo(LiteralB) == 0;
        }

        /// <summary>
        /// Generates the address.
        /// </summary>
        /// <returns></returns>
        private int GenerateAddress()
        {
            Count += 1;
            return Count;
        }
    }
}