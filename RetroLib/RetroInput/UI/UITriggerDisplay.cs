using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITriggerDisplay : MonoBehaviour 
{
    [SerializeField]
    private RectTransform _BG;

    [SerializeField]
    private float _MinValue = 0.0f;

    [SerializeField]
    private float _MaxValue = 100.0f;

    public void SetTriggerValue(float fValue)
    {
        _BG.sizeDelta = new Vector2(_BG.sizeDelta.x,_MinValue + ((_MaxValue - _MinValue) * fValue));
    }
}
