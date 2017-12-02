using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crunch.Communication
{
    [System.Serializable]
	public class DialogueBase 
	{
        [SerializeField] private string m_Name;
        public string name { get { return m_Name; } set { m_Name = value; } }

        //[SerializeField] private List<DialogueBase> m_Targets = new List<DialogueBase>();
        //public List<DialogueBase> targets { get { return m_Targets; } set { m_Targets = value; } }

        public DialogueBase() { }

        public virtual void Initiate(DialogueReader reader) { Debug.Log(name + " BASE"); }

        public virtual DialogueBase Iterate() { Debug.LogError("Do not call Iterate() from base Dialogue class. Call from Speech or Options."); return null; }

        public virtual DialogueBase Iterate(int index) { Debug.LogError("Do not call Iterate(index) from base Dialogue class. Call from Speech or Options."); return null; }
	}
}