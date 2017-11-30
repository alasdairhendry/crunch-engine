using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crunch.Communication
{
    public class DialogueSystem : MonoBehaviour
    {
        public Action<Dialogue> onDialogueInitiate;
        private Action onDialogueEnd;

        public void ListenToDialogueInitiate(Action<Dialogue> _event)
        {
            onDialogueInitiate += _event;
        }

        public void ListenToDialogueEnd(Action _event)
        {
            onDialogueEnd += _event;
        }

        public void CallInitiate(Dialogue dialogue)
        {
            if (onDialogueInitiate != null)
            {
                onDialogueInitiate(dialogue);
                Debug.Log("Dialogue has started");
            }
            else
                Debug.LogError("No dialogue events to initiate.");
        }

        public void CallEnd()
        {
            if (onDialogueEnd != null)
            {
                onDialogueEnd();
                Debug.Log("Dialogue has ended");
            }
            else
                Debug.LogError("No dialogue events to end.");
        }

        private void Start()
        {
            if (GameObject.FindObjectsOfType<DialogueSystem>().Length != 1)
            {
                Debug.LogError("More than one Dialogue System in the scene");
            }
        }

        private void Update()
        {

        }
    }
}