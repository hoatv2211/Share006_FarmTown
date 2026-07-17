using FarmTown.Save;
using NUnit.Framework;
using UnityEngine;

namespace FarmTown.Tests.EditMode
{
    public sealed class ProductionStateTests
    {
        private const string StableId = "production-building-0-0";
        private const string LegacyName = "nhamay00";

        [SetUp]
        public void SetUp() => PlayerPrefs.DeleteAll();

        [TearDown]
        public void TearDown() => PlayerPrefs.DeleteAll();

        [Test]
        public void LoadReadsQueueCompletedAndTimerOnce()
        {
            ProductionPersistence.SetQueueCapacity(StableId, LegacyName, 3);
            ProductionPersistence.SetQueueCount(StableId, LegacyName, 2);
            ProductionPersistence.SetQueuedProduct(StableId, 1, LegacyName, 4);
            ProductionPersistence.SetQueuedProduct(StableId, 2, LegacyName, 7);
            ProductionPersistence.SetCompletedProduct(StableId, 3, LegacyName, 9);
            ProductionPersistence.SetRemainingTime(StableId, LegacyName, 30);
            ProductionPersistence.SetLastTimestamp(StableId, LegacyName, 100);

            var state = ProductionState.Load(StableId, LegacyName);

            Assert.That(state.QueueCapacity, Is.EqualTo(3));
            Assert.That(state.QueueCount, Is.EqualTo(2));
            Assert.That(state.GetQueuedProduct(1), Is.EqualTo(4));
            Assert.That(state.GetQueuedProduct(2), Is.EqualTo(7));
            Assert.That(state.GetCompletedProduct(3), Is.EqualTo(9));
            Assert.That(state.RemainingTime, Is.EqualTo(30));
            Assert.That(state.LastTimestamp, Is.EqualTo(100));
        }

        [Test]
        public void QueueMutationsCompactAndPersistAllSlots()
        {
            ProductionPersistence.SetQueueCapacity(StableId, LegacyName, 3);
            var state = ProductionState.Load(StableId, LegacyName);

            Assert.That(state.TryEnqueue(4), Is.True);
            Assert.That(state.TryEnqueue(7), Is.True);
            Assert.That(state.TryEnqueue(9), Is.True);
            Assert.That(state.TryEnqueue(11), Is.False);
            Assert.That(state.RemoveQueuedProduct(1), Is.EqualTo(4));

            Assert.That(state.QueueCount, Is.EqualTo(2));
            Assert.That(state.GetQueuedProduct(1), Is.EqualTo(7));
            Assert.That(ProductionPersistence.GetQueuedProduct(StableId, 1, LegacyName), Is.EqualTo(7));
            Assert.That(ProductionPersistence.GetQueuedProduct(StableId, 2, LegacyName), Is.EqualTo(9));
            Assert.That(ProductionPersistence.HasQueuedProduct(StableId, 3, LegacyName), Is.False);
        }

        [Test]
        public void CompletedAndTimerMutationsPersistImmediately()
        {
            var state = ProductionState.Load(StableId, LegacyName);

            state.SetCompletedProduct(2, 12);
            state.SetTimer(45, 900);

            Assert.That(state.HasCompletedProduct(2), Is.True);
            Assert.That(ProductionPersistence.GetCompletedProduct(StableId, 2, LegacyName), Is.EqualTo(12));
            Assert.That(ProductionPersistence.GetRemainingTime(StableId, LegacyName), Is.EqualTo(45));
            Assert.That(ProductionPersistence.GetLastTimestamp(StableId, LegacyName), Is.EqualTo(900));

            state.DeleteCompletedProduct(2);
            Assert.That(state.HasCompletedProduct(2), Is.False);
        }

        [Test]
        public void LoadDoesNotRewriteQueueUntilMutation()
        {
            ProductionPersistence.SetQueueCapacity(StableId, LegacyName, 3);
            ProductionPersistence.SetQueueCount(StableId, LegacyName, 2);
            ProductionPersistence.SetQueuedProduct(StableId, 2, LegacyName, 7);

            var state = ProductionState.Load(StableId, LegacyName);

            Assert.That(state.QueueCount, Is.EqualTo(1));
            Assert.That(ProductionPersistence.GetQueueCount(StableId, LegacyName), Is.EqualTo(2));
            Assert.That(ProductionPersistence.HasQueuedProduct(StableId, 2, LegacyName), Is.True);
        }

        [Test]
        public void CompletedSlotsNeverOverwriteExistingProducts()
        {
            var state = ProductionState.Load(StableId, LegacyName);
            state.SetCompletedProduct(1, 4);
            state.SetCompletedProduct(2, 7);

            Assert.That(state.TryGetFreeCompletedSlot(out var freeSlot), Is.True);
            Assert.That(freeSlot, Is.EqualTo(3));
        }
    }
}
