namespace RegularExpressionScratchpad
{
    /// <summary>
    /// RegexItem.
    /// </summary>
    public abstract class RegexItem
    {
        /// <summary>
        /// Parses the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public void Parse(string expression)
        {
        }

        /// <summary>
        /// Toes the string.
        /// </summary>
        /// <param name="indent">The indent.</param>
        /// <returns>string</returns>
        public abstract string ToString(int indent);
    }
}
