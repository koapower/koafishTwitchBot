using Cysharp.Threading.Tasks;
using DragonFruit.Orbit.Api.Beatmaps.Entities;
using Koapower.KoafishTwitchBot.Module.OsuWebApi;
using System.Linq;
using System.Text.RegularExpressions;
using OsuBeatmapset = Koapower.KoafishTwitchBot.Module.OsuWebApi.OsuBeatmapset;

namespace Koapower.KoafishTwitchBot.Module.Message
{
    class OsuHandler : MessageHandler
    {
        public readonly static Regex osuBeatmapUrl = new Regex("[http(s)*://]*osu.ppy.sh/beatmapsets/[0-9]+[(osu|#taiko|#fruits|#mania)/[0-9]+]*");
        private readonly static Regex numberParse = new Regex("[0-9]+");

        internal async override UniTask OnMessageRecieved(TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            var message = e.ChatMessage.Message;
            //Debug.Log($"{e.ChatMessage.UserId}: {message}");

            //check if it is an osu beatmap
            var matches = osuBeatmapUrl.Matches(message);
            //Debug.Log($"matchcount: {matches.Count}");
            for (int i = 0; i < matches.Count; i++)
            {
                var mapInfo = await AddBeatmapToQueue(matches[i].Value);
                var set = mapInfo.Item1;
                var map = mapInfo.Item2;
                sb.Clear();
                sb.Append("[Request by ").Append(e.ChatMessage.Username).Append("] ");
                if (set != null)
                {
                    sb.Append(set.ArtistUnicode).Append(" - ").Append(set.TitleUnicode);
                    if (map != null)
                        sb.Append(" [").Append(map.Name).Append("] by ").Append(set.Mapper).Append(" ★").Append(map.DifficultyRating).Append(" ").Append(map.TotalLength.ToString(@"mm\:ss"));
                    else
                        sb.Append(" by ").Append(set.Mapper);
                }
                else
                {
                    sb.Append("找不到這張圖");
                }

                Main.Client.SendMessage(e.ChatMessage.Channel, sb.ToString());
            }
        }

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
