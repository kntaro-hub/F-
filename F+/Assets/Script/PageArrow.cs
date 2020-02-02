using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class PageArrow : MonoBehaviour
{
    [SerializeField]
    private bool isFlip = false;

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

        if(isFlip)
            image.transform.DOPunchScale(new Vector3(-0.4f, 0.4f), 0.07f);
        else
            image.transform.DOPunchScale(new Vector3(0.4f, 0.4f), 0.07f);

        StartCoroutine(this.LightTimer());
    }

    private IEnumerator LightTimer()
    {
        yield return new WaitForSeconds(0.1f);

        image.color = Color.white;
    }

}
