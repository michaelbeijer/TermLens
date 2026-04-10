using System;
using System.Drawing;
using System.Windows.Forms;

namespace Supervertaler.Trados.Controls
{
    /// <summary>
    /// Dialog for adding a terminology entry or raw note to SuperMemory.
    /// Captures a source term, a target term, optional notes, and
    /// optionally appends the entry to the active translation prompt.
    ///
    /// Two save modes:
    ///   • Structured article (default): writes a ready-to-use .md article
    ///     directly to 02_TERMINOLOGY/ — no AI processing needed.
    ///   • Raw note: writes an unstructured note to 00_INBOX/ for the AI
    ///     to compile into structured articles via Process Inbox. Useful
    ///     when the knowledge is fuzzy or context-dependent ("fiche can
    ///     mean either sheet or plug depending on context").
    /// </summary>
    internal class SuperMemoryQuickAddDialog : Form
    {
        private TextBox _txtTerm;
        private TextBox _txtCorrection;
        private TextBox _txtNotes;
        private CheckBox _chkAppendToPrompt;
        private CheckBox _chkRawNote;

        /// <summary>The source-language term.</summary>
        public string Term => _txtTerm?.Text?.Trim() ?? "";

        /// <summary>The target-language term or translation.</summary>
        public string Correction => _txtCorrection?.Text?.Trim() ?? "";

        /// <summary>Optional notes / context.</summary>
        public string Notes => _txtNotes?.Text?.Trim() ?? "";

        /// <summary>Whether to also append the entry to the active translation prompt.</summary>
        public bool AppendToPrompt => _chkAppendToPrompt?.Checked ?? true;

        /// <summary>
        /// When true, write the entry to 00_INBOX/ as a raw note for AI
        /// processing (via Process Inbox) rather than directly to
        /// 02_TERMINOLOGY/ as a structured article. Useful for knowledge
        /// that is ambiguous or context-dependent.
        /// </summary>
        public bool SaveAsRawNote => _chkRawNote?.Checked ?? false;

        /// <summary>
        /// Creates the Quick Add dialog.
        /// </summary>
        /// <param name="defaultTerm">Pre-filled source term (from selection).</param>
        /// <param name="defaultCorrection">Pre-filled correction (from target selection).</param>
        /// <param name="activePromptName">Display name of the active prompt (shown below checkbox).</param>
        /// <param name="targetLanguage">Display name of the target language (e.g. "English (GB)").</param>
        /// <param name="sourceLanguage">Display name of the source language (e.g. "Dutch (BE)").</param>
        public SuperMemoryQuickAddDialog(string defaultTerm = "", string defaultCorrection = "",
            string activePromptName = null, string targetLanguage = null, string sourceLanguage = null)
        {
            Text = "Quick Add to memory bank";
            Font = new Font("Segoe UI", 9f);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(460, 390);
            BackColor = Color.White;

            var y = 14;

            // ── Source term ──────────────────────────────────────────
            var sourceLabel = string.IsNullOrEmpty(sourceLanguage)
                ? "Source term:"
                : $"Source term ({sourceLanguage}):";
            Controls.Add(new Label
            {
                Text = sourceLabel,
                Location = new Point(16, y),
                AutoSize = true,
                ForeColor = Color.FromArgb(80, 80, 80)
            });
            y += 20;

            _txtTerm = new TextBox
            {
                Location = new Point(16, y),
                Width = ClientSize.Width - 32,
                Text = defaultTerm ?? "",
                BackColor = Color.FromArgb(250, 250, 250),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };
            Controls.Add(_txtTerm);
            y += 30;

            // ── Target term ─────────────────────────────────────────
            var targetLabel = string.IsNullOrEmpty(targetLanguage)
                ? "Target term:"
                : $"Target term ({targetLanguage}):";
            Controls.Add(new Label
            {
                Text = targetLabel,
                Location = new Point(16, y),
                AutoSize = true,
                ForeColor = Color.FromArgb(80, 80, 80)
            });
            y += 20;

            _txtCorrection = new TextBox
            {
                Location = new Point(16, y),
                Width = ClientSize.Width - 32,
                Text = defaultCorrection ?? "",
                BackColor = Color.FromArgb(250, 250, 250),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };
            Controls.Add(_txtCorrection);
            y += 30;

            // ── Notes (optional) ─────────────────────────────────────
            Controls.Add(new Label
            {
                Text = "Notes (optional \u2013 context, alternatives, client preferences):",
                Location = new Point(16, y),
                AutoSize = true,
                ForeColor = Color.FromArgb(80, 80, 80)
            });
            y += 20;

            _txtNotes = new TextBox
            {
                Location = new Point(16, y),
                Width = ClientSize.Width - 32,
                Height = 60,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.FromArgb(250, 250, 250)
            };
            Controls.Add(_txtNotes);
            y += 68;

            // ── Save as raw note toggle ──────────────────────────────
            _chkRawNote = new CheckBox
            {
                Text = "Save as raw note for AI processing (00_INBOX)",
                Location = new Point(14, y),
                AutoSize = true,
                Checked = false,
                ForeColor = Color.FromArgb(60, 60, 60)
            };
            Controls.Add(_chkRawNote);

            // Add a small hint label explaining the two modes
            var rawNoteHint = new Label
            {
                Text = "Unchecked = structured article in 02_TERMINOLOGY (instant).  " +
                       "Checked = raw note for Process Inbox to compile.",
                Location = new Point(32, y + 20),
                AutoSize = false,
                Width = ClientSize.Width - 48,
                Height = 28,
                ForeColor = Color.FromArgb(140, 140, 140),
                Font = new Font("Segoe UI", 7.5f)
            };
            Controls.Add(rawNoteHint);
            y += 52;

            // ── Append to prompt checkbox ────────────────────────────
            _chkAppendToPrompt = new CheckBox
            {
                Text = "Also append to active translation prompt",
                Location = new Point(14, y),
                AutoSize = true,
                Checked = true,
                ForeColor = Color.FromArgb(60, 60, 60)
            };
            Controls.Add(_chkAppendToPrompt);
            y += 22;

            // Show active prompt name (or warning if none)
            if (!string.IsNullOrEmpty(activePromptName))
            {
                Controls.Add(new Label
                {
                    Text = "\u2192 " + activePromptName,
                    Location = new Point(32, y),
                    AutoSize = true,
                    ForeColor = Color.FromArgb(0, 90, 158), // Supervertaler blue
                    Font = new Font("Segoe UI", 8.25f, FontStyle.Italic)
                });
            }
            else
            {
                Controls.Add(new Label
                {
                    Text = "\u26A0 No active prompt set for this project",
                    Location = new Point(32, y),
                    AutoSize = true,
                    ForeColor = Color.FromArgb(180, 120, 0),
                    Font = new Font("Segoe UI", 8.25f, FontStyle.Italic)
                });
                _chkAppendToPrompt.Checked = false;
                _chkAppendToPrompt.Enabled = false;
            }
            y += 22;

            // ── Separator ────────────────────────────────────────────
            Controls.Add(new Label
            {
                Location = new Point(16, y),
                Width = ClientSize.Width - 32,
                Height = 1,
                BorderStyle = BorderStyle.Fixed3D
            });
            y += 10;

            // ── Buttons ──────────────────────────────────────────────
            var btnOk = new Button
            {
                Text = "Add",
                DialogResult = DialogResult.OK,
                Location = new Point(ClientSize.Width - 170, y),
                Width = 75,
                FlatStyle = FlatStyle.System
            };
            Controls.Add(btnOk);
            AcceptButton = btnOk;

            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(ClientSize.Width - 88, y),
                Width = 75,
                FlatStyle = FlatStyle.System
            };
            Controls.Add(btnCancel);
            CancelButton = btnCancel;

            // Focus the target field if source is pre-filled, otherwise the source field
            Load += (s, e) =>
            {
                if (!string.IsNullOrEmpty(_txtTerm.Text) && string.IsNullOrEmpty(_txtCorrection.Text))
                    _txtCorrection.Focus();
                else
                    _txtTerm.Focus();
            };
        }
    }
}
