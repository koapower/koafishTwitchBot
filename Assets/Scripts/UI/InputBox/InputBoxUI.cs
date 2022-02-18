using Koapower.KoafishTwitchBot.Common.ObjectPool;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Koapower.KoafishTwitchBot.UI.InputBox
{
    public class InputBoxUI : UIPage
    {
        [SerializeField] TMPro.TextMeshProUGUI title;
        [SerializeField] ContentBox contentPrefab;
        [SerializeField] Button cancelButton;
        [SerializeField] Button okButton;

        private ObjectPool<ContentBox> contentPool;
        private List<ContentBox> contents = new List<ContentBox>();

        private Request currentRequest;

        public void Open(Request req)
        {
            if (req != null)
            {
                ResetUI();

                this.title.text = req.title;
                foreach (var r in req.reqs)
                {
                    var c = contentPool.Get();
                    c.Setup(r);
                    c.gameObject.SetActive(true);
                }

                cancelButton.gameObject.SetActive(false);
                okButton.gameObject.SetActive(false);
                switch (req.type)
                {
                    case InputBoxType.OKCANCEL:
                        cancelButton.gameObject.SetActive(true);
                        okButton.gameObject.SetActive(true);
                        break;
                    case InputBoxType.OK:
                        okButton.gameObject.SetActive(true);
                        break;
                    default:
                        break;
                }
            }
        }

        public void Close()
        {
            currentRequest = null;
            gameObject.SetActive(false);
        }

        public void ResetUI()
        {
            foreach (var item in contents)
            {
                item.ResetUI();
                item.gameObject.SetActive(false);
                contentPool.Return(item);
            }
            contents.Clear();
        }

        private void Awake()
        {
            contentPool = new ObjectPool<ContentBox>(() => GameObject.Instantiate(contentPrefab, contentPrefab.transform.parent));
            contentPrefab.gameObject.SetActive(false);
            cancelButton.onClick.AddListener(OnCancel);
            okButton.onClick.AddListener(OnOk);
        }

        private void OnCancel()
        {
            if (currentRequest != null)
            {
                currentRequest.response = InputBoxResponse.CANCEL;
                currentRequest.done = true;
            }

            Close();
        }

        private void OnOk()
        {
            if (currentRequest != null)
            {
                foreach (var item in contents)
                {
                    item.FinishInput();
                }

                currentRequest.response = InputBoxResponse.OK;
                currentRequest.done = true;
            }

            Close();
        }
    }
}
