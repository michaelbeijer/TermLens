using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Supervertaler.Trados.Models;
using Supervertaler.Trados.Settings;

namespace Supervertaler.Trados.Core
{
    /// <summary>
    /// Manages the prompt template library: loading, saving, and built-in prompt seeding.
    /// Prompts are stored as .svprompt files in the shared UserDataPath.PromptLibraryDir,
    /// which is the same folder Supervertaler Workbench uses — so prompts are automatically
    /// shared between both products.
    /// </summary>
    public class PromptLibrary
    {
        private static string PromptsDir => UserDataPath.PromptLibraryDir;

        /// <summary>
        /// Full path to the prompts folder on disk.
        /// </summary>
        public static string PromptsFolderPath => UserDataPath.PromptLibraryDir;

        private List<PromptTemplate> _cache;

        /// <summary>
        /// Returns all prompts from the library (cached; call Refresh() to reload).
        /// </summary>
        public List<PromptTemplate> GetAllPrompts()
        {
            if (_cache == null)
                Refresh();
            return _cache;
        }

        /// <summary>
        /// Reloads all prompts from the shared prompt_library folder.
        /// Both Workbench and this plugin read from the same location, so there is
        /// no longer a separate "desktop prompts" scan.
        /// </summary>
        public void Refresh()
        {
            _cache = new List<PromptTemplate>();

            if (Directory.Exists(PromptsDir))
                ScanDirectory(PromptsDir, PromptsDir, isReadOnly: false);
        }

        /// <summary>
        /// Finds a prompt by its relative path. Returns null if not found.
        /// </summary>
        public PromptTemplate GetPromptByRelativePath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return null;

            foreach (var p in GetAllPrompts())
            {
                if (string.Equals(p.RelativePath, relativePath, StringComparison.OrdinalIgnoreCase))
                    return p;
            }
            return null;
        }

        /// <summary>
        /// Applies variable substitution to prompt content.
        /// Supports both {{UPPERCASE}} and {lowercase} placeholder formats.
        /// </summary>
        public static string ApplyVariables(string content, string sourceLang, string targetLang)
        {
            return ApplyVariables(content, sourceLang, targetLang, null, null, null);
        }

        /// <summary>
        /// Applies variable substitution to prompt content, including segment-level variables.
        /// Supports both {{UPPERCASE}} and {lowercase} placeholder formats.
        /// </summary>
        public static string ApplyVariables(string content, string sourceLang, string targetLang,
            string sourceText, string targetText, string selection)
        {
            return ApplyVariables(content, sourceLang, targetLang,
                sourceText, targetText, selection,
                null, null, null, null);
        }

        /// <summary>
        /// Applies variable substitution to prompt content, including all segment-level
        /// and project-level variables.
        /// Supports both {{UPPERCASE}} and {lowercase} placeholder formats.
        /// </summary>
        public static string ApplyVariables(string content, string sourceLang, string targetLang,
            string sourceText, string targetText, string selection,
            string projectName, string documentName, string surroundingSegments, string projectText,
            string tmMatches = null)
        {
            if (string.IsNullOrEmpty(content))
                return content;

            // {{UPPERCASE}} format (Python Supervertaler / Workbench standard)
            content = content.Replace("{{SOURCE_LANGUAGE}}", sourceLang ?? "");
            content = content.Replace("{{TARGET_LANGUAGE}}", targetLang ?? "");

            // Canonical segment variable names
            content = content.Replace("{{SOURCE_SEGMENT}}", sourceText ?? "");
            content = content.Replace("{{TARGET_SEGMENT}}", targetText ?? "");

            // Legacy aliases — kept for backward compatibility
            content = content.Replace("{{SOURCE_TEXT}}", sourceText ?? "");
            content = content.Replace("{{TARGET_TEXT}}", targetText ?? "");

            content = content.Replace("{{SELECTION}}", selection ?? "");

            // Project / document variables
            content = content.Replace("{{PROJECT_NAME}}", projectName ?? "");
            content = content.Replace("{{DOCUMENT_NAME}}", documentName ?? "");
            content = content.Replace("{{SURROUNDING_SEGMENTS}}", surroundingSegments ?? "");
            content = content.Replace("{{PROJECT}}", projectText ?? "");

            // TM matches
            content = content.Replace("{{TM_MATCHES}}", tmMatches ?? "");

            // {lowercase} format (legacy compatibility with Python domain prompts)
            content = content.Replace("{source_lang}", sourceLang ?? "");
            content = content.Replace("{target_lang}", targetLang ?? "");

            return content;
        }

        /// <summary>
        /// Formats a list of TM matches into a human-readable string for prompt injection.
        /// Only includes matches at or above the specified minimum percentage.
        /// </summary>
        public static string FormatTmMatches(List<TmMatch> matches, int minPercent = 70)
        {
            if (matches == null || matches.Count == 0)
                return "(no fuzzy matches above " + minPercent + "%)";

            var filtered = matches.Where(m => m.MatchPercentage >= minPercent).ToList();
            if (filtered.Count == 0)
                return "(no fuzzy matches above " + minPercent + "%)";

            var sb = new StringBuilder();
            foreach (var m in filtered.OrderByDescending(m => m.MatchPercentage))
            {
                if (sb.Length > 0) sb.AppendLine();
                sb.AppendLine($"- {m.MatchPercentage}% match{(string.IsNullOrEmpty(m.TmName) ? "" : " (" + m.TmName + ")")}:");
                sb.AppendLine($"  Source: {m.SourceText}");
                sb.Append($"  Target: {m.TargetText}");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns all prompts that should appear in the QuickLauncher right-click menu.
        /// </summary>
        public List<PromptTemplate> GetQuickLauncherPrompts()
        {
            var all = GetAllPrompts();
            var result = new List<PromptTemplate>();
            foreach (var p in all)
            {
                if (p.IsQuickLauncher)
                    result.Add(p);
            }
            return result;
        }

        /// <summary>
        /// Saves a prompt template to disk as a Markdown file with YAML frontmatter.
        /// Creates directories as needed.
        /// </summary>
        public void SavePrompt(PromptTemplate prompt)
        {
            if (prompt == null || string.IsNullOrEmpty(prompt.Name))
                return;

            // Determine file path
            string filePath;
            if (!string.IsNullOrEmpty(prompt.FilePath) && !prompt.IsReadOnly)
            {
                filePath = prompt.FilePath;
            }
            else
            {
                // New prompt: build path from domain + name
                var folder = string.IsNullOrEmpty(prompt.Domain)
                    ? PromptsDir
                    : Path.Combine(PromptsDir, SanitizeFileName(prompt.Domain));
                Directory.CreateDirectory(folder);
                filePath = Path.Combine(folder, SanitizeFileName(prompt.Name) + ".svprompt");
            }

            var sb = new StringBuilder();
            sb.AppendLine("---");
            sb.AppendLine("name: \"" + EscapeYamlString(prompt.Name) + "\"");
            if (!string.IsNullOrEmpty(prompt.Description))
                sb.AppendLine("description: \"" + EscapeYamlString(prompt.Description) + "\"");
            if (!string.IsNullOrEmpty(prompt.Domain))
                sb.AppendLine("category: \"" + EscapeYamlString(prompt.Domain) + "\"");
            if (prompt.IsBuiltIn)
                sb.AppendLine("built_in: true");
            sb.AppendLine("---");
            sb.AppendLine();
            sb.Append(prompt.Content ?? "");

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);

            prompt.FilePath = filePath;
            prompt.RelativePath = GetRelativePath(filePath, PromptsDir);

            Refresh();
        }

        /// <summary>
        /// Deletes a prompt file from disk.
        /// </summary>
        public void DeletePrompt(PromptTemplate prompt)
        {
            if (prompt == null || prompt.IsReadOnly || string.IsNullOrEmpty(prompt.FilePath))
                return;

            if (File.Exists(prompt.FilePath))
                File.Delete(prompt.FilePath);

            Refresh();
        }

        /// <summary>
        /// Ensures built-in prompts exist in the prompts directory.
        /// Creates any that are missing (idempotent — safe to call on every startup).
        /// Also removes domain-specific translate prompts that were shipped in v4.12.x
        /// but replaced by the single Default Translation Prompt in v4.13.0.
        /// </summary>
        public void EnsureBuiltInPrompts()
        {
            Directory.CreateDirectory(PromptsDir);

            // Clean up domain-specific translate prompts removed in v4.13.0
            CleanUpRetiredPrompts();

            foreach (var builtin in GetBuiltInPromptDefinitions())
            {
                var folder = string.IsNullOrEmpty(builtin.Domain)
                    ? PromptsDir
                    : Path.Combine(PromptsDir, builtin.Domain);
                Directory.CreateDirectory(folder);

                var filePath = Path.Combine(folder, SanitizeFileName(builtin.Name) + ".svprompt");
                if (!File.Exists(filePath))
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("---");
                    sb.AppendLine("name: \"" + EscapeYamlString(builtin.Name) + "\"");
                    if (!string.IsNullOrEmpty(builtin.Description))
                        sb.AppendLine("description: \"" + EscapeYamlString(builtin.Description) + "\"");
                    if (!string.IsNullOrEmpty(builtin.Domain))
                        sb.AppendLine("domain: \"" + EscapeYamlString(builtin.Domain) + "\"");
                    sb.AppendLine("built_in: true");
                    sb.AppendLine("---");
                    sb.AppendLine();
                    sb.Append(builtin.Content);

                    File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                }
            }

            // Invalidate cache so next GetAllPrompts() reloads
            _cache = null;
        }

        /// <summary>
        /// Removes domain-specific translate prompts that were shipped in v4.12.x.
        /// Only deletes files that still contain "built_in: true" — user-modified copies are left alone.
        /// </summary>
        private void CleanUpRetiredPrompts()
        {
            var retiredNames = new[]
            {
                "Medical Translation Specialist",
                "Legal Translation Specialist",
                "Patent Translation Specialist",
                "Financial Translation Specialist",
                "Technical Translation Specialist",
                "Marketing & Creative Specialist",
                "IT & Software Localization Specialist",
                "Professional Tone & Style",
                "Preserve Formatting & Layout"
            };

            var translateDir = Path.Combine(PromptsDir, "Translate");
            if (!Directory.Exists(translateDir)) return;

            foreach (var name in retiredNames)
            {
                var filePath = Path.Combine(translateDir, SanitizeFileName(name) + ".svprompt");
                if (File.Exists(filePath))
                {
                    try
                    {
                        var content = File.ReadAllText(filePath);
                        if (content.Contains("built_in: true"))
                        {
                            File.Delete(filePath);
                        }
                    }
                    catch { /* ignore — file locked or permissions */ }
                }
            }
        }

        /// <summary>
        /// Restores all built-in prompts (overwrites any user edits).
        /// </summary>
        public void RestoreBuiltInPrompts()
        {
            foreach (var builtin in GetBuiltInPromptDefinitions())
            {
                var folder = string.IsNullOrEmpty(builtin.Domain)
                    ? PromptsDir
                    : Path.Combine(PromptsDir, builtin.Domain);
                Directory.CreateDirectory(folder);

                var filePath = Path.Combine(folder, SanitizeFileName(builtin.Name) + ".svprompt");

                var sb = new StringBuilder();
                sb.AppendLine("---");
                sb.AppendLine("name: \"" + EscapeYamlString(builtin.Name) + "\"");
                if (!string.IsNullOrEmpty(builtin.Description))
                    sb.AppendLine("description: \"" + EscapeYamlString(builtin.Description) + "\"");
                if (!string.IsNullOrEmpty(builtin.Domain))
                    sb.AppendLine("domain: \"" + EscapeYamlString(builtin.Domain) + "\"");
                sb.AppendLine("built_in: true");
                sb.AppendLine("---");
                sb.AppendLine();
                sb.Append(builtin.Content);

                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
            }

            _cache = null;
        }

        // ─── Private Methods ─────────────────────────────────────────

        private void ScanDirectory(string dir, string rootDir, bool isReadOnly)
        {
            try
            {
                var seenNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                // Scan .svprompt files first (preferred format)
                foreach (var file in Directory.GetFiles(dir, "*.svprompt", SearchOption.AllDirectories))
                {
                    try
                    {
                        var prompt = ParsePromptFile(file, rootDir);
                        if (prompt != null)
                        {
                            prompt.IsReadOnly = isReadOnly;
                            _cache.Add(prompt);
                            seenNames.Add(prompt.Name);
                        }
                    }
                    catch
                    {
                        // Skip files that can't be parsed
                    }
                }

                // Also scan .md files (legacy format) — skip if .svprompt version exists
                foreach (var file in Directory.GetFiles(dir, "*.md", SearchOption.AllDirectories))
                {
                    try
                    {
                        var prompt = ParsePromptFile(file, rootDir);
                        if (prompt != null && !seenNames.Contains(prompt.Name))
                        {
                            prompt.IsReadOnly = isReadOnly;
                            _cache.Add(prompt);
                        }
                    }
                    catch
                    {
                        // Skip files that can't be parsed
                    }
                }
            }
            catch
            {
                // Skip directories that can't be accessed
            }
        }

        /// <summary>
        /// Parses a Markdown file with optional YAML frontmatter.
        /// YAML is parsed as simple key: "value" pairs (no external library needed).
        /// </summary>
        private PromptTemplate ParsePromptFile(string filePath, string rootDir)
        {
            var text = File.ReadAllText(filePath, Encoding.UTF8);
            if (string.IsNullOrWhiteSpace(text))
                return null;

            var prompt = new PromptTemplate
            {
                FilePath = filePath,
                RelativePath = GetRelativePath(filePath, rootDir)
            };

            // Parse YAML frontmatter (between --- delimiters)
            if (text.TrimStart().StartsWith("---"))
            {
                var idx1 = text.IndexOf("---", StringComparison.Ordinal);
                var idx2 = text.IndexOf("---", idx1 + 3, StringComparison.Ordinal);

                if (idx2 > idx1)
                {
                    var yaml = text.Substring(idx1 + 3, idx2 - idx1 - 3);
                    ParseYamlFrontmatter(prompt, yaml);

                    // Content is everything after the second ---
                    var contentStart = idx2 + 3;
                    prompt.Content = text.Substring(contentStart).TrimStart('\r', '\n');
                }
                else
                {
                    // Malformed frontmatter — treat entire file as content
                    prompt.Content = text;
                }
            }
            else
            {
                // No frontmatter — entire file is content
                prompt.Content = text;
            }

            // Fallback: use filename if no name in frontmatter
            if (string.IsNullOrEmpty(prompt.Name))
                prompt.Name = Path.GetFileNameWithoutExtension(filePath);

            // Fallback: use folder name as domain if not specified in YAML
            if (string.IsNullOrEmpty(prompt.Domain))
            {
                var relDir = Path.GetDirectoryName(prompt.RelativePath);
                if (!string.IsNullOrEmpty(relDir))
                    prompt.Domain = relDir;
            }

            // Normalise domain regardless of whether it came from YAML or folder name
            NormaliseDomain(prompt);

            return prompt;
        }

        /// <summary>
        /// Applies canonical normalisation to prompt.Domain and sets IsQuickLauncher.
        /// Called after both YAML parsing and the folder-name fallback so the logic
        /// is consistent regardless of how the domain was determined.
        /// </summary>
        private static void NormaliseDomain(PromptTemplate prompt)
        {
            if (string.IsNullOrEmpty(prompt.Domain)) return;

            // Normalise legacy names → canonical "QuickLauncher"
            if (prompt.Domain.Equals("quickmenu_prompts", StringComparison.OrdinalIgnoreCase) ||
                prompt.Domain.Equals("quicklauncher_prompts", StringComparison.OrdinalIgnoreCase))
            {
                prompt.Domain = "QuickLauncher";
            }

            if (prompt.Domain.Equals("QuickLauncher", StringComparison.OrdinalIgnoreCase))
                prompt.IsQuickLauncher = true;
        }

        private void ParseYamlFrontmatter(PromptTemplate prompt, string yaml)
        {
            var lines = yaml.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#"))
                    continue;

                var colonIdx = trimmed.IndexOf(':');
                if (colonIdx <= 0)
                    continue;

                var key = trimmed.Substring(0, colonIdx).Trim().ToLowerInvariant();
                var value = trimmed.Substring(colonIdx + 1).Trim();

                // Strip quotes
                if (value.Length >= 2 &&
                    ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                     (value.StartsWith("'") && value.EndsWith("'"))))
                {
                    value = value.Substring(1, value.Length - 2);
                }

                switch (key)
                {
                    case "name":
                        prompt.Name = value;
                        break;
                    case "description":
                        prompt.Description = value;
                        break;
                    case "category":
                    case "domain": // backward compatibility
                        prompt.Domain = value;
                        // Full normalisation (legacy names → QuickLauncher, IsQuickLauncher flag)
                        // runs after all YAML is parsed, in NormaliseDomain().
                        break;
                    case "built_in":
                        prompt.IsBuiltIn = value.Equals("true", StringComparison.OrdinalIgnoreCase);
                        break;
                    case "quicklauncher_label":
                    case "quickmenu_label": // backward compatibility
                        prompt.QuickLauncherLabel = value;
                        break;
                }
            }
        }

        private static string GetRelativePath(string fullPath, string rootDir)
        {
            if (string.IsNullOrEmpty(fullPath) || string.IsNullOrEmpty(rootDir))
                return fullPath;

            var root = rootDir.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;

            if (fullPath.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                return fullPath.Substring(root.Length);

            return fullPath;
        }

        private static string SanitizeFileName(string name)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var sb = new StringBuilder(name.Length);
            foreach (var c in name)
            {
                if (Array.IndexOf(invalid, c) >= 0)
                    sb.Append('_');
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private static string EscapeYamlString(string s)
        {
            return (s ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        // ─── Built-in Prompt Definitions ──────────────────────────────

        private List<PromptTemplate> GetBuiltInPromptDefinitions()
        {
            return new List<PromptTemplate>
            {
                // ─── Default Translation Prompt ──────────────────────
                new PromptTemplate
                {
                    Name = "Default Translation Prompt",
                    Description = "General-purpose translation prompt — use as-is or as a starting point for your own prompts",
                    Domain = "Translate",
                    IsBuiltIn = true,
                    Content = @"You are a professional translator working from {{SOURCE_LANGUAGE}} to {{TARGET_LANGUAGE}}. Translate the source text accurately and naturally, following these guidelines:

## Core principles
- Produce a fluent, idiomatic translation that reads as if originally written in {{TARGET_LANGUAGE}}
- Preserve the meaning, tone, and register of the source text
- Maintain consistency in terminology throughout the document
- Keep numbers, dates, measurements, and proper nouns accurate
- Preserve all formatting, tags, and placeholders exactly as they appear

## Terminology
- Use the glossary terms provided (if any) — they take priority over alternative translations
- When a term has no established equivalent, keep the source term and add a brief explanation in parentheses if needed

## Style
- Match the formality level of the source (formal documents stay formal, casual content stays casual)
- Use natural sentence structures in the target language rather than mirroring source syntax
- Avoid unnecessary additions, omissions, or explanatory notes unless explicitly requested

This prompt is a starting point. Duplicate it in the Prompt Manager and customise it for your domain — add terminology rules, style preferences, or domain-specific instructions to get better results."
                },

                // ─── Proofreading ─────────────────────────────────────────
                new PromptTemplate
                {
                    Name = "Default Proofreading Prompt",
                    Description = "Reviews translations for accuracy, completeness, terminology, grammar, and style issues",
                    Domain = "Proofread",
                    IsBuiltIn = true,
                    Content = @"You are a professional translation proofreader. Your task is to review {{SOURCE_LANGUAGE}} to {{TARGET_LANGUAGE}} translation pairs and identify issues. You must check EVERY segment provided — do not skip any.

For each segment, check the following:

## 1. Accuracy
- Does the translation faithfully convey the meaning of the source?
- Are there any mistranslations, shifts in meaning, or misinterpretations?
- Are ambiguous source phrases resolved appropriately for the target language?

## 2. Completeness
- Is any source content omitted in the translation?
- Is any content added that is not present in the source?
- Are all numbers, dates, and references carried over correctly?

## 3. Terminology Consistency
- Are key terms translated consistently across segments?
- Are domain-specific terms translated correctly?
- Are proper nouns, brand names, and product names handled appropriately?

## 4. Grammar & Style
- Is the translation grammatically correct in {{TARGET_LANGUAGE}}?
- Is the style appropriate for the text type and register?
- Is the sentence structure natural and fluent in the target language?

## 5. Number & Unit Formatting
- Are numbers formatted according to {{TARGET_LANGUAGE}} conventions (decimal separators, thousand separators)?
- Are units of measurement correct and properly formatted?
- Are currency symbols and codes appropriate for the target locale?

## Language-Specific Checks

### Dutch
- Compound words: verify correct spelling (e.g., 'ziekenhuisopname' not 'ziekenhuis opname')
- dt-errors: check verb conjugation (word/wordt, vind/vindt, etc.)
- de/het articles: verify correct article usage with nouns
- Spelling: follow current Woordenlijst Nederlandse Taal (het Groene Boekje)

### German
- Compound nouns: verify correct formation (Zusammenschreibung)
- Capitalization: all nouns must be capitalized
- Case system: check correct use of Nominativ, Akkusativ, Dativ, Genitiv
- Verb position: verify correct verb placement in main and subordinate clauses

### French
- Accents: verify all accents are correct (é, è, ê, ë, à, ç, etc.)
- Gender/number agreement: check adjective-noun and subject-verb agreement
- Punctuation spacing: non-breaking space before ; : ! ? and inside « »
- Elision and liaison rules

## Output Format

You MUST use this exact format for every segment. Check ALL segments — do not skip any.

For segments with no issues:
[SEGMENT XXXX] OK

For segments with issues:
[SEGMENT XXXX] ISSUE
Issue: <brief description of the problem>
Suggestion: <describe what should be changed — do NOT provide a full corrected translation>

IMPORTANT RULES:
- NEVER provide corrected full translations. Only describe the issue and suggest what specifically should be fixed.
- Use the segment number as it appears in the input (e.g., [SEGMENT 0042]).
- Report each distinct issue on its own ISSUE block if a segment has multiple problems.
- You MUST review ALL segments. Do not stop early or summarize remaining segments as 'OK'."
                },

                // ─── QuickLauncher ─────────────────────────────────────
                new PromptTemplate
                {
                    Name = "Assess how I translated the current segment",
                    Description = "Reviews your translation of the active segment and suggests improvements",
                    Domain = "QuickLauncher",
                    IsBuiltIn = true,
                    Content = @"Source ({{SOURCE_LANGUAGE}}):
{{SOURCE_TEXT}}

My translation ({{TARGET_LANGUAGE}}):
{{TARGET_TEXT}}

Assess how I translated the current segment. Point out any inaccuracies, awkward phrasing, or terminology issues, and suggest improvements."
                },
                new PromptTemplate
                {
                    Name = "Define",
                    Description = "Defines the selected term and provides usage examples",
                    Domain = "QuickLauncher",
                    IsBuiltIn = true,
                    Content = @"Define ""{{SELECTION}}"" and give practical examples showing how it's used."
                },
                new PromptTemplate
                {
                    Name = "Explain (in general)",
                    Description = "Explains the selected term in simple, clear language",
                    Domain = "QuickLauncher",
                    IsBuiltIn = true,
                    Content = @"Explain ""{{SELECTION}}"" in simple, clear language. Include a practical example if helpful."
                },
                new PromptTemplate
                {
                    Name = "Explain (within project context)",
                    Description = "Explains the selected term using the full document as context",
                    Domain = "QuickLauncher",
                    IsBuiltIn = true,
                    Content = @"PROJECT CONTEXT - The complete source text from the current translation project:

{{PROJECT}}

---

Explain the term ""{{SELECTION}}"" in simple, clear language. If the project context above provides relevant information about how this term is used in this specific document, reference those segments in your explanation."
                },
                new PromptTemplate
                {
                    Name = "Translate segment using fuzzy matches as reference",
                    Description = "Translates the active segment, using TM fuzzy matches and surrounding context",
                    Domain = "QuickLauncher",
                    IsBuiltIn = true,
                    Content = @"Translate the following from {{SOURCE_LANGUAGE}} to {{TARGET_LANGUAGE}}.

Source: {{SOURCE_SEGMENT}}

Surrounding context:
{{SURROUNDING_SEGMENTS}}

TM fuzzy matches:
{{TM_MATCHES}}

Use the fuzzy matches and surrounding context as reference, but produce a fresh, accurate translation of the source segment."
                },
                new PromptTemplate
                {
                    Name = "Translate selection in context of current project",
                    Description = "Suggests the best translation for a selected term using full document context",
                    Domain = "QuickLauncher",
                    IsBuiltIn = true,
                    Content = @"PROJECT CONTEXT - The complete source text of the current translation project:

{{PROJECT}}

---

Using the project context above, suggest the best translation for ""{{SELECTION}}"" from {{SOURCE_LANGUAGE}} to {{TARGET_LANGUAGE}}. Reference relevant segments in your explanation."
                }
            };
        }
    }
}
