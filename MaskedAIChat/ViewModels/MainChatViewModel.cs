using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Azure.AI.OpenAI;
using CommunityToolkit.Mvvm.ComponentModel;
using MaskedAIChat.Contracts.Services;
using MaskedAIChat.Contracts.ViewModels;
using MaskedAIChat.Core.Contracts.Services;
using MaskedAIChat.Core.Models;
using MaskedAIChat.Core.Services;
using MaskedAIChat.Helpers;
using MaskedAIChat.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace MaskedAIChat.ViewModels;

public partial class MainChatViewModel : ObservableRecipient, INotifyPropertyChanged
{
    private IChatDataService _chatDataService;
    private IMaskDataService _maskDataService;
    private IGptService _gptService;
    private readonly ILocalSettingsService _localSettingsService;
    int messageNumber;

    private string _apiKey;

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

    //https://stackoverflow.com/questions/73521265/binding-and-changing-a-listview-dynamically-using-mvvm

    public ObservableCollection<MessageItem> MessageItems
    {
        get
        {
            ObservableCollection<MessageItem> items = new ObservableCollection<MessageItem>();
            foreach (var message in _chatDataService.Messages)
            {
                var alignment = message.MsgChatRole.Equals("assistant") ? HorizontalAlignment.Left : HorizontalAlignment.Right;
                items.Add(new MessageItem(message.MsgText, message.MsgDateTime, alignment, message.MsgChatRole));
            }
            return items;
        }
    }



    public MainChatViewModel(IChatDataService chatDataService, IMaskDataService maskDataService, IGptService gptService, ILocalSettingsService localSettingsService)
    {
        _chatDataService = chatDataService;
        _maskDataService = maskDataService;
        _gptService = gptService;
        _localSettingsService = localSettingsService;
        
        
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

    // read all settings from local settings service
    public async Task InitializeModelAsync()
    {
        var cacheApiKey = await _localSettingsService.ReadSettingAsync<string>(_localSettingsService.SettingsKey_ApiKey);
        if (!String.IsNullOrEmpty(cacheApiKey))
        {
            _apiKey = cacheApiKey;
            //todo: handle problems with api key
            _gptService.InitializeGptService(_apiKey, "gpt-4");
            
        }

        await Task.CompletedTask;
    }


    public string maskedChatText;

    public IEnumerable<Mask> GetMasks()
    {
        return _maskDataService.GetMasks();
    }



    //handle click in the flyout menu
    public void OnFlyoutElementClicked(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine("Flyout element clicked " + (sender as FrameworkElement).ToString() + " -> " + (sender as AppBarButton).Label);

        switch ((sender as AppBarButton).Label)
        {

            case "Copy":
                var package = new DataPackage();
                var messageItem = (MessageItem)(sender as AppBarButton).DataContext;
                package.SetText(messageItem.MsgText);
                Clipboard.SetContent(package);
                break;
            case "Share":
                break;
            default:
                break;
        }
        // Run code when the element is clicked
        //        //MaskedChatText = "hello world";
        //           }
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

    //call the gpt service to generate a chat completion based on the current chat history
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

    //updates the model with the masked chat text and calls the gpt service
    public void SendChat()
    {
        //give out the masked chat text to console
        Debug.WriteLine(MaskedChatText);
        _chatDataService.Messages.Add(new Message(MaskedChatText, DateTime.Now, "user"));
        //ask the gpt service for a response to the new message
        AskGptService();


    }

    //add a dummy user message to the chat history
    public void AddItemToEnd()
    {
        //InvertedListView.Items.Add(
        //    new Message("Message " + ++messageNumber, DateTime.Now, HorizontalAlignment.Right)
        //    );
        MessageItems.Add(
            new MessageItem("Message " + ++messageNumber, DateTime.Now, HorizontalAlignment.Right)
            );
    }

    //add a dummy assistant message to the chat history
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
