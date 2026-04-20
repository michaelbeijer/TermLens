using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Supervertaler.Trados.Core;
using Supervertaler.Trados.Models;

namespace Supervertaler.Trados.Controls
{
    /// <summary>
    /// WinForms UserControl for the Batch Operations tab.
    /// Supports two modes: Translate (batch AI translation) and Proofread (AI proofreading).
    /// Displays mode toggle, scope selector, provider info, progress, and log.
    /// All layout is programmatic (no designer file).
    /// </summary>
    public class BatchTranslateControl : UserControl
    {
        // Header
        private Label _lblHeader;

        // Mode toggle
        private Panel _modePanel;
        private RadioButton _rbTranslate;
        private RadioButton _rbProofread;
        private BatchMode _currentMode = BatchMode.Translate;

        // Configuration
        private ComboBox _cmbScope;
        private Label _lblScopeLabel;
        private Label _lblMaxSegLabel;
        private NumericUpDown _nudMaxSegments;
        private ComboBox _cmbPrompt;
        private Label _lblPromptLabel;
        private LinkLabel _lblProvider;
        private Label _lblProviderLabel;
        private Label _lblSegmentCount;
        private LinkLabel _lnkAiSettings;

        // Prompt list (aligned with ComboBox indices; index 0 = "None")
        private List<PromptTemplate> _promptList = new List<PromptTemplate>();
        private string _activePromptPath; // per-project active prompt for visual indicator

        // Progress
        private ProgressBar _progressBar;
        private Label _lblProgress;

        // Action
        private Button _btnTranslate;
        private CheckBox _chkAddComments;
        private LinkLabel _lnkGeneratePrompt;

        // Clipboard Mode
        private CheckBox _chkClipboardMode;
        private Button _btnCopyToClipboard;
        private Button _btnPasteFromClipboard;

        // TMX backup
        private CheckBox _chkTmxBackup;
        private LinkLabel _lnkOpenBackupFolder;

        // Log
        private Label _lblLog;
        private TextBox _txtLog;

        // State
        private bool _isRunning;
        private string _currentProvider;
        private string _currentModel;

        /// <summary>Fired when user clicks "Translate".</summary>
        public event EventHandler TranslateRequested;

        /// <summary>Fired when user clicks "Proofread".</summary>
        public event EventHandler ProofreadRequested;

        /// <summary>Fired when user clicks "Stop".</summary>
        public event EventHandler StopRequested;

        /// <summary>Fired when user clicks the "AI Settings…" link.</summary>
        public event EventHandler OpenAiSettingsRequested;

        /// <summary>Fired when user changes the scope dropdown.</summary>
        public event EventHandler ScopeChanged;

        /// <summary>Fired when user switches between Translate and Proofread mode.</summary>
        public event EventHandler BatchModeChanged;

        /// <summary>Fired when user clicks "AutoPrompt".</summary>
        public event EventHandler GeneratePromptRequested;

        /// <summary>
        /// Raised when the user selects a different model from the provider dropdown.
        /// Args: (providerKey, modelId).
        /// </summary>
        public event Action<string, string> ModelChangeRequested;

        /// <summary>Fired when user clicks "Copy to Clipboard" in Clipboard Mode.</summary>
        public event EventHandler CopyToClipboardRequested;

        /// <summary>Fired when user clicks "Paste from Clipboard" in Clipboard Mode.</summary>
        public event EventHandler PasteFromClipboardRequested;

        /// <summary>Gets the current batch mode.</summary>
        public BatchMode CurrentMode => _currentMode;

        /// <summary>Whether Clipboard Mode is active.</summary>
        public bool IsClipboardMode => _chkClipboardMode?.Checked ?? false;

        /// <summary>Whether proofreading issues should also be added as Trados comments.</summary>
        public bool AddAsComments => _chkAddComments?.Checked ?? false;

        /// <summary>Whether the user wants translated segments auto-backed-up to a TMX file.</summary>
        public bool IsTmxBackupEnabled => _chkTmxBackup?.Checked ?? true;

        /// <summary>Fired when user clicks "Open folder…" next to the TMX backup checkbox.</summary>
        public event EventHandler OpenBackupFolderRequested;

        public BatchTranslateControl()
        {
            BuildUI();
            Resize += (s, e) => LayoutPromptRow();
        }

        private void BuildUI()
        {
            SuspendLayout();
            BackColor = Color.White;
            AutoScroll = false;

            var labelColor = Color.FromArgb(80, 80, 80);
            var headerFont = new Font("Segoe UI", 9f, FontStyle.Bold);
            var bodyFont = new Font("Segoe UI", 8.5f);
            var logFont = new Font("Consolas", 8f);

            var y = 10;

            // ─── Header ────────────────────────────────────────
            _lblHeader = new Label
            {
                Text = "Batch Operations",
                Font = headerFont,
                ForeColor = Color.FromArgb(50, 50, 50),
                Location = new Point(12, y),
                AutoSize = true
            };
            Controls.Add(_lblHeader);
            y += 26;

            // ─── Mode Toggle ──────────────────────────────────
            _modePanel = new Panel
            {
                Location = new Point(12, y),
                Size = new Size(300, 24),
                BackColor = Color.Transparent
            };

            _rbTranslate = new RadioButton
            {
                Text = "Translate",
                Location = new Point(0, 2),
                AutoSize = true,
                Font = bodyFont,
                ForeColor = labelColor,
                Checked = true,
                FlatStyle = FlatStyle.Flat
            };
            _rbTranslate.CheckedChanged += OnModeChanged;

            _rbProofread = new RadioButton
            {
                Text = "Proofread",
                Location = new Point(100, 2),
                AutoSize = true,
                Font = bodyFont,
                ForeColor = labelColor,
                FlatStyle = FlatStyle.Flat
            };

            _modePanel.Controls.Add(_rbTranslate);
            _modePanel.Controls.Add(_rbProofread);
            Controls.Add(_modePanel);
            y += 28;

            // ─── Clipboard Mode ──────────────────────────────────
            _chkClipboardMode = new CheckBox
            {
                Text = "Clipboard Mode",
                Location = new Point(12, y),
                AutoSize = true,
                Font = bodyFont,
                ForeColor = labelColor,
                Checked = false
            };
            _chkClipboardMode.CheckedChanged += OnClipboardModeChanged;
            var clipTip = new ToolTip { AutoPopDelay = 10000, InitialDelay = 300 };
            clipTip.SetToolTip(_chkClipboardMode,
                "Copy a ready-to-use prompt with segments, instructions, terminology,\r\n" +
                "and context to the clipboard. Paste into any web-based AI (ChatGPT,\r\n" +
                "Claude, Gemini, etc.), then paste translations back when done.");
            Controls.Add(_chkClipboardMode);
            y += 24;

            // ─── Scope ─────────────────────────────────────────
            _lblScopeLabel = new Label
            {
                Text = "Scope:",
                Location = new Point(12, y + 3),
                AutoSize = true,
                Font = bodyFont,
                ForeColor = labelColor
            };
            _cmbScope = new ComboBox
            {
                Location = new Point(100, y),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = bodyFont
            };
            PopulateTranslateScopes();
            _cmbScope.SelectedIndexChanged += (s, e) => ScopeChanged?.Invoke(this, EventArgs.Empty);
            Controls.Add(_lblScopeLabel);
            Controls.Add(_cmbScope);

            _lblMaxSegLabel = new Label
            {
                Text = "Limit:",
                Location = new Point(_cmbScope.Right + 12, y + 3),
                AutoSize = true,
                Font = bodyFont,
                ForeColor = labelColor
            };
            _nudMaxSegments = new NumericUpDown
            {
                Location = new Point(_lblMaxSegLabel.Right + 4, y),
                Width = 60,
                Minimum = 0,
                Maximum = 99999,
                Value = 0,
                Font = bodyFont
            };
            var limitTip = new ToolTip { AutoPopDelay = 8000, InitialDelay = 300 };
            limitTip.SetToolTip(_nudMaxSegments,
                "Maximum number of segments to process.\r\n" +
                "0 = no limit (process all matching segments).\r\n" +
                "Useful for testing prompts on a small subset.");
            limitTip.SetToolTip(_lblMaxSegLabel,
                "Maximum number of segments to process.\r\n" +
                "0 = no limit (process all matching segments).\r\n" +
                "Useful for testing prompts on a small subset.");
            Controls.Add(_lblMaxSegLabel);
            Controls.Add(_nudMaxSegments);
            y += 28;

            // ─── Prompt ──────────────────────────────────────────
            _lblPromptLabel = new Label
            {
                Text = "Prompt:",
                Location = new Point(12, y + 3),
                AutoSize = true,
                Font = bodyFont,
                ForeColor = labelColor
            };
            _cmbPrompt = new ComboBox
            {
                Location = new Point(100, y),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = bodyFont,
            };
            _cmbPrompt.Items.Add("(None \u2014 default)");
            _cmbPrompt.SelectedIndex = 0;
            Controls.Add(_lblPromptLabel);
            Controls.Add(_cmbPrompt);

            // ─── Generate Prompt link ────────────────────────────
            _lnkGeneratePrompt = new LinkLabel
            {
                Text = "AutoPrompt\u2026",
                Location = new Point(_cmbPrompt.Right + 8, y + 2),
                AutoSize = true,
                Font = bodyFont,
                LinkColor = Color.FromArgb(0, 102, 204)
            };
            _lnkGeneratePrompt.LinkClicked += (s, ev) =>
                GeneratePromptRequested?.Invoke(this, EventArgs.Empty);
            var tip = new ToolTip();
            tip.SetToolTip(_lnkGeneratePrompt,
                "AutoPrompt analyses your project\u2019s content, terminology (via TermScan),\r\n" +
                "and TM data to generate a domain-specific translation prompt using AI.\r\n\r\n" +
                "The result appears in the AI Assistant chat, where you can refine it.\r\n" +
                "Right-click any assistant message \u2192 \u201cSave as Prompt\u2026\u201d to save it.");
            Controls.Add(_lnkGeneratePrompt);
            y += 28;

            // ─── Provider ───────────────────────────────────────
            _lblProviderLabel = new Label
            {
                Text = "Provider:",
                Location = new Point(12, y + 1),
                AutoSize = true,
                Font = bodyFont,
                ForeColor = labelColor
            };
            _lblProvider = new LinkLabel
            {
                Text = "Not configured",
                Location = new Point(100, y + 1),
                AutoSize = true,
                Font = bodyFont,
                LinkColor = Color.FromArgb(50, 50, 50),
                ActiveLinkColor = Color.FromArgb(0, 102, 204),
                VisitedLinkColor = Color.FromArgb(50, 50, 50)
            };
            _lblProvider.LinkClicked += OnProviderSelectorClicked;
            Controls.Add(_lblProviderLabel);
            Controls.Add(_lblProvider);
            y += 22;

            // ─── AI Settings link ─────────────────────────────────
            _lnkAiSettings = new LinkLabel
            {
                Text = "AI Settings\u2026",
                Location = new Point(100, y),
                AutoSize = true,
                Font = bodyFont,
                LinkColor = Color.FromArgb(0, 102, 204)
            };
            _lnkAiSettings.LinkClicked += (s, ev) =>
                OpenAiSettingsRequested?.Invoke(this, EventArgs.Empty);
            Controls.Add(_lnkAiSettings);
            y += 20;

            // ─── Segment count ──────────────────────────────────
            _lblSegmentCount = new Label
            {
                Text = "Segments: \u2014",
                Location = new Point(12, y + 1),
                AutoSize = true,
                Font = bodyFont,
                ForeColor = Color.FromArgb(100, 100, 100)
            };
            Controls.Add(_lblSegmentCount);
            y += 28;

            // ─── Progress bar ───────────────────────────────────
            _progressBar = new ProgressBar
            {
                Location = new Point(12, y),
                Height = 18,
                Width = 320,
                Minimum = 0,
                Maximum = 100,
                Value = 0,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            _lblProgress = new Label
            {
                Text = "",
                Location = new Point(340, y + 1),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(100, 100, 100),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            Controls.Add(_progressBar);
            Controls.Add(_lblProgress);
            y += 28;

            // ─── Translate / Stop button ────────────────────────
            _btnTranslate = new Button
            {
                Text = "\u25B6  Translate",
                Location = new Point(12, y),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowOnly,
                MinimumSize = new Size(120, 28),
                Height = 28,
                FlatStyle = FlatStyle.System,
                Font = bodyFont
            };
            _btnTranslate.Click += OnActionClick;
            Controls.Add(_btnTranslate);

            _chkAddComments = new CheckBox
            {
                Text = "Also add issues as Trados comments",
                Location = new Point(140, y + 4),
                AutoSize = true,
                Font = bodyFont,
                ForeColor = Color.FromArgb(80, 80, 80),
                Checked = false,
                Visible = false // only shown in Proofread mode
            };
            Controls.Add(_chkAddComments);

            // ─── Clipboard Mode buttons (hidden by default) ─────
            _btnCopyToClipboard = new Button
            {
                Text = "\uD83D\uDCCB  Copy to Clipboard",
                Location = new Point(12, y),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowOnly,
                MinimumSize = new Size(140, 28),
                Height = 28,
                FlatStyle = FlatStyle.System,
                Font = bodyFont,
                Visible = false
            };
            _btnCopyToClipboard.Click += (s, ev) =>
                CopyToClipboardRequested?.Invoke(this, EventArgs.Empty);
            Controls.Add(_btnCopyToClipboard);

            _btnPasteFromClipboard = new Button
            {
                Text = "\uD83D\uDCCB  Paste from Clipboard",
                Location = new Point(170, y),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowOnly,
                MinimumSize = new Size(150, 28),
                Height = 28,
                FlatStyle = FlatStyle.System,
                Font = bodyFont,
                Visible = false,
                Enabled = false
            };
            _btnPasteFromClipboard.Click += (s, ev) =>
                PasteFromClipboardRequested?.Invoke(this, EventArgs.Empty);
            Controls.Add(_btnPasteFromClipboard);

            y += 38;

            // ─── TMX Backup ──────────────────────────────────────
            _chkTmxBackup = new CheckBox
            {
                Text = "Auto-backup translations to TMX",
                Location = new Point(12, y),
                AutoSize = true,
                Font = bodyFont,
                ForeColor = Color.FromArgb(80, 80, 80),
                Checked = true
            };
            var tmxTip = new ToolTip { AutoPopDelay = 12000, InitialDelay = 300 };
            tmxTip.SetToolTip(_chkTmxBackup,
                "Saves every translated segment to a TMX file as it arrives.\r\n" +
                "If Trados crashes mid-run, you can import the backup TMX into\r\n" +
                "any TM and recover the completed translations.\r\n\r\n" +
                "The file is also useful for populating TMs in other CAT tools.");
            Controls.Add(_chkTmxBackup);

            _lnkOpenBackupFolder = new LinkLabel
            {
                Text = "Open folder\u2026",
                AutoSize = true,
                Font = bodyFont,
                LinkColor = Color.FromArgb(0, 102, 204)
            };
            _lnkOpenBackupFolder.Location = new Point(_chkTmxBackup.Right + 8, y + 2);
            _lnkOpenBackupFolder.LinkClicked += (s, ev) =>
                OpenBackupFolderRequested?.Invoke(this, EventArgs.Empty);
            tmxTip.SetToolTip(_lnkOpenBackupFolder,
                "Opens the folder where backup TMX files are stored.");
            Controls.Add(_lnkOpenBackupFolder);
            y += 24;

            // ─── Log ────────────────────────────────────────────
            _lblLog = new Label
            {
                Text = "Log:",
                Location = new Point(12, y),
                AutoSize = true,
                Font = bodyFont,
                ForeColor = labelColor
            };
            Controls.Add(_lblLog);
            y += 18;

            _txtLog = new TextBox
            {
                Location = new Point(12, y),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = logFont,
                BackColor = Color.FromArgb(248, 248, 248),
                ForeColor = Color.FromArgb(60, 60, 60),
                BorderStyle = BorderStyle.FixedSingle,
                WordWrap = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            Controls.Add(_txtLog);

            ResumeLayout(false);

            // Handle resize for responsive layout
            Resize += OnResize;
            OnResize(this, EventArgs.Empty);
        }

        private void OnResize(object sender, EventArgs e)
        {
            if (_txtLog == null) return;
            var w = Width - 24;
            _txtLog.Width = Math.Max(100, w);
            _txtLog.Height = Math.Max(40, Height - _txtLog.Top - 8);

            _progressBar.Width = Math.Max(100, w - 80);
            _lblProgress.Location = new Point(_progressBar.Right + 8, _lblProgress.Top);
        }

        // ─── Mode Toggle ──────────────────────────────────────────

        private void OnModeChanged(object sender, EventArgs e)
        {
            if (!_rbTranslate.Checked && !_rbProofread.Checked) return;

            _currentMode = _rbTranslate.Checked ? BatchMode.Translate : BatchMode.Proofread;

            // Update scope dropdown items
            var prevScope = _cmbScope.SelectedIndex;
            if (_currentMode == BatchMode.Translate)
                PopulateTranslateScopes();
            else
                PopulateProofreadScopes();

            // Update action button text
            UpdateActionButtonText();

            // Show/hide mode-specific controls
            _chkAddComments.Visible = _currentMode == BatchMode.Proofread;
            _lnkGeneratePrompt.Visible = _currentMode == BatchMode.Translate;
            var isTranslateMode = _currentMode == BatchMode.Translate && !(_chkClipboardMode?.Checked ?? false);
            _chkTmxBackup.Visible = isTranslateMode;
            _lnkOpenBackupFolder.Visible = isTranslateMode;

            // Notify listeners to refresh prompt dropdown
            BatchModeChanged?.Invoke(this, EventArgs.Empty);
        }

        private void PopulateTranslateScopes()
        {
            _cmbScope.Items.Clear();
            _cmbScope.Items.Add("Empty segments only");
            _cmbScope.Items.Add("All segments");
            _cmbScope.Items.Add("Filtered segments");
            _cmbScope.Items.Add("Filtered (empty only)");
            _cmbScope.SelectedIndex = 0;
        }

        private void PopulateProofreadScopes()
        {
            _cmbScope.Items.Clear();
            _cmbScope.Items.Add("Translated only");
            _cmbScope.Items.Add("Translated + approved/signed-off");
            _cmbScope.Items.Add("All segments");
            _cmbScope.Items.Add("Filtered segments");
            _cmbScope.Items.Add("Filtered (translated only)");
            _cmbScope.SelectedIndex = 0;
        }

        private void UpdateActionButtonText()
        {
            if (_isRunning)
            {
                _btnTranslate.Text = _currentMode == BatchMode.Translate
                    ? "\u25A0  Stop translating"
                    : "\u25A0  Stop proofreading";
            }
            else
            {
                _btnTranslate.Text = _currentMode == BatchMode.Translate
                    ? "\u25B6  Translate"
                    : "\u25B6  Proofread";
            }
        }

        // ─── Clipboard Mode ───────────────────────────────────────

        private void OnClipboardModeChanged(object sender, EventArgs e)
        {
            var clip = _chkClipboardMode.Checked;

            // Toggle visibility: API controls vs clipboard buttons
            _lblProviderLabel.Visible = !clip;
            _lblProvider.Visible = !clip;
            _lnkAiSettings.Visible = !clip;
            _btnTranslate.Visible = !clip;

            _btnCopyToClipboard.Visible = clip;
            _btnPasteFromClipboard.Visible = clip;

            // Proofread comments checkbox stays visible in proofread mode regardless
            if (clip)
                _chkAddComments.Visible = false;
            else
                _chkAddComments.Visible = _currentMode == BatchMode.Proofread;

            // TMX backup only applies to non-clipboard translate mode
            var showTmx = !clip && _currentMode == BatchMode.Translate;
            _chkTmxBackup.Visible = showTmx;
            _lnkOpenBackupFolder.Visible = showTmx;
        }

        /// <summary>
        /// Enables or disables the "Paste from Clipboard" button.
        /// Called by the ViewPart after a successful copy operation.
        /// </summary>
        public void EnablePasteButton(bool enabled)
        {
            if (_btnPasteFromClipboard != null)
                _btnPasteFromClipboard.Enabled = enabled;
        }

        // ─── Event Handlers ──────────────────────────────────────

        private void OnActionClick(object sender, EventArgs e)
        {
            if (_isRunning)
            {
                StopRequested?.Invoke(this, EventArgs.Empty);
            }
            else if (_currentMode == BatchMode.Proofread)
            {
                ProofreadRequested?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                TranslateRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        // ─── Public Methods (called by ViewPart) ─────────────────

        /// <summary>
        /// Updates the displayed provider and model name.
        /// </summary>
        public void UpdateProviderDisplay(string providerName, string modelName)
        {
            _currentProvider = providerName;
            _currentModel = modelName;
            _lblProvider.Text = !string.IsNullOrEmpty(providerName) && !string.IsNullOrEmpty(modelName)
                ? providerName + " / " + modelName
                : "Not configured";
        }

        // ─── Provider/model selector menu ──────────────────────

        private void OnProviderSelectorClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var menu = new ContextMenuStrip { Font = new Font("Segoe UI", UiScale.FontSize(8.5f)) };

            foreach (var providerKey in LlmModels.AllProviderKeys)
            {
                var models = LlmModels.GetModelsForProvider(providerKey);

                // Custom OpenAI profiles are handled separately
                if (providerKey == LlmModels.ProviderCustomOpenAi)
                    continue;

                if (models.Length == 0) continue;

                var providerName = LlmModels.GetProviderDisplayName(providerKey);
                var providerItem = new ToolStripMenuItem(providerName);

                foreach (var model in models)
                {
                    var modelItem = new ToolStripMenuItem(model.DisplayName)
                    {
                        ToolTipText = model.Description,
                        Tag = new[] { providerKey, model.Id }
                    };

                    // Checkmark for current selection
                    if (providerKey == _currentProvider && model.Id == _currentModel)
                        modelItem.Checked = true;

                    modelItem.Click += OnModelMenuItemClicked;
                    providerItem.DropDownItems.Add(modelItem);
                }

                // Bold the provider submenu if it's the active one
                if (providerKey == _currentProvider)
                    providerItem.Font = new Font(providerItem.Font, FontStyle.Bold);

                menu.Items.Add(providerItem);
            }

            menu.Show(_lblProvider, new Point(0, -menu.PreferredSize.Height));
        }

        private void OnModelMenuItemClicked(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            var tag = item?.Tag as string[];
            if (tag == null || tag.Length != 2) return;

            ModelChangeRequested?.Invoke(tag[0], tag[1]);
        }

        /// <summary>
        /// Updates the segment count display.
        /// </summary>
        public void UpdateSegmentCounts(int emptyCount, int totalCount, int filteredCount = -1)
        {
            var scope = GetSelectedScope();
            if ((scope == BatchScope.Filtered || scope == BatchScope.FilteredEmptyOnly) && filteredCount >= 0)
                _lblSegmentCount.Text = $"Segments: {filteredCount} filtered / {emptyCount} empty / {totalCount} total";
            else
                _lblSegmentCount.Text = $"Segments: {emptyCount} empty / {totalCount} total";
            UpdateTranslateButton();
        }

        /// <summary>
        /// Populates the prompt dropdown with available prompts and selects the specified one.
        /// When categoryFilter is provided, only prompts whose Domain matches are shown.
        /// If no prompt matches selectedRelativePath and a projectName is provided,
        /// the dropdown auto-selects the first prompt whose name contains the project name.
        /// </summary>
        public void SetPrompts(List<PromptTemplate> prompts, string selectedRelativePath,
            string categoryFilter = null, string projectName = null, string activePromptPath = null)
        {
            _activePromptPath = activePromptPath;
            _cmbPrompt.Items.Clear();
            _cmbPrompt.Items.Add("(None \u2014 default)");
            _promptList.Clear();

            int selectedIdx = 0;
            int projectMatchIdx = 0;
            if (prompts != null)
            {
                foreach (var p in prompts)
                {
                    // Is this the active prompt for the project?
                    var isActive = !string.IsNullOrEmpty(activePromptPath)
                        && string.Equals(
                            (p.RelativePath ?? "").Replace('/', '\\'),
                            (activePromptPath ?? "").Replace('/', '\\'),
                            StringComparison.OrdinalIgnoreCase);

                    // Filter by category if specified — but always include the active
                    // prompt even if its category doesn't match, so "Set as active"
                    // works for any prompt regardless of folder.
                    if (!string.IsNullOrEmpty(categoryFilter) &&
                        !string.Equals(p.Category, categoryFilter, StringComparison.OrdinalIgnoreCase) &&
                        !isActive)
                        continue;

                    _promptList.Add(p);
                    _cmbPrompt.Items.Add(isActive ? p.Name + "  \u2714" : p.Name);

                    if (!string.IsNullOrEmpty(selectedRelativePath) &&
                        string.Equals(
                            (p.RelativePath ?? "").Replace('/', '\\'),
                            (selectedRelativePath ?? "").Replace('/', '\\'),
                            StringComparison.OrdinalIgnoreCase))
                    {
                        selectedIdx = _cmbPrompt.Items.Count - 1;
                    }

                    // Track first prompt whose name contains the project name (fallback)
                    if (projectMatchIdx == 0 && !string.IsNullOrEmpty(projectName) &&
                        p.Name.IndexOf(projectName, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        projectMatchIdx = _cmbPrompt.Items.Count - 1;
                    }
                }
            }

            // Fall back to project-name match if no prompt was matched by path
            if (selectedIdx == 0 && projectMatchIdx > 0)
                selectedIdx = projectMatchIdx;

            _cmbPrompt.SelectedIndex = selectedIdx;

            // Auto-size dropdown width to fit longest item
            int maxWidth = _cmbPrompt.Width;
            using (var g = _cmbPrompt.CreateGraphics())
            {
                foreach (var item in _cmbPrompt.Items)
                {
                    var w = (int)g.MeasureString(item.ToString(), _cmbPrompt.Font).Width + 24;
                    if (w > maxWidth) maxWidth = w;
                }
            }
            _cmbPrompt.DropDownWidth = maxWidth;
        }

        /// <summary>
        /// Returns the currently selected prompt template, or null if "(None)" is selected.
        /// </summary>
        public PromptTemplate GetSelectedPrompt()
        {
            var idx = _cmbPrompt.SelectedIndex - 1; // 0 = "(None)", so subtract 1
            if (idx < 0 || idx >= _promptList.Count)
                return null;
            return _promptList[idx];
        }

        /// <summary>
        /// Returns the relative path of the selected prompt (for settings persistence).
        /// </summary>
        public string GetSelectedPromptPath()
        {
            var prompt = GetSelectedPrompt();
            return prompt?.RelativePath ?? "";
        }

        /// <summary>
        /// Lays out the prompt combo and AutoPrompt link so the combo fills
        /// available width minus space reserved for the link.
        /// </summary>
        private void LayoutPromptRow()
        {
            if (_cmbPrompt == null || _lnkGeneratePrompt == null) return;
            var linkWidth = _lnkGeneratePrompt.PreferredWidth + 8; // 8px gap
            var availableWidth = ClientSize.Width - _cmbPrompt.Left - linkWidth - 8; // 8px right margin
            if (availableWidth < 100) availableWidth = 100;
            _cmbPrompt.Width = availableWidth;
            _lnkGeneratePrompt.Left = _cmbPrompt.Right + 8;
        }

        /// <summary>
        /// Returns the selected batch scope (for Translate mode).
        /// </summary>
        public BatchScope GetSelectedScope()
        {
            switch (_cmbScope.SelectedIndex)
            {
                case 1: return BatchScope.All;
                case 2: return BatchScope.Filtered;
                case 3: return BatchScope.FilteredEmptyOnly;
                default: return BatchScope.EmptyOnly;
            }
        }

        /// <summary>
        /// Returns the segment limit (0 = no limit).
        /// </summary>
        public int GetMaxSegments() => (int)_nudMaxSegments.Value;

        /// <summary>
        /// Returns the selected proofread scope (for Proofread mode).
        /// </summary>
        public ProofreadScope GetSelectedProofreadScope()
        {
            switch (_cmbScope.SelectedIndex)
            {
                case 1: return ProofreadScope.TranslatedAndConfirmed;
                case 2: return ProofreadScope.AllSegments;
                case 3: return ProofreadScope.Filtered;
                case 4: return ProofreadScope.FilteredConfirmedOnly;
                default: return ProofreadScope.ConfirmedOnly;
            }
        }

        /// <summary>
        /// Reports progress from the batch translator.
        /// </summary>
        public void ReportProgress(int current, int total, string message, bool isError)
        {
            if (total > 0)
            {
                _progressBar.Maximum = total;
                _progressBar.Value = Math.Min(current, total);
                _lblProgress.Text = $"{current}/{total}";
            }

            if (!string.IsNullOrEmpty(message))
                AppendLog(message, isError);
        }

        /// <summary>
        /// Reports batch translation completion.
        /// </summary>
        public void ReportCompleted(int translated, int failed, int skipped,
            TimeSpan elapsed, bool cancelled)
        {
            SetRunning(false);

            var status = cancelled ? "Cancelled" : "Complete";
            AppendLog(
                $"\u2014 {status}: {translated} translated, {failed} failed " +
                $"({elapsed.TotalSeconds:F1}s)",
                false);
        }

        /// <summary>
        /// Reports proofreading progress.
        /// </summary>
        public void ReportProofreadProgress(int current, int total)
        {
            if (total > 0)
            {
                _progressBar.Maximum = total;
                _progressBar.Value = Math.Min(current, total);
                _lblProgress.Text = $"{current}/{total}";
            }

            AppendLog($"\u2713 Checking segment {current}/{total}\u2026", false);
        }

        /// <summary>
        /// Reports proofreading completion with summary.
        /// </summary>
        public void ReportProofreadCompleted(int checkedCount, int issues, int ok,
            TimeSpan elapsed, bool cancelled)
        {
            SetRunning(false);

            var status = cancelled ? "Cancelled" : "Complete";
            var issueMarker = issues > 0 ? "\u26A0" : "\u2713";
            AppendLog(
                $"\u2014 {status}: {issueMarker} {issues} issue{(issues != 1 ? "s" : "")} found, " +
                $"\u2713 {ok} OK ({elapsed.TotalSeconds:F1}s)",
                false);
        }

        /// <summary>
        /// Toggles the UI between running and idle states.
        /// </summary>
        public void SetRunning(bool running)
        {
            _isRunning = running;
            UpdateActionButtonText();
            _cmbScope.Enabled = !running;
            _cmbPrompt.Enabled = !running;
            _rbTranslate.Enabled = !running;
            _rbProofread.Enabled = !running;
            _chkClipboardMode.Enabled = !running;
            if (_btnCopyToClipboard != null)
                _btnCopyToClipboard.Enabled = !running;

            if (!running)
            {
                _progressBar.Value = 0;
                _lblProgress.Text = "";
            }
        }

        /// <summary>
        /// Appends a timestamped line to the log.
        /// </summary>
        public void AppendLog(string message, bool isError = false)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var prefix = isError ? "\u2717 " : "";
            var line = timestamp + "  " + prefix + message + Environment.NewLine;

            _txtLog.AppendText(line);
            // Auto-scroll to bottom
            _txtLog.SelectionStart = _txtLog.TextLength;
            _txtLog.ScrollToCaret();
        }

        /// <summary>
        /// Resets the control state (e.g., when document changes).
        /// </summary>
        public void Reset()
        {
            _progressBar.Value = 0;
            _lblProgress.Text = "";
            _lblSegmentCount.Text = "Segments: \u2014";
            SetRunning(false);
        }

        private void UpdateTranslateButton()
        {
            // Disable translate button if there are no segments
            // (actual logic depends on document state; the ViewPart calls this)
        }
    }
}
