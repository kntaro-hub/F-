using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UI_ExpGauge : MonoBehaviour
{
    // プレイヤー
    private PlayerControll player = null;

    // 経験値ゲージスライダー
    private Slider Exp = null;

    // 経験値テーブル
    private List<LevelTableEntity> levelTable = null;

    // Start is called before the first frame update
    void Start()
    {
        player = SequenceMGR.instance.Player;
        Exp = this.GetComponent<Slider>();
        levelTable = DataBase.instance.GetLevelTable();
    }

    // Update is called once per frame
    void Update()
    {
        // 経験値ゲージ更新
        Exp.value =
            (float)Mathf.Abs(player.Param.exp - levelTable[player.Param.level- 1].Xp) 
            / 
            (float)Mathf.Abs(levelTable[player.Param.level - 1].Xp - levelTable[player.Param.level].Xp);
    }
}
