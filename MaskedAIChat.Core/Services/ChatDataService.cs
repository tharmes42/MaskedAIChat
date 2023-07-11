using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MaskedAIChat.Core.Contracts.Services;
using MaskedAIChat.Core.Models;

namespace MaskedAIChat.Core.Services;


public class ChatDataService : IChatDataService
{
    private string _chatText;

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
