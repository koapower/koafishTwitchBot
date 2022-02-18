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

        public async UniTask LoadAll()
        {
            //secret比較特別要玩家另外從記事本輸入
            var path = Path.Combine(SaveFolder, "secrets.json");
            if (!File.Exists(path))
            {
                Save(secret, path);
                await Main.UIManager.OpenMessageBoxAsync(UI.MessageBox.MessageBoxType.OK, $"因為這些設定比較私人，請用記事本打開後輸入bot相關的資訊並儲存");
                Application.OpenURL(path);
                await Main.UIManager.OpenMessageBoxAsync(UI.MessageBox.MessageBoxType.OK, $"請確認已輸入bot相關的資訊並儲存，好了再按OK");
            }
            Save(secret, path);

            //settings
            path = Path.Combine(SaveFolder, "settings.json");
            Load(settings, path);
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
        }

        public void SaveAll()
        {

        }

        private void Load<T>(T data, string path)
        {
            if (!File.Exists(path))
            {
                Save(data, path);
            }
            var raw = File.ReadAllText(path);
            data = JsonUtility.FromJson<T>(raw);
        }

        private void Save<T>(T data, string path)
        {
            var j = JsonUtility.ToJson(data);
            File.WriteAllText(path, j);
        }
    }
}
