using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using MaskedAIChat.Contracts.ViewModels;
using MaskedAIChat.Core.Contracts.Services;
using MaskedAIChat.Core.Models;

namespace MaskedAIChat.ViewModels;

public partial class MaskDataGridViewModel : ObservableRecipient, INavigationAware
{
    private readonly IMaskDataService _maskDataService;

    public ObservableCollection<Mask> Source { get; } = new ObservableCollection<Mask>();

    public MaskDataGridViewModel(IMaskDataService maskDataService)
    {
        _maskDataService = maskDataService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        Source.Clear();

        var data = await _maskDataService.GetGridDataAsync();

        foreach (var item in data)
        {
            Source.Add(item);
        }
    }

    public void OnNavigatedFrom()
    {
    }
}
