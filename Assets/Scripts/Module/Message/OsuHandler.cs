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
        public readonly static Regex osuBeatmapSetUrl = new Regex("[http(s)*://]*osu.ppy.sh/(beatmapsets|s|beatmaps)/[0-9]+[(osu|#taiko|#fruits|#mania)/[0-9]+]*");
        public readonly static Regex osuBeatmapMapUrl = new Regex("[http(s)*://]*osu.ppy.sh/b/[0-9]+");
        private readonly static Regex numberParse = new Regex("[0-9]+");

        internal async override UniTask OnMessageRecieved(TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            var message = e.ChatMessage.Message;
            //Debug.Log($"{e.ChatMessage.UserId}: {message}");

            //check if it is an osu beatmap
            var matches = osuBeatmapSetUrl.Matches(message);
            for (int i = 0; i < matches.Count; i++)
            {
                var parsed = TryParseBeatmapSetIds(matches[i].Value, out var setId, out var mapId);
                if (!parsed) continue;

                var mapInfo = await AddBeatmapToQueue(setId, mapId);
                var set = mapInfo.Item1;
                var map = mapInfo.Item2;
                SendRecievedRequestMessage(set, map);
            }
            matches = osuBeatmapMapUrl.Matches(message);
            for (int i = 0; i < matches.Count; i++)
            {
                var parsed = TryParseBeatmapMapId(matches[i].Value, out var mapId);
                if (!parsed) continue;

                var mapInfo = await AddBeatmapToQueue(0, mapId);
                var set = mapInfo.Item1;
                var map = mapInfo.Item2;
                SendRecievedRequestMessage(set, map);
            }

            //sub method
            void SendRecievedRequestMessage(OsuBeatmapset set, OsuBeatmap map)
            {
                string twitchMessage, osuIrcMessage;
                sb.Clear();
                sb.Append("[Request by ").Append(e.ChatMessage.DisplayName).Append("] ");
                if (set != null)
                {
                    //twitch
                    sb.Append(set.ArtistUnicode).Append(" - ").Append(set.TitleUnicode);
                    if (map != null)
                        sb.Append(" [").Append(map.Name).Append("] by ").Append(set.Mapper).Append(", ★").Append(map.DifficultyRating).Append(", ").Append(map.TotalLength.ToString(@"mm\:ss"));
                    else
                        sb.Append(" by ").Append(set.Mapper);

                    twitchMessage = sb.ToString();
                    //irc
                    sb.Clear();
                    sb.Append(e.ChatMessage.DisplayName).Append(" > ");
                    map ??= set.Maps.LastOrDefault();
                    if (map != null)
                        sb.Append("[https://osu.ppy.sh/b/").Append(map.Id).Append(' ').Append(set.ArtistUnicode).Append(" - ").Append(set.TitleUnicode)
                            .Append(" [").Append(map.Name).Append("]] by ").Append(set.Mapper).Append(", ★").Append(map.DifficultyRating).Append(", ").Append(map.TotalLength.ToString(@"mm\:ss"));
                    else
                        sb.Append(set.ArtistUnicode).Append(" - ").Append(set.TitleUnicode).Append(" by ").Append(set.Mapper);
                    osuIrcMessage = sb.ToString();
                }
                else
                {
                    sb.Append("找不到這張圖");
                    twitchMessage = sb.ToString();
                    osuIrcMessage = null;
                }

                if (!string.IsNullOrEmpty(twitchMessage))
                    Main.Client.SendMessage(e.ChatMessage.Channel, twitchMessage);
                if (!string.IsNullOrEmpty(osuIrcMessage))
                    Main.Modules.osuIRCManager.SendMessageToOsuUser(osuIrcMessage);
            }
        }

        public async UniTask<(OsuBeatmapset, OsuBeatmap)> AddBeatmapToQueue(uint setId, uint mapId)
        {
            OsuBeatmapset set = null;
            OsuBeatmap map = null;
            if (setId != 0)
            {
                set = await Main.Modules.osuApiClient.GetBeatmapset_fix(setId);
                if (mapId != 0)
                    map = set.Maps.FirstOrDefault(x => x.Id == mapId);
            }
            else if (mapId != 0)
            {
                set = await Main.Modules.osuApiClient.GetBeatmapsetFromMap_fix(mapId);
                map = set.Maps.FirstOrDefault(x => x.Id == mapId);
            }
            if (set != null)
                Main.UIManager.beatmapQueue.Enqueue(set, map);

            return (set, map);
        }

        private bool TryParseBeatmapSetIds(string url, out uint setId, out uint mapId)
        {
            var matches = numberParse.Matches(url);
            setId = mapId = 0u;
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

            return setId != 0;
        }

        private bool TryParseBeatmapMapId(string url, out uint mapId)
        {
            var match = numberParse.Match(url);
            uint.TryParse(match.Value, out mapId);

            return mapId != 0;
        }
    }
}
