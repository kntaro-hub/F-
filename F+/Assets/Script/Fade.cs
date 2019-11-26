using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;     //UIを使用可能にする
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour
{
    // フェードの速さ
    [SerializeField]
    private float FadeSpeed;

    private RawImage rawImage;

    // 色情報
    private Color color;

    // フェードフラグ
    private bool isFade;

    // Start is called before the first frame update
    void Start()
    {
        // 初期色情報取得
        rawImage = GetComponent<RawImage>();
        color.r = color.g = color.b = 0.0f;
        color.a = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(isFade)
        {
            color.a += FadeSpeed;

            if(color.a >= 1.0f)
            {
                color.a = 1.0f;

                switch(SceneManager.GetActiveScene().name)
                {
                    case "Game":
                        SceneManager.LoadScene("Game");
                        break;
                }
            }
        }
        else
        {
            color.a -= FadeSpeed;

            if (color.a < 0.0f)
            {
                color.a = 0.0f;
            }
        }

        rawImage.color = color;

        if(Input.GetKeyDown(KeyCode.X))
        {
            FadeStart();
        }
    }

    public void FadeStart()
    {
        if (color.a == 0.0f)
        {
            isFade = true;
        }
    }
}
