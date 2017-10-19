namespace RegularExpressionScratchpad
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// RegexCharClass.
    /// </summary>
    public class RegexCharClass : RegexItem
    {
        // RegexExpression expression;
        private readonly string description;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexCharClass"/> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public RegexCharClass(RegexBuffer buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer), "RegexBuffer is null");
            }

            int startLoc = buffer.Offset;

            buffer.MoveNext();

            Regex regex = new Regex(@"(?<Negated>\^?)(?<Class>.+?)\]");           
            Match match = regex.Match(buffer.String);
            if (match.Success)
            {
                this.description = string.Format(match.Groups["Negated"].ToString() == "^" ? "Any character not in \"{0}\"" : "Any character in \"{0}\"", match.Groups["Class"]);
                buffer.Offset += match.Groups[0].Length;
            }
            else
            {
                this.description = "missing ']' in character class";
            }

            buffer.AddLookup(this, startLoc, buffer.Offset - 1);
        }

        /// <summary>
        /// Toes the string.
        /// </summary>
        /// <param name="indent">The indent.</param>
        /// <returns>string</returns>
        public override string ToString(int indent)
        {
            return this.description;
        }
    }
}
