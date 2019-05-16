using UnityEngine;
namespace OpenTask.Example
{
    public class TaskUsage : MonoBehaviour
    {
        private string chainID;

        private void Awake()
        {
            var taskA = new TaskA();
            taskA.Schedule();
            var taskB = new TaskB();
            taskB.Schedule();
            var taskC = new TaskC();
            chainID = taskC.Schedule();
            // result:
            // TaskA Finished after 3 Seconds
            // TaskB Finished after 4 Seconds
            // TaskC Finished after 5 Seconds

            AsyncService.OnScheduleFinished += OnScheduleFinish;
        }

        private void OnScheduleFinish(string id)
        {
            AsyncService.OnScheduleFinished -= OnScheduleFinish;
            if (chainID == id)
            {
                Debug.Log("chain finished");
                var taskA = new TaskA();
                taskA.Schedule();
            }
        }
    }
}