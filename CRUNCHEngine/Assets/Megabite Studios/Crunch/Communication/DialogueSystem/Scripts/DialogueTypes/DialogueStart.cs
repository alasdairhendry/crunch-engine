using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crunch.Communication
{
    [System.Serializable]
	public class DialogueStart : DialogueBase 
	{
        [SerializeField] private DialogueBase m_Target;
        public DialogueBase target { get { return m_Target; } set { m_Target = value; } }

        public DialogueStart() { name = "Start"; }

        public override void Initiate(DialogueReader reader)
        {
            reader.OnStart(this);
        }

        public override DialogueBase Iterate()
        {
            return target;
        }

        public override DialogueBase Iterate(int index)
        {
            Debug.LogError("Do not call Iterate(index) from Dialogue Start class. Call Iterate() instead.");
            return null;
        }
    }
}