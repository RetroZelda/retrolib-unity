using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Retro.FSM;

// FSMState Attribute set on class to tell what FSM this is, what state this is, and if this is the default state for the FSM(Blank means false)
[FSMState("ExampleFSM", "StateThree")]
public class ExampleStateThreeOverride : BaseExampleState
{
    public override void OnEnter(params object[] _PassthroughObjects)
    {
        base.OnEnter(_PassthroughObjects);
        Log(string.Format("We are in the state 3 override"));
    }
}