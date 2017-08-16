using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Retro.FSM;

// FSMState Attribute set on class to tell what FSM this is, what state this is, and if this is the default state for the FSM(Blank means false)
[FSMState("ExampleFSM", "StateThree")]
public class ExampleStateThree : BaseExampleState
{
    public override void OnEnter(params object[] _PassthroughObjects)
    {
        base.OnEnter(_PassthroughObjects);

        // retrieve state objects that are passed in
        float fTimeInStateTwo = (float)_PassthroughObjects[0];
        Log(string.Format("Time in state two: {0}", fTimeInStateTwo));
    }
    
    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FSM.ChangeState(typeof(ExampleStateOne)); // can change state based on type
        }
    }
}