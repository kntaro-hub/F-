using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

/// <summary>
/// デバッグログ用クラス
/// </summary>
public class AdDebug : MonoBehaviour
{
    [Conditional("UNITY_EDITOR")]
    public static void Log(string text)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.Log(text);
#endif
    }

    public static void Log(string text, Color color)
    {
        // Color型の値を16進数の文字列に
        string stringColor = ColorUtility.ToHtmlStringRGB(color);
        AdDebug.Log("<color=#" + stringColor + ">" + text + "</color>");
    }

    public static void Log(string text, int size)
    {
        AdDebug.Log("<size=" + size.ToString() + ">" + text + "</size>");
    }

    public static void Log(string text, Color color, int size)
    {
        AdDebug.Log("<size=" + size.ToString() + ">" + text + "</size>", color);
    }

    public static void Log(string text, bool Bold)
    {
        AdDebug.Log("<b>" + text + "</b>");
    }

    public static void Log(string text, Color color, bool Bold)
    {
        // Color型の値を16進数の文字列に
        string stringColor = ColorUtility.ToHtmlStringRGB(color);
        AdDebug.Log("<color=#" + stringColor + ">" + "<b>" + text + "</b>" + "</color>");
    }

    public static void Log(string text, int size, bool Bold)
    {
        AdDebug.Log("<size=" + size.ToString() + ">" + "<b>" + text + "</b>" + "</size>");
    }

    public static void Log(string text, Color color, int size, bool Bold)
    {
        AdDebug.Log("<size=" + size.ToString() + ">" + "<b>" + text + "</b>" + "</size>", color);
    }

}