using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FloorMGR : MonoBehaviour
{
    // 階層数
    private static int floorNum = 1;   // 一階からスタート
    public int FloorNum
    {
        get { return floorNum; }
        set { floorNum = value; }
    }

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
    public void NextFloor()
    {
        floorNum++;
        SequenceMGR.instance.Player.SaveStatus();
        SceneManager.LoadScene("Interval");
    }

    #region singleton

    static FloorMGR _instance;

    public static FloorMGR instance
    {
        get
        {
            if (_instance == null)
            {
                var previous = FindObjectOfType(typeof(FloorMGR));
                if (previous)
                {
                    Debug.LogWarning("Initialized twice. Don't use StageMGR in the scene hierarchy.");
                    _instance = (FloorMGR)previous;
                }
                else
                {
                    var go = new GameObject("StageMGR");
                    _instance = go.AddComponent<FloorMGR>();
                    DontDestroyOnLoad(go);
                    go.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            return _instance;
        }
    }

    #endregion
}
