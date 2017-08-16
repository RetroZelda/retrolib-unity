using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Retro.Events;

namespace Retro.Input.Events
{
    public class InputEvent : IPage
    {
        public float DeltaTime { get; private set; }
        public bool IsJoystick { get; private set; }
        public object BindKey { get; private set; }

        public InputEvent(float fDeltaTime, bool joyStick, object bindKey)
        {
            DeltaTime = fDeltaTime;
            IsJoystick = joyStick;
            BindKey = bindKey;
        }

        public virtual InputEvent Clone()
        {
            InputEvent newEvent = new InputEvent(DeltaTime, IsJoystick, BindKey);
            return newEvent;
        }

        public override string ToString()
        {
            return string.Format("DeltaTime = {0}; IsJoystick = {1}; BindKey = {2}; ", DeltaTime, IsJoystick, BindKey);
        }
    }

    public class KeyInputEvent : InputEvent
    {
        public bool KeyHitState { get; private set; }
        public KeyInputEvent(bool keyHitState, float fDeltaTime, bool joyStick, object bindKey) : base(fDeltaTime, joyStick, bindKey)
        {
            KeyHitState = keyHitState;
        }

        public override InputEvent Clone()
        {
            KeyInputEvent newEvent = new KeyInputEvent(KeyHitState, DeltaTime, IsJoystick, BindKey);
            return newEvent;
        }
        
        public override string ToString()
        {
            return string.Format("KeyHitState = {0}; ", KeyHitState) + base.ToString();
        }
    }

    public class AxisInputEvent : InputEvent
    {
        public Dictionary<string, float> AxisMap { get; private set; }
        public AxisInputEvent(Dictionary<string, float> axisMap, float fDeltaTime, bool joyStick, object bindKey) : base(fDeltaTime, joyStick, bindKey)
        {
            AxisMap = axisMap;
        }

        public override InputEvent Clone()
        {
            AxisInputEvent newEvent = new AxisInputEvent(new Dictionary<string, float>(AxisMap), DeltaTime, IsJoystick, BindKey);
            return newEvent;
        }
        
        public override string ToString()
        {
            string szMap = "[";

            foreach(string szKey in AxisMap.Keys)
            {
                float fVal = AxisMap[szKey];
                szMap += string.Format("{0} = {1}; ", szKey, fVal);
            }

            szMap.Remove(szMap.Length - 2, 2);
            szMap += "]";
            return string.Format("AxisMap = {0}; ", szMap) + base.ToString();
        }
    }
}