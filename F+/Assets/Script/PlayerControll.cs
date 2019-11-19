using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.IO;

/// <summary>
/// プレイヤーキャラクターの操作を管理
/// </summary>
public class PlayerControll : Actor
{
    // =--------- 変数宣言 ---------= //

    // 移動キー押下フラグ
    private bool IsLastMove = false;

    private bool IsInit = false;

    int cntDirect = 0;          // 方向を決めるためのカウンタ
    int cntInput = 0;           // 押されたキー数
    bool IsRotButton = false;   // 回転キーが押されているか

    // アニメータ
    private Animator playerAnimator;

    public ActType GetAct
    {
        get { return status.actType; }
        private set { status.actType = value; }
    }

    // =--------- 定数定義 ---------= //

    // 初期Y座標
    [SerializeField] const float InitPosY = -0.5f;

    // 
    [SerializeField] const float CameraDist = 10.0f;

    // =----------------------------= //


    // Start is called before the first frame update
    void Start()
    {
        MapData.instance.SetInitY(InitPosY);
        this.transform.position = FieldMGR.GridToWorld(status.gridPos);
        this.transform.position = new Vector3(
            this.transform.position.x,
            InitPosY,
            this.transform.position.z);

        playerAnimator = this.GetComponent<Animator>();

        status.gridPos = new Point();
        status.direct = Direct.forward;
    }

    public void Init()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // プレイヤーキャラクターの操作を一括管理する
        this.Controll();

        this.CalcCameraPos();

        if(status.actType == ActType.TurnEnd)
          this.UpdatePosition();
    }

    private void UpdatePosition()
    {
        this.transform.position = FieldMGR.GridToWorld(this.status.gridPos);
    }

    private void CalcCameraPos()
    {
        Camera.main.transform.position = new Vector3(
            this.transform.position.x,
            this.transform.position.y + 7.0f,
            this.transform.position.z - 4.0f);
        Camera.main.transform.LookAt(this.transform.position);
    }

    /// <summary>
    /// キャラ一回分の更新
    /// </summary>
    override protected void UpdateProc()
    {
        if(status.actType == ActType.Move)
        {// 移動開始
            this.Move(status.direct);
        }
        else if(status.actType == ActType.Act)
        {// 行動開始

        }
    }

    private void Controll()
    {
        

        if (status.actType == ActType.Wait)
        {// 待機中のみ行動できる

            #region 移動
            this.Controll_Move();
            #endregion

            #region 行動
            this.Controll_Act();
            #endregion
        }

        if (Input.GetKey(KeyCode.S))
        {// セーブ
            this.SaveStatus();
        }
        if (Input.GetKey(KeyCode.L))
        {// ロード
            this.LoadStatus();
        }
    }

    private void Controll_Move()
    {
        cntDirect = 0;          // 方向を決めるためのカウンタ
        cntInput = 0;           // 押されたキー数
        IsRotButton = false;    // 回転キーが押されているか

        if (Input.GetKey(KeyCode.F))
        {
            // 回転フラグon
            IsRotButton = true;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {// 右
            status.direct = Direct.right;
            cntDirect += 1; ++cntInput;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {// 左
            status.direct = Direct.left;
            cntDirect += 2; ++cntInput;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {// 奥
            status.direct = Direct.forward;
            cntDirect += 4; ++cntInput;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {// 手前
            status.direct = Direct.back;
            cntDirect += 6; ++cntInput;
        }
        else
        {
            // 立ちモーション
            playerAnimator.Play("Standing@loop");
        }

        if (cntInput == 2)
        {
            switch (cntDirect)
            {
                case (int)Direct.right_forward:
                    status.direct = Direct.right_forward;
                    break;

                case (int)Direct.left_forward:
                    status.direct = Direct.left_forward;
                    break;

                case (int)Direct.right_back:
                    status.direct = Direct.right_back;
                    break;

                case (int)Direct.left_back:
                    status.direct = Direct.left_back;
                    break;
            }
        }

        if (cntDirect > 0)
        {
            if (!IsRotButton)
            {
                status.actType = ActType.Move;
                this.UpdateProc();
            }
            else
            {
                this.ChangeRotate();
            }
        }
    }

    private void Controll_Act()
    {
        //if(cntDirect == 0 || cntInput == 2)
        {// 移動していない場合
            if(Input.GetKeyDown(KeyCode.Return))
            {
                this.Attack();
            }
        }
    }

    private void Attack()
    {
        this.transform.DOPunchPosition(FieldMGR.GridToWorld(this.GetDirect()), MoveTime);
        status.actType = ActType.Act;
        StartCoroutine(AtkTimer());
    }

    /// <summary>
    /// 移動用関数
    /// </summary>
    /// <param name="Direct">目的地</param>
    private void Move(Direct direct)
    {
        Vector3 moveValue = new Vector3();
        Point movedGrid;
        float rotY = this.transform.rotation.y;

        if (cntInput == 2)
        {
            switch (direct)
            {
                case Direct.right_forward:
                    direct = Direct.right;
                    break;

                case Direct.left_forward:
                    direct = Direct.left;
                    break;

                case Direct.right_back:
                    direct = Direct.right;
                    break;

                case Direct.left_back:
                    direct = Direct.left;
                    break;
            }
        }

        switch (direct)
        {
            case Direct.right: movedGrid = new Point(1, 0); rotY = 90.0f; break;
            case Direct.left: movedGrid = new Point(-1, 0); rotY = 270.0f; break;
            case Direct.forward: movedGrid = new Point(0, 1); rotY = 0.0f; break;
            case Direct.back: movedGrid = new Point(0, -1); rotY = 180.0f; break;
            default: movedGrid = new Point(); break;
        }

        this.transform.DORotate(new Vector3(
               this.transform.rotation.x,
               rotY,
               this.transform.rotation.z),
               0.1f);
        status.direct = direct;

        //if (status.gridPos.x + movedGrid.x < 0 ||
        //    status.gridPos.x + movedGrid.x > MapData.instance.Width - 1 ||
        //    status.gridPos.y + movedGrid.y < 0 ||
        //    status.gridPos.y + movedGrid.y > MapData.instance.Height - 1)
        //{// 範囲外
        //    MessageWindow.instance.AddMessage("範囲外です", Color.red);

        //    // MoveTime秒経つまで次の入力を受け付けないようにする
        //    StartCoroutine(MoveTimer(true));
        //    return;
        //}

        if (MapData.instance.GridInfos
            [status.gridPos.x + movedGrid.x,
            status.gridPos.y + movedGrid.y]
            .Type != FieldInformation.FieldType.wall)
        {
            status.actType = ActType.Move;

            if (IsLastMove)
            {
                playerAnimator.Play("Walking@loop");
            }

            switch (direct)
            {
                case Direct.right:
                    moveValue = Vector3.right;
                    status.gridPos.x++;
                    MessageWindow.instance.AddMessage("右に進みました", Color.white);
                    break;
                case Direct.left:
                    moveValue = Vector3.left;
                    status.gridPos.x--;
                    MessageWindow.instance.AddMessage("左に進みました", Color.white);
                    break;
                case Direct.forward:
                    moveValue = Vector3.forward;
                    status.gridPos.y++;
                    MessageWindow.instance.AddMessage("奥に進みました", Color.white);
                    break;
                case Direct.back:
                    moveValue = Vector3.back;
                    status.gridPos.y--;
                    MessageWindow.instance.AddMessage("手前に進みました", Color.white);
                    break;
            }
            IsLastMove = true;

            // MoveTime秒経つまで次の入力を受け付けないようにする
            StartCoroutine(MoveTimer(false));

            // 敵を動かす
            SequenceMGR.instance.ActProc();

            moveValue *= FieldInformation.GridSize;
            // MoveTime秒かけて目的地へ
            this.transform.DOMove(new Vector3(
                    this.transform.position.x + moveValue.x,
                    this.transform.position.y + moveValue.y,
                    this.transform.position.z + moveValue.z),
                    MoveTime).SetEase(Ease.Linear);
        }
        else
        {
            // MoveTime秒経つまで次の入力を受け付けないようにする
            StartCoroutine(MoveTimer(true));

            MessageWindow.instance.AddMessage("壁だこれは", Color.red);
        }
    }

    private void ChangeRotate()
    {
        float rotY = this.transform.rotation.y;

        switch (status.direct)
        {
            case Direct.right:          rotY = 90.0f; break;
            case Direct.left:           rotY = 270.0f; break;
            case Direct.forward:        rotY = 0.0f; break;
            case Direct.back:           rotY = 180.0f; break;
            case Direct.right_forward:  rotY = 45.0f; break;
            case Direct.left_forward:   rotY = 315.0f; break;
            case Direct.right_back:     rotY = 135.0f; break;
            case Direct.left_back:      rotY = 225.0f; break;
            default: break;
        }

        this.transform.rotation = (Quaternion.Euler(
               this.transform.rotation.x,
               rotY,
               this.transform.rotation.z));
    }
   
    private void SaveStatus()
    {
        StreamWriter writer;

        writer = new StreamWriter(Application.dataPath + "/PlayerData.json", false);

        string jsonstr = JsonUtility.ToJson(status);
        jsonstr = jsonstr + "\n";
        writer.Write(jsonstr);
        writer.Flush();

        writer.Close();
    }

    private void LoadStatus()
    {
        string datastr = "";
        StreamReader reader;
        reader = new StreamReader(Application.dataPath + "/PlayerData.json");
        datastr = reader.ReadToEnd();
        reader.Close();

        status = JsonUtility.FromJson<Status>(datastr);

        #region ステータス反映

        this.transform.position = FieldMGR.GridToWorld(status.gridPos);
        float rotY = this.transform.rotation.y;

        switch (status.direct)
        {
            case Direct.right:  rotY = 90.0f; break;
            case Direct.left:   rotY = 270.0f; break;
            case Direct.forward:rotY = 0.0f; break;
            case Direct.back:   rotY = 180.0f; break;
            default:  break;
        }

        this.transform.rotation = (Quaternion.Euler(
               this.transform.rotation.x,
               rotY,
               this.transform.rotation.z));

        #endregion
    }

    private Point GetDirect()
    {
        switch(status.direct)
        {
            case Direct.right:          return new Point(1, 0);
            case Direct.left:           return new Point(-1, 0);
            case Direct.forward:        return new Point(0, 1);
            case Direct.back:           return new Point(0, -1);
            case Direct.right_forward:  return new Point(1, 1);
            case Direct.left_forward:   return new Point(-1, 1);
            case Direct.right_back:     return new Point(1, -1);
            case Direct.left_back:      return new Point(-1, -1);
            default: return new Point(0, 0);
        }
    }

    public Point GetPoint()
    {
        return FieldMGR.WorldToGrid(this.transform.position);
    }

    /// <summary>
    /// 指定の時間が経ったら入力を受け付けられるようにする
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveTimer(bool isWall)
    {
        // MoveTime秒まつ
        yield return new WaitForSeconds(MoveTime);

        if (!IsLastMove)
        {
            // 立ちモーション
            playerAnimator.Play("Standing@loop");
        } 
        else
        {

        }
        status.actType = ActType.TurnEnd;
        if (SequenceMGR.instance.IsTurnEnd())
        {
            SequenceMGR.instance.ResetAct();
        }
        if(isWall)
        {
            SequenceMGR.instance.ResetAct();
        }
    }

    private IEnumerator AtkTimer()
    {
        // MoveTime秒まつ
        yield return new WaitForSeconds(MoveTime);

        StartCoroutine(ActProcTimer());
    }


    // MoveTime後に敵のターン
    private IEnumerator ActProcTimer()
    {
        // MoveTime秒まつ
        yield return new WaitForSeconds(MoveTime * 0.5f);

        // 攻撃
        EnemyBase obj = SequenceMGR.instance.GetEnemyFromPoint(status.gridPos + this.GetDirect());
        if(obj != null) obj.Destroy();
        SequenceMGR.instance.CheckDestroy();

        // MoveTime秒まつ
        yield return new WaitForSeconds(MoveTime * 0.5f);

        SequenceMGR.instance.ActProc();
        status.actType = ActType.TurnEnd;

        if (SequenceMGR.instance.IsTurnEnd())
        {
            SequenceMGR.instance.ResetAct();
        }
    }
}