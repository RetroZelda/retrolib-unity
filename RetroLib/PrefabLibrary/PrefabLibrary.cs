using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrefabLibrary : ScriptableObject
{
    [SerializeField]
    private List<string> _keys = new List<string>();

    [SerializeField]
    private List<GameObject> _Values = new List<GameObject>();
    
    public List<string> Keys { get { return _keys; } set { _keys = value; } }
    public List<GameObject> Values { get { return _Values; } set { _Values = value; } }

    public void Set(string szKey, GameObject value)
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

    public GameObject Obtain(string szKey)
    {
        int nIndex = _keys.FindIndex((k) => k == szKey);
        if (nIndex >= 0)
        {
            return _Values[nIndex];
        }
        return null;
    }

    public GameObject ObtainNewInstance(string szKey)
    {
        GameObject prefab = Obtain(szKey);
        if (prefab == null)
            return null;

        return Instantiate<GameObject>(prefab);
    }
}
