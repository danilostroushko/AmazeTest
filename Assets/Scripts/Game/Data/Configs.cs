using System;
using System.Collections.Generic;
using UnityEngine;

public static class Configs
{
    private static readonly Dictionary<Type, ScriptableObject> _data = new Dictionary<Type, ScriptableObject>();

    public static T Get<T>() where T : ScriptableObject
    {
        Type type = typeof(T);

        if (!_data.TryGetValue(type, out ScriptableObject config))
        {
            config = Resources.Load<ScriptableObject>($"Game/{type.Name}");

            _data.Add(type, config);
        }

        return config as T;
    }
}