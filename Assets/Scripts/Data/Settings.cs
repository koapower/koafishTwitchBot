using System;

namespace Koapower.KoafishTwitchBot.Data
{
    [Serializable]
    public class Settings
    {
        public StringProperty channel_name = new StringProperty();
        public StringProperty osu_ingame_name = new StringProperty();
        public StringProperty downloadPath = new StringProperty();

        public StringProperty commandPrefix = new StringProperty("!");
    }
}
