namespace RegularExpressionScratchpad
{
    using System;
    using System.Collections;
    using System.Text.RegularExpressions;

    /// <summary>
    /// String with a pointer in it.
    /// </summary>
    public class RegexBuffer
    {
        private readonly ArrayList expressionLookup = new ArrayList();
        private readonly string expression;
        private int offset;
        private int errorLocation = -1;
        private int errorLength = -1;
        private RegexOptions regexOptions;
        private bool inseries;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexBuffer"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public RegexBuffer(string expression)
        {
            this.expression = expression;
        }

        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <value>The current.</value>
        public char Current
        {
            get
            {
                if (this.offset >= this.expression.Length)
                {
                    throw new Exception("Interpretation not possible");
                }

                return this.expression[this.offset];
            }
        }

        /// <summary>
        /// Gets a value indicating whether [at end].
        /// </summary>
        /// <value><c>true</c> if [at end]; otherwise, <c>false</c>.</value>
        public bool AtEnd
        {
            get
            {
                return this.offset >= this.expression.Length;
            }
        }

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        /// <value>The offset.</value>
        public int Offset
        {
            get
            {
                return this.offset;
            }

            set
            {
                this.offset = value;
            }
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <value>The string.</value>
        public string String
        {
            get
            {
                return this.expression.Substring(this.offset);
            }
        }

        /// <summary>
        /// Gets or sets the error location.
        /// </summary>
        /// <value>The error location.</value>
        public int ErrorLocation
        {
            get
            {
                return this.errorLocation;
            }

            set
            {
                this.errorLocation = value;
            }
        }

        /// <summary>
        /// Gets or sets the length of the error.
        /// </summary>
        /// <value>The length of the error.</value>
        public int ErrorLength
        {
            get
            {
                return this.errorLength;
            }

            set
            {
                this.errorLength = value;
            }
        }

        /// <summary>
        /// Gets or sets the regex options.
        /// </summary>
        /// <value>The regex options.</value>
        public RegexOptions RegexOptions
        {
            get
            {
                return this.regexOptions;
            }

            set
            {
                this.regexOptions = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [ignore pattern whitespace].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [ignore pattern whitespace]; otherwise, <c>false</c>.
        /// </value>
        public bool IgnorePatternWhitespace
        {
            get
            {
                return (this.regexOptions & RegexOptions.IgnorePatternWhitespace) != 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [explicit capture].
        /// </summary>
        /// <value><c>true</c> if [explicit capture]; otherwise, <c>false</c>.</value>
        public bool ExplicitCapture
        {
            get
            {
                return (this.regexOptions & RegexOptions.ExplicitCapture) != 0;
            }
        }

        /// <summary>
        /// Substrings the specified start.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>RegexBuffer</returns>
        public RegexBuffer Substring(int start, int end)
        {
            return new RegexBuffer(this.expression.Substring(start, end - start + 1));
        }

        /// <summary>
        /// Moves the next.
        /// </summary>
        public void MoveNext()
        {
            this.offset++;
        }

        /// <summary>
        /// Adds the lookup.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="startLocation">The start location.</param>
        /// <param name="endLocation">The end location.</param>
        public void AddLookup(RegexItem item, int startLocation, int endLocation)
        {
            this.AddLookup(item, startLocation, endLocation, false);
        }

        /// <summary>
        /// Clears the in series.
        /// </summary>
        public void Clearinseries()
        {
            this.inseries = false;
        }

        /// <summary>
        /// Adds the lookup.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="startLocation">The start location.</param>
        /// <param name="endLocation">The end location.</param>
        /// <param name="canCoalesce">if set to <c>true</c> [can coalesce].</param>
        public void AddLookup(RegexItem item, int startLocation, int endLocation, bool canCoalesce)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item", "RegexItem is null");
            }

            if (this.inseries)
            {
                    // in a series, add character to the previous one...
                if (canCoalesce)
                {
                    RegexRef lastItem = (RegexRef)this.expressionLookup[this.expressionLookup.Count - 1];
                    lastItem.StringValue += item.ToString(0);
                    lastItem.Length += endLocation - startLocation + 1;
                }
                else
                {
                    this.expressionLookup.Add(new RegexRef(item, startLocation, endLocation));
                    this.inseries = false;
                }
            }
            else
            {
                if (canCoalesce)
                {
                    this.inseries = true;
                }

                this.expressionLookup.Add(new RegexRef(item, startLocation, endLocation));
            }
        }

        /// <summary>
        /// Matches the locations.
        /// </summary>
        /// <param name="spot">The spot.</param>
        /// <returns> RegexRef </returns>
        public RegexRef MatchLocations(int spot)
        {
            ArrayList locations = new ArrayList();
            foreach (RegexRef regexRef in this.expressionLookup)
            {
                if (regexRef.InRange(spot))
                {
                    locations.Add(regexRef);
                }
            }

            locations.Sort();
            if (locations.Count != 0)
            {
                return (RegexRef)locations[0];
            }

            return null;
        }
    }
}
