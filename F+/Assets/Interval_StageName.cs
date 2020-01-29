using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Interval_StageName : MonoBehaviour
{
    [SerializeField] Interval_FloorNum floorNumText;
    [SerializeField] MoveImage[] moveImages;
    TextMeshProUGUI stageNameText;

    const float moveTime = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        if(floorNumText == null)
        {
            floorNumText = FindObjectOfType<Interval_FloorNum>();
        }

        stageNameText = this.GetComponent<TextMeshProUGUI>();
        StartCoroutine(this.MoveTimer());
    }

    private IEnumerator MoveTimer()
    {
        yield return new WaitForSeconds(moveTime * 0.5f);

        this.stageNameText.rectTransform.DOMoveY((float)Screen.height / 2.0f + 300.0f, moveTime);

        foreach(var itr in moveImages)
        {
            itr.MoveStart();
        }

        yield return new WaitForSeconds(moveTime);

        this.floorNumText.ShowFloorNum();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
