# RWS App Store Manager – v4.19.83.0

**Version number:** `4.19.83.0`
**Minimum studio version:** `18.0`
**Maximum studio version:** `18.9`
**Checksum:** `ff98ef4d506b40e3e5fef75fb05bb8ac242e1d921f4fb776f7798972d03d78f5`

---

## Changelog

### Fixed
- After the v4.19.79 small-A / big-A redesign, the bigger bold "A" on the right of the TermLens header strip was getting its bottom edge clipped at high Windows display scaling – the small regular "A" rendered fully, but the bold one didn't have enough vertical room inside the 28 px-tall header panel.
- Fix at [`TermLensControl.cs`](src/Supervertaler.Trados/Controls/TermLensControl.cs): bump header height 28 → 32, and trim the big-A font from 11 pt → 10 pt. The size delta vs the small 7 pt A is still clearly visible (and tooltips spell out increase / decrease either way), but the bold A now has comfortable clearance even at 150% scaling.

For the full changelog, see: https://github.com/Supervertaler/Supervertaler-for-Trados/releases