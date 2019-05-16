using System;
using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using OpenTask;
using UnityEngine;
namespace OpenTask.Example
{
    public class TaskA : GameTask
    {
        public override event TaskDelegate OnComplete;
        public override event TaskDelegate OnError;

        public override async void Execute()
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(1f));
                Debug.Log("TaskA Finished after 1 Seconds");
                if (OnComplete != null) OnComplete(null);
            }
            catch (Exception e)
            {
                if (OnError != null) OnError(e.Message);
            }
        }
    }
}