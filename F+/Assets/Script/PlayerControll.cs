using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerControll : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.Controll();
    }

    private void Controll()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.transform.DOMove(new Vector3(
                this.transform.position.x + FieldInfomation.GridSize,
                this.transform.position.y,
                this.transform.position.z),
                0.5f).SetEase(Ease.InOutCubic);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            this.transform.DOMove(new Vector3(
                this.transform.position.x - FieldInfomation.GridSize,
                this.transform.position.y,
                this.transform.position.z),
                0.5f).SetEase(Ease.InOutCubic);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            this.transform.DOMove(new Vector3(
                this.transform.position.x,
                this.transform.position.y,
                this.transform.position.z + FieldInfomation.GridSize),
                0.5f).SetEase(Ease.InOutCubic);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            this.transform.DOMove(new Vector3(
                this.transform.position.x,
                this.transform.position.y,
                this.transform.position.z - FieldInfomation.GridSize),
                0.5f).SetEase(Ease.InOutCubic);
        }
    }
}
