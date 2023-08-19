using System.ComponentModel;
using Azure.AI.OpenAI;

namespace MaskedAIChat.Core.Contracts.Services;


public interface ITransformerService : INotifyPropertyChanged
{
    public bool IsInitialized { get; }
    void InitializeTransformerService(string apiKey, string model);
    Task<string> GenerateChatCompletionAsync(string prompt, int maxTokens = 256);
    Task<string> GenerateChatCompletionAsync(ChatCompletionsOptions chatCompletionsOptions);
}


