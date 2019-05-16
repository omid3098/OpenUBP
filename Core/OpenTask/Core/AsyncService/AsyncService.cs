namespace OpenTask
{
    using System.Collections.Generic;
    using System.Collections;
    using System;
    using UnityEngine.Assertions;
    using UnityEngine;


    [Serializable]
    public class ListHolder<T>
    {
        [SerializeField]
        public List<T> list;
    }

    public class AsyncService
    {
        public delegate void OnAsyncServiceReadyDelegate();
        public static event OnAsyncServiceReadyDelegate OnReady;
        public delegate void ScheduleDelegate(string id);
        public static event ScheduleDelegate OnScheduleFinished;
        static GameTask currentExecutingTask;
        private static string currentScheduleID;

        static public List<SerializedTask> savedTaskes { get; private set; }
        static List<GameTask> taskPool { get; set; }
        public static int poolCount { get { return taskPool.Count; } }
        public static bool ready { get; private set; }
        const string TaskKey = "TASK_KEY";

        static void Init()
        {
            if (ready) return;
            savedTaskes = new List<SerializedTask>();
            taskPool = new List<GameTask>();
            currentExecutingTask = null;
            Ready();
        }

        /// <summary>
        /// call this method when your game is ready to update unfinished tasks
        /// </summary>
        static void UpdateUnfinishedTasks()
        {
            savedTaskes = LoadAllTasks();
            if (savedTaskes != null && savedTaskes.Count != 0)
            {
                var updateOfflineTasks = new UpdateOfflineTasks(savedTaskes[0]);
                updateOfflineTasks.Execute();
            }
            else
            {
                Ready();
            }
        }

        static void Ready()
        {
            ready = true;
            if (OnReady != null)
            {
                OnReady.Invoke();
            }
        }

        void SaveTask(GameTask task)
        {
            if (!ready) Init();
            Debug.Log("Saving Task");
            if (savedTaskes == null) savedTaskes = new List<SerializedTask>();
            savedTaskes.Add(new SerializedTask()
            {
                id = task.id,
                classType = task.GetType().FullName,
                data = task.GetData(),
                additionalData = task.additionalData,
                executionTime = task.executionTime.ToString(),
            });
            SaveAllTasks(savedTaskes);
        }

        public static void SavedTaskDone()
        {
            if (!ready) Init();
            var _task = savedTaskes[0];
            savedTaskes.Remove(_task);
            SaveAllTasks(savedTaskes);
            UpdateUnfinishedTasks();
        }

        public static string Schedule(GameTask task)
        {
            if (!ready) Init();
            taskPool.Add(task);
            if (currentExecutingTask == null)
            {
                ExecuteFirstTask();
                currentScheduleID = Guid.NewGuid().ToString();
            }
            return currentScheduleID;
        }

        static void ExecuteFirstTask()
        {
            if (taskPool.Count == 0)
            {
                if (OnScheduleFinished != null) OnScheduleFinished.Invoke(currentScheduleID);
                return;
            }
            currentExecutingTask = taskPool[0];
            currentExecutingTask.OnComplete += RemoveTaskFromPool;
            currentExecutingTask.Execute();
        }

        static void RemoveTaskFromPool(string data)
        {
            currentExecutingTask.OnComplete -= RemoveTaskFromPool;
            taskPool.Remove(currentExecutingTask);
            currentExecutingTask = null;
            ExecuteFirstTask();
        }

        static void SaveAllTasks(List<SerializedTask> tasks)
        {
            ListHolder<SerializedTask> holder = new ListHolder<SerializedTask>();
            holder.list = tasks;
            var data = JsonUtility.ToJson(holder);
            Debug.Log(data);
            PlayerPrefs.SetString(TaskKey, data);
        }

        static List<SerializedTask> LoadAllTasks()
        {
            ListHolder<SerializedTask> holder = new ListHolder<SerializedTask>();
            var data = PlayerPrefs.GetString(TaskKey, "");
            if (string.IsNullOrEmpty(data))
            {
                return holder.list;
            }
            else
            {
                holder = JsonUtility.FromJson<ListHolder<SerializedTask>>(data);
                Debug.Log(holder.list);
                return holder.list;
            }
        }
    }
}