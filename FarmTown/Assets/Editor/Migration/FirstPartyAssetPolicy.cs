using System;
using System.Collections.Generic;

namespace FarmTown.Editor.Migration
{
    public static class FirstPartyAssetPolicy
    {
        private static readonly string[] VendorPrefixes =
        {
            "Assets/GoogleMobileAds/",
            "Assets/Spine/",
            "Assets/Plugins/",
            "Assets/Parse/",
            "Assets/Mini_Game/",
            "Assets/Mini_Game/Minigame/Plugins/",
            "Assets/Mini_Game/Minigame/Epic Toon FX/",
            "Assets/Mini_Game/Minigame/FX Quest/",
            "Assets/Mini_Game/Minigame/Minigame_v2.0/Epic Toon FX/",
            "Assets/Mini_Game/Minigame/SuperLibrary/",
            "Assets/MobileDependencyResolver/",
        };

        public static IReadOnlyList<string> ExcludedVendorPrefixes => VendorPrefixes;

        public static bool IsFirstParty(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            var normalizedPath = path.Replace('\\', '/');
            if (!normalizedPath.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
                return false;

            foreach (var prefix in VendorPrefixes)
            {
                if (normalizedPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            return true;
        }
    }
}
