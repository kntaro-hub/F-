using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FieldMGR : MonoBehaviour
{
    // =--------- 定数定義 ---------= //
    public const int fieldMax = 10;

    // =--------- 構造体定義 ---------= //
    private struct SaveData
    {
        public FieldInformation.GridInfo[,] infos;
    }

    // =--------- 変数宣言 ---------= //
    // グリッド座標→ワールド座標変換時のズレ
    private float initPosY = 0.0f; 

    // マップデータ保存用構造体
    private SaveData saveData = new SaveData();

    // 生成したマップオブジェクトリスト
    private List<GameObject> mapObjects = new List<GameObject>();

    // マップデータ
    private FieldInformation.GridInfo[,] gridInfos = new FieldInformation.GridInfo[fieldMax, fieldMax];
    public FieldInformation.GridInfo[,] GridInfos
    {
        get { return gridInfos; }
    }

    // =--------- プレハブ ---------= //
    // 壁
    [SerializeField] private GameObject wallPrefab;
    

    // Start is called before the first frame update
    void Start()
    {
        this.CreateStage();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            this.SaveField();
        }
    }

    private void SaveField()
    {
        StreamWriter writer;

        writer = new StreamWriter(Application.dataPath + "/MapData.json", false);

        saveData.infos = new FieldInformation.GridInfo[fieldMax, fieldMax];
        for (int i = 0; i < fieldMax; ++i)
        {
            for (int j = 0; j < fieldMax; ++j)
            {
                saveData.infos[i,j] = gridInfos[i, j];
                string jsonstr = JsonUtility.ToJson(saveData.infos[i, j]);
                jsonstr = jsonstr + "\n";
                writer.Write(jsonstr);
                writer.Flush();
            }
        }

        
        writer.Close();
    }

    private void LoadField()
    {
        string datastr = "";
        StreamReader reader;
        reader = new StreamReader(Application.dataPath + "/MapData.json");
        datastr = reader.ReadToEnd();
        reader.Close();

        for (int i = 0; i < fieldMax; ++i)
        {
            for (int j = 0; j < fieldMax; ++j)
            {
                gridInfos[i, j] = JsonUtility.FromJson<FieldInformation.GridInfo>(datastr);
            }
        }
        foreach(var itr in mapObjects)
        {
            Destroy(itr);
        }
        mapObjects.Clear();
    }

    /// <summary>
    /// 配列からステージを生成する
    /// </summary>
    private void CreateStage()
    {
        for(int i = 0; i < fieldMax; ++i)
        {
            for (int j = 0; j < fieldMax; ++j)
            {
                gridInfos[i, j].GridNum = new Vector2Int(i, j);
                gridInfos[i, j].Type = FieldInformation.FieldType.none;
                gridInfos[i, j].OnFieldObj = null;
            }
        }

        gridInfos[1, 3].Type = FieldInformation.FieldType.wall;
        mapObjects.Add(Instantiate(wallPrefab, this.GridToWorld(new Point(1, 3)), Quaternion.identity));
    }

    public void SetInitY(float posY)
    {
        initPosY = posY;
    }

    public Vector3 GridToWorld(Point grid)
    {
        Vector3 world = new Vector3(
            (float)(grid.x * FieldInformation.GridSize),
            initPosY,
            (float)(grid.y * FieldInformation.GridSize));

        return world;
    }

    public Point WorldToGrid(Vector3 world)
    {
        Point grid = new Point(
            (int)(world.x / FieldInformation.GridSize),
            (int)(world.z / FieldInformation.GridSize));

        return grid;
    }

    #region singleton

    static FieldMGR _instance;

    public static FieldMGR instance
    {
        get
        {
            if (_instance == null)
            {
                var previous = FindObjectOfType(typeof(FieldMGR));
                if (previous)
                {
                    Debug.LogWarning("Initialized twice. Don't use MidiBridge in the scene hierarchy.");
                    _instance = (FieldMGR)previous;
                }
                else
                {
                    var go = new GameObject("MessageWindow");
                    _instance = go.AddComponent<FieldMGR>();
                    DontDestroyOnLoad(go);
                    go.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            return _instance;
        }
    }

    #endregion
}
