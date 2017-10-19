namespace RegularExpressionScratchpad
{
    using System.Collections;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// RegexExpression.
    /// </summary>
    public class RegexExpression : RegexItem
    {
        private readonly ArrayList items = new ArrayList();

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexExpression"/> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public RegexExpression(RegexBuffer buffer)
        {
            this.Parse(buffer);
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public ArrayList Items => this.items;

        /// <summary>
        /// Override ToString()
        /// </summary>
        /// <param name="indent">The indent.</param>
        /// <returns>string</returns>
        public override string ToString(int indent)
        {
            StringBuilder buf = new StringBuilder();
            StringBuilder bufChar = new StringBuilder();

            foreach (RegexItem item in this.items)
            {
                if (item is RegexCharacter regexChar && !regexChar.Special)
                {
                    bufChar.Append(regexChar.ToString(indent));
                }
                else
                {
                    // add any buffered chars...
                    if (bufChar.Length != 0)
                    {
                        buf.Append(new string(' ', indent));
                        buf.Append(bufChar + "\r\n");
                        bufChar = new StringBuilder();
                    }

                    buf.Append(new string(' ', indent));
                    string itemString = item.ToString(indent);
                    if (itemString.Length != 0)
                    {
                        buf.Append(itemString);
                        Regex newLineAlready = new Regex(@"\r\n$");
                        if (!newLineAlready.IsMatch(itemString))
                        {
                            buf.Append("\r\n");
                        }
                    }
                }
            }

            if (bufChar.Length != 0)
            {
                buf.Append(new string(' ', indent));
                buf.Append(bufChar + "\r\n");
            }

            return buf.ToString();
        }
        
        // eat the whole comment until the end of line...
        private static void EatComment(RegexBuffer buffer)
        {
            while (buffer.Current != '\r')
            {
                buffer.MoveNext();
            }
        }

        /// <summary>
        /// Parses the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        private void Parse(RegexBuffer buffer)
        {
            while (!buffer.AtEnd)
            {
                // if this regex ignores whitespace, we need to ignore these
                if (buffer.IgnorePatternWhitespace && ((buffer.Current == ' ') || (buffer.Current == '\r') || (buffer.Current == '\n') || (buffer.Current == '\t')))
                {
                    buffer.MoveNext();
                }
                else
                {
                    switch (buffer.Current)
                    {
                        case '(':
                            this.items.Add(new RegexCapture(buffer));
                            break;
                        case ')':
                            // end of closure; just return.
                            return;
                        case '[':
                            this.items.Add(new RegexCharClass(buffer));
                            break;
                        case '{':
                            this.items.Add(new RegexQuantifier(buffer));
                            break;
                        case '|':
                            this.items.Add(new RegexAlternate(buffer));
                            break;
                        case '\\':
                            this.items.Add(new RegexCharacter(buffer));
                            break;
                        case '#':
                            if (buffer.IgnorePatternWhitespace)
                            {
                                EatComment(buffer);
                            }
                            else
                            {
                                this.items.Add(new RegexCharacter(buffer));
                            }

                            break;
                        default:
                            this.items.Add(new RegexCharacter(buffer));
                            break;
                    }
                }
            }
        }
    }
}
