using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class UI_Interruption : UI_Base
{
    // =--------- 列挙体定義 ---------= //
    enum TextType   // 表示するテキストの種類
    {
        Yes,           // はい     1
        No,            // いいえ   2
        max             // 最大値
    }

    // =--------- プレハブ ---------= //

    [SerializeField]
    private TextMeshProUGUI textPrefab;     // テキストプレハブ
    [SerializeField]
    private Image cursorPrefab;   // カーソルプレハブ

    // =--------- パラメータ ---------= //
    [SerializeField]
    private float initializePositionX = 0.0f;   // テキスト基準座標X
    [SerializeField]
    private float initializePositionY = 0.0f;   // テキスト基準座標Y
    [SerializeField]
    private float offsetX;
    [SerializeField]
    private float cursorOffsetX;
    [SerializeField]
    private float moveTime = 0.3f;

    // =--------- 変数宣言 ---------= //

    private int buttonNum = 0;        // 選択しているボタン番号
    private Image panel = null;     // パネル
    private Image cursor = null;     // 選択カーソル
    private bool isShow = false;    // メニューを表示しているか
    private Color initPanelColor;         // 初期メニュー背景パネルカラー

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
        TextMeshProUGUI Yes = Instantiate(textPrefab, this.transform); Yes.text = "はい";
        TextMeshProUGUI No = Instantiate(textPrefab, this.transform); No.text = "いいえ";

        textList.Add(Yes);
        textList.Add(No);

        panel = this.GetComponent<Image>();
        initPanelColor = panel.color;
        panel.color = Color.clear;

        int cnt = 0;
        foreach (var itr in textList)
        {
            itr.rectTransform.localPosition = new Vector3(initializePositionX, initializePositionY - itr.rectTransform.sizeDelta.y * 0.5f * cnt);
            ++cnt;
        }

        // 0番にカーソル位置合わせ
        this.CursorSet(0);
        cursor.color = Color.clear;
    }

    // Update is called once per frame
    void Update()
    {
        int cnt = 0;
        foreach (var itr in textList)
        {
            itr.rectTransform.localPosition = new Vector3(initializePositionX + offsetX * cnt, initializePositionY);
            ++cnt;
        }
    }

    public override void UpdateProc_UI()
    {
        if (isShow)
        {// メニュー表示中のみ
            if (PS4Input.GetCrossKeyL())
            {// ←ボタンでカーソルを左に
                buttonNum--;
                this.CheckFlow();
                this.CursorSet(buttonNum);
                SoundMGR.PlaySe("Choice", 0.2f);
            }
            if (PS4Input.GetCrossKeyR())
            {// →ボタンでカーソルを右に
                buttonNum++;
                this.CheckFlow();
                this.CursorSet(buttonNum);
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
        panel.color = new Color(0.0f, 0.0f, 0.0f, 0.8f);
        isShow = true;

        // 0番にカーソル位置を合わせる
        buttonNum = 0;
        this.CursorSet(buttonNum);

        // 表示音再生
        SoundMGR.PlaySe("Choice", 0.2f);

        this.transform.localPosition = new Vector3(this.transform.localPosition.x,1080.0f,0.0f);

        this.transform.DOLocalMoveY(0.0f, moveTime).SetEase( Ease.InOutCubic);
    }

    /// <summary>
    /// メニューを閉じる
    /// </summary>
    public override void HideMenu()
    {
        isShow = false;

        this.transform.DOLocalMoveY(-1080.0f, moveTime).SetEase(Ease.InOutCubic);
    }

    // =--------- // =--------- コマンド ---------= // ---------= //
    /// <summary>
    /// [アイテム]コマンド
    /// </summary>
    private void Com_Yes()
    {
        Fade.instance.FadeOut("Title");
        SequenceMGR.instance.Player.ResetStatus();
        SequenceMGR.instance.seqType = SequenceMGR.SeqType.moveImpossible;
    }
    /// <summary>
    /// [マップ]コマンド
    /// </summary>
    private void Com_No()
    {// UIを消す
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
                textList[i].rectTransform.localPosition.x - cursorOffsetX,
                textList[i].rectTransform.localPosition.y);

        SoundMGR.PlaySe("Choice", 0.2f);
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
            case (int)TextType.Yes: this.Com_Yes(); break;
            case (int)TextType.No: this.Com_No(); break;
        }
    }
}
