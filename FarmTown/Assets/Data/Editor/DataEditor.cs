using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class DataEditor  {

    [MenuItem("Data/Crops/Create Crop Database")]
    public static void CreateCropDatabase()
    {
        CropDatabase cropDatabase = ScriptableObject.CreateInstance<CropDatabase>();
        AssetDatabase.CreateAsset(cropDatabase, "Assets/Data/Crops/CropDatabase.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = cropDatabase;
    }
    [MenuItem("Data/Animals/Create Animal Database")]
    public static void CreateAnimalDatabase()
    {
        AnimalDatabase animalDatabase = ScriptableObject.CreateInstance<AnimalDatabase>();
        AssetDatabase.CreateAsset(animalDatabase, "Assets/Data/Animals/AnimalDatabase.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = animalDatabase;
    }
    [MenuItem("Data/Buildings/Create Building Database")]
    public static void CreateBuildingDatabase()
    {
        BuildingDatabase buildingDatabase = ScriptableObject.CreateInstance<BuildingDatabase>();
        AssetDatabase.CreateAsset(buildingDatabase, "Assets/Data/Buildings/BuildingDatabase.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = buildingDatabase;
    }
    [MenuItem("Data/Decoration Items/Create Decoration Item Database")]
    public static void CreateDecorationItemDatabase()
    {
        DecorationItemDatabase decorationItemDatabase = ScriptableObject.CreateInstance<DecorationItemDatabase>();
        AssetDatabase.CreateAsset(decorationItemDatabase, "Assets/Data/DecorationItems/DecorationItemDatabase.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = decorationItemDatabase;
    }
}
