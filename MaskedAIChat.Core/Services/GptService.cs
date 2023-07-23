using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Azure;
using Azure.AI.OpenAI;
using MaskedAIChat.Core.Contracts.Services;

namespace MaskedAIChat.Core.Services;

//todo: write tests
//using Azure.AI.OpenAI nuget package, see https://github.com/Azure/azure-sdk-for-net/blob/Azure.AI.OpenAI_1.0.0-beta.6/sdk/openai/Azure.AI.OpenAI/README.md
public class GptService : IGptService
{

    private OpenAIClient gptClient;
    private string ApiKey
    {
        get; set;
    }

    private string Model
    {
        get; set;
    }


    //private const string ApiUrl = "https://api.openai.com/v1/engines/davinci-codex/completions";
    private const string ApiUrl = "https://api.openai.com/v1/chat/completions";

    public GptService(string apiKey, string model = "gpt-4")
    {
        ApiKey = apiKey;
        Model = model;
        //HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
        gptClient = new OpenAIClient(ApiKey, new OpenAIClientOptions());
    }

    public async Task<string> GenerateChatCompletionAsync(string prompt, int maxTokens = 256)
    {

        var chatCompletionsOptions = new ChatCompletionsOptions()
        {
            Messages =
            {
                new ChatMessage(ChatRole.System, "You are a helpful assistant."),
                new ChatMessage(ChatRole.User, prompt),
                //new ChatMessage(ChatRole.Assistant, "Arrrr! Of course, me hearty! What can I do for ye?"),
                //new ChatMessage(ChatRole.User, "What's the best way to train a parrot?"),
            }
        };

        Response<StreamingChatCompletions> response = await gptClient.GetChatCompletionsStreamingAsync(
            deploymentOrModelName: Model,
            chatCompletionsOptions);
        using StreamingChatCompletions streamingChatCompletions = response.Value;

        StringBuilder stringBuilder = new StringBuilder();

        await foreach (StreamingChatChoice choice in streamingChatCompletions.GetChoicesStreaming())
        {
            await foreach (ChatMessage message in choice.GetMessageStreaming())
            {
                //Debug.Write(message.Content);
                stringBuilder.Append(message.Content);
            }
            //Debug.WriteLine("");
        }
        Debug.WriteLine(stringBuilder.ToString());
        //ChatChoice responseChoice = response.Value.Choices[0];
        //ChatChoice responseChoice = response.Value.GetChoicesStreaming()[0];
        return stringBuilder.ToString();
    }


    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);
    protected void RaisePropertyChanged(string propertyName) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

}
