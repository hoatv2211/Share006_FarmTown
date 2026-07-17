using System.Collections;
using System.Collections.Generic;
using FarmTown.Save;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class OutOfSeedsPanel : MonoBehaviour {
    private static int TargetCount => PlayerPrefs.HasKey(SaveKeys.OutOfSeedTargetCount) ? PlayerPrefs.GetInt(SaveKeys.OutOfSeedTargetCount) : PlayerPrefs.GetInt(LegacySaveKeys.OutOfSeedTargetCount);

    [FormerlySerializedAs("slhg")] public Text seedCountText;
    [FormerlySerializedAs("slkimcuong")] public Text gemCostText;
    [FormerlySerializedAs("iconhg")] public Image seedIcon;
    [FormerlySerializedAs("id")] public int cropId;
    public static bool TryResolveTargets(int count, out List<CropPlot> plots)
    {
        plots = new List<CropPlot>(count);
        var stableIds = new List<string>(count);
        for (var index = 0; index < count; index++)
        {
            var stableId = PlayerPrefs.GetString(SaveKeys.OutOfSeedTarget(index));
            if (stableId.Length == 0 && FarmingPlotPersistence.TryGetStableId(PlayerPrefs.GetString(LegacySaveKeys.OutOfSeedTarget(index)), out var migratedId))
                stableId = migratedId;
            stableIds.Add(stableId);
        }

        if (!FarmingPlotPersistence.TryResolveStableInstances(stableIds, out var identities))
            return false;
        foreach (var identity in identities)
        {
            var plot = identity.GetComponent<CropPlot>();
            if (plot == null)
            {
                plots.Clear();
                return false;
            }
            plots.Add(plot);
        }
        return true;
    }

    private static bool TryGetTarget(int index, out CropPlot plot)
    {
        plot = null;
        var stableId = PlayerPrefs.GetString(SaveKeys.OutOfSeedTarget(index));
        if (stableId.Length == 0 && FarmingPlotPersistence.TryGetStableId(PlayerPrefs.GetString(LegacySaveKeys.OutOfSeedTarget(index)), out var migratedId)) stableId = migratedId;
        if (!StableInstanceId.TryFind(stableId, out var identity)) return false;
        plot = identity.GetComponent<CropPlot>();
        return plot != null;
    }

	public void BuyDiamonds()
    {
        if (TargetCount <= PlayerPrefs.GetInt("diamond"))
        {
            if (!TryResolveTargets(TargetCount, out var targets))
            {
                NotificationController.Instance.ShowCanvasNotification("Không thể tìm thấy ô đất", "Crop plot could not be found");
                return;
            }

            foreach (var plot in targets)
            plot.Plant(cropId);
            GameBootstrap.Instance.AddDiamonds(-TargetCount);
            Close();
        }
        else
        {
            NotificationController.Instance.ShowCanvasNotification("Không đủ kim cương", "Not enough diamonds!");
            //khong du kim cuong
            Close();
        }
    }
    public void Close()
    {
        for (int i = 0; i < TargetCount; i++)
        {
            if (TryGetTarget(i, out var plot)) plot.HideOutOfSeedIndicator();
        }
        PlayerPrefs.DeleteKey(SaveKeys.OutOfSeedTargetCount);
        PlayerPrefs.DeleteKey(LegacySaveKeys.OutOfSeedTargetCount);
        gameObject.SetActive(false);
        MainCamera.Instance.unLockCam();
    }
}
