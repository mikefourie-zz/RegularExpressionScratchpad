namespace RegularExpressionScratchpad
{
    /// <summary>
    /// RegexAlternate
    /// </summary>
    public class RegexAlternate : RegexItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegexAlternate"/> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public RegexAlternate(RegexBuffer buffer)
        {
            buffer.AddLookup(this, buffer.Offset, buffer.Offset);

            // skip "|"
            buffer.MoveNext();
        }

        /// <summary>
        /// To the string.
        /// </summary>
        /// <param name="indent">The indent.</param>
        /// <returns>string</returns>
        public override string ToString(int indent)
        {
            return new string(' ', indent) + "or";
        }
    }
}
