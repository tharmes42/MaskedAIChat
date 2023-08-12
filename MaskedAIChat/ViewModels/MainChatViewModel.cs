using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Azure.AI.OpenAI;
using CommunityToolkit.Mvvm.ComponentModel;
using MaskedAIChat.Contracts.ViewModels;
using MaskedAIChat.Core.Contracts.Services;
using MaskedAIChat.Core.Models;
using MaskedAIChat.Core.Services;
using MaskedAIChat.Helpers;
using MaskedAIChat.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;

namespace MaskedAIChat.ViewModels;

public partial class MainChatViewModel : ObservableRecipient, INavigationAware, INotifyPropertyChanged
{
    private IChatDataService _chatDataService;
    private IMaskDataService _maskDataService;
    private IGptService _gptService;
    int messageNumber;

    public string ChatText
    {
        get => _chatDataService.ChatText;
        set
        {
            _chatDataService.ChatText = value;
            RaisePropertyChanged();
        }
    }

    private string _maskedChatText;
    public string MaskedChatText
    {
        get => _maskedChatText;

        set
        {
            _maskedChatText = value;
            RaisePropertyChanged();
        }
    }

    private string _chatCompletionText;
    public string ChatCompletionText
    {

        get => _chatCompletionText;

        set
        {

            _chatCompletionText = value;
            RaisePropertyChanged();
        }
    }

    //https://stackoverflow.com/questions/73521265/binding-and-changing-a-listview-dynamically-using-mvvm

    public ObservableCollection<MessageItem> MessageItems
    {
        //todo: attach to chatdataservice.Messages
        get
        {
            ObservableCollection<MessageItem> items = new ObservableCollection<MessageItem>();
            foreach (var message in _chatDataService.Messages)
            {
                var alignment = message.MsgChatRole.Equals("assistant") ? HorizontalAlignment.Left : HorizontalAlignment.Right;
                items.Add(new MessageItem(message.MsgText, message.MsgDateTime, alignment));
            }
            return items;
        }
        //DateTime.Now
        //set => SetProperty(ref _messageItems, value);
        //set
        //{
        //    //_messageItems = value;
        //    _chatDataService.Messages.Add(new Message(ChatText, DateTime.Now, "user"));
        //    RaisePropertyChanged();
        //}


    }


    public MainChatViewModel(IChatDataService chatDataService, IMaskDataService maskDataService, IGptService gptService)
    {
        _chatDataService = chatDataService;
        _maskDataService = maskDataService;
        _gptService = gptService;
        _chatDataService.PropertyChanged += OnModelPropertyChanged;
        if (_chatDataService.Messages.Count == 0)
        {
            chatDataService.Messages.Add(new Message("You are a highly skilled helpful assistant.", DateTime.Now, "system"));
        }
        //todo: remove this
        if (ChatText.Equals(""))
        {
            string intialMessage =
@"Viele Grüße
Tobias

Von: Substack Reads<read@substack.com>
            Gesendet: Samstag, 24.Juni 2023 15:02
An: tge@example.com
            Betreff: Substack Reads: Our house obsession, the K-pop power shift, and Ukraine’s mobile bakery

Image
Your weekend digest of the best writing from across Substack is here!";
            ChatText = intialMessage;
        }

    }


    public string maskedChatText;

    public IEnumerable<Mask> GetMasks()
    {
        return _maskDataService.GetMasks();
    }

    public void OnNavigatedTo(object parameter)
    {
        // Run code when the app navigates to this page
        //MaskedChatText = "hello world";

    }

    public void OnNavigatedFrom()
    {
        // Run code when the app navigates away from this page

    }

    //update the maskedchattext on chattext change
    private void ChatChanged(object sender, RoutedEventArgs e)
    {
        //var source = sender as RichEditBox;
        //if (sender == null)
        //    return;

        //MainChat_ChatText.Document.GetText(Microsoft.UI.Text.TextGetOptions.None, out var value);
        ////save to model, so we don't loose data on navigation,
        //chatDataService.SetChatText(value);

        ////TODO: remove maskDataService from code behind
        ////find sensitive information and build mask lisk
        //maskDataService.BuildMasks(value);
        ////mask the whole text based on previously constructed mask list 
        //value = maskDataService.MaskText(value);
        ////update destination chat with masked text
        //REBDestination.Document.SetText(Microsoft.UI.Text.TextSetOptions.None, value);

    }

    //direct update of settings via XAML checkbox
    //settingskey must be passed as Checkbox CommandParameter
    //TODO: copied, remove this after implementation of chatChanged
    private async void SettingChanged_CheckedAsync(object sender, RoutedEventArgs e)
    {
        var settingsKey = (sender as CheckBox)?.CommandParameter;
        var settingsVal = (sender as CheckBox)?.IsChecked;

        if (settingsKey != null && settingsVal != null)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync(settingsKey.ToString(), settingsVal.ToString());
        }
    }

    // react to change notfication of Model 
    private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            // if we are notfied of a chattext change, rebuild masks and mask the text (this will also trigger a property change event)
            case nameof(ChatDataService.ChatText):
                _maskDataService.BuildMasks(ChatText);
                MaskedChatText = _maskDataService.MaskText(ChatText);
                break;

            // if we are notified of a chat history change
            case nameof(ChatDataService.Messages):
                //_chatDataService.Messages.Add(new Message(ChatText, DateTime.Now, "user"));
                RaisePropertyChanged(nameof(MessageItems));
                break;

        }
    }

    private async void AskGptService()
    {
        var messages = new List<ChatMessage>();
        foreach (var message in _chatDataService.Messages)
        {
            //todo: check if role is system
            messages.Add(new ChatMessage(message.MsgChatRole.Equals("assistant") ? ChatRole.Assistant : ChatRole.User, message.MsgText));
        }
        var chatOptions = new ChatCompletionsOptions(messages);
        var gptResponse = await _gptService.GenerateChatCompletionAsync(chatOptions);
        _chatDataService.Messages.Add(new Message(gptResponse, DateTime.Now, "assistant"));
    }


    public event PropertyChangedEventHandler PropertyChanged;

    // one of our viewmodels changed a property, notify the view
    void RaisePropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void SendChat()
    {
        //give out the masked chat text to console
        Debug.WriteLine(MaskedChatText);
        _chatDataService.Messages.Add(new Message(MaskedChatText, DateTime.Now, "user"));
        //ask the gpt service for a response to the new message
        AskGptService();


    }

    public void AddItemToEnd()
    {
        //InvertedListView.Items.Add(
        //    new Message("Message " + ++messageNumber, DateTime.Now, HorizontalAlignment.Right)
        //    );
        MessageItems.Add(
            new MessageItem("Message " + ++messageNumber, DateTime.Now, HorizontalAlignment.Right)
            );
    }

    public void MessageReceived(object sender, RoutedEventArgs e)
    {
        //InvertedListView.Items.Add(
        //    new Message("Message " + ++messageNumber, DateTime.Now, HorizontalAlignment.Left)
        //    );
        MessageItems.Add(
            new MessageItem("Message " + ++messageNumber, DateTime.Now, HorizontalAlignment.Left)
            );
    }
}
