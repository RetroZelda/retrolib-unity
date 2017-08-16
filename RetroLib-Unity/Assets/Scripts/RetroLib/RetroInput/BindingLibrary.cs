using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Retro.Input
{
    public class BindingLibrary : ScriptableObject
    {
        [SerializeField]
        private List<string> _keys = new List<string>();

        [SerializeField]
        private List<JoystickLayoutMapping> _Values = new List<JoystickLayoutMapping>();

        public List<string> Keys { get { return _keys; } set { _keys = value; } }
        public List<JoystickLayoutMapping> Values { get { return _Values; } set { _Values = value; } }

        public void Set(string szKey, JoystickLayoutMapping value)
        {
            int nIndex = _keys.FindIndex((k) => k == szKey);
            if(nIndex < 0)
            {
                _keys.Add(szKey);
                _Values.Add(value);
            }
            else
            {
                _Values[nIndex] = value;
            }
        }

        public void Remove(string szKey)
        {
            int nIndex = _keys.FindIndex((k) => k == szKey);
            if (nIndex >= 0)
            {
                _keys.RemoveAt(nIndex);
                _Values.RemoveAt(nIndex);
            }
        }

        public JoystickLayoutMapping Obtain(string szKey)
        {
            int nIndex = _keys.FindIndex((k) => k == szKey);
            if (nIndex >= 0)
            {
                return _Values[nIndex];
            }
            return null;
        }
    }

}