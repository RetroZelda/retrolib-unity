using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Retro.Input;
using Retro.Types;

[CustomEditor(typeof(JoystickLayoutMapping))]
public class JoystickLayoutMappingInspector : Editor 
{
	[MenuItem("Assets/Create/ScriptableObject/JoystickBinding")]
	public static void CreateAsset()
	{
		ScriptableObjectUtility.CreateAsset<JoystickLayoutMapping>();
	}

    private bool _bSave = false;

	public override void OnInspectorGUI()
	{
		JoystickLayoutMapping layout = target as JoystickLayoutMapping;

		FieldInfo[] fields = typeof(JoystickLayoutMapping).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		foreach(FieldInfo f in fields)
		{
			// str += f.Name + " = " + f.GetValue(data) + "\r\n";
			if (f.FieldType == typeof(KeyBind))
			{
				DrawKeybind(f.Name, f.GetValue(layout) as KeyBind);
			}
			else if (f.FieldType == typeof(AxisBind))
			{
				DrawAxisbind(f.Name, f.GetValue(layout) as AxisBind);
			}
		}


        EditorGUILayout.BeginHorizontal ();
        bool bAddKey = GUILayout.Button("Add Custom Key");
        bool bClearKey = GUILayout.Button("Clear Custom Keys");
        EditorGUILayout.EndHorizontal ();

        EditorGUILayout.BeginHorizontal ();
        bool bAddAxis = GUILayout.Button("Add Custom Axis");
        bool bClearAxis = GUILayout.Button("Clear Custom Axis");
        EditorGUILayout.EndHorizontal ();

        if (bAddKey)
        {
            int nCount = layout.CustomKeyBind.D.Count;
            string szText = "";
            do
            {
                szText = "New Key #" + nCount++;
            } while(layout.CustomKeyBind.D.ContainsKey(szText));
            layout.CustomKeyBind.D.Add(szText, new KeyBind());
            // layout.CustomKeyBind.OnBeforeSerialize();
            _bSave = true;
        }

        if (bAddAxis)
        {
            int nCount = layout.CustomAxisBind.D.Count;
            string szText = "";
            do
            {
                szText = "New Axis #" + nCount++;
            } while(layout.CustomKeyBind.D.ContainsKey(szText));
            layout.CustomAxisBind.D.Add(szText, new AxisBind());
            // layout.CustomAxisBind.OnBeforeSerialize();
            _bSave = true;
        }

        if (bClearKey)
        {
            layout.CustomKeyBind.D.Clear();
            // layout.CustomKeyBind.OnBeforeSerialize();
            _bSave = true;
        }

        if (bClearAxis)
        {
            layout.CustomAxisBind.D.Clear();
            // layout.CustomAxisBind.OnBeforeSerialize();
            _bSave = true;
        }
            
        EditorGUILayout.LabelField ("Custom Keys:"); 
        string[] keys = new List<string>(layout.CustomKeyBind.D.Keys).ToArray();
        KeyBind[] vals = new List<KeyBind>(layout.CustomKeyBind.D.Values).ToArray();
        for (int nKeyIndex = 0; nKeyIndex < layout.CustomKeyBind.D.Count; ++nKeyIndex)
        {
            DrawCustomKeybind(nKeyIndex, keys[nKeyIndex], vals[nKeyIndex]);
        }

        GUILayout.Space(5);
        EditorGUILayout.LabelField ("Custom Axis:");
        string[] axisKeys = new List<string>(layout.CustomAxisBind.D.Keys).ToArray();
        AxisBind[] axisVals = new List<AxisBind>(layout.CustomAxisBind.D.Values).ToArray();
        for (int nKeyIndex = 0; nKeyIndex < layout.CustomAxisBind.D.Count; ++nKeyIndex)
        {
            DrawCustomAxisbind(nKeyIndex, axisKeys[nKeyIndex], axisVals[nKeyIndex]);
        } 

        if (_bSave)
        {
            // layout.CustomKeyBind.OnAfterDeserialize();
            // layout.CustomAxisBind.OnAfterDeserialize();
            Debug.Log("Saving");
            EditorUtility.SetDirty(target); 
            AssetDatabase.SaveAssets();
            _bSave = false;
        } 
	}
        
    private void DrawKeybind(string szLabel, KeyBind kb)
    {
        EditorGUILayout.BeginHorizontal ();

        int nCache = kb.KeyID;
        int nCache1 = kb.KeyID_Win;

        EditorGUILayout.LabelField (szLabel);
        kb.KeyID = EditorGUILayout.IntField (kb.KeyID);
        kb.KeyID_Win = EditorGUILayout.IntField (kb.KeyID_Win);

        _bSave = _bSave || nCache != kb.KeyID || nCache1 != kb.KeyID_Win;

        EditorGUILayout.EndHorizontal ();

    }

    private void DrawAxisbind(string szLabel, AxisBind ab)
    {
        EditorGUILayout.BeginHorizontal ();

        string szCache = ab.AxisID;
        string szCache1 = ab.AxisID_Win;

        EditorGUILayout.LabelField (szLabel);
        ab.AxisID = EditorGUILayout.TextField (ab.AxisID);
        ab.AxisID_Win = EditorGUILayout.TextField (ab.AxisID_Win);

        _bSave = _bSave || szCache != ab.AxisID || szCache1 != ab.AxisID_Win;
         
        EditorGUILayout.EndHorizontal ();
    }

    private void DrawCustomKeybind(int nIndex, string szLabel, KeyBind kb)
    {
        JoystickLayoutMapping layout = target as JoystickLayoutMapping;
        EditorGUILayout.BeginHorizontal ();

        bool bDelete = GUILayout.Button("X");
        string szCache = EditorGUILayout.TextField (szLabel);
        int nCache = EditorGUILayout.IntField (kb.KeyID);
        int nCache1 = EditorGUILayout.IntField (kb.KeyID_Win);

        if (bDelete)
        {
            layout.CustomKeyBind.D.Remove(szLabel);
        }
        else
        {
            if (szCache != szLabel)
            {
                layout.CustomKeyBind.SetValue(nIndex, szCache, kb);
                _bSave = true;
            }

            if (nCache != kb.KeyID)
            {
                kb.KeyID = nCache;
                _bSave = true;
            }

            if (nCache1 != kb.KeyID_Win)
            {
                kb.KeyID_Win = nCache1;
                _bSave = true;
            }
        }

        EditorGUILayout.EndHorizontal ();

    }

    private void DrawCustomAxisbind(int nIndex, string szLabel, AxisBind ab)
    {
        JoystickLayoutMapping layout = target as JoystickLayoutMapping;
        EditorGUILayout.BeginHorizontal ();

        bool bDelete = GUILayout.Button("X");
        string szCache0 = EditorGUILayout.TextField (szLabel);
        string szCache1 = EditorGUILayout.TextField (ab.AxisID);
        string szCache2 = EditorGUILayout.TextField (ab.AxisID_Win);

        if (bDelete)
        {
            layout.CustomAxisBind.D.Remove(szLabel);
        }
        else
        {
            if (szCache0 != szLabel)
            {
                layout.CustomAxisBind.SetValue(nIndex, szCache0, ab);
                _bSave = true;
            }

            if (szCache1 != ab.AxisID)
            {
                ab.AxisID = szCache1;
                _bSave = true;
            }

            if (szCache2 != ab.AxisID_Win)
            {
                ab.AxisID_Win = szCache2;
                _bSave = true;
            }
        }

        EditorGUILayout.EndHorizontal ();
    }
}
