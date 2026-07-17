using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BuildingDatabase : ScriptableObject
{
    [System.Serializable]
    public struct BuildingDefinition
    {
        [FormerlySerializedAs("ten")] public string vietnameseName;
        [FormerlySerializedAs("name")] public string englishName;
        [FormerlySerializedAs("capmo")] public int unlockLevel;
        [FormerlySerializedAs("capmo2")] public int secondaryUnlockLevel;
        [FormerlySerializedAs("iconshop")] public Sprite shopIcon;
        [FormerlySerializedAs("iconshoplock")] public Sprite lockedShopIcon;
        [FormerlySerializedAs("congtrinh")] public GameObject prefab;
    }

    [FormerlySerializedAs("data")] public BuildingDefinition[] buildings;
}
