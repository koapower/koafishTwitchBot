using Koapower.KoafishTwitchBot.Module.OsuWebApi;

namespace Koapower.KoafishTwitchBot.Module
{
    public class ModuleMain
    {
        private bool inited;
        public OsuHandler osuHandler { get; private set; }
        public OsuApiClient osuApiClient { get; private set; }

        public void Setup()
        {
            if (inited) return;

            inited = true;
            osuHandler = new OsuHandler();
            osuApiClient = new OsuApiClient();
        }
    }
}
