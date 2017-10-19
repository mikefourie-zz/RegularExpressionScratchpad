namespace RegularExpressionScratchpad
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// RegexQuantifier.
    /// </summary>
    public class RegexQuantifier : RegexItem
    {
        private readonly string description;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexQuantifier"/> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public RegexQuantifier(RegexBuffer buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer), "RegexBuffer is null");
            }

            int startLoc = buffer.Offset;
            buffer.MoveNext();

            // look for "n}", "n,}", or "n,m}"
            Regex regex = new Regex(@"(?<n>\d+)(?<Comma>,?)(?<m>\d*)\}");
            Match match = regex.Match(buffer.String);
            if (match.Success)
            {
                if (match.Groups["m"].Length != 0)
                {
                    this.description = $"At least {match.Groups["n"]}, but not more than {match.Groups["m"]} times";
                }
                else if (match.Groups["Comma"].Length != 0)
                {
                    this.description = $"At least {match.Groups["n"]} times";
                }
                else
                {
                    this.description = $"Exactly {match.Groups["n"]} times";
                }

                buffer.Offset += match.Groups[0].Length;

                if (!buffer.AtEnd && buffer.Current == '?')
                {
                    this.description += " (non-greedy)";
                    buffer.MoveNext();
                }
            }
            else
            {
                this.description = "missing '}' in quantifier";
            }

            buffer.AddLookup(this, startLoc, buffer.Offset - 1);
        }

        /// <summary>
        /// Toes the string.
        /// </summary>
        /// <param name="indent">The offset.</param>
        /// <returns>string</returns>
        public override string ToString(int indent)
        {
            return this.description;
        }
    }
}
