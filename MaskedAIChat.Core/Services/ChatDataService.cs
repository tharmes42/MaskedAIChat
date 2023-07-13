using System.ComponentModel;
using MaskedAIChat.Core.Contracts.Services;

namespace MaskedAIChat.Core.Services;


public class ChatDataService : IChatDataService
{
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

    public ChatDataService()
    {
        _chatText ??= "";

    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);
    protected void RaisePropertyChanged(string propertyName) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

}
