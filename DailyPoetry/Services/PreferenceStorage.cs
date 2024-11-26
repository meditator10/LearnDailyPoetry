using DailyPoetry.Library.Services;

namespace DailyPoetry.Services;

public class PreferenceStorage : IPreferenceStorage
{
    public void Set(string key, int value) => Preferences.Set(key, value);

    public int Get(string key, int defaultValue)
    {
        var value = Preferences.Get(key, defaultValue);
        return value == default ? defaultValue : value;
    }

    public void Set(string key, string value) => Preferences.Set(key, value);

    public string Get(string key, string defaultValue) =>
        Preferences.Get(key, defaultValue) ?? defaultValue;

    public void Set(string key, DateTime value) => Preferences.Set(key, value);

    public DateTime Get(string key, DateTime defaultValue)
    {
        var value = Preferences.Get(key, defaultValue);
        return value == default ? defaultValue : value;
    }
}
