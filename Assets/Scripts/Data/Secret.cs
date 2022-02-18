using System;

namespace Koapower.KoafishTwitchBot.Data
{
    [Serializable]
    public class Secret
    {
        //twitch bot
        public string client_id = "";
        public string client_secret = "";
        public string bot_access_token = "";
        public string bot_refresh_token = "";
        //osu app
        public string osu_app_client_id = "";
        public string osu_app_client_secret = "";
    }
}
