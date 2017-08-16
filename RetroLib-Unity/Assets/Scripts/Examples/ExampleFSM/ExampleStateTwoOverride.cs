using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Retro.FSM;

// FSMState Attribute set on class to tell what FSM this is, what state this is, and if this is the default state for the FSM(Blank means false)
// This particular state class is an override for StateThree.
[FSMState("ExampleFSMOverride", "StateTwo")]
public class ExampleStateTwoOverride : BaseExampleState
{
    public override void OnEnter(params object[] _PassthroughObjects)
    {
        base.OnEnter(_PassthroughObjects);
        Log(string.Format("We are in the state 2 override"));
    }
}