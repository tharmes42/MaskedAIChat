using System.ComponentModel;
using System.Runtime.CompilerServices;
using MaskedAIChat.Helpers;
using MaskedAIChat.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows.Storage;

namespace MaskedAIChat.Views;

// TODO: Set the URL for your privacy policy by updating SettingsPage_PrivacyTermsLink.NavigateUri in Resources.resw.
public sealed partial class SettingsPage : Page, INotifyPropertyChanged
{
    //setting for the API key
    private string _apiKey = "";

    public string ApiKey
    {
        get
        {
            return _apiKey;
        }
        set
        {
            Set(ref _apiKey, value);
        }
    }

    private async Task InitializeAsync()
    {

        ApiKey = await ApplicationData.Current.LocalSettings.ReadAsync<string>("ApiKey");

        await Task.CompletedTask;
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        await InitializeAsync();
    }

    public SettingsViewModel ViewModel
    {
        get;
    }

    public SettingsPage()
    {
        //TODO: wahrscheinlich muss man doch nochmal die SettingsViewModel.cs anpassen und das ganze Settings-Speichern dort abhandeln, 
        // weil diese Klasse hier bei jedem View-Change neu instanziert wird und somit die Settings nicht gespeichert werden
        ViewModel = App.GetService<SettingsViewModel>();
        InitializeComponent();
    }





    private void RevealModeCheckbox_Changed(object sender, RoutedEventArgs e)
    {
        if (revealModeCheckBox.IsChecked == true)
        {
            passworBoxApiKey.PasswordRevealMode = PasswordRevealMode.Visible;
        }
        else
        {
            passworBoxApiKey.PasswordRevealMode = PasswordRevealMode.Hidden;
        }
    }

    //direct update of settings via XAML checkbox
    //settingskey must be passed as Checkbox CommandParameter
    private async void SettingChanged_CheckedAsync(object sender, RoutedEventArgs e)
    {
        var settingsKey = "";
        var settingsVal = "";
        if (sender.GetType() == typeof(CheckBox))
        {
            settingsKey = (sender as CheckBox)?.CommandParameter.ToString();
            settingsVal = (sender as CheckBox)?.IsChecked.ToString();

        }
        else if (sender.GetType() == typeof(PasswordBox))
        {
            settingsKey = (sender as PasswordBox)?.Name.ToString();
            settingsVal = (sender as PasswordBox)?.Password.ToString();
        }

        if (settingsKey != null && settingsVal != null)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync(settingsKey.ToString(), settingsVal.ToString());
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    //used to update the visual elements on value change
    private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(storage, value))
        {
            return;
        }

        storage = value;
        OnPropertyChanged(propertyName);
    }

    private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
