using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Title_Camera : MonoBehaviour
{
    [SerializeField] private Vector3 target;
    [SerializeField] private Vector3 rot;

    private bool isMoved = false;
    public bool IsMoved
    {
        get { return isMoved; }
        set { isMoved = value; }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MoveStart(float moveTime)
    {
        this.transform.DOMove(target, moveTime).SetEase(Ease.InOutCubic);
        this.transform.DORotate(rot, moveTime).SetEase(Ease.InOutCubic);

        StartCoroutine(this.MoveTimer(moveTime));
    }

    private IEnumerator MoveTimer(float moveTime)
    {
        yield return new WaitForSeconds(moveTime);

        isMoved = true;
    }
}
