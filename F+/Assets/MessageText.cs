using DG.Tweening;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageText : MonoBehaviour
{
    // =--------- 変数宣言 ---------= //
    // テキスト
    private TextMeshProUGUI textMesh;

    // =--------- 定数定義 ---------= //
    private const float fadeTime = 0.1f;
    private readonly Color fadeOutColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    private readonly Color fadeInColor = Color.white;

    // =--------- プロパティ ---------= //
    public TextMeshProUGUI mesh
    {
        get
        {
            if (textMesh != null)
                return textMesh;
            else
            {
                textMesh = this.GetComponent<TextMeshProUGUI>();
                return textMesh;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // テキストメッシュ取得
        textMesh = this.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// テキストをfadeTime秒で出現させる
    /// </summary>
    public void ShowText()
    {
        textMesh.DOColor(fadeInColor, fadeTime);
    }

    /// <summary>
    /// テキストをfadeTime秒で隠す
    /// </summary>
    public void HideText()
    {
        textMesh.DOColor(fadeOutColor, fadeTime);
    }
}
