using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : UI_Base
{
    public enum EquipType
    {
        weapon = 0,
        shield,
        max
    }

    // =--------- プレハブ ---------= //

    [SerializeField]
    private TextMeshProUGUI textPrefab;     // テキストプレハブ
    [SerializeField]
    private Image cursorPrefab;   // カーソルプレハブ
    [SerializeField]
    private UI_ItemInfo ItemText;

    // =--------- パラメータ ---------= //

    [SerializeField]
    private float offsetText = 0.5f;            // テキスト同士のY座標間隔
    [SerializeField]
    private float initializePositionX = 0.0f;   // テキスト基準座標X
    [SerializeField]
    private float initializePositionY = 0.0f;   // テキスト基準座標Y
    [SerializeField]
    private int ShowItemNum = 8;
    [SerializeField]
    private UI_ItemMenu itemMenu;
    [SerializeField]
    private Image equipIconPrefab;

    // =--------- 変数宣言 ---------= //

    private int buttonNum = 0;        // 選択しているボタン番号
    private Image panel = null;     // パネル
    private Image cursor = null;     // 選択カーソル
    private Image[] equipIcon = new Image[(int)EquipType.max]; // 装備中アイコン
    private int[] equipInventoryID = new int[(int)EquipType.max]; // 装備中アイテムのインベントリID 
    private UI_Map ui_Map = null;     // マップ表示用
    private bool isShow = false;    // メニューを表示しているか

    private int pageNum = 0;

    private List<TextMeshProUGUI> textList = new List<TextMeshProUGUI>();   // 生成したテキストリスト

    private Player_Items items;
    
    // =--------- プロパティ ---------= //
    public bool IsShowMenu
    {
        get { return isShow; }
        set { isShow = value; }
    }

    public int[] EquipInventoryID
    {
        get { return equipInventoryID; }
        set { equipInventoryID = value; }
    }

    // =--------- // =--------- ---------= // ---------= //

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        //equipIcon = new Image[(int)EquipType.max];
        //equipInventoryID = new int[(int)EquipType.max];

        cursor = Instantiate(cursorPrefab, this.transform);

        items = FindObjectOfType<Player_Items>();
        panel = this.GetComponent<Image>();
        panel.color = Color.clear;

        equipIcon[(int)EquipType.weapon] = Instantiate(equipIconPrefab, this.transform);
        equipIcon[(int)EquipType.shield] = Instantiate(equipIconPrefab, this.transform);
        equipIcon[(int)EquipType.weapon].color = Color.clear;
        equipIcon[(int)EquipType.shield].color = Color.clear;

        // 初期化
        this.Init();
    }

    // Update is called once per frame
    void Update()
    {
        this.PositionSet();
    }

    public override void UpdateProc_UI()
    {
        if (isShow)
        {// メニュー表示中のみ
            if (items.StockCount() > 0)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {// ↑キーでカーソルを上に
                    buttonNum--;
                    this.CheckFlow();
                    this.CursorSet(buttonNum);
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {// ↓キーでカーソルを下に
                    buttonNum++;
                    this.CheckFlow();
                    this.CursorSet(buttonNum);
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {// ←キーでカーソルを左に
                    buttonNum -= (ShowItemNum / 2);
                    this.CheckFlow();
                    this.CursorSet(buttonNum);
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {// ↓キーでカーソルを下に
                    buttonNum += (ShowItemNum / 2);
                    this.CheckFlow();
                    this.CursorSet(buttonNum);
                }

                if (Input.GetKeyDown(KeyCode.Return))
                {// エンターキーで決定
                    this.SwitchCommand();
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {// escキーでメニュー表示/非表示
                UI_MGR.instance.ReturnUI();
            }
        }
    }

    /// <summary>
    /// 初期処理
    /// </summary>
    private void Init()
    {
        // 座標矯正
        int cnt = 0;
        foreach (var itr in textList)
        {
            if (cnt < (ShowItemNum / 2))
            {// 4つまでは左
                // 透明に
                itr.color = Color.clear;
                itr.rectTransform.localPosition = new Vector3(initializePositionX,
                    initializePositionY + cnt * offsetText);
            }
            else
            {// 4つ以降右
                // 透明に
                itr.color = Color.clear;
                itr.rectTransform.localPosition = new Vector3(initializePositionX + this.panel.rectTransform.sizeDelta.x * 0.5f,
                    initializePositionY + (cnt - (ShowItemNum / 2)) * offsetText);
            }

            ++cnt;
        }

        // カーソルも透明に
        cursor.color = Color.clear;
        // パネルも
        panel.color = Color.clear;
        // アイテム説明欄も
        ItemText.Hide();        
    }

    public void EraseText(int inventoryID)
    {
        Destroy(textList[inventoryID]);
        textList.RemoveAt(inventoryID);
    }

    private void PositionSet()
    {
        // 座標矯正
        int cnt = 0;
        foreach (var itr in textList)
        {
            if (cnt < (ShowItemNum / 2))
            {// 4つまでは左
                itr.rectTransform.localPosition = new Vector3(initializePositionX,
                    initializePositionY + cnt * offsetText);
            }
            else
            {// 4つ以降右
                itr.rectTransform.localPosition = new Vector3(initializePositionX + this.panel.rectTransform.sizeDelta.x * 0.5f,
                    initializePositionY + (cnt - (ShowItemNum / 2)) * offsetText);
            }

            ++cnt;
        }
    }
    public void SetEquipIcon(int InventoryID, EquipType type)
    {
        if (InventoryID != Actor.Parameter.notEquipValue)
        {
            equipIcon[(int)type].transform.parent = textList[InventoryID].transform;
            equipIcon[(int)type].transform.localPosition = new Vector3();
            equipIcon[(int)type].rectTransform.localPosition = new Vector3(-textList[InventoryID].rectTransform.sizeDelta.x * 0.5f - 10.0f, 10.0f);
            equipIcon[(int)type].color = Color.white;

            equipInventoryID[(int)type] = InventoryID;
        }
    }
    public void RemoveEquipIcon(EquipType type)
    {
        equipIcon[(int)type].color = Color.clear;
        equipInventoryID[(int)type] = Actor.Parameter.notEquipValue;
    }

    public int GetEquipInventoryID(EquipType type)
    {
        return equipInventoryID[(int)type];
    }
    public int GetEquipInventoryID(ItemType type)
    {
        if(type == ItemType.Weapon)
        {
            return equipInventoryID[(int)EquipType.weapon];
        }
        else if (type == ItemType.Shield)
        {
            return equipInventoryID[(int)EquipType.shield];
        }
        return 0;
        
    }
    // =--------- // =--------- メニュー表示/非表示 ---------= // ---------= //

    /// <summary>
    /// メニューを開く
    /// </summary>
    public override void ShowMenu()
    {
        if (items.StockCount() > 0)
        {
            for (int i = 0; i < items.StockCount(); i++)
            {// アイテム数だけテキストを表示
                if (textList.Count > i)
                {// テキストがすでに生成されていたらテキストを更新するだけ
                    textList[i].text = DataBase.instance.GetItemTable(items.GetItemID(i)).Name;
                }
                else
                {
                    TextMeshProUGUI itemText = Instantiate(textPrefab, this.transform);
                    itemText.text = DataBase.instance.GetItemTable(items.GetItemID(i)).Name;
                    textList.Add(itemText);
                }
            }
            foreach (var itr in textList)
            {
                itr.color = Color.white;
            }
            cursor.color = Color.white;

            for (int i = 0; i < (int)EquipType.max; ++i)
            {
                if(equipInventoryID[i] != Actor.Parameter.notEquipValue)
                {
                    equipIcon[i].color = Color.white;
                }
            }

            // アイテム説明欄を表示
            ItemText.Show();

            this.PositionSet();

            // 0番にカーソル位置を合わせる
            buttonNum = 0;
            this.CursorSet(buttonNum);
        }
        else
        {
            cursor.color = Color.clear;
        }

        panel.color = new Color(0.0f, 0.0f, 0.0f, 0.3f);
        isShow = true;

        UI_MGR.instance.Ui_Inventory.SetEquipIcon(equipInventoryID[(int)EquipType.weapon], EquipType.weapon);
        UI_MGR.instance.Ui_Inventory.SetEquipIcon(equipInventoryID[(int)EquipType.shield], EquipType.shield);
    }

    /// <summary>
    /// メニューを開く
    /// </summary>
    public void UpdateText()
    {
        if (items.StockCount() > 0)
        {
            for (int i = 0; i < items.StockCount(); i++)
            {// アイテム数だけテキストを表示
                if (textList.Count > i)
                {// テキストがすでに生成されていたらテキストを更新するだけ
                    textList[i].text = DataBase.instance.GetItemTable(items.GetItemID(i)).Name;
                }
                else
                {
                    TextMeshProUGUI itemText = Instantiate(textPrefab, this.transform);
                    itemText.text = DataBase.instance.GetItemTable(items.GetItemID(i)).Name;
                    textList.Add(itemText);
                }
            }
        }
        else
        {
            cursor.color = Color.clear;
        }
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

        foreach (var itr in equipIcon)
        {
            itr.color = Color.clear;
        }

        // アイテム説明欄も
        ItemText.Hide();
        isShow = false;
    }

    // =--------- // =--------- コマンド ---------= // ---------= //
    /// <summary>
    /// [先へ進む]コマンド
    /// </summary>
    private void Com_Next()
    {// 次のステージへ

    }
    /// <summary>
    /// [キャンセル]コマンド
    /// </summary>
    private void Com_Cancel()
    {// マップUIを消す
        this.HideMenu();
    }

    // =--------- // =--------- ---------= // ---------= //
    
    /// <summary>
    /// テキストの横にカーソルの座標を合わせる
    /// </summary>
    /// <param name="i"></param>
    private void CursorSet(int i)
    {
        cursor.rectTransform.localPosition =
            new Vector3(
                textList[i].rectTransform.localPosition.x - textList[i].rectTransform.sizeDelta.x * 0.55f,
                textList[i].rectTransform.localPosition.y);

        // アイテムの説明欄を更新
        ItemText.SetText(DataBase.instance.GetItemTable(items.GetItemID(i)).Detail);
    }

    /// <summary>
    /// オーバーフローorアンダーフロー対策
    /// </summary>
    private void CheckFlow()
    {
        if (buttonNum >= (int)textList.Count)
        {
            buttonNum = 0;
        }
        else if (buttonNum < 0)
        {
            buttonNum = (int)textList.Count - 1;
        }
    }

    /// <summary>
    /// 現在の選んでいるボタンごとの処理を実行する
    /// </summary>
    private void SwitchCommand()
    {
        // インベントリ番号からアイテムIDとプレイヤーのアイテムストックIDを取得
        itemMenu.SetItemID(items.GetItemID(buttonNum + pageNum * ShowItemNum), (buttonNum + pageNum * ShowItemNum));
        UI_MGR.instance.ShowUI(UI_MGR.UIType.itemMenu);
        UI_MGR.instance.UpdatePosUI(UI_MGR.UIType.itemMenu, this.textList[buttonNum + pageNum * ShowItemNum].rectTransform.position);
    }
}
