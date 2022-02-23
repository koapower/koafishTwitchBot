using OsuMemoryDataProvider;
using OsuMemoryDataProvider.OsuMemoryModels;
using OsuMemoryDataProvider.OsuMemoryModels.Direct;

namespace Koapower.KoafishTwitchBot.Module.OsuDynamicData
{
    public class OsuDataProvider
    {
        //example usage please refer to https://github.com/Piotrekol/ProcessMemoryDataFinder/blob/master/StructuredOsuMemoryProviderTester/Form1.cs
        public StructuredOsuMemoryReader reader = StructuredOsuMemoryReader.Instance;
        public OsuBaseAddresses baseAddresses = new OsuBaseAddresses();

        public CurrentBeatmap ReadCurrentBeatmap()
        {
            var success = reader.TryRead(baseAddresses.Beatmap);
            return success ? baseAddresses.Beatmap : null;
        }
    }
}
