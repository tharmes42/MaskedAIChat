using System.Reflection;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MaskedAIChat.Contracts.Services;
using MaskedAIChat.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel;

namespace MaskedAIChat.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    // settings key names, not defined as const because it is used in page xaml
    public string SettingsKey_ApiKey => _localSettingsService.SettingsKey_ApiKey;

    private readonly ILocalSettingsService _localSettingsService;
    private readonly IThemeSelectorService _themeSelectorService;

    //reminder: ObservableProperty automatically creates "Var" for "_var"
    [ObservableProperty]
    private ElementTheme _elementTheme;

    [ObservableProperty]
    private string _versionDescription;

    [ObservableProperty]
    private string _apiKey;

    public ICommand SwitchThemeCommand
    {
        get;
    }

    public SettingsViewModel(IThemeSelectorService themeSelectorService, ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
        _themeSelectorService = themeSelectorService;
        _elementTheme = _themeSelectorService.Theme;
        _versionDescription = GetVersionDescription();
        _apiKey = ""; // ApiKey = ""; would work as well, is later initialized by InitializeModelAsync()


        //TODO: check if this also can by handled by SettingChanged_SaveAsync
        SwitchThemeCommand = new RelayCommand<ElementTheme>(
            async (param) =>
            {
                if (ElementTheme != param)
                {
                    ElementTheme = param;
                    await _themeSelectorService.SetThemeAsync(param);
                }
            });
    }


    // read all settings from local settings service
    public async Task InitializeModelAsync()
    {
        var cacheApiKey = await _localSettingsService.ReadSettingAsync<string>(SettingsKey_ApiKey);
        if (!String.IsNullOrEmpty(cacheApiKey))
        {
            ApiKey = cacheApiKey;
        }

        await Task.CompletedTask;
    }

    private static string GetVersionDescription()
    {
        Version version;

        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;

            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        return $"{"AppDisplayName".GetLocalized()} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }

    //settingskey must be passed as Checkbox CommandParameter
    public async void SettingChanged_SaveAsync(object sender, RoutedEventArgs e)
    {
        //check if sender is type of passwordbox
        if (sender is PasswordBox)
        {
            var settingsKey = (sender as PasswordBox)?.Name;
            var settingsVal = (sender as PasswordBox)?.Password;

            if (settingsKey != null && settingsVal != null)
            {
                //await ApplicationData.Current.LocalSettings.SaveAsync(settingsKey.ToString(), settingsVal.ToString());
                await _localSettingsService.SaveSettingAsync(settingsKey, settingsVal);
            }
        }


    }

}
