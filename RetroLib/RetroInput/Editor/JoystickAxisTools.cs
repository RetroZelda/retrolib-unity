using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Retro.Input
{

#if UNITY_EDITOR
    using UnityEditor;

    public class JoystickAxisTools : EditorWindow
    {

        public enum AxisType
        {
            KeyOrMouseButton = 0,
            MouseMovement = 1,
            JoystickAxis = 2
        };

        public class InputAxis
        {
            public string name;
            public string descriptiveName;
            public string descriptiveNegativeName;
            public string negativeButton;
            public string positiveButton;
            public string altNegativeButton;
            public string altPositiveButton;

            public float gravity;
            public float dead;
            public float sensitivity;

            public bool snap = false;
            public bool invert = false;

            public AxisType type;

            public int axis;
            public int joyNum;
        }

        private static string JoystickHeader = "Joystick";
        private static string AxisHeader = "Axis";

        private static SerializedProperty InputAsset
        {
            get
            {
                return new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]).FindProperty("m_Axes");
            }
        }

        private static List<InputAxis> AxisList { get; set; }


        [MenuItem("JoystickAxis/Refresh Mappings", false, 0)]
        public static void RefreshMappings()
        {
            ReadSavedAxis();
            RemoveAllMappings();
            InsertMappings();
            SaveAllAxis();
        }

        [MenuItem("JoystickAxis/Clear Mappings", false, 0)]
        public static void ClearMappings()
        {
            ReadSavedAxis();
            RemoveAllMappings();
            SaveAllAxis();
        }

        private static void RemoveAllMappings()
        {
            List<InputAxis> AxisTraversalCopy = new List<InputAxis>(AxisList);
            for (int nAxisIndex = 0; nAxisIndex < AxisTraversalCopy.Count; ++nAxisIndex)
            {
                InputAxis axis = AxisTraversalCopy[nAxisIndex];

                if (axis.name.Contains(JoystickHeader) && axis.name.Contains(AxisHeader))
                {
                    AxisList.Remove(axis);
                }
            }
        }

        private static void InsertMappings()
        {

            for (int nJoystickIndex = 0; nJoystickIndex < 12; ++nJoystickIndex)
            {
                // build the joystick index
                string szJoystick = JoystickHeader;
                if (nJoystickIndex > 0)
                {
                    szJoystick += nJoystickIndex.ToString();
                }

                for (int nAxisIndex = 0; nAxisIndex < 28; ++nAxisIndex)
                {
                    // build the axis index
                    string szAxis = AxisHeader + nAxisIndex.ToString();
                    string szFinal = szJoystick + szAxis;

                    InputAxis newAxis = new InputAxis();
                    newAxis.name = szFinal;
                    newAxis.type = AxisType.JoystickAxis;
                    newAxis.joyNum = nJoystickIndex;
                    newAxis.axis = nAxisIndex + 1;
                    newAxis.gravity = 0;
                    newAxis.dead = 0.19f;
                    newAxis.sensitivity = 1.0f;

                    AxisList.Add(newAxis);
                }
            }
        }

        private static void ReadSavedAxis()
        {
            // get to the list of axis'
            SerializedProperty axisList = InputAsset;
            axisList.Next(true);
            axisList.Next(true);

            AxisList = new List<InputAxis>();

            while (axisList.Next(false))
            {
                // the axis' info level
                SerializedProperty axis = axisList.Copy();

                InputAxis newAxis = new InputAxis();

                newAxis.name = GetChildProperty(axis, "m_Name").stringValue;
                newAxis.descriptiveName = GetChildProperty(axis, "descriptiveName").stringValue;
                newAxis.descriptiveNegativeName = GetChildProperty(axis, "descriptiveNegativeName").stringValue;
                newAxis.negativeButton = GetChildProperty(axis, "negativeButton").stringValue;
                newAxis.positiveButton = GetChildProperty(axis, "positiveButton").stringValue;
                newAxis.altNegativeButton = GetChildProperty(axis, "altNegativeButton").stringValue;
                newAxis.altPositiveButton = GetChildProperty(axis, "altPositiveButton").stringValue;
                newAxis.gravity = GetChildProperty(axis, "gravity").floatValue;
                newAxis.dead = GetChildProperty(axis, "dead").floatValue;
                newAxis.sensitivity = GetChildProperty(axis, "sensitivity").floatValue;
                newAxis.snap = GetChildProperty(axis, "snap").boolValue;
                newAxis.invert = GetChildProperty(axis, "invert").boolValue;
                newAxis.type = (AxisType)GetChildProperty(axis, "type").intValue;
                newAxis.axis = GetChildProperty(axis, "axis").intValue + 1;
                newAxis.joyNum = GetChildProperty(axis, "joyNum").intValue;

                AxisList.Add(newAxis);
            }
        }

        private static void SaveAllAxis()
        {
            // we need both here :/
            SerializedObject InputAsset = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            SerializedProperty axisList = InputAsset.FindProperty("m_Axes");

            // setup the final array for everything
            axisList.ClearArray();
            axisList.arraySize = AxisList.Count;

            int nCurIndexLocation = 0;
            foreach (InputAxis AxisToSave in AxisList)
            {
                SerializedProperty axis = axisList.GetArrayElementAtIndex(nCurIndexLocation++);

                GetChildProperty(axis, "m_Name").stringValue = AxisToSave.name;
                GetChildProperty(axis, "descriptiveName").stringValue = AxisToSave.descriptiveName;
                GetChildProperty(axis, "descriptiveNegativeName").stringValue = AxisToSave.descriptiveNegativeName;
                GetChildProperty(axis, "negativeButton").stringValue = AxisToSave.negativeButton;
                GetChildProperty(axis, "positiveButton").stringValue = AxisToSave.positiveButton;
                GetChildProperty(axis, "altNegativeButton").stringValue = AxisToSave.altNegativeButton;
                GetChildProperty(axis, "altPositiveButton").stringValue = AxisToSave.altPositiveButton;
                GetChildProperty(axis, "gravity").floatValue = AxisToSave.gravity;
                GetChildProperty(axis, "dead").floatValue = AxisToSave.dead;
                GetChildProperty(axis, "sensitivity").floatValue = AxisToSave.sensitivity;
                GetChildProperty(axis, "snap").boolValue = AxisToSave.snap;
                GetChildProperty(axis, "invert").boolValue = AxisToSave.invert;
                GetChildProperty(axis, "type").intValue = (int)AxisToSave.type;
                GetChildProperty(axis, "axis").intValue = AxisToSave.axis - 1;
                GetChildProperty(axis, "joyNum").intValue = AxisToSave.joyNum;
            }
            InputAsset.ApplyModifiedProperties();
        }

        private static SerializedProperty GetChildProperty(SerializedProperty parent, string name)
        {
            // jump down to the real properties
            SerializedProperty child = parent.Copy();
            child.Next(true);

            // check each property
            do
            {
                if (child.name == name)
                {
                    return child;
                }
            }
            while (child.Next(false));

            // not found
            return null;
        }

    }
#endif
}