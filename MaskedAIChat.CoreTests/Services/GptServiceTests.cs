using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MaskedAIChat.Core.Services.Tests
{
    [TestClass()]
    public class GptServiceTests
    {
        private static GptService _gptService;

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
        public async Task GenerateCompletionAsyncTest()
        {

            var gptResponse = await _gptService.GenerateChatCompletionAsync("What's the best way to train a parrot?");
            //Debug.WriteLine(gptResponse);
            // find string "parrot" in gtpResponse
            Assert.IsTrue(gptResponse.Contains("parrot"));

        }
    }
}