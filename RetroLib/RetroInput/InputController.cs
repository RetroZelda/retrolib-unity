using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Retro.Events;
using Retro.Input.Events;

namespace Retro.Input
{

    public delegate int InputEventHandler(InputEvent e);

    /// <summary>
    /// Input controller to handle sending events for mapped key events and axis deltas.
    /// Generic type is binding type for key identification
    /// </summary>
    /// 
    public class InputController<T>
    {

        private Dictionary<T, List<IInputBinding>> _BindingMap;

        ///// singleton stuff... should this be a singleton>  #yolo ///////////////////////////////
        private InputController()
        {
            _BindingMap = new Dictionary<T, List<IInputBinding>>();
        }

        private static InputController<T> _Instance = new InputController<T>();
        protected static InputController<T> Instance { get { return _Instance; } }

        ///////////////////////////////////////////////////////////////////////////////////////////

        ///// internal methods that should be wrapped through static/singleton instance calls /////
        private void PollInputInternal()
        {
            foreach (KeyValuePair<T, List<IInputBinding>> bindingList in _BindingMap)
            {
                foreach (IInputBinding binding in bindingList.Value.ToArray())
                {
                    binding.PollBinding();
                }
            }
        }

        private void RegisterAxisInternal(T BindKey, bool bLockDelta, params string[] axis)
        {
            if (_BindingMap.ContainsKey(BindKey) == false)
            {
                _BindingMap.Add(BindKey, new List<IInputBinding>());
            }
            _BindingMap[BindKey].Add(new CAxisBinding<T>(BindKey, bLockDelta, axis));
        }

        private void RegisterKeyInernal(T BindKey, KeyCode key, BroadcastKeyState listenKeyState)
        {
            if (_BindingMap.ContainsKey(BindKey) == false)
            {
                _BindingMap.Add(BindKey, new List<IInputBinding>());
            }
            _BindingMap[BindKey].Add(new CKeyBinding<T>(BindKey, key, listenKeyState));
        }

        private void RegisterMouseInernal(T BindKey, MouseButton btn, BroadcastKeyState listenKeyState)
        {
            if (_BindingMap.ContainsKey(BindKey) == false)
            {
                _BindingMap.Add(BindKey, new List<IInputBinding>());
            }
            _BindingMap[BindKey].Add(new CMouseBinding<T>(BindKey, btn, listenKeyState));
        }

        private void UnregisterAxisInternal(T BindKey, bool bLockDelta, params string[] axis)
        {
            if (_BindingMap.ContainsKey(BindKey) == false)
            {
                return;
            }

            foreach (IInputBinding binding in _BindingMap[BindKey])
            {
                // ensure we are axis type
                if (binding.GetType() == typeof(CAxisBinding<T>))
                {
                    CAxisBinding<T> axisBind = binding as CAxisBinding<T>;

                    // ensure we match the delta lock
                    if (axisBind.LockToDelta != bLockDelta)
                        continue;

                    // ensure we match ALL the axis
                    if (axisBind.BoundInputValues.Keys.Count != axis.Length)
                    {
                        continue;
                    }

                    // ensure we contain all of axis
                    List<string> heldAxis = new List<string>(axisBind.BoundInputValues.Keys);
                    bool bGone = true; // lol... begone... heh
                    foreach (string axisCheck in axis)
                    {
                        if (heldAxis.Contains(axisCheck) == false)
                        {
                            bGone = false;
                            break;
                        }
                    }

                    // we can remove this
                    if (bGone)
                    {
                        _BindingMap[BindKey].Remove(binding);
                        return; // must retun here to avoid a sync runtime error
                    }
                }
            }
        }

        private void UnregisterKeyInernal(T BindKey, KeyCode key, BroadcastKeyState listenKeyState)
        {
            if (_BindingMap.ContainsKey(BindKey) == false)
            {
                return;
            }

            foreach (IInputBinding binding in _BindingMap[BindKey])
            {
                // ensure we are axis type
                if (binding.GetType() == typeof(CKeyBinding<T>))
                {
                    CKeyBinding<T> keyBind = binding as CKeyBinding<T>;

                    // ensure we match the key state to broadcast
                    if (keyBind.KeyStateToBroadcast != listenKeyState)
                        continue;

                    // ensure we match the key
                    // NOTE: seems redundant with the dictionary, but was how im keeping things generic over there
                    //       BUT! it should always have 1 key in there... should... maybe i should use an assert?       nah
                    List<KeyCode> heldKeys = new List<KeyCode>(keyBind.BoundInputValues.Keys);
                    if (heldKeys[0] != key)
                    {
                        continue;
                    }

                    // save to remove
                    _BindingMap[BindKey].Remove(binding);
                    return; // must retun here to avoid a sync runtime error
                }
            }
        }

        private void UnregisterMouseInernal(T BindKey, MouseButton btn, BroadcastKeyState listenKeyState)
        {
            if (_BindingMap.ContainsKey(BindKey) == false)
            {
                return;
            }

            foreach (IInputBinding binding in _BindingMap[BindKey])
            {
                // ensure we are axis type
                if (binding.GetType() == typeof(CMouseBinding<T>))
                {
                    CMouseBinding<T> keyBind = binding as CMouseBinding<T>;

                    // ensure we match the key state to broadcast
                    if (keyBind.KeyStateToBroadcast != listenKeyState)
                        continue;

                    // ensure we match the key
                    // NOTE: seems redundant with the dictionary, but was how im keeping things generic over there
                    //       BUT! it should always have 1 key in there... should... maybe i should use an assert?       nah
                    // Yup... copy pasta...
                    List<MouseButton> heldKeys = new List<MouseButton>(keyBind.BoundInputValues.Keys);
                    if (heldKeys[0] != btn)
                    {
                        continue;
                    }

                    // save to remove
                    _BindingMap[BindKey].Remove(binding);
                    return; // must retun here to avoid a sync runtime error
                }
            }
        }

        private void UnregisterInputInternal(T BindKey)
        {
            _BindingMap.Remove(BindKey);
        }

        private void BindActionInternal<E>(T BindKey, IReader inputReader) where E : InputEvent
        {
            RetroEvents.Chapter("Input_" + BindKey.ToString()).Subscribe<E>(inputReader);
        }

        private void UnbindActionInternal<E>(T BindKey, IReader inputReader) where E : InputEvent
        {
            RetroEvents.Chapter("Input_" + BindKey.ToString()).Unsubscribe<E>(inputReader);
        }
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///// Call stack explosion to keep lines narrow - the public interface of static //////////
        public static void PollInput()
        {
            Instance.PollInputInternal();
        }

        public static void RegisterAxis(T BindKey, bool bLockDelta, params string[] axis)
        {
            Instance.RegisterAxisInternal(BindKey, bLockDelta, axis);
        }

        public static void RegisterKey(T BindKey, KeyCode key, BroadcastKeyState listenKeyState)
        {
            Instance.RegisterKeyInernal(BindKey, key, listenKeyState);
        }

        public static void RegisterMouse(T BindKey, MouseButton btn, BroadcastKeyState listenKeyState)
        {
            Instance.RegisterMouseInernal(BindKey, btn, listenKeyState);
        }

        public static void UnregisterAxis(T BindKey, bool bLockDelta, params string[] axis)
        {
            Instance.UnregisterAxisInternal(BindKey, bLockDelta, axis);
        }

        public static void UnregisterKey(T BindKey, KeyCode key, BroadcastKeyState listenKeyState)
        {
            Instance.UnregisterKeyInernal(BindKey, key, listenKeyState);
        }

        public static void UnregisterMouse(T BindKey, MouseButton btn, BroadcastKeyState listenKeyState)
        {
            Instance.UnregisterMouseInernal(BindKey, btn, listenKeyState);
        }

        public static void UnregisterInput(T BindKey)
        {
            Instance.UnregisterInputInternal(BindKey);
        }

        public static void BindAction<E>(T BindKey, IReader inputReader) where E : InputEvent
        {
            Instance.BindActionInternal<E>(BindKey, inputReader);
        }

        public static void UnbindAction<E>(T BindKey, IReader inputReader) where E : InputEvent
        {
            Instance.UnbindActionInternal<E>(BindKey, inputReader);
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
    }
}