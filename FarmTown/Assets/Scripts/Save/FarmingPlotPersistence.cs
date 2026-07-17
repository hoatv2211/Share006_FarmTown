using System;
using System.Collections.Generic;
using UnityEngine;

namespace FarmTown.Save
{
    public static class FarmingPlotPersistence
    {
        public const int DefaultPlotCount = 6;
        private const string StableIdPrefix = "crop-plot-";
        private const string LegacyAliasPrefix = "odat";

        public static string DefaultStableId(int index)
        {
            ValidateIndex(index);
            return $"{StableIdPrefix}{index}";
        }

        public static string DefaultLegacyAlias(int index)
        {
            ValidateIndex(index);
            return $"{LegacyAliasPrefix}{index}";
        }

        public static bool HasSavedPlotCount() => PlayerPrefs.HasKey(SaveKeys.PlotCount) || PlayerPrefs.HasKey(LegacySaveKeys.PlotCount);

        public static int GetPlotCount() => Math.Max(DefaultPlotCount, Math.Max(
            PlayerPrefs.GetInt(SaveKeys.PlotCount, 0),
            PlayerPrefs.GetInt(LegacySaveKeys.PlotCount, 0)));

        public static void SetPlotCount(int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            PlayerPrefs.SetInt(SaveKeys.PlotCount, count);
            PlayerPrefs.SetInt(LegacySaveKeys.PlotCount, count);
        }

        public static int GetCropQuantity(int cropId) => PlayerPrefs.HasKey(SaveKeys.CropQuantity(cropId))
            ? PlayerPrefs.GetInt(SaveKeys.CropQuantity(cropId))
            : PlayerPrefs.GetInt(LegacySaveKeys.CropQuantity(cropId));

        public static void SetCropQuantity(int cropId, int quantity)
        {
            PlayerPrefs.SetInt(SaveKeys.CropQuantity(cropId), quantity);
            PlayerPrefs.SetInt(LegacySaveKeys.CropQuantity(cropId), quantity);
        }

        public static bool TryResolveStableInstances(IReadOnlyList<string> stableIds, out List<StableInstanceId> instances)
        {
            instances = new List<StableInstanceId>(stableIds.Count);
            foreach (var stableId in stableIds)
            {
                if (!StableInstanceId.TryFind(stableId, out var instance))
                {
                    instances.Clear();
                    return false;
                }
                instances.Add(instance);
            }
            return true;
        }

        public static bool TryGetStableId(string legacyAlias, out string stableId)
        {
            stableId = string.Empty;
            if (string.IsNullOrEmpty(legacyAlias) || !legacyAlias.StartsWith(LegacyAliasPrefix, StringComparison.Ordinal))
                return false;
            var suffix = legacyAlias.Substring(LegacyAliasPrefix.Length);
            if (!int.TryParse(suffix, out var index) || index < 0 || !string.Equals(legacyAlias, DefaultLegacyAlias(index), StringComparison.Ordinal))
                return false;
            stableId = DefaultStableId(index);
            return true;
        }

        public static bool TryGetIndex(string stableId, out int index)
        {
            index = -1;
            if (string.IsNullOrEmpty(stableId) || !stableId.StartsWith(StableIdPrefix, StringComparison.Ordinal))
                return false;
            var suffix = stableId.Substring(StableIdPrefix.Length);
            return int.TryParse(suffix, out index) && index >= 0 && string.Equals(stableId, DefaultStableId(index), StringComparison.Ordinal);
        }

        private static void ValidateIndex(int index)
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
        }
    }
}
