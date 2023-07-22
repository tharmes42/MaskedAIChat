using System.Diagnostics;
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
            _gptService = new GptService("asdf");
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
        public async Task GenerateTextAsyncTest()
        {

            var gptResponse = await _gptService.GenerateTextAsync("asdf");
            Debug.WriteLine(gptResponse);

            Assert.Fail();
        }
    }
}