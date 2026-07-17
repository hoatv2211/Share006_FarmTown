using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace FarmTown.Editor.Migration
{
    public static class RenameInventoryBuilder
    {
        private const string OutputRelativePath = "Docs/Migration/rename-inventory.csv";

        private static readonly IReadOnlyDictionary<string, string> CanonicalNames =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Odat"] = "CropPlot",
                ["HatGiong"] = "Seed",
                ["Liem"] = "Sickle",
                ["NongSan"] = "Crop",
                ["VatNuoi"] = "FarmAnimal",
                ["Chuong"] = "AnimalPen",
                ["ThucAn"] = "Feed",
                ["NhaMay"] = "ProductionBuilding",
                ["SanXuat"] = "ProductionQueue",
                ["VatPham"] = "Product",
                ["Kho"] = "Inventory",
                ["DonHang"] = "Order",
                ["XeOto"] = "DeliveryTruck",
                ["NhaCho"] = "MarketBuilding",
                ["TrangTri"] = "Decoration",
                ["CayRung"] = "ForestTree",
                ["TutorialController"] = "Tutorial",
                ["NotificationController"] = "Notification",
                ["HieuUng"] = "Effect",
                ["Vang"] = "Coin",
                ["KimCuong"] = "Gem",
                ["KinhNghiem"] = "Experience",
                ["GameBootstrap"] = "GameBootstrap",
                ["SceneLoader"] = "SceneLoader",
                ["dialognangcapmagic"] = "MagicUpgradeDialog",
                ["dialogNangCap"] = "UpgradeDialog",
                ["taykeonhamay"] = "ProductionBuildingDragHandle",
                ["hieuungbankho"] = "InventorySaleEffect",
                ["ClickNhatrangtri"] = "DecorationBuildingClick",
                ["Decoration"] = "DecorationItem",
                ["animcaylon"] = "LargeTreeAnimation",
                ["banhmybocay"] = "HerbButterBread",
                ["banhnhanlacvung"] = "PeanutSesameBread",
                ["bantaykeoxuong"] = "HandPullDown",
                ["bantaykeolen"] = "HandPullUp",
                ["taykeoProductionBuilding"] = "ProductionBuildingDragHandle",
                ["CayCoDecorationMap"] = "MapDecorationTree",
                ["ClickNhaDecoration"] = "DecorationBuildingClick",
                ["MovementDialog"] = "MovementDialog",
                ["MagicTreePanel"] = "MagicTreeDialog",
                ["EffectbanInventory"] = "InventorySaleEffect",
                ["EffectChoTien"] = "CoinGrantEffect",
                ["MouseDownHandle"] = "PointerDownHandler",
                ["MagicTreeClickHandler"] = "TreeTopClick",
                ["MagicCoinTree"] = "CoinTree",
                ["MagicTreeRewardIcon"] = "MagicTreeIcon",
                ["IconThuHoach"] = "CompletedProductIcon",
                ["DichuyenOB"] = "ObjectMovement",
                ["nangcapcay"] = "UpgradeTree",
                ["ngoncayexp2"] = "TreeTopExperience2",
                ["ngoncayexp"] = "TreeTopExperience",
                ["DialogCho"] = "DogDialog",
                ["ChoTien"] = "GrantCoins",
                ["CayMaGic"] = "MagicTree",
                ["caytrong"] = "PlantedTree",
                ["nhabanhmi"] = "Bakery",
                ["nhachinh"] = "MainHouse",
                ["NhaCrop2"] = "CropStorage2",
                ["NhaCrop"] = "CropStorage",
                ["NhaInventory2"] = "InventoryBuilding2",
                ["nhaInventory"] = "InventoryBuilding",
                ["NhaLuong"] = "ProduceBarn",
                ["NhaOrder2"] = "OrderBuilding2",
                ["NhaOrder"] = "OrderBuilding",
                ["NhaProduct"] = "ProductStorage",
                ["nhaDecoration"] = "DecorationBuilding",
                ["VatDecoration"] = "DecorationItem",
                ["IItemKeo"] = "IProductionInputReceiver",
                ["itemkeoNM"] = "ProductionInputDragPreview",
                ["ItemKeo"] = "ProductionInputDragHandler",
                ["DialogNhaMay"] = "ProductionPanel",
                ["ClickNhaMay"] = "ProductionBuildingClickHandler",
                ["DongDialog"] = "CloseProductionPanelHandler",
                ["ItemVatPham"] = "ProductItem",
                ["SeedKeo"] = "DraggableSeed",
                ["banhmykem"] = "CreamBread",
                ["banhmybo"] = "ButterBread",
                ["banhgato"] = "Cake",
                ["banhnuong"] = "BakedPastry",
                ["banhquy"] = "Cookie",
                ["banhxeo"] = "Pancake",
                ["Banhtrung"] = "EggCake",
                ["banhmy"] = "Bread",
                ["banhsau1"] = "PastryStage1",
                ["banhsau2"] = "PastryStage2",
                ["banhsau3"] = "PastryStage3",
                ["banhsau4"] = "PastryStage4",
                ["Banhxe"] = "Wheel",
                ["bantay"] = "Hand",
                ["taykeoui2"] = "UiDragHand2",
                ["chatCay"] = "ChopTree",
                ["cuacay"] = "SawTree",
                ["thuhoach"] = "Harvest",
                ["sinhnhat"] = "Birthday",
                ["Dichuyen"] = "Movement",
                ["ForestTreeClickHandler"] = "TreeTop",
                ["goccay1"] = "TreeStump1",
                ["goccay2"] = "TreeStump2",
                ["caydo2"] = "FallenTree2",
                ["caydo"] = "FallenTree",
                ["cay1"] = "Tree1",
                ["cay2"] = "Tree2",
                ["cay3"] = "Tree3",
                ["cay4"] = "Tree4",
                ["cay5"] = "Tree5",
                ["cay"] = "Tree",
                ["Chon1"] = "Select1",
                ["chon2"] = "Select2",
                ["cho"] = "Dog",
                ["nhan"] = "Receive",
                ["nha1"] = "House1",
                ["nha2"] = "House2",
                ["nha3"] = "House3",
                ["nha"] = "House",
                ["bang"] = "Board",
                ["banh"] = "Pastry",
            };

        private static readonly string[] UnresolvedFragments =
        {
            "odat", "hatgiong", "liem", "nongsan", "vatnuoi", "chuong", "thucan", "nhamay",
            "sanxuat", "vatpham", "kho", "donhang", "xeoto", "nhacho", "trangtri", "cayrung",
            "huongdan", "thongbao", "hieuung", "vang", "kimcuong", "kinhnghiem", "batdau",
            "dichuyen", "nhan", "mua", "ban", "cay", "nha", "vat", "cho", "keo", "thuhoach", "nangcap",
        };

        public static void Export()
        {
            var rows = BuildRows();
            var outputPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", OutputRelativePath));
            var outputDirectory = Path.GetDirectoryName(outputPath);
            if (string.IsNullOrEmpty(outputDirectory))
                throw new InvalidOperationException($"Cannot resolve inventory directory for {outputPath}");

            Directory.CreateDirectory(outputDirectory);
            var temporaryPath = outputPath + ".tmp";
            File.WriteAllText(temporaryPath, RenderCsv(rows), new UTF8Encoding(false));

            if (File.Exists(outputPath))
                File.Delete(outputPath);

            File.Move(temporaryPath, outputPath);
            AssetDatabase.Refresh();
            Debug.Log($"Rename inventory exported: {rows.Count} rows to {outputPath}");
        }

        public static IReadOnlyList<RenameInventoryRow> BuildRows()
        {
            var rows = AssetDatabase.GetAllAssetPaths()
                .Where(FirstPartyAssetPolicy.IsFirstParty)
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(BuildRow)
                .ToArray();

            var collisions = rows
                .GroupBy(row => row.NewPath, StringComparer.OrdinalIgnoreCase)
                .Where(group => group.Select(row => row.OldPath).Distinct(StringComparer.OrdinalIgnoreCase).Count() > 1)
                .ToArray();
            if (collisions.Length > 0)
            {
                var details = string.Join("; ", collisions.Select(group =>
                    $"{group.Key} <= {string.Join("|", group.Select(row => row.OldPath))}"));
                throw new InvalidOperationException($"Rename inventory contains target path collisions: {details}");
            }

            return rows;
        }

        public static IReadOnlyList<RenameInventoryRow> ReadInventory()
        {
            var inputPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", OutputRelativePath));
            if (!File.Exists(inputPath))
                return Array.Empty<RenameInventoryRow>();

            return File.ReadLines(inputPath)
                .Skip(1)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(ParseRow)
                .ToArray();
        }

        public static void MarkMoved(IReadOnlyDictionary<string, string> movedPaths)
        {
            var rows = ReadInventory().Select(row =>
            {
                foreach (var move in movedPaths.OrderByDescending(pair => pair.Key.Length))
                {
                    if (!string.Equals(row.OldPath, move.Key, StringComparison.OrdinalIgnoreCase)
                        && !row.OldPath.StartsWith(move.Key + "/", StringComparison.OrdinalIgnoreCase))
                        continue;

                    var suffix = row.OldPath.Substring(move.Key.Length);
                    return new RenameInventoryRow(
                        "Moved",
                        row.Domain,
                        row.AssetType,
                        row.OldPath,
                        move.Value + suffix,
                        row.Guid,
                        row.Notes);
                }

                return row;
            }).ToArray();

            WriteInventory(rows);
        }

        public static void MarkTargetMoved(string currentPath, string newPath)
        {
            var rows = ReadInventory().Select(row =>
            {
                if (!string.Equals(row.NewPath, currentPath, StringComparison.OrdinalIgnoreCase))
                    return row;

                return new RenameInventoryRow(
                    "Moved", row.Domain, row.AssetType, row.OldPath, newPath, row.Guid, row.Notes);
            }).ToArray();
            WriteInventory(rows);
        }

        public static string BuildEnglishPath(string oldPath, out IReadOnlyList<string> unresolvedTokens)
        {
            var newPath = oldPath;
            foreach (var pair in CanonicalNames.OrderByDescending(pair => pair.Key.Length))
                newPath = Regex.Replace(newPath, Regex.Escape(pair.Key), pair.Value, RegexOptions.IgnoreCase);

            unresolvedTokens = FindUnresolvedTokens(newPath);
            return newPath;
        }

        private static RenameInventoryRow BuildRow(string path)
        {
            var guid = AssetDatabase.AssetPathToGUID(path);
            if (string.IsNullOrWhiteSpace(guid))
                throw new InvalidOperationException($"First-party asset has no GUID: {path}");

            var newPath = BuildEnglishPath(path, out var unresolvedTokens);
            return new RenameInventoryRow(
                unresolvedTokens.Count == 0 ? "Ready" : "NeedsMapping",
                ClassifyDomain(path),
                ClassifyAssetType(path),
                path,
                newPath,
                guid,
                unresolvedTokens.Count == 0
                    ? string.Empty
                    : "Unresolved tokens: " + string.Join("|", unresolvedTokens));
        }

        private static IReadOnlyList<string> FindUnresolvedTokens(string path)
        {
            return path.Split('/')
                .Select(segment => Path.GetFileNameWithoutExtension(segment))
                .Where(segment => !string.IsNullOrEmpty(segment))
                .SelectMany(segment => Regex.Matches(segment, "[A-Za-z][A-Za-z0-9]*").Cast<Match>())
                .Select(match => match.Value)
                .Where(ContainsVietnameseTechnicalToken)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(token => token, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static bool ContainsVietnameseTechnicalToken(string token)
        {
            return UnresolvedFragments.Any(fragment =>
                token.IndexOf(fragment, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static string ClassifyDomain(string path)
        {
            var segments = path.Split('/');
            if (segments.Length < 2)
                return "Other";

            switch (segments[1].ToLowerInvariant())
            {
                case "scripts": return "Scripts";
                case "data": return "Data";
                case "prefabs": return "Prefabs";
                case "scenes": return "Scenes";
                case "resources": return "Resources";
                case "sprite": return "Sprite";
                case "animation": return "Animation";
                case "audio": return "Audio";
                case "particle": return "Particle";
                case "mini_game": return "MiniGame";
                default: return "Other";
            }
        }

        private static string ClassifyAssetType(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
                return "Folder";

            var type = AssetDatabase.GetMainAssetTypeAtPath(path);
            return type != null ? type.Name : Path.GetExtension(path).TrimStart('.');
        }

        private static string RenderCsv(IEnumerable<RenameInventoryRow> rows)
        {
            var builder = new StringBuilder("Status,Domain,AssetType,OldPath,NewPath,Guid,Notes\n");
            foreach (var row in rows)
            {
                builder.Append(Escape(row.Status)).Append(',')
                    .Append(Escape(row.Domain)).Append(',')
                    .Append(Escape(row.AssetType)).Append(',')
                    .Append(Escape(row.OldPath)).Append(',')
                    .Append(Escape(row.NewPath)).Append(',')
                    .Append(Escape(row.Guid)).Append(',')
                    .Append(Escape(row.Notes)).Append('\n');
            }

            return builder.ToString();
        }

        private static void WriteInventory(IReadOnlyList<RenameInventoryRow> rows)
        {
            var outputPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", OutputRelativePath));
            File.WriteAllText(outputPath, RenderCsv(rows), new UTF8Encoding(false));
        }

        private static string Escape(string value)
        {
            return value.IndexOfAny(new[] { ',', '"', '\r', '\n' }) < 0
                ? value
                : '"' + value.Replace("\"", "\"\"") + '"';
        }

        private static RenameInventoryRow ParseRow(string line)
        {
            var fields = new List<string>();
            var field = new StringBuilder();
            var quoted = false;
            for (var index = 0; index < line.Length; index++)
            {
                var character = line[index];
                if (character == '"')
                {
                    if (quoted && index + 1 < line.Length && line[index + 1] == '"')
                    {
                        field.Append('"');
                        index++;
                    }
                    else
                    {
                        quoted = !quoted;
                    }
                }
                else if (character == ',' && !quoted)
                {
                    fields.Add(field.ToString());
                    field.Length = 0;
                }
                else
                {
                    field.Append(character);
                }
            }
            fields.Add(field.ToString());

            if (fields.Count != 7)
                throw new FormatException($"Invalid rename inventory row: {line}");

            return new RenameInventoryRow(
                fields[0], fields[1], fields[2], fields[3], fields[4], fields[5], fields[6]);
        }
    }

    public sealed class RenameInventoryRow
    {
        public RenameInventoryRow(
            string status,
            string domain,
            string assetType,
            string oldPath,
            string newPath,
            string guid,
            string notes)
        {
            Status = status;
            Domain = domain;
            AssetType = assetType;
            OldPath = oldPath;
            NewPath = newPath;
            Guid = guid;
            Notes = notes;
        }

        public string Status { get; }
        public string Domain { get; }
        public string AssetType { get; }
        public string OldPath { get; }
        public string NewPath { get; }
        public string Guid { get; }
        public string Notes { get; }
    }
}
