using System;
using System.Collections;
using System.Collections.Generic;
using Retro.FSM;

public abstract class BaseFSMConstructor<T> : IFSMConstructor where T : IState
{
    public abstract string[] MachineHierarchy { get; }

    protected virtual Dictionary<string, StateInfo> GenerateMachine()
    {
        // start from top most machine, and replacing with each child state with matching state ID
        Dictionary<string, StateInfo> FinalMachine = new Dictionary<string, StateInfo>();
        foreach(string szMachineName in MachineHierarchy)
        {
            foreach (StateInfo stateInfo in GetStatesFromID<T>(szMachineName))
            {
                if(FinalMachine.ContainsKey(stateInfo.Attribute.StateID) == false)
                {
                    // add the new state.
                    // NOTE: This will generally only hit in the parent, although some child FSMs could have extra states
                    FinalMachine.Add(stateInfo.Attribute.StateID, stateInfo);
                }
                else
                {
                    // replace the existing state
                    FinalMachine[stateInfo.Attribute.StateID] = stateInfo;
                }
            }
        }
        return FinalMachine;
    }

    public override void Build()
    {
        Dictionary<string, StateInfo> FinalMachine = GenerateMachine();
        foreach (KeyValuePair<string, StateInfo> pair in FinalMachine)
        {
            RetroFSM.Chapter(ChapterHash).AddState(pair.Value);

            // check for default
            if (pair.Value.Attribute.IsDefault)
            {
                RetroFSM.Chapter(ChapterHash).SetDefaultState(pair.Value);
            }
        }

    }

    public override void Destroy()
    {
        Dictionary<string, StateInfo> FinalMachine = GenerateMachine();
        foreach (KeyValuePair<string, StateInfo> pair in FinalMachine)
        {
            RetroFSM.Chapter(ChapterHash).RemoveState(pair.Value);
        }
    }
}
