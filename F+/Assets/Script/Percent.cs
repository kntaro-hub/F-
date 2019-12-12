using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 確率計算用クラス
/// </summary>
public static class Percent
{
    /// <summary>
    /// 入力した確率で判定を行う
    /// </summary>
    public static bool Per(int percent)
    {
        return Per((float)percent);
    }


    /// <summary>
    /// 入力した確率で判定を行う
    /// </summary>
    public static bool Per(float percent)
    {
        //乱数の上限と真と判定するボーダーを設定
        int randomValueLimit = 100;
        int border           = (int)(percent);
        return Random.Range(0,randomValueLimit) <= border;
    }
}