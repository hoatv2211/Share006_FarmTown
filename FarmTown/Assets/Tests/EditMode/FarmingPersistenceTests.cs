using FarmTown.Save;
using NUnit.Framework;
using UnityEngine;

namespace FarmTown.Tests.EditMode
{
    public sealed class FarmingPersistenceTests
    {
        [SetUp]
        public void SetUp() => PlayerPrefs.DeleteAll();

        [TearDown]
        public void TearDown() => PlayerPrefs.DeleteAll();

        [Test]
        public void StableInstanceIdSafelyReadsSerializedValue()
        {
            var gameObject = new GameObject("plot");
            try
            {
                var component = gameObject.AddComponent<StableInstanceId>();
                JsonUtility.FromJsonOverwrite("{\"stableId\":\"crop-plot-3\"}", component);
                Assert.That(component.StableId, Is.EqualTo("crop-plot-3"));
                Assert.That(component.TryGetStableId(out var stableId), Is.True);
                Assert.That(stableId, Is.EqualTo("crop-plot-3"));
            }
            finally { Object.DestroyImmediate(gameObject); }
        }

        [Test]
        public void StableInstanceIdRejectsMissingOrWhitespaceValue()
        {
            var gameObject = new GameObject("plot");
            try
            {
                var component = gameObject.AddComponent<StableInstanceId>();
                Assert.That(component.StableId, Is.EqualTo(string.Empty));
                Assert.That(component.TryGetStableId(out var missingId), Is.False);
                Assert.That(missingId, Is.EqualTo(string.Empty));
                JsonUtility.FromJsonOverwrite("{\"stableId\":\"   \"}", component);
                Assert.That(component.StableId, Is.EqualTo(string.Empty));
                Assert.That(component.TryGetStableId(out var whitespaceId), Is.False);
                Assert.That(whitespaceId, Is.EqualTo(string.Empty));
            }
            finally { Object.DestroyImmediate(gameObject); }
        }

        [TestCase(0, "crop-plot-0", "odat0")]
        [TestCase(1, "crop-plot-1", "odat1")]
        [TestCase(2, "crop-plot-2", "odat2")]
        [TestCase(3, "crop-plot-3", "odat3")]
        [TestCase(4, "crop-plot-4", "odat4")]
        [TestCase(5, "crop-plot-5", "odat5")]
        [TestCase(8, "crop-plot-8", "odat8")]
        public void DefaultPlotIdentityMapsStableIdToLegacyAlias(int index, string stableId, string legacyAlias)
        {
            Assert.That(FarmingPlotPersistence.DefaultStableId(index), Is.EqualTo(stableId));
            Assert.That(FarmingPlotPersistence.DefaultLegacyAlias(index), Is.EqualTo(legacyAlias));
            Assert.That(FarmingPlotPersistence.TryGetStableId(legacyAlias, out var resolved), Is.True);
            Assert.That(resolved, Is.EqualTo(stableId));
        }

        [Test]
        public void StableInstanceIdAssignmentRegistersAndIsIdempotent()
        {
            var gameObject = new GameObject("plot");
            try
            {
                var component = gameObject.AddComponent<StableInstanceId>();
                Assert.That(component.TryAssign("crop-plot-8"), Is.True);
                Assert.That(component.TryAssign("crop-plot-8"), Is.True);
                Assert.That(StableInstanceId.TryFind("crop-plot-8", out var found), Is.True);
                Assert.That(found, Is.SameAs(component));
            }
            finally { Object.DestroyImmediate(gameObject); }

            Assert.That(StableInstanceId.TryFind("crop-plot-8", out _), Is.False);
        }

        [Test]
        public void StableInstanceIdRejectsDuplicateAndReassignment()
        {
            var firstObject = new GameObject("first");
            var secondObject = new GameObject("second");
            try
            {
                var first = firstObject.AddComponent<StableInstanceId>();
                var second = secondObject.AddComponent<StableInstanceId>();
                Assert.That(first.TryAssign("crop-plot-7"), Is.True);
                Assert.That(first.TryAssign("crop-plot-9"), Is.False);
                Assert.That(second.TryAssign("crop-plot-7"), Is.False);
                Assert.That(StableInstanceId.TryFind("crop-plot-7", out var found), Is.True);
                Assert.That(found, Is.SameAs(first));
            }
            finally
            {
                Object.DestroyImmediate(firstObject);
                Object.DestroyImmediate(secondObject);
            }
        }

        [Test]
        public void CompatibilityMigratesPurchasedPlotBeyondDefaultSix()
        {
            PlayerPrefs.SetInt(LegacySaveKeys.PlotCount, 9);
            PlayerPrefs.SetFloat(LegacySaveKeys.PlotPositionX(8), 8.25f);
            PlayerPrefs.SetFloat(LegacySaveKeys.PlotPositionY(8), -8.5f);
            PlayerPrefs.SetInt(LegacySaveKeys.PlotCropId("odat8"), 18);
            PlayerPrefs.SetInt(LegacySaveKeys.PlotRemainingTime("odat8"), 108);
            PlayerPrefs.SetInt(LegacySaveKeys.PlotLastTimestamp("odat8"), 1008);

            new SaveMigrationService(new PlayerPrefsStore()).MigrateToLatest();

            Assert.That(PlayerPrefs.GetFloat(SaveKeys.PlotPositionX("crop-plot-8")), Is.EqualTo(8.25f));
            Assert.That(PlayerPrefs.GetFloat(SaveKeys.PlotPositionY("crop-plot-8")), Is.EqualTo(-8.5f));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.PlotCropId("crop-plot-8")), Is.EqualTo(18));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.PlotRemainingTime("crop-plot-8")), Is.EqualTo(108));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.PlotLastTimestamp("crop-plot-8")), Is.EqualTo(1008));
        }

        [Test]
        public void CompatibilityPreservesLegacyAndEnglishPlotValuesWin()
        {
            PlayerPrefs.SetInt(LegacySaveKeys.PlotCount, 7);
            PlayerPrefs.SetInt(LegacySaveKeys.PlotCropId("odat6"), 6);
            PlayerPrefs.SetInt(SaveKeys.PlotCropId("crop-plot-6"), 16);

            new SaveMigrationService(new PlayerPrefsStore()).MigrateToLatest();

            Assert.That(PlayerPrefs.GetInt(SaveKeys.PlotCropId("crop-plot-6")), Is.EqualTo(16));
            Assert.That(PlayerPrefs.GetInt(LegacySaveKeys.PlotCropId("odat6")), Is.EqualTo(6));
        }

        [Test]
        public void VirginMigrationLeavesPlotCountKeysAbsentForDefaultLayoutBootstrap()
        {
            new SaveMigrationService(new PlayerPrefsStore()).MigrateToLatest();

            Assert.That(PlayerPrefs.HasKey(SaveKeys.PlotCount), Is.False);
            Assert.That(PlayerPrefs.HasKey(LegacySaveKeys.PlotCount), Is.False);
            Assert.That(FarmingPlotPersistence.HasSavedPlotCount(), Is.False);
        }

        [Test]
        public void SavedPlotCountDetectionMatchesBootstrapBranchCondition()
        {
            Assert.That(FarmingPlotPersistence.HasSavedPlotCount(), Is.False);
            PlayerPrefs.SetInt(SaveKeys.PlotCount, 6);
            Assert.That(FarmingPlotPersistence.HasSavedPlotCount(), Is.True);
            PlayerPrefs.DeleteKey(SaveKeys.PlotCount);
            PlayerPrefs.SetInt(LegacySaveKeys.PlotCount, 6);
            Assert.That(FarmingPlotPersistence.HasSavedPlotCount(), Is.True);
        }

        [Test]
        public void CompatibilityUsesMaximumEnglishAndLegacyPlotCounts()
        {
            PlayerPrefs.SetInt(SaveKeys.PlotCount, 7);
            PlayerPrefs.SetInt(LegacySaveKeys.PlotCount, 10);
            PlayerPrefs.SetInt(LegacySaveKeys.PlotCropId("odat9"), 19);

            new SaveMigrationService(new PlayerPrefsStore()).MigrateToLatest();

            Assert.That(PlayerPrefs.GetInt(SaveKeys.PlotCropId("crop-plot-9")), Is.EqualTo(19));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.PlotCount), Is.EqualTo(10));
            Assert.That(PlayerPrefs.GetInt(LegacySaveKeys.PlotCount), Is.EqualTo(10));
        }

        [Test]
        public void CanonicalPlotCountReadsMaximumAndDualWrites()
        {
            PlayerPrefs.SetInt(SaveKeys.PlotCount, 8);
            PlayerPrefs.SetInt(LegacySaveKeys.PlotCount, 6);
            Assert.That(FarmingPlotPersistence.GetPlotCount(), Is.EqualTo(8));

            FarmingPlotPersistence.SetPlotCount(9);

            Assert.That(PlayerPrefs.GetInt(SaveKeys.PlotCount), Is.EqualTo(9));
            Assert.That(PlayerPrefs.GetInt(LegacySaveKeys.PlotCount), Is.EqualTo(9));
        }

        [Test]
        public void CropQuantityReadsEnglishFirstAndDualWrites()
        {
            PlayerPrefs.SetInt(SaveKeys.CropQuantity(3), 11);
            PlayerPrefs.SetInt(LegacySaveKeys.CropQuantity(3), 4);
            Assert.That(FarmingPlotPersistence.GetCropQuantity(3), Is.EqualTo(11));

            FarmingPlotPersistence.SetCropQuantity(3, 10);

            Assert.That(PlayerPrefs.GetInt(SaveKeys.CropQuantity(3)), Is.EqualTo(10));
            Assert.That(PlayerPrefs.GetInt(LegacySaveKeys.CropQuantity(3)), Is.EqualTo(10));
        }

        [Test]
        public void OutOfSeedsTargetResolutionFailsWithoutPartialResults()
        {
            var plotObject = new GameObject("plot");
            try
            {
                var identity = plotObject.AddComponent<StableInstanceId>();
                Assert.That(identity.TryAssign("crop-plot-0"), Is.True);
                var targetIds = new[] { "crop-plot-0", "crop-plot-missing" };

                Assert.That(FarmingPlotPersistence.TryResolveStableInstances(targetIds, out var targets), Is.False);
                Assert.That(targets, Is.Empty);
            }
            finally { Object.DestroyImmediate(plotObject); }
        }

        [Test]
        public void SchemaOneCompatibilityMigratesSixPlotPositionsAndCropState()
        {
            PlayerPrefs.SetInt(SaveKeys.SchemaVersion, SaveMigrationService.LatestSchemaVersion);
            PlayerPrefs.SetInt(LegacySaveKeys.PlotCount, FarmingPlotPersistence.DefaultPlotCount);
            for (var index = 0; index < FarmingPlotPersistence.DefaultPlotCount; index++)
            {
                var alias = $"odat{index}";
                PlayerPrefs.SetFloat($"xodat{index}", index + 0.25f);
                PlayerPrefs.SetFloat($"yodat{index}", -index - 0.5f);
                PlayerPrefs.SetInt($"idodat{alias}", 10 + index);
                PlayerPrefs.SetInt($"timeodat{alias}", 100 + index);
                PlayerPrefs.SetInt($"timethoat{alias}", 1000 + index);
            }
            new SaveMigrationService(new PlayerPrefsStore()).MigrateToLatest();
            Assert.That(PlayerPrefs.GetInt(SaveKeys.SchemaVersion), Is.EqualTo(SaveMigrationService.LatestSchemaVersion));
            for (var index = 0; index < FarmingPlotPersistence.DefaultPlotCount; index++)
            {
                var stableId = $"crop-plot-{index}";
                Assert.That(PlayerPrefs.GetFloat(SaveKeys.PlotPositionX(stableId)), Is.EqualTo(index + 0.25f));
                Assert.That(PlayerPrefs.GetFloat(SaveKeys.PlotPositionY(stableId)), Is.EqualTo(-index - 0.5f));
                Assert.That(PlayerPrefs.GetInt(SaveKeys.PlotCropId(stableId)), Is.EqualTo(10 + index));
                Assert.That(PlayerPrefs.GetInt(SaveKeys.PlotRemainingTime(stableId)), Is.EqualTo(100 + index));
                Assert.That(PlayerPrefs.GetInt(SaveKeys.PlotLastTimestamp(stableId)), Is.EqualTo(1000 + index));
            }
        }

        [Test]
        public void CompatibilityMigratesSelectedPlotAndOutOfSeedTargetsToStableIds()
        {
            PlayerPrefs.SetInt("sohg", 2);
            PlayerPrefs.SetString("chonodat", "odat4");
            PlayerPrefs.SetString("tenodatthieu0", "odat1");
            PlayerPrefs.SetString("tenodatthieu1", "odat5");
            new SaveMigrationService(new PlayerPrefsStore()).MigrateToLatest();
            Assert.That(PlayerPrefs.GetString(SaveKeys.SelectedPlot), Is.EqualTo("crop-plot-4"));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.OutOfSeedTargetCount), Is.EqualTo(2));
            Assert.That(PlayerPrefs.GetString(SaveKeys.OutOfSeedTarget(0)), Is.EqualTo("crop-plot-1"));
            Assert.That(PlayerPrefs.GetString(SaveKeys.OutOfSeedTarget(1)), Is.EqualTo("crop-plot-5"));
        }

        [Test]
        public void ExistingEnglishPlotValuesWinAndLegacyKeysRemain()
        {
            const string stableId = "crop-plot-2";
            const string legacyAlias = "odat2";
            PlayerPrefs.SetInt(SaveKeys.SchemaVersion, SaveMigrationService.LatestSchemaVersion);
            PlayerPrefs.SetFloat(SaveKeys.PlotPositionX(stableId), 99f);
            PlayerPrefs.SetFloat(SaveKeys.PlotPositionY(stableId), 98f);
            PlayerPrefs.SetInt(SaveKeys.PlotCropId(stableId), 77);
            PlayerPrefs.SetInt(SaveKeys.PlotRemainingTime(stableId), 66);
            PlayerPrefs.SetInt(SaveKeys.PlotLastTimestamp(stableId), 55);
            PlayerPrefs.SetString(SaveKeys.SelectedPlot, "crop-plot-5");
            PlayerPrefs.SetFloat("xodat2", 2.25f);
            PlayerPrefs.SetFloat("yodat2", -2.5f);
            PlayerPrefs.SetInt($"idodat{legacyAlias}", 12);
            PlayerPrefs.SetInt($"timeodat{legacyAlias}", 34);
            PlayerPrefs.SetInt($"timethoat{legacyAlias}", 56);
            PlayerPrefs.SetString("chonodat", legacyAlias);

            new SaveMigrationService(new PlayerPrefsStore()).MigrateToLatest();

            Assert.That(PlayerPrefs.GetFloat(SaveKeys.PlotPositionX(stableId)), Is.EqualTo(99f));
            Assert.That(PlayerPrefs.GetFloat(SaveKeys.PlotPositionY(stableId)), Is.EqualTo(98f));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.PlotCropId(stableId)), Is.EqualTo(77));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.PlotRemainingTime(stableId)), Is.EqualTo(66));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.PlotLastTimestamp(stableId)), Is.EqualTo(55));
            Assert.That(PlayerPrefs.GetString(SaveKeys.SelectedPlot), Is.EqualTo("crop-plot-5"));
            Assert.That(PlayerPrefs.HasKey("xodat2"), Is.True);
            Assert.That(PlayerPrefs.HasKey("yodat2"), Is.True);
            Assert.That(PlayerPrefs.HasKey($"idodat{legacyAlias}"), Is.True);
            Assert.That(PlayerPrefs.HasKey($"timeodat{legacyAlias}"), Is.True);
            Assert.That(PlayerPrefs.HasKey($"timethoat{legacyAlias}"), Is.True);
            Assert.That(PlayerPrefs.HasKey("chonodat"), Is.True);
        }
    }
}
