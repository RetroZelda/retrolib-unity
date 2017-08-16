using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControlButtonDisplay : MonoBehaviour {

    [SerializeField]
    private Image _Start;

    [SerializeField]
    private Image _Select;

    [SerializeField]
    private Image _Home;

    [SerializeField]
    private Color _UpColor;

    [SerializeField]
    private Color _DownColor;

    public void SetStartValue(bool bDown)
    {
        if (bDown)
        {
            _Start.color = _DownColor;
        }
        else
        {
            _Start.color = _UpColor;
        }
    }


    public void SetSelectValue(bool bDown)
    {
        if (bDown)
        {
            _Select.color = _DownColor;
        }
        else
        {
            _Select.color = _UpColor;
        }
    }


    public void SetHomeValue(bool bDown)
    {
        if (bDown)
        {
            _Home.color = _DownColor;
        }
        else
        {
            _Home.color = _UpColor;
        }
    }

}
