using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// プレイヤーキャラクターの操作を管理
/// </summary>
public class PlayerControll : MonoBehaviour
{
    // =--------- 列挙体定義 ---------= //
    enum Direct
    {
        right = 0,
        left,
        forward,
        back,
        max
    }

    // =--------- 変数宣言 ---------= //
    // グリッド座標
    protected Vector2Int gridPos = new Vector2Int(0,0);

    // 移動中フラグ
    bool IsInput = false;

    // 移動キー押下フラグ
    bool IsLastMove = false;

    // アニメータ
    private Animator playerAnimator;

    private Vector3 toCameraVector = new Vector3();

    // =--------- 定数定義 ---------= //

    // 移動にかかる時間
    [SerializeField] const float MoveTime = 0.3f;

    // 初期Y座標
    [SerializeField] const float InitPosY = -0.5f;

    [SerializeField] const float CameraDist = 10.0f;

    // =----------------------------= //


    // Start is called before the first frame update
    void Start()
    {
        FieldMGR.instance.SetInitY(InitPosY);
        this.transform.position = FieldMGR.instance.GridToWorld(gridPos);

        playerAnimator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // プレイヤーキャラクターの操作を一括管理する
        this.Controll();

        Camera.main.transform.LookAt(this.transform.position);
    }

    private void Controll()
    {
        if (!IsInput)
        {// 移動中でない場合移動できる
            if (Input.GetKey(KeyCode.RightArrow))
            {// 右
                this.Move(Direct.right);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {// 左
                this.Move(Direct.left);
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {// 奥
                this.Move(Direct.forward);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {// 手前
                this.Move(Direct.back);
            }
            else
            {
                // 立ちモーション
                playerAnimator.Play("Standing@loop");
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
        float rotY = this.transform.rotation.y;

        switch (direct)
        {
            case Direct.right: movedGrid    = new Vector2Int(1, 0);  rotY = 90.0f;  break;
            case Direct.left: movedGrid     = new Vector2Int(-1, 0); rotY = 270.0f; break;
            case Direct.forward: movedGrid  = new Vector2Int(0, 1);  rotY = 0.0f;   break;
            case Direct.back: movedGrid     = new Vector2Int(0, -1); rotY = 180.0f; break;
            default: movedGrid = new Vector2Int(); break;
        }

        this.transform.DORotate(new Vector3(
               this.transform.rotation.x,
               rotY,
               this.transform.rotation.z),
               0.1f);

        if (Mathf.Abs(gridPos.x + movedGrid.x) + FieldMGR.fieldMax / 2 > FieldMGR.fieldMax - 1 ||
                   Mathf.Abs(gridPos.y + movedGrid.y) + FieldMGR.fieldMax / 2 > FieldMGR.fieldMax - 1)
        {// 範囲外
            MessageWindow.instance.AddMessage("範囲外です", Color.red);
            return;
        }

        if (FieldMGR.instance.GridInfos
            [gridPos.x + movedGrid.x + FieldMGR.fieldMax / 2, 
            gridPos.y + movedGrid.y + FieldMGR.fieldMax / 2]
            .Type != FieldInformation.FieldType.wall) 
        {
            if (IsLastMove)
            {
                playerAnimator.Play("Walking@loop");
            }

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
                    MoveTime).SetEase(Ease.Linear); 

            // キー入力フラグon
            IsInput = true;

            IsLastMove = true;

            // MoveTime秒経つまで次の入力を受け付けないようにする
            StartCoroutine(MoveTimer());
        }
        else
        {

            // 移動中フラグon
            IsInput = true;

            // MoveTime秒経つまで次の入力を受け付けないようにする
            StartCoroutine(MoveTimer());

            MessageWindow.instance.AddMessage("壁だこれは", Color.red);
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
        IsInput = false;

        if (!IsLastMove)
        {
            // 立ちモーション
            playerAnimator.Play("Standing@loop");
        }
    }
}
