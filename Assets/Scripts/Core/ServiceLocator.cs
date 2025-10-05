using System;
using System.Collections.Generic;
using UnityEngine;

namespace FocusFounder.Core
{
    /// <summary>
    /// Simple service locator for dependency injection
    /// Provides global access to game services
    /// </summary>
    public class ServiceLocator : MonoBehaviour
    {
        private static ServiceLocator _instance;
        private readonly Dictionary<Type, object> _services = new();

        public static ServiceLocator Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<ServiceLocator>();
                    if (_instance == null)
                    {
                        var go = new GameObject("ServiceLocator");
                        _instance = go.AddComponent<ServiceLocator>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void Register<T>(T service) where T : class
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                Debug.LogWarning($"Service {type.Name} is already registered. Overwriting.");
            }
            _services[type] = service;
            Debug.Log($"Registered service: {type.Name}");
        }

        public T Get<T>() where T : class
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var service))
            {
                return service as T;
            }

            Debug.LogError($"Service {type.Name} not found!");
            return null;
        }

        public bool TryGet<T>(out T service) where T : class
        {
            service = Get<T>();
            return service != null;
        }

        public void Unregister<T>() where T : class
        {
            var type = typeof(T);
            if (_services.Remove(type))
            {
                Debug.Log($"Unregistered service: {type.Name}");
            }
        }

        public void Clear()
        {
            _services.Clear();
            Debug.Log("All services cleared");
        }
    }

    /// <summary>
    /// Static helper for easier service access
    /// </summary>
    public static class Services
    {
        public static T Get<T>() where T : class => ServiceLocator.Instance.Get<T>();
        public static void Register<T>(T service) where T : class => ServiceLocator.Instance.Register(service);
        public static bool TryGet<T>(out T service) where T : class => ServiceLocator.Instance.TryGet(out service);
    }
}