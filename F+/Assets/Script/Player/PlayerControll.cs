using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.IO;
using UnityEngine.UI;

/// <summary>
/// プレイヤーキャラクターの操作を管理
/// </summary>
public class PlayerControll : Actor
{
    // =--------- 変数宣言 ---------= //

    // 初期化済みかどうか
    private static bool isInitialize = false;

    // カメラ固定フラグ
    private bool isCameraSet = true;
    public bool IsCameraSet
    {
        get { return isCameraSet; }
        set { isCameraSet = value; }
    }

    private bool isSkip = false;        // スキップボタンが押されているか
    int cntSteps = 0;   // 歩数カウンタ

    // メインカメラ
    private Camera mainCamera;

    // アイテムリスト
    Player_Items playerItems;

    // アニメータ
    private Animator playerAnimator;
    public Animator PlayerAnimator
    {
        get { return playerAnimator; }
    }
    // =--------- 定数定義 ---------= //

    // 初期Y座標
    [SerializeField] private float InitPosY = -0.5f;

    // 腹減り割合
    [SerializeField] private float HungerBorder = 15.0f;

    // 腹減り歩数
    [SerializeField] private int StemNum_SubHunger = 10;

    // 表情変更用
    [SerializeField] private UI_PlayerImage UIPlayerFace;

    // UIのプレイヤー体力ゲージ
    [SerializeField] private Slider HpGauge;

    // =----------------------------= //

    /// <summary>
    /// 操作タイプ
    /// </summary>
    private enum ControllMode
    {
        normal = 0,    // 通常移動
        diagonally,      // 斜め移動 
        rot,                // 回転
        max
    }
    // 操作タイプ変数
    private ControllMode controllMode = ControllMode.normal;

    // 操作タイプのデリゲート型宣言
    delegate void ControllUpdate();

    // 操作タイプ数分生成
    private ControllUpdate[] controllUpdate = new ControllUpdate[(int)ControllMode.max];

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = MapData.GridToWorld(status.point, InitPosY);

        playerAnimator = this.GetComponent<Animator>();
        playerItems = this.GetComponent<Player_Items>();

        status.point = new Point();
        status.direct = Direct.forward;
        status.characterType = CharaType.player;

        this.param.Name = "ユニティちゃん";

        this.Init();

        mainCamera = Camera.main;

        this.ChangeExpression();
    }

    public void Init()
    {
        if (!isInitialize)
        {// 1階層からの場合初期化をかける
            this.param.atk          = 8;    // ちから
            this.param.maxAtk    = 8;    // ちから最大値
            this.param.level        = 1;    // レベル
            this.param.basicAtk  = DataBase.instance.GetLevelTableEntity(param.level - 1).Atk;    // レベルアップで増える攻撃力
            this.param.hp          = 15;   // 体力
            this.param.maxHp    = 15;   // 体力最大値
            this.param.hunger    = 100;  // 満腹度
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

        // 操作タイプごとの関数ポインタを設定
        controllUpdate[(int)ControllMode.normal]        = this.ControllMode_Normal;
        controllUpdate[(int)ControllMode.diagonally]    = this.Controll_Diagonally;
        controllUpdate[(int)ControllMode.rot]               = this.Controll_Rot;
    }

public override void Damage(int damage)
    {
        int calcDamage = this.CalcDamage(damage);

        MessageWindow.instance.AddMessage($"{this.Param.Name}は{calcDamage}のダメージをうけた！", Color.red);

        // プレイヤー画像をダメージ表情に変更
        UIPlayerFace.ChangeExpression(UI_PlayerImage.Face.damage, true);

        // ダメージエフェクト
        EffectMGR.instance.CreateEffect(EffectMGR.EffectType.Hit_White, this.transform.position);

        if (this.SubHP(calcDamage))
        {
            StartCoroutine(this.DeadAnimation());
        }
        else
        {
            // ダメージアニメーション
            playerAnimator.Play("Damaged");
        }
    }

    private IEnumerator DeadAnimation()
    {
        // 死亡時アニメーション
        playerAnimator.Play("GoDown");

        // 時間をゆっくりに
        Time.timeScale = 0.2f;

        // プレイヤーにカメラを近づける
        mainCamera.transform.DOMove(
            mainCamera.transform.position - ((mainCamera.transform.position - this.transform.position) * 0.5f),
            0.4f);

        // カメラ更新停止
        this.isCameraSet = false;

        // プレイヤーは操作不能に
        SequenceMGR.instance.seqType = SequenceMGR.SeqType.moveImpossible;

        yield return new WaitForSeconds(0.4f);

        // 時間を元に戻す
        Time.timeScale = 1.0f;

        // 半透明フェード
        Fade.instance.Translucent();
    }

    public override void DestroyObject()
    {
        MessageWindow.instance.AddMessage($"{this.param.Name}はしんでしまった…", Color.red);

        // オブジェクト消去
        Destroy(this.gameObject);

        // 死亡エフェクト
        EffectMGR.instance.CreateEffect(EffectMGR.EffectType.Player_Dead, this.status.point);
    }

    // Update is called once per frame
    void Update()
    {
        this.Controll();

        this.CalcCameraPos();


        // 体力ゲージ更新
        this.HpGauge.value = ((float)this.param.hp / param.maxHp);
    }

    private void CalcCameraPos()
    {
        if (isCameraSet)
        {
            mainCamera.transform.position = new Vector3(
                this.transform.position.x,
                this.transform.position.y + 7.0f,
                this.transform.position.z - 4.0f);
            mainCamera.transform.LookAt(this.transform.position);
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
        controllMode = ControllMode.normal;
       isSkip = false;         // スキップボタンが押されているか
     
       if (PS4Input.GetButton(PS4ButtonCode.R1))
       {
           // 斜め移動on
           controllMode = ControllMode.diagonally;
       }
     
       if(PS4Input.GetButton(PS4ButtonCode.Square))
       {
           // 回転モードon
           controllMode = ControllMode.rot;
       }
     
       if (PS4Input.GetButton(PS4ButtonCode.Cross) && !this.IsHunger())
       {
           // スキップフラグon
           isSkip = true;
       }

       // 操作モードごとの更新
        controllUpdate[(int)controllMode]();
    }

    #region 操作タイプごとの更新処理

    private void ControllMode_Normal()
    {
        if (PS4Input.GetCrossKey(PS4KeyCodeLR.CrossKey_R, isSkip))
        {// 右
            status.direct = Direct.right;
            this.DecideMove();
        }
        else if (PS4Input.GetCrossKey(PS4KeyCodeLR.CrossKey_L, isSkip))
        {// 左
            status.direct = Direct.left;
            this.DecideMove();
        }
        else if (PS4Input.GetCrossKey(PS4KeyCodeUD.CrossKey_U, isSkip))
        {// 奥
            status.direct = Direct.forward;
            this.DecideMove();
        }
        else if (PS4Input.GetCrossKey(PS4KeyCodeUD.CrossKey_D, isSkip))
        {// 手前
            status.direct = Direct.back;
            this.DecideMove();
        }
    }

    private void Controll_Diagonally()
    {
        if (PS4Input.GetCrossKey(PS4KeyCodeLR.CrossKey_R, isSkip) && PS4Input.GetCrossKey(PS4KeyCodeUD.CrossKey_U, isSkip))
        {// 右上
            status.direct = Direct.right_forward;
            this.DecideMove();
        }
        else if (PS4Input.GetCrossKey(PS4KeyCodeLR.CrossKey_L, isSkip) && PS4Input.GetCrossKey(PS4KeyCodeUD.CrossKey_U, isSkip))
        {// 左上
            status.direct = Direct.left_forward;
            this.DecideMove();
        }
        else if (PS4Input.GetCrossKey(PS4KeyCodeLR.CrossKey_R, isSkip) && PS4Input.GetCrossKey(PS4KeyCodeUD.CrossKey_D, isSkip))
        {// 右下
            status.direct = Direct.right_back;
            this.DecideMove();
        }
        else if (PS4Input.GetCrossKey(PS4KeyCodeLR.CrossKey_L, isSkip) && PS4Input.GetCrossKey(PS4KeyCodeUD.CrossKey_D, isSkip))
        {// 手前
            status.direct = Direct.left_back;
            this.DecideMove();
        }
    }

    private void Controll_Rot()
    {
        if (PS4Input.GetCrossKey(PS4KeyCodeLR.CrossKey_R) && PS4Input.GetCrossKey(PS4KeyCodeUD.CrossKey_U))
        {// 右上
            status.direct = Direct.right_forward;
            this.ChangeRotate();
        }
        else if (PS4Input.GetCrossKey(PS4KeyCodeLR.CrossKey_L) && PS4Input.GetCrossKey(PS4KeyCodeUD.CrossKey_U))
        {// 左上
            status.direct = Direct.left_forward;
            this.ChangeRotate();
        }
        else if (PS4Input.GetCrossKey(PS4KeyCodeLR.CrossKey_R) && PS4Input.GetCrossKey(PS4KeyCodeUD.CrossKey_D))
        {// 右下
            status.direct = Direct.right_back;
            this.ChangeRotate();
        }
        else if (PS4Input.GetCrossKey(PS4KeyCodeLR.CrossKey_L) && PS4Input.GetCrossKey(PS4KeyCodeUD.CrossKey_D))
        {// 手前
            status.direct = Direct.left_back;
            this.ChangeRotate();
        }
        else if (PS4Input.GetCrossKey(PS4KeyCodeLR.CrossKey_R))
        {// 右
            status.direct = Direct.right;
            this.ChangeRotate();
        }
        else if (PS4Input.GetCrossKey(PS4KeyCodeLR.CrossKey_L))
        {// 左
            status.direct = Direct.left;
            this.ChangeRotate();
        }
        else if (PS4Input.GetCrossKey(PS4KeyCodeUD.CrossKey_U))
        {// 奥
            status.direct = Direct.forward;
            this.ChangeRotate();
        }
        else if (PS4Input.GetCrossKey(PS4KeyCodeUD.CrossKey_D))
        {// 手前
            status.direct = Direct.back;
            this.ChangeRotate();
        }
    }

    #endregion

    private void DecideMove()
    {
        if(isSkip) SequenceMGR.instance.CallAct(SequenceMGR.PlayerActType.skip);
        else        this.MoveReserve();
    }

    public void MoveReserve()
    {
        // この時点で移動後座標を更新する
        this.status.movedPoint = status.point;

        // 向きに合わせて移動後座標を取得
        this.status.movedPoint += MapData.DirectPoints[(int)this.status.direct];

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

    public void SkipReserve()
    {
        // この時点で移動後座標を更新する
        this.status.movedPoint = status.point;

        // 向きに合わせて移動後座標を取得
        this.status.movedPoint += MapData.DirectPoints[(int)this.status.direct];

        // マップ上オブジェクトの消去
        MapData.instance.ResetMapObject(status.point);    // 先に消去と登録をしなければならない

        // マップ上オブジェクトの登録
        MapData.instance.SetMapObject(this.status.movedPoint, MapData.MapObjType.player, param.id);

        // 向きを変える
        this.ChangeRotate();
    }

    public override void UpdatePosition()
    {
        this.transform.position = MapData.GridToWorld(this.status.point, InitPosY);
    }

    private void Controll_Act()
    {
        {// 移動していない場合
            if(PS4Input.GetButtonDown(PS4ButtonCode.Circle))
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
        playerAnimator.Play("Attack");
        status.actType = ActType.Act;
        this.transform.DOPunchPosition(MapData.GridToWorld(MapData.DirectPoints[(int)status.direct]) * 0.4f, MoveTime* 2.0f);
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
                status.point = this.status.movedPoint;

                // MoveTime秒経つまで次の入力を受け付けないようにする
                StartCoroutine(MoveTimer());

                // MoveTime秒かけて目的地へ
                this.transform.DOMove(MapData.GridToWorld(this.status.movedPoint, InitPosY), MoveTime).SetEase(Ease.Linear);

                // 歩行状態へ
                playerAnimator.Play("Running@loop");
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

    public bool Skip()
    {
        // ここでマップに登録してある敵を更新
        SequenceMGR.instance.MapDataUpdate_Enemy();

        // 進む先にオブジェクトがあれば進まない
        MapData.MapObjType onObject = MapData.instance.GetMapObject(this.status.movedPoint).objType;
        if (onObject != MapData.MapObjType.none &&
            onObject != MapData.MapObjType.player)
        {
            // 移動失敗時処理
            this.MoveFailure();
            return false;
        }

        // 進んだ先が壁でなかった場合
        if (MapData.instance.GetMapChipType(this.status.movedPoint.x, this.status.movedPoint.y)
             != MapData.MapChipType.wall)
        {
            status.point = this.status.movedPoint;

            // =--------- 移動後処理 ---------= //
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
            UI_MGR.instance.Ui_Map.UpdateMap();

            // 歩数加算
            ++cntSteps;

            if (param.hunger <= 0)
            {
                // 満腹度矯正
                param.hunger = 0;

                // hpを1ずつ減らす
                this.SubHP(1);
                // hpが0以下なら死亡
                if (this.CheckDestroy())
                {
                    Destroy(this.gameObject);
                }
            }
            else
            {
                if ((cntSteps % StemNum_SubHunger) == 0)
                {// 3歩ごとに満腹度を1下げる
                    int lastHunger = param.hunger;

                    --param.hunger;

                    if(param.hunger  <= 15)
                    {
                        if(lastHunger > 15)
                        {
                            MessageWindow.instance.AddMessage("おなかがへってきた…");
                        }
                    }
                    

                    this.ChangeExpression();
                }
            }

            return true;
        }
        else
        {
            // 移動失敗時処理
            this.MoveFailure();
            return false;
        }
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

    public bool IsHunger()
    {
        if(HungerBorder < (((float)this.param.hunger / (float)this.param.maxHunger) * 100.0f))
        {
            return false;
        }
        return true;
    }

    private float Per_Hunger()
    {
        return (((float)this.param.hunger / (float)this.param.maxHunger) * 100.0f);
    }

    public void MapDataUpdate()
    {
        MapData.instance.SetMapObject(this.status.point, MapData.MapObjType.player, param.id);
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

    /// <summary>
    /// 指定の時間が経ったら入力を受け付けられるようにする
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveTimer()
    {
        // MoveTime秒まつ
        yield return new WaitForSeconds(MoveTime);

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
        UI_MGR.instance.Ui_Map.UpdateMap();

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
            if ((cntSteps % StemNum_SubHunger) == 0)
            {// StemNum_SubHunger歩ごとに満腹度を1下げる
                int lastHunger = param.hunger;

                --param.hunger;

                if (param.hunger <= 15)
                {
                    if (lastHunger > 15)
                    {
                        MessageWindow.instance.AddMessage("おなかがへってきた…");
                    }
                }

                this.ChangeExpression();
            }
        }

        // ターンエンド 
        status.actType = ActType.TurnEnd;
        
        SequenceMGR.instance.ActProc();

        AdDebug.Log(MapData.instance.GetMapChipType(this.status.point).ToString());

        yield return new WaitForSeconds(0.05f);

        // 移動し終わった時点で歩行キー入力がない場合は歩行アニメーションをやめる
        if (!PS4Input.GetAxisX_Any() && !PS4Input.GetAxisY_Any())
        {
            playerAnimator.Play("Standing@loop");
        }
    }

    // MoveTime後に敵のターン
    private IEnumerator ActProcTimer()
    {
        // MoveTime秒まつ
        yield return new WaitForSeconds(MoveTime * 0.5f);

        // マップ上の敵更新
        SequenceMGR.instance.MapDataUpdate_Enemy();

        // 攻撃
        MapData.ObjectOnTheMap mapObj = MapData.instance.GetMapObject(status.point + MapData.DirectPoints[(int)this.status.direct]);
        if (mapObj.objType == MapData.MapObjType.enemy)
        {// 攻撃した先が敵
            if (Percent.Per(90))
            {
                // 敵パラメータを取得
                EnemyBase enemy = SequenceMGR.instance.SearchEnemyFromID(mapObj.id);

                // ダメージ量を計算してhpから減算
                int damage = this.CalcAtk();
                
                enemy.Damage(damage, true);

                EffectMGR.instance.CreateEffect(EffectMGR.EffectType.Player_Attack_Hit, enemy.status.point);
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

        yield return new WaitForSeconds(0.05f);

        // 待機アニメーションへ
        playerAnimator.Play("Standing@loop");

        // 攻撃アニメーションで座標がずれてしまうので矯正
        this.transform.position = MapData.GridToWorld(this.status.point, -0.5f);

        // 角度も矯正
        this.ChangeRotate();
    }

    public void ChangeExpression()
    {
        float per = this.Per_Hunger();

        if(per <= 100.0f)
        {
            UIPlayerFace.ChangeExpression( UI_PlayerImage.Face.laugh1, false, true);
            if(per <= 80.0f)
            {
                UIPlayerFace.ChangeExpression(UI_PlayerImage.Face.laugh2, false, true);
                if (per <= 60.0f)
                {
                    UIPlayerFace.ChangeExpression(UI_PlayerImage.Face.normal, false, true);
                    if (per <= 40.0f)
                    {
                        UIPlayerFace.ChangeExpression(UI_PlayerImage.Face.low, false, true);
                        if (per <= 20.0f)
                        {
                            UIPlayerFace.ChangeExpression(UI_PlayerImage.Face.down, false, true);
                            if (per <= 0.0f)
                            {
                                UIPlayerFace.ChangeExpression(UI_PlayerImage.Face.damage, false, true);
                            }
                        }
                    }
                }
            }
        }
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