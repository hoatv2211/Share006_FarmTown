using System;
using UnityEngine;

namespace FarmTown.Save
{
    public static class ProductionPersistence
    {
        private const string StableIdPrefix = "production-building-";
        private const string LegacyNamePrefix = "nhamay";

        public static string DefaultStableId(int typeId, int instanceIndex)
        {
            ValidateIndex(typeId, nameof(typeId));
            ValidateIndex(instanceIndex, nameof(instanceIndex));
            return $"{StableIdPrefix}{typeId}-{instanceIndex}";
        }

        public static string DefaultLegacyName(int typeId, int instanceIndex)
        {
            ValidateIndex(typeId, nameof(typeId));
            ValidateIndex(instanceIndex, nameof(instanceIndex));
            return $"{LegacyNamePrefix}{typeId}{instanceIndex}";
        }

        public static int GetBuildingCount(int typeId) => ReadInt(
            SaveKeys.FactoryCount(typeId), LegacySaveKeys.FactoryCount(typeId));

        public static void SetBuildingCount(int typeId, int value) => WriteInt(
            SaveKeys.FactoryCount(typeId), LegacySaveKeys.FactoryCount(typeId), value);

        public static float GetPositionX(string stableId, string legacyName) => TryGetIndices(stableId, out var typeId, out var instanceIndex)
            ? ReadFloat(SaveKeys.ProductionBuildingPositionX(stableId), SaveKeys.FactoryPositionX(typeId, instanceIndex), LegacySaveKeys.ProductionBuildingPositionX(legacyName))
            : ReadFloat(SaveKeys.ProductionBuildingPositionX(stableId), LegacySaveKeys.ProductionBuildingPositionX(legacyName));

        public static float GetPositionY(string stableId, string legacyName) => TryGetIndices(stableId, out var typeId, out var instanceIndex)
            ? ReadFloat(SaveKeys.ProductionBuildingPositionY(stableId), SaveKeys.FactoryPositionY(typeId, instanceIndex), LegacySaveKeys.ProductionBuildingPositionY(legacyName))
            : ReadFloat(SaveKeys.ProductionBuildingPositionY(stableId), LegacySaveKeys.ProductionBuildingPositionY(legacyName));

        public static void SetPosition(string stableId, string legacyName, Vector2 position)
        {
            ValidateIdentity(stableId, legacyName);
            WriteFloat(SaveKeys.ProductionBuildingPositionX(stableId), LegacySaveKeys.ProductionBuildingPositionX(legacyName), position.x);
            WriteFloat(SaveKeys.ProductionBuildingPositionY(stableId), LegacySaveKeys.ProductionBuildingPositionY(legacyName), position.y);
            if (TryGetIndices(stableId, out var typeId, out var instanceIndex))
            {
                PlayerPrefs.SetFloat(SaveKeys.FactoryPositionX(typeId, instanceIndex), position.x);
                PlayerPrefs.SetFloat(SaveKeys.FactoryPositionY(typeId, instanceIndex), position.y);
            }
        }

        public static bool IsFlipped(string stableId, string legacyName) => ReadInt(
            SaveKeys.ProductionBuildingFlipped(stableId), LegacySaveKeys.ProductionBuildingFlipped(legacyName)) != 0;

        public static void SetFlipped(string stableId, string legacyName, bool flipped)
        {
            ValidateIdentity(stableId, legacyName);
            PlayerPrefs.SetInt(SaveKeys.ProductionBuildingFlipped(stableId), flipped ? 1 : 0);
            if (flipped) PlayerPrefs.SetInt(LegacySaveKeys.ProductionBuildingFlipped(legacyName), 1);
            else PlayerPrefs.DeleteKey(LegacySaveKeys.ProductionBuildingFlipped(legacyName));
        }

        public static int GetQueueCapacity(string stableId, string legacyName) => ReadInt(
            SaveKeys.ProductionQueueCapacity(stableId), LegacySaveKeys.ProductionQueueCapacity(legacyName));

        public static void SetQueueCapacity(string stableId, string legacyName, int value) => WriteInt(
            SaveKeys.ProductionQueueCapacity(stableId), LegacySaveKeys.ProductionQueueCapacity(legacyName), value);

        public static int GetQueueCount(string stableId, string legacyName) => TryGetIndices(stableId, out var typeId, out var instanceIndex)
            ? ReadInt(SaveKeys.ProductionQueueCount(stableId), SaveKeys.FactoryQueueCount(typeId, instanceIndex), LegacySaveKeys.ProductionQueueCount(legacyName))
            : ReadInt(SaveKeys.ProductionQueueCount(stableId), LegacySaveKeys.ProductionQueueCount(legacyName));

        public static void SetQueueCount(string stableId, string legacyName, int value)
        {
            WriteInt(SaveKeys.ProductionQueueCount(stableId), LegacySaveKeys.ProductionQueueCount(legacyName), value);
            if (TryGetIndices(stableId, out var typeId, out var instanceIndex))
                PlayerPrefs.SetInt(SaveKeys.FactoryQueueCount(typeId, instanceIndex), value);
        }

        public static bool HasQueuedProduct(string stableId, int slotIndex, string legacyName)
        {
            ValidateSlot(slotIndex);
            if (PlayerPrefs.HasKey(SaveKeys.ProductionQueueProductId(stableId, slotIndex)) ||
                PlayerPrefs.HasKey(LegacySaveKeys.ProductionQueueProductId(legacyName, slotIndex))) return true;
            return TryGetIndices(stableId, out var typeId, out var instanceIndex) &&
                PlayerPrefs.HasKey(SaveKeys.FactoryQueueProductId(typeId, instanceIndex, slotIndex));
        }

        public static int GetQueuedProduct(string stableId, int slotIndex, string legacyName)
        {
            ValidateSlot(slotIndex);
            return TryGetIndices(stableId, out var typeId, out var instanceIndex)
                ? ReadInt(SaveKeys.ProductionQueueProductId(stableId, slotIndex), SaveKeys.FactoryQueueProductId(typeId, instanceIndex, slotIndex), LegacySaveKeys.ProductionQueueProductId(legacyName, slotIndex))
                : ReadInt(SaveKeys.ProductionQueueProductId(stableId, slotIndex), LegacySaveKeys.ProductionQueueProductId(legacyName, slotIndex));
        }

        public static void SetQueuedProduct(string stableId, int slotIndex, string legacyName, int productId)
        {
            ValidateSlot(slotIndex);
            WriteInt(SaveKeys.ProductionQueueProductId(stableId, slotIndex), LegacySaveKeys.ProductionQueueProductId(legacyName, slotIndex), productId);
            if (TryGetIndices(stableId, out var typeId, out var instanceIndex))
                PlayerPrefs.SetInt(SaveKeys.FactoryQueueProductId(typeId, instanceIndex, slotIndex), productId);
        }

        public static void DeleteQueuedProduct(string stableId, int slotIndex, string legacyName)
        {
            ValidateSlot(slotIndex);
            PlayerPrefs.DeleteKey(SaveKeys.ProductionQueueProductId(stableId, slotIndex));
            PlayerPrefs.DeleteKey(LegacySaveKeys.ProductionQueueProductId(legacyName, slotIndex));
            if (TryGetIndices(stableId, out var typeId, out var instanceIndex))
                PlayerPrefs.DeleteKey(SaveKeys.FactoryQueueProductId(typeId, instanceIndex, slotIndex));
        }

        public static bool HasCompletedProduct(string stableId, int slotIndex, string legacyName)
        {
            ValidateSlot(slotIndex);
            return PlayerPrefs.HasKey(SaveKeys.ProductionCompletedProductId(stableId, slotIndex)) ||
                PlayerPrefs.HasKey(LegacySaveKeys.ProductionCompletedProductId(legacyName, slotIndex));
        }

        public static int GetCompletedProduct(string stableId, int slotIndex, string legacyName)
        {
            ValidateSlot(slotIndex);
            return ReadInt(SaveKeys.ProductionCompletedProductId(stableId, slotIndex), LegacySaveKeys.ProductionCompletedProductId(legacyName, slotIndex));
        }

        public static void SetCompletedProduct(string stableId, int slotIndex, string legacyName, int productId)
        {
            ValidateSlot(slotIndex);
            WriteInt(SaveKeys.ProductionCompletedProductId(stableId, slotIndex), LegacySaveKeys.ProductionCompletedProductId(legacyName, slotIndex), productId);
        }

        public static void DeleteCompletedProduct(string stableId, int slotIndex, string legacyName)
        {
            ValidateSlot(slotIndex);
            PlayerPrefs.DeleteKey(SaveKeys.ProductionCompletedProductId(stableId, slotIndex));
            PlayerPrefs.DeleteKey(LegacySaveKeys.ProductionCompletedProductId(legacyName, slotIndex));
        }

        public static int GetRemainingTime(string stableId, string legacyName) => ReadInt(
            SaveKeys.ProductionRemainingTime(stableId), LegacySaveKeys.ProductionRemainingTime(legacyName));

        public static void SetRemainingTime(string stableId, string legacyName, int value) => WriteInt(
            SaveKeys.ProductionRemainingTime(stableId), LegacySaveKeys.ProductionRemainingTime(legacyName), value);

        public static int GetLastTimestamp(string stableId, string legacyName) => ReadInt(
            SaveKeys.ProductionLastTimestamp(stableId), LegacySaveKeys.ProductionLastTimestamp(legacyName));

        public static void SetLastTimestamp(string stableId, string legacyName, int value) => WriteInt(
            SaveKeys.ProductionLastTimestamp(stableId), LegacySaveKeys.ProductionLastTimestamp(legacyName), value);

        public static string GetSelectedBuilding(string defaultValue = "") => PlayerPrefs.HasKey(SaveKeys.SelectedProductionBuilding)
            ? PlayerPrefs.GetString(SaveKeys.SelectedProductionBuilding, defaultValue)
            : BackfillSelectedBuilding(defaultValue);

        public static void SetSelectedBuilding(string stableId, string legacyName)
        {
            ValidateIdentity(stableId, legacyName);
            PlayerPrefs.SetString(SaveKeys.SelectedProductionBuilding, stableId);
            PlayerPrefs.SetString(LegacySaveKeys.SelectedProductionBuilding, legacyName);
        }

        private static string BackfillSelectedBuilding(string defaultValue)
        {
            var value = PlayerPrefs.GetString(LegacySaveKeys.SelectedProductionBuilding, defaultValue);
            if (!PlayerPrefs.HasKey(LegacySaveKeys.SelectedProductionBuilding)) return value;
            if (!TryResolveStableId(value, out var stableId)) return defaultValue;
            PlayerPrefs.SetString(SaveKeys.SelectedProductionBuilding, stableId);
            return stableId;
        }

        public static bool TryResolveStableId(string legacyName, out string stableId)
        {
            stableId = string.Empty;
            for (var typeId = 0; typeId < 7; typeId++)
                for (var instanceIndex = 0; instanceIndex < 32; instanceIndex++)
                    if (string.Equals(legacyName, DefaultLegacyName(typeId, instanceIndex), StringComparison.Ordinal))
                    {
                        stableId = DefaultStableId(typeId, instanceIndex);
                        return true;
                    }
            return false;
        }

        private static int ReadInt(string englishKey, string legacyKey)
        {
            if (PlayerPrefs.HasKey(englishKey)) return PlayerPrefs.GetInt(englishKey);
            var value = PlayerPrefs.GetInt(legacyKey);
            if (PlayerPrefs.HasKey(legacyKey)) PlayerPrefs.SetInt(englishKey, value);
            return value;
        }

        private static int ReadInt(string englishKey, string transitionalKey, string legacyKey)
        {
            if (PlayerPrefs.HasKey(englishKey)) return PlayerPrefs.GetInt(englishKey);
            if (PlayerPrefs.HasKey(transitionalKey))
            {
                var transitionalValue = PlayerPrefs.GetInt(transitionalKey);
                PlayerPrefs.SetInt(englishKey, transitionalValue);
                return transitionalValue;
            }
            return ReadInt(englishKey, legacyKey);
        }

        private static float ReadFloat(string englishKey, string legacyKey)
        {
            if (PlayerPrefs.HasKey(englishKey)) return PlayerPrefs.GetFloat(englishKey);
            var value = PlayerPrefs.GetFloat(legacyKey);
            if (PlayerPrefs.HasKey(legacyKey)) PlayerPrefs.SetFloat(englishKey, value);
            return value;
        }

        private static float ReadFloat(string englishKey, string transitionalKey, string legacyKey)
        {
            if (PlayerPrefs.HasKey(englishKey)) return PlayerPrefs.GetFloat(englishKey);
            if (PlayerPrefs.HasKey(transitionalKey))
            {
                var transitionalValue = PlayerPrefs.GetFloat(transitionalKey);
                PlayerPrefs.SetFloat(englishKey, transitionalValue);
                return transitionalValue;
            }
            return ReadFloat(englishKey, legacyKey);
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

        private static void ValidateIdentity(string stableId, string legacyName)
        {
            if (string.IsNullOrWhiteSpace(stableId)) throw new ArgumentException("Stable ID is required.", nameof(stableId));
            if (string.IsNullOrWhiteSpace(legacyName)) throw new ArgumentException("Legacy name is required.", nameof(legacyName));
        }

        private static void ValidateSlot(int slotIndex)
        {
            if (slotIndex < 1) throw new ArgumentOutOfRangeException(nameof(slotIndex));
        }

        private static void ValidateIndex(int value, string parameterName)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(parameterName);
        }

        private static bool TryGetIndices(string stableId, out int typeId, out int instanceIndex)
        {
            typeId = -1;
            instanceIndex = -1;
            if (string.IsNullOrEmpty(stableId) || !stableId.StartsWith(StableIdPrefix, StringComparison.Ordinal)) return false;
            var parts = stableId.Substring(StableIdPrefix.Length).Split('-');
            return parts.Length == 2 && int.TryParse(parts[0], out typeId) && typeId >= 0 &&
                int.TryParse(parts[1], out instanceIndex) && instanceIndex >= 0 &&
                string.Equals(stableId, DefaultStableId(typeId, instanceIndex), StringComparison.Ordinal);
        }
    }
}
