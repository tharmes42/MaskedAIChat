using System.ComponentModel;

namespace MaskedAIChat.Core.Contracts.Services;

public interface IGptService : INotifyPropertyChanged
{
    public interface IGptService
    {
        Task<string> GenerateTextAsync(string prompt, int maxTokens = 60);
    }


}
