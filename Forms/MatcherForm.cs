namespace RegularExpressionScratchpad
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// MatcherForm
    /// </summary>
    public partial class MatcherForm : Form
    {
        private int searchStartPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="MatcherForm"/> class.
        /// </summary>
        public MatcherForm()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the richTextBoxSource control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void richTextBoxSource_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.MatchText(this.richTextBoxSource.SelectedText);
            this.richTextBoxTarget.ScrollToCaret();
        }

        /// <summary>
        /// Handles the TextChanged event of the toolStripTextBox1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            this.searchStartPosition = 0;
            this.MatchTextSource(this.toolStripTextBox1.Text, 0);
            this.richTextBoxSource.ScrollToCaret();
        }

        /// <summary>
        /// Handles the SelectionChanged event of the richTextBoxSource control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void richTextBoxSource_SelectionChanged(object sender, EventArgs e)
        {
            this.MatchText(this.richTextBoxSource.SelectedText);
            this.richTextBoxTarget.ScrollToCaret();
        }

        /// <summary>
        /// Matches the text.
        /// </summary>
        /// <param name="text">The text.</param>
        private void MatchText(string text)
        {
            int position = 0;
            if (text.Length > 3 && this.richTextBoxTarget.Text.IndexOf(text, position, StringComparison.InvariantCultureIgnoreCase) > 0)
            {
                while (this.richTextBoxTarget.Text.IndexOf(text, position, StringComparison.InvariantCultureIgnoreCase) > 0)
                {
                    this.richTextBoxTarget.Select(this.richTextBoxTarget.Text.IndexOf(text, position, StringComparison.InvariantCultureIgnoreCase), text.Length);
                    this.richTextBoxTarget.SelectionColor = Color.Red;
                    this.richTextBoxTarget.SelectionBackColor = Color.Yellow;
                    this.richTextBoxTarget.SelectionFont = new Font("Arial", this.richTextBoxTarget.Font.Size, FontStyle.Bold);
                    position = this.richTextBoxTarget.Text.IndexOf(text, position, StringComparison.InvariantCultureIgnoreCase) + 1;
                }
            }
        }

        /// <summary>
        /// Matches the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="start">The start.</param>
        private void MatchTextSource(string text, int start)
        {
            if (text.Length > 3 && this.richTextBoxSource.Text.IndexOf(text, start, StringComparison.InvariantCultureIgnoreCase) > 0)
            {
                this.searchStartPosition = this.richTextBoxSource.Text.IndexOf(text, start, StringComparison.InvariantCultureIgnoreCase);
                this.richTextBoxSource.Select(this.richTextBoxTarget.Text.IndexOf(text, start, StringComparison.InvariantCultureIgnoreCase), text.Length);
                this.richTextBoxSource.SelectionColor = Color.Red;
                this.richTextBoxSource.SelectionBackColor = Color.Yellow;
                this.richTextBoxSource.SelectionFont = new Font("Arial", this.richTextBoxTarget.Font.Size, FontStyle.Bold);
            }
        }

        /// <summary>
        /// Handles the Click event of the toolStripButtonResetText control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void toolStripButtonResetText_Click(object sender, EventArgs e)
        {
            this.richTextBoxSource.SelectAll();
            this.richTextBoxSource.SelectionFont = new Font(this.richTextBoxSource.Font.Name, this.richTextBoxSource.Font.Size, FontStyle.Regular);
            this.richTextBoxSource.SelectionColor = Color.Black;
            this.richTextBoxSource.SelectionBackColor = Color.White;
            this.richTextBoxSource.Select(0, 0);

            this.richTextBoxTarget.SelectAll();
            this.richTextBoxTarget.SelectionFont = new Font(this.richTextBoxTarget.Font.Name, this.richTextBoxTarget.Font.Size, FontStyle.Regular);
            this.richTextBoxTarget.SelectionColor = Color.Black;
            this.richTextBoxTarget.SelectionBackColor = Color.White;
            this.richTextBoxTarget.Select(0, 0);
        }

        /// <summary>
        /// Handles the Click event of the toolStripButtonNext control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void toolStripButtonNext_Click(object sender, EventArgs e)
        {
            this.MatchTextSource(this.toolStripTextBox1.Text, this.searchStartPosition + 1);
            this.richTextBoxSource.ScrollToCaret();
        }
    }
}
