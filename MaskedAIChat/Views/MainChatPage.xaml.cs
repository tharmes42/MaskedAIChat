using MaskedAIChat.Core.Contracts.Services;
using MaskedAIChat.ViewModels;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;

namespace MaskedAIChat.Views;

public sealed partial class MainChatPage : Page
{
    IChatDataService chatDataService;
    IMaskDataService maskDataService;

    public MainChatViewModel ViewModel
    {
        get;
    }

    public MainChatPage()
    {
        ViewModel = App.GetService<MainChatViewModel>();
        chatDataService = App.GetService<IChatDataService>();
        maskDataService = App.GetService<IMaskDataService>();
        InitializeComponent();
    }


    private void Menu_Opening(object sender, object e)
    {
        CommandBarFlyout myFlyout = sender as CommandBarFlyout;
        if (myFlyout.Target == REBSource)
        {
            AppBarButton myButton = new AppBarButton();
            myButton.Command = new StandardUICommand(StandardUICommandKind.Share);
            myFlyout.PrimaryCommands.Add(myButton);
        }
    }


    private void REBSource_Loaded(object sender, RoutedEventArgs e)
    {
        REBSource.SelectionFlyout.Opening += Menu_Opening;
        REBSource.ContextFlyout.Opening += Menu_Opening;

    }

    private void REBSource_Unloaded(object sender, RoutedEventArgs e)
    {
        REBSource.SelectionFlyout.Opening -= Menu_Opening;
        REBSource.ContextFlyout.Opening -= Menu_Opening;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        //TODO hier stimmt noch etwas nicht
        base.OnNavigatedTo(e);
        //this is kind of asynchroneous, I read from Model, but I write to the data service -> ugly
        REBSource.Document.SetText(Microsoft.UI.Text.TextSetOptions.None, ViewModel.chatText);
    }

    private void REBSource_TextChanged(object sender, RoutedEventArgs e)
    {
        REBSource.Document.GetText(Microsoft.UI.Text.TextGetOptions.None, out var value);
        //save to model, so we don't loose data on navigation,
        chatDataService.SetChatText(value);


        //find sensitive information and build mask lisk
        maskDataService.BuildMasks(value);
        //mask the whole text based on previously constructed mask list 
        value = maskDataService.MaskText(value);
        //update destination chat with masked text
        REBDestination.Document.SetText(Microsoft.UI.Text.TextSetOptions.None, value);
        //highlight masked words
        REBDestinationHighlightMatches();

    }

    private void REBSource_SelectionChanged(object sender, RoutedEventArgs e)
    {
        REBSource.Document.Selection.GetText(TextGetOptions.None, out var selectedText);
        FindBoxHighlightMatches(selectedText);
    }


    private void FindBoxHighlightMatches(string textToFind)
    {
        TextBoxRemoveHighlights();

        Color highlightBackgroundColor = (Color)App.Current.Resources["SystemColorHighlightColor"];
        Color highlightForegroundColor = (Color)App.Current.Resources["SystemColorHighlightTextColor"];

        if (textToFind != null)
        {
            ITextRange searchRange = REBDestination.Document.GetRange(0, 0);
            while (searchRange.FindText(textToFind, TextConstants.MaxUnitCount, FindOptions.None) > 0)
            {
                searchRange.CharacterFormat.BackgroundColor = highlightBackgroundColor;
                searchRange.CharacterFormat.ForegroundColor = highlightForegroundColor;
            }
        }
    }

    private void REBDestinationHighlightMatches()
    {
        TextBoxRemoveHighlights();

        Color highlightBackgroundColor = (Color)App.Current.Resources["SystemColorHighlightColor"];
        Color highlightForegroundColor = (Color)App.Current.Resources["SystemColorHighlightTextColor"];

        var masks = maskDataService.GetMasks();
        if (masks != null)
        {
            foreach (var mask in masks)
            {
                ITextRange searchRange = REBDestination.Document.GetRange(0, 0);
                while (searchRange.FindText(mask.MaskedText, TextConstants.MaxUnitCount, FindOptions.None) > 0)
                {
                    searchRange.CharacterFormat.BackgroundColor = highlightBackgroundColor;
                    searchRange.CharacterFormat.ForegroundColor = highlightForegroundColor;
                }
            }

        }

    }

    private void TextBoxRemoveHighlights()
    {
        ITextRange documentRange = REBDestination.Document.GetRange(0, TextConstants.MaxUnitCount);
        SolidColorBrush defaultBackground = REBDestination.Background as SolidColorBrush;
        SolidColorBrush defaultForeground = REBDestination.Foreground as SolidColorBrush;

        documentRange.CharacterFormat.BackgroundColor = defaultBackground.Color;
        documentRange.CharacterFormat.ForegroundColor = defaultForeground.Color;
    }

    private async void REBSource_Paste(object sender, TextControlPasteEventArgs e)
    {
        if (sender is RichEditBox)
        {
            DataPackageView dataPackageView = Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Text))
            {
                var text = await dataPackageView.GetTextAsync();

                // To output the text from this example, you need a TextBlock control
                // with a name of "TextOutput".
                REBSource.Document.SetText(TextSetOptions.None, text);

            }

            e.Handled = true; // Mark the event as handled to prevent the default paste behavior

        }
    }

}
