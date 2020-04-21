using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HpGauge : MonoBehaviour
{
    // プレイヤー
    private PlayerControll player = null;

    // HPゲージスライダー
    private Slider hp = null;

    // Start is called before the first frame update
    void Start()
    {
        player = SequenceMGR.instance.Player;
        hp = this.GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        // 体力ゲージ更新
        hp.value = ((float)player.Param.hp / player.Param.maxHp);
    }
}
