using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Retro.FSM
{
    public class FSMChapter 
    {
	    // exceptions
	    public class ChapterAlreadyRunningException : System.ApplicationException
	    {
                public ChapterAlreadyRunningException() {}
                public ChapterAlreadyRunningException(string message) {}
                public ChapterAlreadyRunningException(string message, System.Exception inner) {}
	    }
	    public class ChapterNotRunningException : System.ApplicationException
	    {
                public ChapterNotRunningException() {}
                public ChapterNotRunningException(string message) {}
                public ChapterNotRunningException(string message, System.Exception inner) {}
	    }
	    public class StartingChapterNotSetException : System.ApplicationException
	    {
                public StartingChapterNotSetException() {}
                public StartingChapterNotSetException(string message) {}
                public StartingChapterNotSetException(string message, System.Exception inner) {}
	    }
	    public class StateNotFoundException : System.ApplicationException
	    {
                public StateNotFoundException() {}
                public StateNotFoundException(string message) {}
                public StateNotFoundException(string message, System.Exception inner) {}
	    }
	
        private class ClassPassthrough
        {
            public StateInfo StateInfo;
            public StateInfo PreviousStateInfo;
            public object[] PassthroughObjects;

            public ClassPassthrough(StateInfo newState, StateInfo prevState, object[] pto)
            {
                StateInfo = newState;
                PreviousStateInfo = prevState;
                PassthroughObjects = pto;
            }
        }
	
	    // variables
	    private Dictionary<StateInfo, IState> _States;
	    private Stack<StateInfo> _StateStack;
	    private ClassPassthrough _NewPush;
	    // private Object[] _PassthroughObjects;
	    private int _PopCount;
	    private int _ID;	
	    private RunningState _RunningState;

	    public int getID() { return _ID; }
	    public void setID(int _ID) { this._ID = _ID; }

	    // Private methods
        private void ChapterCreate(int nID)
	    {
		    _States = new Dictionary<StateInfo, IState>();
            _StateStack = new Stack<StateInfo>();
		    _NewPush = null;
		    _PopCount = 0;
		    _ID = nID;
		    _RunningState = RunningState.Suspended;
	    }

		private StateInfo GetStateInfoByName(string szStateID)
		{
			foreach(StateInfo si in _States.Keys)
			{
				if(si.Attribute.StateID == szStateID)
				{
					return si;
				}
			}
			return null;
		}

		private StateInfo GetStateInfoByType(Type stateType)
		{
			foreach(StateInfo si in _States.Keys)
			{
				if(si.StateType == stateType)
				{
					return si;
				}
			}
			return null;
		}

	    private void PushState_Internal(ClassPassthrough toState)
	    {
			_StateStack.Push(toState.StateInfo);
			if(_RunningState == RunningState.Running)
			{
				// pause current top
				StateInfo curTop = PeekNext();
				if(curTop != null)
				{
					_States[curTop].OnPause();
				}

				// pass and clear the passthrough
				_States[toState.StateInfo].PreviousState = toState.PreviousStateInfo;
				_States[toState.StateInfo].OnEnter(toState.PassthroughObjects);
				_States[toState.StateInfo].OnResume();

			}
	    }

	    private void PopState_Internal(bool bExit)
	    {
		    StateInfo top = _StateStack.Pop();
		
		    // only exit when running
		    if(_RunningState == RunningState.Running)
		    {
			    _States[top].OnPause();
			    _States[top].OnExit();
			
			    // resume new top
			    if(Peek() != null)
			    {
				    _States[Peek()].OnResume();
			    }
		    }
	    }
	
	    private void FlushStates()
	    {
		    // flush all states, exiting only on the top
		    bool bFirst = true;
		    while(_StateStack.Count > 0)
		    {
			    PopState_Internal(bFirst);
			    bFirst = false;
		    }
	    }

	    private IState CreateState(StateInfo stateType)
	    {
			ConstructorInfo newConstructor = stateType.StateType.GetConstructor(new Type[] {});
			IState ret = (IState)newConstructor.Invoke(new object[] {});
			ret.StateInfo = stateType;

		    return ret;
	    }

        // public processing functions
        public void StartChapter(object[] initObjects)
	    {
		    // ensure we have a start
		    if(_NewPush == null)
		    {
			    throw new StartingChapterNotSetException("Default state not set for chapter " + _ID);
		    }
		
		    if(_RunningState == RunningState.Running)
		    {
			    throw new ChapterAlreadyRunningException("Chapter is already running!");
		    }
		
		    // create all states
	        foreach (IState state in _States.Values) 
	        {
	    	    state.OnCreate(initObjects);
            }
            
            _RunningState = RunningState.Running;
	    }

        public void StopChapter()
	    {
		    if(_RunningState == RunningState.Suspended)
		    {
			    throw new ChapterNotRunningException("Chapter isnt running!");
		    }
		    // exit all current states
		    FlushStates();
		
		    // destroy all states
	        foreach (IState state in _States.Values) 
	        {
	    	    state.OnDestroy();
	        }
		
	        _RunningState = RunningState.Suspended;
	    }

	    public void PauseChapter()
        {
		    if(_RunningState == RunningState.Suspended)
		    {
			    throw new ChapterNotRunningException("Chapter isnt running!");
		    }
		    // pause all states
	        foreach (IState state in _States.Values)
	        {
	    	    state.OnPause();
	        }
	        _RunningState = RunningState.Paused;
        }
	
	    public void ResumeChapter()
        {
		    if(_RunningState == RunningState.Suspended)
		    {
                throw new ChapterNotRunningException("Chapter isnt running or paused!");
		    }
		    // resume all states
	        foreach (IState state in _States.Values)
	        {
	    	    state.OnResume();
	        }
		    _RunningState = RunningState.Running;
        }

        public void PriorityUpdate()
        {
            // remove all pops 
            bool bFirst = true;
            while (_PopCount > 0)
            {
                PopState_Internal(bFirst);
                _PopCount--;
                bFirst = false;
            }

		    // push the new
		    if(_NewPush != null)
		    {
                // doing this in case we push a new in enter/resume
				ClassPassthrough toState = _NewPush;
                _NewPush = null;
                PushState_Internal(toState);
		    }
		
		    StateInfo topState = Peek();
		    if(topState != null)
		    {
			    _States[topState].PriorityUpdate();		
		    }
	    }

        public void Update()
	    {
		    StateInfo topState = Peek();
		    if(topState != null)
		    {
			    _States[topState].Update();		
		    }
	    }

        public void LateUpdate()
	    {		
		    StateInfo topState = Peek();
		    if(topState != null)
		    {
			    _States[topState].LateUpdate();		
		    }
		
	    }
	
	    private Object Invoke(Type State, MethodInfo method, params object[] parameters)
	    {
			StateInfo stateInfo = GetStateInfoByType(State);

			// TODO: throw exception
			if(stateInfo != null)
			{
				Object ret = method.Invoke(_States[stateInfo], parameters);
				return ret;
			}
			throw new StateNotFoundException(string.Format("Cannot call {0} on state {1}.  State not found!", State.ToString(), method.ToString()));
	    }

        public Object Call(String szMethod, params object[] parameters)
	    {		
		    StateInfo topState = Peek();
		    if(topState != null)
		    {
			    // get type array
			    Type[] classTypes = new Type[parameters.Length];
			    int nCount = 0;
			    foreach(object obj in parameters)
			    {
				    classTypes[nCount++] = obj.GetType();
			    }
			
			    // get the method
			    MethodInfo method = topState.StateType.GetMethod(szMethod, classTypes.Length > 0 ? classTypes : null);
			 
			     // invoke it
			    return Invoke(topState.StateType, method, parameters);
		    }
		    return null;
	    }

        public Object[] CallQueue(String szMethod, params object[] parameters)
	    {		
		    if(_StateStack.Count > 0)
		    {
			    // get type array
			    Type[] classTypes = new Type[parameters.Length];
			    int nCount = 0;
			    foreach(object obj in parameters)
			    {
				    classTypes[nCount++] = obj.GetType();
			    }
			
			    // get the method
			    MethodInfo method = Peek().StateType.GetMethod(szMethod, classTypes);
	
			     // invoke it on all objects
			    nCount = 0;
			    Object[] ret = new Object[_StateStack.Count];
			    foreach(StateInfo topState in _StateStack)
			    {
				    ret[nCount] = Invoke(topState.StateType, method, parameters);
			    }
			    return ret;
		    }
		    return null;
	    }
	
	    // public methods
        public FSMChapter(String szID)
	    {
		    ChapterCreate(szID.GetHashCode());
	    }

        public FSMChapter(int nID)
	    {
		    ChapterCreate(nID);
	    }
	
	    public void AddState(StateInfo stateType)
	    {
		    // ensure we don't add while running
		    if(_RunningState == RunningState.Running)
		    {
			    throw new ChapterAlreadyRunningException("Chapter \"" + _ID + "\" is already running.  Use PushState() or ChangeState() instead.");
		    }
		    _States.Add(stateType, CreateState(stateType));
	    }
	
	    public void RemoveState(StateInfo stateType)
	    {
		    // ensure we don't remove while running
		    if(_RunningState == RunningState.Running)
		    {
			    throw new ChapterAlreadyRunningException("Chapter \"" + _ID + "\" is already running.  Use PushState() or ChangeState() instead.");
		    }
		    _States.Remove(stateType);
	    }
	
	    public void SetDefaultState(StateInfo stateType)
	    {
		    // ensure we don't set while running
		    if(_RunningState == RunningState.Running)
		    {
			    throw new ChapterAlreadyRunningException("Chapter \"" + _ID + "\" is already running.  Use PushState() or ChangeState() instead.");
		    }
		    _NewPush = new ClassPassthrough(stateType, null, null);
	    }

	    public StateInfo Peek()
	    {
            if (_StateStack.Count == 0)
                return null;

		    StateInfo ret = _StateStack.Peek();
		    return ret;
	    }
	
	    public StateInfo PeekNext()
	    {
		    int nCount = 0;
		    foreach(StateInfo ret in _StateStack)
		    {
			    if(nCount == 1)
			    {
				    return ret;
			    }
			    nCount++;
		    }
		    return null;
	    }
	
	    // TP
	    public void PushState(Type stateType, params object[] passThrough)
        {
			StateInfo stateInfo = GetStateInfoByType(stateType);
			if(stateInfo != null)
			{
            	_NewPush = new ClassPassthrough(stateInfo, Peek(), passThrough);
			}
			else
			{
				throw new StateNotFoundException(string.Format("Cannot push state {0}.  State not found!", stateType.ToString()));
			}
        }

	    public void PushState(string stateID, params object[] passThrough)
        {
			StateInfo stateInfo = GetStateInfoByName(stateID);
			if(stateInfo != null)
			{
            	_NewPush = new ClassPassthrough(stateInfo, Peek(), passThrough);
			}
			else
			{
				throw new StateNotFoundException(string.Format("Cannot push state {0}.  State not found!", stateID));
			}
        }
	
	    public void PopState()
	    {
		    _PopCount++;
	    }
	
	    public void ChangeState(Type stateType, params object[] passThrough)
	    {			
			StateInfo stateInfo = GetStateInfoByType(stateType);
			if(stateInfo != null)
			{
		   		_PopCount = _StateStack.Count;
            	_NewPush = new ClassPassthrough(stateInfo, Peek(), passThrough);
			}
			else
			{
				throw new StateNotFoundException(string.Format("Cannot change to state {0}.  State not found!", stateType.ToString()));
			}
        }
	
	    public void ChangeState(string stateID, params object[] passThrough)
	    {			
			StateInfo stateInfo = GetStateInfoByName(stateID);
			if(stateInfo != null)
			{
		   		_PopCount = _StateStack.Count;
            	_NewPush = new ClassPassthrough(stateInfo, Peek(), passThrough);
			}
			else
			{
				throw new StateNotFoundException(string.Format("Cannot change to state {0}.  State not found!", stateID));
			}
        }
    }
}