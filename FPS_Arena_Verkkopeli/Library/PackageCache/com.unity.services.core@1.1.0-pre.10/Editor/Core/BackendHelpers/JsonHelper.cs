using System;
using UnityEngine;

namespace Unity.Services.Core.Editor
{
    static class JsonHelper
    {
        internal static bool TryJsonDeserialize<T>(string json, ref T dest)
        {
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    dest = JsonUtility.FromJson<T>(json);
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }
            }

            return false;
        }
    }
}
