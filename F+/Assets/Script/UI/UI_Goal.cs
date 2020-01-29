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
public class UI_Goal : UI_Base
{
    // =--------- 列挙体定義 ---------= //
    enum TextType   // 表示するテキストの種類
    {
        next,           // 先へ進む     1
        cancel,         // キャンセル   2
        max             // 最大値
    }

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

    // =--------- 変数宣言 ---------= //

    private int buttonNum   = 0;        // 選択しているボタン番号
    private Image panel     = null;     // パネル
    private Image cursor    = null;     // 選択カーソル
    private UI_Map ui_Map   = null;     // マップ表示用
    private bool isShow     = false;    // メニューを表示しているか

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
        cursor = Instantiate(cursorPrefab, this.transform);
        TextMeshProUGUI nextStage   = Instantiate(textPrefab, this.transform); nextStage.text = "先へ進む";
        TextMeshProUGUI cancel      = Instantiate(textPrefab, this.transform); cancel.text = "キャンセル";

        textList.Add(nextStage);
        textList.Add(cancel);

        panel = this.GetComponent<Image>();
        panel.color = Color.clear;

        // 初期化
        this.Init();

        // 0番にカーソル位置合わせ
        this.CursorSet(0);
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
            if (PS4Input.GetCrossKeyU())
            {// ↑キーでカーソルを上に
                buttonNum--;
                this.CheckFlow();
                this.CursorSet(buttonNum);
            }
            if (PS4Input.GetCrossKeyD())
            {// ↓キーでカーソルを下に
                buttonNum++;
                this.CheckFlow();
                this.CursorSet(buttonNum);
            }

            if (PS4Input.GetButtonDown(PS4ButtonCode.Circle))
            {// エンターキーで決定
                this.SwitchCommand();
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

        // フェードアウト
        Fade.instance.FadeOut();

        // プレイヤーは操作不能に
        SequenceMGR.instance.seqType = SequenceMGR.SeqType.moveImpossible;
    }
    /// <summary>
    /// [キャンセル]コマンド
    /// </summary>
    private void Com_Cancel()
    {// マップUIを消す
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
                textList[i].rectTransform.localPosition.x - textList[i].rectTransform.sizeDelta.x * 0.55f,
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
    /// 現在の選んでいるボタンごとの処理を実行するs
    /// </summary>
    private void SwitchCommand()
    {
        switch (buttonNum)
        {
            case (int)TextType.next: this.Com_Next(); break;
            case (int)TextType.cancel: this.Com_Cancel(); break;
            default: break;
        }
    }
}
