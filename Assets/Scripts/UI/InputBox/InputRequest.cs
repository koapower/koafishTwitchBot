using Koapower.KoafishTwitchBot.Data;

namespace Koapower.KoafishTwitchBot.UI.InputBox
{
    public abstract class InputRequest
    {
        public InputRequest(Property property, string title, ContentType type, bool optional)
        {
            this.property = property;
            this.title = title;
            this.type = type;
            this.optional = optional;
        }
        public Property property;
        public string title;
        public readonly ContentType type;
        public bool optional;
    }
}
