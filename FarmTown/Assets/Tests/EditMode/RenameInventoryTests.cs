using System.Linq;
using FarmTown.Editor.Migration;
using NUnit.Framework;

namespace FarmTown.Tests.EditMode
{
    public sealed class RenameInventoryTests
    {
        [TestCase("Assets/GoogleMobileAds/Editor/Ads.cs")]
        [TestCase("Assets/Spine/Runtime/spine-csharp.dll")]
        [TestCase("Assets/Plugins/Android/library.aar")]
        [TestCase("Assets/Parse/Parse.dll")]
        [TestCase("Assets/Mini_Game/Minigame/Plugins/Lean/Common.cs")]
        [TestCase("Assets/Mini_Game/Minigame/Epic Toon FX/Effect.prefab")]
        [TestCase("Assets/Mini_Game/Minigame/FX Quest/Effect.prefab")]
        [TestCase("Assets/Mini_Game/Minigame/Minigame_v2.0/Epic Toon FX/Effect.prefab")]
        [TestCase("Assets/Mini_Game/Minigame/SuperLibrary/SuperLibrary.dll")]
        [TestCase("Assets/MobileDependencyResolver/Editor/Resolver.dll")]
        public void VendorPathsAreExcluded(string path)
        {
            Assert.That(FirstPartyAssetPolicy.IsFirstParty(path), Is.False);
        }

        [TestCase("Assets/Scripts/Manager/BatDauGame.cs")]
        [TestCase("Assets/Prefabs/odat/Odat.prefab")]
        [TestCase("Assets/Scenes/map.unity")]
        public void FirstPartyPathsAreIncluded(string path)
        {
            Assert.That(FirstPartyAssetPolicy.IsFirstParty(path), Is.True);
        }

        [Test]
        public void EnglishPathReplacesCanonicalTechnicalNames()
        {
            var path = RenameInventoryBuilder.BuildEnglishPath(
                "Assets/Scripts/Odat/HatGiongKeo.cs",
                out var unresolvedTokens);

            Assert.That(path, Is.EqualTo("Assets/Scripts/CropPlot/DraggableSeed.cs"));
            Assert.That(unresolvedTokens, Is.Empty);
        }

        [Test]
        public void VendorPrefixListHasNoDuplicates()
        {
            Assert.That(
                FirstPartyAssetPolicy.ExcludedVendorPrefixes.Distinct().Count(),
                Is.EqualTo(FirstPartyAssetPolicy.ExcludedVendorPrefixes.Count));
        }
    }
}
