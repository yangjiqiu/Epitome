/*----------------------------------------------------------------
 * 文件名：WindowManager
 * 文件功能描述：窗口管理器
----------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Epitome.Manager.Window
{
    public class WindowManager
    {
        static WindowManager mInstance;

        public static WindowManager GetSingleton() { if (mInstance == null) { mInstance = new WindowManager(); } return mInstance; }

        private WindowManager()
        {
            //运行检查缓存的定时器
        }

        //=================    分    ======================

        private List<Window> mWindowList = new List<Window>();
        private List<Window> mWindowCache = new List<Window>();


        /// <summary>
        /// 打开窗口
        /// </summary>
        /// <param name="varName">窗口名字</param>
        /// <param name="intent"></param>
        public void OpenWindow(string varName, IIntent intent)
        {
            List<Window>.Enumerator tempEnumList = mWindowCache.GetEnumerator();
            Window tempOld = null;
            while (tempEnumList.MoveNext())
            {
                if (tempEnumList.Current.gameObject.name.Equals(varName))
                {
                    tempOld = tempEnumList.Current;
                }
            }
            if (tempOld != null)
            {
                mWindowCache.Remove(tempOld);
                mWindowList.Add(tempOld);
                //手动调用，表示重用
                tempOld.ReStart(intent);
            }
            else
            {
                //为了简单，所以这里就直接使用Resources加载了
                UnityEngine.Object obj = Resources.Load(varName);
                GameObject go = GameObject.Instantiate(obj) as GameObject;
                //通过配置，关联界面和Presenter
                System.Type tempType=null;
                //tempType = PresenterCfg.pconfig[varName];
                IPresenter p = System.Activator.CreateInstance(tempType) as IPresenter;
                Window w = go.AddComponent<Window>();
                w.AddPresenter(p);
                if (mWindowList.Count > 0)
                {
                    mWindowList[mWindowList.Count - 1].Hide();
                }
                mWindowList.Add(w);
                p.SetIntent(intent);
                p.BindView(go);
            }
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="varObject">窗口对象实例</param>
        public void CloseWindow(GameObject varObject)
        {
            int i = 0;
            for (i = 0; i < mWindowList.Count; ++i)
            {
                if (mWindowList[i].gameObject == varObject)
                {
                    //把当前最上面的窗口hide
                    mWindowList[mWindowList.Count - 1].Hide();
                    break;
                }
            }
            //没有找到相应的窗口
            if (i >= mWindowList.Count)
            {
                return;
            }
            for (int j = mWindowList.Count - 1; j >= i; --j)
            {
                mWindowList[j].OnStop();
                //缓存界面
                mWindowCache.Add(mWindowList[j]);
            }
            //弹出栈之后，需要销毁资源
            mWindowList.RemoveRange(i, mWindowList.Count);
            if (mWindowList.Count > 0)
            {
                mWindowList[mWindowList.Count - 1].Show();
            }
        }

        //检查并清理缓存
        private void _Examine()
        {
            if (mWindowCache.Count > 0)
            {
                //先进先出
                Window tempWindow = mWindowCache[0];
                mWindowCache.Remove(tempWindow);
                //释放资源
            }
        }
    }
}
