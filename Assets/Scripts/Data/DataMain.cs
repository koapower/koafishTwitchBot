using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Koapower.KoafishTwitchBot.Data
{
    public class DataMain
    {
        public static string SaveFolder => Path.Combine(Application.dataPath, "Saves");
        public static string DownloadFolder => Path.Combine(Application.dataPath, "Download");

        public event Action onDataLoaded;
        //todo tables
        public Secret secret = new Secret();
        public Settings settings = new Settings();

        public async UniTask LoadAll() //todo 這邊的load的流程還可以再調整
        {
            ensureSaveFolderExists();

            //secret
            var path = Path.Combine(SaveFolder, "secrets.json");
            var loadedSecret = Load<Secret>(path);
            loadedSecret ??= new Secret();
            var inputReqs = new List<UI.InputBox.InputRequest>();
            if (string.IsNullOrEmpty(loadedSecret.client_id.value))
                inputReqs.Add(new UI.InputBox.TextInputRequest(loadedSecret.client_id, "Twitch bot client id", false));
            if (string.IsNullOrEmpty(loadedSecret.client_secret.value))
                inputReqs.Add(new UI.InputBox.TextInputRequest(loadedSecret.client_secret, "Twitch bot client secret", false));
            if (string.IsNullOrEmpty(loadedSecret.bot_access_token.value))
                inputReqs.Add(new UI.InputBox.TextInputRequest(loadedSecret.bot_access_token, "Bot access token", false));
            if (string.IsNullOrEmpty(loadedSecret.bot_refresh_token.value))
                inputReqs.Add(new UI.InputBox.TextInputRequest(loadedSecret.bot_refresh_token, "Bot refresh token", false));
            if (string.IsNullOrEmpty(loadedSecret.osu_app_client_id.value))
                inputReqs.Add(new UI.InputBox.TextInputRequest(loadedSecret.osu_app_client_id, "Osu app client id", false));
            if (string.IsNullOrEmpty(loadedSecret.osu_app_client_secret.value))
                inputReqs.Add(new UI.InputBox.TextInputRequest(loadedSecret.osu_app_client_secret, "Osu app client secret", false));
            if (string.IsNullOrEmpty(loadedSecret.osu_irc_bot_name.value))
                inputReqs.Add(new UI.InputBox.TextInputRequest(loadedSecret.osu_irc_bot_name, "Osu irc bot name", false));
            if (string.IsNullOrEmpty(loadedSecret.osu_irc_bot_password.value))
                inputReqs.Add(new UI.InputBox.TextInputRequest(loadedSecret.osu_irc_bot_password, "Osu irc bot password", false));
            if (inputReqs.Count > 0)
                await Main.UIManager.OpenInputBoxAsync(UI.InputBox.InputBoxType.OK, "Secrets", inputReqs);

            secret = loadedSecret;
            Save(secret, path);

            //settings
            path = Path.Combine(SaveFolder, "settings.json");
            var loadedSettings = Load<Settings>(path);
            if (loadedSettings != null)
                settings = loadedSettings;
            while (string.IsNullOrEmpty(settings.channel_name.value) || string.IsNullOrEmpty(settings.osu_ingame_name.value))
            {
                Debug.LogWarning("Both channel name and osu ingame name should not be empty! please fill in");
                inputReqs.Clear();
                if (string.IsNullOrEmpty(settings.channel_name.value))
                    inputReqs.Add(new UI.InputBox.TextInputRequest(settings.channel_name, "Twitch channel name", false));
                if (string.IsNullOrEmpty(settings.osu_ingame_name.value))
                    inputReqs.Add(new UI.InputBox.TextInputRequest(settings.osu_ingame_name, "Osu ingame name", false));

                await Main.UIManager.OpenInputBoxAsync(UI.InputBox.InputBoxType.OK, "User Settings", inputReqs);

                if (string.IsNullOrEmpty(settings.channel_name.value))
                    await Main.UIManager.OpenMessageBoxAsync(UI.MessageBox.MessageBoxType.OK, $"Twitch channel name is still empty, please fill in.");
                if (string.IsNullOrEmpty(settings.osu_ingame_name.value))
                    await Main.UIManager.OpenMessageBoxAsync(UI.MessageBox.MessageBoxType.OK, $"Osu ingame name is still empty, please fill in.");

                Save(settings, path);
            }
            if (string.IsNullOrEmpty(settings.downloadPath.value))
            {
                settings.downloadPath.value = DownloadFolder;
                Save(settings, path);
            }

            Debug.Log("Data loaded");
            onDataLoaded?.Invoke();
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
