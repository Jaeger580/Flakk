using UnityEditor;
using UnityEngine;
//#if UNITY_EDITOR

namespace GeneralUtility
{
    public static class Editor_Utility
    {
        static public void Ping(GameObject go)
        {
#if UNITY_EDITOR
            EditorGUIUtility.PingObject(go);
#endif
        }

        static public void ThrowWarning(string warning, object obj)
        {
            Debug.LogWarning($"{warning}\nSent by {obj}");
        }
    }
}