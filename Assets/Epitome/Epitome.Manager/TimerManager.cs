using System.Collections.Generic;
using UnityEngine;

namespace Epitome
{
    public delegate void FinishedHandler(bool manual);

    public class Timer
    {
        TimerManager.TimerState timer;

        public bool Running { get { return timer.Running; } }
        public bool Paused { get { return timer.Paused; } }

        public bool Loop
        {
            set { timer.HasRepeat = value; }
            get { return timer.HasRepeat; }
        }

        public event FinishedHandler Finished;

        public Timer(float time, TimeUnit timeUnit,bool ignoreTimeScale = false, bool autoStart = true)
        {
            timer = TimerManager.Instance.CreateTimer(time, timeUnit, ignoreTimeScale);
            timer.Finished += TimerFinished;
            if (autoStart) timer.Start();
        }

        public void Start() { timer.Start(); }

        public void Stop() { timer.Stop(); }

        public void Pause() { timer.Pause(); }

        public void UnPause() { timer.UnPause(); }

        public void TimerFinished(bool manual)
        {
            FinishedHandler handler = Finished;
            if (handler != null)
                handler(manual);
        }
    }

    public enum TimeUnit
    {
        FrameRate, // 帧率
        Second, // 秒
        CentiSecond, // 厘秒：是一秒的百分之一（0.01秒）
        MilliSecond, // 毫秒：是一秒的千分之一（0.001秒）
    }

    public class TimerManager : MonoSingleton<TimerManager>
    {
        public class TimerState
        {
            bool running;
            bool paused;
            bool stopped;

            public bool Running { get { return running; } }

            public bool Paused { get { return paused; } }

            public event FinishedHandler Finished;

            private TimeUnit timeUnit;

            private float delayTime; // 延迟时间
            private float attackTime; // 启动时间
            private  float currentTime; // 当前时间

            public bool HasRepeat; // 一直重复

            public bool ignoreTimeScale { get; private set; } // 忽略时间缩放

            public TimerState(float time, TimeUnit unit, bool ignore)
            {
                timeUnit = unit;
                ignoreTimeScale = ignore;

                delayTime = time;

                ResetState();
            }

            private void ResetState()
            {
                switch (timeUnit)
                {
                    case TimeUnit.FrameRate:
                        currentTime = 0.0f;
                        break;
                    case TimeUnit.Second:
                    case TimeUnit.CentiSecond:
                    case TimeUnit.MilliSecond:
                        if (!ignoreTimeScale) currentTime = 0.0f;
                        else currentTime = Time.realtimeSinceStartup;
                        break;
                }

                attackTime = delayTime + currentTime;
            }

            public void UpdateTime(float time)
            {
                time = ignoreTimeScale ? time - currentTime : time;

                if (running)
                {
                    if (paused) return;

                    switch (timeUnit)
                    {
                        case TimeUnit.FrameRate:
                            currentTime += 1;
                            break;
                        case TimeUnit.Second:
                            currentTime += time;
                            break;
                        case TimeUnit.CentiSecond:
                            currentTime += time * 100;
                            break;
                        case TimeUnit.MilliSecond:
                            currentTime += time * 1000;
                            break;
                    }

                    if (currentTime >= attackTime)
                    {
                        if (HasRepeat)
                        {
                            ResetState();
                        }
                        else
                        {
                            Stop();
                        }

                        FinishedHandler handle = Finished;
                        if (handle != null)
                        {
                            handle(stopped);
                        }
                    }
                }
            }

            public void Start()
            {
                running = true;
            }

            public void Stop()
            {
                stopped = true;
                running = false;
            }

            public void Pause()
            {
                paused = true;
            }

            public void UnPause()
            {
                paused = false;
            }
        }

        private List<TimerState> timerList = new List<TimerState>();

        private void Update()
        {
            for (int i = 0; i < timerList.Count ; i++)
            {
                timerList[i].UpdateTime(timerList[i].ignoreTimeScale ? Time.realtimeSinceStartup : Time.deltaTime);
            }
        }

        public TimerState CreateTimer(float time, TimeUnit timeUnit,bool ignoreTimeScale)
        {
            TimerState newTimer = new TimerState(time, timeUnit, ignoreTimeScale);
            timerList.Add(newTimer);
            return newTimer;
        }

        public void ClearTimer() { }
        public void ClearAllTimer() { }
    }
}


