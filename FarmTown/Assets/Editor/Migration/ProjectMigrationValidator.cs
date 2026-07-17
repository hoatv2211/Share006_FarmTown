using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FarmTown.Editor.Migration
{
    public static class ProjectMigrationValidator
    {
        private static readonly string[] NonEnglishTechnicalFragments =
        {
            "odat", "hatgiong", "liem", "nongsan", "vatnuoi", "chuong", "thucan", "nhamay",
            "sanxuat", "vatpham", "donhang", "xeoto", "nhacho", "trangtri", "cayrung",
            "huongdan", "thongbao", "hieuung", "kimcuong", "kinhnghiem", "dichuyen",
            "thuhoach", "nhan", "bantay", "goccay", "nangcap", "banhmy", "batdau",
        };

        private static readonly Regex ForbiddenLookupPattern = new Regex(
            @"\b(GameObject\.Find|Transform\.Find|transform\.Find|Application\.LoadLevel)\s*\(",
            RegexOptions.Compiled);

        private static readonly Regex SymbolPattern = new Regex(
            @"^\s*(?:(?:public|internal|private|protected)\s+)?(?:(?:sealed|abstract|static|partial)\s+)*(?:class|struct|interface|enum)\s+([A-Za-z_][A-Za-z0-9_]*)",
            RegexOptions.Compiled | RegexOptions.Multiline);

        private static ReferenceScanResult cachedReferenceScan;

        public static ValidationReport ValidateAll()
        {
            cachedReferenceScan = null;
            var issues = new List<ValidationIssue>();
            issues.AddRange(FindMissingScripts().Select(message => Error("MissingScript", message)));
            issues.AddRange(FindMissingObjectReferences().Select(message => Error("MissingReference", message)));
            issues.AddRange(FindForbiddenStringLookups().Select(message => Error("ForbiddenStringLookup", message)));
            issues.AddRange(FindUnexpectedGuidChanges().Select(message => Error("UnexpectedGuidChange", message)));
            issues.AddRange(FindNonEnglishFirstPartySymbols().Select(message => Error("NonEnglishFirstPartySymbol", message)));

            foreach (var row in RenameInventoryBuilder.ReadInventory())
            {
                if (!FirstPartyAssetPolicy.IsFirstParty(row.NewPath))
                    continue;

                if (row.Status == "NeedsMapping")
                    issues.Add(Error("NeedsMapping", row.OldPath));
                if (row.Status == "Ready" && !IsEnglishTechnicalPath(row.NewPath))
                    issues.Add(Error("NonEnglishFirstPartyPath", row.NewPath));
            }

            return new ValidationReport(issues);
        }

        public static bool IsEnglishTechnicalPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || path.Any(character => character > 127))
                return false;

            var fileName = Path.GetFileNameWithoutExtension(path);
            return NonEnglishTechnicalFragments.All(fragment =>
                fileName.IndexOf(fragment, StringComparison.OrdinalIgnoreCase) < 0);
        }

        public static IReadOnlyList<string> FindMissingScripts()
        {
            return GetReferenceScan().MissingScripts;
        }

        public static IReadOnlyList<string> FindMissingObjectReferences()
        {
            return GetReferenceScan().MissingReferences;
        }

        private static ReferenceScanResult GetReferenceScan()
        {
            if (cachedReferenceScan != null)
                return cachedReferenceScan;

            var missingScripts = new List<string>();
            var missingReferences = new List<string>();
            InspectGameObjectAssets((assetPath, root) =>
            {
                foreach (var transform in root.GetComponentsInChildren<Transform>(true))
                {
                    var count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject);
                    if (count > 0)
                        missingScripts.Add($"{assetPath}:{GetObjectPath(transform)} missing={count}");
                }

                foreach (var component in root.GetComponentsInChildren<MonoBehaviour>(true))
                {
                    if (component == null)
                        continue;

                    var serializedObject = new SerializedObject(component);
                    var property = serializedObject.GetIterator();
                    while (property.Next(true))
                    {
                        if (property.propertyType != SerializedPropertyType.ObjectReference)
                            continue;
                        if (property.objectReferenceValue != null || property.objectReferenceInstanceIDValue == 0)
                            continue;

                        missingReferences.Add($"{assetPath}:{GetObjectPath(component.transform)}:{component.GetType().Name}.{property.propertyPath}");
                    }
                }
            });

            cachedReferenceScan = new ReferenceScanResult(
                NormalizeIssues(missingScripts),
                NormalizeIssues(missingReferences));
            return cachedReferenceScan;
        }

        public static IReadOnlyList<string> FindForbiddenStringLookups()
        {
            var issues = new List<string>();
            foreach (var assetPath in AssetDatabase.GetAllAssetPaths()
                         .Where(FirstPartyAssetPolicy.IsFirstParty)
                         .Where(path => path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)))
            {
                var absolutePath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", assetPath));
                var lines = File.ReadAllLines(absolutePath);
                for (var index = 0; index < lines.Length; index++)
                {
                    var line = lines[index].TrimStart();
                    if (line.StartsWith("//", StringComparison.Ordinal))
                        continue;
                    if (ForbiddenLookupPattern.IsMatch(line))
                        issues.Add($"{assetPath}:{index + 1}:{line.Trim()}");
                }
            }
            return issues;
        }

        public static IReadOnlyList<string> FindNonEnglishFirstPartySymbols()
        {
            var issues = new List<string>();
            foreach (var assetPath in AssetDatabase.GetAllAssetPaths()
                         .Where(FirstPartyAssetPolicy.IsFirstParty)
                         .Where(path => path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)))
            {
                var absolutePath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", assetPath));
                var source = File.ReadAllText(absolutePath);
                foreach (Match match in SymbolPattern.Matches(source))
                {
                    var symbol = match.Groups[1].Value;
                    if (!IsEnglishTechnicalSymbol(symbol))
                        issues.Add($"{assetPath}:{symbol}");
                }
            }
            return NormalizeIssues(issues);
        }

        public static bool IsEnglishTechnicalSymbol(string symbol)
        {
            return !string.IsNullOrWhiteSpace(symbol)
                && symbol.All(character => character <= 127)
                && NonEnglishTechnicalFragments.All(fragment =>
                    symbol.IndexOf(fragment, StringComparison.OrdinalIgnoreCase) < 0);
        }

        public static IReadOnlyList<string> FindUnexpectedGuidChanges()
        {
            var issues = new List<string>();
            foreach (var row in RenameInventoryBuilder.ReadInventory())
            {
                var expectedPath = row.Status == "Moved" ? row.NewPath : row.OldPath;
                var actualGuid = AssetDatabase.AssetPathToGUID(expectedPath);
                if (!string.Equals(actualGuid, row.Guid, StringComparison.OrdinalIgnoreCase))
                    issues.Add($"{expectedPath}: expected={row.Guid} actual={actualGuid}");
            }
            return issues;
        }

        private static void InspectGameObjectAssets(Action<string, GameObject> inspect)
        {
            foreach (var prefabPath in AssetDatabase.GetAllAssetPaths()
                         .Where(FirstPartyAssetPolicy.IsFirstParty)
                         .Where(path => path.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase)))
            {
                var root = PrefabUtility.LoadPrefabContents(prefabPath);
                try
                {
                    inspect(prefabPath, root);
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(root);
                }
            }

            var sceneSetup = EditorSceneManager.GetSceneManagerSetup();
            try
            {
                foreach (var scenePath in AssetDatabase.GetAllAssetPaths()
                             .Where(FirstPartyAssetPolicy.IsFirstParty)
                             .Where(path => path.EndsWith(".unity", StringComparison.OrdinalIgnoreCase)))
                {
                    var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                    foreach (var root in scene.GetRootGameObjects())
                        inspect(scenePath, root);
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
            finally
            {
                if (sceneSetup.Any(setup => setup.isLoaded))
                    EditorSceneManager.RestoreSceneManagerSetup(sceneSetup);
            }
        }

        private static IReadOnlyList<string> NormalizeIssues(IEnumerable<string> issues)
        {
            return issues.Distinct(StringComparer.Ordinal).OrderBy(issue => issue, StringComparer.Ordinal).ToArray();
        }

        private static string GetObjectPath(Transform transform)
        {
            var names = new Stack<string>();
            while (transform != null)
            {
                names.Push(transform.name);
                transform = transform.parent;
            }
            return string.Join("/", names);
        }

        private static ValidationIssue Error(string code, string message)
        {
            return new ValidationIssue(code, ValidationSeverity.Error, message);
        }

        private sealed class ReferenceScanResult
        {
            public ReferenceScanResult(
                IReadOnlyList<string> missingScripts,
                IReadOnlyList<string> missingReferences)
            {
                MissingScripts = missingScripts;
                MissingReferences = missingReferences;
            }

            public IReadOnlyList<string> MissingScripts { get; }
            public IReadOnlyList<string> MissingReferences { get; }
        }
    }

    public enum ValidationSeverity
    {
        Warning,
        Error,
    }

    public sealed class ValidationIssue
    {
        public ValidationIssue(string code, ValidationSeverity severity, string message)
        {
            Code = code;
            Severity = severity;
            Message = message;
        }

        public string Code { get; }
        public ValidationSeverity Severity { get; }
        public string Message { get; }
    }

    public sealed class ValidationReport
    {
        public ValidationReport(IReadOnlyList<ValidationIssue> issues)
        {
            Issues = issues ?? throw new ArgumentNullException(nameof(issues));
        }

        public IReadOnlyList<ValidationIssue> Issues { get; }
        public int ErrorCount => Issues.Count(issue => issue.Severity == ValidationSeverity.Error);
        public int WarningCount => Issues.Count(issue => issue.Severity == ValidationSeverity.Warning);
        public bool IsValid => ErrorCount == 0;
    }
}
