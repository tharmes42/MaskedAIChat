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

    //Source is either "User" or "Bot"
    public Message(string text, DateTime dateTime, string chatRole)
    {
        MsgText = text;
        MsgDateTime = dateTime;
        MsgChatRole = chatRole;
    }

    public override string ToString()
    {
        return MsgDateTime.ToString() + " " + MsgText;
    }

}
