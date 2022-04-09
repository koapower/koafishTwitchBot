namespace Koapower.KoafishTwitchBot.Module.IRC
{
    public class OsuIRCClient : IRCClient
    {
        public OsuIRCClient() : base("irc.ppy.sh", 6667)
        {
        }
    }
}
