using Cysharp.Threading.Tasks;
using Koapower.KoafishTwitchBot.UI.BeatmapQueue;
using Koapower.KoafishTwitchBot.UI.InputBox;
using Koapower.KoafishTwitchBot.UI.MessageBox;
using System.Collections.Generic;

namespace Koapower.KoafishTwitchBot.UI
{
    public class UIManager
    {
        public MessageBoxUI messageBox;
        public BeatmapQueueUI beatmapQueue;
        public InputBoxUI inputBox;

        private List<UIPage> uiPages = new List<UIPage>();

        public void Setup()
        {
            messageBox = UIRoot.Instance.GetChildUIPage<MessageBoxUI>("MessageBox");
            uiPages.Add(messageBox);
            beatmapQueue = UIRoot.Instance.GetChildUIPage<BeatmapQueueUI>("BeatmapQueue");
            uiPages.Add(beatmapQueue);
            inputBox = UIRoot.Instance.GetChildUIPage<InputBoxUI>("InputBox");
            uiPages.Add(inputBox);
            foreach (var page in uiPages)
            {
                page.gameObject.SetActive(false);
            }
            //先開這個
            beatmapQueue.gameObject.SetActive(true);
        }

        public async UniTask<MessageBoxResponse> OpenMessageBoxAsync(MessageBoxType type, string message)
        {
            var req = new MessageBox.Request(type, message);
            messageBox.Open(req);
            await UniTask.WaitUntil(() => req.done);

            return req.response;
        }

        public async UniTask<InputBoxResponse> OpenInputBoxAsync(InputBoxType type, string title, List<InputRequest> reqs)
        {
            var req = new InputBox.Request(type, title, reqs);
            inputBox.Open(req);
            await UniTask.WaitUntil(() => req.done);

            return req.response;
        }
    }
}
