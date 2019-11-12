using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// デバッグログ用クラス
/// </summary>
public class AdDebug : MonoBehaviour
{

    public void Log(string text)
    {
#if UNITY_EDITOR
        Debug.Log(text);
#endif
    }

    public void Log(string text, Color color)
    {
        // Color型の値を16進数の文字列に
        string stringColor = ColorUtility.ToHtmlStringRGB(color);
        this.Log("<color=#" + stringColor + ">" + text + "</color>");
    }

    public void Log(string text, int size)
    {
        this.Log("<size=" + size.ToString() + ">" + text + "</size>");
    }

    public void Log(string text, Color color, int size)
    {
        this.Log("<size=" + size.ToString() + ">" + text + "</size>", color);
    }

    public void Log(string text, bool Bold)
    {
        this.Log("<b>" + text + "</b>");
    }

    public void Log(string text, Color color, bool Bold)
    {
        // Color型の値を16進数の文字列に
        string stringColor = ColorUtility.ToHtmlStringRGB(color);
        this.Log("<color=#" + stringColor + ">" + "<b>" + text + "</b>" + "</color>");
    }

    public void Log(string text, int size, bool Bold)
    {
        this.Log("<size=" + size.ToString() + ">" + "<b>" + text + "</b>" + "</size>");
    }

    public void Log(string text, Color color, int size, bool Bold)
    {
        this.Log("<size=" + size.ToString() + ">" + "<b>" + text + "</b>" + "</size>", color);
    }


    #region Singleton

    static AdDebug _instance;

    public static AdDebug instance
    {
        get
        {
            if (_instance == null)
            {
                var previous = FindObjectOfType(typeof(AdDebug));
                if (previous)
                {
                    Debug.LogWarning("Initialized twice. Don't use MidiBridge in the scene hierarchy.");
                    _instance = (AdDebug)previous;
                }
                else
                {
                    var go = new GameObject("Debug");
                    _instance = go.AddComponent<AdDebug>();
                    DontDestroyOnLoad(go);
                    go.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            return _instance;
        }
    }

    #endregion
}