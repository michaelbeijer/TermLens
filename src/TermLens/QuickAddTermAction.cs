using System;
using System.IO;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using TermLens.Core;
using TermLens.Settings;

namespace TermLens
{
    /// <summary>
    /// Editor context menu action: "Quick add Term to TermLens".
    /// Appears in the right-click context menu and responds to Ctrl+Alt+Shift+T.
    /// Extracts selected source/target text and inserts the term directly,
    /// bypassing the AddTermDialog for faster workflow.
    /// </summary>
    [Action("TermLens_QuickAddTerm", typeof(EditorController),
        Name = "Quick add Term to TermLens",
        Description = "Quickly add the selected source/target text as a new term (no dialog)")]
    [ActionLayout(
        typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation), 10,
        DisplayType.Default, "", true)]
    [Shortcut(Keys.Control | Keys.Alt | Keys.Shift | Keys.T)]
    public class QuickAddTermAction : AbstractAction
    {
        protected override void Execute()
        {
            try
            {
                var editorController = SdlTradosStudio.Application.GetController<EditorController>();
                var doc = editorController?.ActiveDocument;
                if (doc == null)
                {
                    MessageBox.Show("No document is open.",
                        "TermLens", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var settings = TermLensSettings.Load();

                // Validate write termbase is configured
                if (settings.WriteTermbaseId < 0)
                {
                    MessageBox.Show(
                        "No write termbase is configured.\n\n" +
                        "Open TermLens settings (gear icon) and check the \u201cWrite\u201d column " +
                        "for the termbase where new terms should be added.",
                        "TermLens \u2014 Quick Add Term",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate termbase path
                if (string.IsNullOrEmpty(settings.TermbasePath) || !File.Exists(settings.TermbasePath))
                {
                    MessageBox.Show(
                        "Termbase file not found. Please check the TermLens settings.",
                        "TermLens \u2014 Quick Add Term",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get text from source and target segments
                string sourceText = "";
                string targetText = "";

                try
                {
                    // Try to get selected text first, fall back to full segment
                    if (doc.ActiveSegmentPair?.Source != null)
                        sourceText = doc.ActiveSegmentPair.Source.ToString() ?? "";
                    if (doc.ActiveSegmentPair?.Target != null)
                        targetText = doc.ActiveSegmentPair.Target.ToString() ?? "";

                    // If there is an active selection, prefer it
                    var selection = doc.Selection;
                    if (selection != null)
                    {
                        try
                        {
                            if (selection.Source != null)
                            {
                                var srcSel = selection.Source.ToString();
                                if (!string.IsNullOrWhiteSpace(srcSel))
                                    sourceText = srcSel;
                            }
                        }
                        catch { /* Selection may not be available */ }

                        try
                        {
                            if (selection.Target != null)
                            {
                                var tgtSel = selection.Target.ToString();
                                if (!string.IsNullOrWhiteSpace(tgtSel))
                                    targetText = tgtSel;
                            }
                        }
                        catch { /* Selection may not be available */ }
                    }
                }
                catch
                {
                    // Fall back to full segment text
                    if (doc.ActiveSegmentPair?.Source != null)
                        sourceText = doc.ActiveSegmentPair.Source.ToString() ?? "";
                    if (doc.ActiveSegmentPair?.Target != null)
                        targetText = doc.ActiveSegmentPair.Target.ToString() ?? "";
                }

                sourceText = sourceText.Trim();
                targetText = targetText.Trim();

                // Validate we have text to work with
                if (string.IsNullOrWhiteSpace(sourceText) || string.IsNullOrWhiteSpace(targetText))
                {
                    MessageBox.Show(
                        "Both source and target text are required.\n\n" +
                        "Make sure you have an active segment with text in both " +
                        "the source and target columns.",
                        "TermLens \u2014 Quick Add Term",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get write termbase metadata
                Models.TermbaseInfo writeTermbase = null;
                using (var reader = new TermbaseReader(settings.TermbasePath))
                {
                    if (reader.Open())
                        writeTermbase = reader.GetTermbaseById(settings.WriteTermbaseId);
                }

                if (writeTermbase == null)
                {
                    MessageBox.Show(
                        "The configured write termbase was not found in the database.\n" +
                        "Please check the TermLens settings.",
                        "TermLens \u2014 Quick Add Term",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Insert the term directly — no dialog
                try
                {
                    var newId = TermbaseReader.InsertTerm(
                        settings.TermbasePath,
                        settings.WriteTermbaseId,
                        sourceText,
                        targetText,
                        writeTermbase.SourceLang,
                        writeTermbase.TargetLang,
                        ""); // No definition for quick-add

                    if (newId > 0)
                    {
                        // Reload term index so the new term appears immediately
                        TermLensEditorViewPart.NotifyTermAdded();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Failed to add term: {ex.Message}\n\n" +
                        "The database may be locked by another application.",
                        "TermLens \u2014 Quick Add Term",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}",
                    "TermLens", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
