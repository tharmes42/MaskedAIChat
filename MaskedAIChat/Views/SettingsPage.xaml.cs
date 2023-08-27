using MaskedAIChat.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace MaskedAIChat.Views;


public sealed partial class SettingsPage : Page
{


    private async Task InitializeAsync()
    {

        //load viewmodel settings async 
        await ViewModel.InitializeModelAsync();
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
        ViewModel = App.GetService<SettingsViewModel>();
        InitializeComponent();
    }


    private void RevealModeCheckbox_Changed(object sender, RoutedEventArgs e)
    {
        //PasswordBox passwordBox1 = this.FindName(ViewModel.SettingsKey_ApiKey) as PasswordBox;


        if (RevealModeCheckBox.IsChecked == true)
        {
            Settings_ApiKey.PasswordRevealMode = PasswordRevealMode.Visible;
            Settings_DeeplApiKey.PasswordRevealMode = PasswordRevealMode.Visible;

        }
        else
        {
            Settings_ApiKey.PasswordRevealMode = PasswordRevealMode.Hidden;
            Settings_DeeplApiKey.PasswordRevealMode = PasswordRevealMode.Hidden;
        }
    }


}
