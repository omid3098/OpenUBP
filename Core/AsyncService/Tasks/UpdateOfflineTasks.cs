using System;
using System.Collections.Generic;
using UnityEngine;

namespace OpenTask
{
    public class UpdateOfflineTasks : GameTask
    {
        private GameTask gameTask;
        private SerializedTask serializedTask;

        public UpdateOfflineTasks(SerializedTask serializedTask, string _data = null) : base(_data)
        {
            this.serializedTask = serializedTask;
        }

        public override event TaskDelegate OnComplete;
        public override event TaskDelegate OnError;
        public override void Execute()
        {
            Debug.Log("____ UpdateOfflineTasks: ____");
            Type type = Type.GetType(serializedTask.classType); //target type
            Debug.Log("type: " + type + " - data: " + serializedTask.data);
            object instanceObject = Activator.CreateInstance(type, new object[] { serializedTask.data }); // an instance of target type
            gameTask = (GameTask)instanceObject;

            gameTask.id = serializedTask.id;
            gameTask.SetAdditionalData(serializedTask.additionalData);
            gameTask.executionTime = DateTime.Parse(serializedTask.executionTime);
            gameTask.OnComplete += MoveNext;
            gameTask.OnError += RetryTask;
            gameTask.Execute();
        }

        private void RetryTask(string data)
        {
            // we dont count for retry because eac task retries itself
            if (OnError != null) OnError.Invoke(data);
        }

        private void MoveNext(string data)
        {
            gameTask.OnComplete -= MoveNext;
            gameTask.OnError -= RetryTask;
            AsyncService.SavedTaskDone();
            if (AsyncService.poolCount == 0) if (OnComplete != null) OnComplete.Invoke(data);
        }
    }
}
