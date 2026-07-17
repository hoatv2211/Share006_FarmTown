namespace FarmTown.Save
{
    public interface ISaveStore
    {
        bool HasKey(string key);
        int GetInt(string key, int defaultValue = 0);
        float GetFloat(string key, float defaultValue = 0f);
        string GetString(string key, string defaultValue = "");
        void SetInt(string key, int value);
        void SetFloat(string key, float value);
        void SetString(string key, string value);
        void Save();
    }
}
