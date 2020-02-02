using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MGR : MonoBehaviour
{
    public enum UIType
    {
        basicMenu = 0,
        goal,
        infomation,
        inventory,
        itemMenu,
        max
    }


    [SerializeField] private UI_BasicMenu    ui_BasicMenu;
    [SerializeField] private UI_Map          ui_Map;
    [SerializeField] private UI_Goal         ui_Goal;
    [SerializeField] private UI_Information  ui_Infomation;
    [SerializeField] private UI_Inventory    ui_Inventory;
    [SerializeField] private UI_ItemMenu     ui_ItemMenu;

    private List<UIType> uiList = new List<UIType>();
    private UI_Base[] ui_Array = new UI_Base[(int)UIType.max];

    public UI_BasicMenu Ui_BasicMenu
    {
        get
        {
            if(ui_BasicMenu == null)
            {
                ui_BasicMenu = FindObjectOfType<UI_BasicMenu>();
            }
            return ui_BasicMenu;
        }
    }
    public UI_Map Ui_Map
    {
        get
        {
            if (ui_Map == null)
            {
                ui_Map = FindObjectOfType<UI_Map>();
            }
            return ui_Map;
        }
    }
    public UI_Goal Ui_Goal
    {
        get
        {
            if (ui_Goal == null)
            {
                ui_Goal = FindObjectOfType<UI_Goal>();
            }
            return ui_Goal;
        }
    }
    public UI_Information Ui_Infomation
    {
        get
        {
            if (ui_Infomation == null)
            {
                ui_Infomation = FindObjectOfType<UI_Information>();
            }
            return ui_Infomation;
        }
    }
    public UI_Inventory Ui_Inventory
    {
        get
        {
            if (ui_Inventory == null)
            {
                ui_Inventory = FindObjectOfType<UI_Inventory>();
            }
            return ui_Inventory;
        }
    }
    public UI_ItemMenu Ui_ItemMenu
    {
        get
        {
            if (ui_ItemMenu == null)
            {
                ui_ItemMenu = FindObjectOfType<UI_ItemMenu>();
            }
            return ui_ItemMenu;
        }
    }

    public const float ShowMenuTime = 0.1f;

    public void ShowUI(UIType type)
    {
        {
            uiList.Insert(0, type);
            ui_Array[(int)uiList[0]].ShowMenu();
        }
    }

    public void ReturnUI()
    {
        if (uiList.Count > 0)
        {
            ui_Array[(int)uiList[0]].HideMenu();
            uiList.RemoveAt(0);
            if (uiList.Count <= 0)
            {
                SequenceMGR.instance.seqType = SequenceMGR.SeqType.keyInput;
            }
        }
        SoundMGR.PlaySe("UI_Return", 0.2f);
    }   

    public void ReturnAllUI()
    {
        if (uiList.Count != 0)
        {
            while (true)
            {
                ui_Array[(int)uiList[0]].HideMenu();
                uiList.RemoveAt(0);
                if(uiList.Count <= 0) break;
            }
            SequenceMGR.instance.seqType = SequenceMGR.SeqType.keyInput;
        }
        SoundMGR.PlaySe("UI_Return", 0.2f);
    }

    public void UpdatePosUI(UIType type, Vector2 pos)
    {
        ui_Array[(int)type].GetComponent<RectTransform>().position = new Vector3(pos.x, pos.y);
    }

    // Start is called before the first frame update
    void Start()
    {
        ui_Array[(int)UIType.basicMenu]     = ui_BasicMenu;
        ui_Array[(int)UIType.goal]          = ui_Goal;
        ui_Array[(int)UIType.infomation]    = ui_Infomation;
        ui_Array[(int)UIType.inventory]     = ui_Inventory;
        ui_Array[(int)UIType.itemMenu]      = ui_ItemMenu;
    }

    // Update is called once per frame
    void Update()
    {
        if (uiList.Count > 0)
        {
            SequenceMGR.instance.seqType = SequenceMGR.SeqType.moveImpossible;
            ui_Array[(int)uiList[0]].UpdateProc_UI();

            if ((PS4Input.GetButtonDown(PS4ButtonCode.Triangle)))
            {
                this.ReturnAllUI();
            }
            if (PS4Input.GetButtonDown(PS4ButtonCode.Cross))
            {// escキーでメニュー表示/非表示
                this.ReturnUI();
            }
        }
        else
        {
            if ((PS4Input.GetButtonDown(PS4ButtonCode.Triangle)))
            {
                if (SequenceMGR.instance.seqType == SequenceMGR.SeqType.keyInput)
                    this.ShowUI( UIType.basicMenu);
            }
        }
    }

    #region singleton

    static UI_MGR _instance;

    public static UI_MGR instance
    {
        get
        {
            if (_instance == null)
            {
                var previous = FindObjectOfType(typeof(UI_MGR));
                if (previous)
                {
                    _instance = (UI_MGR)previous;
                }
                else
                {
                    var go = new GameObject("UI_MGR");
                    _instance = go.AddComponent<UI_MGR>();
                    DontDestroyOnLoad(go);
                    go.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            return _instance;
        }
    }

    #endregion
}
