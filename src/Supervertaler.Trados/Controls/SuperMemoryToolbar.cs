using System;
using System.Drawing;
using System.Windows.Forms;
using Supervertaler.Trados.Core;

namespace Supervertaler.Trados.Controls
{
    /// <summary>
    /// Thin toolbar strip for SuperMemory operations.
    /// Sits below the context strip in the Chat tab.
    /// Two buttons: Process Inbox and Health Check, plus an inbox count label.
    /// </summary>
    public class SuperMemoryToolbar : Panel
    {
        private Label _lblHeading;
        private ComboBox _cmbMemoryBank;
        private LinkLabel _lnkHelp;
        private Button _btnProcessInbox;
        private Button _btnHealthCheck;
        private Button _btnDistill;
        private Button _btnRefresh;
        private Label _lblInboxCount;

        /// <summary>
        /// Suppresses <see cref="MemoryBankChanged"/> while the dropdown is being
        /// populated programmatically. Mirrors the <c>_suppress_combo_change</c>
        /// flag in the Python Supervertaler Assistant.
        /// </summary>
        private bool _suppressComboChange;

        /// <summary>Raised when the user clicks "Process Inbox".</summary>
        public event EventHandler ProcessInboxRequested;

        /// <summary>Raised when the user clicks "Health Check".</summary>
        public event EventHandler HealthCheckRequested;

        /// <summary>Raised when the user clicks "Distill".</summary>
        public event EventHandler DistillRequested;

        /// <summary>Raised when the user clicks the refresh button.</summary>
        public event EventHandler RefreshRequested;

        /// <summary>
        /// Raised when the user picks a different memory bank from the dropdown.
        /// Suppressed while <see cref="SetMemoryBanks"/> is repopulating the list.
        /// </summary>
        public event EventHandler<MemoryBankChangedEventArgs> MemoryBankChanged;

        public SuperMemoryToolbar()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            Height = UiScale.Pixels(32);
            Dock = DockStyle.Top;
            BackColor = Color.FromArgb(245, 248, 252); // light blue-gray tint
            Padding = new Padding(UiScale.Pixels(6), UiScale.Pixels(3), UiScale.Pixels(6), UiScale.Pixels(3));

            var btnFont = new Font("Segoe UI", UiScale.FontSize(7.5f));
            var labelFont = new Font("Segoe UI", UiScale.FontSize(7f));

            // ─── Heading label ───────────────────────────────────────
            _lblHeading = new Label
            {
                Text = "Memory Bank",
                Font = new Font("Segoe UI Semibold", UiScale.FontSize(7f)),
                ForeColor = Color.FromArgb(90, 90, 90),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // ─── Memory bank dropdown ───────────────────────────────
            // Populated by the parent view part via SetMemoryBanks().
            // Switching is immediate: the next chat turn reads from the
            // new bank, chat history is preserved.
            _cmbMemoryBank = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", UiScale.FontSize(8f)),
                FlatStyle = FlatStyle.Flat,
                Width = UiScale.Pixels(180),
                Height = UiScale.Pixels(22),
                TabStop = false
            };
            _cmbMemoryBank.SelectedIndexChanged += OnMemoryBankComboChanged;

            // ─── Help link ──────────────────────────────────────────
            _lnkHelp = new LinkLabel
            {
                Text = "?",
                Font = new Font("Segoe UI", UiScale.FontSize(7f)),
                AutoSize = true,
                LinkColor = Color.FromArgb(100, 140, 180),
                ActiveLinkColor = Color.FromArgb(30, 90, 158),
                VisitedLinkColor = Color.FromArgb(100, 140, 180),
                TabStop = false
            };
            _lnkHelp.LinkClicked += (s, e) =>
                HelpSystem.OpenHelp(HelpSystem.Topics.SuperMemory);

            // ─── Process Inbox button ────────────────────────────────
            _btnProcessInbox = new Button
            {
                Text = "\u2B07 Process Inbox", // ⬇ down arrow
                Font = btnFont,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(30, 90, 158),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                AutoSize = true,
                Padding = new Padding(UiScale.Pixels(4), 0, UiScale.Pixels(4), 0),
                Height = UiScale.Pixels(24),
                TabStop = false,
                UseCompatibleTextRendering = true
            };
            _btnProcessInbox.FlatAppearance.BorderSize = 0;
            _btnProcessInbox.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 232, 245);
            _btnProcessInbox.Click += (s, e) => ProcessInboxRequested?.Invoke(this, EventArgs.Empty);

            // Tooltip explaining what this does
            var tip = new ToolTip { AutoPopDelay = 8000 };
            tip.SetToolTip(_cmbMemoryBank,
                "Active memory bank.\n" +
                "Switching is immediate - the next chat turn reads from\n" +
                "the new bank; chat history is preserved.");
            tip.SetToolTip(_btnProcessInbox,
                "Reads new files from your SuperMemory inbox and uses AI\n" +
                "to organise them into structured knowledge base articles\n" +
                "(client profiles, terminology, domain knowledge, style guides).");

            // ─── Health Check button ─────────────────────────────────
            _btnHealthCheck = new Button
            {
                Text = "\u2714 Health Check", // ✔ check mark
                Font = btnFont,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(30, 90, 158),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                AutoSize = true,
                Padding = new Padding(UiScale.Pixels(4), 0, UiScale.Pixels(4), 0),
                Height = UiScale.Pixels(24),
                TabStop = false,
                UseCompatibleTextRendering = true
            };
            _btnHealthCheck.FlatAppearance.BorderSize = 0;
            _btnHealthCheck.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 232, 245);
            _btnHealthCheck.Click += (s, e) => HealthCheckRequested?.Invoke(this, EventArgs.Empty);

            tip.SetToolTip(_btnHealthCheck,
                "Scans your SuperMemory knowledge base for problems:\n" +
                "conflicting terminology, broken links, stale or duplicate\n" +
                "content. Fixes what it can and flags the rest for review.");

            // ─── Distill button ─────────────────────────────────────
            _btnDistill = new Button
            {
                Text = "\u2697 Distill", // ⚗ alembic
                Font = btnFont,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(30, 90, 158),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                AutoSize = true,
                Padding = new Padding(UiScale.Pixels(4), 0, UiScale.Pixels(4), 0),
                Height = UiScale.Pixels(24),
                TabStop = false,
                UseCompatibleTextRendering = true
            };
            _btnDistill.FlatAppearance.BorderSize = 0;
            _btnDistill.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 232, 245);
            _btnDistill.Click += (s, e) => DistillRequested?.Invoke(this, EventArgs.Empty);

            tip.SetToolTip(_btnDistill,
                "Extract knowledge from translation files (TMX, DOCX, PDF,\n" +
                "termbases) into SuperMemory knowledge base articles.");

            // ─── Inbox count label ───────────────────────────────────
            _lblInboxCount = new Label
            {
                Text = "",
                Font = labelFont,
                ForeColor = Color.FromArgb(140, 140, 140),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(UiScale.Pixels(4), 0, 0, 0)
            };

            // ─── Refresh button ─────────────────────────────────────
            _btnRefresh = new Button
            {
                Text = "\u21BB", // ↻ clockwise arrow
                Font = new Font("Segoe UI", UiScale.FontSize(8.5f)),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(140, 140, 140),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Size = new Size(UiScale.Pixels(24), UiScale.Pixels(24)),
                TabStop = false,
                UseCompatibleTextRendering = true
            };
            _btnRefresh.FlatAppearance.BorderSize = 0;
            _btnRefresh.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 232, 245);
            _btnRefresh.Click += (s, e) => RefreshRequested?.Invoke(this, EventArgs.Empty);

            tip.SetToolTip(_btnRefresh,
                "Refresh the inbox count.\nUse this after adding files via the\nObsidian Web Clipper or file explorer.");

            // Separator line at bottom
            var sep = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 1,
                BackColor = Color.FromArgb(220, 220, 220)
            };

            Controls.Add(sep);
            Controls.Add(_btnRefresh);
            Controls.Add(_lblInboxCount);
            Controls.Add(_btnDistill);
            Controls.Add(_btnHealthCheck);
            Controls.Add(_btnProcessInbox);
            Controls.Add(_lnkHelp);
            Controls.Add(_cmbMemoryBank);
            Controls.Add(_lblHeading);

            // Manual layout — position controls left to right
            Resize += (s, e) => LayoutControls();
            Layout += (s, e) => LayoutControls();
        }

        private void LayoutControls()
        {
            if (_btnProcessInbox == null) return;

            var y = (Height - _btnProcessInbox.Height) / 2;
            var x = UiScale.Pixels(4);

            _lblHeading.Location = new Point(x,
                (Height - _lblHeading.Height) / 2);
            x += _lblHeading.Width + UiScale.Pixels(4);

            _cmbMemoryBank.Location = new Point(x,
                (Height - _cmbMemoryBank.Height) / 2);
            x += _cmbMemoryBank.Width + UiScale.Pixels(4);

            _lnkHelp.Location = new Point(x,
                (Height - _lnkHelp.Height) / 2);
            x += _lnkHelp.Width + UiScale.Pixels(6);

            _btnProcessInbox.Location = new Point(x, y);
            x += _btnProcessInbox.Width + UiScale.Pixels(2);

            _btnHealthCheck.Location = new Point(x, y);
            x += _btnHealthCheck.Width + UiScale.Pixels(2);

            _btnDistill.Location = new Point(x, y);
            x += _btnDistill.Width + UiScale.Pixels(6);

            _lblInboxCount.Location = new Point(x,
                (Height - _lblInboxCount.Height) / 2);
            x += _lblInboxCount.Width + UiScale.Pixels(2);

            _btnRefresh.Location = new Point(x,
                (Height - _btnRefresh.Height) / 2);
        }

        /// <summary>
        /// Updates the inbox file count display and enables/disables the Process Inbox button.
        /// </summary>
        public void UpdateInboxCount(int count)
        {
            if (_lblInboxCount == null) return;
            _lblInboxCount.Text = count > 0
                ? $"{count} file{(count != 1 ? "s" : "")} in inbox"
                : "Inbox empty";
            _btnProcessInbox.Enabled = count > 0;
            _btnProcessInbox.ForeColor = count > 0
                ? Color.FromArgb(30, 90, 158)
                : Color.FromArgb(170, 170, 170);
        }

        /// <summary>
        /// Enables or disables both buttons (e.g. during processing).
        /// </summary>
        public void SetBusy(bool busy)
        {
            _btnProcessInbox.Enabled = !busy;
            _btnHealthCheck.Enabled = !busy;
            _btnDistill.Enabled = !busy;
            if (!busy)
            {
                _btnProcessInbox.ForeColor = _btnProcessInbox.Enabled
                    ? Color.FromArgb(30, 90, 158)
                    : Color.FromArgb(170, 170, 170);
                _btnHealthCheck.ForeColor = Color.FromArgb(30, 90, 158);
                _btnDistill.ForeColor = Color.FromArgb(30, 90, 158);
            }
            else
            {
                _btnProcessInbox.ForeColor = Color.FromArgb(170, 170, 170);
                _btnHealthCheck.ForeColor = Color.FromArgb(170, 170, 170);
                _btnDistill.ForeColor = Color.FromArgb(170, 170, 170);
            }
        }

        // ══════════════════════════════════════════════════════════════
        //  Memory bank dropdown
        // ══════════════════════════════════════════════════════════════

        /// <summary>
        /// Replaces the memory bank dropdown contents with the given list and
        /// selects <paramref name="activeBank"/> if present. Does not raise
        /// <see cref="MemoryBankChanged"/> — callers drive that side effect
        /// themselves so repopulation after a user switch is silent.
        /// </summary>
        /// <param name="banks">Bank names from <c>UserDataPath.ListMemoryBanks()</c>.</param>
        /// <param name="activeBank">The bank that should appear selected, or null.</param>
        public void SetMemoryBanks(System.Collections.Generic.IList<string> banks, string activeBank)
        {
            if (_cmbMemoryBank == null) return;

            _suppressComboChange = true;
            try
            {
                _cmbMemoryBank.Items.Clear();

                if (banks == null || banks.Count == 0)
                {
                    _cmbMemoryBank.Items.Add("(no memory banks)");
                    _cmbMemoryBank.SelectedIndex = 0;
                    _cmbMemoryBank.Enabled = false;
                    LayoutControls();
                    return;
                }

                _cmbMemoryBank.Enabled = true;

                int selected = 0;
                for (int i = 0; i < banks.Count; i++)
                {
                    _cmbMemoryBank.Items.Add(banks[i]);
                    if (string.Equals(banks[i], activeBank, System.StringComparison.Ordinal))
                        selected = i;
                }
                _cmbMemoryBank.SelectedIndex = selected;
            }
            finally
            {
                _suppressComboChange = false;
            }

            LayoutControls();
        }

        /// <summary>Returns the currently selected bank name, or null if none.</summary>
        public string SelectedMemoryBank
        {
            get
            {
                if (_cmbMemoryBank == null) return null;
                var item = _cmbMemoryBank.SelectedItem as string;
                if (string.IsNullOrEmpty(item)) return null;
                if (item == "(no memory banks)") return null;
                return item;
            }
        }

        private void OnMemoryBankComboChanged(object sender, EventArgs e)
        {
            if (_suppressComboChange) return;
            var name = SelectedMemoryBank;
            if (string.IsNullOrEmpty(name)) return;
            MemoryBankChanged?.Invoke(this, new MemoryBankChangedEventArgs(name));
        }
    }

    /// <summary>
    /// Event args for <see cref="SuperMemoryToolbar.MemoryBankChanged"/>.
    /// Carries the name of the bank the user just selected.
    /// </summary>
    public class MemoryBankChangedEventArgs : EventArgs
    {
        public string BankName { get; }

        public MemoryBankChangedEventArgs(string bankName)
        {
            BankName = bankName;
        }
    }
}
