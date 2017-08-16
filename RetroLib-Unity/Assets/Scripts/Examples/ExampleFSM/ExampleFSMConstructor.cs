using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Retro.FSM;

// The FSM Constructor tells the FSMLibrary how to build the FSM. 
// This is override BaseFSMConstructor<T> where T is the base state type for the FSM
public class ExampleFSMConstructor : BaseFSMConstructor<BaseExampleState>
{
    // the MachineHierarchy handles FSM heirarchys.
    // This allows you to have child FSM states on diifferent FSMs, but shares the state logic of its parent
    public override string[] MachineHierarchy { get { return new string[] {"ExampleFSM"}; } }
    
    // This would be an example of having an overide FSM
    // The order is important.  MachineHierarchy[0] is always the root; MachineHierarchy[size-1] is the leaf state
    // public override string[] MachineHierarchy { get { return new string[] {"ExampleFSM", "ExampleFSMOverride"}; } }



    public override void Init(params object[] initObjects)
    {
        // the Chapter is how we Identify this FSM
        Chapter = "ExampleFSM";
    }
}

// The FSM Constructor tells the FSMLibrary how to build the FSM
public class ExampleFSMOverrideConstructor : BaseFSMConstructor<BaseExampleState>
{
    // This would be an example of having an overide FSM
    // The order is important.  MachineHierarchy[0] is always the root; MachineHierarchy[size-1] is the leaf state
    public override string[] MachineHierarchy { get { return new string[] {"ExampleFSM", "ExampleFSMOverride"}; } }



    public override void Init(params object[] initObjects)
    {
        // the Chapter is how we Identify this FSM
        Chapter = "ExampleFSMOverride";
    }
}