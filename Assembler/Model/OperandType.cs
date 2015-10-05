namespace SIC.Assembler.Model
{
    /// <summary>
    ///
    /// </summary>
    public enum OperandType
    {
        /// <summary>
        /// Simple addressing (Operand is address to value).
        /// </summary>
        Simple,

        /// <summary>
        /// Immediate value.
        /// </summary>
        Immediate,

        /// <summary>
        /// Arithmetic expression.
        /// </summary>
        ArithmeticExpression,

        /// <summary>
        /// Indexed addressing (Add X register and Address).
        /// </summary>
        Indexed,

        /// <summary>
        ///Indirect addressing (Address to Address to value).
        /// </summary>
        Indirect,

        /// <summary>
        /// Literal value that represents a series of character(s).
        /// </summary>
        LiteralString,

        /// <summary>
        /// Literal value that represents a number.
        /// </summary>
        LiteralNumber,

        /// <summary>
        /// Unknown type.
        /// </summary>
        Unknown
    }
}