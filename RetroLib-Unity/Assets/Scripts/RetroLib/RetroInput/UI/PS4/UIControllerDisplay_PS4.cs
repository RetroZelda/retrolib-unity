using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControllerDisplay_PS4 : UIControllerDisplay
{
    [SerializeField]
    private UITouchpadDisplay _Touchpad;

    protected override void Start ()
    {
        base.Start();

        InputToDisplay.OnCustomKey += OnCustomKeyHandler;
        InputToDisplay.OnCustomAxis += OnCustomAxisHandler;
    }

    private void OnCustomKeyHandler(string szKey, bool bHitState)
    {
        // Debug.Log(szKey + " - " + bHitState);
        if (szKey == "TouchPadButton")
        {
            _Touchpad.SetButtonValue(bHitState);
        }
    }

    private void OnCustomAxisHandler(string szKey, Vector2 values)
    {
        // Debug.Log(szKey + " - " + values);
        if (szKey == "TouchPadX")
        {
            _Touchpad.SetPositionValueX(values.x);
        }
        if (szKey == "TouchPadY")
        {
            _Touchpad.SetPositionValueY(values.x);
        }
    }

}
