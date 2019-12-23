using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class UI_ItemMenu : UI_Base
{
    // =--------- 列挙体定義 ---------= //
    enum TextType   // 表示するテキストの種類
    {
        use,    // 使う   1
        throwItem,   // アイテムを投げる   2
        put,    // 置く   3
        max     // 最大値
    }

    // =--------- プレハブ ---------= //

    [SerializeField]
    private TextMeshProUGUI textPrefab;     // テキストプレハブ
    [SerializeField]
    private Image cursorPrefab;   // カーソルプレハブ
    [SerializeField]
    private ThrowObject throwObjPrefab;

    // =--------- パラメータ ---------= //

    [SerializeField]
    private float offsetText = 0.5f;            // テキスト同士のY座標間隔
    [SerializeField]
    private float initializePositionX = 0.0f;   // テキスト基準座標X
    [SerializeField]
    private float initializePositionY = 0.0f;   // テキスト基準座標Y

    // =--------- 変数宣言 ---------= //

    private int buttonNum = 0;        // 選択しているボタン番号
    private Image panel = null;     // パネル
    private Image cursor = null;     // 選択カーソル
    private UI_Map ui_Map = null;     // マップ表示用
    private bool isShow = false;    // メニューを表示しているか

    private int selectedItemID = 0;
    private int inventoryID = 0;

    private List<TextMeshProUGUI> textList = new List<TextMeshProUGUI>();   // 生成したテキストリスト
    private Player_Items items; // プレイヤーが所持しているアイテム

    // =--------- プロパティ ---------= //
    public bool IsShowMenu
    {
        get { return isShow; }
        set { isShow = value; }
    }

    // =--------- 定数定義 ---------= //
    private float ThrowSpeed = 0.04f;

    // =--------- // =--------- ---------= // ---------= //

    // Start is called before the first frame update
    void Start()
    {
        cursor = Instantiate(cursorPrefab, this.transform);
        TextMeshProUGUI use     = Instantiate(textPrefab, this.transform); use.text     = "使う";
        TextMeshProUGUI info    = Instantiate(textPrefab, this.transform); info.text    = "投げる";
        TextMeshProUGUI put     = Instantiate(textPrefab, this.transform); put.text     = "置く";

        textList.Add(use);
        textList.Add(info);
        textList.Add(put);

        panel = this.GetComponent<Image>();
        items = SequenceMGR.instance.Player.GetComponent<Player_Items>();
        panel.color = Color.clear;

        // 初期化
        this.Init();

        // 0番にカーソル位置合わせ
        this.CursorSet(0);
    }

    // Update is called once per frame
    void Update()
    {
        // 座標矯正
        int cnt = 0;
        foreach (var itr in textList)
        {
            itr.rectTransform.localPosition = new Vector3(initializePositionX,
                initializePositionY - itr.rectTransform.sizeDelta.y * cnt + offsetText * cnt);
            ++cnt;
        }
    }

    public override void UpdateProc_UI()
    {
        if (isShow)
        {// メニュー表示中のみ
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {// ↑キーでカーソルを上に
                buttonNum--;
                this.CheckFlow();
                this.CursorSet(buttonNum);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {// ↓キーでカーソルを下に
                buttonNum++;
                this.CheckFlow();
                this.CursorSet(buttonNum);
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {// エンターキーで決定
                this.SwitchCommand();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {// escキーでメニュー表示/非表示
                UI_MGR.instance.ReturnUI();
            }
        }
        int cnt = 0;
        foreach (var itr in textList)
        {
            itr.rectTransform.localPosition = new Vector3(initializePositionX,
                initializePositionY - itr.rectTransform.sizeDelta.y * cnt + offsetText);
            ++cnt;
        }
    }

    /// <summary>
    /// 初期処理
    /// </summary>
    private void Init()
    {
        int cnt = 0;
        foreach (var itr in textList)
        {
            // 透明に
            itr.color = Color.clear;
            itr.rectTransform.localPosition = new Vector3(initializePositionX,
                initializePositionY - itr.rectTransform.sizeDelta.y * cnt + offsetText);
            ++cnt;
        }
        // カーソルも透明に
        cursor.color = Color.clear;
        // パネルも
        panel.color = Color.clear;
    }

    // =--------- // =--------- メニュー表示/非表示 ---------= // ---------= //

    public void SetItemID(int selectItemID, int invID)
    {
        selectedItemID = selectItemID;    // 選んだアイテムのIDを登録
        inventoryID = invID;
    }

    /// <summary>
    /// メニューを開く
    /// </summary>
    public override void ShowMenu()
    {
        foreach (var itr in textList)
        {
            itr.color = Color.white;
        }
        cursor.color = Color.white;
        panel.color = new Color(0.0f, 0.0f, 0.0f, 0.3f);
        isShow = true;

        ItemType type = DataBase.instance.GetItemTable(selectedItemID).Type;
        switch (type)
        {
            case ItemType.Consumables:
                textList[(int)TextType.use].text = "使う"; break;
            case ItemType.Weapon:
            case ItemType.Shield:

                if(UI_MGR.instance.Ui_Inventory.GetEquipInventoryID(type) != inventoryID)
                {
                    textList[(int)TextType.use].text = "装備";
                }
                else
                {
                    textList[(int)TextType.use].text = "外す";
                }

                break;
        }

        // 0番にカーソル位置を合わせる
        buttonNum = 0;
        this.CursorSet(buttonNum);
    }

    /// <summary>
    /// メニューを閉じる
    /// </summary>
    public override void HideMenu()
    {
        foreach (var itr in textList)
        {
            itr.color = Color.clear;
        }
        cursor.color = Color.clear;
        panel.color = Color.clear;
        isShow = false;
    }

    // =--------- // =--------- コマンド ---------= // ---------= //
    /// <summary>
    /// [使う]コマンド
    /// </summary>
    private void Com_Use()
    {// 選択しているアイテムを使う
        this.SwitchUseItemType(DataBase.instance.GetItemTable(selectedItemID).Type);

        if (DataBase.instance.GetItemTable(selectedItemID).UseMessage != "")
        {
            MessageWindow.instance.AddMessage(DataBase.instance.GetItemTable(selectedItemID).UseMessage, Color.white);
        }

        // ウィンドウを閉じる
        UI_MGR.instance.ReturnUI();
    }
    /// <summary>
    /// [投げる]コマンド
    /// </summary>
    private void Com_Throw()
    {// 選択しているアイテムを前方に投げる
        Point playerPoint = SequenceMGR.instance.Player.status.point;
        Point point = new Point();
        switch (SequenceMGR.instance.Player.status.direct)
        {
            case Actor.Direct.right:        point.x =  1; break;
            case Actor.Direct.left:         point.x = -1; break;
            case Actor.Direct.forward:      point.y =  1; break;
            case Actor.Direct.back:         point.y = -1; break;

            case Actor.Direct.right_forward: point.x =  1; point.y =  1; break;
            case Actor.Direct.left_forward:  point.x = -1; point.y =  1; break;
            case Actor.Direct.right_back:    point.x =  1; point.y = -1; break;
            case Actor.Direct.left_back:     point.x = -1; point.y = -1; break;                 
        }

        while(true)
        {
            // 当たったあとの座標
            Point hitedPoint = playerPoint + point;

            bool isHit = false;

            if (MapData.instance.GetMapChipType(hitedPoint) == MapData.MapChipType.wall)        isHit = true;
            if (MapData.instance.GetMapObject(hitedPoint).objType == MapData.MapObjType.enemy)  isHit = true;
               
            if(!isHit)
            {// 1マス先へ
                if (point.x != 0) point.x += (int)Mathf.Sign((float)point.x);
                if (point.y != 0) point.y += (int)Mathf.Sign((float)point.y);
            }
           else
            {
                // 壁or敵にぶつかったら1マス下がる
                if (point.x != 0) point.x -= (int)Mathf.Sign((float)point.x);
                if (point.y != 0) point.y -= (int)Mathf.Sign((float)point.y);

                // あたる直前の座標
                Point hitPoint = playerPoint + point;

                switch (DataBase.instance.GetItemTable(selectedItemID).Type)
                {
                    case ItemType.Consumables:
                        break;

                    case ItemType.Weapon:
                        // 武器を装備していた場合
                        if (inventoryID == UI_MGR.instance.Ui_Inventory.GetEquipInventoryID(UI_Inventory.EquipType.weapon))
                        {// 武器を外す
                            UI_MGR.instance.Ui_Inventory.RemoveEquipIcon(UI_Inventory.EquipType.weapon);
                            SequenceMGR.instance.Player.Param = ItemMGR.instance.RemoveEquip(SequenceMGR.instance.Player.Param, UI_Inventory.EquipType.weapon);
                        }
                        break;

                    case ItemType.Shield:
                        // 盾を装備していた場合
                        if (inventoryID == UI_MGR.instance.Ui_Inventory.GetEquipInventoryID(UI_Inventory.EquipType.shield))
                        {// 盾を外す
                            UI_MGR.instance.Ui_Inventory.RemoveEquipIcon(UI_Inventory.EquipType.shield);
                            SequenceMGR.instance.Player.Param = ItemMGR.instance.RemoveEquip(SequenceMGR.instance.Player.Param, UI_Inventory.EquipType.shield);
                        }
                        break;
                }

                // メッセージ表示
                MessageWindow.instance.AddMessage($"{DataBase.instance.GetItemTable(selectedItemID).Name}を投げた！", Color.white);

                // 投げる用オブジェクト
                ThrowObject throwItem = Instantiate(throwObjPrefab, MapData.GridToWorld(playerPoint), Quaternion.identity);

                float throwTime = ThrowSpeed * (int)Mathf.Max(Mathf.Abs((float)(playerPoint.x - (playerPoint.x + point.x))), Mathf.Abs((float)(playerPoint.y - (playerPoint.y + point.y))));

                throwItem.Move(hitedPoint, throwTime);

                MapData.ObjectOnTheMap mapObject = MapData.instance.GetMapObject(hitedPoint);
                if (mapObject.objType == MapData.MapObjType.enemy)
                {// 敵に当たった場合
                    StartCoroutine(this.ItemThrowTimer(throwTime, throwItem, selectedItemID, hitPoint, false));

                    // 敵にダメージを与える
                    EnemyBase enemy = SequenceMGR.instance.SearchEnemyFromID(mapObject.id);

                    int damage = 0;
                    ItemTableEntity item = DataBase.instance.GetItemTable(selectedItemID);
                    switch(item.Type)
                    {
                        case ItemType.Consumables:  damage = 1;
                            break;

                        case ItemType.Weapon:       damage = Mathf.Abs(item.Atk - enemy.Param.atk) * 2;
                            break;

                        case ItemType.Shield:       damage = item.Def / 3;
                            break;
                    }
                    enemy.Param.SubHP(damage);
                    MessageWindow.instance.AddMessage($"{enemy.Param.Name}に{damage}のダメージ！", Color.white);
                }
                else
                {// 敵以外に当たった場合
                    StartCoroutine(this.ItemThrowTimer(throwTime, throwItem, selectedItemID, hitPoint, true));
                }

                // インベントリから使ったアイテムを削除
                items.Erase(items.GetStockID(inventoryID));
                UI_MGR.instance.Ui_Inventory.EraseText(inventoryID);

                // ウィンドウを閉じる
                UI_MGR.instance.ReturnAllUI();

                SequenceMGR.instance.seqType = SequenceMGR.SeqType.moveImpossible;
                break;
            }
        }
    }
    /// <summary>
    /// [置く]コマンド
    /// </summary>
    private void Com_Put()
    {// 選択しているアイテムをその場の足元に置く

        switch (DataBase.instance.GetItemTable(selectedItemID).Type)
        {
            case ItemType.Consumables:
                break;

            case ItemType.Weapon:
                // 武器を装備していた場合
                if (inventoryID == UI_MGR.instance.Ui_Inventory.GetEquipInventoryID(UI_Inventory.EquipType.weapon))
                {// 武器を外す
                    UI_MGR.instance.Ui_Inventory.RemoveEquipIcon(UI_Inventory.EquipType.weapon);
                    SequenceMGR.instance.Player.Param = ItemMGR.instance.RemoveEquip(SequenceMGR.instance.Player.Param, UI_Inventory.EquipType.weapon);
                }
                break;

            case ItemType.Shield:
                // 盾を装備していた場合
                if (inventoryID == UI_MGR.instance.Ui_Inventory.GetEquipInventoryID(UI_Inventory.EquipType.shield))
                {// 盾を外す
                    UI_MGR.instance.Ui_Inventory.RemoveEquipIcon(UI_Inventory.EquipType.shield);
                    SequenceMGR.instance.Player.Param = ItemMGR.instance.RemoveEquip(SequenceMGR.instance.Player.Param, UI_Inventory.EquipType.shield);
                }
                break;
        }
        
        // アイテム設置
        ItemMGR.instance.CreateItem(SequenceMGR.instance.Player.status.point, selectedItemID);

        // インベントリから使ったアイテムを削除
        items.Erase(items.GetStockID(inventoryID));
        UI_MGR.instance.Ui_Inventory.EraseText(inventoryID);

        // メッセージ表示
        MessageWindow.instance.AddMessage($"{DataBase.instance.GetItemTable(selectedItemID).Name}を置いた。", Color.white);

        // ウィンドウを閉じる
        UI_MGR.instance.ReturnUI();
    }

    // =--------- // =--------- ---------= // ---------= //

    private void SwitchUseItemType(ItemType type)
    {
        switch(type)
        {
            case ItemType.Consumables:
                // 一定時間操作不能に
                SequenceMGR.instance.seqType = SequenceMGR.SeqType.moveImpossible;
                StartCoroutine(this.ItemUseTimer());

                // アイテムの効果を使う
                SequenceMGR.instance.Player.Param = ItemMGR.instance.UseItem(selectedItemID, SequenceMGR.instance.Player.Param);

                // インベントリから使ったアイテムを削除
                items.Erase(items.GetStockID(inventoryID));
                UI_MGR.instance.Ui_Inventory.EraseText(inventoryID);
                break;

            case ItemType.Weapon:
                // 武器を装備
                if (inventoryID != UI_MGR.instance.Ui_Inventory.GetEquipInventoryID( UI_Inventory.EquipType.weapon))
                {
                    SequenceMGR.instance.Player.Param = ItemMGR.instance.EquipWeapon(selectedItemID, SequenceMGR.instance.Player.Param);
                    MessageWindow.instance.AddMessage(DataBase.instance.GetItemTable(selectedItemID).Name + "を装備した。", Color.white);
                    UI_MGR.instance.Ui_Inventory.SetEquipIcon(inventoryID, UI_Inventory.EquipType.weapon);
                }
                else
                {// 武器を外す
                    MessageWindow.instance.AddMessage(DataBase.instance.GetItemTable(selectedItemID).Name + "を外した。", Color.white);
                    UI_MGR.instance.Ui_Inventory.RemoveEquipIcon(UI_Inventory.EquipType.weapon);
                    SequenceMGR.instance.Player.Param = ItemMGR.instance.RemoveEquip(SequenceMGR.instance.Player.Param, UI_Inventory.EquipType.weapon);

                }
                break;

            case ItemType.Shield:
                // 盾を装備
                if (inventoryID != UI_MGR.instance.Ui_Inventory.GetEquipInventoryID(UI_Inventory.EquipType.shield))
                {
                    SequenceMGR.instance.Player.Param = ItemMGR.instance.EquipShield(selectedItemID, SequenceMGR.instance.Player.Param);
                    MessageWindow.instance.AddMessage(DataBase.instance.GetItemTable(selectedItemID).Name + "を装備した。", Color.white);
                    UI_MGR.instance.Ui_Inventory.SetEquipIcon(inventoryID, UI_Inventory.EquipType.shield);
                }
                else
                {// 盾を外す
                    MessageWindow.instance.AddMessage(DataBase.instance.GetItemTable(selectedItemID).Name + "を外した。", Color.white);
                    UI_MGR.instance.Ui_Inventory.RemoveEquipIcon(UI_Inventory.EquipType.shield);
                    SequenceMGR.instance.Player.Param = ItemMGR.instance.RemoveEquip(SequenceMGR.instance.Player.Param, UI_Inventory.EquipType.shield);
                }
                break;
        }
    }

    /// <summary>
    /// テキストの横にカーソルの座標を合わせる
    /// </summary>
    /// <param name="i"></param>
    private void CursorSet(int i)
    {
        cursor.rectTransform.localPosition =
            new Vector3(
                textList[i].rectTransform.localPosition.x - 50.0f,
                textList[i].rectTransform.localPosition.y);
    }

    /// <summary>
    /// オーバーフローorアンダーフロー対策
    /// </summary>
    private void CheckFlow()
    {
        if (buttonNum >= (int)TextType.max)
        {
            buttonNum = 0;
        }
        else if (buttonNum < 0)
        {
            buttonNum = (int)TextType.max - 1;
        }
    }

    /// <summary>
    /// 現在の選んでいるボタンごとの処理を実行する
    /// </summary>
    private void SwitchCommand()
    {
        switch (buttonNum)
        {
            case (int)TextType.use:     this.Com_Use(); break;
            case (int)TextType.throwItem:    this.Com_Throw(); break;
            case (int)TextType.put:     this.Com_Put(); break;
            default: break;
        }
    }

    // =--------- // =--------- コルーチン ---------= // ---------= //
    private IEnumerator ItemUseTimer()
    {
        yield return new WaitForSeconds(1.0f);
        SequenceMGR.instance.seqType = SequenceMGR.SeqType.keyInput;
        MessageWindow.instance.AddMessage(DataBase.instance.GetItemTable(selectedItemID).UsedMessage, Color.white);
    }

    private IEnumerator ItemThrowTimer(
        float time,
        ThrowObject throwObject,
        int ID,
        Point point,
        bool isCreateItem)
    {
        yield return new WaitForSeconds(time);

        Destroy(throwObject.gameObject);

        if(isCreateItem)
            ItemMGR.instance.CreateItem(point, ID);

        SequenceMGR.instance.seqType = SequenceMGR.SeqType.keyInput;
    }
}
