using System;
using System.Collections.Generic;
using UnityEngine;

namespace FarmTown.Save
{
    public sealed class StableInstanceId : MonoBehaviour
    {
        private static readonly Dictionary<string, StableInstanceId> Registry = new(StringComparer.Ordinal);

        [SerializeField] private string stableId = string.Empty;

        public string StableId => string.IsNullOrWhiteSpace(stableId) ? string.Empty : stableId;

        public bool TryAssign(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            value = value.Trim();
            if (StableId.Length > 0 && !string.Equals(StableId, value, StringComparison.Ordinal))
                return false;
            if (Registry.TryGetValue(value, out var existing) && existing != null && existing != this)
                return false;

            stableId = value;
            if (isActiveAndEnabled)
                Registry[value] = this;
            return true;
        }

        public bool TryGetStableId(out string value)
        {
            value = StableId;
            return value.Length > 0;
        }

        public static bool TryFind(string value, out StableInstanceId instance)
        {
            instance = null;
            if (string.IsNullOrWhiteSpace(value))
                return false;
            if (!Registry.TryGetValue(value, out var found) || found == null)
                return false;
            instance = found;
            return true;
        }

        private void OnEnable()
        {
            if (StableId.Length == 0)
                return;
            if (Registry.TryGetValue(StableId, out var existing) && existing != null && existing != this)
            {
                Debug.LogError($"Duplicate stable instance ID '{StableId}' on {name}.", this);
                enabled = false;
                return;
            }
            Registry[StableId] = this;
        }

        private void OnDisable() => Unregister();
        private void OnDestroy() => Unregister();

        private void Unregister()
        {
            if (StableId.Length > 0 && Registry.TryGetValue(StableId, out var existing) && existing == this)
                Registry.Remove(StableId);
        }
    }
}
