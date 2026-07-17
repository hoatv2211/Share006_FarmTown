namespace FarmTown.Save
{
    public static class LegacySaveKeys
    {
        public const string Gold = "gold";
        public const string Diamond = "diamond";
        public const string Level = "level";
        public const string Experience = "kinhnghiem";
        public const string ExperienceLimit = "kinhnghiemmax";
        public const string TutorialStep = "huongdan";
        public const string PlotCount = "soodat";
        public const string OrderCount = "sodonhang";
        public const string LandExpansionLevel = "capmoodat";
        public const string LandExpansionUnlocked = "movungdat";
        public const string LandExpansionPrice = "giamuaodat";
        public const string MiniGame2Level = "LevelOfMinigame2";
        public const string MiniGame3Level = "LevelMiniGame3";
        public const string SelectedPlot = "chonodat";
        public const string OutOfSeedTargetCount = "sohg";

        public static string CropQuantity(int id) => $"slnongsan{id}";
        public static string ProductQuantity(int id) => $"slvatpham{id}";
        public static string AnimalPenQuantity(int id) => $"slchuong{id}";
        public static string AnimalPenLimit(int id) => $"slchuongmax{id}";
        public static string FarmAnimalQuantity(int id) => $"slvatnuoi{id}";
        public static string FarmAnimalLimit(int id) => $"slvatnuoimax{id}";
        public static string AnimalPenPrice(int id) => $"giachuong{id}";
        public static string FarmAnimalPrice(int id) => $"giavatnuoi{id}";
        public static string AnimalPenPositionX(string penName) => $"x{penName}";
        public static string AnimalPenPositionY(string penName) => $"y{penName}";
        public static string AnimalPenFlipped(string penName) => $"quay{penName}";
        public static string AnimalPenOccupancy(string penName) => $"{penName}slvatnuoi";
        public static string FarmAnimalState(string animalName, string penName) => $"trangthai{animalName}{penName}";
        public static string FarmAnimalRemainingTime(string animalName, string penName) => $"timevatnuoi{animalName}{penName}";
        public static string FarmAnimalLastTimestamp(string animalName, string penName) => $"timevatnuoithoat{animalName}{penName}";
        public static string DecorationQuantity(int id) => $"sltrangtri{id}";
        public static string DecorationLimit(int id) => $"sltrangtrimax{id}";
        public static string OrderState(int id) => $"dh{id}";
        public static string OrderDeliveryState(int id) => $"dangtaidh{id}";
        public static string OrderRemainingTime(int id) => $"timedh{id}";
        public static string MiniGameEnabled(int version) => $"IsEnableMiniGame{version}";
        public static string MiniGameLevel(int version) => $"LevelMiniGame{version}";
        public static string PlotPositionX(int index) => $"xodat{index}";
        public static string PlotPositionY(int index) => $"yodat{index}";
        public static string PlotCropId(string legacyAlias) => $"idodat{legacyAlias}";
        public static string PlotRemainingTime(string legacyAlias) => $"timeodat{legacyAlias}";
        public static string PlotLastTimestamp(string legacyAlias) => $"timethoat{legacyAlias}";
        public static string OutOfSeedTarget(int index) => $"tenodatthieu{index}";
        public static string FactoryCount(int typeId) => $"slnhamay{typeId}";
        public static string FactoryPositionX(int typeId, int instanceId) => $"xnhamay{typeId}{instanceId}";
        public static string FactoryPositionY(int typeId, int instanceId) => $"ynhamay{typeId}{instanceId}";
        public static string FactoryQueueCount(int typeId, int instanceId) => $"dangsanxuatnhamay{typeId}{instanceId}";
        public static string FactoryQueueProductId(int queueIndex, int typeId, int instanceId) =>
            $"idsanxuat{queueIndex}nhamay{typeId}{instanceId}";
        public const string SelectedProductionBuilding = "chonnhamay";
        public static string ProductionBuildingPositionX(string legacyName) => $"x{legacyName}";
        public static string ProductionBuildingPositionY(string legacyName) => $"y{legacyName}";
        public static string ProductionBuildingFlipped(string legacyName) => $"quay{legacyName}";
        public static string ProductionQueueCapacity(string legacyName) => $"osanxuat{legacyName}";
        public static string ProductionQueueCount(string legacyName) => $"dangsanxuat{legacyName}";
        public static string ProductionQueueProductId(string legacyName, int slotIndex) => $"idsanxuat{slotIndex}{legacyName}";
        public static string ProductionCompletedProductId(string legacyName, int slotIndex) => $"idhoanthanh{slotIndex}{legacyName}";
        public static string ProductionRemainingTime(string legacyName) => $"time{legacyName}";
        public static string ProductionLastTimestamp(string legacyName) => $"timethoat{legacyName}";
    }
}
