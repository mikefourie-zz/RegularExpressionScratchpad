namespace RegularExpressionScratchpad
{
    using System;
    using System.Text;

    /// <summary>
    ///  RegexConditional.
    /// </summary>
    public class RegexConditional : RegexItem
    {
        private readonly RegexExpression expression;
        private readonly RegexExpression yesNo;
        private readonly int startLocation;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexConditional"/> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public RegexConditional(RegexBuffer buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer), "RegexBuffer is null");
            }

            this.startLocation = buffer.Offset;

            this.expression = new RegexExpression(buffer);
            this.CheckClosingParen(buffer);

            this.yesNo = new RegexExpression(buffer);
            this.CheckClosingParen(buffer);

            buffer.AddLookup(this, this.startLocation, buffer.Offset - 1);
        }

        /// <summary>
        /// Toes the string.
        /// </summary>
        /// <param name="indent">The indent.</param>
        /// <returns>string</returns>
        public override string ToString(int indent)
        {
            string indents = new string(' ', indent);
            StringBuilder result = new StringBuilder();
            result.Append(indents + "if: " + this.expression.ToString(0));

            result.Append(indents + "match: ");

                // walk through until we find an alternation
            foreach (RegexItem item in this.yesNo.Items)
            {
                if (item is RegexAlternate)
                {
                    result.Append("\r\n" + indent + "else match: ");
                }
                else
                {
                    result.Append(item.ToString(indent));
                }
            }

            result.Append("\r\n");
            return result.ToString();
        }

        /// <summary>
        /// Checks the closing paren.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        private void CheckClosingParen(RegexBuffer buffer)
        {
            // check for closing ")"
            char current;
            try
            {
                current = buffer.Current;
            }
            catch (Exception e)
            {
                // no closing brace. Set highlight for this capture...
                buffer.ErrorLocation = this.startLocation;
                buffer.ErrorLength = 1;
                throw new Exception("Missing closing \')\' in capture", e);
            }

            if (current != ')')
            {
                throw new Exception($"Unterminated closure at offset {buffer.Offset}");
            }

            // eat closing parenthesis
            buffer.Offset++;
        }
    }
}
