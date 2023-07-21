namespace MaskedAIChat.Contracts.Services;

public interface ILocalSettingsService
{
    Task<T?> ReadSettingAsync<T>(string key);

    Task SaveSettingAsync<T>(string key, T value);

    // settings key names, not defined as const because it is used in page xaml
    public string SettingsKey_ApiKey => "Settings_ApiKey";

}
