using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Retro.Input;

public class UIControllerDisplay : MonoBehaviour 
{
    [SerializeField]
    private BaseJoypadInput _InputToDisplay;
    public BaseJoypadInput InputToDisplay { get { return _InputToDisplay; } set {_InputToDisplay = value; }}

    [SerializeField]
    private UIAnalogDisplay _LeftAnalog;

    [SerializeField]
    private UIAnalogDisplay _RightAnalog;

    [SerializeField]
    private UITriggerDisplay _LeftTrigger;

    [SerializeField]
    private UITriggerDisplay _RightTrigger;

    [SerializeField]
    private UIDPadDisplay _Dpad;

    [SerializeField]
    private UIFaceButtonDisplay _FaceButtons;

    [SerializeField]
    private UIShoulderDisplay _LeftShoulder;

    [SerializeField]
    private UIShoulderDisplay _RightShoulder;

    [SerializeField]
    private UIControlButtonDisplay _ControlButtons;

	// Use this for initialization
	protected virtual void Start () 
    {
        InputToDisplay.OnRightButton       += OnRightButtonHandler;
        InputToDisplay.OnBottomButton      += OnBottomButtonHandler;
        InputToDisplay.OnLeftButton        += OnLeftButtonHandler;
        InputToDisplay.OnTopButton         += OnTopButtonHandler;
        InputToDisplay.OnLeftBumper        += OnLeftBumperHandler;
        InputToDisplay.OnRightBumper       += OnRightBumperHandler;
        InputToDisplay.OnStart             += OnStartHandler;
        InputToDisplay.OnSelect            += OnSelectHandler;
        InputToDisplay.OnHome              += OnHomeHandler;
        InputToDisplay.OnLeftAnalogButton  += OnLeftAnalogButtonHandler;
        InputToDisplay.OnRightAnalogButton += OnRightAnalogButtonHandler;

        InputToDisplay.OnLeftAnalog        += OnLeftAnalogHandler;
        InputToDisplay.OnRightAnalog       += OnRightAnalogHandler;
        InputToDisplay.OnDPad              += OnDPadHandler;
        InputToDisplay.OnLeftTrigger       += OnLeftTriggerHandler;
        InputToDisplay.OnRightTrigger      += OnRightTriggerHandler;
	}
        
    private void OnRightButtonHandler(bool bHitState)
    {
        _FaceButtons.SetRightValue(bHitState);
    }

    private void OnBottomButtonHandler(bool bHitState)
    {
        _FaceButtons.SetBottomValue(bHitState);
    }

    private void OnLeftButtonHandler(bool bHitState)
    {
        _FaceButtons.SetLeftValue(bHitState);
    }

    private void OnTopButtonHandler(bool bHitState)
    {
        _FaceButtons.SetTopValue(bHitState);
    }

    private void OnLeftBumperHandler(bool bHitState)
    {
        _LeftShoulder.SetShoulderValue(bHitState);
    }

    private void OnRightBumperHandler(bool bHitState)
    {
        _RightShoulder.SetShoulderValue(bHitState);
    }

    private void OnStartHandler(bool bHitState)
    {
        _ControlButtons.SetStartValue(bHitState);
    }

    private void OnSelectHandler(bool bHitState)
    {
        _ControlButtons.SetSelectValue(bHitState);
    }

    private void OnHomeHandler(bool bHitState)
    {
        _ControlButtons.SetHomeValue(bHitState);
    }

    private void OnLeftAnalogButtonHandler(bool bHitState)
    {
        _LeftAnalog.SetButtonValue(bHitState);
    }

    private void OnRightAnalogButtonHandler(bool bHitState)
    {
        _RightAnalog.SetButtonValue(bHitState);
    }

    private void OnLeftAnalogHandler(Vector2 values)
    {
        _LeftAnalog.SetAnalogValue(values);
    }

    private void OnRightAnalogHandler(Vector2 values)
    {
        _RightAnalog.SetAnalogValue(values);
    }

    private void OnDPadHandler(Vector2 values)
    {
        _Dpad.SetAnalogValue(values);
    }

    private void OnLeftTriggerHandler(Vector2 values)
    {
        _LeftTrigger.SetTriggerValue(values.x);
    }

    private void OnRightTriggerHandler(Vector2 values)
    {
        _RightTrigger.SetTriggerValue(values.x);
    }

}
