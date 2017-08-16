using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Retro.Input.Events;
using Retro.Events;

using UInput = UnityEngine.Input;

namespace Retro.Input
{

    // TODO: Spread these classes/interfaces out between files... maybe.... yolo
    public enum MouseButton { LEFT = 0, RIGHT = 1, MIDDLE = 2 };
public enum BroadcastKeyState { PRESSED, RELEASED, HELD };

public interface IInputBinding
{
	void PollBinding();
}

public abstract class CInputBinding<T,I,B> : IInputBinding
{
	public abstract void PollBinding();
	public T BoundInputID { protected get; set; }

    protected Dictionary<I, B> _BoundInputValues = new Dictionary<I, B>();
    public Dictionary<I, B> BoundInputValues 
    { 
        get { return _BoundInputValues; }
    }

    public CInputBinding(T inputID)
    {
        BoundInputID = inputID;
    }

}

public abstract class CVarBroadcastBinding<T,I,B> : CInputBinding<T,I,B>
{
    private delegate B KeyPollAction(I inputBind);

    private BroadcastKeyState _KeyStateToBroadcast;
    private KeyPollAction _PollAction;

    // required children
    protected abstract B PollPressed(I inputBind);
    protected abstract B PollReleased(I inputBind);
    protected abstract B PollHeld(I inputBind);

    /// <summary>
    /// Gets or sets what key event we should be sending.
    /// </summary>
    /// <value>The key state to broadcast.</value>
    public BroadcastKeyState KeyStateToBroadcast 
    {
        get { return _KeyStateToBroadcast; } 
        set
        {
            _KeyStateToBroadcast = value;
            switch(_KeyStateToBroadcast)
            {
                case BroadcastKeyState.PRESSED:
                    _PollAction = PollPressed;
                    break;
                case BroadcastKeyState.RELEASED:
                    _PollAction = PollReleased;
                    break;
                case BroadcastKeyState.HELD:
                    _PollAction = PollHeld;
                    break;
            }
        } 
    }

    public CVarBroadcastBinding(T inputID, BroadcastKeyState keystate)
        : base(inputID)
    {
        KeyStateToBroadcast = keystate;
    }

    public override void PollBinding()
    {
        List<I> mappedKeys = new List<I>(_BoundInputValues.Keys);
        foreach (I mapKey in mappedKeys)
        {
            _BoundInputValues[mapKey] = _PollAction(mapKey);
        }

    }

}

public sealed class CKeyBinding<T> : CVarBroadcastBinding<T, KeyCode, bool>
{
    private bool IsJoystick { get; set; }

    public CKeyBinding(T inputID, KeyCode key, BroadcastKeyState keystate) : base(inputID, keystate)
	{
        _BoundInputValues[key] = false;
            
        IsJoystick = key.ToString().Contains("Joystick");
	}

    protected override bool PollPressed(KeyCode inputBind)
    {
        bool bRet = UInput.GetKeyDown(inputBind);

        if (bRet)
        {
            RetroEvents.Chapter("Input_" + BoundInputID.ToString()).Set(new KeyInputEvent(true, Time.deltaTime, IsJoystick, BoundInputID));
        }

        return bRet;
    }

    protected override bool PollReleased(KeyCode inputBind)
    {
        bool bRet = UInput.GetKeyUp(inputBind);

        if (bRet)
        {
            RetroEvents.Chapter("Input_" + BoundInputID.ToString()).Set(new KeyInputEvent(false, Time.deltaTime, IsJoystick, BoundInputID));
        }

        return bRet;
    }

    protected override bool PollHeld(KeyCode inputBind)
    {
        bool bRet = UInput.GetKey(inputBind);

        RetroEvents.Chapter("Input_" + BoundInputID.ToString()).Set(new KeyInputEvent(bRet, Time.deltaTime, IsJoystick, BoundInputID));
          
        return bRet;
    }

}

public sealed class CMouseBinding<T> : CVarBroadcastBinding<T, MouseButton, bool>
{

    public CMouseBinding(T inputID, MouseButton key, BroadcastKeyState keystate)
        : base(inputID, keystate)
    {
        _BoundInputValues[key] = false;
    }

    protected override bool PollPressed(MouseButton inputBind)
    {
        bool bRet = UInput.GetMouseButtonDown((int)inputBind);

        if (bRet)
        {
            RetroEvents.Chapter("Input_" + BoundInputID.ToString()).Set(new KeyInputEvent(true, Time.deltaTime, false, BoundInputID));
        }

        return bRet;
    }

    protected override bool PollReleased(MouseButton inputBind)
    {
        bool bRet = UInput.GetMouseButtonUp((int)inputBind);

        if (bRet)
        {
            RetroEvents.Chapter("Input_" + BoundInputID.ToString()).Set(new KeyInputEvent(false, Time.deltaTime, false, BoundInputID));
        }

        return bRet;
    }

    protected override bool PollHeld(MouseButton inputBind)
    {
        bool bRet = UInput.GetMouseButton((int)inputBind);

        RetroEvents.Chapter("Input_" + BoundInputID.ToString()).Set(new KeyInputEvent(bRet, Time.deltaTime, false, BoundInputID));
            
        return bRet;
    }
}

    public sealed class CAxisBinding<T> : CInputBinding<T, string, float>
    {
        private delegate void AxisLockAction();

        private Dictionary<string, float> _PrevBoundInputValues = new Dictionary<string, float>();
        private AxisLockAction _LockAction;
        bool _LockToDelta = false;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CAxisBinding`1"/> will lock to only send events on axis deltas.
        /// </summary>
        /// <value><c>true</c> if sending events only on deltas; otherwise, <c>false</c> if sending every poll.</value>
        public bool LockToDelta
        {
            get { return _LockToDelta; }
            set
            {
                _LockToDelta = value;
                if (_LockToDelta)
                {
                    _LockAction = OnLock;
                }
                else
                {
                    _LockAction = OnNoLock;
                }
            }
        }

        public CAxisBinding(T inputID, bool bLockDelta, params string[] axisList) : base(inputID)
        {
            // default to not locking input events to deltas
            LockToDelta = bLockDelta;
            foreach (string axis in axisList)
            {
                _BoundInputValues[axis] = 0.0f;
                _PrevBoundInputValues[axis] = -1.0f;
            }
        }

        public override void PollBinding()
        {
            _LockAction();
        }


        private void OnLock()
        {
            bool bSendEvent = false;
            List<string> mappedKeys = new List<string>(_BoundInputValues.Keys);
            foreach (string mapKey in mappedKeys)
            {
                _PrevBoundInputValues[mapKey] = _BoundInputValues[mapKey];
                _BoundInputValues[mapKey] = UInput.GetAxis(mapKey);

                bSendEvent = bSendEvent || (_PrevBoundInputValues[mapKey] != _BoundInputValues[mapKey]);
            }

            if (bSendEvent)
            {
                RetroEvents.Chapter("Input_" + BoundInputID.ToString()).Set(new AxisInputEvent(_BoundInputValues, Time.deltaTime, false, BoundInputID));
            }
        }

        private void OnNoLock()
        {
            List<string> mappedKeys = new List<string>(_BoundInputValues.Keys);
            foreach (string mapKey in mappedKeys)
            {
                _BoundInputValues[mapKey] = UInput.GetAxis(mapKey);
            }

            RetroEvents.Chapter("Input_" + BoundInputID.ToString()).Set(new AxisInputEvent(_BoundInputValues, Time.deltaTime, false, BoundInputID));
        }

    }
}