using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Epitome.Manager.Window
{
    public abstract class Presenter<T> : IPresenter where T : IView
    {
        //protected FSM _fsm = null;
        protected IIntent _intent = null;
        protected T mView = default(T);
        public void SetIntent(IIntent intent) { this._intent = intent; }

        //每次压栈都会调用
        public abstract void OnEnter();

        //每次退栈都会调用
        public abstract void OnLeave();

        //在mono start和时调用
        public virtual void OnStart() { }
        public virtual void OnStop() { }
        public virtual void OnDestroy() { }

        public void BindView(GameObject view)
        {
            mView = System.Activator.CreateInstance<T>();
            mView.Init(view);
        }
    }
}