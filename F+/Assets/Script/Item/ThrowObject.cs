using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ThrowObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 指定したポイントまで飛ばす
    /// </summary>
    /// <param name="point"></param>
    /// <param name="time"></param>
    public void Move(Point point, float time)
    {
        this.transform.DOMove(MapData.GridToWorld(point), time).SetEase(Ease.Linear);
    }
}
