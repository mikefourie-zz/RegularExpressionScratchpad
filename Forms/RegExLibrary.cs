namespace RegularExpressionScratchpad
{
    using System.IO;
    using System.Windows.Forms;
    using System.Xml.Serialization;

    /// <summary>
    /// Regexlibrary
    /// </summary>
    public partial class RegExLibrary : Form
    {
        private RegularExpressionLibrary library;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegExLibrary"/> class.
        /// </summary>
        public RegExLibrary()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Saves the library.
        /// </summary>
        private static void SaveRegularExpressionLibrary()
        {
        }

        /// <summary>
        /// Handles the Click event of the buttonSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void buttonSave_Click(object sender, System.EventArgs e)
        {
            SaveRegularExpressionLibrary();
        }

        /// <summary>
        /// Handles the Load event of the RegExLibrary control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void RegExLibrary_Load(object sender, System.EventArgs e)
        {
            this.LoadRegularExpressionLibrary();
        }

        /// <summary>
        /// Loads the regular expression library.
        /// </summary>
        private void LoadRegularExpressionLibrary()
        {
            this.GetRegularExpressionLibrary();

            foreach (Library lib in this.library.Library)
            {
                this.listBoxLibraryGroups.Items.Add(lib.libraryname);
            }
        }

        /// <summary>
        /// Gets the library.
        /// </summary>
        private void GetRegularExpressionLibrary()
        {
            // Construct an instance of the XmlSerializer with the type
            // of object that is being deserialized.
            XmlSerializer serializer = new XmlSerializer(typeof(RegularExpressionLibrary));

            // To read the file, create a FileStream.
            using (FileStream fileStream = new FileStream(Application.StartupPath + @"\RegExLibrary.xml", FileMode.Open))
            {
                // Call the Deserialize method and cast to the object type.
                this.library = (RegularExpressionLibrary)serializer.Deserialize(fileStream);
            }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the listBoxLibraryGroups control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void listBoxLibraryGroups_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.listBoxLibraryGroups.SelectedIndex >= 0)
            {
                this.ShowTasks();
            }
        }

        /// <summary>
        /// Shows the tasks.
        /// </summary>
        private void ShowTasks()
        {
            this.listBoxActions.Items.Clear();

            foreach (Library lib in this.library.Library)
            {
                if (lib.libraryname == this.listBoxLibraryGroups.SelectedItem.ToString())
                {
                    foreach (task t in lib.task)
                    {
                        this.listBoxActions.Items.Add(t.name);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the listBoxActions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void listBoxActions_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.listBoxActions.SelectedIndex >= 0)
            {
                this.ShowTaskDetails();
            }
        }

        /// <summary>
        /// Shows the task details.
        /// </summary>
        private void ShowTaskDetails()
        {
            this.textBoxPattern.Text = string.Empty;
            this.textBoxReplacement.Text = string.Empty;
            this.checkBoxRecursion.Checked = false;

            foreach (Library lib in this.library.Library)
            {
                if (lib.libraryname == this.listBoxLibraryGroups.SelectedItem.ToString())
                {
                    foreach (task t in lib.task)
                    {
                        if (t.name == this.listBoxActions.SelectedItem.ToString())
                        {
                            // this.textBoxPattern.Text = t
                        }
                    }
                }
            }
        }
    }
}