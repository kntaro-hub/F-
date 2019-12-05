using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MGR : MonoBehaviour
{
    private UI_BasicMenu ui_BasicMenu = null;
    private UI_Map ui_Map = null;
    private UI_Goal ui_Goal = null;

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


    // Start is called before the first frame update
    void Start()
    {
        ui_BasicMenu = FindObjectOfType<UI_BasicMenu>();
        ui_Map = FindObjectOfType<UI_Map>();
        ui_Goal = FindObjectOfType<UI_Goal>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            ui_Map.ShowMapUI();
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
                    Debug.LogWarning("Initialized twice. Don't use UI_MGR in the scene hierarchy.");
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
