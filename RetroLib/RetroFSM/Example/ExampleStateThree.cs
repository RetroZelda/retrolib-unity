using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Retro.FSM;

[FSMState("ExampleFSM", "StateThree")]
public class ExampleStateThree : BaseExampleState
{
    public override void OnEnter(params object[] _PassthroughObjects)
    {
        base.OnEnter(_PassthroughObjects);

        float fTimeInStateTwo = (float)_PassthroughObjects[0];
        Log(string.Format("Time in state two: {0}", fTimeInStateTwo));
    }
    
    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FSM.ChangeState(typeof(ExampleStateOne));
        }
    }
}
