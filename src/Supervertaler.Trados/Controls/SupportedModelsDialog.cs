using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Supervertaler.Trados.Core;

namespace Supervertaler.Trados.Controls
{
    /// <summary>
    /// Read-only dialog showing all supported LLM providers and models as Markdown.
    /// Opened from the "View all supported models" link in AiSettingsPanel.
    /// </summary>
    public class SupportedModelsDialog : Form
    {
        private RichTextBox _rtb;
        private Button _btnCopy;
        private Button _btnClose;

        public SupportedModelsDialog()
        {
            BuildUI();
            PopulateModels();
        }

        private void BuildUI()
        {
            Text = "Supported Models";
            Size = new Size(560, 520);
            MinimumSize = new Size(400, 300);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 9f);
            BackColor = Color.White;

            _rtb = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                BorderStyle = BorderStyle.None,
                Font = new Font("Consolas", 9.5f),
                BackColor = Color.White,
                Margin = new Padding(8),
                WordWrap = true
            };

            var pnlButtons = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 42,
                BackColor = Color.FromArgb(245, 245, 245),
                Padding = new Padding(8, 0, 8, 0)
            };

            _btnCopy = new Button
            {
                Text = "Copy to Clipboard",
                Width = 130,
                Height = 26,
                Location = new Point(8, 8),
                FlatStyle = FlatStyle.System
            };
            _btnCopy.Click += OnCopyClick;

            _btnClose = new Button
            {
                Text = "Close",
                Width = 80,
                Height = 26,
                Location = new Point(146, 8),
                FlatStyle = FlatStyle.System
            };
            _btnClose.Click += (s, e) => Close();

            pnlButtons.Controls.Add(_btnCopy);
            pnlButtons.Controls.Add(_btnClose);

            Controls.Add(_rtb);
            Controls.Add(pnlButtons);
        }

        private void PopulateModels()
        {
            _rtb.Text = BuildMarkdown();
        }

        private static string BuildMarkdown()
        {
            var sb = new StringBuilder();
            sb.AppendLine("# Supported Models");
            sb.AppendLine();

            foreach (var providerKey in LlmModels.AllProviderKeys)
            {
                var displayName = LlmModels.GetProviderDisplayName(providerKey);
                sb.AppendLine($"## {displayName}");
                sb.AppendLine();

                if (providerKey == LlmModels.ProviderCustomOpenAi)
                {
                    sb.AppendLine("Enter your own endpoint, model name, and API key in the Custom section.");
                    sb.AppendLine();
                    continue;
                }

                var models = LlmModels.GetModelsForProvider(providerKey);
                foreach (var m in models)
                {
                    sb.AppendLine($"- **{m.DisplayName}** (`{m.Id}`)");
                    sb.AppendLine($"  {m.Description}");
                }
                sb.AppendLine();
            }

            return sb.ToString().TrimEnd();
        }

        private void OnCopyClick(object sender, EventArgs e)
        {
            Clipboard.SetText(_rtb.Text);
            _btnCopy.Text = "Copied!";
            var timer = new Timer { Interval = 1500 };
            timer.Tick += (s, _) => { _btnCopy.Text = "Copy to Clipboard"; timer.Stop(); timer.Dispose(); };
            timer.Start();
        }
    }
}
