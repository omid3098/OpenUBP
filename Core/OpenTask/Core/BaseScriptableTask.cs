using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace ScriptableTask
{
    public abstract class BaseScriptableTask : ScriptableObject, ITask
    {
        public UnityEvent OnStart { get; set; }
        public UnityEvent OnStop { get; set; }
        public UnityEvent OnPause { get; set; }
        public UnityEvent OnUpdate { get; set; }
        public UnityEvent OnComplete { get; set; }
        public UnityEvent OnResume { get; set; }
        public bool autoPlay { get; set; }
        public GameObject self { get { Debug.LogError("Scriptable Tasks does not support deactiveGameObjectOnStart"); return null; } set { } }

        public abstract void Start();
        public abstract void Stop();
        public abstract void Pause();
        public abstract void Update();
    }
}