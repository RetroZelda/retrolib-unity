using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Retro.FSM;

[FSMState("ExampleFSM", "StateOne", true)]
public class ExampleStateOne : BaseExampleState
{
    public override void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            FSM.ChangeState(typeof(ExampleStateTwo));
        }
    }
}
