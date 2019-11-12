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


    // キー入力待ちフラグ
    private bool IsInput = false;

    // 移動キー押下フラグ
    private bool IsLastMove = false;

    // プレイヤー行動中フラグ
    private bool IsAct = false;

    private bool IsInit = false;

    public bool GetIsAct
    {
        get { return IsAct; }
    }


    // アニメータ
    private Animator playerAnimator;

    private Vector3 toCameraVector = new Vector3();

    public ActType GetAct
    {
        get { return actType; }
        private set { actType = value; }
    }

    // =--------- 定数定義 ---------= //

    // 移動にかかる時間
    [SerializeField] const float MoveTime = 0.1f;

    // 初期Y座標
    [SerializeField] const float InitPosY = -0.5f;

    // 
    [SerializeField] const float CameraDist = 10.0f;

    // =----------------------------= //


    // Start is called before the first frame update
    void Start()
    {
        FieldMGR.instance.SetInitY(InitPosY);
        this.transform.position = FieldMGR.instance.GridToWorld(status.gridPos);

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

        Camera.main.transform.LookAt(this.transform.position);

        //if(Input.GetKeyDown(KeyCode.G))
        //{
        //    this.status.gridPos = AStarSys.instance.A_StarProc_Single();
        //    this.transform.DOMove(
        //        FieldMGR.instance.GridToWorld(status.gridPos), MoveTime);
        //}
        //if (Input.GetKeyDown(KeyCode.J))
        //{
        //    AStarSys.instance.SetGoal(new Point(0,0));
        //}
    }

    /// <summary>
    /// キャラ一回分の更新
    /// </summary>
    override protected void UpdateProc()
    {
        if(actType == ActType.MoveBegin)
        {// 移動開始
            this.Move(status.direct);
        }
        else if(actType == ActType.ActBegin)
        {// 行動開始

        }
    }

    private void Controll()
    {
        

        if (actType == ActType.Wait)
        {// 移動中でない場合移動できる

            // Wait状態の場合は行動中フラグoff
            IsAct = false;

            if (Input.GetKey(KeyCode.RightArrow))
            {// 右
                status.direct = Direct.right;
                actType = ActType.MoveBegin;
                this.UpdateProc();
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {// 左
                status.direct = Direct.left;
                actType = ActType.MoveBegin;
                this.UpdateProc();
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {// 奥
                status.direct = Direct.forward;
                actType = ActType.MoveBegin;
                this.UpdateProc();
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {// 手前
                status.direct = Direct.back;
                actType = ActType.MoveBegin;
                this.UpdateProc();
            }
            else
            {
                // 立ちモーション
                playerAnimator.Play("Standing@loop");
            }
        }
        else
        {
            // Wait状態でない場合は行動中フラグon
            IsAct = true;
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

        this.transform.position = FieldMGR.instance.GridToWorld(status.gridPos);
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

    /// <summary>
    /// 移動用関数
    /// </summary>
    /// <param name="Direct">目的地</param>
    private void Move(Direct direct)
    {
        Vector3 moveValue = new Vector3();
        Point movedGrid;
        float rotY = this.transform.rotation.y;

        switch (direct)
        {
            case Direct.right: movedGrid    = new Point(1, 0);  rotY = 90.0f;  break;
            case Direct.left: movedGrid     = new Point(-1, 0); rotY = 270.0f; break;
            case Direct.forward: movedGrid  = new Point(0, 1);  rotY = 0.0f;   break;
            case Direct.back: movedGrid     = new Point(0, -1); rotY = 180.0f; break;
            default: movedGrid = new Point(); break;
        }

        this.transform.DORotate(new Vector3(
               this.transform.rotation.x,
               rotY,
               this.transform.rotation.z),
               0.1f);
        status.direct = direct;

        if (status.gridPos.x + movedGrid.x < 0 ||
            status.gridPos.x + movedGrid.x > MapData.instance.Width - 1 ||
            status.gridPos.y + movedGrid.y < 0 ||
            status.gridPos.y + movedGrid.y > MapData.instance.Height - 1)
        {// 範囲外
            MessageWindow.instance.AddMessage("範囲外です", Color.red);

            // 移動中フラグon
            IsInput = true;

            // MoveTime秒経つまで次の入力を受け付けないようにする
            StartCoroutine(MoveTimer());
            return;
        }

        if (FieldMGR.instance.GridInfos
            [status.gridPos.x + movedGrid.x,
            status.gridPos.y + movedGrid.y]
            .Type != FieldInformation.FieldType.wall) 
        {
            actType = ActType.Move;

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

            // キー入力フラグon
            IsInput = true;

            IsLastMove = true;

            // MoveTime秒経つまで次の入力を受け付けないようにする
            StartCoroutine(MoveTimer());

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

            // 移動中フラグon
            IsInput = true;

            // MoveTime秒経つまで次の入力を受け付けないようにする
            StartCoroutine(MoveTimer());

            MessageWindow.instance.AddMessage("壁だこれは", Color.red);
        }
    }

    public Point GetPoint()
    {
        return FieldMGR.instance.WorldToGrid(this.transform.position);
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

        actType = ActType.TurnEnd;

        if (!IsLastMove)
        {
            // 立ちモーション
            playerAnimator.Play("Standing@loop");
        } 
        else
        {

        }

        actType = ActType.Wait;
    }
}
