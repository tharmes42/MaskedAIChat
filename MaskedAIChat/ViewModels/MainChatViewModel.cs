using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Azure.AI.OpenAI;
using CommunityToolkit.Mvvm.ComponentModel;
using MaskedAIChat.Contracts.Services;
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
    private ITransformerService _transformerServiceChat;
    private ITransformerService _transformerServiceTranslate;
    private readonly ILocalSettingsService _localSettingsService;
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

    public bool IsGptServiceInitialized => _transformerServiceChat != null && _transformerServiceChat.IsInitialized;
    public bool IsDeeplServiceInitialized => _transformerServiceTranslate != null && _transformerServiceTranslate.IsInitialized;

    //well, in the future this may not be necessary anymore https://github.com/microsoft/microsoft-ui-xaml/issues/5514

    public Visibility ChatServiceWarningVisibility => IsGptServiceInitialized ? Visibility.Collapsed : Visibility.Visible;
    public Visibility TranslationServiceWarningVisibility => IsDeeplServiceInitialized ? Visibility.Collapsed : Visibility.Visible;


    //https://stackoverflow.com/questions/73521265/binding-and-changing-a-listview-dynamically-using-mvvm

    public ObservableCollection<MessageItem> MessageItems
    {
        get
        {
            ObservableCollection<MessageItem> items = new ObservableCollection<MessageItem>();
            foreach (var message in _chatDataService.Messages)
            {
                var alignment = message.MsgChatRole.Equals("assistant") ? HorizontalAlignment.Left : HorizontalAlignment.Right;
                var waitingStatus = message.MsgChatRole.Equals("assistant") && message.MsgText.Equals("");
                items.Add(new MessageItem(message.MsgText, message.MsgDateTime, alignment, message.MsgChatRole, waitingStatus));
            }
            return items;
        }
    }



    public MainChatViewModel(IChatDataService chatDataService, IMaskDataService maskDataService, ILocalSettingsService localSettingsService)
    {
        _chatDataService = chatDataService;
        _maskDataService = maskDataService;
        _transformerServiceChat = new TransformerServiceOpenAI();
        _transformerServiceTranslate = new TransformerServiceDeepl();
        _localSettingsService = localSettingsService;


        _chatDataService.PropertyChanged += OnModelPropertyChanged;
        if (_chatDataService.Messages.Count == 0)
        {
            ClearChat();
        }

        ChatText = "";

    }

    // read all settings from local settings service
    public async Task InitializeModelAsync()
    {
        var cacheApiKey = await _localSettingsService.ReadSettingAsync<string>(_localSettingsService.SettingsKey_ApiKey);
        if (!String.IsNullOrEmpty(cacheApiKey))
        {
            //todo: handle problems with api key
            _transformerServiceChat.InitializeTransformerService(cacheApiKey, "gpt-4");

            if (!_transformerServiceChat.IsInitialized)
            {
                throw new Exception("Failed to initalize GPT Service");
            }
            else
            {
                RaisePropertyChanged(nameof(IsGptServiceInitialized));
                RaisePropertyChanged(nameof(ChatServiceWarningVisibility));
            }
            //_gptService.InitializeGptService(_apiKey, "gpt-4");

        }

        var cacheDeeplApiKey = await _localSettingsService.ReadSettingAsync<string>(_localSettingsService.SettingsKey_DeeplApiKey);
        if (!String.IsNullOrEmpty(cacheDeeplApiKey))
        {
            //todo: handle problems with api key
            _transformerServiceTranslate.InitializeTransformerService(cacheDeeplApiKey, "DE");

            if (!_transformerServiceTranslate.IsInitialized)
            {
                throw new Exception("Failed to initalize Translation Service");
            }
            else
            {
                RaisePropertyChanged(nameof(IsDeeplServiceInitialized));
                RaisePropertyChanged(nameof(TranslationServiceWarningVisibility));
            }
            //_gptService.InitializeGptService(_apiKey, "gpt-4");

        }

        await Task.CompletedTask;
    }


    public string maskedChatText;

    public IEnumerable<Mask> GetMasks()
    {
        return _maskDataService.GetMasks();
    }



    /// <summary>
    /// handle click in the flyout menu
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnFlyoutElementClicked(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine("Flyout element clicked " + (sender as FrameworkElement).ToString() + " -> " + (sender as AppBarButton).Label);
        AppBarButton button = sender as AppBarButton;
        var messageItem = (MessageItem)button.DataContext;
        if (messageItem == null)
        {
            Debug.WriteLine("messageItem is null!");
            return;
        }
        var text = messageItem.MsgText;
        if (messageItem.MsgSelectedText != null)
        {
            text = messageItem.MsgSelectedText;
        }


        switch (button.Label)
        {

            case "Copy":
                var package = new DataPackage();
                package.SetText(text);
                Clipboard.SetContent(package);
                break;
            case "Reuse":
                ChatText = text;
                break;
            case "Translate":
                SendMessageForTranslationAsync(text);
                break;
            case "Share":
                break;
            default:
                break;
        }

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

    /// <summary>
    /// react to change notfication of Model 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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

    /// <summary>
    /// call the gpt service to generate a chat completion based on the current chat history
    /// </summary>
    private async void AskChatService()
    {
        var messages = new List<ChatMessage>();
        foreach (var message in _chatDataService.Messages)
        {
            //todo: check if role is system
            messages.Add(new ChatMessage(message.MsgChatRole.Equals("assistant") ? ChatRole.Assistant : ChatRole.User, message.MsgText));
        }
        var chatOptions = new ChatCompletionsOptions(messages);
        var waitMessage = new Message("", DateTime.Now, "assistant", TransformerService.OpenAI);
        _chatDataService.Messages.Add(waitMessage);
        var gptResponse = await _transformerServiceChat.GenerateChatCompletionAsync(chatOptions);
        _chatDataService.Messages.Remove(waitMessage);
        _chatDataService.Messages.Add(new Message(gptResponse, DateTime.Now, "assistant", TransformerService.OpenAI));
    }




    public event PropertyChangedEventHandler PropertyChanged;

    // one of our viewmodels changed a property, notify the view
    void RaisePropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// updates the model with the masked chat text and calls the gpt service
    /// </summary>
    public void SendChat()
    {
        //give out the masked chat text to console
        Debug.WriteLine(MaskedChatText);
        _chatDataService.Messages.Add(new Message(MaskedChatText, DateTime.Now, "user", TransformerService.Human));
        //ask the gpt service for a response to the new message
        AskChatService();
        ChatText = "";

    }

    /// <summary>
    /// sends a message to the translation service for translation
    /// </summary>
    /// <param name="messageItem"></param>
    public async Task SendMessageForTranslationAsync(string msgText)
    {
        if (_transformerServiceTranslate.IsInitialized)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            _chatDataService.Messages.Add(new Message(culture.Name.ToString() + " | " + msgText, DateTime.Now, "user", TransformerService.Human));
            var translatedText = await _transformerServiceTranslate.GenerateChatCompletionAsync(msgText);
            _chatDataService.Messages.Add(new Message(translatedText, DateTime.Now, "assistant", TransformerService.DeepL));
        }
        else
        {
            _chatDataService.Messages.Add(new Message("Translation Service not initialized", DateTime.Now, "system", TransformerService.DeepL));
        }
    }


    //clears the chat and sets initial message
    public void ClearChat()
    {
        _chatDataService.Messages.Clear();
        _chatDataService.Messages.Add(new Message("You are a highly skilled helpful assistant.", DateTime.Now, "system", TransformerService.OpenAI));
    }

    /// <summary>
    /// Export the chat history to clipboard as text without system messages
    /// </summary>
    /// <param name="backIndex"> Number of messages to retrieve counting from the last message, 0 = all messages </param>
    public void ExportChatToClipboard(int backIndex = 0)
    {
        //all messages except system messages
        List<Message> messagesToExport = _chatDataService.Messages.ToList().Where(x => !x.MsgChatRole.Equals("system")).ToList();
        //skip messages from the back
        if (backIndex > 0)
        {
            messagesToExport = messagesToExport.Skip(Math.Max(0, messagesToExport.Count() - backIndex)).ToList();
        }

        var sb = new StringBuilder();
        var package = new DataPackage();

        //build text
        foreach (var message in messagesToExport)
        {
            sb.AppendLine(message.MsgChatRole + " >>>");
            sb.AppendLine(message.MsgText);
            sb.AppendLine();
        }

        //set clipboard content
        package.SetText(sb.ToString());
        Clipboard.SetContent(package);

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
