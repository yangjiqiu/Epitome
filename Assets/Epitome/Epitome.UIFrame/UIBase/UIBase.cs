using System.Collections;
using UnityEngine;

namespace Epitome.UIFrame
{
    public abstract class UIBase : EpitomeBehaviour
    {
        private Transform cachedTransform;

        public Transform CachedTransform
        {
            get
            {
                if (!cachedTransform)
                    cachedTransform = this.transform;

                return cachedTransform;
            }
        }

        private GameObject cachedGameObject;

        public GameObject CachedGameObject
        {
            get
            {
                if (!cachedGameObject)
                    cachedGameObject = this.gameObject;

                return cachedGameObject;
            }
        }

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

        public event StateChangedEvent StateChanged;

        public abstract string GetUIType();

        protected virtual void SetDepthToTop() { }

        protected override void OnUpdate()
        {
            if (this.state == ObjectState.Ready)
            {
                OnUpdate(Time.deltaTime);
            }
        }

        public void Release()
        {
            this.State = ObjectState.Closing;
            GameObject.Destroy(cachedGameObject);
            OnRelease();
        }

        protected override void OnStart() { }

        protected override void OnAwake()
        {
            this.State = ObjectState.Initial;
            this.State = ObjectState.Loading;
        }

        protected void OnUpdate(float deltaTime) { }

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