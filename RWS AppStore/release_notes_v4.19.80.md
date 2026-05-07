# RWS App Store Manager – v4.19.80.0

**Version number:** `4.19.80.0`
**Minimum studio version:** `18.0`
**Maximum studio version:** `18.9`
**Checksum:** `797f40b05ced71bc8a1a0a4134f75854d3da877772348b124a962b5e4b653f99`

---

## Changelog

### Added
- **The Supervertaler UI scale dropdown in Settings → General previously bottomed out at 100%, so users on hi-DPI machines who found Windows' global scaling too aggressive had no way to dial only the plugin back without changing system-wide settings.**
- Add 70%, 80%, 90% to the existing 100% / 110% / 125% / 150% options. Combined with the auto-detected Windows DPI as the base scale, this lets a user on a 4K monitor at 200% Windows scaling drop the plugin to 200% × 0.8 = 160% effective, etc., without affecting the rest of Trados or other apps.
- Floor stops at 70%: below that the system-rendered widgets (NumericUpDown spinners, checkbox boxes) become disproportionately large vs the text, producing weird-looking layouts. The underlying validation in `TermLensSettings.Load` already permits anything `> 0 && <= 3.0`, so the storage layer was always ready.

For the full changelog, see: https://github.com/Supervertaler/Supervertaler-for-Trados/releases