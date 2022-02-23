using System;

namespace Koapower.KoafishTwitchBot.Data
{
    [Serializable]
    public class Settings
    {
        public string channel_name = "";
        public string downloadPath = "";

        public string commandPrefix = "!";
    }
}
