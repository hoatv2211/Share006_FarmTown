using System;

namespace FarmTown.Save
{
    public sealed class SaveMigrationService
    {
        public const int LatestSchemaVersion = 2;
        private const int CropTypeCount = 22;
        private const int ProductTypeCount = 100;
        private const int FactoryTypeCount = 7;
        private const int AnimalTypeCount = 7;
        private const int DecorationTypeCount = 7;
        private const int OrderSlotCount = 8;
        private const int MiniGameVersionCount = 3;
        private const int MaximumFactoryInstancesPerType = 32;
        private const int MaximumFactoryQueueSlots = 16;

        private readonly ISaveStore store;

        public SaveMigrationService(ISaveStore store)
        {
            this.store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public void MigrateToLatest()
        {
            var version = store.GetInt(SaveKeys.SchemaVersion, 0);
            if (version < 1)
            {
                MigrateSchemaZeroToOne();
                version = 1;
                store.SetInt(SaveKeys.SchemaVersion, version);
            }
            if (version < 2)
            {
                MigrateSchemaOneToTwo();
                version = 2;
                store.SetInt(SaveKeys.SchemaVersion, version);
            }

            MigrateFarmingPlotCompatibility();
            store.Save();
        }

        private void MigrateSchemaOneToTwo()
        {
            for (var factoryTypeId = 0; factoryTypeId < FactoryTypeCount; factoryTypeId++)
                MigrateFactoryType(factoryTypeId);
        }

        private void MigrateSchemaZeroToOne()
        {
            CopyInt(LegacySaveKeys.Gold, SaveKeys.WalletCoins);
            CopyInt(LegacySaveKeys.Diamond, SaveKeys.WalletGems);
            CopyInt(LegacySaveKeys.Level, SaveKeys.ProgressionLevel);
            CopyInt(LegacySaveKeys.Experience, SaveKeys.ProgressionExperience);
            CopyInt(LegacySaveKeys.ExperienceLimit, SaveKeys.ProgressionExperienceLimit);
            CopyInt(LegacySaveKeys.TutorialStep, SaveKeys.TutorialStep);
            CopyInt(LegacySaveKeys.PlotCount, SaveKeys.PlotCount);
            CopyInt(LegacySaveKeys.OrderCount, SaveKeys.OrderCount);
            CopyInt(LegacySaveKeys.LandExpansionLevel, SaveKeys.LandExpansionLevel);
            CopyInt(LegacySaveKeys.LandExpansionUnlocked, SaveKeys.LandExpansionUnlocked);
            CopyInt(LegacySaveKeys.LandExpansionPrice, SaveKeys.LandExpansionPrice);
            CopyInt(LegacySaveKeys.MiniGame2Level, SaveKeys.MiniGame2Level);
            CopyInt(LegacySaveKeys.MiniGame3Level, SaveKeys.MiniGame3Level);

            for (var cropId = 0; cropId < CropTypeCount; cropId++)
                CopyInt(LegacySaveKeys.CropQuantity(cropId), SaveKeys.CropQuantity(cropId));

            for (var productId = 0; productId < ProductTypeCount; productId++)
                CopyInt(LegacySaveKeys.ProductQuantity(productId), SaveKeys.ProductQuantity(productId));

            for (var animalTypeId = 0; animalTypeId < AnimalTypeCount; animalTypeId++)
            {
                CopyInt(LegacySaveKeys.AnimalPenQuantity(animalTypeId), SaveKeys.AnimalPenQuantity(animalTypeId));
                CopyInt(LegacySaveKeys.AnimalPenLimit(animalTypeId), SaveKeys.AnimalPenLimit(animalTypeId));
                CopyInt(LegacySaveKeys.FarmAnimalQuantity(animalTypeId), SaveKeys.FarmAnimalQuantity(animalTypeId));
                CopyInt(LegacySaveKeys.FarmAnimalLimit(animalTypeId), SaveKeys.FarmAnimalLimit(animalTypeId));
            }

            for (var decorationTypeId = 0; decorationTypeId < DecorationTypeCount; decorationTypeId++)
            {
                CopyInt(LegacySaveKeys.DecorationQuantity(decorationTypeId), SaveKeys.DecorationQuantity(decorationTypeId));
                CopyInt(LegacySaveKeys.DecorationLimit(decorationTypeId), SaveKeys.DecorationLimit(decorationTypeId));
            }

            for (var orderId = 0; orderId < OrderSlotCount; orderId++)
            {
                CopyInt(LegacySaveKeys.OrderState(orderId), SaveKeys.OrderState(orderId));
                CopyInt(LegacySaveKeys.OrderDeliveryState(orderId), SaveKeys.OrderDeliveryState(orderId));
                CopyInt(LegacySaveKeys.OrderRemainingTime(orderId), SaveKeys.OrderRemainingTime(orderId));
            }

            for (var version = 1; version <= MiniGameVersionCount; version++)
            {
                CopyInt(LegacySaveKeys.MiniGameEnabled(version), SaveKeys.MiniGameEnabled(version));
                CopyInt(LegacySaveKeys.MiniGameLevel(version), SaveKeys.MiniGameLevel(version));
            }

            var plotCount = store.GetInt(LegacySaveKeys.PlotCount, 0);
            for (var plotId = 0; plotId < plotCount; plotId++)
            {
                CopyFloat(LegacySaveKeys.PlotPositionX(plotId), SaveKeys.PlotPositionX(plotId));
                CopyFloat(LegacySaveKeys.PlotPositionY(plotId), SaveKeys.PlotPositionY(plotId));
            }

            for (var factoryTypeId = 0; factoryTypeId < FactoryTypeCount; factoryTypeId++)
                MigrateFactoryType(factoryTypeId);
        }

        private void MigrateFarmingPlotCompatibility()
        {
            var hasPlotCount = store.HasKey(SaveKeys.PlotCount) || store.HasKey(LegacySaveKeys.PlotCount);
            var plotCount = hasPlotCount
                ? Math.Max(FarmingPlotPersistence.DefaultPlotCount, Math.Max(
                    store.GetInt(SaveKeys.PlotCount, 0),
                    store.GetInt(LegacySaveKeys.PlotCount, 0)))
                : 0;
            if (hasPlotCount)
            {
                store.SetInt(SaveKeys.PlotCount, plotCount);
                store.SetInt(LegacySaveKeys.PlotCount, plotCount);
            }

            for (var index = 0; index < plotCount; index++)
            {
                var stableId = FarmingPlotPersistence.DefaultStableId(index);
                var legacyAlias = FarmingPlotPersistence.DefaultLegacyAlias(index);
                CopyFloat(LegacySaveKeys.PlotPositionX(index), SaveKeys.PlotPositionX(stableId));
                CopyFloat(LegacySaveKeys.PlotPositionY(index), SaveKeys.PlotPositionY(stableId));
                CopyInt(LegacySaveKeys.PlotCropId(legacyAlias), SaveKeys.PlotCropId(stableId));
                CopyInt(LegacySaveKeys.PlotRemainingTime(legacyAlias), SaveKeys.PlotRemainingTime(stableId));
                CopyInt(LegacySaveKeys.PlotLastTimestamp(legacyAlias), SaveKeys.PlotLastTimestamp(stableId));
            }

            CopyStablePlotReference(LegacySaveKeys.SelectedPlot, SaveKeys.SelectedPlot);
            CopyInt(LegacySaveKeys.OutOfSeedTargetCount, SaveKeys.OutOfSeedTargetCount);

            var targetCount = store.HasKey(SaveKeys.OutOfSeedTargetCount)
                ? store.GetInt(SaveKeys.OutOfSeedTargetCount, 0)
                : store.GetInt(LegacySaveKeys.OutOfSeedTargetCount, 0);
            for (var index = 0; index < targetCount; index++)
                CopyStablePlotReference(LegacySaveKeys.OutOfSeedTarget(index), SaveKeys.OutOfSeedTarget(index));
        }

        private void CopyStablePlotReference(string legacyKey, string englishKey)
        {
            if (!store.HasKey(legacyKey) || store.HasKey(englishKey))
                return;

            var legacyAlias = store.GetString(legacyKey);
            if (!FarmingPlotPersistence.TryGetStableId(legacyAlias, out var stableId))
                return;

            store.SetString(englishKey, stableId);
            if (store.GetString(englishKey) != stableId)
                throw new InvalidOperationException($"Failed to migrate plot reference {legacyKey} to {englishKey}");
        }
        private void MigrateFactoryType(int factoryTypeId)
        {
            var legacyCountKey = LegacySaveKeys.FactoryCount(factoryTypeId);
            CopyInt(legacyCountKey, SaveKeys.FactoryCount(factoryTypeId));
            var instanceCount = Math.Min(
                Math.Max(store.GetInt(legacyCountKey, 0), store.GetInt(SaveKeys.FactoryCount(factoryTypeId), 0)),
                MaximumFactoryInstancesPerType);

            for (var instanceId = 0; instanceId < instanceCount; instanceId++)
            {
                var stableId = $"production-building-{factoryTypeId}-{instanceId}";
                var legacyName = $"nhamay{factoryTypeId}{instanceId}";
                CopyFloat(
                    LegacySaveKeys.FactoryPositionX(factoryTypeId, instanceId),
                    SaveKeys.FactoryPositionX(factoryTypeId, instanceId));
                CopyPreferredFloat(
                    SaveKeys.FactoryPositionX(factoryTypeId, instanceId),
                    LegacySaveKeys.ProductionBuildingPositionX(legacyName),
                    SaveKeys.ProductionBuildingPositionX(stableId));
                CopyFloat(
                    LegacySaveKeys.FactoryPositionY(factoryTypeId, instanceId),
                    SaveKeys.FactoryPositionY(factoryTypeId, instanceId));
                CopyPreferredFloat(
                    SaveKeys.FactoryPositionY(factoryTypeId, instanceId),
                    LegacySaveKeys.ProductionBuildingPositionY(legacyName),
                    SaveKeys.ProductionBuildingPositionY(stableId));
                CopyInt(LegacySaveKeys.ProductionBuildingFlipped(legacyName), SaveKeys.ProductionBuildingFlipped(stableId));
                CopyInt(LegacySaveKeys.ProductionQueueCapacity(legacyName), SaveKeys.ProductionQueueCapacity(stableId));
                CopyInt(LegacySaveKeys.ProductionRemainingTime(legacyName), SaveKeys.ProductionRemainingTime(stableId));
                CopyInt(LegacySaveKeys.ProductionLastTimestamp(legacyName), SaveKeys.ProductionLastTimestamp(stableId));

                var legacyQueueCountKey = LegacySaveKeys.FactoryQueueCount(factoryTypeId, instanceId);
                CopyInt(legacyQueueCountKey, SaveKeys.FactoryQueueCount(factoryTypeId, instanceId));
                CopyPreferredInt(SaveKeys.FactoryQueueCount(factoryTypeId, instanceId), legacyQueueCountKey, SaveKeys.ProductionQueueCount(stableId));
                var queueCount = Math.Min(
                    store.GetInt(SaveKeys.ProductionQueueCount(stableId), 0),
                    MaximumFactoryQueueSlots);

                for (var queueIndex = 1; queueIndex <= queueCount; queueIndex++)
                {
                    CopyInt(
                        LegacySaveKeys.FactoryQueueProductId(queueIndex, factoryTypeId, instanceId),
                        SaveKeys.FactoryQueueProductId(factoryTypeId, instanceId, queueIndex));
                    CopyPreferredInt(
                        SaveKeys.FactoryQueueProductId(factoryTypeId, instanceId, queueIndex),
                        LegacySaveKeys.ProductionQueueProductId(legacyName, queueIndex),
                        SaveKeys.ProductionQueueProductId(stableId, queueIndex));
                }

                for (var slotIndex = 1; slotIndex <= MaximumFactoryQueueSlots; slotIndex++)
                    CopyInt(
                        LegacySaveKeys.ProductionCompletedProductId(legacyName, slotIndex),
                        SaveKeys.ProductionCompletedProductId(stableId, slotIndex));

                if (store.GetString(LegacySaveKeys.SelectedProductionBuilding) == legacyName &&
                    !store.HasKey(SaveKeys.SelectedProductionBuilding))
                    store.SetString(SaveKeys.SelectedProductionBuilding, stableId);
            }
        }

        private void CopyInt(string legacyKey, string englishKey)
        {
            if (!store.HasKey(legacyKey) || store.HasKey(englishKey))
                return;

            var value = store.GetInt(legacyKey);
            store.SetInt(englishKey, value);
            if (store.GetInt(englishKey) != value)
                throw new InvalidOperationException($"Failed to migrate integer key {legacyKey} to {englishKey}");
        }

        private void CopyPreferredInt(string preferredKey, string fallbackKey, string targetKey)
        {
            if (store.HasKey(targetKey)) return;
            if (store.HasKey(preferredKey))
            {
                store.SetInt(targetKey, store.GetInt(preferredKey));
                return;
            }
            CopyInt(fallbackKey, targetKey);
        }

        private void CopyPreferredFloat(string preferredKey, string fallbackKey, string targetKey)
        {
            if (store.HasKey(targetKey)) return;
            if (store.HasKey(preferredKey))
            {
                store.SetFloat(targetKey, store.GetFloat(preferredKey));
                return;
            }
            CopyFloat(fallbackKey, targetKey);
        }

        private void CopyFloat(string legacyKey, string englishKey)
        {
            if (!store.HasKey(legacyKey) || store.HasKey(englishKey))
                return;

            var value = store.GetFloat(legacyKey);
            store.SetFloat(englishKey, value);
            if (!store.GetFloat(englishKey).Equals(value))
                throw new InvalidOperationException($"Failed to migrate float key {legacyKey} to {englishKey}");
        }
    }
}
