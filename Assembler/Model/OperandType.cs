namespace SIC.Assembler.Model
{
    /// <summary>
    ///
    /// </summary>
    public enum OperandType
    {
        /// <summary>
        /// Unknown type.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Simple addressing (Operand is address to value).
        /// </summary>
        Simple = 1,

        /// <summary>
        /// Immediate value.
        /// </summary>
        Immediate = 2,

        /// <summary>
        /// Arithmetic expression.
        /// </summary>
        ArithmeticExpression = 3,

        /// <summary>
        /// Indexed addressing (Add X register and Address).
        /// </summary>
        Indexed = 4,

        /// <summary>
        ///Indirect addressing (Address to Address to value).
        /// </summary>
        Indirect = 5,

        /// <summary>
        /// Literal value that represents a series of character(s).
        /// </summary>
        LiteralString = 6,

        /// <summary>
        /// Literal value that represents a number.
        /// </summary>
        LiteralNumber = 7,

        JustNumber = 8,
        ConstantNumber = 9,
        ConstantString = 10
    }
}