using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UInput = UnityEngine.Input;

namespace Retro.Input
{

    public class JoystickBinding
    {
        public bool JoystickInUse { get; set; }
        public int JoystickIndex { get; set; }
        public string JoystickName { get; set; }

        public JoystickBinding()
        {
            JoystickInUse = false;
            JoystickIndex = -1;
            JoystickName = "";
        }

        public JoystickBinding(bool bUse, int nIndex, string szName)
        {
            JoystickInUse = bUse;
            JoystickIndex = nIndex;
            JoystickName = szName;
        }
    }

    public class JoystickAssigner : MonoBehaviour
    {
        public List<JoystickBinding> Joysticks { get; private set; }

        void Awake()
        {
            string[] szJoysticks = UInput.GetJoystickNames();
            Joysticks = new List<JoystickBinding>();

            // get a binding of all the joysticks
            int nIndex = 0;
            foreach (string joystick in szJoysticks)
            {
                if (joystick != "" && !joystick.ToLower().Contains("vjoy"))
                {
                    Joysticks.Add(new JoystickBinding(false, nIndex, joystick));
                }
                nIndex++;
            }
        }

        public JoystickBinding RequestOpenJoystick(string JoystickID)
        {
            foreach (JoystickBinding joy in Joysticks)
            {
                if (joy.JoystickInUse == false && JoystickID == GetJoyID(joy))
                {
                    joy.JoystickInUse = true;
                    return joy;
                }
            }

            // out of free joysticks with the requested ID
            return null;
        }

        public JoystickBinding RequestOpenJoystick()
        {
            foreach (JoystickBinding joy in Joysticks)
            {
                if (joy.JoystickInUse == false)
                {
                    joy.JoystickInUse = true;
                    return joy;
                }
            }

            // out of free joysticks
            return null;
        }

        public void ReturnOpenJoystick(JoystickBinding joy)
        {
            joy.JoystickInUse = false;
        }


        public static string GetJoyID(JoystickBinding joyBinding)
        {
            return "Joystick" + (joyBinding.JoystickIndex + 1).ToString();
        }

        public static KeyCode JoystickKeycode(string joystick, int JoyButton)
        {
            return (KeyCode)System.Enum.Parse(typeof(KeyCode), joystick + "Button" + (JoyButton - 0));
        }
    }
}