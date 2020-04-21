using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PS4のボタンコード
public enum PS4ButtonCode
{
    Square = 0,
    Cross,
    Circle,
    Triangle,
    L1,
    R1,
    L2,
    R2,
    Share,
    Option,
    L3,
    R3,
    PS,
    TouchPad
}

public enum PS4KeyCodeLR
{
    CrossKey_L = -1,
    CrossKey_R = 1
}
public enum PS4KeyCodeUD
{
    CrossKey_U = 1,
    CrossKey_D = -1
}

public enum KeyDirect
{
    L = 0,
    R,
    U,
    D,
    Max
}

public class PS4Input : MonoBehaviour
{
    static bool[] lastInputCrossKey = new bool[(int)KeyDirect.Max];

    static bool[] isInputCrossKey = new bool[(int)KeyDirect.Max];

    private void Update()
    {
        for (int i = 0; i < (int)KeyDirect.Max; ++i)
        {
            isInputCrossKey[i] = false;
        }

        if (GetCrossKey(PS4KeyCodeLR.CrossKey_L) == true)
        {
            if (!lastInputCrossKey[(int)KeyDirect.L])
                isInputCrossKey[(int)KeyDirect.L] = true;

            lastInputCrossKey[(int)KeyDirect.L] = true;
        }
        else lastInputCrossKey[(int)KeyDirect.L] = false;

        if (GetCrossKey(PS4KeyCodeLR.CrossKey_R) == true)
        {
            if (!lastInputCrossKey[(int)KeyDirect.R])
                isInputCrossKey[(int)KeyDirect.R] = true;

            lastInputCrossKey[(int)KeyDirect.R] = true;
        }
        else lastInputCrossKey[(int)KeyDirect.R] = false;

        if (GetCrossKey(PS4KeyCodeUD.CrossKey_U) == true)
        {
            if (!lastInputCrossKey[(int)KeyDirect.U])
                isInputCrossKey[(int)KeyDirect.U] = true;

            lastInputCrossKey[(int)KeyDirect.U] = true;
        }
        else lastInputCrossKey[(int)KeyDirect.U] = false;

        if (GetCrossKey(PS4KeyCodeUD.CrossKey_D) == true)
        {
            if (!lastInputCrossKey[(int)KeyDirect.D])
                isInputCrossKey[(int)KeyDirect.D] = true;

            lastInputCrossKey[(int)KeyDirect.D] = true;
        }
        else lastInputCrossKey[(int)KeyDirect.D] = false;

    }

    public static bool GetButton(PS4ButtonCode code)
    {
        return Input.GetButton($"{code.ToString()}");
    }

    public static bool GetButtonDown(PS4ButtonCode code)
    {
        return Input.GetButtonDown($"{code.ToString()}");
    }

    public static bool GetButtonUp(PS4ButtonCode code)
    {
        return Input.GetButtonUp($"{code.ToString()}");
    }

    public static bool GetCrossKey(PS4KeyCodeLR code, bool isKeyDown)
    {
        if (!isKeyDown) return Input.GetAxis($"CrossKey_LR") == (int)code;
        else if (code == PS4KeyCodeLR.CrossKey_L) return isInputCrossKey[(int)KeyDirect.L];
        else if (code == PS4KeyCodeLR.CrossKey_R) return isInputCrossKey[(int)KeyDirect.R];
        else return false;
    }

    public static bool GetCrossKey(PS4KeyCodeUD code, bool isKeyDown)
    {
        if (!isKeyDown) return Input.GetAxis($"CrossKey_UD") == (int)code;
        else if (code == PS4KeyCodeUD.CrossKey_U) return isInputCrossKey[(int)KeyDirect.U];
        else if (code == PS4KeyCodeUD.CrossKey_D) return isInputCrossKey[(int)KeyDirect.D];
        else return false;
    }

    public static bool GetCrossKey(PS4KeyCodeLR code)
    {
        return Input.GetAxis($"CrossKey_LR") == (int)code;
    }

    public static bool GetCrossKey(PS4KeyCodeUD code)
    {
        return Input.GetAxis($"CrossKey_UD") == (int)code;
    }

    public static bool GetCrossKeyL() { return isInputCrossKey[(int)KeyDirect.L]; }
    public static bool GetCrossKeyR() { return isInputCrossKey[(int)KeyDirect.R]; }
    public static bool GetCrossKeyU() { return isInputCrossKey[(int)KeyDirect.U]; }
    public static bool GetCrossKeyD() { return isInputCrossKey[(int)KeyDirect.D]; }

    public static bool GetAxisX_Any()
    {
        return Input.GetAxis($"CrossKey_LR") != 0;
    }
    public static bool GetAxisY_Any()
    {
        return Input.GetAxis($"CrossKey_UD") != 0;
    }

    #region singleton

    static PS4Input _instance;

    public static PS4Input instance
    {
        get
        {
            if (_instance == null)
            {
                var previous = FindObjectOfType(typeof(PS4Input));
                if (previous)
                {
                    _instance = (PS4Input)previous;
                }
                else
                {
                    var go = new GameObject("PS4Input");
                    _instance = go.AddComponent<PS4Input>();
                    DontDestroyOnLoad(go);
                    go.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            return _instance;
        }
    }

    #endregion
}
