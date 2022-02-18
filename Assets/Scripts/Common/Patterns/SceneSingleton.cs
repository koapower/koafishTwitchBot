using UnityEngine;

namespace Koapower.KoafishTwitchBot.Common.Patterns
{
    /// <summary>
    /// Be aware this will not prevent a non singleton constructor
    ///   such as `T myT = new T();`
    /// To prevent that, add `protected T () {}` to your singleton class.
    /// 
    /// As a note, this is made as MonoBehaviour because we need Coroutines.
    /// </summary>
    public class SceneSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        private static object _lock = new object();

        virtual protected void Awake()
        {
            if (_instance == null)
                _instance = this as T;
        }

        public static T Instance
        {
            get { return _instance; }
        }
    }
}
