using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Retro.FSM;
using Retro.Command;

public class ExampleRunner : MonoBehaviour 
{

	private FSMChapter _FSMOverride;
	private string _FSMID = "FSMOverride"; // "FSMOverride" can be set to any ID we want
	
	void Start () 
	{
		// We can start an FSM through the FSMBehavior by giving it a list of the FSMConstructors we created.  
		// This will start the FSM when the game starts(useful for gameplay states e.g. splashscreen, main menu, gameplay, pause, etc)
		// OR we can manually start a new FSM through this function.  This is helpful for when you want a bunch of gameobjects to share an FSM(e.g AI)
		RetroFSM.StartNewChapter<ExampleFSMOverrideConstructor>(_FSMID);
		
		// this is how we can get a handle to an FSM so we can change states
		_FSMOverride = RetroFSM.Chapter(_FSMID);
	}

	void OnDestroy()
	{
		// this is how we stop an FSM
		RetroFSM.StopChapter<ExampleFSMOverrideConstructor>(_FSMID);
	}

	// This is an example of creating a console command using RetroConsole
	// It is designed to work similarly to UE4's UFUNCTION(exec)
	[RetroCommand("This is an example of using the RetroConsole to define a function as a command.")]
	private void ChangeFSMState(string szStateName)
	{
		_FSMOverride.ChangeState(szStateName);
	}
	
}
