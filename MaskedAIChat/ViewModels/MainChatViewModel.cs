﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
    private ObservableCollection<MessageItem> _messageItems;
    public ObservableCollection<MessageItem> MessageItems
    {
        //todo: attach to chatdataservice.Messages
        get => _messageItems;
        set => SetProperty(ref _messageItems, value);
    }


    public MainChatViewModel(IChatDataService chatDataService, IMaskDataService maskDataService)
    {
        _chatDataService = chatDataService;
        _maskDataService = maskDataService;
        _chatDataService.PropertyChanged += OnModelPropertyChanged;
        _messageItems = new ObservableCollection<MessageItem>();

    }


    public string maskedChatText;

    public IEnumerable<Mask> GetMasks()
    {
        return _maskDataService.GetMasks();
    }

    public void OnNavigatedTo(object parameter)
    {
        // Run code when the app navigates to this page
        MaskedChatText = "hello world";

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

    private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ChatDataService.ChatText):
                _maskDataService.BuildMasks(ChatText);
                MaskedChatText = _maskDataService.MaskText(ChatText);
                break;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    void RaisePropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void SendChat()
    {
        //give out the masked chat text to console
        Debug.WriteLine(MaskedChatText);

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
