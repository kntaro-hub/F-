using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChip_Goal : MapChipBase
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ActiveMapChip(Actor actor)
    {
        UI_MGR.instance.ShowUI(UI_MGR.UIType.goal);
    }
}
