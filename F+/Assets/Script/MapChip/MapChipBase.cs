using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// マップチップ基底クラス
/// </summary>
public class MapChipBase : MonoBehaviour
{
    // マップ上座標
    protected Point point;

    // マップチップID
    protected int ID;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void ActiveMapChip(Actor actor)
    {

    }
}
