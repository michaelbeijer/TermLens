using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Supervertaler.Trados.Settings;

namespace Supervertaler.Trados.Controls
{
    /// <summary>
    /// First-run setup dialog that lets the user choose the shared Supervertaler
    /// user-data folder.  Shown once when no %APPDATA%\Supervertaler\config.json
    /// exists yet.  On OK it calls UserDataPath.SetRoot() to persist the choice.
    /// </summary>
    internal class SetupDialog : Form
    {
        private Label  _headerLabel;
        private Label  _bodyLabel;
        private Label  _pathLabel;
        private TextBox _pathBox;
        private Button  _browseButton;
        private Label  _noteLabel;
        private Button  _okButton;
        private Button  _cancelButton;

        internal SetupDialog()
        {
            InitializeComponent();
            PopulateDefaults();
        }

        // ── Defaults ─────────────────────────────────────────────────

        private void PopulateDefaults()
        {
            var detected = UserDataPath.DetectWorkbenchRoot();
            if (!string.IsNullOrEmpty(detected))
            {
                _pathBox.Text = detected;
                _noteLabel.Text =
                    "An existing Supervertaler Workbench data folder was detected at the path above.\r\n" +
                    "Using it means prompts and termbases are shared between both products automatically.";
                _noteLabel.ForeColor = Color.FromArgb(0, 120, 60);
            }
            else
            {
                _pathBox.Text = UserDataPath.DefaultRoot;
                _noteLabel.Text =
                    "No existing Supervertaler Workbench installation was detected.\r\n" +
                    "The folder will be created automatically when Supervertaler for Trados first saves data.";
                _noteLabel.ForeColor = Color.FromArgb(80, 80, 80);
            }
        }

        // ── Event handlers ────────────────────────────────────────────

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Choose the Supervertaler user-data folder";
                dlg.SelectedPath = _pathBox.Text;
                dlg.ShowNewFolderButton = true;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                    _pathBox.Text = dlg.SelectedPath;
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            var path = _pathBox.Text.Trim();
            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show(this,
                    "Please enter or browse to a folder path.",
                    "Supervertaler Setup",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Attempt to create the folder now so we can validate it's writable
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    "Could not create the folder:\r\n" + ex.Message,
                    "Supervertaler Setup",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            UserDataPath.SetRoot(path);
            DialogResult = DialogResult.OK;
            Close();
        }

        // ── UI construction ───────────────────────────────────────────

        private void InitializeComponent()
        {
            SuspendLayout();

            // ── Form ──────────────────────────────────────────────────
            Text            = "Supervertaler for Trados — First-Run Setup";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition   = FormStartPosition.CenterScreen;
            ClientSize      = new Size(540, 290);
            MaximizeBox     = false;
            MinimizeBox     = false;
            ShowInTaskbar   = false;
            Font            = new Font("Segoe UI", 9f);

            // ── Header ────────────────────────────────────────────────
            _headerLabel = new Label
            {
                Text      = "Welcome to Supervertaler for Trados",
                Font      = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 120),
                Location  = new Point(16, 16),
                Size      = new Size(508, 24),
                AutoSize  = false
            };

            // ── Body ──────────────────────────────────────────────────
            _bodyLabel = new Label
            {
                Text =
                    "Choose a folder to store your data (prompts, settings, and licence information). " +
                    "Pointing this to your existing Supervertaler Workbench data folder lets both " +
                    "products share prompts and termbases automatically.",
                Location  = new Point(16, 48),
                Size      = new Size(508, 52),
                AutoSize  = false
            };

            // ── Path row ──────────────────────────────────────────────
            _pathLabel = new Label
            {
                Text     = "Data folder:",
                Location = new Point(16, 112),
                Size     = new Size(80, 22),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft
            };

            _pathBox = new TextBox
            {
                Location = new Point(100, 110),
                Size     = new Size(336, 22),
                Anchor   = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };

            _browseButton = new Button
            {
                Text     = "Browse…",
                Location = new Point(444, 109),
                Size     = new Size(80, 24)
            };
            _browseButton.Click += BrowseButton_Click;

            // ── Note ──────────────────────────────────────────────────
            _noteLabel = new Label
            {
                Location  = new Point(16, 146),
                Size      = new Size(508, 40),
                AutoSize  = false
            };

            // ── Separator ─────────────────────────────────────────────
            var separator = new Panel
            {
                Location  = new Point(0, 244),
                Size      = new Size(540, 1),
                BackColor = Color.FromArgb(200, 200, 200)
            };

            // ── Buttons ───────────────────────────────────────────────
            _okButton = new Button
            {
                Text         = "OK",
                DialogResult = DialogResult.None, // handled manually
                Location     = new Point(348, 254),
                Size         = new Size(80, 26)
            };
            _okButton.Click += OkButton_Click;
            AcceptButton = _okButton;

            _cancelButton = new Button
            {
                Text         = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location     = new Point(444, 254),
                Size         = new Size(80, 26)
            };
            CancelButton = _cancelButton;

            Controls.AddRange(new Control[]
            {
                _headerLabel, _bodyLabel,
                _pathLabel, _pathBox, _browseButton,
                _noteLabel,
                separator,
                _okButton, _cancelButton
            });

            ResumeLayout(false);
        }
    }
}
