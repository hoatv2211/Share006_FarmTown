using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AnimalDatabase : ScriptableObject {
    [System.Serializable]
    public struct AnimalDefinition
    {
        [FormerlySerializedAs("tenvn")] public string vietnameseName;
        [FormerlySerializedAs("namevn")] public string englishName;
        [FormerlySerializedAs("sanpham")] public string vietnameseProductName;
        [FormerlySerializedAs("product")] public string englishProductName;
        [FormerlySerializedAs("time")] public int productionSeconds;
        [FormerlySerializedAs("capmo")] public int animalUnlockLevel;
        [FormerlySerializedAs("capmo2")] public int secondPenUnlockLevel;
        [FormerlySerializedAs("giamuavn")] public int animalPurchasePrice;
        [FormerlySerializedAs("giamuachuong")] public int penPurchasePrice;
        [FormerlySerializedAs("giaban")] public int productSalePrice;
        [FormerlySerializedAs("exp")] public int experienceReward;
        [FormerlySerializedAs("sanluong")] public int productYield;
        [FormerlySerializedAs("iconvatnuoishop")] public Sprite shopIcon;
        [FormerlySerializedAs("iconvatnuoilock")] public Sprite lockedShopIcon;
        [FormerlySerializedAs("iconsanpham")] public Sprite productIcon;
        [FormerlySerializedAs("tenchuong")] public string vietnamesePenName;
        [FormerlySerializedAs("namechuong")] public string englishPenName;
        [FormerlySerializedAs("iconchuong")] public Sprite penIcon;
        [FormerlySerializedAs("iconchuongLock")] public Sprite lockedPenIcon;
        [FormerlySerializedAs("chuong")] public GameObject penPrefab;
    }
    [FormerlySerializedAs("data")] public AnimalDefinition[] animals;
}
