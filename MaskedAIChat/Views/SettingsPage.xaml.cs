using MaskedAIChat.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace MaskedAIChat.Views;

// TODO: Set the URL for your privacy policy by updating SettingsPage_PrivacyTermsLink.NavigateUri in Resources.resw.
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
        PasswordBox passwordBox = this.FindName(ViewModel.SettingsKey_ApiKey) as PasswordBox;
        if (revealModeCheckBox.IsChecked == true)
        {
            passwordBox.PasswordRevealMode = PasswordRevealMode.Visible;
        }
        else
        {
            passwordBox.PasswordRevealMode = PasswordRevealMode.Hidden;
        }
    }


}
