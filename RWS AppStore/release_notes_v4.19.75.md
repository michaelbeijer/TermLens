# RWS App Store Manager – v4.19.75.0

**Version number:** `4.19.75.0`
**Minimum studio version:** `18.0`
**Maximum studio version:** `18.9`
**Checksum:** `c6d564e51927a089a0b1d23c6fe488604cebebb8b82221c61edbc8ba2be415d4`

---

## Changelog

### Changed
- **The `?` help buttons throughout the plugin used to open URLs like `https://supervertaler.gitbook.io/help/trados/termlens`, which GitBook then 301-redirected to the actual published slug `https://supervertaler.gitbook.io/help/features/termlens`.** Two problems with relying on the redirect: (a) URL fragments such as `#reports-tab` are preserved across redirects only because of browser-side fragment carrying (HTTP spec behaviour), not GitBook itself; (b) GitBook may eventually prune legacy redirect entries, especially after repo migrations like the recent docs split into the standalone `Supervertaler-Help` repo.
- Fix at [`HelpSystem.cs`](src/Supervertaler.Trados/Core/HelpSystem.cs): every `Topics.*` constant now holds the canonical slug GitBook actually publishes. The slug is generated from each `## 🧩 Section` header in [`SUMMARY.md`](https://github.com/Supervertaler/Supervertaler-Help/blob/main/SUMMARY.md), so `Overview` becomes `get-started/trados`, `TermLensPanel` becomes `features/termlens`, `BatchTranslate` becomes `features/batch-operations/batch-translate`, and so on. Verified against the live sitemap (`sitemap-pages.xml`): every constant now resolves with HTTP 200 and zero redirects.
- Mirror of the equivalent fix done earlier today on the Workbench side ([`modules/help_system.py`](https://github.com/Supervertaler/Supervertaler-Workbench/blob/main/modules/help_system.py)). Also updates the audit table in [`HELP-LINKS.md`](HELP-LINKS.md) at the repo root – previously last audited 2026-03-13 with the legacy `gitbook.io/trados` base URL.

For the full changelog, see: https://github.com/Supervertaler/Supervertaler-for-Trados/releases