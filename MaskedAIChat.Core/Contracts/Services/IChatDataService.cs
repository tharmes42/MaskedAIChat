using System.Collections.ObjectModel;
using System.ComponentModel;
using MaskedAIChat.Core.Models;

namespace MaskedAIChat.Core.Contracts.Services;

public interface IChatDataService : INotifyPropertyChanged
{
    public string ChatText
    {
        get; set;
    }

    public ObservableCollection<Message> Messages
    {
        get; set;
    }
}
