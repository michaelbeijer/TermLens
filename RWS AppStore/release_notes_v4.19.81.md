# RWS App Store Manager – v4.19.81.0

**Version number:** `4.19.81.0`
**Minimum studio version:** `18.0`
**Maximum studio version:** `18.9`
**Checksum:** `199b104e4762108875d205fd14e676236769b182f0fc9a1587a99b3b80e5587c`

---

## Changelog

### Fixed
- **At 150% Windows display scaling on the Settings → Prompts tab, the toolbar buttons "New", "Restore" and "Refresh" all clipped their last character ("Ne", "Restor", "Refres"), and the two system-prompt buttons below the right-hand pane ("Edit System Prompt" / "Reset to Default") were similarly clipped.** Cause: each button had a hard-coded `Width` (45 / 65 / 130 / 120 px) chosen to fit at 100% scaling – tight even there, and not enough text room left after the AutoScaleMode.Dpi pass at higher DPIs.
- Fix at [`PromptManagerPanel.cs`](src/Supervertaler.Trados/Controls/PromptManagerPanel.cs): switch every toolbar button (via the `CreateToolbarButton` helper) and the two system-prompt buttons to `AutoSize = true` with `AutoSizeMode.GrowAndShrink`. The previous explicit widths are kept as `MinimumSize` so very-short labels still get a comfortable click target. Position the "Reset to Default" button and the status label dynamically against their neighbours' measured `PreferredSize` / `Right` edges instead of fixed x coordinates, so wider buttons at high DPI don't push them on top of each other.

For the full changelog, see: https://github.com/Supervertaler/Supervertaler-for-Trados/releases