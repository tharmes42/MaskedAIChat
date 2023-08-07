using Microsoft.UI.Xaml;

namespace MaskedAIChat.Models;


// https://github.com/microsoft/WinUI-Gallery/blob/main/WinUIGallery/ControlPages/ListViewPage.xaml.cs
public class MessageItem
{
    public string MsgText
    {
        get; private set;
    }
    public DateTime MsgDateTime
    {
        get; private set;
    }
    public HorizontalAlignment MsgAlignment
    {
        get; set;
    }
    public MessageItem(string text, DateTime dateTime, HorizontalAlignment align)
    {
        MsgText = text;
        MsgDateTime = dateTime;
        MsgAlignment = align;
    }

    public override string ToString()
    {
        return MsgDateTime.ToString() + " " + MsgText;
    }
}

