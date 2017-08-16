using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Retro.Input
{

    [CustomEditor(typeof(BindingLibrary))]
    public class BindingLibraryInspector : Editor
    {
        [MenuItem("Assets/Create/ScriptableObject/BindingLibrary")]
        public static void CreateAsset()
        {
            ScriptableObjectUtility.CreateAsset<BindingLibrary>();
        }

        public override void OnInspectorGUI()
        {
            BindingLibrary lib = target as BindingLibrary;
            

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Key");
            GUILayout.Label("Binding");
            bool bAddNew = GUILayout.Button("New");
            EditorGUILayout.EndHorizontal();

            if (bAddNew)
            {
                lib.Set("New Key #" + lib.Keys.Count, null);
            }

            for (int nIndex = 0; nIndex < lib.Keys.Count; ++nIndex)
            {
                string key = lib.Keys[nIndex];
                JoystickLayoutMapping obj = lib.Values[nIndex];

                EditorGUILayout.BeginHorizontal();
                lib.Keys[nIndex] = GUILayout.TextField(key);
                lib.Values[nIndex] = EditorGUILayout.ObjectField(obj, typeof(JoystickLayoutMapping), false) as JoystickLayoutMapping;
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
}