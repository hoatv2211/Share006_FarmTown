using System;
using System.Collections.Generic;

namespace FarmTown.Save
{
    public sealed class ProductionState
    {
        private readonly string stableId;
        private readonly string legacyName;
        private readonly int maximumSlots;
        private readonly List<int> queuedProducts = new List<int>();
        private readonly Dictionary<int, int> completedProducts = new Dictionary<int, int>();

        private ProductionState(string stableId, string legacyName, int maximumSlots)
        {
            if (string.IsNullOrWhiteSpace(stableId)) throw new ArgumentException("Stable ID is required.", nameof(stableId));
            if (string.IsNullOrWhiteSpace(legacyName)) throw new ArgumentException("Legacy name is required.", nameof(legacyName));
            if (maximumSlots < 1) throw new ArgumentOutOfRangeException(nameof(maximumSlots));
            this.stableId = stableId;
            this.legacyName = legacyName;
            this.maximumSlots = maximumSlots;
        }

        public int QueueCapacity { get; private set; }
        public int QueueCount => queuedProducts.Count;
        public int RemainingTime { get; private set; }
        public int LastTimestamp { get; private set; }
        public bool CanEnqueue => queuedProducts.Count < QueueCapacity && queuedProducts.Count < maximumSlots;

        public static ProductionState Load(string stableId, string legacyName, int maximumSlots = 5)
        {
            var state = new ProductionState(stableId, legacyName, maximumSlots);
            state.QueueCapacity = ProductionPersistence.GetQueueCapacity(stableId, legacyName);
            var queueCount = Math.Min(ProductionPersistence.GetQueueCount(stableId, legacyName), maximumSlots);
            for (var slotIndex = 1; slotIndex <= queueCount; slotIndex++)
                if (ProductionPersistence.HasQueuedProduct(stableId, slotIndex, legacyName))
                    state.queuedProducts.Add(ProductionPersistence.GetQueuedProduct(stableId, slotIndex, legacyName));
            for (var slotIndex = 1; slotIndex <= maximumSlots; slotIndex++)
                if (ProductionPersistence.HasCompletedProduct(stableId, slotIndex, legacyName))
                    state.completedProducts[slotIndex] = ProductionPersistence.GetCompletedProduct(stableId, slotIndex, legacyName);
            state.RemainingTime = ProductionPersistence.GetRemainingTime(stableId, legacyName);
            state.LastTimestamp = ProductionPersistence.GetLastTimestamp(stableId, legacyName);
            return state;
        }

        public int GetQueuedProduct(int slotIndex)
        {
            ValidateSlot(slotIndex);
            if (slotIndex > queuedProducts.Count) throw new ArgumentOutOfRangeException(nameof(slotIndex));
            return queuedProducts[slotIndex - 1];
        }

        public bool TryEnqueue(int productId)
        {
            if (!CanEnqueue) return false;
            queuedProducts.Add(productId);
            PersistQueue();
            return true;
        }

        public void SetQueuedProduct(int slotIndex, int productId)
        {
            ValidateSlot(slotIndex);
            if (slotIndex > queuedProducts.Count) throw new ArgumentOutOfRangeException(nameof(slotIndex));
            queuedProducts[slotIndex - 1] = productId;
            PersistQueue();
        }

        public int RemoveQueuedProduct(int slotIndex)
        {
            ValidateSlot(slotIndex);
            if (slotIndex > queuedProducts.Count) throw new ArgumentOutOfRangeException(nameof(slotIndex));
            var productId = queuedProducts[slotIndex - 1];
            queuedProducts.RemoveAt(slotIndex - 1);
            PersistQueue();
            return productId;
        }

        public void ClearQueue()
        {
            queuedProducts.Clear();
            PersistQueue();
        }

        public bool HasCompletedProduct(int slotIndex)
        {
            ValidateSlot(slotIndex);
            return completedProducts.ContainsKey(slotIndex);
        }

        public bool TryGetFreeCompletedSlot(out int slotIndex)
        {
            for (slotIndex = 1; slotIndex <= maximumSlots; slotIndex++)
                if (!completedProducts.ContainsKey(slotIndex)) return true;
            slotIndex = -1;
            return false;
        }

        public int GetCompletedProduct(int slotIndex)
        {
            ValidateSlot(slotIndex);
            return completedProducts[slotIndex];
        }

        public void SetCompletedProduct(int slotIndex, int productId)
        {
            ValidateSlot(slotIndex);
            completedProducts[slotIndex] = productId;
            ProductionPersistence.SetCompletedProduct(stableId, slotIndex, legacyName, productId);
        }

        public void DeleteCompletedProduct(int slotIndex)
        {
            ValidateSlot(slotIndex);
            completedProducts.Remove(slotIndex);
            ProductionPersistence.DeleteCompletedProduct(stableId, slotIndex, legacyName);
        }

        public void SetQueueCapacity(int capacity)
        {
            if (capacity < 0 || capacity > maximumSlots) throw new ArgumentOutOfRangeException(nameof(capacity));
            QueueCapacity = capacity;
            ProductionPersistence.SetQueueCapacity(stableId, legacyName, capacity);
        }

        public void SetTimer(int remainingTime, int lastTimestamp)
        {
            RemainingTime = remainingTime;
            LastTimestamp = lastTimestamp;
            ProductionPersistence.SetRemainingTime(stableId, legacyName, remainingTime);
            ProductionPersistence.SetLastTimestamp(stableId, legacyName, lastTimestamp);
        }

        private void PersistQueue()
        {
            ProductionPersistence.SetQueueCount(stableId, legacyName, queuedProducts.Count);
            for (var slotIndex = 1; slotIndex <= maximumSlots; slotIndex++)
            {
                if (slotIndex <= queuedProducts.Count)
                    ProductionPersistence.SetQueuedProduct(stableId, slotIndex, legacyName, queuedProducts[slotIndex - 1]);
                else
                    ProductionPersistence.DeleteQueuedProduct(stableId, slotIndex, legacyName);
            }
        }

        private void ValidateSlot(int slotIndex)
        {
            if (slotIndex < 1 || slotIndex > maximumSlots) throw new ArgumentOutOfRangeException(nameof(slotIndex));
        }
    }
}
