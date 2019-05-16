using System;
using UnityEngine.Assertions;

namespace OpenTask
{
    public abstract class GameTask
    {
        public delegate void TaskDelegate(string data);
        public string data { get; protected set; }
        public abstract event TaskDelegate OnComplete;
        public abstract event TaskDelegate OnError;
        public string id;
        public DateTime executionTime;
        public string additionalData { get; private set; }
        public GameTask(string _data = null)
        {
            data = _data;
            id = Guid.NewGuid().ToString();
            executionTime = DateTime.UtcNow;
        }
        public abstract void Execute();

        public string Schedule()
        {
            return AsyncService.Schedule(this);
        }

        public string GetData()
        {
            return data;
        }

        public void SetAdditionalData(string additionalData)
        {
            this.additionalData = additionalData;
        }
    }
}