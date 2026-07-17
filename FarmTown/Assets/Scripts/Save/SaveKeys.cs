namespace FarmTown.Save
{
    public static class SaveKeys
    {
        public const string SchemaVersion = "save.schema.version";
        public const string WalletCoins = "wallet.coins";
        public const string WalletGems = "wallet.gems";
        public const string ProgressionLevel = "progression.level";
        public const string ProgressionExperience = "progression.experience";
        public const string ProgressionExperienceLimit = "progression.experienceLimit";
        public const string TutorialStep = "tutorial.step";
        public const string PlotCount = "world.cropPlot.count";
        public const string OrderCount = "orders.active.count";
        public const string LandExpansionLevel = "world.land.expansionLevel";
        public const string LandExpansionUnlocked = "world.land.unlocked";
        public const string LandExpansionPrice = "world.land.nextPrice";
        public const string MiniGame2Level = "minigame.2.level";
        public const string MiniGame3Level = "minigame.3.level";
        public const string SelectedPlot = "world.cropPlot.selected";
        public const string OutOfSeedTargetCount = "world.cropPlot.outOfSeedTarget.count";

        public static string CropQuantity(int cropId) => $"inventory.crop.{cropId}.quantity";
        public static string ProductQuantity(int productId) => $"inventory.product.{productId}.quantity";
        public static string AnimalPenQuantity(int typeId) => $"inventory.animalPen.{typeId}.quantity";
        public static string AnimalPenLimit(int typeId) => $"inventory.animalPen.{typeId}.limit";
        public static string FarmAnimalQuantity(int typeId) => $"inventory.farmAnimal.{typeId}.quantity";
        public static string FarmAnimalLimit(int typeId) => $"inventory.farmAnimal.{typeId}.limit";
        public static string AnimalPenPrice(int typeId) => $"shop.animalPen.{typeId}.price";
        public static string FarmAnimalPrice(int typeId) => $"shop.farmAnimal.{typeId}.price";
        public static string AnimalPenPositionX(string stableId) => $"animal.pen.{stableId}.position.x";
        public static string AnimalPenPositionY(string stableId) => $"animal.pen.{stableId}.position.y";
        public static string AnimalPenFlipped(string stableId) => $"animal.pen.{stableId}.flipped";
        public static string AnimalPenOccupancy(string stableId) => $"animal.pen.{stableId}.animalCount";
        public static string FarmAnimalState(string penStableId, int slotIndex) =>
            $"animal.instance.{penStableId}.{slotIndex}.state";
        public static string FarmAnimalRemainingTime(string penStableId, int slotIndex) =>
            $"animal.instance.{penStableId}.{slotIndex}.remainingSeconds";
        public static string FarmAnimalLastTimestamp(string penStableId, int slotIndex) =>
            $"animal.instance.{penStableId}.{slotIndex}.lastTimestamp";
        public static string DecorationQuantity(int typeId) => $"inventory.decoration.{typeId}.quantity";
        public static string DecorationLimit(int typeId) => $"inventory.decoration.{typeId}.limit";
        public static string OrderState(int orderId) => $"orders.{orderId}.state";
        public static string OrderDeliveryState(int orderId) => $"orders.{orderId}.deliveryState";
        public static string OrderRemainingTime(int orderId) => $"orders.{orderId}.remainingTime";
        public static string MiniGameEnabled(int version) => $"minigame.{version}.enabled";
        public static string MiniGameLevel(int version) => $"minigame.{version}.level";
        public static string PlotPositionX(int plotId) => $"world.cropPlot.{plotId}.position.x";
        public static string PlotPositionY(int plotId) => $"world.cropPlot.{plotId}.position.y";
        public static string PlotPositionX(string stableId) => $"world.cropPlot.{stableId}.position.x";
        public static string PlotPositionY(string stableId) => $"world.cropPlot.{stableId}.position.y";
        public static string PlotCropId(string stableId) => $"world.cropPlot.{stableId}.cropId";
        public static string PlotRemainingTime(string stableId) => $"world.cropPlot.{stableId}.remainingTime";
        public static string PlotLastTimestamp(string stableId) => $"world.cropPlot.{stableId}.lastTimestamp";
        public static string OutOfSeedTarget(int index) => $"world.cropPlot.outOfSeedTarget.{index}";
        public static string FactoryCount(int factoryTypeId) => $"world.productionBuilding.{factoryTypeId}.count";
        public static string FactoryPositionX(int factoryTypeId, int instanceId) =>
            $"world.productionBuilding.{factoryTypeId}.{instanceId}.position.x";
        public static string FactoryPositionY(int factoryTypeId, int instanceId) =>
            $"world.productionBuilding.{factoryTypeId}.{instanceId}.position.y";
        public static string FactoryQueueCount(int factoryTypeId, int instanceId) =>
            $"production.queue.{factoryTypeId}.{instanceId}.count";
        public static string FactoryQueueProductId(int factoryTypeId, int instanceId, int queueIndex) =>
            $"production.queue.{factoryTypeId}.{instanceId}.{queueIndex}.productId";
        public const string SelectedProductionBuilding = "production.selectedBuilding";
        public static string ProductionBuildingPositionX(string stableId) => $"production.building.{stableId}.position.x";
        public static string ProductionBuildingPositionY(string stableId) => $"production.building.{stableId}.position.y";
        public static string ProductionBuildingFlipped(string stableId) => $"production.building.{stableId}.flipped";
        public static string ProductionQueueCapacity(string stableId) => $"production.building.{stableId}.queue.capacity";
        public static string ProductionQueueCount(string stableId) => $"production.building.{stableId}.queue.count";
        public static string ProductionQueueProductId(string stableId, int slotIndex) =>
            $"production.building.{stableId}.queue.{slotIndex}.productId";
        public static string ProductionCompletedProductId(string stableId, int slotIndex) =>
            $"production.building.{stableId}.completed.{slotIndex}.productId";
        public static string ProductionRemainingTime(string stableId) => $"production.building.{stableId}.remainingTime";
        public static string ProductionLastTimestamp(string stableId) => $"production.building.{stableId}.lastTimestamp";
    }
}
