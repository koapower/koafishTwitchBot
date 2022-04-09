using Koapower.KoafishTwitchBot.Data;

namespace Koapower.KoafishTwitchBot.UI.InputBox
{
    public class TextInputRequest : InputRequest
    {
        public TextInputRequest(StringProperty property, string title, bool optional) : base(property, title, ContentType.TextField, optional)
        {
        }

    }
}
