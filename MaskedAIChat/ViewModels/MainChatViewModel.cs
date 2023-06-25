using System.Reflection.Metadata;
using CommunityToolkit.Mvvm.ComponentModel;
using MaskedAIChat.Contracts.ViewModels;
using MaskedAIChat.Views;

namespace MaskedAIChat.ViewModels;

public partial class MainChatViewModel : ObservableRecipient, INavigationAware
{
    public string chatText {
        get; set;
    }

    public MainChatViewModel()
    {
        chatText = string.Empty;
    }

    public void OnNavigatedTo(object parameter)
    {
        // Run code when the app navigates to this page
 

    }

    public void OnNavigatedFrom()
    {
        // Run code when the app navigates away from this page

    }
}
