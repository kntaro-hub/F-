using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageWindow : MonoBehaviour
{
    // =--------- 構造体定義 ---------= //
    // テキスト一つ分の情報
    struct TextInformation
    {
        public string text;
        public Color color;
    }
   
    // =--------- SerializeField ---------= //
    [SerializeField] MessageText TextMeshPrefab = null; // テキストの基
    [SerializeField] private float   FallSpeed = 0.1f;      // テキストが流れる速度（秒）
    [SerializeField] private float   TextSpace = 60.0f;     // テキスト同士の間隔
    [SerializeField] private int     MaxText = 4;           // 一度に表示されるテキストの最大数
    [SerializeField] private float fadeTime = 1.0f;
    [SerializeField] private Image panel;

    // =--------- 変数宣言 ---------= //
    // 表示しているテキスト
    List<MessageText> texts = new List<MessageText>();

    // 表示する予定のテキスト
    List<TextInformation> reserves = new List<TextInformation>();

    // テキストを流せるか
    private bool IsFallText = true;

    // コルーチン格納用
    private IEnumerator fadeTextTimer;

    // パネル初期カラー
    private Color initPanelColor;

    // Start is called before the first frame update
    void Start()
    {
        fadeTextTimer = this.FadeTextTimer();

        // パネル初期カラー取得
        initPanelColor = panel.color;

        // パネルを透明に
        panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(IsFallText)
        {// テキストを流せる状態なら
            if(reserves.Count > 0)
            {// かつ、予約テキストが一件でもあったら

                // テキスト生成
                MessageText textMesh = Instantiate(TextMeshPrefab);

                // 初期座標保存
                Vector3 InitPos = textMesh.transform.localPosition;

                // 生成したテキストをメッセージウィンドウの子に設定
                textMesh.transform.SetParent(this.transform.GetComponentInChildren<RectMask2D>().transform);

                // 予約テキストからテキストを更新
                textMesh.mesh.text = reserves[reserves.Count - 1].text;

                // 予約テキスト色保存
                Color setColor = reserves[reserves.Count - 1].color;

                // 移し終えたので予約テキスト削除
                reserves.RemoveAt(reserves.Count - 1);

                foreach (var itr in texts)
                {// 今までのテキストをすべて一つ上に流す
                    itr.transform.DOLocalMoveY(itr.transform.localPosition.y + TextSpace, FallSpeed);
                }

                // 生成したテキストを初期位置の一行分下へ
                textMesh.transform.localPosition = new Vector2(InitPos.x, InitPos.y - TextSpace);
                textMesh.mesh.color = Color.clear;

                // 初期位置へ動かす&色を変化
                textMesh.transform.DOLocalMoveY(InitPos.y, FallSpeed);
                textMesh.mesh.DOColor(setColor, FallSpeed);

                // 生成したテキストをリストへ追加
                texts.Add(textMesh);

                // テキストを流せない状態にする
                IsFallText = false;

                // テキストを流せるまでのタイマーをスタートする
                StartCoroutine(TextEraseTimer());
            }
        }
    }

    /// <summary>
    /// メッセージ予約
    /// </summary>
    /// <param name="text">メッセージ内容</param>
    /// <param name="color">メッセージ色</param>
    public void AddMessage(string text, Color color)
    {
        TextInformation information = new TextInformation();
        information.text = text;
        information.color = color;
        reserves.Insert(0,information);

        if(fadeTextTimer != null)
        {
            StopCoroutine(fadeTextTimer);
            fadeTextTimer = null;
            fadeTextTimer = this.FadeTextTimer();
            panel.DOColor(initPanelColor, 0.2f);
        }

        StartCoroutine(fadeTextTimer);
    }
    public void AddMessage(string text)
    {
        this.AddMessage(text, Color.white);
    }

    private IEnumerator FadeTextTimer()
    {
        yield return new WaitForSeconds(fadeTime);

        foreach(var itr in texts)
        {
            itr.HideText();
        }
        panel.DOColor(new Color(initPanelColor.r, initPanelColor.g, initPanelColor.b, 0.0f), 0.2f);
    }

    private IEnumerator TextEraseTimer()
    {
        yield return new WaitForSeconds(FallSpeed);

        // テキストを流せる状態にする
        IsFallText = true;

        if (texts.Count > MaxText)
        {// テキストが最大数を超えたら

            // テキスト削除
            Destroy(texts[0].gameObject);

            // リストからも削除
            texts.RemoveAt(0);
        }
    }

    #region singleton

    static MessageWindow _instance;

    public static MessageWindow instance
    {
        get
        {
            if (_instance == null)
            {
                var previous = FindObjectOfType(typeof(MessageWindow));
                if (previous)
                {
                    _instance = (MessageWindow)previous;
                }
                else
                {
                    var go = new GameObject("MessageWindow");
                    _instance = go.AddComponent<MessageWindow>();
                    DontDestroyOnLoad(go);
                    go.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            return _instance;
        }
    }

    #endregion
}
