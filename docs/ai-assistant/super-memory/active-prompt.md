---
description: Per-project prompt that Quick Add appends terminology to
---

# Active Prompt

Each Trados project can have an **active prompt** -- the prompt that Quick Add appends terminology to. This is also the prompt that is auto-selected in the [Batch Translate](../../batch-translate.md) dropdown when you open the project.

## Setting the active prompt

1. Open **Settings → Prompts**
2. Right-click any prompt in the tree
3. Choose **Set as active prompt for this project**

The active prompt is shown with a pin icon and bold blue text in the Prompt Manager, and a checkmark appears next to its name in the Batch Translate dropdown. The Batch Translate dropdown updates **live** – you do not have to close the Settings dialog for the change to take effect. Cancelling the dialog reverts the change; clicking OK persists it.

To clear the active prompt, right-click it again and choose the same menu item (it toggles).

{% hint style="info" %}
The active prompt works for any prompt regardless of folder or category. Even if a prompt's `Category` is not `Translate` (for example, a prompt at the root of the tree with no category set), it will still appear and be pre-selected in the Batch Translate dropdown once you mark it as active.
{% endhint %}

{% hint style="info" %}
The active prompt is saved [per project](../../settings/project-settings.md). Different Trados projects can have different active prompts.
{% endhint %}

## See Also

* [Quick Add](quick-add.md)
* [Batch Translate](../../batch-translate.md)
* [Prompts](../../settings/prompts.md)
* [Per-Project Settings](../../settings/project-settings.md)
