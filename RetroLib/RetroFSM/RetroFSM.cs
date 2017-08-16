using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Retro.FSM
{
	// data types
	public enum RunningState {Suspended, Paused, Running}

    public class RetroFSM 
    {	
	    // exceptions
	    public class StateExistsException : System.ApplicationException
	    {
            public StateExistsException() {}
            public StateExistsException(string message) {}
            public StateExistsException(string message, System.Exception inner) {}
	    }
	    public class FSMLibraryAlreadyRunningException : System.ApplicationException
	    {
            public FSMLibraryAlreadyRunningException() {}
            public FSMLibraryAlreadyRunningException(string message) {}
            public FSMLibraryAlreadyRunningException(string message, System.Exception inner) {}
	    }
	    public class FSMLibraryNotRunningException : System.ApplicationException
	    {
            public FSMLibraryNotRunningException() {}
            public FSMLibraryNotRunningException(string message) {}
            public FSMLibraryNotRunningException(string message, System.Exception inner) {}
	    }
	
	    // data
	    private List<IFSMConstructor> _Constructors;
	    private Dictionary<int, FSMChapter> _FSMChapters;	
	    private RunningState _RunningState;

        private class ConstructorStartData
        {
            public Type ConstructorType;
            public String ChapterNameOverride;
            public Object[] Passthrough;
        }

        private class ConstructorEndData
        {
            public Type ConstructorType;
            public String ChapterNameOverride;
        }

        private List<ConstructorStartData> _ConstructorToStart;
        private List<ConstructorEndData> _ConstructorToStop;

	    // singleton
	    private static RetroFSM Instance = new RetroFSM();	
	
        private RetroFSM()
	    {
		    _Constructors = new List<IFSMConstructor>();
		    _FSMChapters = new Dictionary<int, FSMChapter>();
		    _RunningState = RunningState.Suspended;
            _ConstructorToStart = new List<ConstructorStartData>();
            _ConstructorToStop = new List<ConstructorEndData>();
	    }
	    /////////////////////////////////////////////////////////
	
	    // private functions
        private IFSMConstructor CreateConstructor(Type constructorType, object[] initObjects)
	    {
		    IFSMConstructor ret = null;

            // No parameters in the constructor
            ConstructorInfo constructor = constructorType.GetConstructor(new Type[] {});
			ret = (IFSMConstructor)constructor.Invoke(new object[] {});
            ret.Init(initObjects);
		    return ret;
	    }
	
	    private void StartNewChapter_Internal(Type constructorType, string szChapterNameOverride, params Object[] passThrough)
	    {
            ConstructorStartData data = new ConstructorStartData();
            data.ConstructorType = constructorType;
            data.Passthrough = passThrough;
            data.ChapterNameOverride = szChapterNameOverride;
            _ConstructorToStart.Add(data);
	    }
	
	    private void StopChapter_Internal(Type constructorType, string szChapterNameOverride)
        {
            ConstructorEndData data = new ConstructorEndData();
            data.ConstructorType = constructorType;
            data.ChapterNameOverride = szChapterNameOverride;
            _ConstructorToStop.Add(data);
	    }

        private void AddConstructor_Internal(string szChapterName, Type constructorType, object[] initObjects) 
	    {
		    if(_RunningState == RunningState.Running)
		    {
			    throw new FSMLibraryAlreadyRunningException("FSM Library is already running!");
		    }
		    if(_RunningState == RunningState.Paused)
		    {
			    throw new FSMLibraryNotRunningException("FSM Library is paused!");
		    }
            IFSMConstructor constructor = CreateConstructor(constructorType, initObjects);  
            if(!string.IsNullOrEmpty(szChapterName))
            {
                constructor.Chapter = szChapterName;
            }

		    if(_FSMChapters.ContainsKey(constructor.ChapterHash))
		    {
			    throw new StateExistsException("Constructor \"" + constructor.Chapter + "\" already exists!");
		    }
		
		    if(!_FSMChapters.ContainsKey(constructor.ChapterHash))
		    {
			    AddChapter(constructor.ChapterHash);
		    }
		    _Constructors.Add(constructor);
	    }
	
	    private void AddChapter(int nChapterHash)
	    {
		    _FSMChapters.Add(nChapterHash, new FSMChapter(nChapterHash));
	    }

	    private void Build_Internal()
	    {
		    for(int nConstructorIndex = 0; nConstructorIndex < _Constructors.Count; ++nConstructorIndex)
		    {
			    _Constructors[nConstructorIndex].Build();
		    }
		    _Constructors.Clear();
	    }
	
	    private FSMChapter GetChapter(int nID)
	    {
		    if(!_FSMChapters.ContainsKey(nID))
		    {
			    AddChapter(nID);
		    }
		    return _FSMChapters[nID];
	    }
	
	    // private processing functions
	    private void StartLibrary_Internal()
	    {
		    if(_RunningState == RunningState.Running)
		    {
			    throw new FSMLibraryAlreadyRunningException("FSM Library is already running!");
		    }
		    if(_RunningState == RunningState.Paused)
		    { 
			    throw new FSMLibraryNotRunningException("FSM Library is paused!");
		    }
		
		    foreach(FSMChapter chapter in _FSMChapters.Values)
		    {
                chapter.StartChapter(new object[] {chapter});
		    }

		    _RunningState = RunningState.Running;
	    }
	
	    private void PriorityUpdate_Internal() 
	    {
		    if(_RunningState == RunningState.Suspended)
		    {
			    throw new FSMLibraryNotRunningException("FSM Library isnt running!");
		    }
		    if(_RunningState == RunningState.Paused)
		    {
			    throw new FSMLibraryNotRunningException("FSM Library is paused!");
		    }

            // remove all stopped FSMs
            if (_ConstructorToStop.Count > 0)
            {
                foreach (ConstructorEndData ced in _ConstructorToStop)
                {
                    IFSMConstructor constructor = CreateConstructor(ced.ConstructorType, null);

                    if(!string.IsNullOrEmpty(ced.ChapterNameOverride))
                        constructor.Chapter = ced.ChapterNameOverride;
                    GetChapter(constructor.ChapterHash).StopChapter();					
                    constructor.Destroy();
					
					_FSMChapters.Remove(constructor.ChapterHash);
                }
                _ConstructorToStop.Clear();
            }

            // process all added FSMs
            if (_ConstructorToStart.Count > 0)
            {
                foreach (ConstructorStartData data in _ConstructorToStart)
                {
                    IFSMConstructor constructor = CreateConstructor(data.ConstructorType, data.Passthrough);
                    if(!string.IsNullOrEmpty(data.ChapterNameOverride))
                        constructor.Chapter = data.ChapterNameOverride;
                    constructor.Build();
                    
                    FSMChapter chapt = GetChapter(constructor.ChapterHash);
                    object[] finalParams;
                    if (data.Passthrough != null && data.Passthrough.Length > 0)
                    {
                        finalParams = new object[data.Passthrough.Length + 1];
                        Array.Copy(data.Passthrough, 0, finalParams, 1, data.Passthrough.Length);
                    }
                    else
                    {
                        finalParams = new object[]{ chapt };
                    }
                    chapt.StartChapter(finalParams);
                }
                _ConstructorToStart.Clear();
            }
		
		    foreach(FSMChapter chapter in _FSMChapters.Values)
		    {
			    chapter.PriorityUpdate();
		    }
	    }
	
	    private void Update_Internal()
	    {
		    if(_RunningState == RunningState.Suspended)
		    {
			    throw new FSMLibraryNotRunningException("FSM Library isnt running!");
		    }
		    if(_RunningState == RunningState.Paused)
		    {
			    throw new FSMLibraryNotRunningException("FSM Library is paused!");
		    }
		
		    foreach(FSMChapter chapter in _FSMChapters.Values)
		    {
			    chapter.Update();
		    }
	    }
	
	    private void LateUpdate_Internal()
	    {
		    if(_RunningState == RunningState.Suspended)
		    {
			    throw new FSMLibraryNotRunningException("FSM Library isnt running!");
		    }
		    if(_RunningState == RunningState.Paused)
		    {
			    throw new FSMLibraryNotRunningException("FSM Library is paused!");
		    }
		
		    foreach(FSMChapter chapter in _FSMChapters.Values)
		    {
			    chapter.LateUpdate();
		    }
	    }
	
	    private void StopLibrary_Internal() 
	    {
		    if(_RunningState == RunningState.Suspended)
		    {
			    throw new FSMLibraryNotRunningException("FSM Library isnt running!");
		    }
		
		    foreach(FSMChapter chapter in _FSMChapters.Values)
		    {
	            chapter.StopChapter();
		    }
		
		    _RunningState = RunningState.Suspended;
	    }

	    private void ResumeLibrary_Internal()
        {
		    if(_RunningState == RunningState.Suspended)
		    {
			    throw new FSMLibraryNotRunningException("FSM Library isnt running!");
		    }
		    if(_RunningState != RunningState.Running)
		    {
			    foreach(FSMChapter chapter in _FSMChapters.Values)
			    {
	                chapter.ResumeChapter();
			    }
			    _RunningState = RunningState.Running;
		    }
        }
	
	    private void PauseLibrary_Internal()
        {
		    if(_RunningState == RunningState.Suspended)
		    {
			    throw new FSMLibraryNotRunningException("FSM Library isnt running!");
		    }
		    if(_RunningState != RunningState.Paused)
		    {
			    foreach(FSMChapter chapter in _FSMChapters.Values)
			    {
	                chapter.PauseChapter();
			    }
			    _RunningState = RunningState.Paused;
		    }
        }
	
	    private Object Call_Internal(int nChapterHash, String szMethod, params object[] parameters)
	    {
		    if(_RunningState == RunningState.Suspended)
		    {
			    throw new FSMLibraryNotRunningException("FSM Library isnt running!");
		    }
		    if(_RunningState == RunningState.Paused)
		    {
			    throw new FSMLibraryNotRunningException("FSM Library is paused!");
		    }
		    return Chapter(nChapterHash).Call(szMethod, parameters);
	    }
	
	    private Object[] CallQueue_Internal(int nChapterHash, String szMethod, params object[] parameters)
	    {
		    if(_RunningState == RunningState.Suspended)
		    {
			    throw new FSMLibraryNotRunningException("FSM Library isnt running!");
		    }
		    if(_RunningState == RunningState.Paused)
		    {
			    throw new FSMLibraryNotRunningException("FSM Library is paused!");
		    }
		    return Chapter(nChapterHash).CallQueue(szMethod, parameters);
	    }

	    // public static bridge functions
	    public static FSMChapter Chapter(int nID)
	    {
		    return Instance.GetChapter(nID);
	    }
	
	    public static FSMChapter Chapter(String szID)
	    {
		    return Instance.GetChapter(szID.GetHashCode());
	    }

        // public static functions
        public static void StartNewChapter<T>(params object[] passThrough) where T : IFSMConstructor
        {
            Type constructorType = typeof(T);
            Instance.StartNewChapter_Internal(constructorType, "", passThrough);
        }

        public static void StartNewChapter<T>(string chapterName, params object[] passThrough) where T : IFSMConstructor
        {
            Type constructorType = typeof(T);
            Instance.StartNewChapter_Internal(constructorType, chapterName, passThrough);
        }
		
        public static void StartNewChapter(Type constructorType, params object[] passThrough)
        {
            Instance.StartNewChapter_Internal(constructorType, "", passThrough);
        }

        public static void StartNewChapter(Type constructorType, string chapterName, params object[] passThrough)
        {
            Instance.StartNewChapter_Internal(constructorType, chapterName, passThrough);
        }

        public static void StopChapter<T>() where T : IFSMConstructor
        {
            Type constructorType = typeof(T);
            Instance.StopChapter_Internal(constructorType, "");
        }

        public static void StopChapter<T>(string szChapterName) where T : IFSMConstructor
        {
            Type constructorType = typeof(T);
            Instance.StopChapter_Internal(constructorType, szChapterName);
        }

        public static void StopChapter(Type constructorType)
        {
            Instance.StopChapter_Internal(constructorType, "");
        }

        public static void StopChapter(Type constructorType, string szChapterName)
        {
            Instance.StopChapter_Internal(constructorType, szChapterName);
        }

        public static void AddConstructor<T>(params object[] initObjects) where T : IFSMConstructor
	    {
            Type constructorType = typeof(T);
            Instance.AddConstructor_Internal("", constructorType, initObjects);
	    }

        public static void AddConstructor(Type constructorType, params object[] initObjects)
        {
            Instance.AddConstructor_Internal("", constructorType, initObjects);
        }

        public static void AddConstructor<T>(string chapterName, params object[] initObjects) where T : IFSMConstructor
        {
            Type constructorType = typeof(T);
            Instance.AddConstructor_Internal(chapterName, constructorType, initObjects);
        }

        public static void AddConstructor(string chapterName, Type constructorType, params object[] initObjects)
        {
            Instance.AddConstructor_Internal(chapterName, constructorType, initObjects);
        }

        public static void Build()
	    {
		    Instance.Build_Internal();
	    }
	
	    // public static bridge processing functions
	    public static void StartLibrary()
	    {
		    Instance.StartLibrary_Internal();
	    }
	
	    public static void PriorityUpdate()
	    {
		    Instance.PriorityUpdate_Internal();
	    }
	
	    public static void Update()
	    {
		    Instance.Update_Internal();
	    }
	
	    public static void LateUpdate() 
	    {
		    Instance.LateUpdate_Internal();
	    }
	
	    public static void StopLibrary() 
	    {
		    Instance.StopLibrary_Internal();
	    }

	    public static Object Call(int nChapterHash, String szMethod, params object[] parameters) 
	    {
		    return Instance.Call_Internal(nChapterHash, szMethod, parameters);
	    }

        public static Object[] CallQueue(int nChapterHash, String szMethod, params object[] parameters) 
	    {
		    return Instance.CallQueue_Internal(nChapterHash, szMethod, parameters);
	    }

	    public static void PauseLibrary() 
        {
		    Instance.PauseLibrary_Internal();
        }

	    public static void ResumeLibrary() 
        {
		    Instance.ResumeLibrary_Internal();
        }

    }
}