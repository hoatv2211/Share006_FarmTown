using System;
using FarmTown.Save;
using NUnit.Framework;
using UnityEngine;

namespace FarmTown.Tests.EditMode
{
    public sealed class ProductionPersistenceTests
    {
        [SetUp]
        public void SetUp() => PlayerPrefs.DeleteAll();

        [TearDown]
        public void TearDown() => PlayerPrefs.DeleteAll();

        [Test]
        public void StableIdentityDoesNotDependOnObjectName()
        {
            Assert.That(ProductionPersistence.DefaultStableId(3, 2), Is.EqualTo("production-building-3-2"));
            Assert.That(ProductionPersistence.DefaultLegacyName(3, 2), Is.EqualTo("nhamay32"));
            Assert.Throws<ArgumentOutOfRangeException>(() => ProductionPersistence.DefaultStableId(-1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => ProductionPersistence.GetQueuedProduct("id", 0, "legacy"));
        }

        [Test]
        public void EnglishValuesWinOverLegacyValues()
        {
            const string stableId = "production-building-0-0";
            const string legacyName = "nhamay00";
            PlayerPrefs.SetInt(LegacySaveKeys.ProductionQueueCount(legacyName), 2);
            PlayerPrefs.SetInt(SaveKeys.ProductionQueueCount(stableId), 4);
            PlayerPrefs.SetInt(LegacySaveKeys.ProductionQueueProductId(legacyName, 1), 8);
            PlayerPrefs.SetInt(SaveKeys.ProductionQueueProductId(stableId, 1), 12);

            Assert.That(ProductionPersistence.GetQueueCount(stableId, legacyName), Is.EqualTo(4));
            Assert.That(ProductionPersistence.GetQueuedProduct(stableId, 1, legacyName), Is.EqualTo(12));
        }

        [Test]
        public void LegacyValuesBackfillEnglishKeys()
        {
            const string stableId = "production-building-1-2";
            const string legacyName = "nhamay12";
            PlayerPrefs.SetFloat(LegacySaveKeys.ProductionBuildingPositionX(legacyName), 3.5f);
            PlayerPrefs.SetInt(LegacySaveKeys.ProductionQueueCapacity(legacyName), 5);
            PlayerPrefs.SetInt(LegacySaveKeys.ProductionQueueProductId(legacyName, 2), 17);
            PlayerPrefs.SetInt(LegacySaveKeys.ProductionCompletedProductId(legacyName, 1), 11);
            PlayerPrefs.SetInt(LegacySaveKeys.ProductionRemainingTime(legacyName), 45);
            PlayerPrefs.SetInt(LegacySaveKeys.ProductionLastTimestamp(legacyName), 1234);

            Assert.That(ProductionPersistence.GetPositionX(stableId, legacyName), Is.EqualTo(3.5f));
            Assert.That(ProductionPersistence.GetQueueCapacity(stableId, legacyName), Is.EqualTo(5));
            Assert.That(ProductionPersistence.GetQueuedProduct(stableId, 2, legacyName), Is.EqualTo(17));
            Assert.That(ProductionPersistence.GetCompletedProduct(stableId, 1, legacyName), Is.EqualTo(11));
            Assert.That(ProductionPersistence.GetRemainingTime(stableId, legacyName), Is.EqualTo(45));
            Assert.That(ProductionPersistence.GetLastTimestamp(stableId, legacyName), Is.EqualTo(1234));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.ProductionQueueProductId(stableId, 2)), Is.EqualTo(17));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.ProductionCompletedProductId(stableId, 1)), Is.EqualTo(11));
        }

        [Test]
        public void LegacySelectionBackfillsStableIdentity()
        {
            PlayerPrefs.SetString(LegacySaveKeys.SelectedProductionBuilding, "nhamay12");

            Assert.That(ProductionPersistence.GetSelectedBuilding(), Is.EqualTo("production-building-1-2"));
            Assert.That(PlayerPrefs.GetString(SaveKeys.SelectedProductionBuilding), Is.EqualTo("production-building-1-2"));
        }

        [Test]
        public void TransitionalEnglishValuesBackfillStableKeys()
        {
            const string stableId = "production-building-2-3";
            const string legacyName = "nhamay23";
            PlayerPrefs.SetFloat(SaveKeys.FactoryPositionX(2, 3), 6.25f);
            PlayerPrefs.SetInt(SaveKeys.FactoryQueueCount(2, 3), 2);
            PlayerPrefs.SetInt(SaveKeys.FactoryQueueProductId(2, 3, 1), 18);

            Assert.That(ProductionPersistence.GetPositionX(stableId, legacyName), Is.EqualTo(6.25f));
            Assert.That(ProductionPersistence.GetQueueCount(stableId, legacyName), Is.EqualTo(2));
            Assert.That(ProductionPersistence.HasQueuedProduct(stableId, 1, legacyName), Is.True);
            Assert.That(ProductionPersistence.GetQueuedProduct(stableId, 1, legacyName), Is.EqualTo(18));
            Assert.That(PlayerPrefs.GetFloat(SaveKeys.ProductionBuildingPositionX(stableId)), Is.EqualTo(6.25f));
        }

        [Test]
        public void WritesEnglishAndLegacyKeysForCompatibilityWindow()
        {
            const string stableId = "production-building-2-1";
            const string legacyName = "nhamay21";
            ProductionPersistence.SetPosition(stableId, legacyName, new Vector2(2.5f, -1.25f));
            ProductionPersistence.SetFlipped(stableId, legacyName, true);
            ProductionPersistence.SetQueueCapacity(stableId, legacyName, 4);
            ProductionPersistence.SetQueueCount(stableId, legacyName, 2);
            ProductionPersistence.SetQueuedProduct(stableId, 1, legacyName, 7);
            ProductionPersistence.SetCompletedProduct(stableId, 2, legacyName, 9);
            ProductionPersistence.SetRemainingTime(stableId, legacyName, 30);
            ProductionPersistence.SetLastTimestamp(stableId, legacyName, 500);
            ProductionPersistence.SetSelectedBuilding(stableId, legacyName);

            Assert.That(PlayerPrefs.GetFloat(SaveKeys.ProductionBuildingPositionX(stableId)), Is.EqualTo(2.5f));
            Assert.That(PlayerPrefs.GetFloat(LegacySaveKeys.ProductionBuildingPositionX(legacyName)), Is.EqualTo(2.5f));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.ProductionQueueProductId(stableId, 1)), Is.EqualTo(7));
            Assert.That(PlayerPrefs.GetInt(LegacySaveKeys.ProductionQueueProductId(legacyName, 1)), Is.EqualTo(7));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.ProductionCompletedProductId(stableId, 2)), Is.EqualTo(9));
            Assert.That(PlayerPrefs.GetInt(LegacySaveKeys.ProductionCompletedProductId(legacyName, 2)), Is.EqualTo(9));
            Assert.That(PlayerPrefs.GetString(SaveKeys.SelectedProductionBuilding), Is.EqualTo(stableId));
            Assert.That(PlayerPrefs.GetString(LegacySaveKeys.SelectedProductionBuilding), Is.EqualTo(legacyName));

            ProductionPersistence.SetFlipped(stableId, legacyName, false);
            Assert.That(PlayerPrefs.GetInt(SaveKeys.ProductionBuildingFlipped(stableId)), Is.EqualTo(0));
            Assert.That(PlayerPrefs.HasKey(LegacySaveKeys.ProductionBuildingFlipped(legacyName)), Is.False);
        }

        [Test]
        public void DeleteSlotRemovesEnglishAndLegacyKeys()
        {
            const string stableId = "production-building-4-0";
            const string legacyName = "nhamay40";
            ProductionPersistence.SetQueuedProduct(stableId, 1, legacyName, 6);
            ProductionPersistence.SetCompletedProduct(stableId, 1, legacyName, 8);

            ProductionPersistence.DeleteQueuedProduct(stableId, 1, legacyName);
            ProductionPersistence.DeleteCompletedProduct(stableId, 1, legacyName);

            Assert.That(ProductionPersistence.HasQueuedProduct(stableId, 1, legacyName), Is.False);
            Assert.That(ProductionPersistence.HasCompletedProduct(stableId, 1, legacyName), Is.False);
        }
    }
}
