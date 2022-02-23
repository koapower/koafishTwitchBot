using System.Collections.Generic;

namespace Koapower.KoafishTwitchBot.Module.Message
{
    public class MessageManager
    {
        CommandHandler commandHandler = new CommandHandler();
        OsuHandler osuHandler = new OsuHandler();
        List<MessageHandler> handlers = new List<MessageHandler>();

        public MessageManager()
        {
            handlers.Add(commandHandler);
            handlers.Add(osuHandler);
        }

        public void OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            foreach (var h in handlers)
            {
                h.OnMessageRecieved(e);
            }
        }

    }
}
