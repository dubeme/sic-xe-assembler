namespace SIC.Assembler.Model
{
    /// <summary>
    ///
    /// </summary>
    public enum LiteralType
    {
        /// <summary>
        /// Unknown literal type.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// A string literal.
        /// </summary>
        StringLiteral = 1,

        /// <summary>
        /// An int literal.
        /// </summary>
        NumberLiteral = 2,

        /// <summary>
        /// The just number
        /// </summary>
        JustNumber = 3,

        /// <summary>
        /// The constant number
        /// </summary>
        WordConstant = 4,

        /// <summary>
        /// The constant string
        /// </summary>
        ByteConstant = 5
    }
}