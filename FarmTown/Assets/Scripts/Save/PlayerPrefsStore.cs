using UnityEngine;

namespace FarmTown.Save
{
    public sealed class PlayerPrefsStore : ISaveStore
    {
        public bool HasKey(string key) => PlayerPrefs.HasKey(key);
        public int GetInt(string key, int defaultValue = 0) => PlayerPrefs.GetInt(key, defaultValue);
        public float GetFloat(string key, float defaultValue = 0f) => PlayerPrefs.GetFloat(key, defaultValue);
        public string GetString(string key, string defaultValue = "") => PlayerPrefs.GetString(key, defaultValue);
        public void SetInt(string key, int value) => PlayerPrefs.SetInt(key, value);
        public void SetFloat(string key, float value) => PlayerPrefs.SetFloat(key, value);
        public void SetString(string key, string value) => PlayerPrefs.SetString(key, value);
        public void Save() => PlayerPrefs.Save();
    }
}
