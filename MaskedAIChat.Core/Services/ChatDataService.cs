using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

    //chat history
    //private ObservableCollection<Message> _messages;
    //public ObservableCollection<Message> Messages
    //{
    //    get => _messages;
    //    set
    //    {
    //        if (_messages != value)
    //        {
    //            _messages = value;
    //            RaisePropertyChanged(nameof(Messages));
    //        }
    //    }
    //}
    public ObservableCollection<Message> Messages
    {
        get; set;
    }

    public ChatDataService()
    {
        _chatText ??= "";
        Messages ??= new ObservableCollection<Message>();
        Messages.CollectionChanged += OnMessageItemsCollectionChanged;

    }

    // https://stackoverflow.com/questions/17183812/observablecollection-setter-isnt-firing-when-item-is-added
    private void OnMessageItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        RaisePropertyChanged(nameof(Messages));
        //todo: get data from MessageItems instead of directly use ChatText
        //todo: fixme

        //System.Windows.MessageBox.Show("Firing");
        //RaisePropertyChanged(() => MasterWorkerList); 
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);
    protected void RaisePropertyChanged(string propertyName) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

}
