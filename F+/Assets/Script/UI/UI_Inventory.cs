using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EquipType
{
    weapon = 0,
    shield,
    arrow,
    max
}

public class UI_Inventory : UI_Base
{
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

    [SerializeField] private PageArrow leftArrow;
    [SerializeField] private PageArrow rightArrow;
    [SerializeField] private TextMeshProUGUI pageNumText;

    // =--------- 変数宣言 ---------= //

    private int buttonNum = 0;        // 選択しているボタン番号
    private int lastButtonNum = 0;   // 直前に選択していたボタン番号
    private Image panel = null;     // パネル
    private Image cursor = null;     // 選択カーソル
    private Image[] equipIcon = new Image[(int)EquipType.max]; // 装備中アイコン
    private int[] equipInventoryID = new int[(int)EquipType.max]; // 装備中アイテムのインベントリID 
    private UI_Map ui_Map = null;     // マップ表示用
    private bool isShow = false;    // メニューを表示しているか

    private int crntPageNum = 0;
    private int maxPageNum = 0;

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
        equipIcon[(int)EquipType.arrow] = Instantiate(equipIconPrefab, this.transform);
        equipIcon[(int)EquipType.weapon].color = Color.clear;
        equipIcon[(int)EquipType.shield].color = Color.clear;
        equipIcon[(int)EquipType.arrow].color = Color.clear;

        // 初期化
        this.Init();

        pageNumText.color = Color.clear;
    }

    // Update is called once per frame
    void Update()
    {
        this.PositionSet();

        this.UpdateMaxPage();
    }

    private void UpdateMaxPage()
    {
        maxPageNum = (items.StockCount() - 1) / ShowItemNum;
    }

    public override void UpdateProc_UI()
    {
        lastButtonNum = buttonNum;

        if (isShow)
        {// メニュー表示中のみ
            if (items.StockCount() > 0)
            {
                if (PS4Input.GetCrossKeyU())
                {// ↑キーでカーソルを上に
                    buttonNum--;
                    this.CheckFlow_U();
                    this.CursorSet(buttonNum);
                }
                if (PS4Input.GetCrossKeyD())
                {// ↓キーでカーソルを下に
                    buttonNum++;
                    this.CheckFlow_D();
                    this.CursorSet(buttonNum);
                }
                else if (PS4Input.GetCrossKeyL())
                {// ←キーでカーソルを左に
                    buttonNum -= (ShowItemNum / 2);
                    this.CheckFlow_L();
                    this.CursorSet(buttonNum);
                }
                else if (PS4Input.GetCrossKeyR())
                {// →キーでカーソルを右に
                    buttonNum += (ShowItemNum / 2);
                    this.CheckFlow_R();
                    this.CursorSet(buttonNum);
                }

                if (PS4Input.GetButtonDown(PS4ButtonCode.Circle))
                {// 〇ボタンで決定
                    this.SwitchCommand();

                    // 決定音再生
                    SoundMGR.PlaySe("Decision");
                }

                if (PS4Input.GetButtonDown(PS4ButtonCode.R1))
                {// R1ボタンで次ページへ
                    this.NextPage();
                    this.CheckFlow_R();
                    this.CursorSet(buttonNum);
                }
                if (PS4Input.GetButtonDown(PS4ButtonCode.L1))
                {// L1ボタンで前ページへ
                    this.PrevPage();
                    this.CheckFlow_L();
                    this.CursorSet(buttonNum);
                }
            }
        }
    }

    /// <summary>
    /// 初期処理
    /// </summary>
    private void Init()
    {
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
        for (int i = 0; i < (int)EquipType.max; ++i)
        {
            if (equipInventoryID[i] != Actor.Parameter.notEquipValue)
            {
                --equipInventoryID[i];
                if (equipInventoryID[i] < 0)
                {
                    equipInventoryID[i] = 0;
                }
            }
        }
        this.ResetCursor();
    }

    private void PositionSet()
    {
        // 座標矯正
        int cnt = 0;
        for (int i = 0; i < textList.Count; ++i)
        {
            cnt = i % ShowItemNum;

            if (cnt < (ShowItemNum / 2))
            {// 4つまでは左
                textList[i].rectTransform.localPosition = new Vector3(initializePositionX,
                    initializePositionY + cnt * offsetText);
            }
            else
            {// 4つ以降右
                textList[i].rectTransform.localPosition = new Vector3(initializePositionX + this.panel.rectTransform.sizeDelta.x * 0.5f,
                    initializePositionY + (cnt - (ShowItemNum / 2)) * offsetText);
            }
        }

        for(int i = 0; i < (textList.Count % ShowItemNum); ++i)
        {
            if (isShow)
            {
                textList[i + (crntPageNum * ShowItemNum)].color = Color.white;
            }
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
        switch(type)
        {
            case ItemType.Weapon: return equipInventoryID[(int)EquipType.weapon];
            case ItemType.Shield: return equipInventoryID[(int)EquipType.shield];
            case ItemType.Arrow: return equipInventoryID[(int)EquipType.arrow];
        }
        return 0;
        
    }
    // =--------- // =--------- メニュー表示/非表示 ---------= // ---------= //

    /// <summary>
    /// メニューを開く
    /// </summary>
    public override void ShowMenu()
    {
        crntPageNum = 0;
        if (items.StockCount() > 0)
        {
            for (int i = 0; i < items.StockCount(); i++)
            {// アイテム数だけテキストを表示
                if (textList.Count > i)
                {// テキストがすでに生成されていたらテキストを更新するだけ
                    textList[i].text = DataBase.instance.GetItemTableEntity(items.GetItemID(i)).Name;
                }
                else
                {// 生成されていない場合は新しく生成
                    TextMeshProUGUI itemText = Instantiate(textPrefab, this.transform);
                    itemText.text = DataBase.instance.GetItemTableEntity(items.GetItemID(i)).Name;
                    textList.Add(itemText);
                }

                if (i >= ShowItemNum) textList[i].color = Color.clear;
                else textList[i].color = Color.white;
            }
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
            if (items.StockCount() > 0)
            {
                cursor.color = Color.white;
                buttonNum = 0;
                this.CursorSet(buttonNum);
            }
        }
        else
        {
            cursor.color = Color.clear;
        }

        panel.color = new Color(0.0f, 0.0f, 0.0f, 0.3f);
        isShow = true;

        // 装備アイコン更新
        this.UpdateEquipIcon();

        // 矢印
        if (textList.Count > ShowItemNum)
        {
            leftArrow.Active();
            rightArrow.Active();
        }

        // ページ数表示
        pageNumText.color = Color.white;
        pageNumText.text = $"{crntPageNum + 1} / {maxPageNum + 1}";
    }

    /// <summary>
    /// 装備アイコン更新
    /// </summary>
    private void UpdateEquipIcon()
    {
        for (int i = 0; i < (int)EquipType.max; ++i)
        {
            {
                if (equipInventoryID[i] < textList.Count)
                {
                    if (equipInventoryID[i] != Actor.Parameter.notEquipValue)
                    {
                        if (textList[equipInventoryID[i]].color != Color.clear)
                        {
                            UI_MGR.instance.Ui_Inventory.SetEquipIcon(equipInventoryID[i], (EquipType)i);
                        }
                        else
                        {
                            equipIcon[i].color = Color.clear;
                        }
                    }
                }
            }
        }
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
                    textList[i].text = DataBase.instance.GetItemTableEntity(items.GetItemID(i)).Name;
                }
                else
                {
                    TextMeshProUGUI itemText = Instantiate(textPrefab, this.transform);
                    itemText.text = DataBase.instance.GetItemTableEntity(items.GetItemID(i)).Name;
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

        leftArrow.Clear();
        rightArrow.Clear();

        pageNumText.color = Color.clear;

        // アイテム説明欄も
        ItemText.Hide();
        isShow = false;
    }
    
    /// <summary>
    /// テキストの横にカーソルの座標を合わせる
    /// </summary>
    /// <param name="i"></param>
    private void CursorSet(int i)
    {
        cursor.rectTransform.localPosition =
            new Vector3(
                textList[i + crntPageNum * ShowItemNum].rectTransform.localPosition.x - textList[i].rectTransform.sizeDelta.x * 0.55f,
                textList[i + crntPageNum * ShowItemNum].rectTransform.localPosition.y);

        // アイテムの説明欄を更新
        ItemText.SetText(DataBase.instance.GetItemTableEntity(items.GetItemID(i + crntPageNum * ShowItemNum)).Detail);
    }

    public void ResetCursor()
    {
        if (textList.Count > 0)
        {
            buttonNum = 0;
            this.CursorSet(buttonNum);
        }
    }

    /// <summary>
    /// オーバーフローorアンダーフロー対策
    /// </summary>
    private void CheckFlow_U()
    {
        if(maxPageNum == crntPageNum)
        {
            if (buttonNum < 0)
            {
                buttonNum = ((textList.Count) % ShowItemNum) - 1;
                if(buttonNum == -1)
                {// アイテムがShowItemNumの倍数だった場合
                    buttonNum = ShowItemNum - 1;
                }
            }
        }
        else
        {
            if (buttonNum < 0)
            {
                buttonNum = ShowItemNum - 1;
            }
        }

        if(lastButtonNum != buttonNum)
        {
            SoundMGR.PlaySe("Choice", 0.2f);
        }
    }

    /// <summary>
    /// オーバーフローorアンダーフロー対策
    /// </summary>
    private void CheckFlow_D()
    {
        if (maxPageNum == crntPageNum)
        { 
            if (-1 != ((textList.Count) % ShowItemNum) - 1)
            {
                if (buttonNum > ((textList.Count) % ShowItemNum) - 1)
                {
                    buttonNum = 0;
                }
            }
            else
            {
                if (buttonNum > ShowItemNum - 1)
                {
                    buttonNum = 0;
                }
            }
        }
        else
        {
            if (buttonNum > ShowItemNum - 1)
            {
                buttonNum = 0;
            }
        }

        if (lastButtonNum != buttonNum)
        {
            SoundMGR.PlaySe("Choice", 0.2f);
        }
    }

    private void CheckFlow_L()
    {        
        if(crntPageNum == maxPageNum)
        {// 現ページが最大ページなら

            if (maxPageNum > 0)
            {// 最大ページが1以上あれば

                if (buttonNum < 0)
                {// 選択番号が範囲外になった場合

                    // 前ページの右側にカーソルを移動
                    buttonNum += ShowItemNum;

                    // 前ページに移動
                    this.PrevPage();
                }
            }
            else
            {// 最大ページが0ページだった場合
                if (buttonNum < 0)
                {// 選択番号が範囲外になった場合

                    // カーソル位置は変わらない
                    buttonNum += ShowItemNum / 2;
                }
            }
        }
        else
        {// 現ページが最大ページでない場合

            if(crntPageNum > 0)
            {// 1ページでもすすんでいたら

                if(buttonNum < 0)
                {// 選択番号が範囲外になった場合

                    // 前ページの右側にカーソルを移動
                    buttonNum += ShowItemNum;

                    // 前ページに移動
                    this.PrevPage();
                }
            }
            else
            {// 現ページが0ページ目だった場合

                if (buttonNum < 0)
                {// 選択番号が範囲外になった場合

                    // カーソル位置は変わらない
                    buttonNum += ShowItemNum / 2;
                }
            }
        }
        if (lastButtonNum != buttonNum)
        {
            SoundMGR.PlaySe("Choice", 0.2f);
        }
    }

    private void CheckFlow_R()
    {
        if (crntPageNum == maxPageNum)
        {// 現ページが最大ページなら

            if (-1 != (textList.Count % ShowItemNum) - 1)
            {
                if (buttonNum > (textList.Count % ShowItemNum) - 1)
                {// 最大アイテム数を超えた場合

                    // 最大アイテム数を選択番号にする
                    buttonNum = ((textList.Count) % ShowItemNum) - 1;
                }
            }
            else
            {
                if (buttonNum > ShowItemNum - 1)
                {// 最大アイテム数を超えた場合

                    // 最大アイテム数を選択番号にする
                    buttonNum = ShowItemNum - 1;
                }
            }
        }
        else
        {// 現ページが最大ページでない場合

            if(buttonNum >= ShowItemNum)
            {// 1ページ表示限界を超えた場合

                // 次のページの左側にカーソルを移動
                buttonNum -= ShowItemNum;

                this.NextPage();
                if (crntPageNum == maxPageNum)
                {// ページ変更後が最大ページだった場合

                    if (-1 != (textList.Count % ShowItemNum) - 1)
                    {
                        // アイテム最大数を超えていたら
                        if (buttonNum > (textList.Count % ShowItemNum) - 1)
                        {
                            // 矯正
                            buttonNum = (textList.Count % ShowItemNum) - 1;
                        }
                    }
                }
            }
        }
        if (lastButtonNum != buttonNum)
        {
            SoundMGR.PlaySe("Choice", 0.2f);
        }
    }

    /// <summary>
    /// 現在の選んでいるボタンごとの処理を実行する
    /// </summary>
    private void SwitchCommand()
    {
        // インベントリ番号からアイテムIDとプレイヤーのアイテムストックIDを取得
        itemMenu.SetItemID(items.GetItemID(buttonNum + crntPageNum * ShowItemNum), (buttonNum + crntPageNum * ShowItemNum));
        UI_MGR.instance.ShowUI(UI_MGR.UIType.itemMenu);
        UI_MGR.instance.UpdatePosUI(UI_MGR.UIType.itemMenu, this.textList[buttonNum + crntPageNum * ShowItemNum].rectTransform.position);
    }

    /// <summary>
    /// インベントリのページを1進める
    /// </summary>
    private bool NextPage()
    {
        // 現ページ数保存
        int lastPageNum = crntPageNum;

        // ページ数加算
        ++crntPageNum;

        // 最大アイテム数を基に最大ページ数矯正
        this.UpdateMaxPage();

        if(maxPageNum < crntPageNum)
        {
            --crntPageNum;
        }

        // ページ数が変わっていたら
        if(lastPageNum != crntPageNum)
        {
            // 現テキストを透明に
            for(int i = lastPageNum * ShowItemNum; i < crntPageNum * ShowItemNum; ++i)
            {
                textList[i].color = Color.clear;
            }

            for (int j = crntPageNum * ShowItemNum; j < (crntPageNum + 1) * ShowItemNum; ++j)
            {
                if (j < textList.Count)
                {
                    textList[j].color = Color.white;
                }
            }

            // 装備アイコン更新
            this.UpdateEquipIcon();

            rightArrow.Light();
            pageNumText.text = $"{crntPageNum + 1} / {maxPageNum + 1}";

            this.UpdateMaxPage();

            // ページ数が変わったかを返す
            return true;
        }

        return false;
    }

    private bool PrevPage()
    {
        // 現ページ数保存
        int lastPageNum = crntPageNum;

        // ページ数減算
        --crntPageNum;

        if (0 > crntPageNum)
        {
            crntPageNum = 0;
        }

        // ページ数が変わっていたら
        if (lastPageNum != crntPageNum)
        {
            // 現テキストを透明に
            foreach(var itr in textList)
            {
                itr.color = Color.clear;
            }

            int d = (crntPageNum * ShowItemNum + ShowItemNum);
            for (int j = crntPageNum * ShowItemNum; j < d; ++j)
            {
                textList[j].color = Color.white;
            }

            // 装備アイコン更新
            this.UpdateEquipIcon();

            leftArrow.Light();
            pageNumText.text = $"{crntPageNum + 1} / {maxPageNum + 1}";

            this.UpdateMaxPage();

            // ページが変わったかを返す
            return true;
        }

        return false;
    }
}
