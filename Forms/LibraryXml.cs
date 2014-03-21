namespace RegularExpressionScratchpad
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    /// <summary>
    /// LibraryXml
    /// </summary>
    public partial class LibraryXml : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LibraryXml"/> class.
        /// </summary>
        public LibraryXml()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles the Load event of the LibraryXml control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void LibraryXml_Load(object sender, EventArgs e)
        {
            this.LoadRegularExpressionLibrary();
        }

        /// <summary>
        /// Loads the regular expression library.
        /// </summary>
        private void LoadRegularExpressionLibrary()
        {
            this.GetRegularExpressionLibrary();
        }

        /// <summary>
        /// Gets the library.
        /// </summary>
        private void GetRegularExpressionLibrary()
        {
            // Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            using (StreamReader sr = new StreamReader(Application.StartupPath + @"\RegExLibrary.xml"))
            {
                this.textBox1.Text = sr.ReadToEnd();
            }
        }
    }
}