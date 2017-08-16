using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShoulderDisplay : MonoBehaviour 
{
    [SerializeField]
    private Image _Shoulder;

    [SerializeField]
    private Color _UpColor;

    [SerializeField]
    private Color _DownColor;

    public void SetShoulderValue(bool bDown)
    {
        if (bDown)
        {
            _Shoulder.color = _DownColor;
        }
        else
        {
            _Shoulder.color = _UpColor;
        }
    }
}
