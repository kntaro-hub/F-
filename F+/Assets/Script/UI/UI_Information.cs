using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 階段メニュー表示
/// </summary>
public class UI_Information : UI_Base
{
    // =--------- 列挙体定義 ---------= //
    enum TextType   // 表示するテキストの種類
    {
        weapon = 0,
        shield,
        deepest,
        hunger,
        xp,
        atk,
        max             // 最大値
    }

    // =--------- プレハブ ---------= //

    [SerializeField]
    private TextMeshProUGUI textPrefab;     // テキストプレハブ

    // =--------- パラメータ ---------= //
    [SerializeField]
    private float offsetY = 0.0f;        // テキスト同士のX座標間隔
    [SerializeField]
    private float TextInitPosX = 0.0f;   // テキスト基準座標X
    [SerializeField]
    private float TextInitPosY = 0.0f;   // テキスト基準座標Y

    private const float ShowedPanelPosX = 765.0f;

    // =--------- 変数宣言 ---------= //
    private Image panel = null;     // パネル
    private PlayerControll player;

    private List<TextMeshProUGUI> textList = new List<TextMeshProUGUI>();   // 生成したテキストリスト
    // =--------- // =--------- ---------= // ---------= //

    // Start is called before the first frame update
    void Start()
    {
        player = SequenceMGR.instance.Player;
        TextMeshProUGUI weapon  = Instantiate(textPrefab, this.transform);  weapon.text  = "武器の強さ" + DataBase.instance.GetWeaponTable(player.Param.weaponId).Atk.ToString();
        TextMeshProUGUI shield  = Instantiate(textPrefab, this.transform);  shield.text  = "盾の強さ" + DataBase.instance.GetShiledTable(player.Param.shieldId).Def.ToString();
        TextMeshProUGUI deepest = Instantiate(textPrefab, this.transform);  deepest.text = "最深層" + 1;
        TextMeshProUGUI hunger  = Instantiate(textPrefab, this.transform);  hunger.text  = "満腹度" + player.Param.hunger.ToString();
        TextMeshProUGUI xp      = Instantiate(textPrefab, this.transform);  xp.text      = "経験値" + player.Param.exp.ToString();
        TextMeshProUGUI atk     = Instantiate(textPrefab, this.transform);  atk.text     = "基本攻撃力" + player.Param.atk.ToString();

        textList.Add(weapon);
        textList.Add(shield);
        textList.Add(deepest);
        textList.Add(hunger);
        textList.Add(xp);
        textList.Add(atk);

        panel = this.GetComponent<Image>();

        // 初期化
        this.Init();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void UpdateProc_UI()
    {

    }

    /// <summary>
    /// 初期処理
    /// </summary>
    private void Init()
    {
        int cnt = 0;
        foreach (var itr in textList)
        {
            // 透明に
            itr.color = Color.clear;
            itr.rectTransform.localPosition = new Vector3(TextInitPosX, TextInitPosY + cnt * offsetY);
            itr.rectTransform.localScale = new Vector3(0.3f, 0.3f);
            ++cnt;
        }
        // パネルも
        panel.color = Color.clear;
    }

    // =--------- // =--------- メニュー表示/非表示 ---------= // ---------= //

    /// <summary>
    /// メニューを開く
    /// </summary>
    public override void ShowMenu()
    {
        textList[(int)TextType.weapon].text  = "武器の強さ: " + DataBase.instance.GetWeaponTable(player.Param.weaponId).Atk.ToString();
        textList[(int)TextType.shield].text  = "盾の強さ　: " + DataBase.instance.GetShiledTable(player.Param.shieldId).Def.ToString();
        textList[(int)TextType.deepest].text = "最深層　　: " + 1;
        textList[(int)TextType.hunger].text  = "満腹度　　: " + player.Param.hunger.ToString();
        textList[(int)TextType.xp].text      = "経験値　　: " + player.Param.exp.ToString();
        textList[(int)TextType.atk].text     = "基本攻撃力: " + player.Param.atk.ToString();

        foreach (var itr in textList)
        {
            itr.color = Color.white;
        }
        panel.color = new Color(0.0f, 0.0f, 0.0f, 0.3f);

        this.transform.DOLocalMoveX(ShowedPanelPosX, UI_MGR.ShowMenuTime);

    }

    /// <summary>
    /// メニューを閉じる
    /// </summary>
    public override void HideMenu()
    {
        this.transform.DOLocalMoveX(ShowedPanelPosX + panel.rectTransform.sizeDelta.x, UI_MGR.ShowMenuTime);
    }
}
