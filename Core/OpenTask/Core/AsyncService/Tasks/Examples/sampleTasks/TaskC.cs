using System;
using System.Collections;
using System.Threading.Tasks;
using OpenTask;
using UnityEngine;
namespace OpenTask.Example
{
    public class TaskC : GameTask
    {
        public override event TaskDelegate OnComplete;
        public override event TaskDelegate OnError;
        public override async void Execute()
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(3f));
                Debug.Log("TaskC Finished after 3 Seconds");
                if (OnComplete != null) OnComplete(null);
            }
            catch (Exception e)
            {
                if (OnError != null) OnError(e.Message);
            }
        }
    }
}