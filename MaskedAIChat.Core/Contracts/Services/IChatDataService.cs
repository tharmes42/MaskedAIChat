using MaskedAIChat.Core.Models;

namespace MaskedAIChat.Core.Contracts.Services;

public interface IChatDataService
{

    string GetChatText();

    void SetChatText(string chatText);
}
