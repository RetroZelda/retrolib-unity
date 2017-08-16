using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(PrefabLibrary))]
public class PrefabLibraryInspector : Editor
{
    [MenuItem("Assets/Create/ScriptableObject/PrefabLibrary")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<PrefabLibrary>();
    }

    public override void OnInspectorGUI()
    {
        PrefabLibrary lib = target as PrefabLibrary;
        

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Key");
        GUILayout.Label("Prefab");
        bool bAddNew = GUILayout.Button("New");
        EditorGUILayout.EndHorizontal();

        if (bAddNew)
        {
            lib.Set("New Key #" + lib.Keys.Count, null);
        }

        for (int nIndex = 0; nIndex < lib.Keys.Count; ++nIndex)
        {
            string key = lib.Keys[nIndex];
            GameObject obj = lib.Values[nIndex];

            EditorGUILayout.BeginHorizontal();
            lib.Keys[nIndex] = GUILayout.TextField(key);
            lib.Values[nIndex] = EditorGUILayout.ObjectField(obj, typeof(GameObject), false) as GameObject;
            bool bDelete = GUILayout.Button("Delete");
            EditorGUILayout.EndHorizontal();

            if(bDelete)
            {
                lib.Remove(key);
                break;
            }
        }

        EditorUtility.SetDirty(target);
    }
}
