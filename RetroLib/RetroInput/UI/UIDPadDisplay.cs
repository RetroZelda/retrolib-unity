using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDPadDisplay : MonoBehaviour 
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

    public void SetAnalogValue(Vector2 v2Val)
    {
        if (v2Val.x > 0.0f)
        {
            _Right.color = _DownColor;
        }
        else
        {
            _Right.color = _UpColor;
        }

        if (v2Val.x < 0.0f)
        {
            _Left.color = _DownColor;
        }
        else
        {
            _Left.color = _UpColor;
        }

        if (v2Val.y > 0.0f)
        {
            _Down.color = _DownColor;
        }
        else
        {
            _Down.color = _UpColor;
        }

        if (v2Val.y < 0.0f)
        {
            _Up.color = _DownColor;
        }
        else
        {
            _Up.color = _UpColor;
        }
    }
}
