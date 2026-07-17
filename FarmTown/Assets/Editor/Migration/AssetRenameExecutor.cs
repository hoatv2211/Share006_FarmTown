using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace FarmTown.Editor.Migration
{
    public static class AssetRenameExecutor
    {
        private const string LegacyBootstrapType = "Bat" + "DauGame";
        private const string LegacyNotificationType = "Thong" + "Bao";

        private static readonly IReadOnlyDictionary<string, string> CoreMoves =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [$"Assets/Scripts/Manager/{LegacyBootstrapType}.cs"] = "Assets/Scripts/Core/GameBootstrap.cs",
                ["Assets/Scripts/LoadMap.cs"] = "Assets/Scripts/Core/SceneLoader.cs",
                ["Assets/Scripts/TimeManager.cs"] = "Assets/Scripts/Core/GameTime.cs",
                ["Assets/Scripts/Sound.cs"] = "Assets/Scripts/Core/AudioManager.cs",
                [$"Assets/Scripts/{LegacyNotificationType}.cs"] = "Assets/Scripts/UI/NotificationController.cs",
                ["Assets/Scripts/DailyReward"] = "Assets/Scripts/Ads",
            };

        private static readonly IReadOnlyDictionary<string, string> FarmingMoves =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Assets/Scripts/Odat/Odat.cs"] = "Assets/Scripts/Farming/CropPlot.cs",
                ["Assets/Scripts/Odat/IHatGiong.cs"] = "Assets/Scripts/Farming/ISeedReceiver.cs",
                ["Assets/Scripts/Odat/HatGiongKeo.cs"] = "Assets/Scripts/Farming/SeedDragHandler.cs",
                ["Assets/Scripts/Odat/ItemHatGiong.cs"] = "Assets/Scripts/Farming/SeedItem.cs",
                ["Assets/Scripts/Odat/Liem.cs"] = "Assets/Scripts/Farming/SickleTool.cs",
                ["Assets/Scripts/Odat/DialogHetHG.cs"] = "Assets/Scripts/Farming/OutOfSeedsPanel.cs",
                ["Assets/Scripts/Odat/dialogHG.cs"] = "Assets/Scripts/Farming/SeedSelectionPanel.cs",
                ["Assets/Scripts/Odat/DialogTime.cs"] = "Assets/Scripts/Farming/CropTimerPanel.cs",
                ["Assets/Scripts/ClickOdat.cs"] = "Assets/Scripts/Farming/CropPlotClickHandler.cs",
                ["Assets/Data/Scripts/DataNongSan.cs"] = "Assets/Scripts/Farming/CropDatabase.cs",
            };

        private static readonly IReadOnlyDictionary<string, string> AnimalMoves =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Assets/Data/Scripts/DataVatNuoi.cs"] = "Assets/Scripts/Animals/AnimalDatabase.cs",
                ["Assets/Data/Data/VatNuoi.asset"] = "Assets/Data/Animals/AnimalDatabase.asset",
                ["Assets/Scripts/VatNuoi/VatNuoi.cs"] = "Assets/Scripts/Animals/FarmAnimal.cs",
                ["Assets/Scripts/VatNuoi/VatNuoiDrag.cs"] = "Assets/Scripts/Animals/AnimalDragHandler.cs",
                ["Assets/Scripts/VatNuoi/ChuongVatNuoi.cs"] = "Assets/Scripts/Animals/AnimalPen.cs",
                ["Assets/Scripts/VatNuoi/IThucAn.cs"] = "Assets/Scripts/Animals/IFeedReceiver.cs",
                ["Assets/Scripts/VatNuoi/ThucAnDrag.cs"] = "Assets/Scripts/Animals/FeedDragHandler.cs",
                ["Assets/Scripts/VatNuoi/iconSanPham.cs"] = "Assets/Scripts/Animals/AnimalProductIcon.cs",
                ["Assets/Scripts/VatNuoi/DialogVatNuoiAn.cs"] = "Assets/Scripts/Animals/AnimalFeedingPanel.cs",
                ["Assets/Scripts/VatNuoi/DialogTimeVatNuoi.cs"] = "Assets/Scripts/Animals/AnimalTimerPanel.cs",
                ["Assets/Scripts/VatNuoi/DialogHetThucAn.cs"] = "Assets/Scripts/Animals/OutOfFeedPanel.cs",
                ["Assets/Scripts/Shop/ItemVatNuoi.cs"] = "Assets/Scripts/Shop/AnimalShopItem.cs",
                ["Assets/Scripts/Shop/ScrollVatNuoi.cs"] = "Assets/Scripts/Shop/AnimalShopList.cs",
                ["Assets/Scripts/Shop/ItemChuong.cs"] = "Assets/Scripts/Shop/AnimalPenShopItem.cs",
                ["Assets/Scripts/Shop/ScrollChuong.cs"] = "Assets/Scripts/Shop/AnimalPenShopList.cs",
            };

        private static readonly IReadOnlyDictionary<string, string> AnimalPrefabMoves =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Assets/Prefabs/ChuongVatNuoi/ChuongGa.prefab"] = "Assets/Prefabs/Animals/ChickenPen.prefab",
                ["Assets/Prefabs/ChuongVatNuoi/ChuongBo.prefab"] = "Assets/Prefabs/Animals/CowPen.prefab",
                ["Assets/Prefabs/ChuongVatNuoi/ChuongVit.prefab"] = "Assets/Prefabs/Animals/DuckPen.prefab",
                ["Assets/Prefabs/ChuongVatNuoi/ChuongHeo.prefab"] = "Assets/Prefabs/Animals/PigPen.prefab",
                ["Assets/Prefabs/ChuongVatNuoi/ChuongCuu.prefab"] = "Assets/Prefabs/Animals/SheepPen.prefab",
                ["Assets/Prefabs/ChuongVatNuoi/ChuongTrau.prefab"] = "Assets/Prefabs/Animals/BuffaloPen.prefab",
                ["Assets/Prefabs/ChuongVatNuoi/ChuongDe.prefab"] = "Assets/Prefabs/Animals/GoatPen.prefab",
                ["Assets/Prefabs/ChuongVatNuoi/ThucAnVatNuoi.prefab"] = "Assets/Prefabs/Animals/AnimalFeed.prefab",
                ["Assets/Prefabs/ChuongVatNuoi/VatNuoiDrag.prefab"] = "Assets/Prefabs/Animals/AnimalDrag.prefab",
            };

        private static readonly IReadOnlyDictionary<string, string> AnimalArtFolderMoves =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Assets/Sprite/map_play/animal/chicken2"] = "Assets/Art/Sprites/Animals/Spine/Chicken",
                ["Assets/Sprite/map_play/animal/dairy cows"] = "Assets/Art/Sprites/Animals/Spine/Cow",
                ["Assets/Sprite/map_play/animal/Pig"] = "Assets/Art/Sprites/Animals/Spine/Pig",
                ["Assets/Sprite/map_play/animal/sheep"] = "Assets/Art/Sprites/Animals/Spine/Sheep",
                ["Assets/Sprite/v3/vit"] = "Assets/Art/Sprites/Animals/Spine/Duck",
                ["Assets/Sprite/v3/trau"] = "Assets/Art/Sprites/Animals/Spine/Buffalo",
                ["Assets/Sprite/v3/de"] = "Assets/Art/Sprites/Animals/Spine/Goat",
                ["Assets/Sprite/map_play/animal/icon"] = "Assets/Art/Sprites/Animals/UI/Legacy",
                ["Assets/Sprite/v3/icon"] = "Assets/Art/Sprites/Animals/UI/Modern",
                ["Assets/Sprite/map_play/chuong"] = "Assets/Art/Sprites/Animals/Pens/Legacy",
                ["Assets/Sprite/v3/chuong"] = "Assets/Art/Sprites/Animals/Pens/Modern",
                ["Assets/Sprite/map_play/animal/sp"] = "Assets/Art/Sprites/Animals/Products/Legacy",
                ["Assets/Sprite/v3/sp_vatnuoi"] = "Assets/Art/Sprites/Animals/Products/Modern",
            };

        private static readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> FarmingCodeReplacements =
            new Dictionary<string, IReadOnlyDictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["Assets/Scripts/Farming/CropPlot.cs"] = new Dictionary<string, string>
                {
                    ["public class Odat : MonoBehaviour, IHatGiong"] = "public class CropPlot : MonoBehaviour, ISeedReceiver",
                    ["dialogHG.instance"] = "SeedSelectionPanel.instance",
                    ["DialogTime.instance"] = "CropTimerPanel.instance",
                    ["Liem.instance"] = "SickleTool.instance",
                },
                ["Assets/Scripts/Farming/ISeedReceiver.cs"] = new Dictionary<string, string>
                {
                    ["public interface  IHatGiong  "] = "public interface  ISeedReceiver  ",
                },
                ["Assets/Scripts/Farming/SeedDragHandler.cs"] = new Dictionary<string, string>
                {
                    ["public class HatGiongKeo : MonoBehaviour"] = "public class SeedDragHandler : MonoBehaviour",
                    ["GetComponent<IHatGiong>()"] = "GetComponent<ISeedReceiver>()",
                },
                ["Assets/Scripts/Farming/SeedItem.cs"] = new Dictionary<string, string>
                {
                    ["public class ItemHatGiong : MonoBehaviour"] = "public class SeedItem : MonoBehaviour",
                    ["GetComponent<HatGiongKeo>()"] = "GetComponent<SeedDragHandler>()",
                    ["dialogHG.instance"] = "SeedSelectionPanel.instance",
                    ["GetComponent<DialogHetHG>()"] = "GetComponent<OutOfSeedsPanel>()",
                },
                ["Assets/Scripts/Farming/SickleTool.cs"] = new Dictionary<string, string>
                {
                    ["public class Liem : MonoBehaviour"] = "public class SickleTool : MonoBehaviour",
                    ["public static Liem instance;"] = "public static SickleTool instance;",
                },
                ["Assets/Scripts/Farming/OutOfSeedsPanel.cs"] = new Dictionary<string, string>
                {
                    ["public class DialogHetHG : MonoBehaviour"] = "public class OutOfSeedsPanel : MonoBehaviour",
                    ["GetComponent<Odat>()"] = "GetComponent<CropPlot>()",
                },
                ["Assets/Scripts/Farming/SeedSelectionPanel.cs"] = new Dictionary<string, string>
                {
                    ["public class dialogHG : MonoBehaviour"] = "public class SeedSelectionPanel : MonoBehaviour",
                    ["public static dialogHG instance;"] = "public static SeedSelectionPanel instance;",
                    ["GetComponent<ItemHatGiong>()"] = "GetComponent<SeedItem>()",
                },
                ["Assets/Scripts/Farming/CropTimerPanel.cs"] = new Dictionary<string, string>
                {
                    ["public class DialogTime : MonoBehaviour"] = "public class CropTimerPanel : MonoBehaviour",
                    ["public static DialogTime instance;"] = "public static CropTimerPanel instance;",
                    ["GetComponent<Odat>()"] = "GetComponent<CropPlot>()",
                },
                ["Assets/Scripts/Farming/CropPlotClickHandler.cs"] = new Dictionary<string, string>
                {
                    ["public class ClickOdat : MonoBehaviour"] = "public class CropPlotClickHandler : MonoBehaviour",
                },
                ["Assets/Scripts/Farming/CropDatabase.cs"] = new Dictionary<string, string>
                {
                    ["public class DataNongSan : ScriptableObject"] = "public class CropDatabase : ScriptableObject",
                },
                ["Assets/Scripts/Core/GameBootstrap.cs"] = new Dictionary<string, string>
                {
                    ["public DataNongSan ns;"] = "public CropDatabase ns;",
                },
                ["Assets/Scripts/MainCamera.cs"] = new Dictionary<string, string>
                {
                    ["dialogHG.instance.close();"] = "SeedSelectionPanel.instance.close();",
                    ["Liem.instance.close();"] = "SickleTool.instance.close();",
                    ["DialogTime.instance.dong();"] = "CropTimerPanel.instance.dong();",
                },
                ["Assets/Scripts/Shop/DialogShop.cs"] = new Dictionary<string, string>
                {
                    ["            dialogHG.instance.close();"] = "            SeedSelectionPanel.instance.close();",
                },
                ["Assets/Data/Editor/DataEditor.cs"] = new Dictionary<string, string>
                {
                    ["[MenuItem(\"Data/Data/NongSan\")]"] = "[MenuItem(\"Data/Crops/Create Crop Database\")]",
                    ["public static void DetailSeeds()"] = "public static void CreateCropDatabase()",
                    ["DataNongSan seed = ScriptableObject.CreateInstance<DataNongSan>();"] =
                        "CropDatabase cropDatabase = ScriptableObject.CreateInstance<CropDatabase>();",
                    ["CropDatabase seed = ScriptableObject.CreateInstance<CropDatabase>();"] =
                        "CropDatabase cropDatabase = ScriptableObject.CreateInstance<CropDatabase>();",
                    ["AssetDatabase.CreateAsset(seed, \"Assets/Data/Data/NongSan.asset\");"] =
                        "AssetDatabase.CreateAsset(cropDatabase, \"Assets/Data/Crops/CropDatabase.asset\");",
                    ["Selection.activeObject = seed;"] = "Selection.activeObject = cropDatabase;",
                },
            };

        public static void ExecuteCore()
        {
            var guidBySource = CoreMoves.ToDictionary(
                pair => pair.Key,
                pair => AssetDatabase.AssetPathToGUID(pair.Key),
                StringComparer.OrdinalIgnoreCase);
            if (guidBySource.Any(pair => string.IsNullOrWhiteSpace(pair.Value)))
                throw new InvalidOperationException("Core rename source missing GUID: " +
                    string.Join(", ", guidBySource.Where(pair => string.IsNullOrWhiteSpace(pair.Value)).Select(pair => pair.Key)));

            EnsureFolder("Assets/Scripts/Core");
            EnsureFolder("Assets/Scripts/UI");
            foreach (var move in CoreMoves)
                Move(move.Key, move.Value);

            RewriteFirstPartyCode();
            RewriteSingletons();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            foreach (var move in CoreMoves)
            {
                var targetGuid = AssetDatabase.AssetPathToGUID(move.Value);
                if (!string.Equals(guidBySource[move.Key], targetGuid, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException($"GUID changed moving {move.Key} to {move.Value}");
            }

            RenameInventoryBuilder.MarkMoved(CoreMoves);
            AssetDatabase.Refresh();
            Debug.Log($"Core migration moved {CoreMoves.Count} assets/folders with stable GUIDs.");
        }

        public static void RewriteCoreReferences()
        {
            RewriteFirstPartyCode();
            RewriteSingletons();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            Debug.Log("Core type references rewritten with identifier boundaries.");
        }

        public static void ExecuteFarming()
        {
            var guidBySource = FarmingMoves.ToDictionary(
                pair => pair.Key,
                pair => AssetDatabase.AssetPathToGUID(pair.Key),
                StringComparer.OrdinalIgnoreCase);
            if (guidBySource.Any(pair => string.IsNullOrWhiteSpace(pair.Value)))
                throw new InvalidOperationException("Farming rename source missing GUID: " +
                    string.Join(", ", guidBySource.Where(pair => string.IsNullOrWhiteSpace(pair.Value)).Select(pair => pair.Key)));

            EnsureFolder("Assets/Scripts/Farming");
            foreach (var move in FarmingMoves)
                Move(move.Key, move.Value);

            RewriteFarmingReferences();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            foreach (var move in FarmingMoves)
            {
                var targetGuid = AssetDatabase.AssetPathToGUID(move.Value);
                if (!string.Equals(guidBySource[move.Key], targetGuid, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException($"GUID changed moving {move.Key} to {move.Value}");
            }

            RenameInventoryBuilder.MarkMoved(FarmingMoves);
            AssetDatabase.Refresh();
            Debug.Log($"Farming migration moved {FarmingMoves.Count} scripts with stable GUIDs.");
        }

        [MenuItem("Tools/Farm Town/Migration/Execute Animals")]
        public static void ExecuteAnimalsRename()
        {
            var allMoves = AnimalPrefabMoves.Concat(ReadAnimalArtMoves())
                .ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.OrdinalIgnoreCase);
            var guidBySource = allMoves.ToDictionary(pair => pair.Key, pair => AssetDatabase.AssetPathToGUID(pair.Key), StringComparer.OrdinalIgnoreCase);
            if (guidBySource.Any(pair => string.IsNullOrWhiteSpace(pair.Value)))
                throw new InvalidOperationException("Animal rename source missing GUID: " +
                    string.Join(", ", guidBySource.Where(pair => string.IsNullOrWhiteSpace(pair.Value)).Select(pair => pair.Key)));

            foreach (var move in allMoves.OrderByDescending(pair => pair.Key.Count(character => character == '/')))
                Move(move.Key, move.Value);
            DeleteLegacyAnimalArtFolders();

            RenameAnimalSpineAssets();
            UpdateAnimalPrefabs();
            RewriteFile("Assets/Scenes/map.unity", new Dictionary<string, string>
            {
                ["m_Name: btnchuong"] = "m_Name: AnimalPenButton",
                ["m_Name: DialogHetThucAn"] = "m_Name: OutOfFeedPanel",
                ["m_Name: VatNuoi"] = "m_Name: Animals",
                ["m_Name: Chuong"] = "m_Name: AnimalPens",
                ["m_Name: iconSanPham"] = "m_Name: AnimalProductIcon",
                ["m_Name: dialogTimeVatNuoi"] = "m_Name: AnimalTimerPanel",
                ["m_Name: btnVatNuoi"] = "m_Name: AnimalButton",
                ["m_Name: taykeochuong"] = "m_Name: AnimalPenDragTutorial",
                ["m_Name: dialogVatNuoiAn"] = "m_Name: AnimalFeedingPanel",
            });

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            foreach (var move in allMoves)
                if (!string.Equals(guidBySource[move.Key], AssetDatabase.AssetPathToGUID(move.Value), StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException($"GUID changed moving {move.Key} to {move.Value}");

            RenameInventoryBuilder.MarkMoved(allMoves);
            Debug.Log($"Animal migration moved {allMoves.Count} assets/folders with stable GUIDs.");
        }

        private static IReadOnlyDictionary<string, string> ReadAnimalArtMoves()
        {
            var prefixes = new[]
            {
                "Assets/Sprite/map_play/animal", "Assets/Sprite/map_play/chuong",
                "Assets/Sprite/v3/chuong", "Assets/Sprite/v3/sp_vatnuoi",
                "Assets/Sprite/v3/de", "Assets/Sprite/v3/trau", "Assets/Sprite/v3/vit", "Assets/Sprite/v3/icon",
            };
            return File.ReadAllLines(ToAbsolutePath("Docs/Migration/rename-inventory.csv"))
                .Select(line => line.Split(','))
                .Where(columns => columns.Length >= 6 && prefixes.Any(prefix => columns[3].StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
                .Where(columns => !string.Equals(columns[2], "Folder", StringComparison.OrdinalIgnoreCase))
                .Where(columns => !string.Equals(columns[3], columns[4], StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(columns => columns[3].Count(character => character == '/'))
                .ToDictionary(columns => columns[3], columns => columns[4], StringComparer.OrdinalIgnoreCase);
        }

        private static void DeleteLegacyAnimalArtFolders()
        {
            var folders = AnimalArtFolderMoves.Keys
                .Concat(new[] { "Assets/Prefabs/ChuongVatNuoi", "Assets/Scripts/VatNuoi" });
            foreach (var folder in folders.OrderByDescending(path => path.Count(character => character == '/')))
                if (AssetDatabase.IsValidFolder(folder) && !AssetDatabase.DeleteAsset(folder))
                    throw new InvalidOperationException($"Cannot remove legacy animal art folder {folder}");
        }

        private static void RenameAnimalSpineAssets()
        {
            foreach (var animalName in new[] { "Chicken", "Cow", "Duck", "Pig", "Sheep", "Buffalo", "Goat" })
            {
                var folder = $"Assets/Art/Sprites/Animals/Spine/{animalName}";
                RewriteFile($"{folder}/skeleton.json", new Dictionary<string, string>
                {
                    ["\"doi\":"] = "\"Idle\":",
                    ["\"an\":"] = "\"Feeding\":",
                    ["\"thuhoach\":"] = "\"Ready\":",
                });
            }
        }

        private static void UpdateAnimalPrefabs()
        {
            var legacyNames = new Dictionary<string, string>
            {
                ["Chicken"] = "ga", ["Cow"] = "bo", ["Duck"] = "Vit", ["Pig"] = "heo",
                ["Sheep"] = "cuu", ["Buffalo"] = "Trau", ["Goat"] = "De",
            };
            foreach (var pair in legacyNames)
            {
                var animalName = pair.Key;
                var assetPath = $"Assets/Prefabs/Animals/{animalName}Pen.prefab";
                var root = PrefabUtility.LoadPrefabContents(assetPath);
                try
                {
                    root.name = animalName + "Pen";
                    var animals = root.GetComponentsInChildren<Transform>(true)
                        .Where(item => item.name == pair.Value || item.name.StartsWith(pair.Value + " (", StringComparison.Ordinal))
                        .OrderBy(item => item.GetSiblingIndex())
                        .ToArray();
                    for (var index = 0; index < animals.Length; index++)
                        animals[index].name = index == 0 ? animalName : $"{animalName} ({index})";
                    PrefabUtility.SaveAsPrefabAsset(root, assetPath);
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(root);
                }
            }
        }

        [MenuItem("Tools/Farm Town/Migration/Execute Farming Assets")]
        public static void ExecuteFarmingAssets()
        {
            const string sourceFolder = "Assets/Prefabs/odat";
            const string targetFolder = "Assets/Prefabs/Farming";
            const string sourceData = "Assets/Data/Data/NongSan.asset";
            const string targetData = "Assets/Data/Crops/CropDatabase.asset";
            var expectedGuids = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Assets/Prefabs/Farming/CropPlot.prefab"] = AssetDatabase.AssetPathToGUID(sourceFolder + "/Odat.prefab"),
                ["Assets/Prefabs/Farming/SeedDrag.prefab"] = AssetDatabase.AssetPathToGUID(sourceFolder + "/hatgiongdrag.prefab"),
                ["Assets/Prefabs/Farming/Sickle.prefab"] = AssetDatabase.AssetPathToGUID(sourceFolder + "/liem.prefab"),
                ["Assets/Prefabs/Farming/PlantingEffect.prefab"] = AssetDatabase.AssetPathToGUID(sourceFolder + "/FxTrong.prefab"),
                [targetData] = AssetDatabase.AssetPathToGUID(sourceData)
            };
            if (expectedGuids.Any(pair => string.IsNullOrEmpty(pair.Value)))
                throw new InvalidOperationException("Farming asset source missing GUID.");

            EnsureFolder("Assets/Prefabs");
            Move(sourceFolder, targetFolder);
            Move(targetFolder + "/Odat.prefab", targetFolder + "/CropPlot.prefab");
            Move(targetFolder + "/hatgiongdrag.prefab", targetFolder + "/SeedDrag.prefab");
            Move(targetFolder + "/liem.prefab", targetFolder + "/Sickle.prefab");
            Move(targetFolder + "/FxTrong.prefab", targetFolder + "/PlantingEffect.prefab");
            EnsureFolder("Assets/Data/Crops");
            Move(sourceData, targetData);

            UpdateFarmingPrefab(targetFolder + "/CropPlot.prefab", "CropPlot", true);
            UpdateFarmingPrefab(targetFolder + "/SeedDrag.prefab", "SeedDrag", false);
            UpdateFarmingPrefab(targetFolder + "/Sickle.prefab", "Sickle", false);
            UpdateFarmingPrefab(targetFolder + "/PlantingEffect.prefab", "PlantingEffect", false);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            foreach (var pair in expectedGuids)
                if (!string.Equals(pair.Value, AssetDatabase.AssetPathToGUID(pair.Key), StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException($"GUID changed moving farming asset to {pair.Key}");

            RenameInventoryBuilder.MarkTargetMoved(sourceFolder, targetFolder);
            RenameInventoryBuilder.MarkTargetMoved(sourceData, targetData);
            Debug.Log("Farming prefabs and crop data moved with stable GUIDs.");
        }

        private static void UpdateFarmingPrefab(string assetPath, string rootName, bool addStableIdentity)
        {
            var root = PrefabUtility.LoadPrefabContents(assetPath);
            try
            {
                root.name = rootName;
                if (addStableIdentity && root.GetComponent<FarmTown.Save.StableInstanceId>() == null)
                    root.AddComponent<FarmTown.Save.StableInstanceId>();
                PrefabUtility.SaveAsPrefabAsset(root, assetPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static void RewriteFarmingReferences()
        {
            foreach (var file in FarmingCodeReplacements)
                RewriteFile(file.Key, file.Value);
        }

        public static void ExecuteAppOpenRename()
        {
            const string source = "Assets/Scripts/Ads/AdsAppOpen.cs";
            const string target = "Assets/Scripts/Ads/AppOpenAdController.cs";
            var sourceGuid = AssetDatabase.AssetPathToGUID(source);
            Move(source, target);

            var targetPath = ToAbsolutePath(target);
            var code = RewriteAppOpenController(File.ReadAllText(targetPath));
            File.WriteAllText(targetPath, code);

            foreach (var assetPath in AssetDatabase.GetAllAssetPaths()
                         .Where(FirstPartyAssetPolicy.IsFirstParty)
                         .Where(path => path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)))
            {
                var absolutePath = ToAbsolutePath(assetPath);
                var sourceCode = File.ReadAllText(absolutePath);
                var rewritten = sourceCode.Replace("AdsAppOpen.Instance", "AppOpenAdController.Instance");
                if (!string.Equals(sourceCode, rewritten, StringComparison.Ordinal))
                    File.WriteAllText(absolutePath, rewritten);
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            if (!string.Equals(sourceGuid, AssetDatabase.AssetPathToGUID(target), StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("GUID changed renaming app-open controller.");

            RenameInventoryBuilder.MarkTargetMoved(source, target);
            AssetDatabase.Refresh();
            Debug.Log("App-open controller renamed with stable GUID.");
        }

        private static void RewriteFirstPartyCode()
        {
            foreach (var assetPath in AssetDatabase.GetAllAssetPaths()
                         .Where(FirstPartyAssetPolicy.IsFirstParty)
                         .Where(path => path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)))
            {
                var absolutePath = ToAbsolutePath(assetPath);
                var source = File.ReadAllText(absolutePath);
                var rewritten = source
                    .Replace($"{LegacyBootstrapType}.instance", "GameBootstrap.Instance")
                    .Replace("LoadMap.instance", "SceneLoader.Instance")
                    .Replace("TimeManager.TimeCurrent", "GameTime.TimeCurrent")
                    .Replace("Sound.instance", "AudioManager.Instance")
                    .Replace($"{LegacyNotificationType}.instance", "NotificationController.Instance")
                    .Replace("MainCamera.instance", "MainCamera.Instance");

                if (!string.Equals(source, rewritten, StringComparison.Ordinal))
                    File.WriteAllText(absolutePath, rewritten);
            }
        }

        private static void RewriteSingletons()
        {
            RewriteFile("Assets/Scripts/Core/GameBootstrap.cs", new Dictionary<string, string>
            {
                [$"public class {LegacyBootstrapType}"] = "public class GameBootstrap",
                [$"public static {LegacyBootstrapType} instance = null;"] = "public static GameBootstrap Instance { get; private set; }",
                ["if (instance == null) instance = this;"] = "if (Instance == null) Instance = this;",
                ["else if (instance != this) Destroy(gameObject);"] = "else if (Instance != this) Destroy(gameObject);",
            });
            RewriteFile("Assets/Scripts/Core/SceneLoader.cs", new Dictionary<string, string>
            {
                ["public class LoadMap"] = "public class SceneLoader",
                ["public static LoadMap instance;"] = "public static SceneLoader Instance { get; private set; }",
                ["instance = this;"] = "Instance = this;",
            });
            RewriteFile("Assets/Scripts/Core/GameTime.cs", new Dictionary<string, string>
            {
                ["public class TimeManager"] = "public static class GameTime",
            });
            RewriteFile("Assets/Scripts/Core/AudioManager.cs", new Dictionary<string, string>
            {
                ["public class Sound"] = "public class AudioManager",
                ["public static Sound instance;"] = "public static AudioManager Instance { get; private set; }",
                ["instance = this;"] = "Instance = this;",
            });
            RewriteFile("Assets/Scripts/UI/NotificationController.cs", new Dictionary<string, string>
            {
                [$"public class {LegacyNotificationType}"] = "public class NotificationController",
                [$"public static {LegacyNotificationType} instance;"] = "public static NotificationController Instance { get; private set; }",
                ["instance = this;"] = "Instance = this;",
            });
            RewriteFile("Assets/Scripts/MainCamera.cs", new Dictionary<string, string>
            {
                ["public static MainCamera instance;"] = "public static MainCamera Instance { get; private set; }",
                ["instance = this;"] = "Instance = this;",
            });
        }

        private static string RewriteAppOpenController(string source)
        {
            var rewritten = source.Replace("\r\n", "\n")
                .Replace("public class AdsAppOpen", "public class AppOpenAdController")
                .Replace("    private static AdsAppOpen instance;\n\n", string.Empty)
                .Replace("    private bool isShowingAd = false;", "    private bool isShowingAd;")
                .Replace(
                    "    public static AdsAppOpen Instance\n    {\n        get\n        {\n            if (instance == null)\n            {\n                instance = new AdsAppOpen();\n            }\n\n            return instance;\n        }\n    }",
                    "    public static AppOpenAdController Instance { get; private set; }")
                .Replace("return ad != null;", "return ad != null && ad.CanShowAd();")
                .Replace(
                    "    private void Start()",
                    "    private void Awake()\n    {\n        if (Instance != null && Instance != this)\n        {\n            Destroy(gameObject);\n            return;\n        }\n\n        Instance = this;\n    }\n\n    private void Start()")
                .Replace("            ad = appOpenAd;", "            DestroyAd();\n            ad = appOpenAd;\n            RegisterCallbacks(ad);")
                .Replace(
                    "        //ad.OnAdDidDismissFullScreenContent += HandleAdDidDismissFullScreenContent;\n        //ad.OnAdFailedToPresentFullScreenContent += HandleAdFailedToPresentFullScreenContent;\n        //ad.OnAdDidPresentFullScreenContent += HandleAdDidPresentFullScreenContent;\n        //ad.OnAdDidRecordImpression += HandleAdDidRecordImpression;\n        //ad.OnPaidEvent += HandlePaidEvent;\n\n        ad.Show();",
                    "        isShowingAd = true;\n        ad.Show();");

            var handlersStart = rewritten.IndexOf("    private void HandleAdDidDismissFullScreenContent", StringComparison.Ordinal);
            if (handlersStart < 0)
                throw new InvalidOperationException("App-open legacy handlers not found.");

            var classEnd = rewritten.LastIndexOf('}');
            var handlers = @"    private void RegisterCallbacks(AppOpenAd loadedAd)
    {
        loadedAd.OnAdFullScreenContentOpened += HandleAdFullScreenContentOpened;
        loadedAd.OnAdFullScreenContentClosed += HandleAdFullScreenContentClosed;
        loadedAd.OnAdFullScreenContentFailed += HandleAdFullScreenContentFailed;
        loadedAd.OnAdImpressionRecorded += HandleAdImpressionRecorded;
        loadedAd.OnAdPaid += HandleAdPaid;
    }

    private void HandleAdFullScreenContentClosed()
    {
        Debug.Log(""Closed app open ad"");
        isShowingAd = false;
        DestroyAd();
        LoadAd();
    }

    private void HandleAdFullScreenContentFailed(AdError error)
    {
        Debug.LogFormat(""Failed to present app open ad: {0}"", error.GetMessage());
        isShowingAd = false;
        DestroyAd();
        LoadAd();
    }

    private void HandleAdFullScreenContentOpened()
    {
        Debug.Log(""Displayed app open ad"");
    }

    private void HandleAdImpressionRecorded()
    {
        Debug.Log(""Recorded ad impression"");
    }

    private void HandleAdPaid(AdValue value)
    {
        Debug.LogFormat(""Received paid event. (currency: {0}, value: {1}"",
                value.CurrencyCode, value.Value);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
        DestroyAd();
    }

    private void DestroyAd()
    {
        if (ad == null)
            return;

        ad.Destroy();
        ad = null;
    }
";
            var result = rewritten.Substring(0, handlersStart) + handlers + rewritten.Substring(classEnd);
            if (result.Contains("AdsAppOpen") ||
                !result.Contains("public static AppOpenAdController Instance { get; private set; }") ||
                !result.Contains("RegisterCallbacks(ad);") ||
                !result.Contains("isShowingAd = true;"))
            {
                throw new InvalidOperationException("App-open controller rewrite did not apply completely.");
            }

            return result;
        }

        private static void RewriteFile(string assetPath, IReadOnlyDictionary<string, string> replacements)
        {
            var absolutePath = ToAbsolutePath(assetPath);
            var source = File.ReadAllText(absolutePath);
            var rewritten = replacements.Aggregate(source, (current, replacement) =>
                current.Replace(replacement.Key, replacement.Value));
            if (!string.Equals(source, rewritten, StringComparison.Ordinal))
                File.WriteAllText(absolutePath, rewritten);
        }

        private static void Move(string source, string target)
        {
            EnsureFolder(Path.GetDirectoryName(target)?.Replace('\\', '/'));
            var error = AssetDatabase.MoveAsset(source, target);
            if (!string.IsNullOrEmpty(error))
                throw new InvalidOperationException($"Cannot move {source} to {target}: {error}");
        }

        private static void EnsureFolder(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath) || AssetDatabase.IsValidFolder(folderPath))
                return;

            var parent = Path.GetDirectoryName(folderPath)?.Replace('\\', '/');
            var name = Path.GetFileName(folderPath);
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }

        private static string ToAbsolutePath(string assetPath)
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, "..", assetPath));
        }
    }
}
