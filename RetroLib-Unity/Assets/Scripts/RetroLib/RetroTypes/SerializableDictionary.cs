using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Retro.Types
{	
	[Serializable]
	public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
	{
	    [SerializeField]
        private List<TKey> keys;

	    [SerializeField]
        private List<TValue> values;

        private Dictionary<TKey, TValue> _d = new Dictionary<TKey, TValue>();
        public Dictionary<TKey, TValue> D { get { return _d; } set { _d = value; } }

        public void SetValue(int nIndex, TKey key, TValue val)
        {
            keys[nIndex] = key;
            values[nIndex] = val;

            OnAfterDeserialize();
            OnBeforeSerialize();
        }

	    // save the dictionary to lists
	    public void OnBeforeSerialize()
	    {
            keys = new List<TKey>(D.Count);
            values = new List<TValue>(D.Count);

            int nIndex = 0;
            foreach (KeyValuePair<TKey, TValue> pair in D)
	        {
                keys.Add(pair.Key);
                values.Add(pair.Value);
                nIndex++;
	        }
	    }

	    // load dictionary from lists
	    public void OnAfterDeserialize()
	    {
            if (keys.Count != values.Count)
	            throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

            D = new Dictionary<TKey, TValue>();
            for (int i = 0; i < keys.Count; i++)
	            D.Add(keys[i], values[i]);
	    }
	}
}