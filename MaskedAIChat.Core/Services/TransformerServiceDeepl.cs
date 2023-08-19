using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Transactions;
using Azure;
using Azure.AI.OpenAI;
using Azure.Core;
using MaskedAIChat.Core.Contracts.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

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


    //private const string ApiUrl = "https://api.openai.com/v1/engines/davinci-codex/completions";
    private const string ApiUrl = "https://api.openai.com/v1/chat/completions";

    public TransformerServiceDeepl()
    {
        Model = null;
        translationClient = null;
    }


    public void InitializeTransformerService(string apiKey, string model = "default")
    {
        ApiKey = apiKey;
        Model = model;
        //HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
        translationClient = new HttpClient();
    }

    // Generate completion with the provided string
    public async Task<string> GenerateChatCompletionAsync(string prompt, int maxTokens = 256)
    {
        throw new NotImplementedException();
        return "";
    }

    // Generate completion with the provided ChatCompletionOptions (useful to provide chat history)
    public async Task<string> GenerateChatCompletionAsync(ChatCompletionsOptions chatCompletionsOptions)
    {

        throw new NotImplementedException();
        return "";
    }



    static async Task TranslateText()
    {
        var url = "https://api-free.deepl.com/v2/translate";
        var textToTranslate = "Hello, world!";
        var targetLang = "DE";

        using (var httpClient = new HttpClient())
        {

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("text", textToTranslate),
                new KeyValuePair<string, string>("target_lang", targetLang)
            });

            var response = await httpClient.PostAsync(url, formContent);

            var data = await response.Content.ReadAsStringAsync();

            var translations = JObject.Parse(data)["translations"];
            foreach (var translation in translations)
            {
                Console.WriteLine(translation["text"]);
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);
    protected void RaisePropertyChanged(string propertyName) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

}
