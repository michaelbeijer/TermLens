using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Supervertaler.Trados.Licensing;

namespace Supervertaler.Trados
{
    /// <summary>
    /// Keyboard action: Ctrl+Shift+P opens the Term Picker dialog.
    /// Lists all matched terms for the current segment and lets the user
    /// select one to insert into the target segment.
    /// (Ctrl-tap and Ctrl+Alt+G now belong to the floating TermLens popup —
    /// see TermLensPopupAction. Was Ctrl+Shift+L until v4.19.35 and
    /// Ctrl+Shift+T in v4.19.36; both collide with Trados Studio's own
    /// shortcuts.)
    /// No context menu entry — keyboard-only.
    /// </summary>
    [Action("TermLens_TermPicker", typeof(EditorController),
        Name = "TermLens: Pick term to insert",
        Description = "Open a dialog to browse and insert matched terms")]
    [Shortcut(Keys.Control | Keys.Shift | Keys.P)]
    public class TermPickerAction : AbstractAction
    {
        protected override void Execute()
        {
            if (!LicenseManager.Instance.HasTier1Access)
            {
                LicenseManager.ShowLicenseRequiredMessage();
                return;
            }

            TermLensEditorViewPart.HandleTermPicker();
        }
    }
}
