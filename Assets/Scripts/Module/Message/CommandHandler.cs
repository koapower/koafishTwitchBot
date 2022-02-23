using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koapower.KoafishTwitchBot.Module.Message
{
    class CommandHandler : MessageHandler
    {
        static char[] commandSeparator = new char[] { ' ' };
        Dictionary<string, string> plainTextCommands = new Dictionary<string, string>();

        internal async override UniTask OnMessageRecieved(TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            var message = e.ChatMessage.Message;
            if (message.StartsWith(Main.Datas.settings.commandPrefix))
            {
                var body = message.Substring(1);
                var split = body.Split(commandSeparator);
                string[] param = null;
                if (split.Length > 1)
                {
                    param = new string[split.Length - 1];
                    Array.Copy(split, 1, param, 0, split.Length - 1);
                }
                string reply = null;
                switch (split[0]) //command, is case-sensitive
                {
                    case "np":
                        reply = GetOsuNowPlaying();
                        break;
                    default:
                        plainTextCommands.TryGetValue(split[0], out reply);
                        break;
                }
                if (!string.IsNullOrEmpty(reply))
                    Main.Client.SendMessage(e.ChatMessage.Channel, reply);
            }

            await UniTask.CompletedTask;
        }

        private string GetOsuNowPlaying()
        {
            var currentBeatmap = Main.Modules.osuDataProvider.ReadCurrentBeatmap();
            return currentBeatmap != null ? currentBeatmap.SetId < 0 ?
                $"{currentBeatmap.MapString} beatmap not uploaded" :
                $"{currentBeatmap.MapString} https://osu.ppy.sh/beatmapsets/{currentBeatmap.SetId}" :
                "Failed getting current beatmap";
        }
    }
}
