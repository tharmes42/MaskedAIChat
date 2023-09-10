namespace MaskedAIChat.Contracts.Services;

public interface ILocalSettingsService
{
    Task<T?> ReadSettingAsync<T>(string key);

    Task SaveSettingAsync<T>(string key, T value);

    // settings key names, not defined as const because it is used in page xaml
    // important: these keys must be named like the ui elements in SettingsPage.xaml
    public string SettingsKey_ApiKey => "Settings_ApiKey";
    public string SettingsKey_ChatSystemPrompt => "Settings_ChatSystemPrompt";
    public string SettingsKey_DeeplApiKey => "Settings_DeeplApiKey";

}
