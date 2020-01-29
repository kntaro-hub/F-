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


/// <summary>
/// 確率計算用クラス
/// </summary>
public static class RandomPlus
{
    /// <summary>
    /// 被りなしで範囲内をnum個取得
    /// </summary>
    public static List<int> RangeList(int min, int max, int num)
    {
        List<int> randomList = new List<int>();

        for (int i = 0; i < num; ++i)
        {
            while (true)
            {
                int randomNum = Random.Range(min, max);

                // リストの中に被りがあるか走査
                foreach (var itr in randomList)
                {
                    if (itr == randomNum)
                    {
                        // 再抽選
                        continue;
                    }
                }
                // 被りがなければ格納する
                randomList.Add(randomNum);
                break;
            }
        }
        return randomList;
    }   
}