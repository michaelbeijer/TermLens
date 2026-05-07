# RWS App Store Manager – v4.19.85.0

**Version number:** `4.19.85.0`
**Minimum studio version:** `18.0`
**Maximum studio version:** `18.9`
**Checksum:** `555faf406fada2b13f52f8662a347ebf1121a53008e70189765888f5245f9d05`

---

## Changelog

### Fixed
- **The "Source: …" preview at the top of the chat panel was showing raw Trados inline-formatting tags – e.g. `Source: "<cf bold=True>SEVT</cf> <cf size=…>` – instead of plain readable text.** The target preview already used `SegmentTagHandler.GetFinalText` (which returns the visible text only), but the source preview called `Source.ToString()` directly, which serialises the segment with all formatting markers.
- Fix at [`AiAssistantViewPart.UpdateContextDisplay`](src/Supervertaler.Trados/AiAssistantViewPart.cs): route the source through `SegmentTagHandler.GetFinalText` too.

For the full changelog, see: https://github.com/Supervertaler/Supervertaler-for-Trados/releases