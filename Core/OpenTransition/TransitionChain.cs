using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace OpenTransition
{

    [ScriptOrder(-200)]
    public class TransitionChain : MonoBehaviour
    {
        [InfoBox("Events of transitions you put here, may be modified, make sure to check them if you want them our of chain")]
        [ReorderableList]
        [SerializeField]
        List<BaseTransition> chain = new List<BaseTransition>();
        [SerializeField] bool deactiveAllOnStart = true;

        void ChainUpdated()
        {
            foreach (var transition in chain)
            {
                transition.SetAutoPlay(false);
            }
        }

        private void Awake()
        {
            for (int i = 0; i < chain.Count; i++)
            {
                BaseTransition transition = chain[i];
                transition.SetUseEvents(true);
                transition.SetAutoPlay(false);
                if (deactiveAllOnStart)
                {
                    if (i != 0)
                        transition.gameObject.SetActive(false);
                }
            }
        }
        private void OnEnable()
        {
            for (int i = 0; i < chain.Count - 1; i++)
            {
                chain[i].onComplete.AddListener(ChainToNext(i));
            }
            chain[0].Play();
        }

        private void OnDisable()
        {
            for (int i = 0; i < chain.Count - 1; i++)
            {
                chain[i].onComplete.RemoveListener(ChainToNext(i));
            }
        }

        private UnityAction ChainToNext(int i)
        {
            return () =>
            {
                chain[i + 1].gameObject.SetActive(true);
                chain[i + 1].Play();
            };
        }
    }
}