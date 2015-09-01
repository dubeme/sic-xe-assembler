namespace SIC.Assembler.Providers.SymbolTable
{
    /// <summary>
    ///
    /// </summary>
    public class Symbol
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance has multiple.
        /// </summary>
        public bool HasMultiple { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is relocatable.
        /// </summary>
        public bool IsRelocatable { get; set; }

        /// <summary>
        /// Gets or sets the key of this Symbol.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value of this Symbol.
        /// </summary>
        public int Value { get; set; }
    }
}