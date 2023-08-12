using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

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
    public string MsgAuthor
    {
        get; private set;
    }

    public CommandBarFlyout MessageItemContextFlyout
    {
        get
        {
            CommandBarFlyout myFlyout = new CommandBarFlyout();
            AppBarButton myButton = new AppBarButton();
            myButton.Command = new StandardUICommand(StandardUICommandKind.Share);
            myFlyout.PrimaryCommands.Add(myButton);
            return myFlyout;
        }
    }




    public MessageItem(string text, DateTime dateTime, HorizontalAlignment align, string author = "")
    {
        MsgText = text;
        MsgDateTime = dateTime;
        MsgAlignment = align;
        MsgAuthor = author;
    }

    public override string ToString()
    {
        return MsgDateTime.ToString() + " " + MsgText;
    }
}

