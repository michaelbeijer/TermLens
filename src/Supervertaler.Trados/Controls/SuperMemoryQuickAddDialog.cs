using System;
using System.Drawing;
using System.Windows.Forms;

namespace Supervertaler.Trados.Controls
{
    /// <summary>
    /// Dialog for adding a new terminology entry to SuperMemory.
    /// Captures a source term/pattern, a correction, optional notes,
    /// and optionally appends the entry to the active translation prompt.
    /// </summary>
    internal class SuperMemoryQuickAddDialog : Form
    {
        private TextBox _txtTerm;
        private TextBox _txtCorrection;
        private TextBox _txtNotes;
        private CheckBox _chkAppendToPrompt;

        /// <summary>The wrong term or Dunglish pattern.</summary>
        public string Term => _txtTerm?.Text?.Trim() ?? "";

        /// <summary>The correct English form.</summary>
        public string Correction => _txtCorrection?.Text?.Trim() ?? "";

        /// <summary>Optional notes / context.</summary>
        public string Notes => _txtNotes?.Text?.Trim() ?? "";

        /// <summary>Whether to also append the entry to the active translation prompt.</summary>
        public bool AppendToPrompt => _chkAppendToPrompt?.Checked ?? true;

        /// <summary>
        /// Creates the Quick Add dialog.
        /// </summary>
        /// <param name="defaultTerm">Pre-filled source term (from selection).</param>
        /// <param name="defaultCorrection">Pre-filled correction (from target selection).</param>
        /// <param name="activePromptName">Display name of the active prompt (shown below checkbox).</param>
        public SuperMemoryQuickAddDialog(string defaultTerm = "", string defaultCorrection = "",
            string activePromptName = null, string targetLanguage = null)
        {
            Text = "Quick Add to memory bank";
            Font = new Font("Segoe UI", 9f);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(460, 334);
            BackColor = Color.White;

            var y = 14;

            // ── Term (what's wrong) ──────────────────────────────
            Controls.Add(new Label
            {
                Text = "Term / pattern (what's wrong):",
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

            // ── Correction (what's right) ────────────────────────
            Controls.Add(new Label
            {
                Text = string.IsNullOrEmpty(targetLanguage)
                    ? "Correction (correct form):"
                    : $"Correction (correct {targetLanguage} form):",
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

            // ── Notes (optional) ─────────────────────────────────
            Controls.Add(new Label
            {
                Text = "Notes (optional context or explanation):",
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

            // ── Append to prompt checkbox ────────────────────────
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

            // ── Separator ────────────────────────────────────────
            Controls.Add(new Label
            {
                Location = new Point(16, y),
                Width = ClientSize.Width - 32,
                Height = 1,
                BorderStyle = BorderStyle.Fixed3D
            });
            y += 10;

            // ── Buttons ──────────────────────────────────────────
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

            // Focus the correction field if term is pre-filled, otherwise the term field
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
