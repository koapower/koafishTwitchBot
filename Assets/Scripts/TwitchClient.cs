using Cysharp.Threading.Tasks;
using Koapower.KoafishTwitchBot.Module;
using TwitchLib.Client.Models;
using TwitchLib.Unity;
using UnityEngine;

namespace Koapower.KoafishTwitchBot
{
    public class TwitchClient
    {
        private static System.Text.StringBuilder sb = new System.Text.StringBuilder();
        public Client client;
        private string channel_name = "koapower";

        public void Initialize()
        {
            ConnectionCredentials credentials = new ConnectionCredentials("koafishbot", Main.Datas.secret.bot_access_token);
            client = new Client();
            client.Initialize(credentials, channel_name);

            //subscribe events here
            client.OnMessageReceived += OnMessageReceived;
            client.OnConnected += OnConnected;
            client.OnConnectionError += OnConnectionError;
            client.OnJoinedChannel += OnJoinedChannel;
        }

        public void Connect()
        {
            client.Connect();
        }

        internal void Update(float deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                client.SendMessage(channel_name, "帽子給我好嗎~");
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
            }
        }

        #region callbacks
        private async void OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            var message = e.ChatMessage.Message;
            //Debug.Log($"{e.ChatMessage.UserId}: {message}");

            //check if it is an osu beatmap
            var matches = OsuHandler.osuBeatmapUrl.Matches(message);
            //Debug.Log($"matchcount: {matches.Count}");
            for (int i = 0; i < matches.Count; i++)
            {
                var mapInfo = await Main.Modules.osuHandler.AddBeatmapToQueue(matches[i].Value);
                var set = mapInfo.Item1;
                var map = mapInfo.Item2;
                sb.Clear();
                sb.Append("[Request by ").Append(e.ChatMessage.Username).Append("] ");
                if (set != null)
                {
                    sb.Append(set.ArtistUnicode).Append(" - ").Append(set.TitleUnicode);
                    if (map != null)
                    {
                        sb.Append(" [").Append(map.Name).Append("] ★").Append(map.DifficultyRating).Append(" ").Append(map.TotalLength.ToString());
                    }
                    sb.Append(" by ").Append(set.Mapper);
                }
                else
                {
                    sb.Append("找不到這張圖");
                }

                client.SendMessage(e.ChatMessage.Channel, sb.ToString());
            }

        }

        private void OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
        {
            Debug.Log($"Connected to {e.AutoJoinChannel}");
        }

        private void OnConnectionError(object sender, TwitchLib.Client.Events.OnConnectionErrorArgs e)
        {
            Debug.LogError($"Failed connecting to channel. {e.Error.Message}");
        }

        private void OnJoinedChannel(object sender, TwitchLib.Client.Events.OnJoinedChannelArgs e)
        {
            Debug.Log($"Joined channel {e.Channel}");
            client.SendMessage(e.Channel, "前來學習 StinkyCheese");
        }
        #endregion
    }
}
