using System;
using System.Collections;
using NUnit.Framework;

namespace FarmTown.Tests.EditMode
{
    public class LegacySaveFixtureTests
    {
        private static IEnumerable MigrationDomainCases
        {
            get
            {
                yield return Case("wallet", "gold", 2450, typeof(int));
                yield return Case("wallet", "diamond", 17, typeof(int));
                yield return Case("wallet", "level", 8, typeof(int));
                yield return Case("wallet", "kinhnghiem", 12, typeof(int));
                yield return Case("wallet", "kinhnghiemmax", 40, typeof(int));
                yield return Case("crop", "slnongsan0", 14, typeof(int));
                yield return Case("product", "slvatpham12", 3, typeof(int));
                yield return Case("plot", "soodat", 8, typeof(int));
                yield return Case("plot", "xodat0", -1.56f, typeof(float));
                yield return Case("plot", "yodat0", 0.8f, typeof(float));
                yield return Case("factory", "slnhamay0", 1, typeof(int));
                yield return Case("factory", "xnhamay00", 2.48f, typeof(float));
                yield return Case("factory", "ynhamay00", -0.64f, typeof(float));
                yield return Case("factory", "dangsanxuatnhamay00", 1, typeof(int));
                yield return Case("factory", "idsanxuat1nhamay00", 12, typeof(int));
                yield return Case("tutorial", "huongdan", 18, typeof(int));
            }
        }

        [TestCaseSource(nameof(MigrationDomainCases))]
        public void ExpandedFarmContainsAllMigrationDomains(
            string domain,
            string key,
            object expectedValue,
            Type expectedType)
        {
            var actual = LegacySaveFixtures.ExpandedFarm[key];

            Assert.That(actual, Is.TypeOf(expectedType), $"{domain} type");
            Assert.That(actual, Is.EqualTo(expectedValue), $"{domain} value");
        }

        private static TestCaseData Case(string domain, string key, object value, Type type)
        {
            return new TestCaseData(domain, key, value, type)
                .SetName($"ExpandedFarm_{domain}_{key}");
        }
    }
}
