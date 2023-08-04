using MaskedAIChat.Core.Contracts.Services;
using MaskedAIChat.ViewModels;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.ApplicationModel.DataTransfer;

namespace MaskedAIChat.Views;

public sealed partial class MainChatPage : Page
{
    IChatDataService chatDataService;
    int messageNumber;

    public MainChatViewModel ViewModel
    {
        get;
    }

    public MainChatPage()
    {
        ViewModel = App.GetService<MainChatViewModel>();
        InitializeComponent();
        // Add first item to inverted list so it's not empty
        AddItemToEnd();
    }


    private void Menu_Opening(object sender, object e)
    {
        CommandBarFlyout myFlyout = sender as CommandBarFlyout;
        if (myFlyout.Target == MainChat_ChatText)
        {
            AppBarButton myButton = new AppBarButton();
            myButton.Command = new StandardUICommand(StandardUICommandKind.Share);
            myFlyout.PrimaryCommands.Add(myButton);
        }
    }


    private void MainChat_ChatText_Loaded(object sender, RoutedEventArgs e)
    {
        MainChat_ChatText.SelectionFlyout.Opening += Menu_Opening;
        MainChat_ChatText.ContextFlyout.Opening += Menu_Opening;

    }

    private void MainChat_ChatText_Unloaded(object sender, RoutedEventArgs e)
    {
        MainChat_ChatText.SelectionFlyout.Opening -= Menu_Opening;
        MainChat_ChatText.ContextFlyout.Opening -= Menu_Opening;
    }


    private void MainChat_ChatText_SelectionChanged(object sender, RoutedEventArgs e)
    {
        //MainChat_ChatText.Document.Selection.GetText(TextGetOptions.None, out var selectedText);
        //FindBoxHighlightMatches(selectedText);
    }


    //private void FindBoxHighlightMatches(string textToFind)
    //{
    //    TextBoxRemoveHighlights();

    //    Color highlightBackgroundColor = (Color)App.Current.Resources["SystemColorHighlightColor"];
    //    Color highlightForegroundColor = (Color)App.Current.Resources["SystemColorHighlightTextColor"];

    //    if (textToFind != null)
    //    {
    //        ITextRange searchRange = REBDestination.Document.GetRange(0, 0);
    //        while (searchRange.FindText(textToFind, TextConstants.MaxUnitCount, FindOptions.None) > 0)
    //        {
    //            searchRange.CharacterFormat.BackgroundColor = highlightBackgroundColor;
    //            searchRange.CharacterFormat.ForegroundColor = highlightForegroundColor;
    //        }
    //    }
    //}

    //private void REBDestinationHighlightMatches()
    //{
    //    TextBoxRemoveHighlights();

    //    Color highlightBackgroundColor = (Color)App.Current.Resources["SystemColorHighlightColor"];
    //    Color highlightForegroundColor = (Color)App.Current.Resources["SystemColorHighlightTextColor"];

    //    var masks = ViewModel.GetMasks();
    //    if (masks != null)
    //    {
    //        foreach (var mask in masks)
    //        {
    //            ITextRange searchRange = REBDestination.Document.GetRange(0, 0);
    //            while (searchRange.FindText(mask.MaskedText, TextConstants.MaxUnitCount, FindOptions.None) > 0)
    //            {
    //                searchRange.CharacterFormat.BackgroundColor = highlightBackgroundColor;
    //                searchRange.CharacterFormat.ForegroundColor = highlightForegroundColor;
    //            }
    //        }

    //    }

    //}

    //private void TextBoxRemoveHighlights()
    //{
    //    ITextRange documentRange = REBDestination.Document.GetRange(0, TextConstants.MaxUnitCount);
    //    SolidColorBrush defaultBackground = REBDestination.Background as SolidColorBrush;
    //    SolidColorBrush defaultForeground = REBDestination.Foreground as SolidColorBrush;

    //    documentRange.CharacterFormat.BackgroundColor = defaultBackground.Color;
    //    documentRange.CharacterFormat.ForegroundColor = defaultForeground.Color;
    //}

    private async void MainChat_ChatText_Paste(object sender, TextControlPasteEventArgs e)
    {
        if (sender is RichEditBox)
        {
            DataPackageView dataPackageView = Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Text))
            {
                var text = await dataPackageView.GetTextAsync();

                // To output the text from this example, you need a TextBlock control
                // with a name of "TextOutput".
                MainChat_ChatText.Document.SetText(TextSetOptions.None, text);

            }

            e.Handled = true; // Mark the event as handled to prevent the default paste behavior

        }
    }

    //MainChat_SendButton_OnSend
    private void MainChat_SendButton_OnSend(object sender, RoutedEventArgs e)
    {
        ViewModel.SendChat();
        AddItemToEnd();
    }

    //===================================================================================================================
    // Inverted List Example
    //===================================================================================================================

    private void AddItemToEnd()
    {
        InvertedListView.Items.Add(
            new Message("Message " + ++messageNumber, DateTime.Now, HorizontalAlignment.Right)
            );
    }

    private void MessageReceived(object sender, RoutedEventArgs e)
    {
        InvertedListView.Items.Add(
            new Message("Message " + ++messageNumber, DateTime.Now, HorizontalAlignment.Left)
            );
    }

}

//TODO: remove from Code Behind
// https://github.com/microsoft/WinUI-Gallery/blob/main/WinUIGallery/ControlPages/ListViewPage.xaml.cs
public class Message
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
    public Message(string text, DateTime dateTime, HorizontalAlignment align)
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
