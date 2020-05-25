using System.Collections;
using UnityEngine;

namespace Epitome
{
    public class Task
    {
        TaskManager.TaskState task;

        public bool Running { get { return task.Running; } }

        public bool Paused { get { return task.Paused; } }

        public delegate void FinishedHandler(bool manual);

        public event FinishedHandler Finished;

        public Task(IEnumerator c,bool autoStart = true)
        {
            task = TaskManager.CreateTask(c);
            task.Finished += TaskFinished;
            if (autoStart)
            {
                task.Start();
            }
        }

        public void Start()
        {
            task.Start();
        }
 
        public void Stop()
        {
            task.Stop();
        }

        public void Pause()
        {
            task.Pause();
        }

        public void UnPause()
        {
            task.UnPause();
        }

        void TaskFinished(bool manual)
        {
            FinishedHandler handler = Finished;
            if (handler != null)
                handler(manual);
        }
    }

    public class TaskManager : MonoSingleton<TaskManager>
    {
        public class TaskState
        {
            bool running;
            bool paused;
            bool stopped;

            public bool Running { get { return running; } }

            public bool Paused { get { return paused; } }

            public delegate void FinishedHandler(bool manual);

            public event FinishedHandler Finished;

            IEnumerator coroutine;

            public TaskState(IEnumerator c)
            {
                coroutine = c;
            }

            public void Pause()
            {
                paused = true;
            }

            public void UnPause()
            {
                paused = false;
            }

            public void Start()
            {
                running = true;
                TaskManager.Instance.StartCoroutine(CallWrapper());
            }

            public void Stop()
            {
                stopped = true;
                running = false;
            }

            IEnumerator CallWrapper()
            {
                yield return null;

                IEnumerator e = coroutine;
                while (running)
                {
                    if (paused)
                    {
                        yield return null;
                    }
                    else
                    {
                        if (e != null && e.MoveNext())
                        {
                            yield return e.Current;
                        }
                        else
                        {
                            running = false;
                        }
                    }
                }

                FinishedHandler handle = Finished;
                if (handle != null)
                {
                    handle(stopped);
                }
            }
        }

        public static TaskState CreateTask(IEnumerator coroutine)
        {
            return new TaskState(coroutine);
        }
    }
}
