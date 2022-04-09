using DragonFruit.Data.Serializers.Newtonsoft;
using DragonFruit.Orbit.Api;
using DragonFruit.Orbit.Api.Auth;
using DragonFruit.Orbit.Api.Auth.Extensions;
using System.IO;
using UnityEngine;

namespace Koapower.KoafishTwitchBot.Module.OsuWebApi
{
    public class OsuApiClient : OrbitClient
    {
        private string _clientId;
        protected override string ClientId => _clientId;
        private string _clientSecret;
        protected override string ClientSecret => _clientSecret;

        public void LoadSecret()
        {
            _clientId = Main.Datas.secret.osu_app_client_id.value;
            _clientSecret = Main.Datas.secret.osu_app_client_secret.value;
        }

        protected override OsuAuthToken GetToken()
        {
            //in here you need to write a method for getting an osu! access token that is **still valid**. This is called if there is no token in a private field or the exising token is expired

            //for a server you might want to use this.Perform(new OsuSessionCredentialRequest())
            //for a client you might want to contact your server with the user's refresh token and request a new access key.

            //in any scenario, you should **write the result to a file and check it first**, Orbit converts the "expires_in" to a datetime, so you can store it in a file
            var filePath = Path.Combine(Application.dataPath, "osuAuthToken.json");
            OsuAuthToken token = FileServices.ReadFileOrDefault<OsuAuthToken>(filePath);

            if (token?.Expired != false)
            {
                if (string.IsNullOrEmpty(ClientId) || string.IsNullOrEmpty(ClientSecret))
                {
                    LoadSecret();
                    if (string.IsNullOrEmpty(ClientId) || string.IsNullOrEmpty(ClientSecret))
                        Debug.LogError("Client Id/Secret Missing");
                }

                token = this.GetSessionToken();

                FileServices.WriteFile(filePath, token);
            }

            return token;
        }
    }
}
