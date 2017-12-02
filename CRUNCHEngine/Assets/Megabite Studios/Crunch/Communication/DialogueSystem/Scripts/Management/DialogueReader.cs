using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crunch.Communication
{
	public class DialogueReader : MonoBehaviour 
	{
        private bool dialogueIsActive = false;
        protected Dialogue activeDialogue;
        private DialogueBase activeDialogueBase;

        //protected Action onStart;
        //protected Action<DialogueSpeech> onSpeech;
        //protected Action<DialogueOptions> onOptions;
        //protected Action onEnd;

        protected virtual void Start () 
		{
            OnDialogueReader();
		}
			
		private void Update () 
		{
			if(dialogueIsActive)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Iterate();
                }
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    Iterate(0);
                }
            }
		}

        protected virtual void OnDialogueReader()
        {
            GameObject.FindObjectOfType<DialogueSystem>().onDialogueInitiate += Listen;
            Debug.Log("ooo");
        }

        protected virtual void Listen(Dialogue dialogue)
        {
            dialogueIsActive = true;
            activeDialogue = dialogue;

            activeDialogueBase = activeDialogue.GetInitial();
            activeDialogueBase.Initiate(this);
            //OnStart();
        }

        protected void Iterate()
        {
            activeDialogueBase = activeDialogue.GetNext(activeDialogueBase);
            activeDialogueBase.Initiate(this);

            if(activeDialogueBase.name == "End")
            {
                GameObject.FindObjectOfType<DialogueSystem>().CallEnd();
            }
        }

        protected void Iterate(int index)
        {            
            activeDialogueBase = activeDialogue.GetNext(activeDialogueBase, index);
            activeDialogueBase.Initiate(this);

            if (activeDialogueBase.name == "End")
            {
                GameObject.FindObjectOfType<DialogueSystem>().CallEnd();
            }
        }

        public virtual void OnStart(DialogueStart start)
        {
            
        }

        public virtual void OnSpeech(DialogueSpeech speech)
        {

        }

        public virtual void OnOptions(DialogueOptions options)
        {

        }

        public virtual void OnEnd(DialogueEnd end)
        {

        }
    }
}