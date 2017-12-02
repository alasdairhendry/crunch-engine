using Crunch.Communication.Backend;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crunch.Communication
{
    [System.Serializable]
    public class DialogueOptions : DialogueBase 
	{
        [SerializeField] private List<DialogueOption> m_Options = new List<DialogueOption>();
        public List<DialogueOption> options { get { return m_Options; } }

        public List<string> GetOptions { get
            {
                List<string> options = new List<string>();
                foreach (DialogueOption item in m_Options)
                {
                    options.Add(item.option);
                }
                return options;
            } }

        public DialogueOptions() { }

        public DialogueOptions(string _name) { name = _name; }

        public void AddOption(string _option)
        {
            DialogueOption option = new DialogueOption(_option);
            option.option = _option;
            m_Options.Add(option);
        }

        public void AddOption(string _option, DialogueBase _target)
        {
            DialogueOption option = new DialogueOption(_option, _target);
            option.option = _option;
            m_Options.Add(option);
        }

        public override void Initiate(DialogueReader reader)
        {
            reader.OnOptions(this);
        }

        public override DialogueBase Iterate()
        {
            Debug.LogError("Do not call Iterate() from Dialogue Options class. Call Iterate(index) instead.");
            return null;
        }

        public override DialogueBase Iterate(int index)
        {
            return options[index].target;
        }
    }
}

namespace Crunch.Communication.Backend
{
    [System.Serializable]
    public class DialogueOption
    {
        [SerializeField] private string m_Option = "New Option";
        public string option { get { return m_Option; } set { m_Option = value; } }

        [SerializeField] private DialogueBase m_Target;
        public DialogueBase target { get { return m_Target; } set { m_Target = value; } }

        public DialogueOption(string _option)
        {
            m_Option = _option;            
        }

        public DialogueOption(string _option, DialogueBase _target)
        {
            m_Option = _option;
            m_Target = _target;
        }                
    }
}