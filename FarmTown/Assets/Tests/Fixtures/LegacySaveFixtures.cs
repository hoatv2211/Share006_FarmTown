using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FarmTown.Tests
{
    public static class LegacySaveFixtures
    {
        public static readonly ReadOnlyDictionary<string, object> ExpandedFarm =
            new ReadOnlyDictionary<string, object>(
                new Dictionary<string, object>
                {
                    ["gold"] = 2450,
                    ["diamond"] = 17,
                    ["level"] = 8,
                    ["kinhnghiem"] = 12,
                    ["kinhnghiemmax"] = 40,
                    ["slnongsan0"] = 14,
                    ["slvatpham12"] = 3,
                    ["huongdan"] = 18,
                    ["soodat"] = 8,
                    ["xodat0"] = -1.56f,
                    ["yodat0"] = 0.8f,
                    ["slnhamay0"] = 1,
                    ["xnhamay00"] = 2.48f,
                    ["ynhamay00"] = -0.64f,
                    ["dangsanxuatnhamay00"] = 1,
                    ["idsanxuat1nhamay00"] = 12,
                });
    }
}
