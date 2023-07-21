using System.ComponentModel;
using System.Text;
using MaskedAIChat.Core.Contracts.Services;
using Newtonsoft.Json;

namespace MaskedAIChat.Core.Services;


public class GptService : IGptService
{

    private static readonly HttpClient HttpClient = new();
    private const string ApiKey = "Your OpenAI API Key";
    private const string ApiUrl = "https://api.openai.com/v1/engines/davinci-codex/completions";

    public GptService()
    {
        HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
    }

    public async Task<string> GenerateTextAsync(string prompt, int maxTokens = 60)
    {
        var request = new { prompt, max_tokens = maxTokens };
        var jsonContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        var response = await HttpClient.PostAsync(ApiUrl, jsonContent);

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
