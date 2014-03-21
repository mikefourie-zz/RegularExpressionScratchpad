namespace RegularExpressionScratchpad
{
    using System;
    using System.Collections;
    using System.Text.RegularExpressions;

    /// <summary>
    /// RegexCapture.
    /// </summary>
    public class RegexCapture : RegexItem
    {
        private static readonly Hashtable OptionNames = new Hashtable();
        private readonly int startLocation;
        private RegexItem expression;
        private string description = "Capture";       

        /// <summary>
        /// Initializes static members of the RegexCapture class
        /// </summary>
        static RegexCapture()
        {
            OptionNames.Add("i", "Ignore Case");
            OptionNames.Add("-i", "Ignore Case Off");
            OptionNames.Add("m", "Multiline");
            OptionNames.Add("-m", "Multiline Off");
            OptionNames.Add("n", "Explicit Capture");
            OptionNames.Add("-n", "Explicit Capture Off");
            OptionNames.Add("s", "Singleline");
            OptionNames.Add("-s", "Singleline Off");
            OptionNames.Add("x", "Ignore Whitespace");
            OptionNames.Add("-x", "Ignore Whitespace Off");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexCapture"/> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public RegexCapture(RegexBuffer buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer", "RegexBuffer is null");
            }

            this.startLocation = buffer.Offset;
            buffer.MoveNext();

                // we're not in a series of normal characters, so clear
            buffer.Clearinseries();

                // if the first character of the capture is a '?',
                // we need to decode what comes after it.
            if (buffer.Current == '?')
            {
                bool decoded = this.CheckNamed(buffer);

                if (!decoded)
                {
                    decoded = this.CheckBalancedGroup(buffer);
                }

                if (!decoded)
                {
                    decoded = this.CheckNonCapturing(buffer);
                }

                if (!decoded)
                {
                    decoded = this.CheckOptions(buffer);
                }

                if (!decoded)
                {
                    decoded = this.CheckLookahead(buffer);
                }

                if (!decoded)
                {
                    decoded = this.CheckNonBacktracking(buffer);
                }
            
                if (!decoded)
                {
                    this.CheckConditional(buffer);
                }
            }
            else
            {
                // plain old capture...
                if (!this.HandlePlainOldCapture(buffer))
                {
                    throw new Exception(string.Format("Unrecognized capture: {0}", buffer.String));
                }
            }

            buffer.AddLookup(this, this.startLocation, buffer.Offset - 1);
        }

        /// <summary>
        /// Toes the string.
        /// </summary>
        /// <param name="indent">The offset.</param>
        /// <returns>string </returns>
        public override string ToString(int indent)
        {
            checked
            {
                string result = this.description;
                if (this.expression != null)
                {
                    result += "\r\n" + this.expression.ToString(indent + 2) +
                              new string(' ', indent) + "End Capture";
                }

                return result;
            }
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
                throw new Exception(
                    string.Format("Missing closing ')' in capture"), e);
            }

            if (current != ')')
            {
                throw new Exception(string.Format("Unterminated closure at offset {0}", buffer.Offset));
            }

            buffer.Offset++; // eat closing parenthesis
        }

        /// <summary>
        /// Handles the plain old capture.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>bool</returns>
        private bool HandlePlainOldCapture(RegexBuffer buffer)
        {
                // we're already at the expression. Just create a new
                // expression, and make sure that we're at a ")" when 
                // we're done
            if (buffer.ExplicitCapture)
            {
                this.description = string.Format("Non-capturing Group");
            }

            this.expression = new RegexExpression(buffer);
            this.CheckClosingParen(buffer);
            return true;
        }

        /// <summary>
        /// Checks the named.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>bool</returns>
        private bool CheckNamed(RegexBuffer buffer)
        {
            // look for ?<Name> or ?'Name' syntax...
            Regex regex = new Regex(
                @"
                        ^                         # anchor to start of string
                        \?(\<|')                  # ?< or ?'
                        (?<Name>[a-zA-Z0-9]+?)    # Capture name
                        (\>|')                    # ?> or ?'
                        (?<Rest>.+)               # The rest of the string
                        ",
                RegexOptions.IgnorePatternWhitespace);

            Match match = regex.Match(buffer.String);
            if (match.Success)
            {
                this.description = string.Format("Capture to <{0}>", match.Groups["Name"]);
                
                    // advance buffer to the rest of the expression
                buffer.Offset += match.Groups["Rest"].Index;
                this.expression = new RegexExpression(buffer);

                this.CheckClosingParen(buffer);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks the non capturing.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>bool</returns>
        private bool CheckNonCapturing(RegexBuffer buffer)
        {
            // Look for non-capturing ?:           
            Regex regex = new Regex(
                        @"
                        ^                         # anchor to start of string
                        \?:
                        (?<Rest>.+)             # The rest of the expression
                        ",
                RegexOptions.IgnorePatternWhitespace);
            Match match = regex.Match(buffer.String);
            if (match.Success)
            {
                this.description = string.Format("Non-capturing Group");

                buffer.Offset += match.Groups["Rest"].Index;
                this.expression = new RegexExpression(buffer);

                this.CheckClosingParen(buffer);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks the balanced group.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>bool</returns>
        private bool CheckBalancedGroup(RegexBuffer buffer)
        {
            // look for ?<Name1-Name2> or ?'Name1-Name2' syntax...
            // look for ?<Name> or ?'Name' syntax...
            Regex regex = new Regex(
                        @"
                        ^                         # anchor to start of string
                        \?[\<|']                  # ?< or ?'
                        (?<Name1>[a-zA-Z]+?)       # Capture name1
                        -
                        (?<Name2>[a-zA-Z]+?)       # Capture name2
                        [\>|']                    # ?> or ?'
                        (?<Rest>.+)               # The rest of the expression
                        ",
                RegexOptions.IgnorePatternWhitespace);

            Match match = regex.Match(buffer.String);
            if (match.Success)
            {
                this.description = string.Format("Balancing Group <{0}>-<{1}>", match.Groups["Name1"], match.Groups["Name2"]);
                buffer.Offset += match.Groups["Rest"].Index;
                this.expression = new RegexExpression(buffer);
                this.CheckClosingParen(buffer);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks the options.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>bool</returns>
        private bool CheckOptions(RegexBuffer buffer)
        {
            // look for ?imnsx-imnsx:
            Regex regex = new Regex(
                        @"
                        ^                         # anchor to start of string
                        \?(?<Options>[imnsx-]+):
                        ",
                RegexOptions.IgnorePatternWhitespace);

            Match match = regex.Match(buffer.String);
            if (match.Success)
            {
                string option = match.Groups["Options"].Value;
                this.description = string.Format("Set options to {0}", OptionNames[option]);
                this.expression = null;
                buffer.Offset += match.Groups[0].Length;
                return true;
            }

            return false;
        }

        private bool CheckLookahead(RegexBuffer buffer)
        {
            Regex regex = new Regex(
                        @"
                        ^                         # anchor to start of string
                        \?
                        (?<Assertion><=|<!|=|!)   # assertion char
                        (?<Rest>.+)               # The rest of the expression
                        ",
                RegexOptions.IgnorePatternWhitespace);

            Match match = regex.Match(buffer.String);
            if (match.Success)
            {
                switch (match.Groups["Assertion"].Value)
                {
                    case "=":
                        this.description = "zero-width positive lookahead";
                        break;

                    case "!":
                        this.description = "zero-width negative lookahead";
                        break;

                    case "<=":
                        this.description = "zero-width positive lookbehind";
                        break;

                    case "<!":
                        this.description = "zero-width negative lookbehind";
                        break;
                }

                buffer.Offset += match.Groups["Rest"].Index;
                this.expression = new RegexExpression(buffer);
                this.CheckClosingParen(buffer);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks the non backtracking.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>bool</returns>
        private bool CheckNonBacktracking(RegexBuffer buffer)
        {
            // Look for non-backtracking sub-expression ?>
            Regex regex = new Regex(
                        @"
                        ^                         # anchor to start of string
                        \?\>
                        (?<Rest>.+)             # The rest of the expression
                        ",
                RegexOptions.IgnorePatternWhitespace);
            Match match = regex.Match(buffer.String);
            if (match.Success)
            {
                this.description = string.Format("Non-backtracking subexpression");

                buffer.Offset += match.Groups["Rest"].Index;
                this.expression = new RegexExpression(buffer);

                this.CheckClosingParen(buffer);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks the conditional.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>bool</returns>
        private bool CheckConditional(RegexBuffer buffer)
        {
            // Look for conditional (?(name)yesmatch|nomatch)
            // (name can also be an expression)
            Regex regex = new Regex(
                        @"
                        ^                         # anchor to start of string
                        \?\(
                        (?<Rest>.+)             # The rest of the expression
                        ",
                RegexOptions.IgnorePatternWhitespace);
            Match match = regex.Match(buffer.String);
            if (match.Success)
            {
                this.description = string.Format("Conditional Subexpression");

                buffer.Offset += match.Groups["Rest"].Index;
                this.expression = new RegexConditional(buffer);
                return true;
            }

            return false;
        }
    }
}
