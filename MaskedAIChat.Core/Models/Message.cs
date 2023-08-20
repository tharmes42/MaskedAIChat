using MaskedAIChat.Core.Contracts.Services;

namespace MaskedAIChat.Core.Models;
public class Message
{
    public string MsgText
    {
        get; private set;
    }
    public DateTime MsgDateTime
    {
        get; private set;
    }
    public string MsgChatRole
    {
        get; set;
    }

    public TransformerService TransformerService
    {
        get; set;
    }

    //Source is either "User" or "Bot"
    public Message(string text, DateTime dateTime, string chatRole, TransformerService transformerService)
    {
        MsgText = text;
        MsgDateTime = dateTime;
        MsgChatRole = chatRole;
        TransformerService = transformerService;
    }

    public override string ToString()
    {
        return MsgDateTime.ToString() + " " + MsgText;
    }

}
