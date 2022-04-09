using TwitchLib.Client.Models;
using TwitchLib.Unity;
using UnityEngine;

namespace Koapower.KoafishTwitchBot
{
    public class TwitchClient
    {
        public Client client;

        public void Initialize()
        {
            ConnectionCredentials credentials = new ConnectionCredentials("koafishbot", Main.Datas.secret.bot_access_token.value);
            client = new Client();
            client.Initialize(credentials, Main.Datas.settings.channel_name.value);

            //subscribe events here
            client.OnMessageReceived += Main.Modules.messageManager.OnMessageReceived;
            client.OnConnected += OnConnected;
            client.OnConnectionError += OnConnectionError;
            client.OnJoinedChannel += OnJoinedChannel;

            Debug.Log("Twitch client initialized");
        }

        public void Connect()
        {
            client.Connect();
            Debug.Log("Twitch client connect");
        }

        public void SendMessage(string channelName, string message)
        {
            client.SendMessage(channelName, message);
        }

        internal void Update(float deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                client.SendMessage(Main.Datas.settings.channel_name.value, "帽子給我好嗎~");
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                var currentBeatmap = Main.Modules.osuDataProvider.ReadCurrentBeatmap();
                if (currentBeatmap != null)
                    Debug.Log(JsonUtility.ToJson(currentBeatmap));
                else
                    Debug.Log("current beatmap is null");
            }
        }

        #region callbacks
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
