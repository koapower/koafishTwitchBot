using System.Collections.Generic;

namespace Koapower.KoafishTwitchBot.UI.InputBox
{
    public class Request
    {
        public Request(InputBoxType type, string title, List<InputRequest> reqs)
        {
            this.type = type;
            this.title = title;
            this.reqs = reqs;
        }
        public readonly InputBoxType type;
        public readonly string title;
        public readonly List<InputRequest> reqs;

        public InputBoxResponse response = InputBoxResponse.None;
        public bool done = false;
    }
}
