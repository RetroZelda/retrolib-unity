using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Retro.Log;
using Retro.FSM;

public class BaseExampleState : IState
{
    private FSMChapter _FSM = null;
    private RetroLogger _Logger = null;

    // An example of holding a reference to this FSM.  Used when changing states.
    protected FSMChapter FSM { get { return _FSM ?? RetroFSM.Chapter("ExampleFSM"); } }

    // An example of getting a Logging category through RetroLog
    private RetroLogger Logger { get { return _Logger ?? RetroLog.RetrieveCatagory(StateInfo.Attribute.MachineID); } }

    /// <summary>
    /// Called when the FSM is created
    /// </summary>
    /// <param name="_InitObjects">Every state in this FSM has the same init objects</param>
    public override void OnCreate(params object[] _InitObjects)
    {
        Log("OnCreate()");
    }

    /// <summary>
    /// Called when the FSM is destroyed
    /// </summary>
    public override void OnDestroy()
    {
        Log("OnDestroy()");
    }
    
    /// <summary>
    /// This is called when we enter a new state
    /// </summary>
    /// <param name="_PassthroughObjects">These objects are set when changing from the previous state.</param>
    public override void OnEnter(params object[] _PassthroughObjects)
    {
        Log("OnEnter()");
    }

    /// <summary>
    /// This is called when the state exits.
    /// </summary>
    public override void OnExit()
    {
        Log("OnExit()");
    }

    /// <summary>
    /// This gets called before OnExit() if exiting, and before OnEnter() if pushing to a new state
    /// </summary>
    public override void OnPause()
    {
        Log("OnPause()");
    }

    /// <summary>
    /// This gets called after OnEnter() when entering this state, and after calling OnExit() when poping from a state.
    /// </summary>
    public override void OnResume()
    {
        Log("OnResume()");
    }

    /// <summary>
    /// This gets called first for every state every engine tick.
    /// </summary>
    public override void PriorityUpdate()
    {

    }

    /// <summary>
    /// This gets called for every state after PriorityUpdate() has been called on every state
    /// </summary>
    public override void Update()
    {

    }

    /// <summary>
    /// This gets called for every state durring the LateUpdate() tick
    /// </summary>
    public override void LateUpdate()
    {

    }

    /// <summary>
    /// This is an example to show logging using the RetroLog.
    /// </summary>
    /// <param name="szText"></param>
    protected void Log(string szText)
    {
        Logger.Log(string.Format("{0}:{1} - {2}", StateInfo.Attribute.MachineID, GetType().Name, szText));
    }
}