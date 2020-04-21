using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_Frame_Back : MonoBehaviour
{
    private Image image = null;

    // プレイヤー
    [SerializeField, Tooltip("プレイヤーをセット")] private PlayerControll player = null;

    // Start is called before the first frame update
    void Start()
    {
        image = this.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        image.color = Color.Lerp(Color.red, new Color(0.347f, 1.0f, 0.5490196f), (float)player.Param.hunger / player.Param.maxHunger);
    }

    public void PunchColor()
    {
        image.color = Color.white;
    }
}
