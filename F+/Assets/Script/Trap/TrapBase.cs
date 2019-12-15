using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 罠の基底クラス
/// </summary>
public class TrapBase : MapChipBase
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 罠起動（罠ごとに効果が違う）
    /// </summary>
    public virtual void ActiveTrap() { }
}
