using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// プレイヤーキャラクターの操作を管理
/// </summary>
public class PlayerControll : MonoBehaviour
{
    enum Direct
    {
        right = 0,
        left,
        forward,
        back,
        max
    }

    // グリッド
    protected Vector2Int gridPos = new Vector2Int(5,5);

    // 移動中フラグ
    bool IsMove = false;

    // 移動にかかる時間
    [SerializeField] const float MoveTime = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = FieldMGR.instance.GridToWorld(gridPos);
    }

    // Update is called once per frame
    void Update()
    {
        // プレイヤーキャラクターの操作を一括管理する
        this.Controll();
    }

    private void Controll()
    {
        if (!IsMove)
        {// 移動中でない場合移動できる
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {// 右
                this.Move(Direct.right);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {// 左
                this.Move(Direct.left);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {// 奥
                this.Move(Direct.forward);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {// 手前
                this.Move(Direct.back);
            }
        }
    }

    /// <summary>
    /// 移動用関数
    /// </summary>
    /// <param name="Direct">目的地</param>
    private void Move(Direct direct)
    {
        Vector3 moveValue = new Vector3();
        Vector2Int movedGrid;

        switch (direct)
        {
            case Direct.right: movedGrid    = new Vector2Int(1, 0);   break;
            case Direct.left: movedGrid     = new Vector2Int(-1, 0);  break;
            case Direct.forward: movedGrid  = new Vector2Int(0, 1);   break;
            case Direct.back: movedGrid     = new Vector2Int(0, -1);  break;
            default: movedGrid = new Vector2Int(); break;
        }

        if (FieldMGR.instance.GridInfos[gridPos.x + movedGrid.x, gridPos.y + movedGrid.y].Type != FieldInformation.FieldType.wall) 
        {

            switch (direct)
            {
                case Direct.right:
                    moveValue = Vector3.right;
                    gridPos.x++;
                    MessageWindow.instance.AddMessage("右に進みました", Color.white);
                    break;
                case Direct.left:
                    moveValue = Vector3.left;
                    gridPos.x--;
                    MessageWindow.instance.AddMessage("左に進みました", Color.white);
                    break;
                case Direct.forward:
                    moveValue = Vector3.forward;
                    gridPos.y++;
                    MessageWindow.instance.AddMessage("奥に進みました", Color.white);
                    break;
                case Direct.back:
                    moveValue = Vector3.back;
                    gridPos.y--;
                    MessageWindow.instance.AddMessage("手前に進みました", Color.white);
                    break;
            }

            moveValue *= FieldInformation.GridSize;
            // MoveTime秒かけて目的地へ
            this.transform.DOMove(new Vector3(
                    this.transform.position.x + moveValue.x,
                    this.transform.position.y + moveValue.y,
                    this.transform.position.z + moveValue.z),
                    MoveTime).SetEase(Ease.InOutCubic);
            // 移動中フラグon
            IsMove = true;

            // MoveTime秒経つまで次の入力を受け付けないようにする
            StartCoroutine(MoveTimer());
        }
    }

    /// <summary>
    /// 指定の時間が経ったら入力を受け付けられるようにする
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveTimer()
    {
        // MoveTime秒まつ
        yield return new WaitForSeconds(MoveTime);

        // 再入力可能状態へ
        IsMove = false;
    }
}
