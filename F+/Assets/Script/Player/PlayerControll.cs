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
    private bool IsMove = false;

    private bool IsInit = false;

    int cntDirect = 0;          // 方向を決めるためのカウンタ
    int cntInput = 0;           // 押されたキー数
    bool IsRotButton = false;   // 回転キーが押されているか

    // アニメータ
    private Animator playerAnimator;
    public Animator PlayerAnimator
    {
        get { return playerAnimator; }
    }



    // 基本メニューUI
    private UI_BasicMenu ui_BasicMenu;

    // 移動後の予定座標
    private Point movedPoint;
    public Point MovedPoint
    {
        get { return movedPoint; }
    }

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
        this.transform.position = MapData.GridToWorld(status.gridPos);
        this.transform.position = new Vector3(
            this.transform.position.x,
            InitPosY,
            this.transform.position.z);

        playerAnimator = this.GetComponent<Animator>();
        ui_BasicMenu = FindObjectOfType<UI_BasicMenu>();

        status.gridPos = new Point();
        status.direct = Direct.forward;

        this.Init();
    }

    public void Init()
    {
        param.atk = 1;
        param.hp = 3;
        param.id = 0;
        param.def = 1;
    }

    // Update is called once per frame
    void Update()
    {
        //// プレイヤーキャラクターの操作を一括管理する
        //if (SequenceMGR.instance.seqType == SequenceMGR.SeqType.keyInput)
        //{// メニューを表示していない間だけ操作可能にする
            this.Controll();
        //}

        this.CalcCameraPos();

        if(status.actType == ActType.TurnEnd)
          this.UpdatePosition();
    }

    private void UpdatePosition()
    {
        this.transform.position = MapData.GridToWorld(this.status.gridPos);
    }

    private void CalcCameraPos()
    {
        Camera.main.transform.position = new Vector3(
            this.transform.position.x,
            this.transform.position.y + 7.0f,
            this.transform.position.z - 4.0f);
        Camera.main.transform.LookAt(this.transform.position);
    }

    private void Controll()
    { 
        if (SequenceMGR.instance.seqType == SequenceMGR.SeqType.keyInput && status.actType == ActType.Wait)
        {// 待機中のみ行動できる

            #region 移動
            this.Controll_Move();
            #endregion

            #region 行動
            this.Controll_Act();
            #endregion
        }
    }

    private void Controll_Move()
    {
        {
            IsRotButton = false;    // 回転キーが押されているか

            if (Input.GetKey(KeyCode.F))
            {
                // 回転フラグon
                IsRotButton = true;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {// 右
                if(Input.GetKey(KeyCode.UpArrow))
                {
                    status.direct = Direct.right_forward;
                }
                else if(Input.GetKey(KeyCode.DownArrow))
                {
                    status.direct = Direct.right_back;
                }
                else status.direct = Direct.right;
                this.MoveReserve();
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {// 左
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    status.direct = Direct.left_forward;
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    status.direct = Direct.left_back;
                }
                else status.direct = Direct.left;
                this.MoveReserve();
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {// 奥
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    status.direct = Direct.right_forward;
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    status.direct = Direct.left_forward;
                }
                else status.direct = Direct.forward;
                this.MoveReserve();
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {// 手前
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    status.direct = Direct.right_back;
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    status.direct = Direct.left_back;
                }
                else status.direct = Direct.back;
                this.MoveReserve();
            }
        }
    }

    private void MoveReserve()
    {
        if (!IsRotButton)
        {
            {
                // この時点で移動後座標を更新する
                movedPoint = status.gridPos;
                switch (status.direct)
                {
                    case Direct.right:          movedPoint.x++; break;
                    case Direct.left:           movedPoint.x--; break;
                    case Direct.forward:        movedPoint.y++; break;
                    case Direct.back:           movedPoint.y--; break;
                    case Direct.right_back:     movedPoint.x++; movedPoint.y--; break;
                    case Direct.left_back:      movedPoint.x--; movedPoint.y--; break;
                    case Direct.right_forward:  movedPoint.x++; movedPoint.y++; break;
                    case Direct.left_forward:   movedPoint.x--; movedPoint.y++; break;
                }

                // プレイヤーが移動した場合
                SequenceMGR.instance.CallAct(SequenceMGR.PlayerActType.move);

                // マップ上オブジェクトの消去
                MapData.instance.ResetMapObject(status.gridPos);    // 先に消去と登録をしなければならない

                // マップ上オブジェクトの登録
                MapData.instance.SetMapObject(movedPoint, MapData.MapObjType.player, param.id);

                // 移動状態に遷移
                status.actType = ActType.Move;

                // 予約を一件実行
                SequenceMGR.instance.ActProc();
            }
        }
        else
        {
            this.ChangeRotate();
        }
    }

    private void Controll_Act()
    {
        {// 移動していない場合
            if(Input.GetKeyDown(KeyCode.P))
            {
                // プレイヤーが行動した場合
                SequenceMGR.instance.CallAct(SequenceMGR.PlayerActType.act);

                // 行動状態にする
                this.status.actType = ActType.Act;

                // 予約を一件実行
                SequenceMGR.instance.ActProc();                
            }
        }
    }

    public void Attack()
    {
        this.transform.DOPunchPosition(MapData.GridToWorld(this.GetDirect()), MoveTime);
        status.actType = ActType.Act;
        StartCoroutine(ActProcTimer());
    }

    /// <summary>
    /// 移動用関数
    /// </summary>
    /// <param name="Direct">目的地</param>
    public bool Move()
    {
        if (status.actType == ActType.Move)
        {
            this.ChangeRotate();

            // ここでマップに登録してある敵を更新
            SequenceMGR.instance.MapDataUpdate_Enemy();

            // 進む先にオブジェクトがあれば進まない
            MapData.MapObjType onObject = MapData.instance.GetMapObject(movedPoint).objType;
            if (onObject != MapData.MapObjType.none &&
                onObject != MapData.MapObjType.player)
            {
                // 移動失敗時処理
                this.MoveFailure();
                SequenceMGR.instance.ActFailed();
                return false;
            }

            if (MapData.instance.GetValue(movedPoint.x, movedPoint.y)
                 != (int)MapData.MapChipType.wall)
            {
                if (IsMove)
                {
                    playerAnimator.Play("Walking@loop");
                }

                status.gridPos = movedPoint;

                // MoveTime秒経つまで次の入力を受け付けないようにする
                StartCoroutine(MoveTimer());

                // MoveTime秒かけて目的地へ
                this.transform.DOMove(MapData.GridToWorld(movedPoint), MoveTime).SetEase(Ease.Linear);
                return true;
            }
            else
            {
                // 移動失敗時処理
                this.MoveFailure();

                SequenceMGR.instance.ActFailed();
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 移動失敗時処理
    /// </summary>
    private void MoveFailure()
    {
        // 移動失敗時処理

        // マップを移動前の状態に戻す
        MapData.instance.SetMapObject(status.gridPos, MapData.MapObjType.player, param.id);
        MapData.instance.ResetMapObject(movedPoint);
    }

    /// <summary>
    /// ダメージ処理
    /// </summary>
    public void Damage(int atk)
    {
        // ダメージ中にする
        status.actType = ActType.Damage;

        // ダメージ量を計算してhpから減算
        this.param.hp -= this.param.CalcDamage(atk);

        // hpが0以下なら死亡
        if (this.param.hp <= 0)
        {
            Destroy(this.gameObject);
        }

        StartCoroutine(this.DamagedTimer());
        playerAnimator.Play("Damaged", 0, 0.0f);
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

        this.transform.position = MapData.GridToWorld(status.gridPos);
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
            case Direct.right:          return new Point( 1,  0);
            case Direct.left:           return new Point(-1,  0);
            case Direct.forward:        return new Point( 0,  1);
            case Direct.back:           return new Point( 0, -1);
            case Direct.right_forward:  return new Point( 1,  1);
            case Direct.left_forward:   return new Point(-1,  1);
            case Direct.right_back:     return new Point( 1, -1);
            case Direct.left_back:      return new Point(-1, -1);
            default: return new Point(0, 0);
        }
    }

    public Point GetPoint()
    {
        return MapData.WorldToGrid(this.transform.position);
    }

    /// <summary>
    /// 指定の時間が経ったら入力を受け付けられるようにする
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveTimer()
    {
        // MoveTime秒まつ
        yield return new WaitForSeconds(MoveTime);

        if (!IsMove)
        {
            // 立ちモーション
            playerAnimator.Play("Standing@loop");
        } 
        // マップ情報上のプレイヤーを更新
        UI_MGR.instance.Ui_Map.UpdateMapPlayer();

        // 足元がゴールかチェック
        if(MapData.instance.CheckGoal(status.gridPos))
        {
            // ゴールUIを表示する
            UI_MGR.instance.Ui_Goal.ShowMenu();
        }
        status.actType = ActType.TurnEnd;
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
        status.actType = ActType.TurnEnd;
        // MoveTime秒まつ
        yield return new WaitForSeconds(MoveTime * 0.5f);

        // 攻撃
        MapData.ObjectOnTheMap mapObj = MapData.instance.GetMapObject(status.gridPos + this.GetDirect());
        if (mapObj.objType == MapData.MapObjType.enemy)
        {// 攻撃した先が敵
            // 敵パラメータを取得
            EnemyBase enemy = SequenceMGR.instance.SearchEnemyFromID(mapObj.id);

            // ダメージ量を計算してhpから減算
            // 一時変数に値をコピー（こうしないとParamは参照型のためコンパイルエラーとなる）
            Parameter enemyParam = enemy.Param; 
            enemyParam.hp -= enemy.Param.CalcDamage(this.param.atk);
            enemy.Param = enemyParam;

            // hpが0以下なら死亡
            if(enemy.Param.hp <= 0)
            {
                SequenceMGR.instance.DestroyEnemyFromID(enemy.Param.id);
            }
        }
        // MoveTime秒まつ
        yield return new WaitForSeconds(MoveTime * 0.5f);
    }
    private IEnumerator DamagedTimer()
    {
        // MoveTime秒まつ
        yield return new WaitForSeconds(MoveTime);

        // 操作可能にする
        status.actType = ActType.TurnEnd;
    }
}