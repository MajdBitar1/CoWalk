using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Singleton<T> : NetworkBehaviour where T : Component
{
    private static T _instance;

    public static T Instance
    {
        get 
        {
            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));
                if (_instance == null)
                {
                    SetupInstance();
                }
            }
            return _instance;
        }
    }

    public virtual void Awake()
    {
        RemoveDuplicate();
    }

    private static void SetupInstance()
    {
        _instance = (T)FindObjectOfType(typeof(T));

        if (_instance == null)
        {
            GameObject obj = new GameObject();
            obj.name = typeof(T).Name;
            _instance = obj.AddComponent<T>();
            DontDestroyOnLoad(obj);
        }
    }

    private void RemoveDuplicate()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
