using System.ComponentModel;
using System.Text;
using Azure;
using Azure.AI.OpenAI;
using MaskedAIChat.Core.Contracts.Services;

namespace MaskedAIChat.Core.Services;

//using Azure.AI.OpenAI nuget package, see https://github.com/Azure/azure-sdk-for-net/blob/Azure.AI.OpenAI_1.0.0-beta.6/sdk/openai/Azure.AI.OpenAI/README.md
public class TransformerServiceOpenAI : ITransformerService
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

    public bool IsInitialized => gptClient != null;


    //private const string ApiUrl = "https://api.openai.com/v1/engines/davinci-codex/completions";
    private const string ApiUrl = "https://api.openai.com/v1/chat/completions";

    public TransformerServiceOpenAI()
    {
        Model = null;
        gptClient = null;
        //load api key from user secrets -> right click on project -> Manage User Secrets
        //var builder = new ConfigurationBuilder().AddUserSecrets<GptService>();
        //IConfigurationRoot Configuration = builder.Build();
        //ApiKey = Configuration["Services:ApiKey"];
        //gptClient = new OpenAIClient(ApiKey, new OpenAIClientOptions());
    }


    public void InitializeTransformerService(string apiKey, string model = "gpt-4")
    {
        ApiKey = apiKey;
        Model = model;
        //HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
        gptClient = new OpenAIClient(ApiKey, new OpenAIClientOptions());
    }

    // Generate completion with the provided string
    public async Task<string> GenerateChatCompletionAsync(string prompt, int maxTokens = 256)
    {

        var chatCompletionsOptions = new ChatCompletionsOptions()
        {
            MaxTokens = maxTokens,
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
                stringBuilder.Append(message.Content);
            }
        }
        return stringBuilder.ToString();
    }

    // Generate completion with the provided ChatCompletionOptions (useful to provide chat history)
    public async Task<string> GenerateChatCompletionAsync(ChatCompletionsOptions chatCompletionsOptions)
    {
        StringBuilder stringBuilder = new StringBuilder();

        try
        {
            Response<StreamingChatCompletions> response = await gptClient.GetChatCompletionsStreamingAsync(
            deploymentOrModelName: Model,
            chatCompletionsOptions);

            using StreamingChatCompletions streamingChatCompletions = response.Value;

            await foreach (StreamingChatChoice choice in streamingChatCompletions.GetChoicesStreaming())
            {
                await foreach (ChatMessage message in choice.GetMessageStreaming())
                {
                    stringBuilder.Append(message.Content);
                }
            }

        }
        catch (Azure.RequestFailedException requestFailedException)
        {
            stringBuilder.Append(requestFailedException.Message);
        }
        catch (Exception ex)
        {
            stringBuilder.Append(ex.Message);
        }
        return stringBuilder.ToString();
    }


    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);
    protected void RaisePropertyChanged(string propertyName) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

}
