using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Supervertaler.Trados.Licensing;

namespace Supervertaler.Trados
{
    /// <summary>
    /// Base class for Ctrl+Alt+digit QuickLauncher shortcut slots.
    /// Each subclass maps a slot number (1–10) to a keyboard shortcut.
    /// The user assigns prompts to slots in Settings; pressing the shortcut
    /// runs the assigned prompt directly without opening the QuickLauncher menu.
    /// </summary>
    public abstract class QuickLauncherSlotActionBase : AbstractAction
    {
        protected abstract int Slot { get; }

        protected override void Execute()
        {
            if (!LicenseManager.Instance.HasTier2Access)
            {
                LicenseManager.ShowUpgradeMessage();
                return;
            }

            QuickLauncherSlotRunner.RunSlot(Slot);
        }
    }

    [Action("Supervertaler_QuickLauncherSlot1", typeof(EditorController),
        Name = "QuickLauncher: Run shortcut slot 1",
        Description = "Run the QuickLauncher prompt assigned to slot 1")]
    [Shortcut(Keys.Control | Keys.Alt | Keys.D1)]
    public class QuickLauncherSlot1Action : QuickLauncherSlotActionBase
    {
        protected override int Slot => 1;
    }

    [Action("Supervertaler_QuickLauncherSlot2", typeof(EditorController),
        Name = "QuickLauncher: Run shortcut slot 2",
        Description = "Run the QuickLauncher prompt assigned to slot 2")]
    [Shortcut(Keys.Control | Keys.Alt | Keys.D2)]
    public class QuickLauncherSlot2Action : QuickLauncherSlotActionBase
    {
        protected override int Slot => 2;
    }

    [Action("Supervertaler_QuickLauncherSlot3", typeof(EditorController),
        Name = "QuickLauncher: Run shortcut slot 3",
        Description = "Run the QuickLauncher prompt assigned to slot 3")]
    [Shortcut(Keys.Control | Keys.Alt | Keys.D3)]
    public class QuickLauncherSlot3Action : QuickLauncherSlotActionBase
    {
        protected override int Slot => 3;
    }

    [Action("Supervertaler_QuickLauncherSlot4", typeof(EditorController),
        Name = "QuickLauncher: Run shortcut slot 4",
        Description = "Run the QuickLauncher prompt assigned to slot 4")]
    [Shortcut(Keys.Control | Keys.Alt | Keys.D4)]
    public class QuickLauncherSlot4Action : QuickLauncherSlotActionBase
    {
        protected override int Slot => 4;
    }

    [Action("Supervertaler_QuickLauncherSlot5", typeof(EditorController),
        Name = "QuickLauncher: Run shortcut slot 5",
        Description = "Run the QuickLauncher prompt assigned to slot 5")]
    [Shortcut(Keys.Control | Keys.Alt | Keys.D5)]
    public class QuickLauncherSlot5Action : QuickLauncherSlotActionBase
    {
        protected override int Slot => 5;
    }

    [Action("Supervertaler_QuickLauncherSlot6", typeof(EditorController),
        Name = "QuickLauncher: Run shortcut slot 6",
        Description = "Run the QuickLauncher prompt assigned to slot 6")]
    [Shortcut(Keys.Control | Keys.Alt | Keys.D6)]
    public class QuickLauncherSlot6Action : QuickLauncherSlotActionBase
    {
        protected override int Slot => 6;
    }

    [Action("Supervertaler_QuickLauncherSlot7", typeof(EditorController),
        Name = "QuickLauncher: Run shortcut slot 7",
        Description = "Run the QuickLauncher prompt assigned to slot 7")]
    [Shortcut(Keys.Control | Keys.Alt | Keys.D7)]
    public class QuickLauncherSlot7Action : QuickLauncherSlotActionBase
    {
        protected override int Slot => 7;
    }

    [Action("Supervertaler_QuickLauncherSlot8", typeof(EditorController),
        Name = "QuickLauncher: Run shortcut slot 8",
        Description = "Run the QuickLauncher prompt assigned to slot 8")]
    [Shortcut(Keys.Control | Keys.Alt | Keys.D8)]
    public class QuickLauncherSlot8Action : QuickLauncherSlotActionBase
    {
        protected override int Slot => 8;
    }

    [Action("Supervertaler_QuickLauncherSlot9", typeof(EditorController),
        Name = "QuickLauncher: Run shortcut slot 9",
        Description = "Run the QuickLauncher prompt assigned to slot 9")]
    [Shortcut(Keys.Control | Keys.Alt | Keys.D9)]
    public class QuickLauncherSlot9Action : QuickLauncherSlotActionBase
    {
        protected override int Slot => 9;
    }

    [Action("Supervertaler_QuickLauncherSlot10", typeof(EditorController),
        Name = "QuickLauncher: Run shortcut slot 10",
        Description = "Run the QuickLauncher prompt assigned to slot 10")]
    [Shortcut(Keys.Control | Keys.Alt | Keys.D0)]
    public class QuickLauncherSlot10Action : QuickLauncherSlotActionBase
    {
        protected override int Slot => 10;
    }
}
