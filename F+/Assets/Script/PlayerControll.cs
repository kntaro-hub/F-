using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// プレイヤーキャラクターの操作を管理
/// </summary>
public class PlayerControll : MonoBehaviour
{
    // 移動中フラグ
    bool IsMove = false;

    // 移動にかかる時間
    [SerializeField] const float MoveTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
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
                this.Move(new Vector3(1.0f, 0.0f, 0.0f) * FieldInfomation.GridSize);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {// 左
                this.Move(new Vector3(-1.0f, 0.0f, 0.0f) * FieldInfomation.GridSize);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {// 奥
                this.Move(new Vector3(0.0f, 0.0f, 1.0f) * FieldInfomation.GridSize);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {// 手前
                this.Move(new Vector3(0.0f, 0.0f, -1.0f) * FieldInfomation.GridSize);
            }
        }
    }

    /// <summary>
    /// 移動用関数
    /// </summary>
    /// <param name="Direct">目的地</param>
    private void Move(Vector3 Direct)
    {
        // MoveTime秒かけて目的地へ
        this.transform.DOMove(new Vector3(
                this.transform.position.x + Direct.x,
                this.transform.position.y + Direct.y,
                this.transform.position.z + Direct.z),
                MoveTime).SetEase(Ease.InOutCubic);
        // 移動中フラグon
        IsMove = true;

        // MoveTime秒経つまで次の入力を受け付けないようにする
        StartCoroutine(MoveTimer());
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
