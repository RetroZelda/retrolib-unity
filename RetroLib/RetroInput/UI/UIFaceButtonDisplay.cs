using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFaceButtonDisplay : MonoBehaviour 
{
    [SerializeField]
    private Image _Left;

    [SerializeField]
    private Image _Right;

    [SerializeField]
    private Image _Up;

    [SerializeField]
    private Image _Down;

    [SerializeField]
    private Color _UpColor;

    [SerializeField]
    private Color _DownColor;

    public void SetTopValue(bool bDown)
    {
        if (bDown)
        {
            _Up.color = _DownColor;
        }
        else
        {
            _Up.color = _UpColor;
        }
    }


    public void SetBottomValue(bool bDown)
    {
        if (bDown)
        {
            _Down.color = _DownColor;
        }
        else
        {
            _Down.color = _UpColor;
        }
    }


    public void SetLeftValue(bool bDown)
    {
        if (bDown)
        {
            _Left.color = _DownColor;
        }
        else
        {
            _Left.color = _UpColor;
        }
    }


    public void SetRightValue(bool bDown)
    {
        if (bDown)
        {
            _Right.color = _DownColor;
        }
        else
        {
            _Right.color = _UpColor;
        }
    }

}
