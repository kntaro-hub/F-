using DG.Tweening;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadeText : MonoBehaviour
{
    [SerializeField] Color fadeOutColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    [SerializeField] Color fadeInColor = Color.white;

    private TextMeshProUGUI text = null;

    private const float fadeTime = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        // テキストメッシュ取得
        text = this.GetComponent<TextMeshProUGUI>();

        // 色を透明に
        text.color = fadeOutColor;
    }

    public void FadeStart()
    {
        // フェード開始
        this.FadeIn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FadeOut()
    {
        this.text.DOColor(fadeOutColor, fadeTime);
        StartCoroutine(this.FadeOutCoroutine());
    }

    private void FadeIn()
    {
        this.text.DOColor(fadeInColor, fadeTime);
        StartCoroutine(this.FadeInCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        yield return new WaitForSeconds(fadeTime);

        this.FadeIn();
    }

    private IEnumerator FadeInCoroutine()
    {
        yield return new WaitForSeconds(fadeTime);

        this.FadeOut();
    }
}
