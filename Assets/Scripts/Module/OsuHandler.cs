using Cysharp.Threading.Tasks;
using DragonFruit.Orbit.Api.Beatmaps.Entities;
using Koapower.KoafishTwitchBot.Module.OsuWebApi;
using System.Linq;
using System.Text.RegularExpressions;
using OsuBeatmapset = Koapower.KoafishTwitchBot.Module.OsuWebApi.OsuBeatmapset;

namespace Koapower.KoafishTwitchBot.Module
{
    public class OsuHandler
    {
        public readonly static Regex osuBeatmapUrl = new Regex("[http(s)*://]*osu.ppy.sh/beatmapsets/[0-9]+[(osu|#taiko|#fruits|#mania)/[0-9]+]*");
        private readonly static Regex numberParse = new Regex("[0-9]+");
        public async UniTask<(OsuBeatmapset, OsuBeatmap)> AddBeatmapToQueue(string url)
        {
            var matches = numberParse.Matches(url);
            var setId = 0u;
            var mapId = 0u;
            switch (matches.Count)
            {
                case 1:
                    uint.TryParse(matches[0].Value, out setId);
                    break;
                case 2:
                    uint.TryParse(matches[0].Value, out setId);
                    uint.TryParse(matches[1].Value, out mapId);
                    break;
                default:
                    break;
            }

            OsuBeatmapset set = null;
            OsuBeatmap map = null;
            if (mapId != 0)
            {
                set = await Main.Modules.osuApiClient.GetBeatmapset_fix(setId);
                map = set.Maps.FirstOrDefault(x => x.Id == mapId);
            }
            else if (setId != 0)
            {
                set = await Main.Modules.osuApiClient.GetBeatmapset_fix(setId);
            }
            if (set != null)
                Main.UIManager.beatmapQueue.Enqueue(set, map);

            return (set, map);
        }
    }
}
