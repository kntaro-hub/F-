using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeUIBG : MonoBehaviour
{
    private Image image;
    private RectTransform rect;

    private float Scale = 0.0f;

    private const float ScaleUpSpeed = 0.01f;

    private bool IsScaleFade = false;

    // Start is called before the first frame update
    void Start()
    {
        image = this.gameObject.GetComponent<Image>();
        rect = this.gameObject.GetComponent<RectTransform>();

        image.color = Color.white;
        rect.localScale = new Vector3(1.0f,0.0f,0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            this.ScaleUp();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            this.ScaleDown();
        }
    }

    private void ScaleUp()
    {
        if(!IsScaleFade)
        {
            rect.DOScaleY(0.3f, 0.7f);
            IsScaleFade = true;
            StartCoroutine(Timer());
        }
    }

    private void ScaleDown()
    {
        if (!IsScaleFade)
        {
            rect.DOScaleY(0.0f, 0.7f);
            IsScaleFade = true;
            StartCoroutine(Timer());
        }
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(0.7f);
        IsScaleFade = false;
    }
}
