using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 基本メニューUI表示クラス
/// </summary>
public class UI_BasicMenu : UI_Base
{
    // =--------- 列挙体定義 ---------= //
    enum TextType   // 表示するテキストの種類
    {
        item,           // 道具     1
        map,            // マップ   2
        interruption,   // 中断     4
        close,          // 閉じる   5
        max             // 最大値
    }

    // =--------- プレハブ ---------= //

    [SerializeField]
    private TextMeshProUGUI textPrefab;     // テキストプレハブ
    [SerializeField]
    private Image           cursorPrefab;   // カーソルプレハブ

    // =--------- パラメータ ---------= //
    [SerializeField]
    private float initializePositionX = 0.0f;   // テキスト基準座標X
    [SerializeField]
    private float initializePositionY = 0.0f;   // テキスト基準座標Y

    // =--------- 変数宣言 ---------= //

    private int     buttonNum   = 0;        // 選択しているボタン番号
    private Image   panel       = null;     // パネル
    private Image   cursor      = null;     // 選択カーソル
    private UI_Map  ui_Map      = null;     // マップ表示用
    private bool    isShow      = false;    // メニューを表示しているか
    private Color   initPanelColor;         // 初期メニュー背景パネルカラー

    private List<TextMeshProUGUI> textList = new List<TextMeshProUGUI>();   // 生成したテキストリスト
        
    // =--------- プロパティ ---------= //
    public bool IsShowMenu
    {
        get { return isShow; }
        set { isShow = value; }
    }

    // =--------- // =--------- ---------= // ---------= //

    // Start is called before the first frame update
    void Start()
    {
        this.CreateUI();
    }

    public void CreateUI()
    {
        cursor = Instantiate(cursorPrefab, this.transform);
        TextMeshProUGUI item = Instantiate(textPrefab, this.transform); item.text = "どうぐ";
        TextMeshProUGUI map = Instantiate(textPrefab, this.transform); map.text = "マップ";
        TextMeshProUGUI interruption = Instantiate(textPrefab, this.transform); interruption.text = "やめる";
        TextMeshProUGUI close = Instantiate(textPrefab, this.transform); close.text = "とじる";

        ui_Map = FindObjectOfType<UI_Map>();

        textList.Add(item);
        textList.Add(map);
        textList.Add(interruption);
        textList.Add(close);

        panel = this.GetComponent<Image>();
        initPanelColor = panel.color;
        panel.color = Color.clear;

        int cnt = 0;
        foreach (var itr in textList)
        {
            // 透明に
            itr.color = Color.clear;
            itr.rectTransform.localPosition = new Vector3(initializePositionX, initializePositionY - itr.rectTransform.sizeDelta.y * 0.5f * cnt);
            itr.rectTransform.localScale = new Vector3(0.2f, 0.2f);
            ++cnt;
        }

        // 0番にカーソル位置合わせ
        this.CursorSet(0);

        // マップUI作成
        ui_Map.CreateMapUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void UpdateProc_UI()
    {
        if (isShow)
        {// メニュー表示中のみ
            if (PS4Input.GetCrossKeyU())
            {// ↑キーでカーソルを上に
                buttonNum--;
                this.CheckFlow();
                this.CursorSet(buttonNum);
                SoundMGR.PlaySe("Choice", 0.2f);
            }
            if (PS4Input.GetCrossKeyD())
            {// ↓キーでカーソルを下に
                buttonNum++;
                this.CheckFlow();
                this.CursorSet(buttonNum);
                SoundMGR.PlaySe("Choice", 0.2f);
            }

            if (PS4Input.GetButtonDown(PS4ButtonCode.Circle) || PS4Input.GetButtonDown(PS4ButtonCode.Option))
            {// 〇 or Optionボタンで決定
                this.SwitchCommand();

                // 決定音再生
                SoundMGR.PlaySe("Decision");
            }
        }
    }

    // =--------- // =--------- メニュー表示/非表示 ---------= // ---------= //

    /// <summary>
    /// メニューを開く
    /// </summary>
    public override void ShowMenu()
    {
        foreach (var itr in textList)
        {
            itr.color = Color.white;
        }
        cursor.color = Color.white;
        panel.color = initPanelColor;
        isShow = true;

        this.transform.DOMoveX(0.0f + panel.rectTransform.sizeDelta.x * 0.5f, UI_MGR.ShowMenuTime);

        // 0番にカーソル位置を合わせる
        buttonNum = 0;
        this.CursorSet(buttonNum);

        UI_MGR.instance.Ui_Infomation.ShowMenu();

        // 表示音再生
        SoundMGR.PlaySe("Choice", 0.2f);
    }

    /// <summary>
    /// メニューを閉じる
    /// </summary>
    public override void HideMenu()
    {
        isShow = false;

        this.transform.DOMoveX(0.0f - panel.rectTransform.sizeDelta.x * 0.5f, UI_MGR.ShowMenuTime);

        UI_MGR.instance.Ui_Infomation.HideMenu();
    }

    // =--------- // =--------- コマンド ---------= // ---------= //
    /// <summary>
    /// [アイテム]コマンド
    /// </summary>
    private void Com_Item()
    {
        UI_MGR.instance.ShowUI(UI_MGR.UIType.inventory);

    }
    /// <summary>
    /// [マップ]コマンド
    /// </summary>
    private void Com_Map()
    {// 2Dマップを表示する
        UI_MGR.instance.Ui_Map.ShowMapUI();
    }
    /// <summary>
    /// [調べる]コマンド
    /// </summary>
    private void Com_Inspect()
    {

    }
    /// <summary>
    /// [中断]コマンド
    /// </summary>
    private void Com_Interruption()
    {

    }
    /// <summary>
    /// [閉じる]コマンド
    /// </summary>
    private void Com_Close()
    {
        UI_MGR.instance.ReturnUI();
    }

    // =--------- // =--------- ---------= // ---------= //

    /// <summary>
    /// テキストの横にカーソルの座標を合わせる
    /// </summary>
    /// <param name="i"></param>
    private void CursorSet(int i)
    {
        cursor.rectTransform.localPosition =
            new Vector3(
                textList[i].rectTransform.localPosition.x - 150.0f,
                textList[i].rectTransform.localPosition.y);
    }

    /// <summary>
    /// オーバーフローorアンダーフロー対策
    /// </summary>
    private void CheckFlow()
    {
        if (buttonNum >= (int)TextType.max)
        {
            buttonNum = 0;
        }
        else if (buttonNum < 0)
        {
            buttonNum = (int)TextType.max - 1;
        }
    }

    /// <summary>
    /// 現在の選んでいるボタンごとの処理を実行する
    /// </summary>
    private void SwitchCommand()
    {
        switch (buttonNum)
        {
            case (int)TextType.item: this.Com_Item(); break;
            case (int)TextType.map: this.Com_Map(); break;
            case (int)TextType.interruption: this.Com_Interruption(); break;
            case (int)TextType.close: this.Com_Close(); break;
        }
    }
}
