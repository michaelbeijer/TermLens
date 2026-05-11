using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Supervertaler.Trados.Controls
{
    /// <summary>
    /// Result of the <see cref="DistillChoiceDialog"/>.
    /// </summary>
    internal enum DistillChoice
    {
        Cancelled,
        DistillInbox,
        SelectFiles
    }

    /// <summary>
    /// Small modal dialog shown when the user clicks the Distill button.
    /// Offers two paths: distill all non-Markdown files sitting in the
    /// active memory bank's 00_INBOX, or open a file picker to choose
    /// files from disk.
    /// </summary>
    internal class DistillChoiceDialog : Form
    {
        /// <summary>The user's choice.</summary>
        public DistillChoice Choice { get; private set; } = DistillChoice.Cancelled;

        /// <param name="inboxFiles">
        /// Non-Markdown files currently in the inbox (top-level only,
        /// excluding <c>_archive/</c>). May be empty.
        /// </param>
        public DistillChoiceDialog(IReadOnlyList<string> inboxFiles)
        {
            Icon = Supervertaler.Trados.Core.IconHelper.AppIcon;
            // Let WinForms scale this dialog by system DPI so it doesn't squish
            // at >100% Windows display scaling. Cheap fallback; for surfaces
            // with their own UiScale-driven layout, set AutoScaleMode = None
            // instead and let UiScale own scaling.
            AutoScaleMode = AutoScaleMode.Dpi;
            Text = "Distill";
            Font = new Font("Segoe UI", 9f);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;

            bool hasInbox = inboxFiles != null && inboxFiles.Count > 0;
            int fileCount = hasInbox ? inboxFiles.Count : 0;

            // ── Description label ──────────────────────────────────
            var lblDesc = new Label
            {
                Text = "Choose where to distill files from:",
                Location = new Point(16, 16),
                AutoSize = true,
                ForeColor = Color.FromArgb(80, 80, 80)
            };
            Controls.Add(lblDesc);

            // ── Inbox button ───────────────────────────────────────
            string inboxLabel = hasInbox
                ? $"\u2697  Distill inbox ({fileCount} file{(fileCount == 1 ? "" : "s")})"
                : "\u2697  Distill inbox (empty)";

            var btnInbox = new Button
            {
                Text = inboxLabel,
                Location = new Point(16, 44),
                Width = 368,
                Height = 32,
                TextAlign = ContentAlignment.MiddleLeft,
                FlatStyle = FlatStyle.System,
                Enabled = hasInbox
            };
            btnInbox.Click += (s, e) =>
            {
                Choice = DistillChoice.DistillInbox;
                DialogResult = DialogResult.OK;
            };
            Controls.Add(btnInbox);

            // ── File list (if inbox has files) ─────────────────────
            int nextY = 80;
            if (hasInbox)
            {
                var names = inboxFiles.Select(Path.GetFileName).ToList();
                // Show up to 8 filenames; truncate if more
                var display = names.Count <= 8
                    ? string.Join("\n", names)
                    : string.Join("\n", names.Take(7)) + $"\n\u2026 and {names.Count - 7} more";

                var lblFiles = new Label
                {
                    Text = display,
                    Location = new Point(32, nextY),
                    AutoSize = true,
                    ForeColor = Color.FromArgb(120, 120, 120),
                    Font = new Font("Segoe UI", 8.25f)
                };
                Controls.Add(lblFiles);
                nextY = lblFiles.Bottom + 12;
            }

            // ── Select files button ────────────────────────────────
            var btnSelect = new Button
            {
                Text = "\U0001F4C2  Select files\u2026",
                Location = new Point(16, nextY),
                Width = 368,
                Height = 32,
                TextAlign = ContentAlignment.MiddleLeft,
                FlatStyle = FlatStyle.System
            };
            btnSelect.Click += (s, e) =>
            {
                Choice = DistillChoice.SelectFiles;
                DialogResult = DialogResult.OK;
            };
            Controls.Add(btnSelect);

            // ── Separator ──────────────────────────────────────────
            int sepY = nextY + 44;
            Controls.Add(new Label
            {
                Location = new Point(16, sepY),
                Width = 368,
                Height = 1,
                BorderStyle = BorderStyle.Fixed3D
            });

            // ── Cancel button ──────────────────────────────────────
            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(309, sepY + 10),
                Width = 75,
                FlatStyle = FlatStyle.System
            };
            Controls.Add(btnCancel);
            CancelButton = btnCancel;

            ClientSize = new Size(400, sepY + 48);
        }
    }
}
