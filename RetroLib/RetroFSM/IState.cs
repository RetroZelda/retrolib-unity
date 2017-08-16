
namespace Retro.FSM
{
    public abstract class IState
    {
		public StateInfo StateInfo { get; set; }
		public StateInfo PreviousState { get; set; }

        public abstract void OnCreate(params object[] _InitObjects);
	    public abstract void OnEnter(params object[] _PassthroughObjects);
	    public abstract void PriorityUpdate();
	    public abstract void Update();
	    public abstract void LateUpdate();
	    public abstract void OnExit();
	    public abstract void OnDestroy();
	    public abstract void OnPause();
	    public abstract void OnResume();
    }
}