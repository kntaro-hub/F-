using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Title_MenuUI : UI_Base
{
    // =--------- 列挙体定義 ---------= //
    enum TextType   // 表示するテキストの種類
    {
        start = 0,      // スタート
        end,             // しゅうりょう
        max             // 最大値
    }

    // =--------- プレハブ ---------= //

    [SerializeField] private Title_UI text_Start;   // Startテキスト
    [SerializeField] private Title_UI text_End;   // Endテキスト
    [SerializeField] private Title_UI panel;
    [SerializeField] private Title_UI cursor;                // カーソルプレハブ
    [SerializeField] private Fade fadeImage;                      // フェード画像
            
    // =--------- パラメータ ---------= //
    [SerializeField] private float initializePositionX = 0.0f;   // テキスト基準座標X
    [SerializeField] private float initializePositionY = 0.0f;   // テキスト基準座標Y

    // =--------- 変数宣言 ---------= //

    private int buttonNum = 0;        // 選択しているボタン番号

    private SpriteRenderer panelSprite;
    private SpriteRenderer cursorSprite;                // カーソルプレハブ
    private List<SpriteRenderer> textList = new List<SpriteRenderer>();   // 生成したテキストリスト

    // =--------- // =--------- ---------= // ---------= //

    // Start is called before the first frame update
    void Start()
    {
        this.CreateUI();
    }

    private void CreateUI()
    {
        textList.Add(text_Start.GetComponent<SpriteRenderer>());
        textList.Add(text_End.GetComponent<SpriteRenderer>());

        int cnt = 0;
        foreach (var itr in textList)
        {
            // 透明に
            itr.color = Color.clear;
            ++cnt;
        }

        cursorSprite = cursor.GetComponent<SpriteRenderer>();

        panelSprite = panel.GetComponent<SpriteRenderer>();

        cursorSprite.color = Color.clear;

        panelSprite.color = Color.clear;

        // 0番にカーソル位置合わせ
        this.CursorSet(0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void UpdateProc_UI()
    {
        int lastButtonNum = buttonNum;
        {// メニュー表示中のみ
            if (PS4Input.GetCrossKeyU())
            {// ↑キーでカーソルを上に
                buttonNum--;
                this.CheckFlow();
                this.CursorSet(buttonNum);

                if(buttonNum != lastButtonNum) SoundMGR.PlaySe("Choice", 0.2f);
            }
            if (PS4Input.GetCrossKeyD())
            {// ↓キーでカーソルを下に
                buttonNum++;
                this.CheckFlow();
                this.CursorSet(buttonNum);

                if (buttonNum != lastButtonNum) SoundMGR.PlaySe("Choice", 0.2f);
            }

            if (PS4Input.GetButtonDown(PS4ButtonCode.Circle) || PS4Input.GetButtonDown(PS4ButtonCode.Option))
            {// 〇 or Optionボタンで決定
                this.SwitchCommand();
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
            itr.DOColor(Color.white, 0.4f);
        }
        cursorSprite.DOColor(Color.white, 0.4f);
        panelSprite.DOColor(new Color(1.0f, 1.0f, 1.0f, 0.5f), 0.4f);

        // 0番にカーソル位置を合わせる
        buttonNum = 0;
        this.CursorSet(buttonNum);
    }

    /// <summary>
    /// メニューを閉じる
    /// </summary>
    public override void HideMenu()
    {
        
    }

    // =--------- // =--------- コマンド ---------= // ---------= //
    /// <summary>
    /// [スタート]コマンド
    /// </summary>
    private void Com_Start()
    {
        fadeImage.FadeOut("Interval");

        // スタートSE再生
        SoundMGR.PlaySe("Start");
    }
    /// <summary>
    /// [しゅうりょう]コマンド
    /// </summary>
    private void Com_End()
    {
        fadeImage.GameEnd();

        // 決定音再生
        SoundMGR.PlaySe("Decision");
    }

    // =--------- // =--------- ---------= // ---------= //

    /// <summary>
    /// テキストの横にカーソルの座標を合わせる
    /// </summary>
    /// <param name="i"></param>
    private void CursorSet(int i)
    {
        cursor.transform.localPosition =
            new Vector3(
                1.7f,
                textList[i].transform.localPosition.y);
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
            case (int)TextType.start: this.Com_Start(); break;
            case (int)TextType.end: this.Com_End(); break;
        }
    }
}
