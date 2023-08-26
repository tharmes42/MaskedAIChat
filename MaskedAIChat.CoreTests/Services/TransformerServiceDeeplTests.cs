using System.Diagnostics;
using MaskedAIChat.Core.Contracts.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MaskedAIChat.Core.Services.Tests;

[TestClass()]
public class TransformerServiceDeeplTests
{

    private static ITransformerService _transformerService;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        Debug.WriteLine("ClassInitialize");

        var builder = new ConfigurationBuilder().AddUserSecrets<TransformerServiceDeeplTests>();

        IConfigurationRoot Configuration = builder.Build();

        string apiKey = Configuration["Services:DeeplApiKey"];

        _transformerService = new TransformerServiceDeepl();
        _transformerService.InitializeTransformerService(apiKey, "DE");
        Assert.IsNotNull(_transformerService);
    }


    [TestMethod()]
    public void GenerateChatCompletionAsyncTest_TextToTranslate_IsNotEmpty()
    {
        var textToTranslate = "Hello World!";
        var translatedText = _transformerService.GenerateChatCompletionAsync(textToTranslate);

        Assert.IsFalse(translatedText.Result.Equals(""));
    }
}