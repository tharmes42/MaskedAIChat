using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MaskedAIChat.Core.Services.Tests;

[TestClass()]
public class MaskDataServiceTests
{
    private static MaskDataService _maskDataService;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        Debug.WriteLine("ClassInitialize");


        _maskDataService = new MaskDataService();
        Assert.IsNotNull(_maskDataService);
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

    [TestMethod()]
    public void MaskTextTest()
    {
        var ChatText = "This is a text from secretemail@example.com";
        var MaskedChatText = "";
        _maskDataService.BuildMasks(ChatText);
        MaskedChatText = _maskDataService.MaskText(ChatText);
        Assert.IsFalse(MaskedChatText.Contains("secretemail@example.com"));
        Assert.IsTrue(MaskedChatText.Contains("Masked1@example.com"));
    }
}