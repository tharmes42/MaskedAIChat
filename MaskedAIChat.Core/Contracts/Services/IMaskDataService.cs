using MaskedAIChat.Core.Models;

namespace MaskedAIChat.Core.Contracts.Services;

// Remove this class once your pages/features are using your data.
public interface IMaskDataService
{
    string MaskText(string textToMask);

    Task<IEnumerable<Mask>> GetContentGridDataAsync();

    Task<IEnumerable<Mask>> GetGridDataAsync();

    Task<IEnumerable<Mask>> GetListDetailsDataAsync();
    void BuildMasks(string value);
    IEnumerable<Mask> GetMasks();
}
