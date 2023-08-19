using System.Diagnostics;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/*
Naming convention see https://stackoverflow.com/questions/5666013/c-sharp-unit-test-naming-convention-for-overloaded-method-tests 
and https://osherove.com/blog/2005/4/3/naming-standards-for-unit-tests.html
  MethodName_StateOfTheObject_ExpectedResult
  So for the behaviors mentioned above:

  [TestMethod]
  IsEmpty_OnEmptyStack_ReturnsTrue

  [TestMethod]
  Push_OnEmptyStack_MakesStackNotEmpty
*/

namespace MaskedAIChat.Core.Services.Tests
{
    [TestClass()]
    public class GptServiceTests
    {
        private static TransformerServiceOpenAI _gptService;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Debug.WriteLine("ClassInitialize");

            var builder = new ConfigurationBuilder().AddUserSecrets<GptServiceTests>();

            IConfigurationRoot Configuration = builder.Build();

            string apiKey = Configuration["Services:ApiKey"];

            _gptService = new GptService(apiKey);
            Assert.IsNotNull(_gptService);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Debug.WriteLine("ClassCleanup");
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Debug.WriteLine("TestInitialize");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Debug.WriteLine("TestCleanup");
        }

        [TestMethod()]
        public async Task GenerateCompletionAsync_SimpleStringMessage_ReturnsAnswer()
        {

            var gptResponse = await _gptService.GenerateChatCompletionAsync("What's the best way to train a parrot? Answer in max. 100 Words.");
            //Debug.WriteLine(gptResponse);
            // find string "parrot" in gtpResponse
            Assert.IsTrue(gptResponse.Contains("parrot"));

        }

        [TestMethod()]
        public async Task GenerateCompletionAsync_ChatCompletionOptions_ReturnsAnswer()
        {

            var chatOptions = new ChatCompletionsOptions()
            {

                MaxTokens = 256,
                Messages =
                {
                    new ChatMessage(ChatRole.System, "You are a helpful assistant. But you limit your answer to a maximum of 10 Words each time."),
                    new ChatMessage(ChatRole.User, "What's the best way to train a parrot? Answer in max. 100 Words"),
                    new ChatMessage(ChatRole.Assistant, "Start with trust building, then teach step-up command, keep sessions short."),
                    new ChatMessage(ChatRole.User, "What's the best way to train a parrot? Answer in max. 100 Words"),
                }
            };
            var gptResponse = await _gptService.GenerateChatCompletionAsync(chatOptions);
            Debug.WriteLine(gptResponse);
            // string is larger than 5 characters
            Assert.IsTrue(gptResponse.Length > 5);

        }
    }
}