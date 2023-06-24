using MaskedAIChat.ViewModels;
using System;
using System.Collections.Generic;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace MaskedAIChat.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
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



    private void REBSource_TextChanged(object sender, RoutedEventArgs e)
    {
        REBSource.Document.GetText(Microsoft.UI.Text.TextGetOptions.None, out string value);
        REBDestination.Document.SetText(Microsoft.UI.Text.TextSetOptions.None, value);
    }

    private void REBSource_SelectionChanged(object sender, RoutedEventArgs e)
    {
        REBSource.Document.Selection.GetText(TextGetOptions.None, out string selectedText);
        FindBoxHighlightMatches(selectedText);
    }


    private void FindBoxHighlightMatches(string textToFind)
    {
        FindBoxRemoveHighlights();

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

    private void FindBoxRemoveHighlights()
    {
        ITextRange documentRange = REBDestination.Document.GetRange(0, TextConstants.MaxUnitCount);
        SolidColorBrush defaultBackground = REBDestination.Background as SolidColorBrush;
        SolidColorBrush defaultForeground = REBDestination.Foreground as SolidColorBrush;

        documentRange.CharacterFormat.BackgroundColor = defaultBackground.Color;
        documentRange.CharacterFormat.ForegroundColor = defaultForeground.Color;
    }



}
