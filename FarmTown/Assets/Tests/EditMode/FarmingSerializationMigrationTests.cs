using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace FarmTown.Tests.EditMode
{
    public sealed class FarmingSerializationMigrationTests
    {
        [Test]
        public void CropDatabasePreservesAllCropDefinitions()
        {
            var database = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>("Assets/Data/Crops/CropDatabase.asset");
            Assert.That(database, Is.Not.Null);
            var crops = new SerializedObject(database).FindProperty("crops");
            Assert.That(crops.arraySize, Is.EqualTo(22));
            Assert.That(crops.GetArrayElementAtIndex(0).FindPropertyRelative("englishName").stringValue, Is.EqualTo("Wheat"));
            Assert.That(crops.GetArrayElementAtIndex(21).FindPropertyRelative("englishName").stringValue, Is.EqualTo("Watermelon"));
            for (var index = 0; index < crops.arraySize; index++)
            {
                var crop = crops.GetArrayElementAtIndex(index);
                Assert.That(crop.FindPropertyRelative("growthStage1").objectReferenceValue, Is.Not.Null);
                Assert.That(crop.FindPropertyRelative("growthStage2").objectReferenceValue, Is.Not.Null);
                Assert.That(crop.FindPropertyRelative("growthStage3").objectReferenceValue, Is.Not.Null);
                Assert.That(crop.FindPropertyRelative("productIcon").objectReferenceValue, Is.Not.Null);
            }
        }

        [Test]
        public void CropDatabaseFieldsKeepLegacySerializationAliases()
        {
            AssertAlias(RuntimeTypes.Get("CropDatabase"), "crops", "data");
            AssertAlias(RuntimeTypes.Get("CropDatabase.CropDefinition"), "vietnameseName", "ten");
            AssertAlias(RuntimeTypes.Get("CropDatabase.CropDefinition"), "englishName", "name");
            AssertAlias(RuntimeTypes.Get("CropDatabase.CropDefinition"), "vietnameseProductName", "sanpham");
            AssertAlias(RuntimeTypes.Get("CropDatabase.CropDefinition"), "englishProductName", "product");
            AssertAlias(RuntimeTypes.Get("CropDatabase.CropDefinition"), "growthSeconds", "time");
            AssertAlias(RuntimeTypes.Get("CropDatabase.CropDefinition"), "unlockLevel", "capmo");
            AssertAlias(RuntimeTypes.Get("CropDatabase.CropDefinition"), "purchasePrice", "giamua");
            AssertAlias(RuntimeTypes.Get("CropDatabase.CropDefinition"), "salePrice", "giaban");
            AssertAlias(RuntimeTypes.Get("CropDatabase.CropDefinition"), "experienceReward", "exp");
            AssertAlias(RuntimeTypes.Get("CropDatabase.CropDefinition"), "harvestYield", "sanluong");
            AssertAlias(RuntimeTypes.Get("CropDatabase.CropDefinition"), "growthStage1", "gd1");
            AssertAlias(RuntimeTypes.Get("CropDatabase.CropDefinition"), "growthStage2", "gd2");
            AssertAlias(RuntimeTypes.Get("CropDatabase.CropDefinition"), "growthStage3", "gd3");
            AssertAlias(RuntimeTypes.Get("CropDatabase.CropDefinition"), "productIcon", "iconsanpham");
        }

        [Test]
        public void FarmingComponentsKeepLegacySerializationAliases()
        {
            AssertAlias(RuntimeTypes.Get("GameBootstrap"), "cropDatabase", "ns");
            AssertAlias(RuntimeTypes.Get("GameBootstrap"), "defaultCropPlotPositions", "vtNew");
            AssertAlias(RuntimeTypes.Get("GameBootstrap"), "cropPlotPrefab", "odat");
            AssertAlias(RuntimeTypes.Get("CropPlot"), "cropRenderer", "spr");
            AssertAlias(RuntimeTypes.Get("CropPlot"), "cropId", "idcay");
            AssertAlias(RuntimeTypes.Get("CropPlot"), "remainingSeconds", "time");
            AssertAlias(RuntimeTypes.Get("CropPlot"), "totalGrowthSeconds", "timegoc");
            AssertAlias(RuntimeTypes.Get("CropPlot"), "plantingEffectPrefab", "fxTrong");
            AssertAlias(RuntimeTypes.Get("CropPlot"), "experienceEffectPrefab", "fxExp");
            AssertAlias(RuntimeTypes.Get("CropPlot"), "productRewardPrefab", "fxSanPham");
            AssertAlias(RuntimeTypes.Get("CropPlot"), "harvestEffectPrefab", "fxThuHoach");
            AssertAlias(RuntimeTypes.Get("SeedItem"), "scrollRect", "MainScroll");
            AssertAlias(RuntimeTypes.Get("SeedItem"), "cropId", "id");
            AssertAlias(RuntimeTypes.Get("SeedItem"), "seedDragPrefab", "obSinh");
            AssertAlias(RuntimeTypes.Get("SeedItem"), "outOfSeedsPanel", "dialogHetHG");
            AssertAlias(RuntimeTypes.Get("SickleTool"), "sicklePrefab", "obsinh");
            AssertAlias(RuntimeTypes.Get("SickleTool"), "spawnedSickle", "obj");
            AssertAlias(RuntimeTypes.Get("OutOfSeedsPanel"), "seedCountText", "slhg");
            AssertAlias(RuntimeTypes.Get("OutOfSeedsPanel"), "gemCostText", "slkimcuong");
            AssertAlias(RuntimeTypes.Get("OutOfSeedsPanel"), "seedIcon", "iconhg");
            AssertAlias(RuntimeTypes.Get("OutOfSeedsPanel"), "cropId", "id");
            AssertAlias(RuntimeTypes.Get("SeedSelectionPanel"), "seedItemTemplate", "hg");
            AssertAlias(RuntimeTypes.Get("SeedSelectionPanel"), "seedListContent", "content");
            AssertAlias(RuntimeTypes.Get("CropTimerPanel"), "cropNameText", "txtName");
            AssertAlias(RuntimeTypes.Get("CropTimerPanel"), "timeText", "txtTime");
            AssertAlias(RuntimeTypes.Get("CropTimerPanel"), "gemCostText", "txtkc");
            AssertAlias(RuntimeTypes.Get("CropTimerPanel"), "progressImage", "imgTime");
            AssertAlias(RuntimeTypes.Get("CropTimerPanel"), "useGemsButton", "btnkc");
            AssertAlias(RuntimeTypes.Get("CropTimerPanel"), "gemCost", "sokc");
            AssertAlias(RuntimeTypes.Get("SeedDragHandler"), "cropId", "id");
        }

        [Test]
        public void CropPlotPrefabKeepsRequiredReferences()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Farming/CropPlot.prefab");
            Assert.That(prefab, Is.Not.Null);
            var plot = RuntimeTypes.GetComponent(prefab, "CropPlot");
            Assert.That(plot, Is.Not.Null);
            var serialized = new SerializedObject(plot);
            Assert.That(serialized.FindProperty("cropRenderer").objectReferenceValue, Is.Not.Null);
            Assert.That(serialized.FindProperty("plantingEffectPrefab").objectReferenceValue, Is.Not.Null);
            Assert.That(serialized.FindProperty("experienceEffectPrefab").objectReferenceValue, Is.Not.Null);
            Assert.That(serialized.FindProperty("productRewardPrefab").objectReferenceValue, Is.Not.Null);
            Assert.That(serialized.FindProperty("harvestEffectPrefab").objectReferenceValue, Is.Not.Null);
        }

        [Test]
        public void FarmingSourcesDoNotKeepLegacyDeclarations()
        {
            var sourcePaths = AssetDatabase.FindAssets("t:MonoScript", new[] { "Assets/Scripts/Farming" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => path.EndsWith(".cs", StringComparison.Ordinal))
                .Append("Assets/Scripts/Core/GameBootstrap.cs");

            var forbiddenDeclarations = new[]
            {
                @"\bstruct\s+NongSan\b",
                @"\bstruct\s+vtOdat\b",
                @"\bCropDatabase\s+ns\b",
                @"\bNongSan\[\]\s+data\b",
            };

            foreach (var path in sourcePaths)
            {
                var source = System.IO.File.ReadAllText(path);
                foreach (var pattern in forbiddenDeclarations)
                    Assert.That(Regex.IsMatch(source, pattern), Is.False, $"{path}: {pattern}");
            }
        }

        private static void AssertAlias(Type type, string fieldName, string legacyName)
        {
            var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, $"{type.Name}.{fieldName}");
            var aliases = field.GetCustomAttributes<FormerlySerializedAsAttribute>().Select(attribute => attribute.oldName);
            Assert.That(aliases, Does.Contain(legacyName), $"{type.Name}.{fieldName}");
        }
    }
}
