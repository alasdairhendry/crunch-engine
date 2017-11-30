using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crunch.Communication
{
    [System.Serializable]
	public class DialogueSpeech : DialogueBase 
	{
        [SerializeField] private string m_speech;
        public string speech { get { return m_speech; } set { m_speech = value; } }

        [SerializeField] private DialogueBase m_Target;
        public DialogueBase target { get { return m_Target; } set { m_Target = value; } }

        public DialogueSpeech() { }

        public DialogueSpeech(string _name) { name = _name; }

        public DialogueSpeech(string _name, string _speech) { name = _name; speech = _speech; }

        public override void Initiate(DialogueReader reader)
        {
            reader.OnSpeech(this);
        }

        public override DialogueBase Iterate()
        {
            return target;
        }

        public override DialogueBase Iterate(int index)
        {
            Debug.LogError("Do not call Iterate(index) from Dialogue Speech class. Call Iterate() instead.");
            return null;
        }
    }
}