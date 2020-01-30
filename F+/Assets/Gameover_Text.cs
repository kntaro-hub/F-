using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Gameover_Text : MonoBehaviour
{
    [SerializeField] private float FadeStartTime = 1.0f;

    private TextMeshProUGUI text;
    private bool isFadeEnd = false;

    // Start is called before the first frame update
    void Start()
    {
        text = this.GetComponent<TextMeshProUGUI>();
        text.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        StartCoroutine(this.FadeTimer());
    }

    // Update is called once per frame
    void Update()
    {
        if(this.isFadeEnd)
        {
            if(PS4Input.GetButtonDown( PS4ButtonCode.Circle))
            {
                Fade.instance.FadeOut("Interval");
                isFadeEnd = false;
            }
        }
    }

    private IEnumerator FadeTimer()
    {
        yield return new WaitForSeconds(FadeStartTime);

        text.DOColor(Color.white, 1.2f);

        this.isFadeEnd = true;
    }
}
