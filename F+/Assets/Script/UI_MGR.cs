using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MGR : MonoBehaviour
{
    private UI_BasicMenu ui_BasicMenu = null;
    private UI_Map ui_Map = null;

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


    // Start is called before the first frame update
    void Start()
    {
        ui_BasicMenu = FindObjectOfType<UI_BasicMenu>();
        ui_Map = FindObjectOfType<UI_Map>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
