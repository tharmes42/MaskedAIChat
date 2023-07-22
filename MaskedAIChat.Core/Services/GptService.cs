using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using MaskedAIChat.Core.Contracts.Services;
using Newtonsoft.Json;

namespace MaskedAIChat.Core.Services;

//todo: write tests
public class GptService : IGptService
{

    private static readonly HttpClient HttpClient = new();
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
        HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
    }

    public async Task<string> GenerateTextAsync(string prompt, int maxTokens = 256)
    {
        var request = new
        {
            model = Model,
            messages = new
            {
                prompt
            },
            temperature = 1,
            max_tokens = 256,
            top_p = 1,
            frequency_penalty = 0,
            presence_penalty = 0
        };
        // var request = new { prompt,  max_tokens = maxTokens };
        var jsonContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        Debug.WriteLine(request);

        var response = await HttpClient.PostAsync(ApiUrl, jsonContent);

        //todo: handle errors properly
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException("Error occurred while generating text.");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);

        return responseObject.choices[0].text.ToString();
    }


    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);
    protected void RaisePropertyChanged(string propertyName) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

}
