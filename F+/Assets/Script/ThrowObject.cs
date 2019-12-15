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

    public void Move(float time, Point point)
    {
        this.transform.DOMove(MapData.GridToWorld(point), time).SetEase(Ease.Linear);
    }
}
