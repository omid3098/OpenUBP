using System.Collections.Generic;
using NaughtyAttributes;
using OpenTransition;
using UnityEngine;
using UnityEngine.Events;

namespace ScriptableTask
{
    [ScriptOrder(-220)]
    public class GenericChain<TEnum> : MonoBehaviour where TEnum : ITask
    {
        [SerializeField] protected bool autoStart = false;
        [InfoBox("Use for task that will run on game objects. like Ui transitions")] [SerializeField] protected bool areScriptableObjects = true;

        [ReorderableList] [SerializeField] List<TEnum> chain = new List<TEnum>();
        private void Awake()
        {
            for (int i = 0; i < chain.Count; i++)
            {
                chain[i].autoPlay = false;
                if (!areScriptableObjects)
                {
                    if (i != 0)
                        chain[i].self.SetActive(false);
                }
            }
        }
        private void OnEnable()
        {
            for (int i = 0; i < chain.Count - 1; i++)
            {
                chain[i].OnComplete.AddListener(ChainToNext(i));
            }
            if (autoStart) StartChain();
        }

        public void StartChain()
        {
            chain[0].Start();
        }

        private void OnDisable()
        {
            for (int i = 0; i < chain.Count - 1; i++)
            {
                chain[i].OnComplete.RemoveListener(ChainToNext(i));
            }
        }

        private UnityAction ChainToNext(int i)
        {
            return () =>
            {
                if (!areScriptableObjects) chain[i + 1].self.SetActive(true);
                chain[i + 1].Start();
            };
        }
    }
}