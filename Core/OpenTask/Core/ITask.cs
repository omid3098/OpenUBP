using UnityEngine;
using UnityEngine.Events;

namespace ScriptableTask
{
    public interface ITask
    {
        bool autoPlay { get; set; }
        GameObject self { get; set; }
        UnityEvent OnStart { get; set; }
        UnityEvent OnStop { get; set; }
        UnityEvent OnPause { get; set; }
        UnityEvent OnResume { get; set; }
        UnityEvent OnUpdate { get; set; }
        UnityEvent OnComplete { get; set; }
        void Start();
        void Stop();
        void Pause();
        void Update();
    }
}