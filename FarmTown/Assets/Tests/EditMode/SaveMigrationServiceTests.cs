using FarmTown.Save;
using NUnit.Framework;
using UnityEngine;

namespace FarmTown.Tests.EditMode
{
    public sealed class SaveMigrationServiceTests
    {
        [SetUp]
        public void SetUp() => PlayerPrefs.DeleteAll();

        [TearDown]
        public void TearDown() => PlayerPrefs.DeleteAll();

        [Test]
        public void SchemaZeroCopiesWalletProgressInventoryPlotsAndFactoryQueue()
        {
            PlayerPrefs.SetInt("gold", 2450);
            PlayerPrefs.SetInt("diamond", 17);
            PlayerPrefs.SetInt("level", 8);
            PlayerPrefs.SetInt("kinhnghiem", 12);
            PlayerPrefs.SetInt("kinhnghiemmax", 40);
            PlayerPrefs.SetInt("slnongsan0", 14);
            PlayerPrefs.SetInt("slvatpham12", 3);
            PlayerPrefs.SetInt("huongdan", 18);
            PlayerPrefs.SetInt("soodat", 1);
            PlayerPrefs.SetFloat("xodat0", -1.56f);
            PlayerPrefs.SetFloat("yodat0", 0.8f);
            PlayerPrefs.SetInt("slnhamay0", 1);
            PlayerPrefs.SetFloat("xnhamay00", 2.48f);
            PlayerPrefs.SetFloat("ynhamay00", -0.64f);
            PlayerPrefs.SetInt("dangsanxuatnhamay00", 1);
            PlayerPrefs.SetInt("idsanxuat1nhamay00", 12);
            PlayerPrefs.SetInt("osanxuatnhamay00", 4);
            PlayerPrefs.SetInt("idhoanthanh1nhamay00", 9);
            PlayerPrefs.SetInt("timenhamay00", 35);
            PlayerPrefs.SetInt("timethoatnhamay00", 1234);
            PlayerPrefs.SetInt("quaynhamay00", 1);
            PlayerPrefs.SetString("chonnhamay", "nhamay00");
            PlayerPrefs.SetInt("slchuong0", 2);
            PlayerPrefs.SetInt("slvatnuoi0", 3);
            PlayerPrefs.SetInt("sltrangtri0", 1);
            PlayerPrefs.SetInt("sodonhang", 4);
            PlayerPrefs.SetInt("dh0", 7);
            PlayerPrefs.SetInt("dangtaidh0", 1);
            PlayerPrefs.SetInt("timedh0", 90);
            PlayerPrefs.SetInt("capmoodat", 6);
            PlayerPrefs.SetInt("LevelOfMinigame2", 5);
            PlayerPrefs.SetInt("LevelMiniGame3", 3);
            PlayerPrefs.SetInt("IsEnableMiniGame1", 0);

            new SaveMigrationService(new PlayerPrefsStore()).MigrateToLatest();

            Assert.That(PlayerPrefs.GetInt(SaveKeys.SchemaVersion), Is.EqualTo(SaveMigrationService.LatestSchemaVersion));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.WalletCoins), Is.EqualTo(2450));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.WalletGems), Is.EqualTo(17));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.ProgressionLevel), Is.EqualTo(8));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.ProgressionExperience), Is.EqualTo(12));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.ProgressionExperienceLimit), Is.EqualTo(40));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.CropQuantity(0)), Is.EqualTo(14));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.ProductQuantity(12)), Is.EqualTo(3));
            Assert.That(PlayerPrefs.GetFloat(SaveKeys.PlotPositionX(0)), Is.EqualTo(-1.56f));
            Assert.That(PlayerPrefs.GetFloat(SaveKeys.FactoryPositionY(0, 0)), Is.EqualTo(-0.64f));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.FactoryQueueProductId(0, 0, 1)), Is.EqualTo(12));
            const string productionStableId = "production-building-0-0";
            Assert.That(PlayerPrefs.GetFloat(SaveKeys.ProductionBuildingPositionX(productionStableId)), Is.EqualTo(2.48f));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.ProductionQueueCapacity(productionStableId)), Is.EqualTo(4));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.ProductionQueueCount(productionStableId)), Is.EqualTo(1));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.ProductionQueueProductId(productionStableId, 1)), Is.EqualTo(12));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.ProductionCompletedProductId(productionStableId, 1)), Is.EqualTo(9));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.ProductionRemainingTime(productionStableId)), Is.EqualTo(35));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.ProductionLastTimestamp(productionStableId)), Is.EqualTo(1234));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.ProductionBuildingFlipped(productionStableId)), Is.EqualTo(1));
            Assert.That(PlayerPrefs.GetString(SaveKeys.SelectedProductionBuilding), Is.EqualTo(productionStableId));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.AnimalPenQuantity(0)), Is.EqualTo(2));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.FarmAnimalQuantity(0)), Is.EqualTo(3));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.DecorationQuantity(0)), Is.EqualTo(1));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.OrderState(0)), Is.EqualTo(7));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.OrderRemainingTime(0)), Is.EqualTo(90));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.LandExpansionLevel), Is.EqualTo(6));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.MiniGame2Level), Is.EqualTo(5));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.MiniGame3Level), Is.EqualTo(3));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.MiniGameEnabled(1)), Is.EqualTo(0));
            Assert.That(PlayerPrefs.GetInt("gold"), Is.EqualTo(2450));
        }

        [Test]
        public void LatestSchemaDoesNotOverwriteEnglishValues()
        {
            PlayerPrefs.SetInt(SaveKeys.SchemaVersion, SaveMigrationService.LatestSchemaVersion);
            PlayerPrefs.SetInt(SaveKeys.WalletCoins, 99);
            PlayerPrefs.SetInt(LegacySaveKeys.Gold, 2450);

            new SaveMigrationService(new PlayerPrefsStore()).MigrateToLatest();

            Assert.That(PlayerPrefs.GetInt(SaveKeys.WalletCoins), Is.EqualTo(99));
        }

        [Test]
        public void SchemaOneMigratesStableProductionKeys()
        {
            PlayerPrefs.SetInt(SaveKeys.SchemaVersion, 1);
            PlayerPrefs.SetInt(LegacySaveKeys.FactoryCount(0), 1);
            PlayerPrefs.SetInt(LegacySaveKeys.ProductionQueueCapacity("nhamay00"), 4);
            PlayerPrefs.SetInt(LegacySaveKeys.ProductionQueueCount("nhamay00"), 1);
            PlayerPrefs.SetInt(LegacySaveKeys.ProductionQueueProductId("nhamay00", 1), 12);

            new SaveMigrationService(new PlayerPrefsStore()).MigrateToLatest();

            Assert.That(PlayerPrefs.GetInt(SaveKeys.SchemaVersion), Is.EqualTo(2));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.ProductionQueueCapacity("production-building-0-0")), Is.EqualTo(4));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.ProductionQueueProductId("production-building-0-0", 1)), Is.EqualTo(12));
        }

        [Test]
        public void SchemaOnePrefersTransitionalEnglishProductionValues()
        {
            PlayerPrefs.SetInt(SaveKeys.SchemaVersion, 1);
            PlayerPrefs.SetInt(LegacySaveKeys.FactoryCount(0), 1);
            PlayerPrefs.SetFloat(LegacySaveKeys.FactoryPositionX(0, 0), 2f);
            PlayerPrefs.SetFloat(SaveKeys.FactoryPositionX(0, 0), 8f);
            PlayerPrefs.SetInt(LegacySaveKeys.FactoryQueueCount(0, 0), 1);
            PlayerPrefs.SetInt(LegacySaveKeys.FactoryQueueProductId(1, 0, 0), 4);
            PlayerPrefs.SetInt(SaveKeys.FactoryQueueProductId(0, 0, 1), 14);

            new SaveMigrationService(new PlayerPrefsStore()).MigrateToLatest();

            Assert.That(PlayerPrefs.GetFloat(SaveKeys.ProductionBuildingPositionX("production-building-0-0")), Is.EqualTo(8f));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.ProductionQueueProductId("production-building-0-0", 1)), Is.EqualTo(14));
        }

        [Test]
        public void MissingLegacyKeysDoNotCreateEnglishKeys()
        {
            new SaveMigrationService(new PlayerPrefsStore()).MigrateToLatest();

            Assert.That(PlayerPrefs.HasKey(SaveKeys.WalletCoins), Is.False);
            Assert.That(PlayerPrefs.GetInt(SaveKeys.SchemaVersion), Is.EqualTo(SaveMigrationService.LatestSchemaVersion));
        }

        [Test]
        public void PartialMigrationDoesNotOverwriteEnglishValues()
        {
            PlayerPrefs.SetInt(LegacySaveKeys.Gold, 2450);
            PlayerPrefs.SetInt(SaveKeys.WalletCoins, 99);

            new SaveMigrationService(new PlayerPrefsStore()).MigrateToLatest();

            Assert.That(PlayerPrefs.GetInt(SaveKeys.WalletCoins), Is.EqualTo(99));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.SchemaVersion), Is.EqualTo(SaveMigrationService.LatestSchemaVersion));
        }
    }
}
