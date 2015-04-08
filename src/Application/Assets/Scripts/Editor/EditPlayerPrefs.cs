using UnityEngine;
using UnityEditor;
using System.Collections;

public class EditPlayerPrefs : EditorWindow
{

    class PrefInfo
    {
        public string Key;
        public string Type;
    }

    // 表示対象のキーとタイプ
    PrefInfo[] keys = new PrefInfo[] {
        new PrefInfo { Key = "CachePath", Type = "string" },
        new PrefInfo { Key = "DBPath", Type = "string" },
    };

    [MenuItem("Window/Edit PlayerPrefs")]
    public static void ShowWindow()
    {
        EditorWindow win = EditorWindow.GetWindow<EditPlayerPrefs>();
        win.title = "PlayerPrefs";
    }

    void OnGUI()
    {
        foreach (var keyInfo in keys)
        {
            switch (keyInfo.Type)
            {
                case "string":
                    var originalString = PlayerPrefs.GetString(keyInfo.Key);
                    var currentString = EditorGUILayout.TextField(keyInfo.Key, originalString);
                    if (currentString != originalString)
                    {
                        PlayerPrefs.SetString(keyInfo.Key, currentString);
                        Debug.Log("*** update " + originalString + " => " + currentString);
                    }
                    break;

                case "int":
                    var originalInteger = PlayerPrefs.GetInt(keyInfo.Key);
                    var currentInteger = System.Convert.ToInt32(EditorGUILayout.TextField(keyInfo.Key, originalInteger.ToString()));
                    if (currentInteger != originalInteger)
                    {
                        PlayerPrefs.SetInt(keyInfo.Key, currentInteger);
                        Debug.Log("*** update " + originalInteger.ToString() + " => " + currentInteger.ToString());
                    }
                    break;

                default:
                    throw new System.Exception("Unsupported pref type : " + keyInfo.Type);
            }
        }
    }
}