using UnityEngine;
namespace ScriptableTask
{
    public interface IScriptableTask
    {
        void Execute();
        void Stop();
    }
    public abstract class BaseScriptableTask : ScriptableObject, IScriptableTask
    {
        public abstract void Execute();
        public abstract void Stop();
    }
}