using System.Linq;
using FarmTown.Editor.Migration;
using NUnit.Framework;

namespace FarmTown.Tests.EditMode
{
    public sealed class ProjectReferenceTests
    {
        [Test]
        public void ReadyRowsHaveEnglishTargetPathsAndStableGuids()
        {
            var rows = RenameInventoryBuilder.ReadInventory();
            Assert.That(rows, Is.Not.Empty);
            foreach (var row in rows.Where(row => row.Status == "Ready" && FirstPartyAssetPolicy.IsFirstParty(row.NewPath)))
            {
                Assert.That(ProjectMigrationValidator.IsEnglishTechnicalPath(row.NewPath), Is.True, row.NewPath);
                Assert.That(row.Guid, Is.Not.Empty, row.OldPath);
            }
        }

        [Test]
        public void InventoryHasNoUnexpectedGuidChanges()
        {
            Assert.That(ProjectMigrationValidator.FindUnexpectedGuidChanges(), Is.Empty);
        }

        [Test]
        public void ValidationReportCountsSeverity()
        {
            var report = new ValidationReport(new[]
            {
                new ValidationIssue("Warning", ValidationSeverity.Warning, "warning"),
                new ValidationIssue("Error", ValidationSeverity.Error, "error"),
            });

            Assert.That(report.WarningCount, Is.EqualTo(1));
            Assert.That(report.ErrorCount, Is.EqualTo(1));
            Assert.That(report.IsValid, Is.False);
        }

        [TestCase("GameBootstrap", true)]
        [TestCase("BatDauGame", false)]
        [TestCase("SanXuat", false)]
        public void TechnicalSymbolLanguageIsDetected(string symbol, bool expected)
        {
            Assert.That(ProjectMigrationValidator.IsEnglishTechnicalSymbol(symbol), Is.EqualTo(expected));
        }
    }
}
