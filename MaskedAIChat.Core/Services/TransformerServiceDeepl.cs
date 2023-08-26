using System.ComponentModel;
using System.Text;
using Azure.AI.OpenAI;
using MaskedAIChat.Core.Contracts.Services;
using Newtonsoft.Json;

namespace MaskedAIChat.Core.Services;


//todo: change this to use deepl api
// https://www.deepl.com/docs-api
public class TransformerServiceDeepl : ITransformerService
{

    private HttpClient translationClient;
    private string ApiKey
    {
        get; set;
    }

    private string Model
    {
        get; set;
    }

    public bool IsInitialized => translationClient != null;


    private const string ApiUrl = "https://api-free.deepl.com/v2/translate";

    public TransformerServiceDeepl()
    {
        Model = null;
        translationClient = null;
    }

    /// <summary>
    /// initialize the transformer service
    /// see https://www.deepl.com/docs-api
    /// </summary>
    /// <param name="apiKey">Deepl API Key</param>
    /// <param name="model">Target Language</param>
    public void InitializeTransformerService(string apiKey, string model)
    {
        ApiKey = apiKey;
        //model is here the target language
        Model = model;
        //HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
        translationClient = new HttpClient();
    }

    // Generate completion with the provided string
    public async Task<string> GenerateChatCompletionAsync(string prompt, int maxTokens = 256)
    {
        StringBuilder stringBuilder = new StringBuilder();


        if (translationClient == null)
        {
            translationClient = new HttpClient();
        }

        translationClient.DefaultRequestHeaders.Add("Authorization", $"DeepL-Auth-Key {ApiKey}");
        translationClient.DefaultRequestHeaders.Add("User-Agent", "MaskedAiChat/1.0.0");

        var requestData = new
        {
            text = new[] { prompt },
            target_lang = Model
        };

        var json = JsonConvert.SerializeObject(requestData);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await translationClient.PostAsync(ApiUrl, data);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            var translationResponse = JsonConvert.DeserializeObject<TranslationResponse>(result);

            if (translationResponse != null && translationResponse.Translations.Count > 0)
            {
                Console.WriteLine($"Translated Text: {translationResponse.Translations[0].Text}");
                Console.WriteLine($"Detected Source Language: {translationResponse.Translations[0].DetectedSourceLanguage}");
                stringBuilder.Append(translationResponse.Translations[0].Text);
            }
        }
        else
        {
            Console.WriteLine($"Failed to translate text. Status code: {response.StatusCode}");
        }

        return stringBuilder.ToString();
    }

    // Generate completion with the provided ChatCompletionOptions (useful to provide chat history)
    public async Task<string> GenerateChatCompletionAsync(ChatCompletionsOptions chatCompletionsOptions)
    {

        throw new NotImplementedException();
        return "";
    }

    public class TranslationResponse
    {
        [JsonProperty("translations")]
        public System.Collections.Generic.List<Translation> Translations
        {
            get; set;
        }
    }

    public class Translation
    {
        [JsonProperty("detected_source_language")]
        public string DetectedSourceLanguage
        {
            get; set;
        }

        [JsonProperty("text")]
        public string Text
        {
            get; set;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);
    protected void RaisePropertyChanged(string propertyName) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

}
