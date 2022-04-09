using System;

namespace Koapower.KoafishTwitchBot.Data
{
    [Serializable]
    public class Secret
    {
        //twitch bot
        public StringProperty client_id = new StringProperty();
        public StringProperty client_secret = new StringProperty();
        public StringProperty bot_access_token = new StringProperty();
        public StringProperty bot_refresh_token = new StringProperty();
        //osu app
        public StringProperty osu_app_client_id = new StringProperty();
        public StringProperty osu_app_client_secret = new StringProperty();
        //osu irc bot
        public StringProperty osu_irc_bot_name = new StringProperty();
        public StringProperty osu_irc_bot_password = new StringProperty();
    }
}
