namespace Epitome
{
    public class ModuleBase
    {
        public ModuleBase() { }

        public enum RegisterType
        {
            NotRegister,

            AutoRegister,

            AlreadyRegister,
        }

        private ObjectState state = ObjectState.Initial;

        public ObjectState State
        {
            get { return state; }

            set
            {
                if (state == value) return;

                ObjectState oldState = state;
                state = value;

                if (StateChanged != null)
                {
                    StateChanged(this, state, oldState);
                }

                OnStateChanged(state, oldState);
            }
        }

        public event StateChangedEvent StateChanged;

        protected virtual void OnStateChanged(ObjectState newState, ObjectState oldState) { }

        private RegisterType registerType = RegisterType.NotRegister;

        public bool AutoRegister
        {
            get { return registerType == RegisterType.NotRegister ? false : true; }

            set
            {
                if (registerType == RegisterType.NotRegister || registerType == RegisterType.AutoRegister)
                {
                    registerType = value ? RegisterType.AutoRegister : RegisterType.NotRegister;
                }
            }
        }

        public bool HasRegistered
        {
            get { return registerType == RegisterType.AlreadyRegister; }
        }

        public void Load()
        {
            if (State != ObjectState.Initial) return;

            if (registerType == RegisterType.AutoRegister)
            {
                ModuleManager.Instance.Register(this);
                registerType = RegisterType.AlreadyRegister;
            }

            OnLoad();
            State = ObjectState.Ready;
        }

        protected virtual void OnLoad() { }

        public void Release()
        {
            if (State != ObjectState.Disabled)
            {
                State = ObjectState.Disabled;

                if (registerType == RegisterType.AlreadyRegister)
                {
                    ModuleManager.Instance.UnRegister(this);
                    registerType = RegisterType.AutoRegister;
                }
            }

            OnRelease();
        }

        protected virtual void OnRelease() { }
    }
}
