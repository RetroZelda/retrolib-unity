using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Retro.Command;

public class TestBehavior : MonoBehaviour 
{
	[RetroCommand()]
	public void PublicTestCommand_NoDesc()
	{
		Debug.Log("PublicTestCommand");
	}

    [RetroCommand("A public test command")]
    public void PublicTestCommand()
    {
        Debug.Log("PublicTestCommand");
    }

    [RetroCommand("A private test command")]
    private void PrivateTestCommand()
    {
        Debug.Log("PrivateTestCommand");
    }

    [RetroCommand("A private test command with one param")]
    private void PrivateTestCommand_OneParam(int nNumber)
    {
        Debug.Log("PrivateTestCommand_OneParam - " + nNumber);
    }

    [RetroCommand("A private test command with two params")]
    private void PrivateTestCommand_TwoParam(int nNumber, float fValue)
    {
        Debug.Log("PrivateTestCommand_TwoParam - " + nNumber + " " + fValue);
    }

    [RetroCommand("A private test command with three params")]
    private void PrivateTestCommand_ThreeParam(int nNumber, float fValue, string szText)
    {
        Debug.Log("PrivateTestCommand_ThreeParam - " + nNumber + " " + fValue + " " + szText);
    }

    [RetroCommand("A private test command with four params")]
    private void PrivateTestCommand_FourParam(int nNumber, float fValue, string szText, bool bToggle)
    {
        Debug.Log("PrivateTestCommand_FourParam - " + nNumber + " " + fValue + " " + szText + " " + bToggle);
    }
}
