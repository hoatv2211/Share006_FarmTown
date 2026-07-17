using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FarmTown.Tests.EditMode
{
    public sealed class FarmingArtMigrationTests
    {
        private static readonly IReadOnlyDictionary<string, string> RequiredGuidMoves = new Dictionary<string, string>
        {
            ["17367888b36df6649bab3fdcaa21770c"] = "Assets/Art/Sprites/Farming/Plots/CropPlot.png",
            ["d6a53f1f4a72b2c499a6f8f0aa965a97"] = "Assets/Art/Sprites/Farming/Plots/LockedCropPlot.png",
            ["78dc5017777695a42b6692e6f7fc1510"] = "Assets/Art/Sprites/Farming/Plots/CropPlotVariant.png",
            ["0be8916edcf0f4848821fec959b0eaa0"] = "Assets/Art/Sprites/Farming/UI/OutOfSeeds/BuyButton.png",
            ["ad96a75e535656d47ada8e59121e7a0e"] = "Assets/Art/Sprites/Farming/UI/OutOfSeeds/Panel.png",
            ["4cdd3684b4171844e8d41eea58c303ef"] = "Assets/Art/Sprites/Farming/UI/SeedSelectionBackground.png",
            ["55e2d01f77a3c4f45b716b66aba6755d"] = "Assets/Art/Animations/Farming/CropPlot.controller",
            ["4b67e2e5221d6a548ae140a7a00c1492"] = "Assets/Art/Animations/Farming/CropPlotClick.anim",
            ["ee3d4e51f9e621a429c1704dc88ee703"] = "Assets/Art/Animations/Farming/SeedSelection.controller",
            ["84656f872b953f440b0f703f2b82911e"] = "Assets/Art/Animations/Farming/SeedSelectionOpen.anim",
            ["81a4abb2a10e80f43831b869a3f82016"] = "Assets/Art/Animations/Farming/SeedSelectionClose.anim",
            ["474f1097288d19c438871154d9d83e57"] = "Assets/Art/Animations/Farming/SicklePanel.controller",
            ["ccafd96cf7a417c43915a0293184c536"] = "Assets/Art/Animations/Farming/SicklePanelOpen.anim",
            ["98c3fb59984054f448af9ec81f6fb1a4"] = "Assets/Art/Animations/Farming/PlantingEffect.controller",
            ["e7f5bee6f5d86af41baecf80b0830cc8"] = "Assets/Art/Animations/Farming/PlantingEffect.anim",
            ["9a80b755db238f94f908a14898d75a66"] = "Assets/Art/Animations/Farming/CropGrowth.controller",
            ["23e271280f0d799439d0fca82f8902f4"] = "Assets/Art/Animations/Farming/CropGrowth.anim",
        };

        [Test]
        public void FarmingAssetsKeepExpectedGuidsAtEnglishPaths()
        {
            foreach (var expected in RequiredGuidMoves)
                Assert.That(AssetDatabase.GUIDToAssetPath(expected.Key), Is.EqualTo(expected.Value), expected.Key);
        }

        [Test]
        public void FarmingControllersUseEnglishNamesAndStates()
        {
            AssertController("Assets/Art/Animations/Farming/CropPlot.controller", "CropPlot", "CropPlotClick");
            AssertController("Assets/Art/Animations/Farming/SicklePanel.controller", "SicklePanel", "SicklePanelOpen");
            AssertController("Assets/Art/Animations/Farming/PlantingEffect.controller", "PlantingEffect", "PlantingEffect");
            AssertController("Assets/Art/Animations/Farming/CropGrowth.controller", "CropGrowth", "CropGrowth");
            var seedController = AssertController("Assets/Art/Animations/Farming/SeedSelection.controller", "SeedSelection", "SeedSelectionOpen", "SeedSelectionClose");
            Assert.That(seedController.parameters.Select(parameter => parameter.name), Is.EquivalentTo(new[] { "SeedSelectionState" }));
        }

        [Test]
        public void FarmingAnimationClipsUseEnglishNames()
        {
            AssertClip("Assets/Art/Animations/Farming/CropPlotClick.anim", "CropPlotClick");
            AssertClip("Assets/Art/Animations/Farming/SeedSelectionOpen.anim", "SeedSelectionOpen");
            AssertClip("Assets/Art/Animations/Farming/SeedSelectionClose.anim", "SeedSelectionClose");
            AssertClip("Assets/Art/Animations/Farming/SicklePanelOpen.anim", "SicklePanelOpen");
            AssertClip("Assets/Art/Animations/Farming/PlantingEffect.anim", "PlantingEffect");
            AssertClip("Assets/Art/Animations/Farming/CropGrowth.anim", "CropGrowth");
        }

        [Test]
        public void MapUsesEnglishFarmingPanelNames()
        {
            var setup = EditorSceneManager.GetSceneManagerSetup();
            try
            {
                var scene = EditorSceneManager.OpenScene("Assets/Scenes/map.unity", OpenSceneMode.Single);
                var names = scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<Transform>(true)).Select(transform => transform.name).ToArray();
                Assert.That(names, Does.Contain("SeedSelectionPanel"));
                Assert.That(names.Count(name => name == "SicklePanel"), Is.EqualTo(1));
                Assert.That(names, Does.Contain("CropTimerPanel"));
                Assert.That(names, Does.Contain("OutOfSeedsPanel"));
                Assert.That(names, Does.Not.Contain("dialogHatgiong"));
                Assert.That(names.Count(name => name == "SickleDialog"), Is.EqualTo(3));
                Assert.That(names, Does.Not.Contain("dialogTime"));
                Assert.That(names, Does.Not.Contain("DialogHetHG"));
            }
            finally
            {
                if (setup.Length > 0)
                    EditorSceneManager.RestoreSceneManagerSetup(setup);
            }
        }


        [Test]
        public void FarmingRuntimeUsesEnglishAnimatorHashes()
        {
            var animatorParameters = File.ReadAllText(Path.Combine(Application.dataPath, "Scripts/Core/AnimatorParameters.cs"));
            Assert.That(animatorParameters, Does.Contain("Animator.StringToHash(\"CropPlotClick\")"));
            Assert.That(animatorParameters, Does.Contain("Animator.StringToHash(\"SeedSelectionState\")"));
            Assert.That(animatorParameters, Does.Contain("Animator.StringToHash(\"SicklePanelOpen\")"));

            var farmingScripts = new[]
            {
                "Scripts/Farming/CropPlotClickHandler.cs",
                "Scripts/Farming/SeedSelectionPanel.cs",
                "Scripts/Farming/SickleTool.cs",
            };
            foreach (var relativePath in farmingScripts)
            {
                var source = File.ReadAllText(Path.Combine(Application.dataPath, relativePath));
                Assert.That(source, Does.Not.Contain("dialoghg"), relativePath);
                Assert.That(source, Does.Not.Contain("dialogLiem"), relativePath);
                Assert.That(source, Does.Not.Contain("odatclick"), relativePath);
            }
        }
        private static AnimatorController AssertController(string path, string expectedName, params string[] expectedStates)
        {
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            Assert.That(controller, Is.Not.Null, path);
            Assert.That(controller.name, Is.EqualTo(expectedName));
            Assert.That(controller.layers.SelectMany(layer => layer.stateMachine.states).Select(state => state.state.name), Is.EquivalentTo(expectedStates));
            return controller;
        }

        private static void AssertClip(string path, string expectedName)
        {
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            Assert.That(clip, Is.Not.Null, path);
            Assert.That(clip.name, Is.EqualTo(expectedName));
        }
    }
}
