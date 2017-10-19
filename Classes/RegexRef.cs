namespace RegularExpressionScratchpad
{
    using System;

    /// <summary>
    /// RegexRef.
    /// </summary>
    public class RegexRef : IComparable
    {
        private readonly int start;
        private int end;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexRef"/> class.
        /// </summary>
        /// <param name="regexItem">The regex item.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public RegexRef(RegexItem regexItem, int start, int end)
        {
            if (regexItem == null)
            {
                throw new ArgumentNullException(nameof(regexItem), "RegexItem is null");
            }

            this.StringValue = regexItem.ToString(0);
            this.start = start;
            this.end = end;
        }

        /// <summary>
        /// Gets or sets the string value.
        /// </summary>
        /// <value>The string value.</value>
        public string StringValue { get; set; }

        /// <summary>
        /// Gets the start.
        /// </summary>
        /// <value>The start.</value>
        public int Start => this.start;

        /// <summary>
        /// Gets the end.
        /// </summary>
        /// <value>The end.</value>
        public int End => this.end;

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public int Length
        {
            get => this.end - this.start + 1;

            set => this.end = this.start + value - 1;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than obj. Zero This instance is equal to obj. Greater than zero This instance is greater than obj.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">obj is not the same type as this instance. </exception>
        public int CompareTo(object obj)
        {
            RegexRef ref2 = (RegexRef)obj;
            if (this.Length < ref2.Length)
            {
                return -1;
            }

            return this.Length > ref2.Length ? 1 : 0;
        }

        /// <summary>
        /// Ins the range.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>bool</returns>
        public bool InRange(int location)
        {
            return (location >= this.start) && (location <= this.end);
        }
    }
}
