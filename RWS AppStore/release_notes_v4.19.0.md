# RWS App Store Manager — v4.19.0.0

**Version number:** `4.19.0.0`
**Minimum studio version:** `18.0`
**Maximum studio version:** `18.9`
**Checksum:** `<to-be-filled-after-build>`

---

## Changelog

This is a significant release. The headline feature is **SuperMemory multi-bank support** — you can now keep several self-contained knowledge bases side by side (one per client, per domain, per language pair) and switch between them in a single click. SuperMemory is now consistently positioned as the brand name for the system, with "memory banks" as the containers within it. The release also rolls up all the improvements that have accumulated since v4.18.49 (Gemma 4 models, SuperSearch preview refinements, shorter panel names, proofreading tag fix, and a long list of polish items).

### Added — SuperMemory multi-bank support

- **Memory Bank dropdown** in the Supervertaler Assistant toolbar. Lists every bank under your user-data folder with the active one pre-selected. Switching is immediate: the next chat turn, batch translation and Process Inbox run all read from the new bank, and your chat history is preserved across the switch. The active bank is persisted in settings and survives Trados restarts.
- **Create new banks from the toolbar** — pick `+ New memory bank…` at the bottom of the dropdown, enter a short name (lowercase letters, digits, hyphens or underscores) with a live preview of the final folder name, and the bank is created on disk with the full seven-folder skeleton and activated in one click. No need to touch Settings or File Explorer.
- **Bundled template files** — every new bank ships with the canonical `compile.md`, `lint.md`, `query.md`, `translate_with_kb.md` and "Claude dump" helper templates in `06_TEMPLATES/`, so Process Inbox and Health Check work against a fresh bank out of the box.
- **Heal-on-activation prompt** — if you activate an older bank missing its canonical template files (a bank created before this release, or one where you accidentally deleted a template), the plugin shows a one-time dialog offering to restore them from the built-in defaults. Existing template files are never overwritten.
- **Shared with the Python Supervertaler Assistant** — memory banks live in the shared Supervertaler user-data folder with byte-identical layout, so a bank created in Trados works unchanged in the standalone app and vice versa. You can create one in either product and immediately use it in the other.
- **Legacy single-bank migration** — the first time you open this release against an older single-bank installation, the plugin offers to move your existing `memory-bank/` or `supermemory/` folder into the new `memory-banks/<name>/` layout automatically.

### Improved — SuperMemory workflow

- **Distill now archives source files dropped in the inbox.** If you drop a TMX, PDF or DOCX directly into a bank's `00_INBOX/` folder and run Distill on it, the source file is moved to `00_INBOX/_archive/` after a successful distill — mirroring how Process Inbox archives the Markdown files it compiles.
- **Process Inbox now recognises non-Markdown files in the inbox.** Instead of silently ignoring TMX, PDF or DOCX files dropped in `00_INBOX/`, the button lights up and clicking it shows a helpful message pointing you at Distill for binary files. Mixed inboxes (both `.md` and binary files) process the Markdown and warn about the rest.
- **Health Check shows progress instantly.** The feature used to scan the entire bank synchronously on the UI thread before adding any chat message, leaving the user staring at a frozen UI for several seconds on mature banks. The scan now runs on a background thread and a "Health Check — scanning memory bank …" bubble appears the moment you click the button.
- **Next-steps messages in Distill and Process Inbox summaries.** After a successful Distill, the chat summary now suggests running Process Inbox next and then Health Check; the Process Inbox summary suggests running Health Check afterwards. Obsidian review is positioned as the optional step, not the main one — the self-documenting workflow is now: Distill → Process Inbox → Health Check.

### Improved — chat panel

- **Chat context bar is now green.** The "Dutch (BE) → English (GB) | Source: …" line at the top of the Supervertaler Assistant panel is now rendered in forest green (Material Design Green 800) instead of medium grey, making the current language pair and source segment much easier to spot at a glance.

### Fixed — chat panel

- **Chat auto-scroll now behaves correctly on long chat histories.** A combination of WinForms layout quirks caused the chat panel to scroll into a vast area of ghost white space below the last message on long histories, forcing users to click **Clear** as a workaround before every operation. The plugin now manually manages the chat panel's scroll range based on the actual position of the last bubble. No more ghost white space, no more bouncing when scrolling to the bottom, no more disappearing messages after clicking Send.
- **"Thinking…" bubble no longer bounces the chat.** The animation timer used to re-scroll the chat every 2 seconds, yanking the user back every time they tried to scroll up during a long operation. The per-tick re-scroll has been removed.
- **User scroll is respected during long operations.** If you scroll up to read older content while Health Check or Distill is running, the chat stays where you put it. Scroll back to the bottom and auto-scroll re-engages automatically.
- **Process Inbox button is correctly disabled when the inbox is empty.** Previously the button was unconditionally re-enabled after every long-running operation (Health Check, Distill, AutoPrompt) even when the inbox had no files, leading to a dead-end click. The toolbar now tracks the last known inbox count and respects it.
- **Reports tab label for Process Inbox** now reads "SuperMemory: Process Inbox" instead of the legacy internal name "SuperMemory: Compile", matching the toolbar button label.

### Changed

- **SuperMemory is the brand name; memory banks are the containers.** The two-level terminology (similar to how Gmail uses "Gmail" for the product and "inbox" for the container, or Obsidian uses "Obsidian" for the product and "vault" for the container) is now reflected consistently. Chat banners, Reports tab labels, and the help menu use "SuperMemory:" as the system prefix. The toolbar dropdown and the "+ New memory bank…" sentinel use "memory bank" for the individual container.
- **SuperMemory help pages reorganised** around a "one canonical home per concept" principle. `context-awareness.md` is the single authoritative menu of context sources with memory banks as one section among several; `memory-banks.md` is the noun page covering what SuperMemory is, what memory banks are, how to create and switch them, and how they sync with the Python assistant; `memory-banks/ai-integration.md` is the power-user deep dive on the loading algorithm and token budget.
- **Tooltip text** on Process Inbox, Health Check and Distill clarified to refer to "the active memory bank" rather than "your SuperMemory" generically.
- **Quick Add dialog title** changed from "Add to SuperMemory" to "Quick Add to memory bank".

### Also included (since v4.18.49)

All the improvements from v4.18.50 through v4.18.57 are rolled up into this release:

- **Shorter panel names** — docking tabs and ribbon buttons show "TermLens" and "SuperSearch" instead of "Supervertaler TermLens" and "Supervertaler SuperSearch".
- **SuperSearch improvements** — resizable preview pane (draggable splitter), visible source/target divider, highlight rendering fix (no more "documentsare" collisions), preview pane click reliability, header label clipping fix, match truncation fix (matches no longer show "Da..." instead of "Dawn").
- **SuperSearch screencast** embedded at the top of the SuperSearch help page.
- **Gemma 4 models** — Google's Gemma 4 31B and Gemma 4 26B MoE added to both the Gemini provider and OpenRouter routes.
- **Proofreading false positives for inline tags** — source and target now use the same plain-text extraction so tag markup never reaches the AI proofreader.

---

For the full changelog, see: https://github.com/Supervertaler/Supervertaler-for-Trados/releases
