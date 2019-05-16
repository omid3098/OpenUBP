using System;
using System.Collections;
using System.Threading.Tasks;
using OpenTask;
using UnityEngine;
public class TaskB : GameTask
{
    public override event TaskDelegate OnComplete;
    public override event TaskDelegate OnError;


    public override async void Execute()
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(2f));
            Debug.Log("TaskB Finished after 2 Seconds");
            if (OnComplete != null) OnComplete(null);
        }
        catch (Exception e)
        {
            if (OnError != null) OnError(e.Message);
        }
    }
}
