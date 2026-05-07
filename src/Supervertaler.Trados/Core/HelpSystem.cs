using System.Diagnostics;

namespace Supervertaler.Trados.Core
{
    /// <summary>
    /// Centralized context-sensitive help system.
    /// Maps UI elements to GitBook documentation pages and opens them in the browser.
    /// </summary>
    public static class HelpSystem
    {
        /// <summary>
        /// Base URL for the unified Supervertaler GitBook documentation site.
        /// Trados Plugin pages live under <c>/help/trados/</c>; Workbench pages
        /// live under <c>/help/workbench/</c>. The <c>/help</c> slug is the
        /// site path on GitBook (free plan requires a non-empty slug, so root
        /// publishing isn't available – <c>help</c> is the cleanest choice and
        /// pairs nicely with a future <c>help.supervertaler.com</c> custom
        /// domain.
        /// </summary>
        private const string DocsBaseUrl = "https://supervertaler.gitbook.io/help";

        /// <summary>
        /// Help topic identifiers. Each value is appended to <see cref="DocsBaseUrl"/>
        /// to form a full URL.
        /// <para>
        /// IMPORTANT: these are NOT file paths. They are the URL slugs that
        /// GitBook generates from each <c>## 🧩 Section</c> header in
        /// <c>SUMMARY.md</c> in the <c>Supervertaler-Help</c> repo. The slugs
        /// happen to look path-like but the section name comes from the
        /// SUMMARY heading, not the file path on disk – e.g. the Trados
        /// "AI Proofreader" page lives at <c>trados/ai-proofreader.md</c>
        /// in the repo but is published at
        /// <c>features/batch-operations/ai-proofreader</c> on GitBook
        /// because it's nested under the "Features" section.
        /// </para>
        /// <para>
        /// Old <c>trados/...</c>-style values still resolve via GitBook's
        /// 301 redirect map, but using the canonical slugs avoids the
        /// extra hop and is robust to GitBook eventually pruning legacy
        /// redirects.
        /// </para>
        /// <para>
        /// Regenerating after a SUMMARY.md restructure: the SUMMARY.md
        /// links and the GitBook sitemap-pages.xml line up 1:1 in
        /// document order (homepage included). Walk both, zip them, and
        /// emit a fresh map.
        /// </para>
        /// </summary>
        public static class Topics
        {
            public const string Overview            = "get-started/trados";
            public const string Installation        = "get-started/installation";
            public const string GettingStarted      = "get-started/getting-started";
            public const string Licensing           = "get-started/licensing";
            public const string AiCostGuide         = "get-started/ai-cost-guide";

            public const string TermLensPanel       = "features/termlens";
            public const string AddTermDialog       = "features/termlens/adding-terms";
            public const string TermLensPopup       = "features/termlens/termlens-popup";
            public const string TermPickerDialog    = "features/termlens/term-picker";
            public const string MultiTermSupport    = "features/multiterm-support";

            public const string AiAssistantChat     = "features/ai-assistant";
            public const string StudioTools         = "features/ai-assistant/studio-tools";

            // Memory banks (formerly "SuperMemory") – nested under the Supervertaler
            // Assistant section in SUMMARY.md. C# identifier names kept as
            // SuperMemory* for backwards-compat with existing call sites; rename
            // when the Trados UI strings are updated to match the new memory bank
            // terminology.
            public const string SuperMemory         = "features/ai-assistant/super-memory";
            public const string SuperMemoryQuickAdd = "features/ai-assistant/super-memory/quick-add";
            public const string SuperMemoryInbox    = "features/ai-assistant/super-memory/process-inbox";
            public const string SuperMemoryHealth   = "features/ai-assistant/super-memory/health-check";
            public const string SuperMemoryDistill  = "features/ai-assistant/super-memory/distill";
            public const string SuperMemoryObsidian = "features/ai-assistant/super-memory/obsidian-setup";

            public const string SuperSearch         = "features/supersearch";
            public const string QuickLauncher       = "features/quicklauncher";

            public const string BatchOperations     = "features/batch-operations";
            public const string BatchTranslate      = "features/batch-operations/batch-translate";
            public const string AiProofreader       = "features/batch-operations/ai-proofreader";
            public const string AiProofreaderReports = "features/batch-operations/ai-proofreader#reports-tab";
            public const string ClipboardMode       = "features/batch-operations/clipboard-mode";
            public const string GeneratePrompt      = "features/batch-operations/generate-prompt";

            public const string TermbaseEditor      = "terminology/termbase-management";

            public const string SettingsTermLens    = "settings/termlens";
            public const string SettingsAi          = "settings/ai-settings";
            public const string PromptLogging       = "settings/ai-settings#prompt-logging";
            public const string SettingsPrompts     = "settings/prompts";
            public const string SettingsBackup      = "settings/backup";
            public const string SettingsUsageStats  = "settings/usage-statistics";
            public const string SettingsGeneral     = "settings/usage-statistics";
            public const string ProjectSettings     = "settings/project-settings";

            public const string KeyboardShortcuts   = "reference/keyboard-shortcuts";
            public const string Troubleshooting     = "reference/troubleshooting";
        }

        /// <summary>
        /// Opens the help page for the given topic identifier.
        /// Falls back to the Trados landing page if topic is null/empty.
        /// </summary>
        public static void OpenHelp(string topic = null)
        {
            string url = string.IsNullOrEmpty(topic)
                ? DocsBaseUrl + "/" + Topics.Overview
                : DocsBaseUrl + "/" + topic.TrimStart('/');

            OpenUrl(url);
        }

        /// <summary>
        /// Opens the docs site root (the product chooser landing page).
        /// </summary>
        public static void OpenDocsHome()
        {
            OpenUrl(DocsBaseUrl);
        }

        private static void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch
            {
                // No default browser configured – silently ignore
            }
        }
    }
}
