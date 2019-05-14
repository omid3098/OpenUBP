using UnityEngine;
using UnityEngine.Events;

namespace ScriptableTask
{
    public interface ITask
    {
        void Start();
        void Stop();
        void Pause();
        void Update();
        UnityEvent OnStart { get; set; }
        UnityEvent OnStop { get; set; }
        UnityEvent OnPause { get; set; }
        UnityEvent OnUpdate { get; set; }
        UnityEvent OnComplete { get; set; }
    }
    public abstract class BaseScriptableTask : ScriptableObject, ITask
    {
        public UnityEvent OnStart { get; set; }
        public UnityEvent OnStop { get; set; }
        public UnityEvent OnPause { get; set; }
        public UnityEvent OnUpdate { get; set; }
        public UnityEvent OnComplete { get; set; }

        public abstract void Pause();
        public abstract void Start();
        public abstract void Stop();
        public abstract void Update();
    }
}