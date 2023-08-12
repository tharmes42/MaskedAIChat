using System.ComponentModel;
using Azure.AI.OpenAI;

namespace MaskedAIChat.Core.Contracts.Services;


public interface IGptService : INotifyPropertyChanged
{
    Task<string> GenerateChatCompletionAsync(string prompt, int maxTokens = 256);
    Task<string> GenerateChatCompletionAsync(ChatCompletionsOptions chatCompletionsOptions);
}


