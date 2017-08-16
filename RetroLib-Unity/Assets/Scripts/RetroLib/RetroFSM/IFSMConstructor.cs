using System;
using System.Reflection;
using System.Collections.Generic;

namespace Retro.FSM
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FSMState : Attribute
    {
        public string MachineID { get; private set; }
        public string StateID { get; private set; }
        public bool IsDefault { get; private set; }
        
        public FSMState(string machineID, string stateID)
        {
            MachineID = machineID;
            StateID = stateID;
            IsDefault = false;
        }
        
        public FSMState(string machineID, string stateID, bool isDefault)
        {
            MachineID = machineID;
            StateID = stateID;
            IsDefault = isDefault;
        }
    }

    public class StateInfo
    {
        public FSMState Attribute;
        public Type StateType;

        public StateInfo(FSMState _att, Type _type)
        {
            Attribute = _att;
            StateType = _type;
        }
    }

    // note an interface anymore, so the name is wrong, but oh well...
    public abstract class IFSMConstructor
    {
        private string _szChapter;
        private int _nChapterHash;
        public virtual string Chapter { get { return _szChapter; } set { _szChapter = value; _nChapterHash = value.GetHashCode(); } }
        public virtual int ChapterHash { get { return _nChapterHash; } }

        public abstract void Build();
        public abstract void Destroy();

        public virtual void Init(params object[] initObjects)
        {
            Chapter = GetType().FullName;
        }

        protected static StateInfo[] GetStates<T>() where T : IState
        {
            return GetStates(typeof(T));
        }


        protected static StateInfo[] GetStatesFromID<T>(string szID) where T : IState
        {
            return GetStatesFromID(typeof(T), szID);
        }

        protected static StateInfo[] GetStates(Type type)
        {
            Type parent = type;
            List<StateInfo> types = new List<StateInfo>();
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (parent.IsAssignableFrom(t) && t != parent && t.IsAbstract == false)
                {
                    // we have the state, find the attribute
                    object[] atts = t.GetCustomAttributes(typeof(FSMState), false);
                    foreach (Attribute att in atts)
                    {
                        FSMState stateAttribute = att as FSMState;
                        if (stateAttribute != null)
                        {
                            // we found the attribute.  add the state to the list and break out of the attribute search
                            types.Add(new StateInfo(stateAttribute, t));
                            break;
                        }
                    }
                }
            }
            return types.ToArray();
        }
        
        protected static StateInfo[] GetStatesFromID(Type type, string ID)
        {
            Type parent = type;
            List<StateInfo> types = new List<StateInfo>();
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (parent.IsAssignableFrom(t) && t != parent && t.IsAbstract == false)
                {
                    // check for a set FSM ID
                    object[] atts = t.GetCustomAttributes(typeof(FSMState), false);
                    foreach (Attribute att in atts)
                    {
                        FSMState stateAttribute = att as FSMState;
                        if (stateAttribute != null && stateAttribute.MachineID == ID)
                        {
                            types.Add(new StateInfo(stateAttribute, t));
                            break;
                        }
                    }
                }
            }
            return types.ToArray();
        }

        protected static bool IsDefaultState<T>(Type type) where T : IState
        {
            return IsDefaultState(typeof(T), type);
        }

        protected static bool IsDefaultState(Type parent, Type type)
        {
            // ensure we are a child type of the base type
            if (parent != typeof(IState) && !parent.IsAssignableFrom(type))
                return false;

            // check for the default attribute
            object[] atts = type.GetCustomAttributes(typeof(FSMState), false); // NOTE: Maybe we DO want to inherit the default state
            foreach (Attribute att in atts)
            {
                FSMState stateAtt = att as FSMState;
                if (stateAtt != null && stateAtt.IsDefault)
                {
                    return true;
                }
            }
            return false;
        }
    }
}