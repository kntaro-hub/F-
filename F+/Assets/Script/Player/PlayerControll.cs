﻿using System.Collections;
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

    // 初期化済みかどうか
    private static bool isInitialize = false;

    private bool isCameraSet = true;
    public bool IsCameraSet
    {
        get { return isCameraSet; }
        set { isCameraSet = value; }
    }

    int cntDirect = 0;          // 方向を決めるためのカウンタ
    int cntInput = 0;           // 押されたキー数
    bool IsRotButton = false;   // 回転キーが押されているか

    int cntSteps = 0;   // 歩数カウンタ

    Player_Items playerItems;

    // アニメータ
    private Animator playerAnimator;
    public Animator PlayerAnimator
    {
        get { return playerAnimator; }
    }



    // 基本メニューUI
    private UI_BasicMenu ui_BasicMenu;

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
        this.transform.position = MapData.GridToWorld(status.point);
        this.transform.position = new Vector3(
            this.transform.position.x,
            InitPosY,
            this.transform.position.z);

        playerAnimator = this.GetComponent<Animator>();
        playerItems = this.GetComponent<Player_Items>();
        ui_BasicMenu = FindObjectOfType<UI_BasicMenu>();

        status.point = new Point();
        status.direct = Direct.forward;
        status.characterType = CharaType.player;

        this.param.Name = "ユニティちゃん";

        this.Init();
    }

    public void Init()
    {
        if (!isInitialize)
        {// 1階層からの場合初期化をかける
            this.param.atk           = 8;    // ちから
            this.param.maxAtk        = 8;    // ちから最大値
            this.param.level         = 1;    // レベル
            this.param.basicAtk      = DataBase.instance.GetLevelTableEntity(param.level - 1).Atk;    // レベルアップで増える攻撃力
            this.param.hp            = 15;   // 体力
            this.param.maxHp         = 15;   // 体力最大値
            this.param.hunger        = 100;  // 満腹度
            this.param.maxHunger     = 100;  // 満腹度最大値
            this.param.exp           = 0;    // 今まで取得した経験値
            this.param.id            = 0;    // キャラクターID
            // 非装備にする
            this.param.weaponId = DataBase.instance.GetItemTableCount() - 1;
            this.param.shieldId = DataBase.instance.GetItemTableCount() - 1;
            UI_MGR.instance.Ui_Inventory.EquipInventoryID[0] = Actor.Parameter.notEquipValue;
            UI_MGR.instance.Ui_Inventory.EquipInventoryID[1] = Actor.Parameter.notEquipValue;
            isInitialize = true;
        }
        else
        {// 2階層以上の場合は前階層のデータを読み込む
            this.LoadStatus();
        }
    }

    public override void Damage(int damage)
    {
        int calcDamage = this.CalcDamage(damage);

        MessageWindow.instance.AddMessage($"{this.Param.Name}は{calcDamage}のダメージをうけた！", Color.red);

        // ダメージアニメーション
        playerAnimator.Play("Damaged", 0, 0.0f);

        if (this.SubHP(calcDamage))
        {
            this.DestroyObject();
        }
    }

    public override void DestroyObject()
    {
        MessageWindow.instance.AddMessage($"{this.param.Name}はしんでしまった…", Color.red);

        // オブジェクト消去
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        this.Controll();

        this.CalcCameraPos();

        if(status.actType == ActType.TurnEnd)
          this.UpdatePosition();


        if(Input.GetKeyDown( KeyCode.Space))
        {
            //this.SaveStatus();

            
        }

        if(Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log(KeyCode.None.ToString());
        }
    }

    private void UpdatePosition()
    {
        this.transform.position = MapData.GridToWorld(this.status.point);
    }

    private void CalcCameraPos()
    {
        if (isCameraSet)
        {
            Camera.main.transform.position = new Vector3(
                this.transform.position.x,
                this.transform.position.y + 7.0f,
                this.transform.position.z - 4.0f);
            Camera.main.transform.LookAt(this.transform.position);
        }
    }

    private void Controll()
    { 
        if (SequenceMGR.instance.seqType == SequenceMGR.SeqType.keyInput 
            && status.actType == ActType.Wait &&
            SequenceMGR.instance.isControll)
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
                this.status.movedPoint = status.point;
                switch (status.direct)
                {
                    case Direct.right:          this.status.movedPoint.x++; break;
                    case Direct.left:           this.status.movedPoint.x--; break;
                    case Direct.forward:        this.status.movedPoint.y++; break;
                    case Direct.back:           this.status.movedPoint.y--; break;
                    case Direct.right_back:     this.status.movedPoint.x++; this.status.movedPoint.y--; break;
                    case Direct.left_back:      this.status.movedPoint.x--; this.status.movedPoint.y--; break;
                    case Direct.right_forward:  this.status.movedPoint.x++; this.status.movedPoint.y++; break;
                    case Direct.left_forward:   this.status.movedPoint.x--; this.status.movedPoint.y++; break;
                }

                // プレイヤーが移動した場合
                SequenceMGR.instance.CallAct(SequenceMGR.PlayerActType.move);

                // マップ上オブジェクトの消去
                MapData.instance.ResetMapObject(status.point);    // 先に消去と登録をしなければならない

                // マップ上オブジェクトの登録
                MapData.instance.SetMapObject(this.status.movedPoint, MapData.MapObjType.player, param.id);

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
            MapData.MapObjType onObject = MapData.instance.GetMapObject(this.status.movedPoint).objType;
            if (onObject != MapData.MapObjType.none &&
                onObject != MapData.MapObjType.player)
            {
                // 移動失敗時処理
                this.MoveFailure();
                SequenceMGR.instance.ActFailed();
                return false;
            }

            if (MapData.instance.GetMapChipType(this.status.movedPoint.x, this.status.movedPoint.y)
                 != MapData.MapChipType.wall)
            {
                if (IsMove)
                {
                    playerAnimator.Play("Walking@loop");
                }

                status.point = this.status.movedPoint;

                // MoveTime秒経つまで次の入力を受け付けないようにする
                StartCoroutine(MoveTimer());

                // MoveTime秒かけて目的地へ
                this.transform.DOMove(MapData.GridToWorld(this.status.movedPoint), MoveTime).SetEase(Ease.Linear);
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
        MapData.instance.SetMapObject(status.point, MapData.MapObjType.player, param.id);
        MapData.instance.ResetMapObject(this.status.movedPoint);
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

    struct SaveData
    {
        // =--------- パラメータ ---------= //
        public Parameter parameter;

        // =--------- 所持アイテム ---------= //

        public Player_Items.StockItem[] StockItems;
        public int[] equipInventoryID;
    }

    public void SaveStatus()
    {
        StreamWriter writer;

        SaveData saveData = new SaveData();

        saveData.parameter = this.param;

        saveData.StockItems = this.GetComponent<Player_Items>().Stocks.ToArray();
        
        saveData.equipInventoryID = UI_MGR.instance.Ui_Inventory.EquipInventoryID;

        writer = new StreamWriter(Application.dataPath + "/PlayerData.json", false);

        string jsonstr = JsonUtility.ToJson(saveData);
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

        SaveData saveData;

        saveData = JsonUtility.FromJson<SaveData>(datastr);

        #region ステータス反映

        param = saveData.parameter;
        this.GetComponent<Player_Items>().Stocks.AddRange(saveData.StockItems);
        UI_MGR.instance.Ui_Inventory.EquipInventoryID = saveData.equipInventoryID;

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

        // 足元のオブジェクトを起動
        MapData.instance.ActiveMapChip(this.status.point, this);

        // マップ情報上のアイテムを更新
        ItemMGR.instance.UpdateMapObject();
        MapData.ObjectOnTheMap mapObject = MapData.instance.GetMapObject(this.status.point);
        if (mapObject.objType == MapData.MapObjType.item)
        {// アイテムの上に乗った
            // インベントリに収納
            playerItems.AddItem(mapObject.id);

            MapData.instance.ResetMapObject(this.status.point);

            // 取得したアイテムをマップから消す
            ItemMGR.instance.DestroyItem(this.status.point);
        }

        // マップ情報上のプレイヤーを更新
        UI_MGR.instance.Ui_Map.UpdateMapPlayer();

        // 歩数加算
        ++cntSteps;

        if (param.hunger <= 0)
        {
            // 満腹度矯正
            param.hunger = 0;

            // hpを1ずつ減らす
            this.SubHP(1);

            // hpが0以下なら死亡
            if(this.CheckDestroy())
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            if ((cntSteps % 3) == 0)
            {// 3歩ごとに満腹度を1下げる
                --param.hunger;
            }
        }

        // ターンエンド 
        status.actType = ActType.TurnEnd;

        AdDebug.Log(MapData.instance.GetMapChipType(this.status.point).ToString());
    }

    // MoveTime後に敵のターン
    private IEnumerator ActProcTimer()
    {
        // MoveTime秒まつ
        yield return new WaitForSeconds(MoveTime * 0.5f);

        // マップ上の敵更新
        SequenceMGR.instance.MapDataUpdate_Enemy();

        // 攻撃
        MapData.ObjectOnTheMap mapObj = MapData.instance.GetMapObject(status.point + this.GetDirect());
        if (mapObj.objType == MapData.MapObjType.enemy)
        {// 攻撃した先が敵
            if (Percent.Per(90))
            {
                // 敵パラメータを取得
                EnemyBase enemy = SequenceMGR.instance.SearchEnemyFromID(mapObj.id);

                // ダメージ量を計算してhpから減算
                int damage = this.CalcAtk();
                
                enemy.Damage(damage, true);
            }
            else
            {
                MessageWindow.instance.AddMessage("攻撃は外れてしまった。", Color.red);
            }
        }
        // MoveTime秒まつ
        yield return new WaitForSeconds(MoveTime * 0.5f);
        
        StartCoroutine(SequenceMGR.instance.ActProcTimer(MoveTime));

        status.actType = ActType.TurnEnd;
    }

    #region ダメージ計算

    /// <summary>
    /// トルネコ式攻撃力計算
    /// </summary>
    /// <returns>整数の攻撃値</returns>
    public override int CalcAtk()
    {
        // Atk計算                                                                                      // 力の初期値
        float WeaponAtk;
        if (this.param.weaponId != DataBase.instance.GetItemTableCount() - 1)
        {
            WeaponAtk = (this.param.basicAtk * (DataBase.instance.GetItemTableEntity(this.param.weaponId).Atk + this.param.atk - 8.0f) / 16.0f);
        }
        else // なにも装備していない場合
        {
            WeaponAtk = (this.param.basicAtk * 0 + (this.param.atk - 8.0f) / 16.0f);
        }
        int Atk = (int)(this.param.basicAtk + Mathf.Round(WeaponAtk));

        // 計算結果を返す
        return Atk;
    }

    /// <summary>
    /// トルネコ式ダメージ計算
    /// 1を下回った場合、最低1ダメージ
    /// </summary>
    /// <param name="atk">攻撃側の計算後攻撃力</param>
    /// <returns>整数のダメージ値</returns>
    public override int CalcDamage(int Atk)
    {
        if (this.param.shieldId != DataBase.instance.GetItemTableCount() - 1)
        {
            // 防御力計算
            Atk = (int)(Atk * Mathf.Pow((15.0f / 16.0f), DataBase.instance.GetItemTableEntity(this.param.weaponId).Def));  // 攻撃力と基本ダメージ
            Atk = (int)Mathf.Floor(Atk * Random.Range(112, 143) / 128);   // 結果
        }
        else
        {// なにも装備していない場合
         // 防御力計算
            Atk = (int)Mathf.Floor(Atk * Random.Range(112, 143) / 128);   // 結果
        }

        if (Atk < 1)
        {// 計算結果が1を下回った場合

            // 最低でも1ダメージ
            Atk = 1;
        }

        // 計算結果を返す
        return Atk;
    }

    #endregion
}