using System;
using System.Reflection;
using Retro.Input.Events;
using UnityEngine;

namespace Retro.Input
{

    /// <summary>
    ///      Not the only way to use/wrap the input controller... but a behavior to be
    ///      attached to a scene object and referenced around the codebase
    /// </summary>
    public class InputBehavior : MonoBehaviour
    {
        // idk why i did it this way... ( •_•)  ( •_•)>⌐■-■  #DealWithIt  (⌐■_■)
        public Type InputType = typeof(string);
        private MethodInfo PollInputMethod;
        private MethodInfo RegisterKeyInputMethod;
        private MethodInfo RegisterAxisInputMethod;
        private MethodInfo RegisterMouseMethod;
        private MethodInfo UnregisterAxisMethod;
        private MethodInfo UnregisterKeyMethod;
        private MethodInfo UnregisterMouseMethod;
        private MethodInfo UnregisterInputMethod;
        private MethodInfo BindActionMethod;
        private MethodInfo UnbindActionMethod;

        // Use this for initialization
        private void Awake()
        {
            PollInputMethod = typeof(InputController<>).MakeGenericType(InputType).GetMethod("PollInput");
            RegisterKeyInputMethod = typeof(InputController<>).MakeGenericType(InputType).GetMethod("RegisterKey");
            RegisterAxisInputMethod = typeof(InputController<>).MakeGenericType(InputType).GetMethod("RegisterAxis");
            RegisterMouseMethod = typeof(InputController<>).MakeGenericType(InputType).GetMethod("RegisterMouse");
            UnregisterAxisMethod = typeof(InputController<>).MakeGenericType(InputType).GetMethod("UnregisterAxis");
            UnregisterKeyMethod = typeof(InputController<>).MakeGenericType(InputType).GetMethod("UnregisterKey");
            UnregisterMouseMethod = typeof(InputController<>).MakeGenericType(InputType).GetMethod("UnregisterMouse");
            UnregisterInputMethod = typeof(InputController<>).MakeGenericType(InputType).GetMethod("UnregisterInput");
            BindActionMethod = typeof(InputController<>).MakeGenericType(InputType).GetMethod("BindAction");
            UnbindActionMethod = typeof(InputController<>).MakeGenericType(InputType).GetMethod("UnbindAction");
        }

        // Update is called once per frame
        private void Update()
        {
            PollInputMethod.Invoke(null, null);
        }

        public void RegisterAxis(object BindKey, bool bLockDelta, params string[] axis)
        {
            RegisterAxisInputMethod.Invoke(null, new[] { BindKey, bLockDelta, axis });
        }

        public void RegisterKey(object BindKey, KeyCode key, BroadcastKeyState listenKeyState)
        {
            RegisterKeyInputMethod.Invoke(null, new[] { BindKey, key, listenKeyState });
        }

        public void RegisterMouse(object BindKey, MouseButton btn, BroadcastKeyState listenKeyState)
        {
            RegisterMouseMethod.Invoke(null, new[] { BindKey, btn, listenKeyState });
        }

        public void UnregisterAxis(object BindKey, bool bLockDelta, params string[] axis)
        {
            UnregisterAxisMethod.Invoke(null, new[] { BindKey, bLockDelta, axis });
        }

        public void UnregisterKey(object BindKey, KeyCode key, BroadcastKeyState listenKeyState)
        {
            UnregisterKeyMethod.Invoke(null, new[] { BindKey, key, listenKeyState });
        }

        public void UnregisterMouse(object BindKey, MouseButton btn, BroadcastKeyState listenKeyState)
        {
            UnregisterMouseMethod.Invoke(null, new[] { BindKey, btn, listenKeyState });
        }

        public void UnregisterInput(object BindKey)
        {
            UnregisterInputMethod.Invoke(null, new[] { BindKey });
        }

        public void BindAction(object BindKey, Action<InputEvent> InputHandler)
        {
            BindActionMethod.Invoke(null, new[] { BindKey, InputHandler });
        }

        public void UnbindAction(object BindKey, Action<InputEvent> InputHandler)
        {
            UnbindActionMethod.Invoke(null, new[] { BindKey, InputHandler });
        }
    }
}