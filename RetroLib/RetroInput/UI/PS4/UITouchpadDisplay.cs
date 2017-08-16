using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITouchpadDisplay : MonoBehaviour 
{
    [SerializeField]
    private RectTransform _PositionNode;

    [SerializeField]
    private Image _PositionImage;

    [SerializeField]
    private Vector2 _PositionMin = new Vector2(0.0f, 0.0f);

    [SerializeField]
    private Vector2 _PositionMax = new Vector2(100.0f, 75.0f);

    [SerializeField]
    private Color _UpColor;

    [SerializeField]
    private Color _DownColor;

    private Vector2 _Position;

    public void SetPositionValueX(float fVal)
    {
        _Position.x = _PositionMin.x + ((_PositionMax.x - _PositionMin.x) * fVal);
        _PositionNode.anchoredPosition = _Position;
    }

    public void SetPositionValueY(float fVal)
    {
        _Position.y = _PositionMin.y + ((_PositionMax.y - _PositionMin.y) * fVal);
        _PositionNode.anchoredPosition = _Position;
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
