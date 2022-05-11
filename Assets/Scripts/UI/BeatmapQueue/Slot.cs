using Cysharp.Threading.Tasks;
using DragonFruit.Orbit.Api.Beatmaps.Entities;
using System;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using OsuBeatmapset = Koapower.KoafishTwitchBot.Module.OsuWebApi.OsuBeatmapset;

namespace Koapower.KoafishTwitchBot.UI.BeatmapQueue
{
    class Slot : MonoBehaviour
    {
        public Image bg;
        public Image progressFill;
        public Image successFill;
        public Image failedFill;
        public TMPro.TextMeshProUGUI artist;
        public TMPro.TextMeshProUGUI title;
        public TMPro.TextMeshProUGUI mapper;
        public GameObject mapInfoRoot;
        public TMPro.TextMeshProUGUI diffName;
        public TMPro.TextMeshProUGUI starRating;
        public Button fullButton;
        public Button closeButton;

        BeatmapQueueUI mapQ;
        public SlotPhase phase { get; private set; } = SlotPhase.Reset;
        private OsuBeatmapset mapset;
        private OsuBeatmap map;
        private string savePath;

        public void Setup(BeatmapQueueUI Q, OsuBeatmapset mapset, OsuBeatmap map = null, bool autoDownload = true)
        {
            phase = SlotPhase.Setup;
            this.mapQ = Q;
            this.mapset = mapset;
            this.map = map;
            this.savePath = Path.Combine(Main.Datas.settings.downloadPath.value, $"{mapset.Id}_autodl.osz");
            Directory.CreateDirectory(Main.Datas.settings.downloadPath.value);

            artist.text = mapset.Artist;
            title.text = mapset.Title;
            mapper.text = mapset.Mapper;
            if (map != null)
            {
                mapInfoRoot.SetActive(true);
                diffName.text = map.Name;
                starRating.text = map.DifficultyRating.ToString();
            }
            else
                mapInfoRoot.SetActive(false);

            //load bg image
            LoadBg().Forget();

            phase = SlotPhase.WaitForDownload;
            if (autoDownload)
                DownloadMapset().Forget();
        }

        public void ResetUI()
        {
            phase = SlotPhase.Reset;
            progressFill.fillAmount = 0;
            successFill.gameObject.SetActive(false);
            failedFill.gameObject.SetActive(false);
            mapset = null;
            map = null;
            savePath = null;
        }

        private void Awake()
        {
            fullButton.onClick.AddListener(OnFullButtonClicked);
            closeButton.onClick.AddListener(OnCloseButtonClicked);
            ResetUI();
        }

        private void OnFullButtonClicked()
        {
            switch (phase)
            {
                case SlotPhase.WaitForDownload:
                    DownloadMapset().Forget();
                    break;
                case SlotPhase.DownloadFinished:
                    OpenDownloadedFile();
                    mapQ.CloseSlot(this);
                    break;
                case SlotPhase.DownloadFailed:
                    OpenBeatmapUrl();
                    mapQ.CloseSlot(this);
                    break;
                default:
                    break;
            }
        }

        private void OnCloseButtonClicked()
        {
            phase = SlotPhase.Canceled;
            mapQ.CloseSlot(this);
        }

        private async UniTask LoadBg()
        {
            Debug.Log($"LoadBg {mapset.Id}");
            //使用slimcover
            var coverUrl = $"https://assets.ppy.sh/beatmaps/{mapset.Id}/covers/slimcover.jpg";
            var req = UnityWebRequestTexture.GetTexture(coverUrl);
            try
            {
                await req.SendWebRequest();
                var texture = DownloadHandlerTexture.GetContent(req);
                bg.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            req.Dispose();

            //其他備選
            //https://assets.ppy.sh/beatmaps/1034260/covers/cover.jpg
            //https://assets.ppy.sh/beatmaps/1034260/covers/cover@2x.jpg
            //https://assets.ppy.sh/beatmaps/1034260/covers/card@2x.jpg
            //https://assets.ppy.sh/beatmaps/1034260/covers/list.jpg
            //https://assets.ppy.sh/beatmaps/1034260/covers/slimcover@2x.jpg
        }

        private async UniTask DownloadMapset()
        {
            Debug.Log($"DownloadMapset {mapset.Id}");
            if (phase >= SlotPhase.Downloading) return;
            phase = SlotPhase.Downloading;
            progressFill.fillAmount = 0;
            successFill.gameObject.SetActive(false);
            failedFill.gameObject.SetActive(false);
            //官網需要用帳號才能下載，有人是複製自己瀏覽器的cookies去下載，先用鏡像站試試
            var downloadUrl = $"https://beatconnect.io/b/{mapset.Id}";
            var dlSuccess = await TryDownload();
            if (!dlSuccess)
            {
                downloadUrl = $"https://api.chimu.moe/v1/download/{mapset.Id}";
                dlSuccess = await TryDownload();
            }
            //能試的下載點都試完之後再決定是否要讓使用者手動開網頁載
            if (dlSuccess)
            {
                phase = SlotPhase.DownloadFinished;
                progressFill.fillAmount = 0;
                successFill.gameObject.SetActive(true);
                Debug.Log($"DownloadFinish {mapset.Id}");
            }
            else
            {
                progressFill.fillAmount = 0;
                failedFill.gameObject.SetActive(true);
                phase = SlotPhase.DownloadFailed;
            }

            async UniTask<bool> TryDownload()
            {
                try
                {
                    using (var wc = new WebClient())
                    {
                        wc.DownloadProgressChanged += (sender, e) =>
                        {
                            //Debug.Log($"Downloading {mapset.Id} {e.ProgressPercentage} {e.BytesReceived}/{e.TotalBytesToReceive}");
                            progressFill.fillAmount = (float)e.BytesReceived / e.TotalBytesToReceive;
                        };

                        await wc.DownloadFileTaskAsync(downloadUrl, savePath);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                    return false;
                }

                return true;
            }
        }

        private void OpenDownloadedFile()
        {
            Debug.Log($"OpenDownloadedFile {mapset.Id}");
            phase = SlotPhase.FileOpened;
            progressFill.fillAmount = 0;
            Application.OpenURL(savePath);
        }

        private void OpenBeatmapUrl()
        {
            Application.OpenURL($"https://osu.ppy.sh/beatmapsets/{mapset.Id}");
        }
    }
}
