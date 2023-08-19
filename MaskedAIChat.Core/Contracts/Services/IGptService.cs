using System.ComponentModel;
using Azure.AI.OpenAI;

namespace MaskedAIChat.Core.Contracts.Services;


public interface IGptService : INotifyPropertyChanged
{
    public bool IsInitialized { get; }
    void InitializeGptService(string apiKey, string model = "gpt-4");
    Task<string> GenerateChatCompletionAsync(string prompt, int maxTokens = 256);
    Task<string> GenerateChatCompletionAsync(ChatCompletionsOptions chatCompletionsOptions);
}


