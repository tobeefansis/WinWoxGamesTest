using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    private static ServiceLocator _instance;

    public static ServiceLocator Instance
    {
        get
        {
            if (_instance == null)
                new GameObject("[ServiceLocator]").AddComponent<ServiceLocator>();
            return _instance;
        }
    }

    private readonly Dictionary<Type, object> _services = new();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    public void Register<T>(T service) where T : class
        => _services[typeof(T)] = service;

    public void Unregister<T>() where T : class
        => _services.Remove(typeof(T));

    public T Get<T>() where T : class
        => _services.TryGetValue(typeof(T), out var s) ? s as T : null;
}
