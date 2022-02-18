using UnityEngine.UI;

namespace Koapower.KoafishTwitchBot.UI.MessageBox
{
    public class MessageBoxUI : UIPage
    {
        public TMPro.TextMeshProUGUI text;
        public Button cancelButton;
        public Button okButton;

        private Request currentRequest;

        public void Open(Request req)
        {
            if(req != null)
            {
                currentRequest = req;
                gameObject.SetActive(true);
                text.text = req.message;
            }
        }

        public void Close()
        {
            currentRequest = null;
            gameObject.SetActive(false);
        }

        private void Awake()
        {
            cancelButton.onClick.AddListener(OnCancel);
            okButton.onClick.AddListener(OnOk);
        }

        private void OnCancel()
        {
            if (currentRequest != null)
            {
                currentRequest.response = MessageBoxResponse.CANCEL;
                currentRequest.done = true;
            }

            Close();
        }

        private void OnOk()
        {
            if (currentRequest != null)
            {
                currentRequest.response = MessageBoxResponse.OK;
                currentRequest.done = true;
            }

            Close();
        }
    }
}
