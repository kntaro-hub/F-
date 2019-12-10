using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

/// <summary>
/// 階段メニュー表示
/// </summary>
public class UI_Status : MonoBehaviour
{
    // =--------- プレハブ ---------= //

    [SerializeField]
    private TextMeshProUGUI Prefab_Text_Floor;

    [SerializeField]
    private TextMeshProUGUI Prefab_Text_HP;

    [SerializeField]
    private TextMeshProUGUI Prefab_Text_Lv;

    // =--------- 変数宣言 ---------= //
    private PlayerControll player;

    private TextMeshProUGUI Text_Floor;

    private TextMeshProUGUI Text_HP;

    private TextMeshProUGUI Text_Lv;

    // =--------- // =--------- ---------= // ---------= //

    // Start is called before the first frame update
    void Start()
    {
        player = SequenceMGR.instance.Player;

        this.CreateText();

        this.UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        this.UpdateText();
    }

    /// <summary>
    /// テキスト生成
    /// </summary>
    private void CreateText()
    {
        Text_Floor  = Instantiate(Prefab_Text_Floor, this.transform);
        Text_Lv     = Instantiate(Prefab_Text_Lv, this.transform);
        Text_HP     = Instantiate(Prefab_Text_HP, this.transform);
    }

    /// <summary>
    /// テキスト更新
    /// </summary>
    private void UpdateText()
    {
        Text_Floor.text = 1 + "階";
        Text_Lv.text    = "Lv." + player.Param.level;
        Text_HP.text    = "HP " + player.Param.hp;
    }
}
