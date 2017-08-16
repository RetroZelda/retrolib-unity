using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAnalogDisplay : MonoBehaviour 
{
    [SerializeField]
    private RectTransform _PositionNode;

    [SerializeField]
    private Image _PositionImage;

    [SerializeField]
    private float _PositionScale = 1.0f;

    [SerializeField]
    private Color _UpColor;

    [SerializeField]
    private Color _DownColor;

    private Vector2 _Position;
	
    public void SetAnalogValue(Vector2 v2Val)
    {
        _Position = v2Val;
        _Position.y *= -1.0f;
        _PositionNode.anchoredPosition = _Position * _PositionScale;
    }

    public void SetButtonValue(bool bDown)
    {
        if (bDown)
        {
            _PositionImage.color = _DownColor;
        }
        else
        {
            _PositionImage.color = _UpColor;
        }
    }
}
