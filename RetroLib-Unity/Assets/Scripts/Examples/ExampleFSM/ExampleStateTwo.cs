using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Retro.FSM;

// FSMState Attribute set on class to tell what FSM this is, what state this is, and if this is the default state for the FSM(Blank means false)
[FSMState("ExampleFSM", "StateTwo")]
public class ExampleStateTwo : BaseExampleState
{
    private float _TimeInState;

    public override void OnEnter(params object[] _PassthroughObjects)
    {
        base.OnEnter(_PassthroughObjects);
        _TimeInState = 0.0f;
    }

    // This update happens in all states before Update()
    public override void PriorityUpdate()
    {
        base.PriorityUpdate();
        _TimeInState += Time.deltaTime;
    }

    // This update happens in all states after Update()
    public override void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FSM.ChangeState("StateThree", _TimeInState); // can change state through the string ID.  Can pass any arguments the state will need
        }
    }
}