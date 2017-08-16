using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Retro.FSM;

public class ExampleFSMConstructor : BaseFSMConstructor<BaseExampleState>
{
    public override string[] MachineHierarchy { get { return new string[] {"ExampleFSM"}; } }
    public override void Init(params object[] initObjects)
    {
        Chapter = "ExampleFSM";
    }
}
