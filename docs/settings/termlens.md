# TermLens Settings

Configure how TermLens loads and displays terminology in Trados Studio.

## Accessing TermLens settings

Click the **gear icon** in the TermLens panel, or open the plugin **Settings** dialog and switch to the **TermLens** tab.

## Database path

The path to your Supervertaler termbase `.db` file. Click **Browse** to select a database, or **Create New** to start with an empty one.

{% hint style="info" %}
**Auto-detect:** If Supervertaler Workbench is installed on the same machine, the plugin can automatically detect its default database location. Click **Auto-detect** to find and use it.
{% endhint %}

## Termbase toggles

Each Supervertaler termbase in the database has three toggles. See [Termbase Management](../termbase-management.md) for full details.

| Toggle | Purpose |
|--------|---------|
| **Read** | Load terms for matching –only termbases with Read enabled appear in TermLens |
| **Write** | Receive new terms added via the [quick-add shortcuts](../termlens/adding-terms.md) |
| **Project** | Mark as the project termbase (shown in pink, prioritised) |

## MultiTerm termbases

If your Trados project has MultiTerm termbases (`.sdltb` files) attached, they appear at the bottom of the termbase list with a **[MultiTerm]** label and a light green row background. The **Read** toggle controls visibility in TermLens; **Write** and **Project** are always disabled because MultiTerm termbases are read-only.

To add or remove MultiTerm termbases, use Trados Studio's **Project Settings > Language Pairs > Termbases**. See [MultiTerm Support](../multiterm-support.md) for full details.

## Auto-load on startup

When enabled, the plugin automatically loads the termbase database when Trados Studio opens. This means terms are available immediately when you start translating, without needing to open the settings first.

If disabled, the termbase loads the first time you open the TermLens settings or click the TermLens panel.

## Panel font size

Adjust the font size used in the TermLens display panel. Valid range: **7 pt** to **16 pt**.

Increase the font size if TermLens text is hard to read; decrease it to fit more terms on screen.

## Term shortcuts

Choose how Alt+digit shortcuts work when a segment has more than 9 matched terms:

* **Sequential** (default) — type the term number digit by digit. Alt+45 inserts term 45. Badges show clean sequential numbers (10, 11, 12, ...). There is a brief delay after each digit while the system waits for a possible next digit.
* **Repeated digit** — press the same digit key multiple times. Alt+55 inserts term 14 (the 5th term in the second tier). Badges show repeated digits (11, 22, 333, ...). No delay, but the badges are less intuitive.

Both modes behave identically when a segment has 9 or fewer matches — pressing Alt+N inserts immediately with no delay.

## Shortcut delay

Controls how long the system waits for the next digit in **Sequential** mode (in milliseconds). Default: **1100 ms**. Valid range: **300 ms** to **3000 ms**.

Increase the delay if you need more time between keystrokes. Decrease it if you find the pause too long when inserting single-digit terms in segments with 10+ matches. This setting has no effect in Repeated digit mode.

See [Keyboard Shortcuts](../keyboard-shortcuts.md) for the full reference.

## Export and import settings

Use the **Export Settings** and **Import Settings** buttons in the **Backup** tab of the Settings dialog to back up and restore your Supervertaler configuration.

### Export

Click **Export Settings...** to save a copy of your current settings to a JSON file. Choose a location and filename — the default is `supervertaler-settings.json`. This file contains all your plugin settings: termbase paths, toggle states, font size, shortcut preferences, AI provider keys, model selections, and prompt configuration.

{% hint style="info" %}
**Tip:** Export your settings before upgrading the plugin or switching machines, so you can quickly restore your setup.
{% endhint %}

### Import

Click **Import Settings...** to restore settings from a previously exported JSON file. The import process:

1. Validates that the selected file is a valid Supervertaler settings file
2. Creates an automatic backup of your current settings (`settings.backup.json`)
3. Replaces your current settings with the imported ones
4. Closes the Settings dialog and applies the new settings immediately

{% hint style="warning" %}
Importing settings replaces **all** current settings. Your previous settings are automatically backed up in case you need to revert.
{% endhint %}

### Settings file location

Your settings are stored at:

```
%LocalAppData%\Supervertaler.Trados\settings.json
```

You can also manually back up or edit this file. After an import, the previous settings are saved as `settings.backup.json` in the same folder.

---

## See Also

- [Termbase Management](../termbase-management.md)
- [MultiTerm Support](../multiterm-support.md)
- [AI Settings](ai-settings.md)
- [TermLens (Workbench)](https://supervertaler.gitbook.io/supervertaler/glossaries/termlens)
