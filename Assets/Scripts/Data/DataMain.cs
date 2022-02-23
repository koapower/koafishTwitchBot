using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Koapower.KoafishTwitchBot.Data
{
    public class DataMain
    {
        public static string SaveFolder => Path.Combine(Application.dataPath, "Saves");
        public static string DownloadFolder => Path.Combine(Application.dataPath, "Download");

        //todo tables
        public Secret secret = new Secret();
        public Settings settings = new Settings();

        public async UniTask LoadAll() //todo 這邊的load的流程還可以再調整
        {
            ensureSaveFolderExists();

            //secret
            var path = Path.Combine(SaveFolder, "secrets.json");
            var loadedSecret = Load<Secret>(path);
            if (loadedSecret == null)
            {
                var inputReqs = new List<UI.InputBox.InputRequest>()
                {
                    new UI.InputBox.InputRequest("Twitch bot client id", UI.InputBox.ContentType.TextField, false),
                    new UI.InputBox.InputRequest("Twitch bot client secret", UI.InputBox.ContentType.TextField, false),
                    new UI.InputBox.InputRequest("Bot access token", UI.InputBox.ContentType.TextField, false),
                    new UI.InputBox.InputRequest("Bot refresh token", UI.InputBox.ContentType.TextField, false),
                    new UI.InputBox.InputRequest("Osu app client id", UI.InputBox.ContentType.TextField, false),
                    new UI.InputBox.InputRequest("Osu app client secret", UI.InputBox.ContentType.TextField, false),
                };
                await Main.UIManager.OpenInputBoxAsync(UI.InputBox.InputBoxType.OK, "Secrets", inputReqs);

                secret.client_id = inputReqs[0].stringResult;
                secret.client_secret = inputReqs[1].stringResult;
                secret.bot_access_token = inputReqs[2].stringResult;
                secret.bot_refresh_token = inputReqs[3].stringResult;
                secret.osu_app_client_id = inputReqs[4].stringResult;
                secret.osu_app_client_secret = inputReqs[5].stringResult;

                Save(secret, path);
            }
            else
                secret = loadedSecret;

            //settings
            path = Path.Combine(SaveFolder, "settings.json");
            var loadedSettings = Load<Settings>(path);
            if (loadedSettings != null)
                settings = loadedSettings;
            while (string.IsNullOrEmpty(settings.channel_name))
            {
                Debug.LogWarning("Channel name should not be empty! please fill in");
                var inputReqs = new List<UI.InputBox.InputRequest>()
                {
                    new UI.InputBox.InputRequest("Twitch channel name", UI.InputBox.ContentType.TextField, false)
                };
                await Main.UIManager.OpenInputBoxAsync(UI.InputBox.InputBoxType.OK, "Twitch Informations", inputReqs);

                settings.channel_name = inputReqs[0].stringResult;
                if (string.IsNullOrEmpty(settings.channel_name))
                    await Main.UIManager.OpenMessageBoxAsync(UI.MessageBox.MessageBoxType.OK, $"Twitch channel name is still empty, please fill in.");
                else
                    Save(settings, path);
            }
            if (string.IsNullOrEmpty(settings.downloadPath))
            {
                settings.downloadPath = DownloadFolder;
                Save(settings, path);
            }

            Debug.Log("Data loaded");
        }

        public void SaveAll()
        {

        }

        private void ensureSaveFolderExists()
        {
            Directory.CreateDirectory(SaveFolder);
        }

        private T Load<T>(string path) where T : class
        {
            if (!File.Exists(path))
                return null;

            var raw = File.ReadAllText(path);
            var data = JsonUtility.FromJson<T>(raw);

            return data;
        }

        private void Save<T>(T data, string path)
        {
            var j = JsonUtility.ToJson(data);
            File.WriteAllText(path, j);
        }
    }
}
