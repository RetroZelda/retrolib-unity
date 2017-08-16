using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Retro.Events;
using System;
using Retro.Input.Events;
using Retro.Log;

namespace Retro.Input
{

    // NOTE: This class is left here as an example of Joystick binding

    /// <summary>
    /// A place where the default keybind exist
    /// TODO: Stop using strings
    /// </summary>
    public abstract class BaseJoypadInput : MonoBehaviour, IReader
    {
        public delegate void KeyAction(bool bHitState);
        public delegate void AxisAction(Vector2 values);
        public delegate void CustomKeyAction(string szID, bool bHitState);
        public delegate void CustomAxisAction(string szID, Vector2 values);

        private RetroLogger _log;
        protected RetroLogger Log
        {
            get {return _log ?? RetroLog.RetrieveCatagory("Unassigned Joystick");}
        }


        private JoystickAssigner assigner;
        private InputBehavior input;

        private JoystickBinding _FocusedJoystick = null;
        public JoystickBinding FocusedJoystick
        {
            get
            {
                return _FocusedJoystick;
            }
            // NOTE: This will NOT return the joystick to the assigner
            set
            {
                _FocusedJoystick = value;
                if (_FocusedJoystick != null)
                {
                    JoystickID = JoystickAssigner.GetJoyID(_FocusedJoystick);
                    _log = RetroLog.RetrieveCatagory(JoystickID);
                }
                else
                {
                    JoystickID = "";
                    _log = RetroLog.RetrieveCatagory("Unassigned Joystick");
                }
            }
        }
        public string JoystickID { get; private set; }

        [SerializeField]
        private JoystickLayoutMapping _layoutMap;
        public JoystickLayoutMapping LayoutMap 
        {
            get{ return _layoutMap; }
            set
            {
                if (_bRegistered)
                {
                    UnregisterInput();
                    _layoutMap = value;
                    RegisterInput();
                }
                else
                {
                    _layoutMap = value;
                }
            }
        }

        [SerializeField]
        // We wont want to automatically get a joystick if the joystick is to be attached dynamically(e.g. vehicle, god, etc)
        private bool AutomaticallyRequestJoystick = true;

        private bool _bRegistered = false;

        public KeyAction OnRightButton;
        public KeyAction OnBottomButton;
        public KeyAction OnLeftButton;
		public KeyAction OnTopButton;
		public KeyAction OnLeftBumper;
		public KeyAction OnRightBumper;
		public KeyAction OnStart;
		public KeyAction OnSelect;
		public KeyAction OnHome;
		public KeyAction OnLeftAnalogButton;
		public KeyAction OnRightAnalogButton;

		public AxisAction OnLeftAnalog;
        public AxisAction OnRightAnalog;
		public AxisAction OnDPad;
		public AxisAction OnLeftTrigger;
		public AxisAction OnRightTrigger;

        public CustomKeyAction OnCustomKey;
        public CustomAxisAction OnCustomAxis;

        public virtual void ReadPage(IPage page)
        {
            if (page is KeyInputEvent)
            {
				
                KeyInputEvent keyEvent = page as KeyInputEvent;
                if (keyEvent.BindKey.ToString() == JoystickID + "TopFace")          if(OnTopButton != null)         OnTopButton.Invoke(keyEvent.KeyHitState);
                if (keyEvent.BindKey.ToString() == JoystickID + "LeftFace")         if(OnLeftButton != null)        OnLeftButton.Invoke(keyEvent.KeyHitState);
                if (keyEvent.BindKey.ToString() == JoystickID + "RightFace")        if(OnRightButton != null)       OnRightButton.Invoke(keyEvent.KeyHitState);
                if (keyEvent.BindKey.ToString() == JoystickID + "BottomFace")       if(OnBottomButton != null)      OnBottomButton.Invoke(keyEvent.KeyHitState);
                if (keyEvent.BindKey.ToString() == JoystickID + "LeftBump")         if(OnLeftBumper != null)        OnLeftBumper.Invoke(keyEvent.KeyHitState);
                if (keyEvent.BindKey.ToString() == JoystickID + "RightBump")        if(OnRightBumper != null)       OnRightBumper.Invoke(keyEvent.KeyHitState);
                if (keyEvent.BindKey.ToString() == JoystickID + "Start")            if(OnStart != null)             OnStart.Invoke(keyEvent.KeyHitState);
                if (keyEvent.BindKey.ToString() == JoystickID + "Select")           if(OnSelect != null)            OnSelect.Invoke(keyEvent.KeyHitState);
                if (keyEvent.BindKey.ToString() == JoystickID + "Home")             if(OnHome != null)              OnHome.Invoke(keyEvent.KeyHitState);
                if (keyEvent.BindKey.ToString() == JoystickID + "LeftAnalogClick")  if(OnLeftAnalogButton != null)  OnLeftAnalogButton.Invoke(keyEvent.KeyHitState);
                if (keyEvent.BindKey.ToString() == JoystickID + "RightAnalogClick") if(OnRightAnalogButton != null) OnRightAnalogButton.Invoke(keyEvent.KeyHitState);

                {
                    // handle customs
                    foreach (KeyValuePair<string, KeyBind> pair in LayoutMap.CustomKeyBind.D)
                    {
                        if (keyEvent.BindKey.ToString() == JoystickID + pair.Key)
                        {
                            if (OnCustomKey != null)
                                OnCustomKey.Invoke(pair.Key, keyEvent.KeyHitState);
                        }
                    }
                }
                    
            }
            else if (page is AxisInputEvent)
            {
                AxisInputEvent AxisEvent = page as AxisInputEvent;
                if (AxisEvent.BindKey.ToString() == JoystickID + "LeftAnalog")
                {
                    if (OnLeftAnalog != null)
                        OnLeftAnalog.Invoke(new Vector2(AxisEvent.AxisMap[JoystickID + LayoutMap.LeftAnalogAxisX], AxisEvent.AxisMap[JoystickID + LayoutMap.LeftAnalogAxisY]));
                }
                else if (AxisEvent.BindKey.ToString() == JoystickID + "RightAnalog")
                {
                    if (OnRightAnalog != null)
                        OnRightAnalog.Invoke(new Vector2(AxisEvent.AxisMap[JoystickID + LayoutMap.RightAnalogAxisX], AxisEvent.AxisMap[JoystickID + LayoutMap.RightAnalogAxisY]));
                }
                else if (AxisEvent.BindKey.ToString() == JoystickID + "DPad")
                {
                    if (OnDPad != null)
                        OnDPad.Invoke(new Vector2(AxisEvent.AxisMap[JoystickID + LayoutMap.DirectionAxisX], AxisEvent.AxisMap[JoystickID + LayoutMap.DirectionAxisY]));
                }
                else if (AxisEvent.BindKey.ToString() == JoystickID + "LeftTrigger")
                {
                    if (OnLeftTrigger != null)
                        OnLeftTrigger.Invoke(new Vector2(AxisEvent.AxisMap[JoystickID + LayoutMap.LeftTriggerAxis], 0.0f));
                }
                else if (AxisEvent.BindKey.ToString() == JoystickID + "RightTrigger")
                {
                    if (OnRightTrigger != null)
                        OnRightTrigger.Invoke(new Vector2(AxisEvent.AxisMap[JoystickID + LayoutMap.RightTriggerAxis], 0.0f));
                }
                else
                {
                    // handle customs
                    foreach (KeyValuePair<string, AxisBind> pair in LayoutMap.CustomAxisBind.D)
                    {
                        if (AxisEvent.BindKey.ToString() == JoystickID + pair.Key)
                        {
                            if (OnCustomAxis != null)
                                OnCustomAxis.Invoke(pair.Key, new Vector2(AxisEvent.AxisMap[JoystickID + pair.Value.PlatformAxisID], 0.0f));
                        }
                    }
                }
            }
        }

        protected virtual void Awake()
        {
            input = FindObjectOfType<InputBehavior>();
            assigner = FindObjectOfType<JoystickAssigner>();

            if (AutomaticallyRequestJoystick)
            {
                // TODO: Push this to a function
                FocusedJoystick = assigner.RequestOpenJoystick();
                if(FocusedJoystick != null)
                {
                    JoystickID = JoystickAssigner.GetJoyID(FocusedJoystick);
                }
                else
                {
                    Debug.LogError("Error: No Joysticks available!");
                }
            }
        }

        protected virtual void OnDestroy()
        {
            if(FocusedJoystick != null)
            {
                assigner.ReturnOpenJoystick(FocusedJoystick);
                FocusedJoystick = null;
            }
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }

        public virtual void RegisterInput()
        {
            Log.LogWarning("NEED TO UNREGISTER!!!");
            if (FocusedJoystick == null || _bRegistered == true)
                return;

            input.RegisterKey(JoystickID + "TopFace",          JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.TopFaceButton),         BroadcastKeyState.PRESSED);
            input.RegisterKey(JoystickID + "LeftFace",         JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.LeftFaceButton),        BroadcastKeyState.PRESSED);
            input.RegisterKey(JoystickID + "RightFace",        JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.RightFaceButton),       BroadcastKeyState.PRESSED);
            input.RegisterKey(JoystickID + "BottomFace",       JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.BottomFaceButton),      BroadcastKeyState.PRESSED);
            input.RegisterKey(JoystickID + "LeftBump",         JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.LeftShoulderButton),    BroadcastKeyState.PRESSED);
            input.RegisterKey(JoystickID + "RightBump",        JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.RightShoulderButton),   BroadcastKeyState.PRESSED);
            input.RegisterKey(JoystickID + "Start",            JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.StartButton),           BroadcastKeyState.PRESSED);
            input.RegisterKey(JoystickID + "Select",           JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.SelectButton),          BroadcastKeyState.PRESSED);
            input.RegisterKey(JoystickID + "Home",             JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.HomeButton),            BroadcastKeyState.PRESSED);
            input.RegisterKey(JoystickID + "LeftAnalogClick",  JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.LeftAnalogButton),      BroadcastKeyState.PRESSED);
            input.RegisterKey(JoystickID + "RightAnalogClick", JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.RightAnalogButton),      BroadcastKeyState.PRESSED);


            input.RegisterKey(JoystickID + "TopFace",          JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.TopFaceButton),         BroadcastKeyState.RELEASED);
            input.RegisterKey(JoystickID + "LeftFace",         JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.LeftFaceButton),        BroadcastKeyState.RELEASED);
            input.RegisterKey(JoystickID + "RightFace",        JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.RightFaceButton),       BroadcastKeyState.RELEASED);
            input.RegisterKey(JoystickID + "BottomFace",       JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.BottomFaceButton),      BroadcastKeyState.RELEASED);
            input.RegisterKey(JoystickID + "LeftBump",         JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.LeftShoulderButton),    BroadcastKeyState.RELEASED);
            input.RegisterKey(JoystickID + "RightBump",        JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.RightShoulderButton),   BroadcastKeyState.RELEASED);
            input.RegisterKey(JoystickID + "Start",            JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.StartButton),           BroadcastKeyState.RELEASED);
            input.RegisterKey(JoystickID + "Select",           JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.SelectButton),          BroadcastKeyState.RELEASED);
            input.RegisterKey(JoystickID + "Home",             JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.HomeButton),            BroadcastKeyState.RELEASED);
            input.RegisterKey(JoystickID + "LeftAnalogClick",  JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.LeftAnalogButton),      BroadcastKeyState.RELEASED);
            input.RegisterKey(JoystickID + "RightAnalogClick", JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.RightAnalogButton),      BroadcastKeyState.RELEASED);
            
            input.RegisterAxis(JoystickID + "LeftAnalog",      false, JoystickID + LayoutMap.LeftAnalogAxisX,    JoystickID + LayoutMap.LeftAnalogAxisY);
            input.RegisterAxis(JoystickID + "RightAnalog",     false, JoystickID + LayoutMap.RightAnalogAxisX,   JoystickID + LayoutMap.RightAnalogAxisY);
            input.RegisterAxis(JoystickID + "DPad",            false, JoystickID + LayoutMap.DirectionAxisX,     JoystickID + LayoutMap.DirectionAxisY);
            input.RegisterAxis(JoystickID + "LeftTrigger",     false, JoystickID + LayoutMap.LeftTriggerAxis);
            input.RegisterAxis(JoystickID + "RightTrigger",    false, JoystickID + LayoutMap.RightTriggerAxis);

            // listen for the input events
            RetroEvents.Chapter("Input_" + JoystickID + "TopFace").Subscribe<KeyInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "LeftFace").Subscribe<KeyInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "RightFace").Subscribe<KeyInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "BottomFace").Subscribe<KeyInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "LeftBump").Subscribe<KeyInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "RightBump").Subscribe<KeyInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "Start").Subscribe<KeyInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "Select").Subscribe<KeyInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "Home").Subscribe<KeyInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "LeftAnalogClick").Subscribe<KeyInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "RightAnalogClick").Subscribe<KeyInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "LeftAnalog").Subscribe<AxisInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "RightAnalog").Subscribe<AxisInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "DPad").Subscribe<AxisInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "LeftTrigger").Subscribe<AxisInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "RightTrigger").Subscribe<AxisInputEvent>(this);

            // handle custom keys
            foreach (KeyValuePair<string, KeyBind> pair in LayoutMap.CustomKeyBind.D)
            {
                input.RegisterKey(JoystickID + pair.Key, JoystickAssigner.JoystickKeycode(JoystickID, pair.Value.PlatformKeyID), BroadcastKeyState.HELD);
                RetroEvents.Chapter("Input_" + JoystickID + pair.Key).Subscribe<KeyInputEvent>(this);
            }


            // handle custom axis
            foreach (KeyValuePair<string, AxisBind> pair in LayoutMap.CustomAxisBind.D)
            {
                input.RegisterAxis(JoystickID + pair.Key, false, JoystickID + pair.Value.PlatformAxisID);
                RetroEvents.Chapter("Input_" + JoystickID + pair.Key).Subscribe<AxisInputEvent>(this);
            }


            _bRegistered = true;
        }

        public virtual void UnregisterInput()
        {
            if (FocusedJoystick == null || _bRegistered == false)
                return;

            input.UnregisterKey(JoystickID + "TopFace",          JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.TopFaceButton),         BroadcastKeyState.PRESSED);
            input.UnregisterKey(JoystickID + "LeftFace",         JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.LeftFaceButton),        BroadcastKeyState.PRESSED);
            input.UnregisterKey(JoystickID + "RightFace",        JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.RightFaceButton),       BroadcastKeyState.PRESSED);
            input.UnregisterKey(JoystickID + "BottomFace",       JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.BottomFaceButton),      BroadcastKeyState.PRESSED);
            input.UnregisterKey(JoystickID + "LeftBump",         JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.LeftShoulderButton),    BroadcastKeyState.PRESSED);
            input.UnregisterKey(JoystickID + "RightBump",        JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.RightShoulderButton),   BroadcastKeyState.PRESSED);
            input.UnregisterKey(JoystickID + "Start",            JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.StartButton),           BroadcastKeyState.PRESSED);
            input.UnregisterKey(JoystickID + "Select",           JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.SelectButton),          BroadcastKeyState.PRESSED);
            input.UnregisterKey(JoystickID + "Home",             JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.HomeButton),            BroadcastKeyState.PRESSED);
            input.UnregisterKey(JoystickID + "LeftAnalogClick",  JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.LeftAnalogButton),      BroadcastKeyState.PRESSED);
            input.UnregisterKey(JoystickID + "RightAnalogClick", JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.RightAnalogButton),      BroadcastKeyState.PRESSED);

            input.UnregisterKey(JoystickID + "TopFace",          JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.TopFaceButton),         BroadcastKeyState.RELEASED);
            input.UnregisterKey(JoystickID + "LeftFace",         JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.LeftFaceButton),        BroadcastKeyState.RELEASED);
            input.UnregisterKey(JoystickID + "RightFace",        JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.RightFaceButton),       BroadcastKeyState.RELEASED);
            input.UnregisterKey(JoystickID + "BottomFace",       JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.BottomFaceButton),      BroadcastKeyState.RELEASED);
            input.UnregisterKey(JoystickID + "LeftBump",         JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.LeftShoulderButton),    BroadcastKeyState.RELEASED);
            input.UnregisterKey(JoystickID + "RightBump",        JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.RightShoulderButton),   BroadcastKeyState.RELEASED);
            input.UnregisterKey(JoystickID + "Start",            JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.StartButton),           BroadcastKeyState.RELEASED);
            input.UnregisterKey(JoystickID + "Select",           JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.SelectButton),          BroadcastKeyState.RELEASED);
            input.UnregisterKey(JoystickID + "Home",             JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.HomeButton),            BroadcastKeyState.RELEASED);
            input.UnregisterKey(JoystickID + "LeftAnalogClick",  JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.LeftAnalogButton),      BroadcastKeyState.RELEASED);
            input.UnregisterKey(JoystickID + "RightAnalogClick", JoystickAssigner.JoystickKeycode(JoystickID, LayoutMap.RightAnalogButton),      BroadcastKeyState.RELEASED);

            input.UnregisterAxis(JoystickID + "LeftAnalog",      false, JoystickID + LayoutMap.LeftAnalogAxisX,    JoystickID + LayoutMap.LeftAnalogAxisY);
            input.UnregisterAxis(JoystickID + "RightAnalog",     false, JoystickID + LayoutMap.RightAnalogAxisX,   JoystickID + LayoutMap.RightAnalogAxisY);
            input.UnregisterAxis(JoystickID + "DPad",            false, JoystickID + LayoutMap.DirectionAxisX,     JoystickID + LayoutMap.DirectionAxisY);
            input.UnregisterAxis(JoystickID + "LeftTrigger",     false, JoystickID + LayoutMap.LeftTriggerAxis);
            input.UnregisterAxis(JoystickID + "RightTrigger",    false, JoystickID + LayoutMap.RightTriggerAxis);

            // listen for the input events
            RetroEvents.Chapter("Input_" + JoystickID + "TopFace").Unsubscribe<KeyInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "LeftFace").Unsubscribe<KeyInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "RightFace").Unsubscribe<KeyInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "BottomFace").Unsubscribe<KeyInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "LeftBump").Unsubscribe<KeyInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "RightBump").Unsubscribe<KeyInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "Start").Unsubscribe<KeyInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "Select").Unsubscribe<KeyInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "Home").Unsubscribe<KeyInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "LeftAnalogClick").Unsubscribe<KeyInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "RightAnalogClick").Unsubscribe<KeyInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "LeftAnalog").Unsubscribe<AxisInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "RightAnalog").Unsubscribe<AxisInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "DPad").Unsubscribe<AxisInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "LeftTrigger").Unsubscribe<AxisInputEvent>(this);
            RetroEvents.Chapter("Input_" + JoystickID + "RightTrigger").Unsubscribe<AxisInputEvent>(this);

            // handle custom keys
            foreach (KeyValuePair<string, KeyBind> pair in LayoutMap.CustomKeyBind.D)
            {
                input.UnregisterKey(JoystickID + pair.Key, JoystickAssigner.JoystickKeycode(JoystickID, pair.Value.PlatformKeyID), BroadcastKeyState.HELD);
                RetroEvents.Chapter("Input_" + JoystickID + pair.Key).Unsubscribe<KeyInputEvent>(this);
            }


            // handle custom axis
            foreach (KeyValuePair<string, AxisBind> pair in LayoutMap.CustomAxisBind.D)
            {
                input.UnregisterAxis(JoystickID + pair.Key, false, JoystickID + pair.Value.PlatformAxisID);
                RetroEvents.Chapter("Input_" + JoystickID + pair.Key).Unsubscribe<AxisInputEvent>(this);
            }

            _bRegistered = false;
        }

        protected virtual void Update()
        {
            /*
            // NOTE: polling axis info
            for (int i = 0; i < 10; ++i ) // HACK: only axis 9
            {
                float fVal = Input.GetAxis(JoystickID + "Axis" + i);
                if ((fVal > 0.0f || fVal < 0.0f) && fVal > -1.0f && fVal < 1.0f)
                    Debug.Log(JoystickID + "Axis" + i + ": " + fVal);
            }

            // NOTE: polling button
            for(int i = 1; i < 20; ++i)
            {
                if(Input.GetKeyDown(JoystickKeycode(JoystickID, i)))
                {
                    Debug.Log(JoystickID + "Btn" + i);
                }
            }
            */
        }

    }
}