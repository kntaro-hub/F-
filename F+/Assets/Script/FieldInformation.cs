using UnityEngine;

/// <summary>
/// フィールドの情報を格納する
/// </summary>
public class FieldInformation : MonoBehaviour
{
    public enum FieldType
    {
        none = 0,   // 何もない
        wall,       // 壁
        max
    }

    // マス一個分の情報
    public struct GridInfo
    {
        public GridBase     onFieldObj;
        public Vector2Int   gridNum;
        public FieldType    type;

        public GridBase OnFieldObj
        {
            get { return onFieldObj; }
            set { onFieldObj = value; }
        }

        public Vector2Int GridNum
        {
            get { return gridNum; }
            set { gridNum = value; }
        }

        public FieldType Type
        {
            get { return type; }
            set { type = value; }
        }
    }

    // グリッドサイズ
    public const float GridSize = 1.0f;
 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
