using FarmTown.Save;
using NUnit.Framework;
using UnityEngine;

namespace FarmTown.Tests.EditMode
{
    public sealed class AnimalPersistenceTests
    {
        [SetUp]
        public void SetUp() => PlayerPrefs.DeleteAll();

        [TearDown]
        public void TearDown() => PlayerPrefs.DeleteAll();

        [Test]
        public void StablePenIdentityDoesNotDependOnObjectName()
        {
            Assert.That(AnimalPersistence.DefaultPenStableId(3, 2), Is.EqualTo("animal-pen-3-2"));
            Assert.That(AnimalPersistence.DefaultLegacyPenName(3, 2), Is.EqualTo("chuong32"));
            Assert.That(AnimalPersistence.DefaultLegacyAnimalName(0, 0), Is.EqualTo("ga"));
            Assert.That(AnimalPersistence.DefaultLegacyAnimalName(2, 1), Is.EqualTo("Vit (1)"));
            Assert.That(AnimalPersistence.DefaultLegacyAnimalName(6, 2), Is.EqualTo("De (2)"));
        }

        [Test]
        public void EnglishValuesWinOverLegacyValues()
        {
            const string stableId = "animal-pen-1-0";
            const string legacyName = "chuong10";
            PlayerPrefs.SetFloat(LegacySaveKeys.AnimalPenPositionX(legacyName), 3f);
            PlayerPrefs.SetFloat(SaveKeys.AnimalPenPositionX(stableId), 9f);
            PlayerPrefs.SetString(LegacySaveKeys.FarmAnimalState("bo1", legacyName), "doi");
            PlayerPrefs.SetString(SaveKeys.FarmAnimalState(stableId, 0), "thuhoach");

            Assert.That(AnimalPersistence.GetPenPositionX(stableId, legacyName), Is.EqualTo(9f));
            Assert.That(AnimalPersistence.GetAnimalState(stableId, 0, "bo1", legacyName), Is.EqualTo("thuhoach"));
        }

        [Test]
        public void WritesEnglishAndLegacyKeysForOneRelease()
        {
            const string stableId = "animal-pen-2-1";
            const string legacyName = "chuong21";
            AnimalPersistence.SetPenPosition(stableId, legacyName, new Vector2(2.5f, -1.25f));
            AnimalPersistence.SetPenFlipped(stableId, legacyName, true);
            AnimalPersistence.SetPenOccupancy(stableId, legacyName, 2);
            AnimalPersistence.SetAnimalState(stableId, 1, "heo2", legacyName, "an");
            AnimalPersistence.SetAnimalRemainingTime(stableId, 1, "heo2", legacyName, 45);
            AnimalPersistence.SetAnimalLastTimestamp(stableId, 1, "heo2", legacyName, 1234);

            Assert.That(PlayerPrefs.GetFloat(SaveKeys.AnimalPenPositionX(stableId)), Is.EqualTo(2.5f));
            Assert.That(PlayerPrefs.GetFloat(LegacySaveKeys.AnimalPenPositionX(legacyName)), Is.EqualTo(2.5f));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.AnimalPenFlipped(stableId)), Is.EqualTo(1));
            Assert.That(PlayerPrefs.GetInt(LegacySaveKeys.AnimalPenFlipped(legacyName)), Is.EqualTo(1));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.AnimalPenOccupancy(stableId)), Is.EqualTo(2));
            Assert.That(PlayerPrefs.GetInt(LegacySaveKeys.AnimalPenOccupancy(legacyName)), Is.EqualTo(2));
            Assert.That(PlayerPrefs.GetString(SaveKeys.FarmAnimalState(stableId, 1)), Is.EqualTo("an"));
            Assert.That(PlayerPrefs.GetString(LegacySaveKeys.FarmAnimalState("heo2", legacyName)), Is.EqualTo("an"));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.FarmAnimalRemainingTime(stableId, 1)), Is.EqualTo(45));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.FarmAnimalLastTimestamp(stableId, 1)), Is.EqualTo(1234));

            AnimalPersistence.SetPenFlipped(stableId, legacyName, false);
            Assert.That(PlayerPrefs.GetInt(SaveKeys.AnimalPenFlipped(stableId)), Is.EqualTo(0));
            Assert.That(PlayerPrefs.HasKey(LegacySaveKeys.AnimalPenFlipped(legacyName)), Is.False);
        }

        [Test]
        public void LegacyValuesRemainReadableBeforeMigration()
        {
            const string stableId = "animal-pen-0-0";
            const string legacyName = "chuong00";
            PlayerPrefs.SetInt(LegacySaveKeys.AnimalPenOccupancy(legacyName), 1);
            PlayerPrefs.SetInt(LegacySaveKeys.FarmAnimalRemainingTime("ga1", legacyName), 30);

            Assert.That(AnimalPersistence.GetPenOccupancy(stableId, legacyName), Is.EqualTo(1));
            Assert.That(AnimalPersistence.GetAnimalRemainingTime(stableId, 0, "ga1", legacyName), Is.EqualTo(30));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.AnimalPenOccupancy(stableId)), Is.EqualTo(1));
            Assert.That(PlayerPrefs.GetInt(SaveKeys.FarmAnimalRemainingTime(stableId, 0)), Is.EqualTo(30));
        }
    }
}
