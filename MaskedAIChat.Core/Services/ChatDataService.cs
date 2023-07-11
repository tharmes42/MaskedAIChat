using MaskedAIChat.Core.Contracts.Services;

namespace MaskedAIChat.Core.Services;


public class ChatDataService : IChatDataService
{
    private string _chatText;
    private string _chatMaskedText;

    public ChatDataService()
    {
        _chatText ??= "";

    }

    public string GetChatText()
    {

        return _chatText;
    }

    public void SetChatText(string chatText)
    {

        _chatText = chatText;
    }

}
