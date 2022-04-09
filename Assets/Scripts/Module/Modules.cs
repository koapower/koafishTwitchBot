using Koapower.KoafishTwitchBot.Module.IRC;
using Koapower.KoafishTwitchBot.Module.Message;
using Koapower.KoafishTwitchBot.Module.OsuDynamicData;
using Koapower.KoafishTwitchBot.Module.OsuWebApi;
using OsuMemoryDataProvider;
using OsuMemoryDataProvider.OsuMemoryModels;

namespace Koapower.KoafishTwitchBot.Module
{
    public class ModuleMain
    {
        private bool inited;
        public MessageManager messageManager { get; private set; }
        public OsuApiClient osuApiClient { get; private set; }
        public OsuDataProvider osuDataProvider { get; private set; }
        public OsuIRCManager osuIRCManager { get; private set; }

        public void Setup()
        {
            if (inited) return;

            inited = true;
            messageManager = new MessageManager();
            osuApiClient = new OsuApiClient();
            osuDataProvider = new OsuDataProvider();
            osuIRCManager = new OsuIRCManager();
        }
    }
}
