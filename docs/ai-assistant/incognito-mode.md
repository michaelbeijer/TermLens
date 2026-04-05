# Incognito Mode

{% hint style="info" %}
You are viewing help for **Supervertaler for Trados** -- the Trados Studio plugin. Looking for help with the standalone app? Visit [Supervertaler Workbench help](https://help.supervertaler.com).
{% endhint %}

Incognito Mode tells the AI to **anonymise all personal and project data** in its responses. When enabled, project names, file paths, TM names, user names, and other identifying information are automatically replaced with plausible placeholders -- so you can share your screen, record videos, or post screenshots without worrying about exposing confidential client data.

🕵️ Think of it as a privacy filter for your AI chat.

## When to Use It

| Scenario | Example |
| -------- | ------- |
| **Screen sharing** | Presenting to colleagues or in a webinar while working on a real project |
| **Recording demos** | Making tutorial videos that show real workflows without real client names |
| **Forum posts** | Sharing a helpful AI response in a community without revealing client details |
| **Client presentations** | Showing how the tool works without exposing other clients' data |
| **Training** | Onboarding new team members on live projects |

## How It Works

When Incognito Mode is enabled, the AI receives an instruction to replace all identifying data with anonymised equivalents. For example:

| Real data | Anonymised |
| --------- | ---------- |
| Acme Corporation | Client Alpha |
| D:\Jobs\ACME\Q1_report.docx | D:\Projects\Client Alpha\document.docx |
| Jane Smith | User A |
| ACME_NL-EN.sdltm | Client_Alpha_NL-EN.sdltm |

The AI uses **consistent replacements** within a conversation, so if "Acme Corporation" becomes "Client Alpha" in one response, it stays "Client Alpha" throughout the session.

### What is NOT anonymised

Some data is left untouched because it carries no identifying information:

* Language codes (en-GB, nl-NL, de-DE, etc.)
* Segment counts, word counts, and statistics
* Translation status values (draft, translated, approved, etc.)
* The actual source and target text you are translating
* Technical identifiers (tool names, status labels)

## Enabling Incognito Mode

1. Open **Settings** (gear icon in the Assistant toolbar)
2. Go to the **AI Settings** tab
3. Scroll down to **AI context (Chat and QuickLauncher)**
4. Tick **Incognito mode**
5. Click **OK**

The setting takes effect immediately on your next message. Toggle it off when you no longer need anonymisation.

{% hint style="success" %}
**Tip:** The setting persists across Trados sessions, so remember to toggle it off when you are done sharing. You do not want anonymised data in your regular workflow -- it can make the AI's answers less specific.
{% endhint %}

{% hint style="info" %}
Incognito Mode works with all AI providers, not just Claude. The anonymisation instructions are included in the system prompt that every provider receives.
{% endhint %}

## Limitations

* Incognito Mode instructs the AI to anonymise data in its **responses**. It does not prevent data from being sent to the AI provider -- your source text, TM matches, and terminology are still included in the prompt as usual. If you need to prevent data from being sent entirely, disable those context options individually in AI Settings.
* The AI does its best to catch all identifying information, but it cannot guarantee 100% coverage. Always review responses before sharing publicly.
* [Studio Tools](studio-tools.md) results (project lists, TM searches, etc.) are also anonymised -- the AI receives the real data from the tools but presents it with placeholder names.

## See Also

* [Supervertaler Assistant](../ai-assistant.md) -- Overview
* [AI Settings](../settings/ai-settings.md) -- Configure context options and Incognito Mode
* [Context Awareness](context-awareness.md) -- What context is sent to the AI
