using FocusFounder.Services;
using UnityEngine;

namespace FocusFounder.Core
{
    // Singleton<T> should only store a reference to T, not Singleton<T>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<T>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject(typeof(T).Name);
                        instance = obj.AddComponent<T>();
                        Debug.Log(typeof(T).Name + "Created");
                    }
                }
                return instance;
            }
        }
    }

    public static class SingletonExtensions
    {
        public static T GetSingletonInstance<T>(this T instance) where T : MonoBehaviour
        {

            T singletonInstance = Singleton<T>.Instance;
            instance = singletonInstance;
            return instance;
        }
    }
}