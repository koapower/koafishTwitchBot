using UnityEngine;
using UnityEngine.UI;

namespace Koapower.KoafishTwitchBot.UI.InputBox
{
    class ContentBox : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI title;
        public TMPro.TMP_InputField inputField;
        public Toggle toggle;

        public InputRequest request { get; private set; }

        public void Setup(InputRequest req)
        {
            request = req;

            this.title.text = req.title;
            inputField.gameObject.SetActive(false);
            toggle.gameObject.SetActive(false);
            switch (request.type)
            {
                case ContentType.UNKNOWN:
                    break;
                case ContentType.TextField:
                    inputField.gameObject.SetActive(true);
                    inputField.text = string.Empty;
                    break;
                case ContentType.Toggle:
                    toggle.gameObject.SetActive(true);
                    toggle.isOn = false;
                    break;
                default:
                    break;
            }
        }

        public void ResetUI()
        {
            request = null;
            inputField.text = string.Empty;
            toggle.isOn = false;
        }

        public void FinishInput()
        {
            if (request != null)
            {
                request.stringResult = inputField.text;
                request.boolResult = toggle.isOn;
            }
        }
    }
}
