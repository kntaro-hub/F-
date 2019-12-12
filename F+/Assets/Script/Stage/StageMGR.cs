using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMGR : MonoBehaviour
{
    // 階層数
    private static int floorNum = 1;   // 一階からスタート

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddFloorNum(int num)
    {
        floorNum += num;
    }

    public int GetFloorNum()
    {
        return floorNum;
    }
    public void SetFloorNum(int num)
    {
        floorNum = num;
    }

    #region singleton

    static StageMGR _instance;

    public static StageMGR instance
    {
        get
        {
            if (_instance == null)
            {
                var previous = FindObjectOfType(typeof(StageMGR));
                if (previous)
                {
                    Debug.LogWarning("Initialized twice. Don't use StageMGR in the scene hierarchy.");
                    _instance = (StageMGR)previous;
                }
                else
                {
                    var go = new GameObject("StageMGR");
                    _instance = go.AddComponent<StageMGR>();
                    DontDestroyOnLoad(go);
                    go.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            return _instance;
        }
    }

    #endregion
}
