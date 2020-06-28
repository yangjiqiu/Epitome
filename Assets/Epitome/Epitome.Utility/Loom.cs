using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Epitome
{
    public class Loom : MonoSingleton<Loom>
    {
        private struct DelayedQueultem
        {
            public float time;
            public Action<object> action;
            public object param;
        }

        private struct NoDelayedQueultem
        {
            public Action<object> action;
            public object param;
        }

        private List<NoDelayedQueultem> _actions = new List<NoDelayedQueultem>();
        private List<NoDelayedQueultem> currentActions = new List<NoDelayedQueultem>();
        private List<DelayedQueultem> _delayeds = new List<DelayedQueultem>();
        private List<DelayedQueultem> currentDelayeds = new List<DelayedQueultem>();

        public static int maxThreads = 8;
        private static int numThreads;

        public static void QueueOnMainThread(Action<object> action)
        {
            QueueOnMainThread(action, null, 0f);
        }

        public static void QueueOnMainThread(Action<object> action,object param)
        {
            QueueOnMainThread(action, param, 0f);
        }

        public static void QueueOnMainThread(Action<object> vAction,object vParam, float vTime)
        {
            if (vTime != 0)
            {
                lock (Instance._delayeds)
                {
                    Instance._delayeds.Add(new DelayedQueultem { time = Time.time + vTime, action = vAction });
                }
            }
            else
            {
                lock (Instance._actions)
                {
                    Instance._actions.Add(new NoDelayedQueultem { action = vAction, param = vParam });
                }
            }
        }

        public static Thread RunAsync(Action action)
        {
            if (Loom.Instance == null) return null;

            while (numThreads >= maxThreads)
            {
                Thread.Sleep(1);
            }
            Interlocked.Increment(ref numThreads);
            ThreadPool.QueueUserWorkItem(RunAction, action);
            return null;
        }

        private static void RunAction(object action)
        {
            try
            {
                ((Action)action)();
            }
            catch { }
            finally { Interlocked.Decrement(ref numThreads); }
        }

        private void OnDisable()
        {
            
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            if (_actions.Count > 0)
            {
                lock (_actions)
                {
                    currentActions.Clear();
                    currentActions.AddRange(_actions);
                    _actions.Clear();
                }
                for (int i = 0; i < currentActions.Count; i++)
                {
                    currentActions[i].action(currentActions[i].param);
                }
            }
            if (_delayeds.Count > 0)
            {
                lock (_delayeds)
                {
                    currentDelayeds.Clear();
                    currentDelayeds.AddRange(_delayeds.Where(d => d.time <= Time.time));
                    for (int i = 0; i < currentDelayeds.Count; i++)
                    {
                        _delayeds.Remove(currentDelayeds[i]);
                    }
                }
                for (int i = 0; i < currentDelayeds.Count; i++)
                {
                    currentDelayeds[i].action(currentDelayeds[i].param);
                }
            }
        }
    }
}
