using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CropDatabase : ScriptableObject
{
    [System.Serializable]

    public struct CropDefinition
    {
        [FormerlySerializedAs("ten")] public string vietnameseName;
        [FormerlySerializedAs("name")] public string englishName;
        [FormerlySerializedAs("sanpham")] public string vietnameseProductName;
        [FormerlySerializedAs("product")] public string englishProductName;
        [FormerlySerializedAs("time")] public int growthSeconds;
        [FormerlySerializedAs("capmo")] public int unlockLevel;
        [FormerlySerializedAs("giamua")] public int purchasePrice;
        [FormerlySerializedAs("giaban")] public int salePrice;
        [FormerlySerializedAs("exp")] public int experienceReward;
        [FormerlySerializedAs("sanluong")] public int harvestYield;
        [FormerlySerializedAs("gd1")] public Sprite growthStage1;
        [FormerlySerializedAs("gd2")] public Sprite growthStage2;
        [FormerlySerializedAs("gd3")] public Sprite growthStage3;
        [FormerlySerializedAs("iconsanpham")] public Sprite productIcon;
    }
    [FormerlySerializedAs("data")] public CropDefinition[] crops;
}
