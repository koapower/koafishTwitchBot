using Koapower.KoafishTwitchBot.Common.Patterns;
using Koapower.KoafishTwitchBot.Data;
using Koapower.KoafishTwitchBot.Module;
using Koapower.KoafishTwitchBot.UI;
using System.IO;
using UnityEngine;

namespace Koapower.KoafishTwitchBot
{
    public class Main : Singleton<Main>
    {
        TwitchClient client = new TwitchClient();
        public static TwitchClient Client => Instance.client;

        ModuleMain modules = new ModuleMain();
        public static ModuleMain Modules => Instance.modules;

        UIManager uiManager = new UIManager();
        public static UIManager UIManager => Instance.uiManager;

        DataMain datas = new DataMain();
        public static DataMain Datas => Instance.datas;

        private async void Start()
        {
            Application.runInBackground = true;

            //先load ui，modules就可以用ui的東西
            uiManager.Setup();
            modules.Setup();

            await datas.LoadAll();

            client.Initialize();
            client.Connect();
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            client.Update(deltaTime);
        }
    }
}
