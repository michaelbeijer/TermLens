# RWS App Store Manager — v4.18.12.0

**Version number:** `4.18.12.0`
**Minimum studio version:** `18.0`
**Maximum studio version:** `18.9`
**Checksum:** `c2a807f2da41b926e258050ff237374d40193667d6476ef2ab1413fc0e5a2972`

---

## Changelog

### Added
- **MultiTerm termbases in AI Settings** — MultiTerm termbases now appear in the "Termbases included in AI prompts" checklist on the AI Settings tab, matching what's shown on the TermLens tab
- **Unified prompt library schema** — prompts now use a consistent YAML frontmatter format (`category`, `app`, `built_in`) shared between Supervertaler Workbench and Supervertaler for Trados
- **App-specific prompt filtering** — prompts tagged `app: "workbench"` are hidden in Trados; prompts tagged `app: "trados"` are hidden in Workbench; `app: "both"` (default) shows everywhere
- **App dropdown in prompt editor** — new "App" field lets you choose whether a prompt is for Both, Trados only, or Workbench only
- **Variable insertion menu reorganised** — Ctrl+, now groups variables into Common and Trados-specific sections
- **MultiTerm diagnostic logging** — loading failures are now logged to `%LocalAppData%\Supervertaler.Trados\multiterm_debug.log` instead of being silently swallowed
- **Markdown rendering in TermLens popup** — Notes and Definition fields now render Markdown formatting (tables, bold, italic, headings, bullet lists, code blocks) instead of plain text
- **Resizable TermLens popup** — drag the bottom-right corner grip to resize the popup; width is remembered for the rest of the session
- **Copy raw Markdown from AI Assistant** — right-click → Copy on a chat bubble now copies the original Markdown (preserving tables and formatting) instead of stripped plain text

### Changed
- **Clearer expired trial message** – both panels now say "Click the ⚙ button above" instead of the vague "Enter a license key in Settings → License"
- **Feature renamed: AutoPrompt** — "Analyse Project & Generate Prompt" is now called **AutoPrompt** throughout the UI, docs, and log labels
- **TermScan** naming consistent in docs — the automatic glossary extraction step is consistently referred to as TermScan
- **Prompt YAML keys standardised** — `domain` → `category`, `sv_quickmenu`/`quick_run` → `quickmenu`, `quicklauncher_label` → `quickmenu_label`; legacy keys are still accepted for backward compatibility
- **Prompt library cleaned up** — removed duplicate folders and files, fixed malformed YAML frontmatter, standardised variable names (`{{SOURCE_TEXT}}` → `{{SOURCE_SEGMENT}}`)
- **Wider TermLens popup** — default maximum width increased from 500px to 700px so tables display more clearly
- **AI Assistant uses proper Markdown tables** — system prompt now instructs the AI to use valid Markdown table syntax with pipe delimiters and separator rows
- **User data folder restructured** — Trados settings files (`settings.json`, `license.json`, `chat_history.json`) now live under `trados/settings/` instead of directly in `trados/`; auto-migrated on first run

### Fixed
- **Settings accessible when trial expires** – the gear button on both the TermLens and Supervertaler Assistant panels now opens the Settings dialog even when the trial has expired or no licence is active, so users can enter a licence key
- **AI Assistant gear button visible above overlay** – the settings and help buttons are no longer hidden behind the licence overlay
- **In-plugin purchase links now use live checkout** — the "Buy" links in Settings → License were still pointing to test mode URLs

For the full changelog, see: https://github.com/Supervertaler/Supervertaler-for-Trados/releases