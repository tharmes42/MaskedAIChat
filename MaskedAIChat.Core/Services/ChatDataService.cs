using System.Collections.ObjectModel;
using System.ComponentModel;
using MaskedAIChat.Core.Contracts.Services;
using MaskedAIChat.Core.Models;

namespace MaskedAIChat.Core.Services;


public class ChatDataService : IChatDataService
{
    //current chat text, not yet sent 
    private string _chatText;
    public string ChatText
    {

        get => _chatText;
        set
        {

            if (_chatText != value)
            {
                _chatText = value;
                RaisePropertyChanged(nameof(ChatText));
            }

        }

    }

    private ObservableCollection<Message> _messages;
    public ObservableCollection<Message> Messages
    {
        get; set;
    }


    public ChatDataService()
    {
        _chatText ??= "";
        _messages ??= new ObservableCollection<Message>();

    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);
    protected void RaisePropertyChanged(string propertyName) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

}
