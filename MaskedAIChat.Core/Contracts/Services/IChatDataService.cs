using System.ComponentModel;

namespace MaskedAIChat.Core.Contracts.Services;

public interface IChatDataService : INotifyPropertyChanged
{
    public string ChatText
    {
        get; set;
    }


}
