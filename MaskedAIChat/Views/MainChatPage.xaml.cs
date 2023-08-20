using System.Diagnostics;
using MaskedAIChat.Models;
using MaskedAIChat.ViewModels;
using Microsoft.UI.Input;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;

namespace MaskedAIChat.Views;

public sealed partial class MainChatPage : Page
{
    // Dictionary to maintain information about each active pointer. 
    // An entry is added during PointerPressed/PointerEntered events and removed 
    // during PointerReleased/PointerCaptureLost/PointerCanceled/PointerExited events.
    Dictionary<uint, Microsoft.UI.Xaml.Input.Pointer> pointers;


    public MainChatViewModel ViewModel
    {
        get;
    }

    public MainChatPage()
    {
        ViewModel = App.GetService<MainChatViewModel>();
        InitializeComponent();
        MainChat_ChatText.Language = Windows.Globalization.Language.CurrentInputMethodLanguageTag;
        // Initialize the dictionary.
        pointers = new Dictionary<uint, Microsoft.UI.Xaml.Input.Pointer>();
        // Add first item to inverted list so it's not empty
        // ViewModel.AddItemToEnd();
        //MainChat_InvertedListView.ContextFlyout.Opening += Menu_Opening;
    }

    private async Task InitializeAsync()
    {

        //load viewmodel settings async 
        await ViewModel.InitializeModelAsync();
        await Task.CompletedTask;
    }

    protected async override void OnNavigatedTo(NavigationEventArgs e)
    {
        // Run code when the app navigates to this page

        await InitializeAsync();
    }


    public void OnNavigatedFrom()
    {
        // Run code when the app navigates away from this page

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


    //private void MainChat_InvertedListView_Loaded(object sender, RoutedEventArgs e)
    //{
    //    MainChat_InvertedListView.ContextFlyout.Opening += Menu_Opening;
    //}

    //private void MainChat_InvertedListView_Unloaded(object sender, RoutedEventArgs e)
    //{
    //    MainChat_InvertedListView.ContextFlyout.Opening -= Menu_Opening;
    //}


    private void MainChat_ChatText_SelectionChanged(object sender, RoutedEventArgs e)
    {
        //MainChat_ChatText.Document.Selection.GetText(TextGetOptions.None, out var selectedText);
        //FindBoxHighlightMatches(selectedText);
    }

    private void MainChat_MessageItem_SelectionChanged(object sender, RoutedEventArgs e)
    {
        TextBlock tb = sender as TextBlock;
        if (tb != null)
        {
            MessageItem msgItem = tb.DataContext as MessageItem;
            if (msgItem != null)
            {
                if (tb.SelectedText.Equals(""))
                {
                    msgItem.MsgSelectedText = null;
                }
                else
                {
                    msgItem.MsgSelectedText = tb.SelectedText;
                }
            }
            Debug.WriteLine("MainChat_MessageItem_SelectionChanged" + tb.ToString());
        }
        else
        {
            Debug.WriteLine("MainChat_MessageItem_SelectionChanged" + (sender as FrameworkElement).ToString());
        }


        // show flyout if we a selection
        MainChat_ShowMessageItemFlyout(sender, true);
    }

    private void MainChat_InvertedListView_Click(object sender, RoutedEventArgs e)
    {
        //MainChat_ShowMessageItemFlyout(sender, false);

    }

    private void MainChat_ShowMessageItemFlyout(object sender, bool isTransient)
    {
        FlyoutShowOptions myOption = new FlyoutShowOptions();
        myOption.ShowMode = isTransient ? FlyoutShowMode.Transient : FlyoutShowMode.Standard;
        MessageItemFlyout.ShowAt((sender as FrameworkElement), myOption);
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

    private async void MainChat_ChatText_Paste(object sender, RoutedEventArgs e)
    {
        if (sender is Button)
        {
            DataPackageView dataPackageView = Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Text))
            {
                var text = await dataPackageView.GetTextAsync();

                // To output the text from this example, you need a TextBlock control
                // with a name of "TextOutput".
                MainChat_ChatText.Document.SetText(TextSetOptions.None, text);

            }
        }
    }

    private void MainChat_SendButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.SendChat();
    }

    private void MainChat_ClearChatButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.ClearChat();
    }


    /// <summary>
    /// The pointer entered event handler.
    /// We do not capture the pointer on this event.
    /// </summary>
    /// <param name="sender">Source of the pointer event.</param>
    /// <param name="e">Event args for the pointer routed event.</param>
    /// https://learn.microsoft.com/en-us/windows/apps/design/input/handle-pointer-input#pointer-event-example
    private void MessageItem_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        // Prevent most handlers along the event route from handling the same event again.
        e.Handled = true;

        PointerPoint ptrPt = e.GetCurrentPoint((sender as FrameworkElement));

        Debug.WriteLine("Pointer entered: " + ptrPt.PointerId);

        // Check if pointer already exists (if enter occurred prior to down).
        if (!pointers.ContainsKey(ptrPt.PointerId))
        {
            // Add contact to dictionary.
            pointers[ptrPt.PointerId] = e.Pointer;
        }

        if (pointers.Count == 1)
        {
            // show flyout if we have a single pointer
            MainChat_ShowMessageItemFlyout(sender, true);
        }

    }

    /// <summary>
    /// The pointer exited event handler.
    /// </summary>
    /// <param name="sender">Source of the pointer event.</param>
    /// <param name="e">Event args for the pointer routed event.</param>
    /// https://learn.microsoft.com/en-us/windows/apps/design/input/handle-pointer-input#pointer-event-example
    private void MessageItem_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        // Prevent most handlers along the event route from handling the same event again.
        e.Handled = true;

        PointerPoint ptrPt = e.GetCurrentPoint((sender as FrameworkElement));

        Debug.WriteLine("Pointer exited: " + ptrPt.PointerId);

        // Remove contact from dictionary.
        if (pointers.ContainsKey(ptrPt.PointerId))
        {
            pointers[ptrPt.PointerId] = null;
            pointers.Remove(ptrPt.PointerId);
        }

        //if (pointers.Count == 0)
        //{
        //    MessageItemFlyout.Hide();
        //}


    }

}

