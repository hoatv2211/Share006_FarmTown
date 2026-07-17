using System;
using System.Linq;
using UnityEditor.Build;
using UnityEngine;

namespace FarmTown.Editor.Migration
{
    public static class ProjectMigrationCli
    {
        private static readonly string[] SummaryCodes =
        {
            "MissingScript",
            "MissingReference",
            "UnexpectedGuidChange",
            "NeedsMapping",
            "NonEnglishFirstPartyPath",
            "NonEnglishFirstPartySymbol",
            "ForbiddenStringLookup",
        };

        public static void Validate()
        {
            var report = ProjectMigrationValidator.ValidateAll();
            foreach (var group in report.Issues.GroupBy(issue => issue.Code))
            {
                foreach (var issue in group.Take(25))
                    Debug.LogError($"[{issue.Code}] {issue.Message}");

                var omitted = group.Count() - 25;
                if (omitted > 0)
                    Debug.LogWarning($"[{group.Key}] omitted {omitted} additional issues from console output.");
            }

            foreach (var code in SummaryCodes)
                Debug.Log($"{code}={report.Issues.Count(issue => issue.Code == code)}");

            if (!report.IsValid)
                throw new BuildFailedException($"Migration validation failed with {report.ErrorCount} errors.");
        }
    }
}
