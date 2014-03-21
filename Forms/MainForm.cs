namespace RegularExpressionScratchpad
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using System.Xml.Serialization;
    using RegularExpressionScratchpad.Properties;

    /// <summary>
    /// Main Form
    /// </summary>
    public partial class MainForm : Form
    {
        private const string BlogUrl = "http://www.freetodev.com";
        private RegexBuffer buffer;
        private int regexInsertionPoint = -1;
        private DirectoryInfo dir;
        private Regex parseRegEx;
        private RegularExpressionLibrary mylibrary;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Interprets the reg ex.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intended")]
        private void InterpretRegEx()
        {
            this.buffer = new RegexBuffer(this.textBoxRegex.Text) { RegexOptions = this.CreateRegexOptions() };
            try
            {
                RegexExpression exp = new RegexExpression(this.buffer);
                this.textBoxInterpretation.Text = exp.ToString(0);
                this.textBoxInterpretation.ForeColor = Color.Black;
            }
            catch (Exception ex)
            {
                this.textBoxInterpretation.Text = "We have a situation...\r\n\r\n (" + ex.Message + ")";
                this.textBoxRegex.Focus();
                this.textBoxInterpretation.ForeColor = Color.Gray;
            }
        }

        /// <summary>
        /// Creates the regex options.
        /// </summary>
        /// <returns>RegexOptions</returns>
        private RegexOptions CreateRegexOptions()
        {
            RegexOptions regOp = new RegexOptions();
            if (this.checkedListBoxOptions.CheckedItems.Contains("Ignore whitespace"))
            {
                regOp |= RegexOptions.IgnorePatternWhitespace;
            }

            if (this.checkedListBoxOptions.CheckedItems.Contains("Ignore case"))
            {
                regOp |= RegexOptions.IgnoreCase;
            }

            if (this.checkedListBoxOptions.CheckedItems.Contains("Explicit capture"))
            {
                regOp |= RegexOptions.ExplicitCapture;
            }

            if (this.checkedListBoxOptions.CheckedItems.Contains("Singleline"))
            {
                regOp |= RegexOptions.Singleline;
            }

            if (this.checkedListBoxOptions.CheckedItems.Contains("Multiline"))
            {
                regOp |= RegexOptions.Multiline;
            }

            if (this.checkedListBoxOptions.CheckedItems.Contains("Right to left"))
            {
                regOp |= RegexOptions.RightToLeft;
            }

            return regOp;
        }

        /// <summary>
        /// Replaces the text.
        /// </summary>
        private void ReplaceText()
        {
            try
            {
                Regex regex = this.CreateRegex();
                string[] strings;

                // if checked, pass all lines as a single block
                if (this.checkBoxTreatAsSingleString.Checked)
                {
                    strings = new string[1];
                    strings[0] = this.textBoxInput.Text;
                }
                else
                {
                    strings = Regex.Split(this.textBoxInput.Text, @"\r\n");

                    // strings = Strings.Text.Split('\n\r');
                }

                StringBuilder outString = new StringBuilder();
                string replace = this.textBoxReplacement.Text;
                foreach (string s in strings)
                {
                    outString.Append(regex.Replace(s, replace));
                }

                this.textBoxInput.SelectionColor = Color.Black;
                this.textBoxInput.SelectionFont = new Font("Arial", 10, FontStyle.Regular);
                this.textBoxInput.Text = outString.ToString();
            }
            catch (Exception ex)
            {
                this.ShowException(ex);
                return;
            }
        }

        /// <summary>
        /// Shows the exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        private void ShowException(Exception ex)
        {
            StringBuilder error = new StringBuilder();
            error.Append("An error has occured:\r\n");
            error.Append(ex.Message);
            this.textBoxInput.Text = error.ToString();
            this.textBoxInput.Find("An error has occured:", 0, RichTextBoxFinds.MatchCase);
            this.textBoxInput.SelectionFont = new Font("Arial", 10, FontStyle.Bold);
            this.textBoxInput.SelectionColor = Color.Red;
        }

        /// <summary>
        /// Creates the regex.
        /// </summary>
        /// <returns>regex</returns>
        private Regex CreateRegex()
        {
            RegexOptions regOp = this.CreateRegexOptions();
            return new Regex(this.textBoxRegex.Text, regOp);
        }

        /// <summary>
        /// Handles the TextChanged event of the textBoxRegex control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void TextBoxRegex_TextChanged(object sender, EventArgs e)
        {
            this.InterpretRegEx();
        }

        /// <summary>
        /// Handles the DragEnter event of the textBoxInput control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
        private void TextBoxInput_DragEnter(object sender, DragEventArgs e)
        {
            // If the data is a file or a bitmap, display the copy cursor.
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        /// <summary>
        /// Handles the dragdrop event of the textBoxInput control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
        private void TextBoxInput_dragdrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            try
            {
                using (StreamReader sr = new StreamReader(files[0]))
                {
                    this.textBoxInput.Text = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        /// <summary>
        /// Handles the Click event of the ToolStripButtonReplace control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ToolStripButtonReplace_Click(object sender, EventArgs e)
        {
            this.ReplaceText();
        }

        /// <summary>
        /// Inserts the text.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void InsertText(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;

            Regex regexBreak = new Regex(".+: (?<Placeholder>.+)");
            Match match = regexBreak.Match(menuItem.Text);
            if (match.Success)
            {
                string insert = match.Groups["Placeholder"].ToString();
                this.regexInsertionPoint = this.textBoxRegex.SelectionStart;
                string start = this.textBoxRegex.Text.Substring(0, this.regexInsertionPoint);
                string end = this.textBoxRegex.Text.Substring(this.regexInsertionPoint);

                this.textBoxRegex.Text = start + insert + end;

                Regex regexSelect = new Regex("(?<Select><[^<]+?>)");
                match = regexSelect.Match(insert);
                if (match.Success)
                {
                    Group g = match.Groups["Select"];
                    this.textBoxRegex.SelectionStart = this.regexInsertionPoint + g.Index;
                    this.textBoxRegex.SelectionLength = g.Length;
                }
                else
                {
                    this.textBoxRegex.SelectionStart = this.regexInsertionPoint;
                }

                this.textBoxRegex.Focus();
                this.textBoxRegex.Select(this.textBoxRegex.Text.Length, 0);
            }
        }

        /////// <summary>
        /////// Handles the KeyDown event of the TextBoxRegex control.
        /////// </summary>
        /////// <param name="sender">The source of the event.</param>
        /////// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        ////private void TextBoxRegex_KeyDown(object sender, KeyEventArgs e)
        ////{
        ////    if (e.KeyCode.ToString() == "Return")
        ////    {
        ////        this.ReplaceText();
        ////        this.InterpretRegEx();
        ////        e.SuppressKeyPress = true;
        ////    }
        ////}

        /// <summary>
        /// Handles the Load event of the MainForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Width = Convert.ToInt32(Settings.Default.WindowWidth);
            this.Height = Convert.ToInt32(Settings.Default.WindowHeight);
            this.LoadRegularExpressionLibrary();
            this.textBoxRegex.Focus();
        }

        /// <summary>
        /// Loads the regular expression library.
        /// </summary>
        private void LoadRegularExpressionLibrary()
        {
            if (File.Exists(Application.StartupPath + @"\RegExLibrary.xml") == false)
            {
                this.treeView1.Enabled = false;
                this.toolStripButtonLibrary.Enabled = false;
                this.labelLibrary.Text = "Task Library not Available";
                return;
            }

            try
            {
                // Construct an instance of the XmlSerializer with the type
                // of object that is being deserialized.
                XmlSerializer serializer = new XmlSerializer(typeof(RegularExpressionLibrary));

                // To read the file, create a FileStream.
                using (FileStream fileStream = new FileStream(Application.StartupPath + @"\RegExLibrary.xml", FileMode.Open))
                {
                    // Call the Deserialize method and cast to the object type.
                    this.mylibrary = (RegularExpressionLibrary)serializer.Deserialize(fileStream);
                }

                foreach (Library lib in this.mylibrary.Library)
                {
                    TreeNode libNode = new TreeNode(lib.libraryname);
                    foreach (task t in lib.task)
                    {
                        TreeNode taskNode = new TreeNode(t.name);
                        libNode.Nodes.Add(taskNode);
                    }

                    this.treeView1.Nodes.Add(libNode);
                }

                this.treeView1.Enabled = true;
                this.toolStripButtonLibrary.Enabled = true;
                this.labelLibrary.Text = "Task Library";
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was en error loading the Regular Expression Library: " + Environment.NewLine + ex.Message, "Library Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.treeView1.Enabled = false;
            }
        }

        /// <summary>
        /// Handles the TextChanged event of the textBoxInput control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void TextBoxInput_TextChanged(object sender, EventArgs e)
        {
            this.toolStripStatusLabel1.Text = this.textBoxInput.Text.Length + " Characters";
        }

        /// <summary>
        /// Handles the Click event of the toolStripButton2 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ToolStripButton2_Click(object sender, EventArgs e)
        {
            int characterCount = this.textBoxInput.Text.Length;
            int difference = 999;
            
            while (difference > 0)
            {
                this.DoWork();
                int currentcount = this.textBoxInput.Text.Length;
                difference = characterCount - currentcount;
                characterCount = currentcount;
            }
        }

        /// <summary>
        /// Does the work.
        /// </summary>
        private void DoWork()
        {
            int taskCount = 0;

            foreach (TreeNode libraryNode in this.treeView1.Nodes)
            {
                foreach (TreeNode taskNode in libraryNode.Nodes)
                {
                    if (taskNode.Checked)
                    {
                        taskCount++;
                    }
                }
            }

            if (taskCount > 0)
            {
                this.toolStripProgressBar1.Maximum = taskCount;
                this.toolStripProgressBar1.Visible = true;
                this.toolStripStatusLabelAction.Visible = true;
                foreach (TreeNode libraryNode in this.treeView1.Nodes)
                {
                    foreach (TreeNode taskNode in libraryNode.Nodes)
                    {
                        if (taskNode.Checked)
                        {
                            this.toolStripStatusLabelAction.Text = libraryNode.Text + " --- " + taskNode.Text;
                            this.Refresh();
                            this.ProcessLibraryTask(libraryNode.Text, taskNode.Text);
                            this.toolStripStatusLabel1.Text = this.textBoxInput.Text.Length + " Characters";
                            this.toolStripProgressBar1.Value++;
                            this.Refresh();
                        }
                    }
                }
            }

            this.toolStripProgressBar1.Value = 0;
            this.toolStripProgressBar1.Visible = false;
            this.toolStripStatusLabelAction.Text = string.Empty;
            this.toolStripStatusLabelAction.Visible = false;
        }

        /// <summary>
        /// Processes the library task.
        /// </summary>
        /// <param name="library">The library.</param>
        /// <param name="task">The task.</param>
        private void ProcessLibraryTask(string library, string task)
        {
            for (int i = 0; i < this.mylibrary.Library.Length; i++)
            {
                if (this.mylibrary.Library[i].libraryname == library)
                {
                    foreach (task t in this.mylibrary.Library[i].task)
                    {
                        if (t.name == task)
                        {
                            foreach (action a in t.action)
                            {
                                Regex regex = new Regex(a.pattern);
                                if (a.supportsRecursion)
                                {
                                    while (regex.Match(this.textBoxInput.Text).Success)
                                    {
                                        this.textBoxInput.Text = regex.Replace(this.textBoxInput.Text, a.replacement);
                                    }
                                }
                                else
                                {
                                    this.textBoxInput.Text = regex.Replace(this.textBoxInput.Text, a.replacement);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the toolStripStatusLabel2 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ToolStripStatusLabel2_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(BlogUrl);
            }
            catch (Exception ex)
            {
                this.ShowException(ex);
                return;
            }
        }

        /// <summary>
        /// Handles the Click event of the toolStripButton3 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ToolStripButton3_Click(object sender, EventArgs e)
        {
            // RegExLibrary library = new RegExLibrary();
            LibraryXml library = new LibraryXml();
            library.Show();
        }

        /// <summary>
        /// Handles the AfterCheck event of the treeView1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.TreeViewEventArgs"/> instance containing the event data.</param>
        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Nodes != null)
            {
                foreach (TreeNode childNode in e.Node.Nodes)
                {
                    childNode.Checked = e.Node.Checked;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the toolStripDropDownButton1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {
            using (AboutForm aboutForm = new AboutForm())
            {
                aboutForm.ShowDialog();
            }
        }

        /// <summary>
        /// Handles the Click event of the toolStripButtonReplaceInFiles control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void toolStripButtonReplaceInFiles_Click(object sender, EventArgs e)
        {
            try
            {
                this.folderBrowserDialog1.Description = "Select a path to Run the replacement on";
                DialogResult result = this.folderBrowserDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    this.dir = new DirectoryInfo(this.folderBrowserDialog1.SelectedPath);
                    this.ReplaceInFiles();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        /// <summary>
        /// Fixes this instance.
        /// </summary>
        private void ReplaceInFiles()
        {
            // Load the regex to use
            this.parseRegEx = this.CreateRegex();

            // Call the GetFileSystemInfos method.
            FileSystemInfo[] infos = this.dir.GetFileSystemInfos("*");

            // Iterate through each item.
            foreach (FileSystemInfo i in infos)
            {
                // Check to see if this is a FileInfo object.
                if (i is FileInfo)
                {
                    this.ParseAndReplaceFile(i);
                }
            }

            MessageBox.Show("Replacement Complete", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Parses the and replace file.
        /// </summary>
        /// <param name="parseFile">The parse file.</param>
        private void ParseAndReplaceFile(FileSystemInfo parseFile)
        {
            // Open the file and attempt to read the encoding from the BOM.
            string entireFile;
            using (StreamReader streamReader = new StreamReader(parseFile.FullName, true))
            {
                entireFile = streamReader.ReadToEnd();
            }

            // Parse the entire file.
            string newFile = this.parseRegEx.Replace(entireFile, this.textBoxReplacement.Text);

            // First make sure the file is writable.
            FileAttributes fileAttributes = File.GetAttributes(parseFile.FullName);

            // If readonly attribute is set, reset it.
            if ((fileAttributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                File.SetAttributes(parseFile.FullName, fileAttributes ^ FileAttributes.ReadOnly);
            }

            // Write out the new file.
            using (StreamWriter streamWriter = new StreamWriter(parseFile.FullName.Replace("dbo.p_Service", string.Empty).Replace(".StoredProcedure", string.Empty), false))
            {
                streamWriter.Write(newFile);
            }
        }

        /// <summary>
        /// Handles the ResizeEnd event of the MainForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            Settings.Default.WindowHeight = this.Height.ToString();
            Settings.Default.WindowWidth = this.Width.ToString();
            Settings.Default.Save();
        }

        /// <summary>
        /// Handles the Click event of the toolStripButton1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            MatcherForm matcher = new MatcherForm();
            matcher.Show();
        }

        /// <summary>
        /// Handles the Click event of the toolStripMatch control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void toolStripMatch_Click(object sender, EventArgs e)
        {
            this.Match();
        }

        /// <summary>
        /// Matches this instance.
        /// </summary>
        private void Match()
        {
            try
            {
                this.textBoxInput.SelectAll();
                this.textBoxInput.SelectionFont = new Font(this.textBoxInput.Font.Name, this.textBoxInput.Font.Size, FontStyle.Regular);
                this.textBoxInput.SelectionColor = Color.Black;
                this.textBoxInput.SelectionBackColor = Color.White;
                this.textBoxInput.Select(0, 0);

                Regex regex = this.CreateRegex();
                Match m = regex.Match(this.textBoxInput.Text);

                while (m.Success)
                {
                    Group g = m.Groups[0];
                    CaptureCollection cc = g.Captures;
                    for (int j = 0; j < cc.Count; j++)
                    {
                        Capture c = cc[j];
                        this.textBoxInput.Select(c.Index, c.Length);
                        this.textBoxInput.SelectionColor = Color.Red;
                        this.textBoxInput.SelectionBackColor = Color.Yellow;
                    }

                    m = m.NextMatch();
                }

                this.textBoxInput.ScrollToCaret();
            }
            catch (Exception ex)
            {
                this.ShowException(ex);
                return;
            }
        }

        /// <summary>
        /// Handles the KeyDown event of the textBoxRegex control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void textBoxRegex_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "Return")
            {
                this.Match();
                e.SuppressKeyPress = true;
            }
        }
    }
}