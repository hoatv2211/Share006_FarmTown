using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FarmTown.Editor.Migration
{
    public static class FarmingArtMigration
    {
        private const string CropSource = "Assets/Sprite/map_play/caytrong";
        private const string CropTarget = "Assets/Art/Sprites/Farming/Crops";

        private static readonly IReadOnlyDictionary<string, string> CropFolders = new Dictionary<string, string>
        {
            ["bingo"] = "Pumpkin",
            ["cachua"] = "Tomato",
            ["cai thao"] = "Cabbage",
            ["carot"] = "Carrot",
            ["cu cai tim"] = "PurpleRadish",
            ["cu cai trang"] = "Turnip",
            ["cu lac"] = "Peanut",
            ["dautam"] = "Mulberry",
            ["dautay"] = "Strawberry",
            ["dautuong"] = "Soybean",
            ["duahau"] = "Watermelon",
            ["dualeo"] = "Cucumber",
            ["hanhcu"] = "Onion",
            ["hoa blue"] = "BlueFlower",
            ["hongdo"] = "Rose",
            ["hongvang"] = "YellowRose",
            ["icon_sp cay"] = "ProductIcons",
            ["khoaitay"] = "Potato",
            ["luami"] = "Wheat",
            ["mia"] = "SugarCane",
            ["ngo"] = "Corn",
            ["oai huong"] = "Lavender",
            ["ot"] = "Chili",
        };

        private static readonly IReadOnlyDictionary<string, string> ProductIcons = new Dictionary<string, string>
        {
            ["bingo.png"] = "Pumpkin.png",
            ["cachua.png"] = "Tomato.png",
            ["caithao.png"] = "Cabbage.png",
            ["carrot.png"] = "Carrot.png",
            ["cucaitim.png"] = "PurpleRadish.png",
            ["cucaitrang.png"] = "Turnip.png",
            ["culac.png"] = "Peanut.png",
            ["dautam.png"] = "Mulberry.png",
            ["dautay.png"] = "Strawberry.png",
            ["dautuong.png"] = "Soybean.png",
            ["dauhau.png"] = "Watermelon.png",
            ["dualeo.png"] = "Cucumber.png",
            ["hanhcu.png"] = "Onion.png",
            ["hoa blue.png"] = "BlueFlower.png",
            ["hoahong.png"] = "Rose.png",
            ["hongvang.png"] = "YellowRose.png",
            ["khoaitay.png"] = "Potato.png",
            ["luami.png"] = "Wheat.png",
            ["mia.png"] = "SugarCane.png",
            ["ngo.png"] = "Corn.png",
            ["hoaoaihuong.png"] = "Lavender.png",
            ["ot.png"] = "Chili.png",
        };

        private static readonly IReadOnlyDictionary<string, string> DirectMoves = new Dictionary<string, string>
        {
            ["Assets/Sprite/Ui/HetHatGiong"] = "Assets/Art/Sprites/Farming/UI/OutOfSeeds",
            ["Assets/Sprite/Ui/bg_hatgiong.png"] = "Assets/Art/Sprites/Farming/UI/SeedSelectionBackground.png",
            ["Assets/Sprite/map_play/odat.png"] = "Assets/Art/Sprites/Farming/Plots/CropPlot.png",
            ["Assets/Sprite/map_play/odat_lock.png"] = "Assets/Art/Sprites/Farming/Plots/LockedCropPlot.png",
            ["Assets/Sprite/odat2.png"] = "Assets/Art/Sprites/Farming/Plots/CropPlotVariant.png",
            ["Assets/Animation/Odat.controller"] = "Assets/Art/Animations/Farming/CropPlot.controller",
            ["Assets/Animation/odatclick.anim"] = "Assets/Art/Animations/Farming/CropPlotClick.anim",
            ["Assets/Animation/dialogHatgiong.controller"] = "Assets/Art/Animations/Farming/SeedSelection.controller",
            ["Assets/Animation/dialoghg.anim"] = "Assets/Art/Animations/Farming/SeedSelectionOpen.anim",
            ["Assets/Animation/dialoghgdong.anim"] = "Assets/Art/Animations/Farming/SeedSelectionClose.anim",
            ["Assets/Animation/DialogLiem.controller"] = "Assets/Art/Animations/Farming/SicklePanel.controller",
            ["Assets/Animation/dialogLiem.anim"] = "Assets/Art/Animations/Farming/SicklePanelOpen.anim",
            ["Assets/Animation/fxtrong.controller"] = "Assets/Art/Animations/Farming/PlantingEffect.controller",
            ["Assets/Animation/fxtrong.anim"] = "Assets/Art/Animations/Farming/PlantingEffect.anim",
            ["Assets/Animation/cay.controller"] = "Assets/Art/Animations/Farming/CropGrowth.controller",
            ["Assets/Animation/animcaylon.anim"] = "Assets/Art/Animations/Farming/CropGrowth.anim",
        };

        [MenuItem("Tools/Farm Town/Migration/Execute Farming Art")]
        public static void Execute()
        {
            var inventoryMoves = BuildInventoryMoves();
            var expectedGuids = inventoryMoves.ToDictionary(pair => pair.Value, pair => ExistingGuid(pair.Key, pair.Value));
            if (expectedGuids.Any(pair => string.IsNullOrEmpty(pair.Value)))
                throw new InvalidOperationException("Farming art source missing GUID: " + string.Join(", ", expectedGuids.Where(pair => string.IsNullOrEmpty(pair.Value)).Select(pair => pair.Key)));

            EnsureFolder("Assets/Art/Sprites/Farming");
            EnsureFolder("Assets/Art/Sprites/Farming/UI");
            EnsureFolder("Assets/Art/Sprites/Farming/Plots");
            EnsureFolder("Assets/Art/Animations/Farming");

            Move(CropSource, CropTarget);
            foreach (var folder in CropFolders)
                Move(CropTarget + "/" + folder.Key, CropTarget + "/" + folder.Value);
            foreach (var icon in ProductIcons)
                Move(CropTarget + "/ProductIcons/" + icon.Key, CropTarget + "/ProductIcons/" + icon.Value);

            foreach (var move in DirectMoves)
                Move(move.Key, move.Value);
            Move("Assets/Art/Sprites/Farming/UI/OutOfSeeds/bt_buy.png", "Assets/Art/Sprites/Farming/UI/OutOfSeeds/BuyButton.png");
            Move("Assets/Art/Sprites/Farming/UI/OutOfSeeds/dialog.png", "Assets/Art/Sprites/Farming/UI/OutOfSeeds/Panel.png");

            RenameController("Assets/Art/Animations/Farming/CropPlot.controller", "CropPlot", new Dictionary<string, string> { ["odatclick"] = "CropPlotClick" });
            RenameController("Assets/Art/Animations/Farming/SeedSelection.controller", "SeedSelection", new Dictionary<string, string> { ["dialoghg"] = "SeedSelectionOpen", ["dialoghgdong"] = "SeedSelectionClose" }, "dialoghg", "SeedSelectionState");
            RenameController("Assets/Art/Animations/Farming/SicklePanel.controller", "SicklePanel", new Dictionary<string, string> { ["dialogLiem"] = "SicklePanelOpen" });
            RenameController("Assets/Art/Animations/Farming/PlantingEffect.controller", "PlantingEffect", new Dictionary<string, string> { ["fxtrong"] = "PlantingEffect" });
            RenameController("Assets/Art/Animations/Farming/CropGrowth.controller", "CropGrowth", new Dictionary<string, string> { ["animcaylon"] = "CropGrowth" });

            RenameClip("Assets/Art/Animations/Farming/CropPlotClick.anim", "CropPlotClick");
            RenameClip("Assets/Art/Animations/Farming/SeedSelectionOpen.anim", "SeedSelectionOpen");
            RenameClip("Assets/Art/Animations/Farming/SeedSelectionClose.anim", "SeedSelectionClose");
            RenameClip("Assets/Art/Animations/Farming/SicklePanelOpen.anim", "SicklePanelOpen");
            RenameClip("Assets/Art/Animations/Farming/PlantingEffect.anim", "PlantingEffect");
            RenameClip("Assets/Art/Animations/Farming/CropGrowth.anim", "CropGrowth");
            RenameMapObjects();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            foreach (var expected in expectedGuids)
                if (!string.Equals(expected.Value, AssetDatabase.AssetPathToGUID(expected.Key), StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("GUID changed moving farming art to " + expected.Key);

            RenameInventoryBuilder.MarkMoved(inventoryMoves);
            Debug.Log($"Farming art migration moved {inventoryMoves.Count} tracked assets/folders with stable GUIDs.");
        }

        private static Dictionary<string, string> BuildInventoryMoves()
        {
            var moves = new Dictionary<string, string>(DirectMoves, StringComparer.OrdinalIgnoreCase)
            {
                [CropSource] = CropTarget,
                ["Assets/Sprite/Ui/HetHatGiong/bt_buy.png"] = "Assets/Art/Sprites/Farming/UI/OutOfSeeds/BuyButton.png",
                ["Assets/Sprite/Ui/HetHatGiong/dialog.png"] = "Assets/Art/Sprites/Farming/UI/OutOfSeeds/Panel.png",
            };
            foreach (var folder in CropFolders)
                moves[CropSource + "/" + folder.Key] = CropTarget + "/" + folder.Value;
            foreach (var icon in ProductIcons)
                moves[CropSource + "/icon_sp cay/" + icon.Key] = CropTarget + "/ProductIcons/" + icon.Value;
            return moves;
        }

        private static void RenameController(string path, string controllerName, IReadOnlyDictionary<string, string> states, string oldParameter = null, string newParameter = null)
        {
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            if (controller == null)
                throw new InvalidOperationException("Animator controller missing: " + path);
            controller.name = controllerName;
            foreach (var state in controller.layers.SelectMany(layer => layer.stateMachine.states).Select(child => child.state))
                if (states.TryGetValue(state.name, out var stateName))
                    state.name = stateName;
            if (!string.IsNullOrEmpty(oldParameter))
            {
                var parameters = controller.parameters;
                foreach (var parameter in parameters)
                    if (parameter.name == oldParameter)
                        parameter.name = newParameter;
                controller.parameters = parameters;
                RenameTransitionConditions(controller, oldParameter, newParameter);
            }
            EditorUtility.SetDirty(controller);
        }

        private static void RenameTransitionConditions(AnimatorController controller, string oldParameter, string newParameter)
        {
            foreach (var stateMachine in controller.layers.Select(layer => layer.stateMachine))
            {
                foreach (var transition in stateMachine.anyStateTransitions)
                    RenameConditions(transition, oldParameter, newParameter);
                foreach (var transition in stateMachine.entryTransitions)
                    RenameConditions(transition, oldParameter, newParameter);
                foreach (var state in stateMachine.states.Select(child => child.state))
                    foreach (var transition in state.transitions)
                        RenameConditions(transition, oldParameter, newParameter);
            }
        }

        private static void RenameConditions(AnimatorTransitionBase transition, string oldParameter, string newParameter)
        {
            var conditions = transition.conditions;
            for (var index = 0; index < conditions.Length; index++)
                if (conditions[index].parameter == oldParameter)
                    conditions[index].parameter = newParameter;
            transition.conditions = conditions;
            EditorUtility.SetDirty(transition);
        }
        private static void RenameClip(string path, string clipName)
        {
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip == null)
                throw new InvalidOperationException("Animation clip missing: " + path);
            clip.name = clipName;
            EditorUtility.SetDirty(clip);
        }

        private static void RenameMapObjects()
        {
            var setup = EditorSceneManager.GetSceneManagerSetup();
            try
            {
                var scene = EditorSceneManager.OpenScene("Assets/Scenes/map.unity", OpenSceneMode.Single);
                var names = new Dictionary<string, string>
                {
                    ["dialogHatgiong"] = "SeedSelectionPanel",
                    ["dialogLiem"] = "SicklePanel",
                    ["dialogTime"] = "CropTimerPanel",
                    ["DialogHetHG"] = "OutOfSeedsPanel",
                };
                var transforms = scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<Transform>(true)).ToArray();
                foreach (var transform in transforms)
                {
                    if (transform.name == "dialogLiem" || transform.name == "SicklePanel")
                    {
                        transform.name = "dialogLiem";
                        continue;
                    }
                    if (names.TryGetValue(transform.name, out var name))
                        transform.name = name;
                }
                foreach (var behaviour in transforms.SelectMany(transform => transform.GetComponents<MonoBehaviour>()).Where(HasSickleToolScript))
                {
                    var panel = behaviour.transform.parent;
                    while (panel != null && panel.name != "dialogLiem")
                        panel = panel.parent;
                    if (panel != null)
                        panel.name = "SicklePanel";
                }
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
            finally
            {
                if (setup.Length > 0)
                    EditorSceneManager.RestoreSceneManagerSetup(setup);
            }
        }

        private static bool HasSickleToolScript(MonoBehaviour behaviour)
        {
            if (behaviour == null)
                return false;
            var script = new SerializedObject(behaviour).FindProperty("m_Script")?.objectReferenceValue;
            return script != null && AssetDatabase.GetAssetPath(script) == "Assets/Scripts/Farming/SickleTool.cs";
        }
        private static void EnsureFolder(string path)
        {
            var current = "Assets";
            foreach (var segment in path.Split('/').Skip(1))
            {
                var next = current + "/" + segment;
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, segment);
                current = next;
            }
        }

        private static string ExistingGuid(string source, string target)
        {
            var guid = AssetDatabase.AssetPathToGUID(source);
            return string.IsNullOrEmpty(guid) ? AssetDatabase.AssetPathToGUID(target) : guid;
        }

        private static void Move(string source, string target)
        {
            var sourceGuid = AssetDatabase.AssetPathToGUID(source);
            var targetGuid = AssetDatabase.AssetPathToGUID(target);
            if ((!string.IsNullOrEmpty(sourceGuid) && string.Equals(sourceGuid, targetGuid, StringComparison.OrdinalIgnoreCase))
                || (string.IsNullOrEmpty(sourceGuid) && !string.IsNullOrEmpty(targetGuid)))
                return;
            var error = AssetDatabase.MoveAsset(source, target);
            if (!string.IsNullOrEmpty(error))
                throw new InvalidOperationException($"Asset move failed: {source} -> {target}: {error}");
        }
    }
}
