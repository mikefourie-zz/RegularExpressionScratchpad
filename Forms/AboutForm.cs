namespace RegularExpressionScratchpad
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Windows.Forms;

    /// <summary>
    /// AboutForm
    /// </summary>
    public partial class AboutForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AboutForm"/> class.
        /// </summary>
        public AboutForm()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the file version.
        /// </summary>
        /// <param name="asm">The asm.</param>
        /// <returns>version information</returns>
        private static Version GetFileVersion(Assembly asm)
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(asm.Location);
            Version ver = new Version(versionInfo.FileMajorPart, versionInfo.FileMinorPart, versionInfo.FileBuildPart, versionInfo.FilePrivatePart);
            return ver;
        }

        /// <summary>
        /// Handles the LinkClicked event of the linkLabel1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://mikefourie.wordpress.com/");
            this.Close();
        }

        /// <summary>
        /// Handles the Load event of the AboutForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AboutForm_Load(object sender, EventArgs e)
        {
            this.label1.Text = "Build: " + GetFileVersion(Assembly.GetExecutingAssembly());
        }
    }
}