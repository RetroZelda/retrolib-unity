using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Retro.FSM;

// FSMState Attribute set on class to tell what FSM this is, what state this is, and if this is the default state for the FSM
[FSMState("ExampleFSM", "StateOne", true)]
public class ExampleStateOne : BaseExampleState
{
    public override void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            FSM.ChangeState(typeof(ExampleStateTwo)); // changing state based on type
        }
    }
}
