namespace RegularExpressionScratchpad
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Text.RegularExpressions;

    /// <summary>
    /// RegexCharacter
    /// </summary>
    public class RegexCharacter : RegexItem
    {
        private static readonly Hashtable Escaped;
        private string character;
        private bool special;

        /// <summary>
        /// Initializes static members of the RegexCharacter class
        /// </summary>
        static RegexCharacter()
        {
            Escaped = new Hashtable
            {
                { 'a', @"A bell (alarm) \u0007 " },
                { 'b', @"Word boundary between //w and //W" },
                { 'B', @"Not at a word boundary between //w and //W" },
                { 't', @"A tab \u0009 " },
                { 'r', @"A carriage return \u000D " },
                { 'v', @"A vertical tab \u000B " },
                { 'f', @"A form feed \u000C " },
                { 'n', @"A new line \u000A " },
                { 'e', @"An escape \u001B " },
                { 'w', "Any word character " },
                { 'W', "Any non-word character " },
                { 's', "Any whitespace character " },
                { 'S', "Any non-whitespace character " },
                { 'd', "Any digit " },
                { 'D', "Any non-digit " },
                { 'A', "Anchor to start of string (ignore multiline)" },
                { 'Z', "Anchor to end of string or before \\n (ignore multiline)" },
                { 'z', "Anchor to end of string (ignore multiline)" }
            };

            // character escapes

            // character classes

            // anchors
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RegexCharacter"/> class.
        /// </summary>
        /// <param name="characters">The characters.</param>
        public RegexCharacter(string characters)
        {
            this.character = characters;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexCharacter"/> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public RegexCharacter(RegexBuffer buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer), "RegexBuffer is null");
            }

            int startLoc = buffer.Offset;
            bool quantifier = false;

            switch (buffer.Current)
            {
                case '.':
                    this.character = ". (any character)";
                    buffer.MoveNext();
                    this.special = true;
                    break;

                case '+':
                    this.character = "+ (one or more times)";
                    buffer.MoveNext();
                    this.special = true;
                    quantifier = true;
                    break;

                case '*':
                    this.character = "* (zero or more times)";
                    buffer.MoveNext();
                    this.special = true;
                    quantifier = true;
                    break;

                case '?':
                    this.character = "? (zero or one time)";
                    buffer.MoveNext();
                    this.special = true;
                    quantifier = true;
                    break;

                case '^':
                    this.character = "^ (anchor to start of string)";
                    buffer.MoveNext();
                    break;

                case '$':
                    this.character = "$ (anchor to end of string)";
                    buffer.MoveNext();
                    break;

                case ' ':
                    this.character = "' ' (space)";
                    buffer.MoveNext();
                    break;

                case '\\':
                    this.DecodeEscape(buffer);
                    break;

                default:
                    this.character = buffer.Current.ToString(CultureInfo.CurrentCulture);
                    buffer.MoveNext();
                    this.special = false;
                    break;
            }

            if (quantifier)
            {
                if (!buffer.AtEnd && buffer.Current == '?')
                {
                    this.character += " (non-greedy)";
                    buffer.MoveNext();
                }
            }

            buffer.AddLookup(this, startLoc, buffer.Offset - 1, this.character.Length == 1);
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="RegexCharacter"/> is special.
        /// </summary>
        /// <value><c>true</c> if special; otherwise, <c>false</c>.</value>
        public bool Special => this.special;

        /// <summary>
        /// Toes the string.
        /// </summary>
        /// <param name="indent">The indent.</param>
        /// <returns>string</returns>
        public override string ToString(int indent)
        {
            return this.character;
        }

        /// <summary>
        /// Decodes the escape.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        private void DecodeEscape(RegexBuffer buffer)
        {
            buffer.MoveNext();

            this.character = (string)Escaped[buffer.Current];
            if (this.character == null)
            {
                bool decoded = this.CheckBackReference(buffer);
                if (!decoded)
                {
                        // TODO: Handle other items below:
                    switch (buffer.Current)
                    {
                        case 'u':
                            buffer.MoveNext();
                            string unicode = buffer.String.Substring(0, 4);
                            this.character = "Unicode " + unicode;
                            buffer.Offset += 4;
                            break;

                        case ' ':
                            this.character = "' ' (space)";
                            this.special = false;
                            buffer.MoveNext();
                            break;

                        case 'c':
                            buffer.MoveNext();
                            this.character = "CTRL-" + buffer.Current;
                            buffer.MoveNext();
                            break;

                        case 'x':
                            buffer.MoveNext();
                            string number = buffer.String.Substring(0, 2);
                            this.character = "Hex " + number;
                            buffer.Offset += 2;
                            break;

                        default:
                            this.character = new string(buffer.Current, 1);
                            this.special = false;
                            buffer.MoveNext();
                            break;
                    }
                }
            }
            else
            {
                this.special = true;
                buffer.MoveNext();
            }
        }

        /// <summary>
        /// Checks the back reference.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>bool</returns>
        private bool CheckBackReference(RegexBuffer buffer)
        {
                // look for \k<name>
            Regex regex = new Regex(
                        @"
                        k\<(?<Name>.+?)\>
                        ",
                RegexOptions.IgnorePatternWhitespace);

            Match match = regex.Match(buffer.String);
            if (match.Success)
            {
                this.special = true;
                this.character = $"Backreference to match: {match.Groups["Name"]}";
                buffer.Offset += match.Groups[0].Length;
                return true;
            }

            return false;
        }
    }
}
