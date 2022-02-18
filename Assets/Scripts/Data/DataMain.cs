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

        //todo tables
        public Secret secret = new Secret();
        public Settings settings = new Settings();

        public async UniTask LoadAll() //todo 這邊的load的流程還可以再調整
        {
            ensureSaveFolderExists();

            //secret比較特別要玩家另外從記事本輸入
            var path = Path.Combine(SaveFolder, "secrets.json");
            if (!File.Exists(path))
            {
                Save(secret, path);
                Application.OpenURL(path);
                await Main.UIManager.OpenMessageBoxAsync(UI.MessageBox.MessageBoxType.OK, $"Please edit the secret file by any text editor and save it manually. After editing, press ok to continue.");
            }
            secret = Load<Secret>(path);
            if(secret == null)
            {
                await Main.UIManager.OpenMessageBoxAsync(UI.MessageBox.MessageBoxType.OK, $"Secret file does not exist!");
                Application.Quit();
                return;
            }

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
