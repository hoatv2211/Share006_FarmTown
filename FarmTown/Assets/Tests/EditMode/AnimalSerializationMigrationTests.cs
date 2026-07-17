using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using FarmTown.Save;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace FarmTown.Tests.EditMode
{
    public sealed class AnimalSerializationMigrationTests
    {
        private const string DatabasePath = "Assets/Data/Animals/AnimalDatabase.asset";

        [Test]
        public void AnimalDatabasePreservesDefinitionsAndReferences()
        {
            var database = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(DatabasePath);
            Assert.That(database, Is.Not.Null);
            Assert.That(AssetDatabase.AssetPathToGUID(DatabasePath), Is.EqualTo("f40fa68052038384484d5f4b5f8fbe41"));
            var animals = new SerializedObject(database).FindProperty("animals");
            Assert.That(animals.arraySize, Is.EqualTo(7));
            for (var index = 0; index < animals.arraySize; index++)
            {
                var animal = animals.GetArrayElementAtIndex(index);
                Assert.That(animal.FindPropertyRelative("shopIcon").objectReferenceValue, Is.Not.Null);
                Assert.That(animal.FindPropertyRelative("lockedShopIcon").objectReferenceValue, Is.Not.Null);
                Assert.That(animal.FindPropertyRelative("productIcon").objectReferenceValue, Is.Not.Null);
                Assert.That(animal.FindPropertyRelative("penIcon").objectReferenceValue, Is.Not.Null);
                Assert.That(animal.FindPropertyRelative("lockedPenIcon").objectReferenceValue, Is.Not.Null);
                Assert.That(animal.FindPropertyRelative("penPrefab").objectReferenceValue, Is.Not.Null);
            }
        }

        [Test]
        public void AnimalDatabaseFieldsKeepLegacySerializationAliases()
        {
            AssertAlias(RuntimeTypes.Get("AnimalDatabase"), "animals", "data");
            AssertAlias(RuntimeTypes.Get("AnimalDatabase.AnimalDefinition"), "vietnameseName", "tenvn");
            AssertAlias(RuntimeTypes.Get("AnimalDatabase.AnimalDefinition"), "englishName", "namevn");
            AssertAlias(RuntimeTypes.Get("AnimalDatabase.AnimalDefinition"), "vietnameseProductName", "sanpham");
            AssertAlias(RuntimeTypes.Get("AnimalDatabase.AnimalDefinition"), "englishProductName", "product");
            AssertAlias(RuntimeTypes.Get("AnimalDatabase.AnimalDefinition"), "productionSeconds", "time");
            AssertAlias(RuntimeTypes.Get("AnimalDatabase.AnimalDefinition"), "animalUnlockLevel", "capmo");
            AssertAlias(RuntimeTypes.Get("AnimalDatabase.AnimalDefinition"), "secondPenUnlockLevel", "capmo2");
            AssertAlias(RuntimeTypes.Get("AnimalDatabase.AnimalDefinition"), "animalPurchasePrice", "giamuavn");
            AssertAlias(RuntimeTypes.Get("AnimalDatabase.AnimalDefinition"), "penPurchasePrice", "giamuachuong");
            AssertAlias(RuntimeTypes.Get("AnimalDatabase.AnimalDefinition"), "productSalePrice", "giaban");
            AssertAlias(RuntimeTypes.Get("AnimalDatabase.AnimalDefinition"), "experienceReward", "exp");
            AssertAlias(RuntimeTypes.Get("AnimalDatabase.AnimalDefinition"), "productYield", "sanluong");
            AssertAlias(RuntimeTypes.Get("AnimalDatabase.AnimalDefinition"), "shopIcon", "iconvatnuoishop");
            AssertAlias(RuntimeTypes.Get("AnimalDatabase.AnimalDefinition"), "lockedShopIcon", "iconvatnuoilock");
            AssertAlias(RuntimeTypes.Get("AnimalDatabase.AnimalDefinition"), "productIcon", "iconsanpham");
            AssertAlias(RuntimeTypes.Get("AnimalDatabase.AnimalDefinition"), "vietnamesePenName", "tenchuong");
            AssertAlias(RuntimeTypes.Get("AnimalDatabase.AnimalDefinition"), "englishPenName", "namechuong");
            AssertAlias(RuntimeTypes.Get("AnimalDatabase.AnimalDefinition"), "penIcon", "iconchuong");
            AssertAlias(RuntimeTypes.Get("AnimalDatabase.AnimalDefinition"), "lockedPenIcon", "iconchuongLock");
            AssertAlias(RuntimeTypes.Get("AnimalDatabase.AnimalDefinition"), "penPrefab", "chuong");
            AssertAlias(RuntimeTypes.Get("GameBootstrap"), "animalDatabase", "vn");
        }

        [Test]
        public void AnimalRuntimeScriptsKeepGuidsAtEnglishPaths()
        {
            var expected = new Dictionary<string, string>
            {
                ["Assets/Scripts/Animals/FarmAnimal.cs"] = "84d59168ab0d6a946915af5d182fa97a",
                ["Assets/Scripts/Animals/AnimalDragHandler.cs"] = "db9517af4c3b7564eb81988d2eb7fac3",
                ["Assets/Scripts/Animals/AnimalPen.cs"] = "956833b9937a4e7459d8fc57660c080d",
                ["Assets/Scripts/Animals/IFeedReceiver.cs"] = "6455a19485b98784faa81f66268430e4",
                ["Assets/Scripts/Animals/FeedDragHandler.cs"] = "68fd495638533a74e90fb5dc2cfb2b67",
                ["Assets/Scripts/Animals/AnimalProductIcon.cs"] = "49b30ffba3aa26c458b06420801499c9",
                ["Assets/Scripts/Animals/AnimalFeedingPanel.cs"] = "e895bccc32c0d9b4985c381c92e44969",
                ["Assets/Scripts/Animals/AnimalTimerPanel.cs"] = "4a64bcfb90f10fa4a83f2d3c3a5f8b54",
                ["Assets/Scripts/Animals/OutOfFeedPanel.cs"] = "4f61f7336db4ab24089e1e3c477675fc",
                ["Assets/Scripts/Shop/AnimalShopItem.cs"] = "32a297c3dbffeec4baea0bc07d4f2cde",
                ["Assets/Scripts/Shop/AnimalShopList.cs"] = "289399b9753e1d34dbe0e05ab4c6381f",
                ["Assets/Scripts/Shop/AnimalPenShopItem.cs"] = "8bbce149606282c409051bdffd26818d",
                ["Assets/Scripts/Shop/AnimalPenShopList.cs"] = "0e2b7a671c5029e4bb7934220f407779",
            };

            foreach (var pair in expected)
                Assert.That(AssetDatabase.AssetPathToGUID(pair.Key), Is.EqualTo(pair.Value), pair.Key);
        }

        [Test]
        public void AnimalRuntimeFieldsKeepLegacySerializationAliases()
        {
            AssertAlias(RuntimeTypes.Get("AnimalPen"), "animalTypeId", "id");
            AssertAlias(RuntimeTypes.Get("AnimalPen"), "feedCropId", "idThucAn");
            AssertAlias(RuntimeTypes.Get("AnimalPen"), "instanceIndex", "stt");
            AssertAlias(RuntimeTypes.Get("FarmAnimal"), "remainingSeconds", "time");
            AssertAlias(RuntimeTypes.Get("FarmAnimal"), "totalProductionSeconds", "timegoc");
            AssertAlias(RuntimeTypes.Get("FarmAnimal"), "experienceRewardPrefab", "iconExp");
            AssertAlias(RuntimeTypes.Get("FarmAnimal"), "productRewardPrefab", "iconSp");
            AssertAlias(RuntimeTypes.Get("FarmAnimal"), "feedingEffectPrefab", "FxAn");
            AssertAlias(RuntimeTypes.Get("FarmAnimal"), "harvestEffectPrefab", "HieuUngThuHoach");
            AssertAlias(RuntimeTypes.Get("AnimalDragHandler"), "animalTypeId", "id");
            AssertAlias(RuntimeTypes.Get("AnimalDragHandler"), "hasValidTarget", "check");
            AssertAlias(RuntimeTypes.Get("AnimalDragHandler"), "targetPen", "ob");
            AssertAlias(RuntimeTypes.Get("FeedDragHandler"), "feedCropId", "id");
            AssertAlias(RuntimeTypes.Get("AnimalFeedingPanel"), "feedDragPrefab", "obsinh");
            AssertAlias(RuntimeTypes.Get("AnimalFeedingPanel"), "outOfFeedPanel", "dialogHetTa");
            AssertAlias(RuntimeTypes.Get("AnimalTimerPanel"), "animalNameText", "txtName");
            AssertAlias(RuntimeTypes.Get("AnimalTimerPanel"), "timeText", "txtTime");
            AssertAlias(RuntimeTypes.Get("AnimalTimerPanel"), "gemCostText", "txtkc");
            AssertAlias(RuntimeTypes.Get("AnimalTimerPanel"), "progressImage", "imgTime");
            AssertAlias(RuntimeTypes.Get("AnimalTimerPanel"), "useGemsButton", "btnkc");
            AssertAlias(RuntimeTypes.Get("AnimalTimerPanel"), "gemCost", "sokc");
            AssertAlias(RuntimeTypes.Get("OutOfFeedPanel"), "targetCountText", "slhg");
            AssertAlias(RuntimeTypes.Get("OutOfFeedPanel"), "gemCostText", "slkimcuong");
            AssertAlias(RuntimeTypes.Get("OutOfFeedPanel"), "feedIcon", "iconhg");
            AssertAlias(RuntimeTypes.Get("OutOfFeedPanel"), "feedCropId", "id");
            AssertAlias(RuntimeTypes.Get("AnimalShopItem"), "scrollRect", "MainScroll");
            AssertAlias(RuntimeTypes.Get("AnimalShopItem"), "animalDragPrefab", "vatnuoi");
            AssertAlias(RuntimeTypes.Get("AnimalShopItem"), "animalTypeId", "id");
            AssertAlias(RuntimeTypes.Get("AnimalPenShopItem"), "scrollRect", "MainScroll");
            AssertAlias(RuntimeTypes.Get("AnimalPenShopItem"), "cropPlotPrefab", "Odat");
            AssertAlias(RuntimeTypes.Get("AnimalPenShopItem"), "itemTypeId", "id");
            AssertAlias(RuntimeTypes.Get("AnimalPenShopList"), "cropPlotIcon", "iconOdat");
            AssertAlias(RuntimeTypes.Get("AnimalPenShopList"), "lockedCropPlotIcon", "iconodatlock");
        }

        [Test]
        public void AnimalPrefabsKeepGuidsAtEnglishPaths()
        {
            var expected = new Dictionary<string, string>
            {
                ["Assets/Prefabs/Animals/ChickenPen.prefab"] = "3b51724a286813a4abb7fbd4c537d148",
                ["Assets/Prefabs/Animals/CowPen.prefab"] = "8a77676f0a0e86441970e70c48d96974",
                ["Assets/Prefabs/Animals/DuckPen.prefab"] = "7226585c080f69b47a9557fc9752d66c",
                ["Assets/Prefabs/Animals/PigPen.prefab"] = "dd26f6dc9ac02f94b816326a25f61c22",
                ["Assets/Prefabs/Animals/SheepPen.prefab"] = "74c961efff745ef449a5a6e3c803e103",
                ["Assets/Prefabs/Animals/BuffaloPen.prefab"] = "e72db0d208fa6ef47b8b3f221b26e19c",
                ["Assets/Prefabs/Animals/GoatPen.prefab"] = "1baf87cbe18dff347812425486d213d1",
                ["Assets/Prefabs/Animals/AnimalFeed.prefab"] = "e05c7f9d6d775c44aa2fe5d1b0e42734",
                ["Assets/Prefabs/Animals/AnimalDrag.prefab"] = "d3fa95f9336dbac4dabeda5faa19688b",
            };

            foreach (var pair in expected)
                Assert.That(AssetDatabase.AssetPathToGUID(pair.Key), Is.EqualTo(pair.Value), pair.Key);
        }

        [Test]
        public void AnimalDatabaseUsesEnglishPrefabPaths()
        {
            var database = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(DatabasePath);
            var expectedPaths = new[]
            {
                "Assets/Prefabs/Animals/ChickenPen.prefab",
                "Assets/Prefabs/Animals/CowPen.prefab",
                "Assets/Prefabs/Animals/DuckPen.prefab",
                "Assets/Prefabs/Animals/PigPen.prefab",
                "Assets/Prefabs/Animals/SheepPen.prefab",
                "Assets/Prefabs/Animals/BuffaloPen.prefab",
                "Assets/Prefabs/Animals/GoatPen.prefab",
            };

            var animals = new SerializedObject(database).FindProperty("animals");
            var actualPaths = Enumerable.Range(0, animals.arraySize)
                .Select(index => AssetDatabase.GetAssetPath(animals.GetArrayElementAtIndex(index).FindPropertyRelative("penPrefab").objectReferenceValue));
            Assert.That(actualPaths, Is.EqualTo(expectedPaths));
        }

        [Test]
        public void AnimalSpineAnimationsUseEnglishKeys()
        {
            foreach (var animalName in new[] { "Chicken", "Cow", "Duck", "Pig", "Sheep", "Buffalo", "Goat" })
            {
                var json = File.ReadAllText($"Assets/Art/Sprites/Animals/Spine/{animalName}/skeleton.json");
                Assert.That(json, Does.Contain("\"Idle\":"));
                Assert.That(json, Does.Contain("\"Feeding\":"));
                Assert.That(json, Does.Contain("\"Ready\":"));
                Assert.That(json, Does.Not.Contain("\"doi\":"));
                Assert.That(json, Does.Not.Contain("\"an\":"));
                Assert.That(json, Does.Not.Contain("\"thuhoach\":"));
            }
        }

        [TestCase(0, 0, "ga")]
        [TestCase(0, 1, "ga (1)")]
        [TestCase(0, 2, "ga (2)")]
        [TestCase(6, 0, "De")]
        [TestCase(6, 1, "De (1)")]
        [TestCase(6, 2, "De (2)")]
        public void LegacyAnimalNamesPreserveSlotSpecificSaveKeys(int animalTypeId, int slotIndex, string expected)
        {
            Assert.That(AnimalPersistence.DefaultLegacyAnimalName(animalTypeId, slotIndex), Is.EqualTo(expected));
        }

        [Test]
        public void EnglishAnimalChildrenResolveByStableRuntimeKey()
        {
            var root = PrefabUtility.LoadPrefabContents("Assets/Prefabs/Animals/ChickenPen.prefab");
            try
            {
                var animals = RuntimeTypes.GetComponentsInChildren(root, "FarmAnimal")
                    .OrderBy(animal => animal.transform.GetSiblingIndex())
                    .ToArray();

                Assert.That(animals.Select(animal => animal.name), Is.EqualTo(new[] { "Chicken", "Chicken (1)", "Chicken (2)" }));
                var animalType = RuntimeTypes.Get("FarmAnimal");
                var ensureIdentity = animalType.GetMethod("EnsureIdentity", BindingFlags.Instance | BindingFlags.NonPublic);
                var findRuntimeAnimal = animalType.GetMethod("FindRuntimeAnimal", BindingFlags.Static | BindingFlags.Public);
                Assert.That(ensureIdentity, Is.Not.Null);
                Assert.That(findRuntimeAnimal, Is.Not.Null);

                for (var slotIndex = 0; slotIndex < animals.Length; slotIndex++)
                {
                    ensureIdentity.Invoke(animals[slotIndex], null);
                    Assert.That(findRuntimeAnimal.Invoke(null, new object[] { $"animal-pen-0-0:{slotIndex}" }), Is.SameAs(animals[slotIndex]));
                }
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
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
