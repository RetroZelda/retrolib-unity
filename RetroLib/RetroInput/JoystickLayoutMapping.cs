using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Retro.Types;

namespace Retro.Input
{
	[System.Serializable]
	public enum BindingType {Bind_Key, Bind_Axis};

	[System.Serializable]
	public abstract class BaseBind
	{
		public abstract BindingType BindType {get;}
	}

	[System.Serializable]
	public class KeyBind : BaseBind
	{
		public override BindingType BindType { get { return BindingType.Bind_Key; } }

        [SerializeField]
        private int _nKeyID;
        public int KeyID 
        {
            get { return _nKeyID; } 

            #if UNITY_EDITOR
            set { _nKeyID = value; } 
            #endif

        }

        [SerializeField]
        private int _nKeyID_Win;
        public int KeyID_Win 
        {
            get { return _nKeyID_Win; } 

            #if UNITY_EDITOR
            set { _nKeyID_Win = value; } 
            #endif

        }


        public int PlatformKeyID
        {
            #if UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
            get { return _nKeyID;}
            #else
            get { return _nKeyID_Win;}
            #endif
        }

        public KeyBind()
        {
            _nKeyID = _nKeyID_Win = -1;
        }

        public KeyBind(int nKeyID)
        {
            _nKeyID = _nKeyID_Win = nKeyID;
        }
	}

	[System.Serializable]
	public class AxisBind : BaseBind
	{
		public override BindingType BindType { get { return BindingType.Bind_Axis; } }

        [SerializeField]
        private string _szAxisID;
        public string AxisID 
        {
            get { return _szAxisID; } 

            #if UNITY_EDITOR
            set { _szAxisID = value; } 
            #endif
        }

        [SerializeField]
        private string _szAxisID_Win;
        public string AxisID_Win
        {
            get { return _szAxisID_Win; } 

            #if UNITY_EDITOR
            set { _szAxisID_Win = value; } 
            #endif
        }


        public string PlatformAxisID
        {
            #if UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
            get { return _szAxisID;}
            #else
            get { return _szAxisID_Win;}
            #endif
        }
            
        public AxisBind()
        {
            _szAxisID =_szAxisID_Win = "";
        }

		public AxisBind(string szAxisID)
		{
            _szAxisID =_szAxisID_Win = szAxisID;
		}
	}

    [System.Serializable]
    public class KeyBindDictionary : SerializableDictionary<string, KeyBind> {}

    [System.Serializable]
    public class AxisBindDictionary : SerializableDictionary<string, AxisBind> {}

    [System.Serializable]
	public class JoystickLayoutMapping : ScriptableObject 
	{
		[SerializeField]
		private KeyBind 	_LeftFaceButton;

		[SerializeField]  
		private KeyBind 	_RightFaceButton;

		[SerializeField]  
		private KeyBind 	_TopFaceButton;

		[SerializeField]
		private KeyBind 	_BottomFaceButton;

		[SerializeField] 
		private AxisBind 	_DirectionAxisX;

		[SerializeField]
		private AxisBind 	_DirectionAxisY;

		[SerializeField]
		private KeyBind 	_LeftShoulderButton;

		[SerializeField]
		private KeyBind 	_RightShoulderButton;

		[SerializeField]
		private KeyBind 	_StartButton;

		[SerializeField]
		private KeyBind 	_SelectButton;

		[SerializeField]
		private KeyBind 	_HomeButton;

		[SerializeField]
		private AxisBind 	_LeftAnalogAxisX;

		[SerializeField]
		private AxisBind 	_LeftAnalogAxisY;

		[SerializeField]
		private KeyBind 	_LeftAnalogButton;

		[SerializeField]
		private AxisBind 	_RightAnalogAxisX;

		[SerializeField]
		private AxisBind 	_RightAnalogAxisY;

		[SerializeField]
		private KeyBind 	_RightAnalogButton;

		[SerializeField]
		private AxisBind 	_LeftTriggerAxis;

		[SerializeField]
		private AxisBind 	_RightTriggerAxis;

		[SerializeField]
        private KeyBindDictionary  _CustomKeyBind = new KeyBindDictionary();

		[SerializeField]
        private AxisBindDictionary _CustomAxisBind = new AxisBindDictionary();

        #if UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
        public int 	    LeftFaceButton 		{ get { return _LeftFaceButton.KeyID;		} }
        public int 	    RightFaceButton 	{ get { return _RightFaceButton.KeyID;		} }
        public int 	    TopFaceButton 		{ get { return _TopFaceButton.KeyID;		} }
        public int 	    BottomFaceButton 	{ get { return _BottomFaceButton.KeyID;		} }
        public string	DirectionAxisX 		{ get { return _DirectionAxisX.AxisID;	    } }
        public string	DirectionAxisY 		{ get { return _DirectionAxisY.AxisID;	    } }
        public int 	    LeftShoulderButton 	{ get { return _LeftShoulderButton.KeyID;	} }
        public int 	    RightShoulderButton { get { return _RightShoulderButton.KeyID;	} }
        public int 	    StartButton 		{ get { return _StartButton.KeyID;			} }
        public int 	    SelectButton 		{ get { return _SelectButton.KeyID;			} }
        public int 	    HomeButton 			{ get { return _HomeButton.KeyID;    		} }
        public string   LeftAnalogAxisX 	{ get { return _LeftAnalogAxisX.AxisID;		} }
        public string   LeftAnalogAxisY 	{ get { return _LeftAnalogAxisY.AxisID;		} }
        public int 	    LeftAnalogButton 	{ get { return _LeftAnalogButton.KeyID;		} }
        public string   RightAnalogAxisX 	{ get { return _RightAnalogAxisX.AxisID;	} }
        public string   RightAnalogAxisY 	{ get { return _RightAnalogAxisY.AxisID;	} }
        public int 	    RightAnalogButton 	{ get { return _RightAnalogButton.KeyID;	} }
        public string   LeftTriggerAxis 	{ get { return _LeftTriggerAxis.AxisID;		} }
        public string   RightTriggerAxis 	{ get { return _RightTriggerAxis.AxisID;	} }
        #else
        public int      LeftFaceButton      { get { return _LeftFaceButton.KeyID_Win;       } }
        public int      RightFaceButton     { get { return _RightFaceButton.KeyID_Win;      } }
        public int      TopFaceButton       { get { return _TopFaceButton.KeyID_Win;        } }
        public int      BottomFaceButton    { get { return _BottomFaceButton.KeyID_Win;     } }
        public string   DirectionAxisX      { get { return _DirectionAxisX.AxisID_Win;      } }
        public string   DirectionAxisY      { get { return _DirectionAxisY.AxisID_Win;      } }
        public int      LeftShoulderButton  { get { return _LeftShoulderButton.KeyID_Win;   } }
        public int      RightShoulderButton { get { return _RightShoulderButton.KeyID_Win;  } }
        public int      StartButton         { get { return _StartButton.KeyID_Win;          } }
        public int      SelectButton        { get { return _SelectButton.KeyID_Win;         } }
        public int      HomeButton          { get { return _HomeButton.KeyID_Win;           } }
        public string   LeftAnalogAxisX     { get { return _LeftAnalogAxisX.AxisID_Win;     } }
        public string   LeftAnalogAxisY     { get { return _LeftAnalogAxisY.AxisID_Win;     } }
        public int      LeftAnalogButton    { get { return _LeftAnalogButton.KeyID_Win;     } }
        public string   RightAnalogAxisX    { get { return _RightAnalogAxisX.AxisID_Win;    } }
        public string   RightAnalogAxisY    { get { return _RightAnalogAxisY.AxisID_Win;    } }
        public int      RightAnalogButton   { get { return _RightAnalogButton.KeyID_Win;    } }
        public string   LeftTriggerAxis     { get { return _LeftTriggerAxis.AxisID_Win;     } }
        public string   RightTriggerAxis    { get { return _RightTriggerAxis.AxisID_Win;    } }
        #endif

        public KeyBindDictionary CustomKeyBind
        {
            get { return _CustomKeyBind; }

            #if UNITY_EDITOR
            set { _CustomKeyBind = value; }
            #endif
        }

        public AxisBindDictionary CustomAxisBind
        {
            get { return _CustomAxisBind; }

            #if UNITY_EDITOR
            set { _CustomAxisBind = value; }
            #endif
        }

		public KeyBind GetCustomKeyBind(string szID)
		{
            return _CustomKeyBind.D[szID];
		}

		public AxisBind GetCustomAxisBind(string szID)
		{
            return _CustomAxisBind.D[szID];
		}
	}
}