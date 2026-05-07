# Help Link Reference

All context-sensitive help links in the plugin, mapped to their online documentation pages.

Help links are defined in [`HelpSystem.cs`](src/Supervertaler.Trados/Core/HelpSystem.cs) and opened via `HelpSystem.OpenHelp(topic)`. The base URL is `https://supervertaler.gitbook.io/help` (the unified Workbench + Trados help site).

Last audited: 2026-05-07

---

## HelpSystem topics

These are the topic constants defined in `HelpSystem.Topics` and the documentation pages they link to. The slug values are GitBook's auto-generated URL paths derived from the section headers in [`SUMMARY.md`](https://github.com/Supervertaler/Supervertaler-Help/blob/main/SUMMARY.md) – they intentionally do not match the on-disk file paths in the `Supervertaler-Help` repo.

| Topic constant | Online URL | Used by |
|---|---|---|
| `Overview` | [/help/get-started/trados](https://supervertaler.gitbook.io/help/get-started/trados) | OpenHelp fallback |
| `Installation` | [/help/get-started/installation](https://supervertaler.gitbook.io/help/get-started/installation) | – (not yet used in UI) |
| `GettingStarted` | [/help/get-started/getting-started](https://supervertaler.gitbook.io/help/get-started/getting-started) | – (not yet used in UI) |
| `Licensing` | [/help/get-started/licensing](https://supervertaler.gitbook.io/help/get-started/licensing) | Settings dialog – Licence tab |
| `AiCostGuide` | [/help/get-started/ai-cost-guide](https://supervertaler.gitbook.io/help/get-started/ai-cost-guide) | – (not yet used in UI) |
| `TermLensPanel` | [/help/features/termlens](https://supervertaler.gitbook.io/help/features/termlens) | MainPanelControl (? button, F1 key) |
| `AddTermDialog` | [/help/features/termlens/adding-terms](https://supervertaler.gitbook.io/help/features/termlens/adding-terms) | AddTermDialog, BulkAddNTDialog, TermEntryEditorDialog |
| `TermLensPopup` | [/help/features/termlens/termlens-popup](https://supervertaler.gitbook.io/help/features/termlens/termlens-popup) | – (not yet used in UI) |
| `TermPickerDialog` | [/help/features/termlens/term-picker](https://supervertaler.gitbook.io/help/features/termlens/term-picker) | TermPickerDialog |
| `MultiTermSupport` | [/help/features/multiterm-support](https://supervertaler.gitbook.io/help/features/multiterm-support) | MainPanelControl (MultiTerm help link) |
| `AiAssistantChat` | [/help/features/ai-assistant](https://supervertaler.gitbook.io/help/features/ai-assistant) | AiAssistantControl (? button when on Chat tab) |
| `StudioTools` | [/help/features/ai-assistant/studio-tools](https://supervertaler.gitbook.io/help/features/ai-assistant/studio-tools) | – (not yet used in UI) |
| `SuperMemory` | [/help/features/ai-assistant/super-memory](https://supervertaler.gitbook.io/help/features/ai-assistant/super-memory) | – (not yet used in UI) |
| `SuperMemoryQuickAdd` | [/help/features/ai-assistant/super-memory/quick-add](https://supervertaler.gitbook.io/help/features/ai-assistant/super-memory/quick-add) | – (not yet used in UI) |
| `SuperMemoryInbox` | [/help/features/ai-assistant/super-memory/process-inbox](https://supervertaler.gitbook.io/help/features/ai-assistant/super-memory/process-inbox) | – (not yet used in UI) |
| `SuperMemoryHealth` | [/help/features/ai-assistant/super-memory/health-check](https://supervertaler.gitbook.io/help/features/ai-assistant/super-memory/health-check) | – (not yet used in UI) |
| `SuperMemoryDistill` | [/help/features/ai-assistant/super-memory/distill](https://supervertaler.gitbook.io/help/features/ai-assistant/super-memory/distill) | – (not yet used in UI) |
| `SuperMemoryObsidian` | [/help/features/ai-assistant/super-memory/obsidian-setup](https://supervertaler.gitbook.io/help/features/ai-assistant/super-memory/obsidian-setup) | – (not yet used in UI) |
| `SuperSearch` | [/help/features/supersearch](https://supervertaler.gitbook.io/help/features/supersearch) | – (not yet used in UI) |
| `QuickLauncher` | [/help/features/quicklauncher](https://supervertaler.gitbook.io/help/features/quicklauncher) | – (not yet used in UI) |
| `BatchOperations` | [/help/features/batch-operations](https://supervertaler.gitbook.io/help/features/batch-operations) | AiAssistantControl (? button when on Batch tab) |
| `BatchTranslate` | [/help/features/batch-operations/batch-translate](https://supervertaler.gitbook.io/help/features/batch-operations/batch-translate) | – (not yet used in UI) |
| `AiProofreader` | [/help/features/batch-operations/ai-proofreader](https://supervertaler.gitbook.io/help/features/batch-operations/ai-proofreader) | – (not yet used in UI) |
| `AiProofreaderReports` | [/help/features/batch-operations/ai-proofreader#reports-tab](https://supervertaler.gitbook.io/help/features/batch-operations/ai-proofreader#reports-tab) | Reports tab help link |
| `ClipboardMode` | [/help/features/batch-operations/clipboard-mode](https://supervertaler.gitbook.io/help/features/batch-operations/clipboard-mode) | – (not yet used in UI) |
| `GeneratePrompt` | [/help/features/batch-operations/generate-prompt](https://supervertaler.gitbook.io/help/features/batch-operations/generate-prompt) | – (not yet used in UI) |
| `TermbaseEditor` | [/help/terminology/termbase-management](https://supervertaler.gitbook.io/help/terminology/termbase-management) | TermbaseEditorDialog, NewTermbaseDialog |
| `SettingsTermLens` | [/help/settings/termlens](https://supervertaler.gitbook.io/help/settings/termlens) | Settings dialog – TermLens tab |
| `SettingsAi` | [/help/settings/ai-settings](https://supervertaler.gitbook.io/help/settings/ai-settings) | Settings dialog – AI Settings tab |
| `PromptLogging` | [/help/settings/ai-settings#prompt-logging](https://supervertaler.gitbook.io/help/settings/ai-settings#prompt-logging) | Prompt-logging help link |
| `SettingsPrompts` | [/help/settings/prompts](https://supervertaler.gitbook.io/help/settings/prompts) | Settings dialog – Prompts tab, PromptEditorDialog |
| `SettingsBackup` | [/help/settings/backup](https://supervertaler.gitbook.io/help/settings/backup) | Settings dialog – Backup tab |
| `SettingsUsageStats` | [/help/settings/usage-statistics](https://supervertaler.gitbook.io/help/settings/usage-statistics) | – (not yet used in UI) |
| `SettingsGeneral` | [/help/settings/usage-statistics](https://supervertaler.gitbook.io/help/settings/usage-statistics) | (alias for `SettingsUsageStats`) |
| `ProjectSettings` | [/help/settings/project-settings](https://supervertaler.gitbook.io/help/settings/project-settings) | – (not yet used in UI) |
| `KeyboardShortcuts` | [/help/reference/keyboard-shortcuts](https://supervertaler.gitbook.io/help/reference/keyboard-shortcuts) | – (not yet used in UI) |
| `Troubleshooting` | [/help/reference/troubleshooting](https://supervertaler.gitbook.io/help/reference/troubleshooting) | – (not yet used in UI) |

## Other links in the plugin

These are hardcoded URLs outside `HelpSystem`, found in the About dialog and licence manager.

| Link | URL | Location |
|---|---|---|
| Documentation (home) | [supervertaler.gitbook.io/help](https://supervertaler.gitbook.io/help) | AboutDialog – "Documentation" link (`HelpSystem.OpenDocsHome()`) |
| Website | [supervertaler.com](https://supervertaler.com) | AboutDialog – "Website" link |
| Support & Community | [supervertaler.com/trados/#support](https://supervertaler.com/trados/#support) | AboutDialog – "Support & Community" link |
| Source code | [github.com/Supervertaler/Supervertaler-for-Trados](https://github.com/Supervertaler/Supervertaler-for-Trados) | AboutDialog – "Source Code" link |
| Changelog | [CHANGELOG.md on GitHub](https://github.com/Supervertaler/Supervertaler-for-Trados/blob/main/CHANGELOG.md) | Website trados page – nav bar + footer |
| Purchase page | supervertaler.com/trados/ | LicenseManager – shown in trial-expired / upgrade-required messages |

## Docs source

The documentation source files live in the standalone [`Supervertaler-Help`](https://github.com/Supervertaler/Supervertaler-Help) repo and are synced to GitBook from its `main` branch. The repo's root holds [`SUMMARY.md`](https://github.com/Supervertaler/Supervertaler-Help/blob/main/SUMMARY.md) plus `trados/`, `workbench/`, and `.gitbook/` subfolders.

## Slug-generation note

GitBook does NOT use the markdown filename for the URL slug. It uses the slugified `## 🧩 Section` header from `SUMMARY.md` as the path's first component, then a slugified version of the page itself. Identical section names across the Trados (🧩) and Workbench (🖥️) halves get a `-1` numeric suffix on the second occurrence (Trados gets the unsuffixed version; Workbench gets `-1`). For example: a Trados page under `## 🧩 Settings` becomes `/help/settings/...`, but a Workbench page under `## 🖥️ Settings` becomes `/help/settings-1/...`.

To regenerate the topic-to-slug map after a SUMMARY.md restructure, walk both `SUMMARY.md` (in document order) and the GitBook [sitemap-pages.xml](https://supervertaler.gitbook.io/help/sitemap-pages.xml) (also in document order). They line up 1:1, including the homepage entry. Same recipe used to regenerate the equivalent `Topics` enum on the Workbench side ([`modules/help_system.py`](https://github.com/Supervertaler/Supervertaler-Workbench/blob/main/modules/help_system.py)).

## Notes

- GitBook still serves legacy `trados/...`-style URLs via 301 redirects, so the previous topic values continued to resolve. The canonical slugs avoid the redirect hop and are robust to GitBook eventually pruning legacy redirects.
- The Settings dialog help button (`?` in title bar) and F1 key both call `GetCurrentHelpTopic()` which maps the active tab index to a topic.
- Many topics are defined for future use even though they're not currently referenced in the UI. Removing unused topics is OK – the constants are public consts so the linker will tell you what's still wired up.
