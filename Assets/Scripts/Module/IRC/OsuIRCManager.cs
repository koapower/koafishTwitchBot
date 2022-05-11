using Koapower.KoafishTwitchBot.Module.IRC.Message;
using UnityEngine;

namespace Koapower.KoafishTwitchBot.Module.IRC
{
    public class OsuIRCManager
    {
        public OsuIRCManager()
        {
            Main.Datas.onDataLoaded += OnDataLoaded;
            osuIRCClient.onMessageRecieved += OnMessageRecieved;
        }
        public OsuIRCClient osuIRCClient { get; private set; } = new OsuIRCClient();

        private void OnDataLoaded()
        {
            osuIRCClient.SetUserInfo(
                Main.Datas.secret.osu_irc_bot_name.value,
                Main.Datas.secret.osu_irc_bot_password.value,
                Main.Datas.secret.osu_irc_bot_name.value
                );

            osuIRCClient.Restart();
            SendMessageToOsuUser("Koahi");
        }

        private void OnMessageRecieved(IMessage msg)
        {
            Debug.Log($"[OsuIRC] Recieved - {msg.User}: {msg.Message}");
        }

        public void SendMessageToOsuUser(string message)
        {
            osuIRCClient.SendPrivateMessage(new IRCMessage(Main.Datas.settings.osu_ingame_name.value, message));
        }
    }
}
