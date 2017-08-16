using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UInput = UnityEngine.Input;

namespace Retro.Command
{    
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class RetroCommand : Attribute
    {
        public string Description { get; private set; }
        public RetroCommand()
        {
            Description = "No Description.";
        }

        public RetroCommand(string szDescription)
        {
            Description = szDescription;
        }
    }

    public class RetroConsole : MonoBehaviour 
    {
        private class Command
        {
            public Type OwningBehaviorType; 
            public string Name;
            public string Description;
            public string ParamDescription;
            public System.Type[] Parameters;

            public override string ToString()
            {
                return Name + " - " + Description;
            }
        }

        [SerializeField]
        private Canvas _UIRoot;

        [SerializeField]
        private ScrollRect _ScrollZone;

        [SerializeField]
        private Text _ConsoleTextDestination;

        [SerializeField]
        private InputField _CommandLocation;

        [SerializeField]
        private Text _CommandLocationHelpText;

        [SerializeField]
        private Button _CommandSubmitButton;

        [SerializeField]
        private int _MaxLogChars = 10000;

        [SerializeField]
        private Button _CloseButton;


        [SerializeField]
        [Range(0, 100)]
        private int _MaxSuggestions = 5;

        [SerializeField]
        private RectTransform _SuggestionPanel;

        [SerializeField]
        private SuggestionButton _SuggestionButtonPrefab;

        public bool IsOpen { get; private set; }

        private LinkedList<Command> _Commands;
        private int _nPrevTouchCount;
        private int _nCurTouchCount;

        void Start()
        {
            _nPrevTouchCount = 0;
            _nCurTouchCount = 0;

            _ConsoleTextDestination.text = ""; // clear the field

            CloseConsole(); // start closed
            ObtainAllCommands();
        }

        void OnEnable()
        {
            Application.logMessageReceived += RecieveLogCallback;
            _CommandLocation.onValueChanged.AddListener(OnCommandGettingTypedIn);
        }

        void OnDisable()
        {
            Application.logMessageReceived -= RecieveLogCallback;
            _CommandLocation.onValueChanged.RemoveListener(OnCommandGettingTypedIn);
        }

        void Update()
        {
            _nPrevTouchCount = _nCurTouchCount;
            _nCurTouchCount = UInput.touchCount;

            if ((_nPrevTouchCount != _nCurTouchCount && _nCurTouchCount == 4) || (UInput.GetKeyDown(KeyCode.BackQuote)))
            {
                if (IsOpen)
                {
                    CloseConsole();
                }
                else
                {
                    OpenConsole();
                }
            }

            if (UInput.GetKeyDown(KeyCode.Return) || UInput.GetKeyDown(KeyCode.KeypadEnter))
            {
                SubmitCommand();
            }
        }

        void LateUpdate()
        {
        }

        public void OpenConsole()
        {
            _UIRoot.gameObject.SetActive(true);
            IsOpen = true;

            // ensure we start at the bottom 
            _ScrollZone.verticalScrollbar.value = 0.0f;

            // sub to buttons and crap
            _CommandSubmitButton.onClick.AddListener(SubmitCommand);
            _CloseButton.onClick.AddListener(CloseConsole);

            _CommandLocation.Select();
        }

        public void CloseConsole()
        {
            _UIRoot.gameObject.SetActive(false);
            IsOpen = false;

            // unsub to buttons and crap
            _CommandSubmitButton.onClick.RemoveListener(SubmitCommand);
            _CloseButton.onClick.RemoveListener(CloseConsole);
        }

        [RetroCommand("Display all commands")]
        private void Help()
        {
            foreach (Command cmd in _Commands)
            {
                Debug.Log("<b>" + cmd.Name + "</b> - " + cmd.Description + " - <color=#9999999>" + cmd.ParamDescription + "</color>");                
            } 
        }


        private void OnCommandGettingTypedIn(string szCommand)
        {
            List<Command> matchingCommands = new List<Command>();
            _CommandLocationHelpText.text = "Enter Command...";

            // compile list of commands
            if(string.IsNullOrEmpty(szCommand) == false)
            {
                _CommandLocationHelpText.text = "";
                _CommandLocationHelpText.enabled = true;
                string szRawCommand = szCommand.Trim(' ');

                // grab every command that matches our partial command
                foreach(Command cmd in _Commands)
                {
                    // handle if we have a command typed in full
                    if(cmd.Name == szRawCommand)
                    {
                        _CommandLocationHelpText.text = szRawCommand + " " + cmd.ParamDescription;
                    }
                    else if(cmd.Name.Contains(szCommand))
                    {
                        matchingCommands.Add(cmd);
                    }
                }
            }
            else
            {
                _CommandLocationHelpText.enabled = false;
            }

            // enable/disable the suggestion view based on the commands we found
            int nTotalCount = Mathf.Min(matchingCommands.Count, _MaxSuggestions);            
            if(nTotalCount > 0)
            {
                _SuggestionPanel.gameObject.SetActive(true);

                // wipe butts that are out of range
                while(_SuggestionPanel.childCount > nTotalCount)
                {
                    Transform child = _SuggestionPanel.GetChild(0);
                    child.SetParent(null);
                    Destroy(child.gameObject);
                }

                // add butts that we will need to reach max
                while(_SuggestionPanel.childCount < nTotalCount)
                {
                    SuggestionButton newButt = Instantiate(_SuggestionButtonPrefab);
                    newButt.OnButtSlapped += ButtSlappedHandler;
                    Transform newChild = newButt.transform;
                    newChild.SetParent(_SuggestionPanel);
                }

                // handle each butt
                for(int nButtIndex = 0; nButtIndex < nTotalCount; ++nButtIndex)
                {
                    SuggestionButton childButt = _SuggestionPanel.GetChild(nButtIndex).GetComponent<SuggestionButton>();
                    childButt.Text = matchingCommands[nButtIndex].Name;
                }

                // set the help to be the first 
                Command first = matchingCommands[0];
                if(first.Name.StartsWith(szCommand))
                {
                    _CommandLocationHelpText.text = first.Name + " " + matchingCommands[0].ParamDescription;
                }
                else
                {
                    _CommandLocationHelpText.enabled = false;
                }
            }
            else
            {
                _SuggestionPanel.gameObject.SetActive(false);
            }
        }

        private void ButtSlappedHandler(SuggestionButton butt)
        {
            _CommandLocation.text = butt.Text + " ";
            _CommandLocation.Select();
            _CommandLocation.ForceLabelUpdate();

            // because ForceLabelUpdate() doesnt work
            StartCoroutine(MoveCaretToEndRoutine(_CommandLocation));
        }

        private IEnumerator MoveCaretToEndRoutine(InputField field)
        {
            yield return 0;
            _CommandLocation.MoveTextEnd(false);
        }
        
        private void ObtainAllCommands()
        {
            _Commands = new LinkedList<Command>();
            // Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies(); // NOTE: Iterate through this if we are looking through all assemblies in the project
            // foreach (Assembly ass in assemblies)
            Assembly ass = Assembly.GetExecutingAssembly();
            {
                Type[] types = ass.GetTypes();
                foreach (Type type in types)
                { 
                    if (type.IsSubclassOf(typeof(MonoBehaviour)))
                    {
                        // check for the RetroCommand attribute in each method
                        MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                        foreach (MethodInfo method in methods)
                        {
                            object[] atts = method.GetCustomAttributes(typeof(RetroCommand), false);
                            foreach (Attribute att in atts)
                            {
                                if (att is RetroCommand)
                                {
                                    RetroCommand cmd = att as RetroCommand;

                                    Command newCmd = new Command();
                                    newCmd.OwningBehaviorType = type;
                                    newCmd.Name = method.Name;
                                    newCmd.Description = cmd.Description;
                                    newCmd.ParamDescription = "";

                                    // get the parameter list
                                    ParameterInfo[] pars = method.GetParameters();
                                    newCmd.Parameters = new Type[pars.Length];
                                    foreach (ParameterInfo p in pars) 
                                    {
                                        // Console.WriteLine(p.ParameterType);
                                        newCmd.Parameters[p.Position] = p.ParameterType;
                                        newCmd.ParamDescription += "{" + p.ParameterType.Name + " : " + p.Name + "} ";
                                    }

                                    _Commands.AddLast(newCmd);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SubmitCommand()
        {
            bool bFoundCommand = false;
            if (_CommandLocation.text != "")
            {
                string szRawCommand = _CommandLocation.text;
                Debug.Log("cmd: " + szRawCommand);

                // split the raw into the command and params
                string[] split = szRawCommand.Split(' ');

                // find hte method
                foreach (Command cmd in _Commands)
                {
                    if (cmd.Name == split[0])
                    {
                        bFoundCommand = true;

                        // build params
                        object[] paramArray = new object[cmd.Parameters.Length];
                        for (int nParamIndex = 0; nParamIndex < cmd.Parameters.Length; ++nParamIndex)
                        {
                            Type paramType = cmd.Parameters[nParamIndex];
                            string szRawParam = split[nParamIndex + 1];

                            if (paramType == typeof(byte))
                            {
                                paramArray[nParamIndex] = Convert.ToByte(szRawParam);
                            }
                            if (paramType == typeof(sbyte))
                            {
                                paramArray[nParamIndex] = Convert.ToSByte(szRawParam);
                            }
                            if (paramType == typeof(short))
                            {
                                paramArray[nParamIndex] = Convert.ToInt16(szRawParam);
                            }
                            if (paramType == typeof(ushort))
                            {
                                paramArray[nParamIndex] = Convert.ToUInt16(szRawParam);
                            }
                            if (paramType == typeof(int))
                            {
                                paramArray[nParamIndex] = Convert.ToInt32(szRawParam);
                            }
                            if (paramType == typeof(uint))
                            {
                                paramArray[nParamIndex] = Convert.ToUInt32(szRawParam);
                            }
                            if (paramType == typeof(long))
                            {
                                paramArray[nParamIndex] = Convert.ToInt64(szRawParam);
                            }
                            if (paramType == typeof(ulong))
                            {
                                paramArray[nParamIndex] = Convert.ToUInt64(szRawParam);
                            }
                            if (paramType == typeof(float))
                            {
                                paramArray[nParamIndex] = Convert.ToSingle(szRawParam);
                            }
                            if (paramType == typeof(double))
                            {
                                paramArray[nParamIndex] = Convert.ToDouble(szRawParam);
                            }
                            if (paramType == typeof(decimal))
                            {
                                paramArray[nParamIndex] = Convert.ToDecimal(szRawParam);
                            }
                            if (paramType == typeof(char))
                            {
                                paramArray[nParamIndex] = szRawParam[0];
                            }
                            if (paramType == typeof(string))
                            {
                                paramArray[nParamIndex] = szRawParam;
                            }
                            if (paramType == typeof(bool))
                            {
                                paramArray[nParamIndex] = Convert.ToBoolean(szRawParam);
                            }
                        }

                        UnityEngine.Object[] objs = FindObjectsOfType(cmd.OwningBehaviorType);
                        foreach (UnityEngine.Object obj in objs)
                        {
                            cmd.OwningBehaviorType.InvokeMember(cmd.Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, obj, paramArray);
                        }
                    }
                }

                if (!bFoundCommand)
                {
                    Debug.LogWarning("Command \"" + split[0] + "\" not found!");
                    Debug.LogWarning("Type \"Help\" to view all available commands.");
                }
                _CommandLocation.text = "";
            }
            _ScrollZone.verticalScrollbar.value = 0.0f;
            _CommandLocation.Select();
        }

        private void RecieveLogCallback(string szLog, string szStack, LogType logType)
        {
            string szLogText = szLog;
            switch (logType)
            {

                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    szLogText = "<color=#ff0000ff>" + szLogText + "</color>";
                    break;
                case LogType.Warning:
                    szLogText = "<color=#ffff00ff>" + szLogText + "</color>";
                    break;
                case LogType.Log:
                    szLogText = "<color=#ffffffff>" + szLogText + "</color>";
                    break;
            }

            // add the log
            _ConsoleTextDestination.text += szLogText;
            _ConsoleTextDestination.text += '\n';

            if (_ConsoleTextDestination.text.Length > _MaxLogChars)
            {
                int nOverrunCharCount = _ConsoleTextDestination.text.Length - _MaxLogChars;
                int nOverrunNewLine = _ConsoleTextDestination.text.IndexOf("\n", nOverrunCharCount);

                _ConsoleTextDestination.text = _ConsoleTextDestination.text.Remove(0, nOverrunNewLine + 1);
            }

            /*
            // lock to bottom is needed
            if (bLockToZero)
            {
                _ScrollZone.SetLayoutVertical();
                _ScrollZone.verticalNormalizedPosition = 0.0f; // Mathf.Lerp(_ScrollZone.verticalNormalizedPosition, 0.0f, 0.5f);
                _ScrollZone.verticalScrollbar.value = 0.0f; //Mathf.Lerp(_ScrollZone.verticalScrollbar.value, 0.0f, 0.5f);
                bLockToZero = false;
            }
            */
        }
    }
}