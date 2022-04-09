namespace Koapower.KoafishTwitchBot.Module.IRC.Message
{
    internal interface IMessage
    {
        string User { get; set; }
        string Message { get; set; }
    }
}
