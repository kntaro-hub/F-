using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Interval_FloorNum : MonoBehaviour
{
    [SerializeField] Fade fade;
    [SerializeField] FadeText[] guideText;
    
    TextMeshProUGUI floorNumText;

    private const float fadeTime = 2.0f;

    private bool isMoved = false;

    // Start is called before the first frame update
    void Start()
    {
        floorNumText = this.GetComponent<TextMeshProUGUI>();
        this.floorNumText.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoved)
        {
            if (PS4Input.GetButtonDown(PS4ButtonCode.Circle) ||
                PS4Input.GetButtonDown(PS4ButtonCode.Cross) ||
                PS4Input.GetButtonDown(PS4ButtonCode.Option) ||
                PS4Input.GetButtonDown(PS4ButtonCode.TouchPad))
            {
                isMoved = false;
                fade.FadeOut($"Game");
            }
        }
    }

    public void ShowFloorNum()
    {
        this.floorNumText.DOColor(Color.white, fadeTime);
        this.floorNumText.text = $"第   { FloorMGR.instance.FloorNum }   階";
        StartCoroutine(this.FadeTimer());
    }

    private IEnumerator FadeTimer()
    {
        yield return new WaitForSeconds(fadeTime * 0.3f);

        // ガイドテキストをフェード開始
        foreach (var itr in guideText)
        {
            itr.FadeStart();
        }

        yield return new WaitForSeconds(fadeTime * 0.7f);

        // ボタン入力受付開始
        isMoved = true;
    }
}
