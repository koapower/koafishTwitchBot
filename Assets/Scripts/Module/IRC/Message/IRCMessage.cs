namespace Koapower.KoafishTwitchBot.Module.IRC.Message
{
    public struct IRCMessage : IMessage
    {
        public string User { get; set; }
        public string Message { get; set; }

        public IRCMessage(string user, string message)
        {
            User = user;
            Message = message;
        }
    }
}
