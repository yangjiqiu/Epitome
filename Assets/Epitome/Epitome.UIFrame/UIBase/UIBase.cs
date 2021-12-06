using System.Collections;
using UnityEngine;

namespace Epitome.UIFrame
{
    public abstract class UIBase : EpitomeBehaviour
    {
        public event StateChangedEvent StateChanged;

        protected ObjectState state = ObjectState.None;

        public ObjectState State
        {
            protected set
            {
                if (value != state)
                {
                    ObjectState oldState = state;
                    state = value;
                    if (StateChanged != null)
                    {
                        StateChanged(this,state,oldState);
                    }
                }
            }

            get { return this.state; }
        }

        public abstract string GetUIType();

        protected virtual void SetDepthToTop() { }

        protected override void OnAwake()
        {
            this.State = ObjectState.Initial;
        }

        protected override void OnStart() { }

        protected override void OnUpdate()
        {
            if (this.state == ObjectState.Ready)
            {
                OnUpdate(Time.deltaTime);
            }
        }

        protected virtual void OnUpdate(float deltaTime) { }

        public void Release()
        {
            this.State = ObjectState.Closing;
            GameObject.Destroy(this.gameObject);
            OnRelease();
        }

        protected virtual void OnRelease() { }

        protected virtual void SetUI(params object[] uiParams)
        {
            this.State = ObjectState.Loading;
        }

        public void SetUIWhenOpening(params object[] uiParams)
        {
            SetUI(uiParams);
            new Task(AsyncOnLoadData());
        }

        public virtual void SetUIParams(params object[] uiParams) { }

        protected virtual void OnLoadData() { }

        private IEnumerator AsyncOnLoadData()
        {
            yield return new WaitForSeconds(0);
            if (this.State == ObjectState.Loading)
            {
                this.OnLoadData();
                this.State = ObjectState.Ready;
            }
        }

        #region UI state event

        public virtual void Display() {  }

        public virtual void Freeze() { }

        public virtual void Redisplay() { }

        public virtual void Hiding() { this.State = ObjectState.Disabled; }

        #endregion
    }
}