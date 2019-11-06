using System.Collections;
using System.Collections.Generic;
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
        private GridBase     onFieldObj;
        private Vector2Int   gridNum;
        private FieldType    type;

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
