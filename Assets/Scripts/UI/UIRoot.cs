using UnityEngine;
using System.Collections;
using Koapower.KoafishTwitchBot.Common.Patterns;

namespace Koapower.KoafishTwitchBot.UI
{
    public class UIRoot : Singleton<UIRoot>
    {
        public T GetChildUIPage<T>(string gameObjectName) where T : UIPage
        {
            return transform.Find(gameObjectName)?.GetComponent<T>();
        }
    }
}
