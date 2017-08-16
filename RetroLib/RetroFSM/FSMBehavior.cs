using UnityEngine;
using System.Collections;
using Retro.FSM;
using System;
using System.Linq;

public class FSMBehavior : MonoBehaviour
{
    [SerializeField]
    private string[] _FSMConstructorTypes;

    // Use this for initialization
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Debug.Log("FSMBehavior - Awake");

        // TODO: add all initial constructors here
        foreach(string szType in _FSMConstructorTypes)
        {
            Type constructorType = Type.GetType(szType);
            if(constructorType != null)
            {
                if (constructorType.IsSubclassOf(typeof(IFSMConstructor)))
                {
                    RetroFSM.AddConstructor(constructorType);
                }
                else
                {
                    Debug.LogError(string.Format("Type \"{0}\" not derived IFSMConstructor type!", szType));
                }

            }
            else
            {
                Debug.LogError(string.Format("Constructor Type \"{0}\" not found!", szType));
            }
        }

        // build all constructors
        RetroFSM.Build();

        // Start Library here because OnStart() needs to happen before OnEnable, and Start() doesnt
        RetroFSM.StartLibrary();
    }

    void Start()
    {
        Debug.Log("FSMBehavior - Start");
    }

    void OnDestroy()
    {
        Debug.Log("FSMBehavior - OnDestroy");
        RetroFSM.StopLibrary();
    }


    void OnEnable()
    {
        Debug.Log("FSMBehavior - OnEnable");
        RetroFSM.ResumeLibrary();
    }

    void OnDisable()
    {
        Debug.Log("FSMBehavior - OnDisable");
        RetroFSM.PauseLibrary();
    }

    // Update is called once per frame
    void Update()
    {
        RetroFSM.PriorityUpdate();
        RetroFSM.Update();
    }

    void LateUpdate()
    {
        RetroFSM.LateUpdate();
    }
}
