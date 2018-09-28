using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Epitome.Manager.Window
{

    public interface IPresenter
    {
        void OnStart();
        void OnEnter();
        void OnLeave();
        void OnStop();
        void OnDestroy();
        void BindView(GameObject go); //这就是绑定gameObject到逻辑
        void SetIntent(IIntent intent); //传递界面参数
    }

    public interface IView
    {
        void Init(GameObject view); //在Presenter中会把传递的界面gameObject绑定到View上，Presenter持有View的引用，而不直接持有gameObject
    }

    public interface IIntent { }


    public class Window : MonoBehaviour
    {
        private IPresenter mPresenter = null;
        private bool _isStart = false;
        void Start()
        {

            _isStart = true;
            gameObject.layer = (int)UnityLayer.ShowUILayer;
            mPresenter.OnStart();
            this.Show();
        }
        void OnDestroy()
        {
            mPresenter.OnDestroy();
        }
        public void AddPresenter(IPresenter presenter)
        {
            this.mPresenter = presenter;
        }
        public void Show()
        {
            if (_isStart)
            {
                mPresenter.OnEnter();
            }
        }

        public void Hide()
        {
            mPresenter.OnLeave();
        }

        public void OnStop()
        {
            mPresenter.OnStop();
        }

        //重用界面时调用
        public void ReStart(IIntent intent)
        {
            mPresenter.SetIntent(intent);
            mPresenter.OnStart();
            this.Show();
        }
    }
}
