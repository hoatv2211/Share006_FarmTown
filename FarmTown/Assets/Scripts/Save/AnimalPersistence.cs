using System;
using UnityEngine;

namespace FarmTown.Save
{
    public static class AnimalPersistence
    {
        private const string PenStableIdPrefix = "animal-pen-";
        private const string LegacyPenNamePrefix = "chuong";
        private static readonly string[] LegacyAnimalNamePrefixes =
        {
            "ga", "bo", "Vit", "heo", "cuu", "Trau", "De",
        };

        public static string DefaultPenStableId(int typeId, int instanceIndex)
        {
            ValidateIndex(typeId, nameof(typeId));
            ValidateIndex(instanceIndex, nameof(instanceIndex));
            return $"{PenStableIdPrefix}{typeId}-{instanceIndex}";
        }

        public static string DefaultLegacyPenName(int typeId, int instanceIndex)
        {
            ValidateIndex(typeId, nameof(typeId));
            ValidateIndex(instanceIndex, nameof(instanceIndex));
            return $"{LegacyPenNamePrefix}{typeId}{instanceIndex}";
        }

        public static string DefaultLegacyAnimalName(int typeId, int slotIndex)
        {
            if (typeId < 0 || typeId >= LegacyAnimalNamePrefixes.Length)
                throw new ArgumentOutOfRangeException(nameof(typeId));
            if (slotIndex < 0 || slotIndex > 2)
                throw new ArgumentOutOfRangeException(nameof(slotIndex));
            return slotIndex == 0
                ? LegacyAnimalNamePrefixes[typeId]
                : $"{LegacyAnimalNamePrefixes[typeId]} ({slotIndex})";
        }

        public static int GetPenQuantity(int typeId) => ReadInt(
            SaveKeys.AnimalPenQuantity(typeId), LegacySaveKeys.AnimalPenQuantity(typeId));

        public static void SetPenQuantity(int typeId, int value) => WriteInt(
            SaveKeys.AnimalPenQuantity(typeId), LegacySaveKeys.AnimalPenQuantity(typeId), value);

        public static int GetPenLimit(int typeId) => ReadInt(
            SaveKeys.AnimalPenLimit(typeId), LegacySaveKeys.AnimalPenLimit(typeId));

        public static void SetPenLimit(int typeId, int value) => WriteInt(
            SaveKeys.AnimalPenLimit(typeId), LegacySaveKeys.AnimalPenLimit(typeId), value);

        public static int GetAnimalQuantity(int typeId) => ReadInt(
            SaveKeys.FarmAnimalQuantity(typeId), LegacySaveKeys.FarmAnimalQuantity(typeId));

        public static void SetAnimalQuantity(int typeId, int value) => WriteInt(
            SaveKeys.FarmAnimalQuantity(typeId), LegacySaveKeys.FarmAnimalQuantity(typeId), value);

        public static int GetAnimalLimit(int typeId) => ReadInt(
            SaveKeys.FarmAnimalLimit(typeId), LegacySaveKeys.FarmAnimalLimit(typeId));

        public static void SetAnimalLimit(int typeId, int value) => WriteInt(
            SaveKeys.FarmAnimalLimit(typeId), LegacySaveKeys.FarmAnimalLimit(typeId), value);

        public static int GetPenPrice(int typeId) => ReadInt(
            SaveKeys.AnimalPenPrice(typeId), LegacySaveKeys.AnimalPenPrice(typeId));

        public static void SetPenPrice(int typeId, int value) => WriteInt(
            SaveKeys.AnimalPenPrice(typeId), LegacySaveKeys.AnimalPenPrice(typeId), value);

        public static int GetAnimalPrice(int typeId) => ReadInt(
            SaveKeys.FarmAnimalPrice(typeId), LegacySaveKeys.FarmAnimalPrice(typeId));

        public static void SetAnimalPrice(int typeId, int value) => WriteInt(
            SaveKeys.FarmAnimalPrice(typeId), LegacySaveKeys.FarmAnimalPrice(typeId), value);

        public static float GetPenPositionX(string stableId, string legacyPenName) => ReadFloat(
            SaveKeys.AnimalPenPositionX(stableId), LegacySaveKeys.AnimalPenPositionX(legacyPenName));

        public static float GetPenPositionY(string stableId, string legacyPenName) => ReadFloat(
            SaveKeys.AnimalPenPositionY(stableId), LegacySaveKeys.AnimalPenPositionY(legacyPenName));

        public static void SetPenPosition(string stableId, string legacyPenName, Vector2 position)
        {
            WriteFloat(SaveKeys.AnimalPenPositionX(stableId), LegacySaveKeys.AnimalPenPositionX(legacyPenName), position.x);
            WriteFloat(SaveKeys.AnimalPenPositionY(stableId), LegacySaveKeys.AnimalPenPositionY(legacyPenName), position.y);
        }

        public static bool IsPenFlipped(string stableId, string legacyPenName) => ReadInt(
            SaveKeys.AnimalPenFlipped(stableId), LegacySaveKeys.AnimalPenFlipped(legacyPenName)) != 0;

        public static void SetPenFlipped(string stableId, string legacyPenName, bool flipped)
        {
            PlayerPrefs.SetInt(SaveKeys.AnimalPenFlipped(stableId), flipped ? 1 : 0);
            if (flipped)
                PlayerPrefs.SetInt(LegacySaveKeys.AnimalPenFlipped(legacyPenName), 1);
            else
                PlayerPrefs.DeleteKey(LegacySaveKeys.AnimalPenFlipped(legacyPenName));
        }

        public static int GetPenOccupancy(string stableId, string legacyPenName) => ReadInt(
            SaveKeys.AnimalPenOccupancy(stableId), LegacySaveKeys.AnimalPenOccupancy(legacyPenName), -1);

        public static void SetPenOccupancy(string stableId, string legacyPenName, int value) => WriteInt(
            SaveKeys.AnimalPenOccupancy(stableId), LegacySaveKeys.AnimalPenOccupancy(legacyPenName), value);

        public static string GetAnimalState(string penStableId, int slotIndex, string legacyAnimalName, string legacyPenName) => ReadString(
            SaveKeys.FarmAnimalState(penStableId, slotIndex), LegacySaveKeys.FarmAnimalState(legacyAnimalName, legacyPenName), string.Empty);

        public static bool HasAnimalState(string penStableId, int slotIndex, string legacyAnimalName, string legacyPenName) =>
            PlayerPrefs.HasKey(SaveKeys.FarmAnimalState(penStableId, slotIndex)) ||
            PlayerPrefs.HasKey(LegacySaveKeys.FarmAnimalState(legacyAnimalName, legacyPenName));

        public static void SetAnimalState(string penStableId, int slotIndex, string legacyAnimalName, string legacyPenName, string value) => WriteString(
            SaveKeys.FarmAnimalState(penStableId, slotIndex), LegacySaveKeys.FarmAnimalState(legacyAnimalName, legacyPenName), value);

        public static int GetAnimalRemainingTime(string penStableId, int slotIndex, string legacyAnimalName, string legacyPenName) => ReadInt(
            SaveKeys.FarmAnimalRemainingTime(penStableId, slotIndex), LegacySaveKeys.FarmAnimalRemainingTime(legacyAnimalName, legacyPenName));

        public static void SetAnimalRemainingTime(string penStableId, int slotIndex, string legacyAnimalName, string legacyPenName, int value) => WriteInt(
            SaveKeys.FarmAnimalRemainingTime(penStableId, slotIndex), LegacySaveKeys.FarmAnimalRemainingTime(legacyAnimalName, legacyPenName), value);

        public static int GetAnimalLastTimestamp(string penStableId, int slotIndex, string legacyAnimalName, string legacyPenName) => ReadInt(
            SaveKeys.FarmAnimalLastTimestamp(penStableId, slotIndex), LegacySaveKeys.FarmAnimalLastTimestamp(legacyAnimalName, legacyPenName));

        public static void SetAnimalLastTimestamp(string penStableId, int slotIndex, string legacyAnimalName, string legacyPenName, int value) => WriteInt(
            SaveKeys.FarmAnimalLastTimestamp(penStableId, slotIndex), LegacySaveKeys.FarmAnimalLastTimestamp(legacyAnimalName, legacyPenName), value);

        private static int ReadInt(string englishKey, string legacyKey, int defaultValue = 0)
        {
            if (PlayerPrefs.HasKey(englishKey)) return PlayerPrefs.GetInt(englishKey, defaultValue);
            var value = PlayerPrefs.GetInt(legacyKey, defaultValue);
            if (PlayerPrefs.HasKey(legacyKey)) PlayerPrefs.SetInt(englishKey, value);
            return value;
        }

        private static float ReadFloat(string englishKey, string legacyKey)
        {
            if (PlayerPrefs.HasKey(englishKey)) return PlayerPrefs.GetFloat(englishKey);
            var value = PlayerPrefs.GetFloat(legacyKey);
            if (PlayerPrefs.HasKey(legacyKey)) PlayerPrefs.SetFloat(englishKey, value);
            return value;
        }

        private static string ReadString(string englishKey, string legacyKey, string defaultValue)
        {
            if (PlayerPrefs.HasKey(englishKey)) return PlayerPrefs.GetString(englishKey, defaultValue);
            var value = PlayerPrefs.GetString(legacyKey, defaultValue);
            if (PlayerPrefs.HasKey(legacyKey)) PlayerPrefs.SetString(englishKey, value);
            return value;
        }

        private static void WriteInt(string englishKey, string legacyKey, int value)
        {
            PlayerPrefs.SetInt(englishKey, value);
            PlayerPrefs.SetInt(legacyKey, value);
        }

        private static void WriteFloat(string englishKey, string legacyKey, float value)
        {
            PlayerPrefs.SetFloat(englishKey, value);
            PlayerPrefs.SetFloat(legacyKey, value);
        }

        private static void WriteString(string englishKey, string legacyKey, string value)
        {
            PlayerPrefs.SetString(englishKey, value);
            PlayerPrefs.SetString(legacyKey, value);
        }

        private static void ValidateIndex(int value, string parameterName)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(parameterName);
        }
    }
}
