# RWS App Store Manager — v4.19.20.0

**Version number:** `4.19.20.0`
**Minimum studio version:** `18.0`
**Maximum studio version:** `18.9`

This release covers everything since v4.19.13.0. Headline changes: Claude Opus 4.7 support, two fixes to the Prompt Manager ↔ Batch Translate sync (new prompts now appear in the dropdown, and "Set as active prompt for this project" takes effect live), and corrected cost estimates for Claude Opus 4.6 and Haiku 4.5.

---

## Changelog

### Added

- **Claude Opus 4.7 support.** Anthropic's new flagship model (released 2026-04-16) is now selectable in AI Settings under the Claude provider and via the OpenRouter gateway (`anthropic/claude-opus-4.7`). Opus 4.7 has a 1M-token context window, 128k max output, and is Anthropic's most capable generally available model. Pricing is $5 / input MTok, $25 / output MTok — the same as Opus 4.6. Sonnet 4.6 remains the recommended default for most translation work; reach for Opus 4.7 when you need top-tier reasoning or long-context jobs.

### Fixed

- **"Set as active prompt for this project" now takes effect immediately in the Batch Translate dropdown and works for any prompt.** Right-clicking a prompt in the Prompt Manager and choosing "Set as active prompt for this project" used to have no visible effect on the Batch Translate dropdown until the Settings dialog was closed — and even then, prompts whose `Category` was not `Translate` (or `Proofread`, in proofread mode) were silently filtered out of the dropdown. The Batch dropdown now live-refreshes the moment the active prompt is toggled, without closing the Settings dialog, and the active prompt always appears regardless of its folder or category. The fix works from all three Settings entry points (AI Assistant gear, TermLens gear, About dialog). Cancelling the Settings dialog still reverts the change; clicking OK persists it.
- **New prompts created at the tree root in the Prompt Manager are now visible in the Batch Translate dropdown.** Creating a new prompt without first selecting a folder used to leave its `Category` empty, and the Batch dropdown filter would silently exclude it. New prompts now default to the `Translate` category when no folder is selected. Existing root-level prompts with empty categories still need to be moved into the `Translate` folder (or re-categorised in the editor) to become visible.
- **Corrected stale pricing for Claude Opus 4.6 and Haiku 4.5.** The internal pricing table had Opus 4.6 at the pre-4.6 rate of $15 / $75 per MTok — Anthropic dropped Opus pricing to $5 / $25 with the 4.6 release. Haiku 4.5 was listed at $0.80 / $4.00, corrected to the current $1.00 / $5.00. Cost estimates shown in the AI Assistant and Batch Translate were over-stating Opus usage and under-stating Haiku usage — now accurate.

### Note on Opus 4.7 tokenizer

- Claude Opus 4.7 uses a new tokenizer that can use **~1.0×–1.35× more tokens** for the same text compared to earlier models. Pre-send cost estimates (`chars / 4` heuristic) will under-estimate Opus 4.7 costs by a similar margin. Actual billing is based on Anthropic's token counts.

For the full changelog, see: https://github.com/Supervertaler/Supervertaler-for-Trados/blob/main/CHANGELOG.md
