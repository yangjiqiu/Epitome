using System;
using System.Collections;
using UnityEngine;

namespace Epitome
{
    public class EpitomeBehaviour : MonoBehaviour
    {
        public void Invoke(Action action, float time)
        {
            Invoke(action.Method.Name, time);
        }

        public void InvokeRepeating(Action action, float time, float repeatRate)
        {
            InvokeRepeating(action.Method.Name, time, repeatRate);
        }

        public Task InvokeTask(Action action,float time)
        {
            return new Task(Invokelmpl(action, time));
        }

        private IEnumerator Invokelmpl(Action action, float time)
        {
            yield return new WaitForSeconds(time);
            action();
        }

        public string RelativePath()
        {
            return ProjectPath.RelativePath(this);
        }

        private void Awake()
        {
            OnAwake();
        }

        private void Start()
        {
            OnStart();
        }

        private void Update()
        {
            OnUpdate();
        }

        protected virtual void OnAwake() { }
        protected virtual void OnStart() { }
        protected virtual void OnUpdate() { }

        protected void AddMessageListener(string messageName)
        {
            MessageCenter.Instance.AddListener(this, messageName, UpdateMessage);
        }

        protected virtual void UpdateMessage(Message message) { }
    }
}