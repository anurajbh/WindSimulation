using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null && !_isShuttingDown)
            {
                // Try to find an existing instance in the scene
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    Debug.LogError($"No instance of {typeof(T)} found in the scene.");
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            // If no instance exists, set this as the singleton instance
            _instance = this as T;
        }
        else if (_instance != this)
        {
            // Destroy duplicate instances
            Debug.LogWarning($"Duplicate instance of {typeof(T)} detected. Destroying {gameObject.name}.");
            Destroy(gameObject);
        }
    }
    private static bool _isShuttingDown = false;

    private void OnApplicationQuit()
    {
        _isShuttingDown = true;
    }

    private void OnDestroy()
    {
        _isShuttingDown = true;
    }
}
