using UnityEngine;
using UnityEditor;
using Retro.Log;
using System.Collections;
using System.Collections.Generic;

namespace Retro.Log
{
    public class RetroLogEditor : EditorWindow
    {
        public readonly static string ENGINE_CATEGORY = "UNITYENGINE";
        public readonly static string ASSET_PATH_PREFIX = "Assets/";
        public readonly static bool FILTER_DEFAULT_VISIBILITY = true;
        public readonly static float HEADER_HEIGHT = 25.0f;
        public readonly static float INFO_RECT_HEIGHT = 30.0f;
        public readonly static float INFO_RECT_SPACE = 5.0f;
        public readonly static float CALLSTACK_BUTTON_HEIGHT = 60.0f;

        [System.Serializable]
        protected class CallStackDisplayNode
        {
            public string Function { get; private set; }
            public string File { get; private set; }
            public int Line { get; private set; }

            public CallStackDisplayNode(string szFunction, string szFile, int nLine)
            {
                Function = szFunction;
                File = szFile;
                Line = nLine;
            }

            public override string ToString()
            {
                // return Function + " ( at " + File + ":" + Line + ")";
                if (File == "")
                {
                    return "\n" + Function + "\n";
                }
                return Function + "\nat\n" + File + ":" + Line;
            }
        }

        [System.Serializable]
        protected class LogInfoDisplay
        {
            public string LogTime { get; private set; }
            public string Category { get; private set; }
            public string Level { get; private set; }
            public string Text { get; private set; }
            public CallStackDisplayNode[] Callstack { get; private set; }

            public LogInfoDisplay(string szCategory, string szLevel, string szText, CallStackDisplayNode[] pCallstack)
            {
                LogTime = Time.realtimeSinceStartup.ToString("0.0000");
                Category = szCategory;
                Level = szLevel;
                Text = szText;
                Callstack = pCallstack;
            }

            public override string ToString()
            {
                return "[" + LogTime + "]" + "[" + Category + "]" + Level + RetroLog.LOG_SEPARATOR + Text;
            }
        }

        [SerializeField]
        private List<LogInfoDisplay> LogStack = new List<LogInfoDisplay>();
        [SerializeField]
        private Queue<LogInfoDisplay> TempLogQueue = new Queue<LogInfoDisplay>();

        [SerializeField]
        private Dictionary<string, bool> FilterOptions = new Dictionary<string, bool>();

        [SerializeField]
        private Vector2 v2LogScrollPos;
        [SerializeField]
        private Vector2 v2CallStackScrollPos;
        [SerializeField]
        private float fCurrentScrollViewHeight;
        [SerializeField]
        private bool bResize;
        [SerializeField]
        private Rect ResizeRect;

        [SerializeField]
        private int nSelectedLog = -1;
        [SerializeField]
        private int nNextSelectedLog = -1;
        [SerializeField]
        private bool bIsDirty = false; // we need to use this because we can ONLY call Repaint after the GUILayout has been set ~PK
        [SerializeField]
        private bool bIsDrawing = false;
        [SerializeField]
        private short eLogVisibilityMask = -1; // unsigned 0xFFFF; defaults to ALL flags on

        [MenuItem("RetroTools/RetroLog", false, 5000)]
        static void OpenRetroLog()
        {
            // Get existing open window or if none, make a new one:
            RetroLogEditor elogWindow = (RetroLogEditor)EditorWindow.GetWindow<RetroLogEditor>("RetroLog");
            // Application.logMessageReceived += elogWindow.HandleLog;
            elogWindow.Show();
        }

        protected void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
            bResize = false;
            v2LogScrollPos = Vector2.zero;
            v2CallStackScrollPos = Vector2.zero;

            fCurrentScrollViewHeight = position.height / 2.0f;
            ResizeRect = new Rect(0.0f, fCurrentScrollViewHeight, position.width, 1.0f);
        }

        protected void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void HandleLog(string szLogText, string szStackTrace, LogType type)
        {
            string szCategory = "NONE";
            string szLevel = "";
            string szText = "";
            CallStackDisplayNode[] CallStack;

            // determine if RetroLog or standard log
            if (szLogText.Contains(RetroLog.LOG_HEADER))
            {
                szCategory = szLogText.Substring(RetroLog.LOG_HEADER.Length + 1);
                int nEndOfCategoryIndex = szCategory.IndexOf("]");

                szLevel = szCategory.Substring(nEndOfCategoryIndex + 1);
                int nEndOfLevelIndex = szLevel.IndexOf("]") + 1;

                szText = szLevel.Substring(nEndOfLevelIndex + RetroLog.LOG_SEPARATOR.Length);
                szCategory = szCategory.Remove(nEndOfCategoryIndex);
                szLevel = szLevel.Remove(nEndOfLevelIndex);

            }
            // standard log
            else
            {
                szCategory = ENGINE_CATEGORY;
                szLevel = LogLevelFromLogType(type).ToShortString();
                szText = szLogText;
            }

            // add log to our temp queue because of a race condition
            BuildCallstackArray(szStackTrace, out CallStack);

            if (bIsDrawing)
            {
                TempLogQueue.Enqueue(new LogInfoDisplay(szCategory, szLevel, szText, CallStack));
                bIsDirty = true;
            }
            else
            {
                LogStack.Insert(0, new LogInfoDisplay(szCategory, szLevel, szText, CallStack));
                Repaint();
            }

            // ensure we keep the same log selected
            if (nSelectedLog >= 0)
                nNextSelectedLog = nSelectedLog + 1;

        }

        protected void OnGUI()
        {
            bIsDrawing = true;

            // ensure our selected is within range 
            nSelectedLog = Mathf.Clamp(nSelectedLog, -1, LogStack.Count - 1);

            GUILayout.BeginVertical();

            // we need this
            DrawHeader();

            // draw the output
            // NOTE:    Theres a bug right here where the scrolling uses the current v2LogScrollPos.y
            //          and not the most uptodate v2LogScrollPos.y when using the scrollwheel.
            //          I cant figure out a fix, but it works when you manually use the scroll bar
            //          ~PK
            v2LogScrollPos = GUI.BeginScrollView(new Rect(0, HEADER_HEIGHT, position.width, fCurrentScrollViewHeight),
                v2LogScrollPos, new Rect(0, 0, position.width,
                                        ((((float)LogStack.Count + 1) * (INFO_RECT_HEIGHT + INFO_RECT_SPACE))) - (v2LogScrollPos.y + (INFO_RECT_HEIGHT + INFO_RECT_SPACE))));
            DrawInfo();
            GUI.EndScrollView(true);

            // get the count of the selected output's callstack
            int nNumCallstack = 0;
            if (LogStack.Count > 0 && nSelectedLog >= 0)
            {
                nNumCallstack = LogStack[nSelectedLog].Callstack.Length;
            }

            // start drawing the callstack
            v2CallStackScrollPos = GUI.BeginScrollView(new Rect(0, fCurrentScrollViewHeight + HEADER_HEIGHT, position.width, position.height - fCurrentScrollViewHeight - INFO_RECT_SPACE),
            v2CallStackScrollPos, new Rect(0, HEADER_HEIGHT, position.width, ((nNumCallstack + 1) * CALLSTACK_BUTTON_HEIGHT)));

            // draw if we have a selection
            if (LogStack.Count > 0 && nSelectedLog >= 0)
            {
                nSelectedLog = Mathf.Clamp(nSelectedLog, 0, LogStack.Count - 1);
                DrawCallStack(LogStack[nSelectedLog]);
            }

            GUI.EndScrollView(true);

            ResizeSplitView();

            GUILayout.EndVertical();

            OnGUICleanup();
        }

        private bool bClear = false;

        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayoutOption opt_height = GUILayout.Height(HEADER_HEIGHT - INFO_RECT_SPACE);

            // set the display
            List<string> CategoryList = new List<string>(RetroLog.GetCategoryList().Length + 3);
            foreach (string szCategory in RetroLog.GetCategoryList())
            {
                // ensure we have the filter
                if (!FilterOptions.ContainsKey(szCategory))
                {
                    FilterOptions.Add(szCategory, FILTER_DEFAULT_VISIBILITY);
                }

                // handle the display
                if (FilterOptions[szCategory] == true)
                {
                    CategoryList.Add(" ON - [" + szCategory + "]");
                }
                else
                {
                    CategoryList.Add("OFF - [" + szCategory + "]");
                }
            }

            // manually add the UNITYENGINE filter
            if (!FilterOptions.ContainsKey(ENGINE_CATEGORY))
                FilterOptions.Add(ENGINE_CATEGORY, FILTER_DEFAULT_VISIBILITY);
            if (FilterOptions[ENGINE_CATEGORY] == true)
                CategoryList.Insert(0, " ON - " + ENGINE_CATEGORY);
            else
                CategoryList.Insert(0, "OFF - " + ENGINE_CATEGORY);

            CategoryList.Insert(0, "Everything");
            CategoryList.Insert(0, "Nothing");

            // show the popup
            EditorGUILayout.LabelField("Filter:", GUILayout.Width(35.0f), opt_height);
            int nSelected = EditorGUILayout.Popup("", -1, CategoryList.ToArray(), GUILayout.Width(100.0f), opt_height);
            string[] keys = new string[FilterOptions.Keys.Count];
            FilterOptions.Keys.CopyTo(keys, 0);

            // handle the result
            switch (nSelected)
            {
                case -1:
                    // NOTHING SELECTED
                    break;
                case 0: // "Nothing"
                    foreach (string key in keys)
                    {
                        FilterOptions[key] = false;
                    }
                    break;
                case 1: // "Everything"
                    foreach (string key in keys)
                    {
                        FilterOptions[key] = true;
                    }
                    break;
                case 2: // "UNITYENGINE"
                    FilterOptions[ENGINE_CATEGORY] = !FilterOptions[ENGINE_CATEGORY]; // ugly line; sorry
                    break;
                default: // real ones
                    nSelected -= 3;
                    FilterOptions[RetroLog.GetCategoryList()[nSelected]] = !FilterOptions[RetroLog.GetCategoryList()[nSelected]]; // another ugly line; sorry
                    break;
            }

            // handle the visibility
            EditorGUILayout.LabelField("Visibility:", GUILayout.Width(55.0f), opt_height);
            eLogVisibilityMask = (short)(LogLevel)EditorGUILayout.EnumMaskPopup(GUIContent.none, (LogLevel)eLogVisibilityMask, GUILayout.Width(50.0f), opt_height);

            // handle the buttons
            bClear = GUILayout.Button("Clear", GUILayout.Width(100.0f), opt_height);


            // these are debug buttons
            if (GUILayout.Button("Log", opt_height))
            {
                RetroLog.RetrieveCatagory("LogTest").Log("Test #" + nDebugCount++);
            }
            if (GUILayout.Button("Log Error", opt_height))
            {
                RetroLog.RetrieveCatagory("LogErrorTest").LogError("Test #" + nDebugCount++);
            }
            if (GUILayout.Button("Log Warning", opt_height))
            {
                RetroLog.RetrieveCatagory("LogWarningTest").LogWarning("Test #" + nDebugCount++);
            }
            if (GUILayout.Button("Unity Log", opt_height))
            {
                Debug.Log("Test #" + nDebugCount++);
            }

            // end of debug buttons
            EditorGUILayout.EndHorizontal();
        }
        static int nDebugCount = 0;

        private void DrawInfo()
        {
            // textures
            Texture2D logTex0;
            Texture2D logTex1;
            Texture2D logTexSelected;

            logTexSelected = new Texture2D(1, 1);
            logTex0 = new Texture2D(1, 1);
            logTex1 = new Texture2D(1, 1);
            logTexSelected.SetPixels(new Color[] { new Color(62.0f / 256.0f, 95.0f / 256.0f, 150.0f / 256.0f, 1.0f) });
            logTex0.SetPixels(new Color[] { new Color(60.0f / 256.0f, 60.0f / 256.0f, 60.0f / 256.0f, 1.0f) });
            logTex1.SetPixels(new Color[] { new Color(55.0f / 256.0f, 55.0f / 256.0f, 55.0f / 256.0f, 1.0f) });
            // logTex0.SetPixels(new Color[] { Color.red });
            // logTex1.SetPixels(new Color[] { Color.blue });

            bool bColorToggle = false;
            float fRectOffset = -v2LogScrollPos.y;// 0.0f;

            int nLogCount = 0;
            foreach (LogInfoDisplay logInfo in LogStack)
            {
                // handle category filter
                if (!FilterOptions.ContainsKey(logInfo.Category) || !FilterOptions[logInfo.Category])
                    continue;

                // handle log level visibility
                if (((byte)LogLevelFromShortString(logInfo.Level) & eLogVisibilityMask) == 0)
                    continue;

                Rect logRect = new Rect(0.0f, fRectOffset, position.width, INFO_RECT_HEIGHT);

                // determine rect color
                if (nLogCount == nSelectedLog)
                {
                    GUI.DrawTexture(logRect, logTexSelected, ScaleMode.StretchToFill);
                }
                else if (bColorToggle)
                {
                    GUI.DrawTexture(logRect, logTex0, ScaleMode.StretchToFill);
                }
                else
                {
                    GUI.DrawTexture(logRect, logTex1, ScaleMode.StretchToFill);
                }

                // set the font color
                Color fontColor = Color.black;
                switch (LogLevelFromShortString(logInfo.Level))
                {
                    case LogLevel.Exception:
                    case LogLevel.Error:
                        fontColor = Color.red;
                        break;
                    case LogLevel.Warning:
                        fontColor = Color.yellow;
                        break;
                }

                // display information
                GUIStyle s = new GUIStyle(EditorStyles.textField);
                s.normal.textColor = fontColor;
                // GUI.contentColor = fontColor;
                EditorGUI.LabelField(logRect, logInfo.ToString(), s);

                // check rect click
                if (Event.current.type == EventType.mouseDown && logRect.Contains(Event.current.mousePosition))
                {
                    nNextSelectedLog = nLogCount;
                    bIsDirty = true;
                }

                // update vars
                fRectOffset += INFO_RECT_HEIGHT + INFO_RECT_SPACE;
                bColorToggle = !bColorToggle;
                nLogCount++;
            }
        }

        private void DrawCallStack(LogInfoDisplay logInfo)
        {
            if (logInfo == null)
                return;

            GUI.contentColor = Color.white; // forces white if left off on an error inside DrawInfo();
            foreach (CallStackDisplayNode stackNode in logInfo.Callstack)
            {
                EditorGUILayout.BeginHorizontal();
                GUI.enabled = stackNode.File != "";
                if (GUILayout.Button(stackNode.ToString(), GUILayout.Height(CALLSTACK_BUTTON_HEIGHT)))
                {
                    // open the script in default editor
                    TextAsset script = AssetDatabase.LoadAssetAtPath<TextAsset>(stackNode.File);
                    AssetDatabase.OpenAsset(script, stackNode.Line);
                    // UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(stackNode.File, stackNode.Line); // opens in mono only
                }
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
            }
        }

        private void ResizeSplitView()
        {
            // black texture
            Texture2D resizeTex = new Texture2D(1, 1);
            resizeTex.SetPixels(new Color[] { Color.black });

            ResizeRect.Set(ResizeRect.x, fCurrentScrollViewHeight + HEADER_HEIGHT, position.width, ResizeRect.height);
            Rect bufferRect = new Rect(ResizeRect.x, fCurrentScrollViewHeight + HEADER_HEIGHT, ResizeRect.width, ResizeRect.height * 5.0f);

            GUI.DrawTexture(ResizeRect, resizeTex, ScaleMode.StretchToFill);
            EditorGUIUtility.AddCursorRect(bufferRect, MouseCursor.ResizeVertical);

            // flag resize if needed
            if (Event.current.type == EventType.mouseDown && bufferRect.Contains(Event.current.mousePosition))
                bResize = true;

            // handle resizing
            if (bResize)
            {
                fCurrentScrollViewHeight = (Event.current.mousePosition.y - HEADER_HEIGHT) - (bufferRect.height / 2.0f);
                bIsDirty = true;
            }

            if (Event.current.type == EventType.MouseUp)
                bResize = false;
        }

        public void ClearLog()
        {
            LogStack.Clear();
            nSelectedLog = -1;
            bIsDirty = true;
        }

        // handle everything relating to GUI elements in here
        private void OnGUICleanup()
        {
            // handle clearing the log
            if (bClear)
            {
                ClearLog();
                bClear = false;
            }

            // flush our queue
            while (TempLogQueue.Count > 0)
            {
                LogStack.Insert(0, TempLogQueue.Dequeue());
            }

            // refresh our selected
            if (nNextSelectedLog != nSelectedLog)
                nSelectedLog = nNextSelectedLog;

            // repaint if needed
            if (bIsDirty)
            {
                bIsDirty = false;
                Repaint();
            }
            bIsDrawing = false;
        }

        private static int BuildCallstackArray(string szStackTrace, out CallStackDisplayNode[] OutArray)
        {
            string szRemainingStackTrace = szStackTrace;
            List<CallStackDisplayNode> tempStackList = new List<CallStackDisplayNode>();

            // continue going until end
            while (szRemainingStackTrace.Length > 0)
            {
                string szFunction = "";
                string szFile = "";
                int nLine = -1;
                bool bAdd = true;

                // grab the function name (beginning of line --> first whitespace)
                int nNewLineIndex = szRemainingStackTrace.IndexOf(")\n") + 1;
                int nSpaceIndex = szRemainingStackTrace.IndexOf(") ") + 1;

                // if a new line is before a space, then this stack node doesnt have an editable file
                if (nSpaceIndex > 0 && nSpaceIndex < nNewLineIndex)
                {
                    // use the space index to pull our function name
                    szFunction = szRemainingStackTrace.Substring(0, nSpaceIndex);

                    // grab file name ("at " --> colon (:))
                    int nAtIndex = szRemainingStackTrace.IndexOf("at ") + 3;
                    int nExtentionIndex = szRemainingStackTrace.IndexOf(".cs") + 3;
                    szFile = szRemainingStackTrace.Substring(nAtIndex, nExtentionIndex - nAtIndex);

                    // stop the callstack at any RetroLog related points
                    if (szFile.Contains("RetroLog.cs") && szFunction.Contains(":Log"))
                    {
                        bAdd = false;
                    }
                    // ensure we only hold local asset scripts
                    else if (!szFile.StartsWith(ASSET_PATH_PREFIX))
                    {
                        szFile = "";
                    }
                    else
                    {
                        // and grab line number(colon --> close parentesis (")")
                        string szLine = szRemainingStackTrace.Substring(nExtentionIndex + 1).Remove(nNewLineIndex - nExtentionIndex - 2);
                        nLine = int.Parse(szLine);
                    }
                }
                else
                {
                    // use the newline index to pull our function name
                    szFunction = szRemainingStackTrace.Substring(0, nNewLineIndex);
                    bAdd = !szFunction.Contains("UnityEngine.Debug:Log");
                }

                // add our info
                if (bAdd)
                    tempStackList.Add(new CallStackDisplayNode(szFunction, szFile, nLine));

                // remove what we dont need
                szRemainingStackTrace = szRemainingStackTrace.Remove(0, nNewLineIndex + 1);
            }

            OutArray = tempStackList.ToArray();
            return tempStackList.Count;
        }

        public static LogLevel LogLevelFromLogType(LogType type)
        {
            switch (type)
            {
                case LogType.Assert:
                    return LogLevel.Error;
                case LogType.Error:
                    return LogLevel.Error;
                case LogType.Exception:
                    return LogLevel.Exception;
                case LogType.Log:
                    return LogLevel.Debug;
                case LogType.Warning:
                    return LogLevel.Warning;
            }
            return LogLevel.Verbose;
        }

        public static LogLevel LogLevelFromShortString(string szShort)
        {
            switch (szShort.ToLower())
            {
                case "v":
                case "[v]":
                    return LogLevel.Verbose;
                case "e":
                case "[e]":
                    return LogLevel.Error;
                case "w":
                case "[w]":
                    return LogLevel.Warning;
                case "d":
                case "[d]":
                    return LogLevel.Debug;
                case "x":
                case "[x]":
                    return LogLevel.Exception;
                default:
                    return LogLevel.None;
            }
        }
    }
}