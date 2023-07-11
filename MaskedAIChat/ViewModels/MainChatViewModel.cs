using System.Reflection.Metadata;
using CommunityToolkit.Mvvm.ComponentModel;
using MaskedAIChat.Contracts.ViewModels;
using MaskedAIChat.Core.Contracts.Services;
using MaskedAIChat.Views;

namespace MaskedAIChat.ViewModels;

public partial class MainChatViewModel : ObservableRecipient, INavigationAware
{
    private readonly IChatDataService _chatDataService;

    public MainChatViewModel(IChatDataService chatDataService)
    {
        _chatDataService = chatDataService;
    }

    public string chatText;

    public void OnNavigatedTo(object parameter)
    {
        // Run code when the app navigates to this page
        chatText = _chatDataService.GetChatText();  

    }

    public void OnNavigatedFrom()
    {
        // Run code when the app navigates away from this page

    }
}
