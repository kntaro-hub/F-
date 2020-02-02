using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMGR : MonoBehaviour
{
    /// <summary>
    /// 操作タイプ
    /// </summary>
    public enum UpdateMode
    {
        logo = 0,
        moving,
        menu,
        max
    }
    // 操作タイプ変数
    private UpdateMode controllMode = UpdateMode.logo;
    public UpdateMode SetControllMode
    {
        set { controllMode = value; }
    }

    // 操作タイプのデリゲート型宣言
    delegate void ControllUpdate();

    // 操作タイプ数分生成
    private ControllUpdate[] controllUpdate = new ControllUpdate[(int)UpdateMode.max];

    [SerializeField] private FadeText[] fadeTexts;
    [SerializeField] private Title_Camera titleCamera;
    [SerializeField] private float moveTime = 1.0f;
    [SerializeField] private Title_MenuUI menuUI;

    // Start is called before the first frame update
    void Start()
    {
        this.controllUpdate[(int)UpdateMode.logo] = this.Update_Logo;
        this.controllUpdate[(int)UpdateMode.moving] = this.Update_Moving;
        this.controllUpdate[(int)UpdateMode.menu] = this.Update_Menu;

        foreach (var itr in fadeTexts)
        {
            itr.FadeStart();
        }

        // BGMを流す
        SoundMGR.PlayBgm("TitleBGM", 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        this.controllUpdate[(int)controllMode]();
    }

    private void Update_Logo()
    {
        if(PS4Input.GetButtonDown( PS4ButtonCode.Circle) ||
            PS4Input.GetButtonDown(PS4ButtonCode.Option) ||
            PS4Input.GetButtonDown(PS4ButtonCode.Cross) ||
            PS4Input.GetButtonDown(PS4ButtonCode.TouchPad) ||
            PS4Input.GetButtonDown(PS4ButtonCode.PS))
        {
            SoundMGR.PlaySe("Decision");

            foreach(var itr in fadeTexts)
            {
                itr.FadeEnd();
            }

            titleCamera.MoveStart(moveTime);

            StartCoroutine(this.MoveTimer());

            this.controllMode = UpdateMode.moving;
        }
    }

    private void Update_Moving()
    {
        // 処理なし
    }

    private void Update_Menu()
    {
        menuUI.UpdateProc_UI();
    }

    private IEnumerator MoveTimer()
    {
        yield return new WaitForSeconds(moveTime);

        controllMode = UpdateMode.menu;

        this.menuUI.ShowMenu();
    }
}
