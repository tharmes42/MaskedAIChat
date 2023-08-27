using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;

namespace MaskedAIChat.Models;


// https://github.com/microsoft/WinUI-Gallery/blob/main/WinUIGallery/ControlPages/ListViewPage.xaml.cs
public class MessageItem : INotifyPropertyChanged
{
    private string _msgText;
    public string MsgText
    {
        get => _msgText;

        set
        {
            _msgText = value;
            RaisePropertyChanged();
        }
    }
    public DateTime MsgDateTime
    {
        get; private set;
    }
    public HorizontalAlignment MsgAlignment
    {
        get; set;
    }
    public string MsgAuthor
    {
        get; private set;
    }

    public string MsgSelectedText
    {
        get; set;
    }

    // indicates if the message is waiting for a response from the bot
    private bool _msgWaitingStatus = false;
    public bool MsgWaitingStatus
    {
        get => _msgWaitingStatus;

        set
        {
            _msgWaitingStatus = value;
            //we notify the view that the visibility property has changed
            RaisePropertyChanged(nameof(MsgWaitingStatusVisibility));
        }
    }

    // indicates if the message is waiting for a response from the bot
    public Visibility MsgWaitingStatusVisibility => MsgWaitingStatus ? Visibility.Visible : Visibility.Collapsed;


    public event PropertyChangedEventHandler PropertyChanged;

    // one of our viewmodels changed a property, notify the view
    void RaisePropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }




    public MessageItem(string text, DateTime dateTime, HorizontalAlignment align, string author = "", bool waitingStatus = false)
    {
        MsgText = text;
        MsgDateTime = dateTime;
        MsgAlignment = align;
        MsgAuthor = author;
        MsgSelectedText = null;
        MsgWaitingStatus = waitingStatus;
    }

    public override string ToString()
    {
        return MsgDateTime.ToString() + " " + MsgText;
    }
}

