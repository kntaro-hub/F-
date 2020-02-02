using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeTrans : MonoBehaviour
{
    private Image image;

    void Start()
    {
        image = this.GetComponent<Image>();
    }

    public void Fade(float time)
    {
        image.color = Color.clear;
        image.DOColor(new Color(0.0f, 0.0f, 0.0f, 0.5f), time);
    }
}
