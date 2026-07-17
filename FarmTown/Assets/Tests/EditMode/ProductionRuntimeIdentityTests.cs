using System.Reflection;
using NUnit.Framework;
using UnityEditor;

namespace FarmTown.Tests.EditMode
{
    public sealed class ProductionRuntimeIdentityTests
    {
        [Test]
        public void ProductionPrefabRegistersStableIdentityWithoutUsingObjectName()
        {
            var root = PrefabUtility.LoadPrefabContents("Assets/Prefabs/Production/Bakery.prefab");
            try
            {
                var building = RuntimeTypes.GetComponent(root, "ProductionBuilding");
                Assert.That(building, Is.Not.Null);
                root.name = "RenamedEnglishBuilding";

                var buildingType = building.GetType();
                buildingType.GetMethod("InitializeIdentity").Invoke(building, new object[] { 0, 3 });
                var stableId = (string)buildingType.GetProperty("StableId").GetValue(building);
                var legacyName = (string)buildingType.GetProperty("LegacyName").GetValue(building);
                var tryFind = buildingType.GetMethod("TryFind", BindingFlags.Static | BindingFlags.Public);

                Assert.That(stableId, Is.EqualTo("production-building-0-3"));
                Assert.That(legacyName, Is.EqualTo("nhamay03"));
                var arguments = new object[] { stableId, null };
                Assert.That(tryFind.Invoke(null, arguments), Is.True);
                Assert.That(arguments[1], Is.SameAs(building));

                buildingType.GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(building, null);
                arguments = new object[] { stableId, null };
                Assert.That(tryFind.Invoke(null, arguments), Is.False);
                buildingType.GetMethod("OnEnable", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(building, null);
                arguments = new object[] { stableId, null };
                Assert.That(tryFind.Invoke(null, arguments), Is.True);
                Assert.That(arguments[1], Is.SameAs(building));
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }
    }
}
