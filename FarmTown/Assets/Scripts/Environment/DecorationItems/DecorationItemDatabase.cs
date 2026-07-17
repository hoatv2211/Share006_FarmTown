using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DecorationItemDatabase : ScriptableObject {
    [System.Serializable]

    public struct DecorationItemDefinition
    {
        [FormerlySerializedAs("ten")] public string vietnameseName;
        [FormerlySerializedAs("name")] public string englishName;
        [FormerlySerializedAs("loaitien")] public bool usesGems;
        [FormerlySerializedAs("capmo")] public int unlockLevel;
        [FormerlySerializedAs("giamua")] public int purchasePrice;
        [FormerlySerializedAs("exp")] public int experienceReward;
        [FormerlySerializedAs("iconshop")] public Sprite shopIcon;
        [FormerlySerializedAs("iconlock")] public Sprite lockedIcon;
        [FormerlySerializedAs("vatpham")] public GameObject prefab;
    }
    [FormerlySerializedAs("data")] public DecorationItemDefinition[] decorationItems;
    // false selects gold; true selects diamonds.
}
