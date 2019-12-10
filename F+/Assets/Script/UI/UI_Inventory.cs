﻿using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : UI_Base
{
    // =--------- プレハブ ---------= //

    [SerializeField]
    private TextMeshProUGUI textPrefab;     // テキストプレハブ
    [SerializeField]
    private Image cursorPrefab;   // カーソルプレハブ

    // =--------- パラメータ ---------= //

    [SerializeField]
    private float offsetText = 0.5f;            // テキスト同士のY座標間隔
    [SerializeField]
    private float initializePositionX = 0.0f;   // テキスト基準座標X
    [SerializeField]
    private float initializePositionY = 0.0f;   // テキスト基準座標Y
    [SerializeField]
    private int ShowItemNum = 8;
    [SerializeField]
    private UI_ItemMenu itemMenu;

    // =--------- 変数宣言 ---------= //

    private int buttonNum = 0;        // 選択しているボタン番号
    private Image panel = null;     // パネル
    private Image cursor = null;     // 選択カーソル
    private UI_Map ui_Map = null;     // マップ表示用
    private bool isShow = false;    // メニューを表示しているか

    private int pageNum = 0;

    private List<TextMeshProUGUI> textList = new List<TextMeshProUGUI>();   // 生成したテキストリスト

    private Player_Items items;
    
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
        cursor = Instantiate(cursorPrefab, this.transform);

        items = FindObjectOfType<Player_Items>();
        panel = this.GetComponent<Image>();
        panel.color = Color.clear;

        // 初期化
        this.Init();
    }

    // Update is called once per frame
    void Update()
    {
        // 座標矯正
        int cnt = 0;
        foreach (var itr in textList)
        {
            itr.rectTransform.localPosition = new Vector3(initializePositionX,
                initializePositionY - itr.rectTransform.sizeDelta.y * cnt + offsetText * cnt);
            ++cnt;
        }
    }

    public override void UpdateProc_UI()
    {
        if (isShow)
        {// メニュー表示中のみ
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {// ↑キーでカーソルを上に
                buttonNum--;
                this.CheckFlow();
                this.CursorSet(buttonNum);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {// ↓キーでカーソルを下に
                buttonNum++;
                this.CheckFlow();
                this.CursorSet(buttonNum);
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {// エンターキーで決定
                this.SwitchCommand();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {// escキーでメニュー表示/非表示
                UI_MGR.instance.ReturnUI();
            }
        }
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
            itr.rectTransform.localPosition = new Vector3(initializePositionX,
                initializePositionY - itr.rectTransform.sizeDelta.y * cnt + offsetText);
            itr.rectTransform.localScale = new Vector3(1.0f, 0.3f);
            ++cnt;
        }
        // カーソルも透明に
        cursor.color = Color.clear;
        // パネルも
        panel.color = Color.clear;
    }

    // =--------- // =--------- メニュー表示/非表示 ---------= // ---------= //

    /// <summary>
    /// メニューを開く
    /// </summary>
    public override void ShowMenu()
    {
        for (int i = 0; i < items.StockCount(); i++)
        {// アイテム数だけテキストを表示
            if (textList.Count > i)
            {// テキストを更新するだけ
                textList[i].text = DataBase.instance.GetItemTable(items.GetItemID(i)).Name;
            }
            else
            {
                TextMeshProUGUI itemText = Instantiate(textPrefab, this.transform);
                itemText.text = DataBase.instance.GetItemTable(items.GetItemID(i)).Name;
                textList.Add(itemText);
            }
        }

        foreach (var itr in textList)
        {
            itr.color = Color.white;
        }
        cursor.color = Color.white;
        panel.color = new Color(0.0f, 0.0f, 0.0f, 0.3f);
        isShow = true;

        // 0番にカーソル位置を合わせる
        buttonNum = 0;
        this.CursorSet(buttonNum);
    }

    /// <summary>
    /// メニューを閉じる
    /// </summary>
    public override void HideMenu()
    {
        foreach (var itr in textList)
        {
            itr.color = Color.clear;
        }
        cursor.color = Color.clear;
        panel.color = Color.clear;
        isShow = false;
    }

    // =--------- // =--------- コマンド ---------= // ---------= //
    /// <summary>
    /// [先へ進む]コマンド
    /// </summary>
    private void Com_Next()
    {// 次のステージへ

    }
    /// <summary>
    /// [キャンセル]コマンド
    /// </summary>
    private void Com_Cancel()
    {// マップUIを消す
        this.HideMenu();
    }

    // =--------- // =--------- ---------= // ---------= //
    
    /// <summary>
    /// テキストの横にカーソルの座標を合わせる
    /// </summary>
    /// <param name="i"></param>
    private void CursorSet(int i)
    {
        if (textList.Count > 0)
        {
            cursor.rectTransform.localPosition =
                new Vector3(
                    textList[i].rectTransform.localPosition.x - textList[i].rectTransform.sizeDelta.x * 0.55f,
                    textList[i].rectTransform.localPosition.y);
        }
    }

    /// <summary>
    /// オーバーフローorアンダーフロー対策
    /// </summary>
    private void CheckFlow()
    {
        if (buttonNum >= (int)textList.Count)
        {
            buttonNum = 0;
        }
        else if (buttonNum < 0)
        {
            buttonNum = (int)textList.Count - 1;
        }
    }

    /// <summary>
    /// 現在の選んでいるボタンごとの処理を実行する
    /// </summary>
    private void SwitchCommand()
    {
        itemMenu.SetItemID(buttonNum + pageNum * ShowItemNum);
        UI_MGR.instance.ShowUI(UI_MGR.UIType.itemMenu);
    }
}
