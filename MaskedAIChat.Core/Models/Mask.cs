namespace MaskedAIChat.Core.Models;

// Model for the MaskDataService. 
public class Mask
{
    public long MaskID
    {
        get; set;
    }

    public string UnmaskedText
    {
        get; set;
    }

    public string MaskedText
    {
        get; set;
    }

    public string ShortDescription => $"Mask ID: {MaskID}";

    public override string ToString() => $"{MaskedText}";
}
