using Koapower.KoafishTwitchBot.Data;

namespace Koapower.KoafishTwitchBot.UI.InputBox
{
    public class BoolInputRequest : InputRequest
    {
        public BoolInputRequest(BoolProperty property, string title, bool optional) : base(property, title, ContentType.Toggle, optional)
        {
        }

    }
}
