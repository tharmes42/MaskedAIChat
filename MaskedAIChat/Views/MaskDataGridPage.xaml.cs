using MaskedAIChat.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace MaskedAIChat.Views;

// TODO: Change the grid as appropriate for your app. Adjust the column definitions on DataGridPage.xaml.
// For more details, see the documentation at https://docs.microsoft.com/windows/communitytoolkit/controls/datagrid.
public sealed partial class MaskDataGridPage : Page
{
    public MaskDataGridViewModel ViewModel
    {
        get;
    }

    public MaskDataGridPage()
    {
        ViewModel = App.GetService<MaskDataGridViewModel>();
        InitializeComponent();
    }
}
