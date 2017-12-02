using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crunch.Communication
{
	public class Dialogue : MonoBehaviour 
	{
        public DialogueStart start = new DialogueStart();
        public DialogueEnd end = new DialogueEnd();
        public List<DialogueSpeech> speechEntries = new List<DialogueSpeech>();
        public List<DialogueOptions> optionsEntries = new List<DialogueOptions>();

        [ContextMenu("Initiate Dialogue")]
        public void Initiate()
        {
            GameObject.FindObjectOfType<DialogueSystem>().CallInitiate(this);
        }

        public DialogueBase GetInitial()
        {
            DialogueBase _base = start.target;

            if (_base == null)
            {
                Debug.LogError("Cannot iterate to invalid dialogue");
                return null;
            }

            foreach (DialogueSpeech speech in speechEntries)
            {
                if (_base.name == speech.name)
                {
                    return speech;
                }
            }

            foreach (DialogueOptions option in optionsEntries)
            {
                if (_base.name == option.name)
                {
                    return option;
                }
            }

            if (_base.name == "End")
            {
                return end;
            }

            Debug.LogError("Found No Object");
            return null;
        }

        public DialogueBase GetNext(DialogueBase current)
        {            
            DialogueBase _base = current.Iterate();

            if(_base == null)
            {
                Debug.LogError("Cannot iterate to invalid dialogue");
                return null;
            }

            foreach (DialogueSpeech speech in speechEntries)
            {
                if (_base.name == speech.name)
                {
                    return speech;
                }
            }

            foreach (DialogueOptions option in optionsEntries)
            {
                if (_base.name == option.name)
                {
                    return option;
                }
            }

            if(_base.name == "End")
            {
                return end;
            }

            Debug.LogError("Found No Object");
            return null;
        }

        public DialogueBase GetNext(DialogueBase current, int index)
        {
            DialogueBase _base = current.Iterate(index);

            if (_base == null)
            {
                Debug.LogError("Cannot iterate to invalid dialogue");
                return null;
            }

            foreach (DialogueSpeech speech in speechEntries)
            {
                if (_base.name == speech.name)
                {
                    return speech;
                }
            }

            foreach (DialogueOptions option in optionsEntries)
            {
                if (_base.name == option.name)
                {
                    return option;
                }
            }

            if (_base.name == "End")
            {
                return end;
            }

            Debug.LogError("Found No Object");
            return null;
        }

        [ContextMenu("Create Template")]
        private void CreateTemplateDialogue()
        {
            DialogueSpeech speech01 = AddSpeech("Intro", "Well, Hello there!");         

            DialogueSpeech speech02 = AddSpeech("Ending", "Goodbye!");
            DialogueSpeech speech03 = AddSpeech("Turns Nigga", "Haha, SUP NIBBA");
            DialogueSpeech speech04 = AddSpeech("Triggered", "Wow.. Rude");

            DialogueOptions options01 = AddOptions("Intro Response");
            options01.AddOption("Yeah, hey", speech02);
            options01.AddOption("Sup dawg", speech03);
            options01.AddOption("I ain't got time for you!", speech04);

            speech01.target = options01;
            speech02.target = end;
            speech03.target = end;
            speech04.target = end;
            start.target = speech01;
            end.target = start;
        }

        [ContextMenu("Clear Dialogue")]
        private void Clear()
        {
            speechEntries.Clear();
            optionsEntries.Clear();
        }

        private DialogueSpeech AddSpeech()
        {
            DialogueSpeech speech = new DialogueSpeech();
            speechEntries.Add(speech);
            return speech;
        }

        public DialogueSpeech AddSpeech(string name)
        {
            DialogueSpeech speech = new DialogueSpeech(name);
            speechEntries.Add(speech);
            return speech;
        }

        private DialogueSpeech AddSpeech(string _name, string _speech)
        {
            DialogueSpeech speech = new DialogueSpeech(_name, _speech);
            speechEntries.Add(speech);
            return speech;
        }

        private DialogueOptions AddOptions()
        {
            DialogueOptions options = new DialogueOptions();
            optionsEntries.Add(options);
            return options;
        }

        public DialogueOptions AddOptions(string name)
        {
            DialogueOptions options = new DialogueOptions(name);
            optionsEntries.Add(options);
            return options;
        }
    }
}