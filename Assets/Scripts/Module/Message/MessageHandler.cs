using Cysharp.Threading.Tasks;

namespace Koapower.KoafishTwitchBot.Module.Message
{
    abstract class MessageHandler
    {
        protected static System.Text.StringBuilder sb = new System.Text.StringBuilder();
        internal abstract UniTask OnMessageRecieved(TwitchLib.Client.Events.OnMessageReceivedArgs e);
    }
}
