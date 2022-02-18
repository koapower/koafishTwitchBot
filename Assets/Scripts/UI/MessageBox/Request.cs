namespace Koapower.KoafishTwitchBot.UI.MessageBox
{
    public class Request
    {
        public Request(MessageBoxType type, string message)
        {
            this.type = type;
            this.message = message;
        }
        public readonly MessageBoxType type;
        public readonly string message;

        public MessageBoxResponse response = MessageBoxResponse.None;
        public bool done = false;
    }
}
