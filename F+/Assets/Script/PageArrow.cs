using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class PageArrow : MonoBehaviour
{
    // 画像
    Image image = null;

    // Start is called before the first frame update
    void Start()
    {
        image = this.GetComponent<Image>();
        this.Clear();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Clear()
    {
        image.color = Color.clear;
    }

    public void Active()
    {
        image.color = Color.white;
    }

    public void Light()
    {
        image.color = Color.yellow;
        StartCoroutine(this.LightTimer());
    }

    private IEnumerator LightTimer()
    {
        yield return new WaitForSeconds(0.1f);

        image.color = Color.white;
    }

}
