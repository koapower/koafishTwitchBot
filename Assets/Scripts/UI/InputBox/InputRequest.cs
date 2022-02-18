namespace Koapower.KoafishTwitchBot.UI.InputBox
{
    public class InputRequest
    {
        public InputRequest(string title, ContentType type, bool optional)
        {
            this.title = title;
            this.type = type;
            this.optional = optional;
        }
        public string title;
        public ContentType type;
        public bool optional;

        public string stringResult;
        public bool boolResult;
    }
}
