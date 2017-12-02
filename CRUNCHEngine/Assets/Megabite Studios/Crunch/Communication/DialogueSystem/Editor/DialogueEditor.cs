using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Crunch.Communication
{
    [CustomEditor(typeof(Dialogue))]
	public class DialogueEditor : Editor 
	{
        Dialogue t;
        private bool drawSpeech = false;
        private bool drawOptions = false;

        private string saveTo = "";

        public void OnEnable()
        {
            t = (Dialogue)target;
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            drawSpeech = EditorGUILayout.Foldout(drawSpeech, new GUIContent("Speech Dialogues"));
            if (drawSpeech)
                DrawSpeech();

            drawOptions = EditorGUILayout.Foldout(drawOptions, new GUIContent("Option Dialogues"));
            if (drawOptions)
                DrawOptions();

            if (GUILayout.Button("Open in Node Editor"))
            {
                NodeEditor window = EditorWindow.GetWindow<NodeEditor>();
                window.SetDialogue(t);
            }
        }

        private void DrawSpeech()
        {            
            EditorGUILayout.BeginVertical("Box");            
            EditorGUILayout.BeginVertical();
            foreach (DialogueSpeech item in t.speechEntries)
            {
                GUILayout.Label(t.speechEntries.IndexOf(item) + ". " + item.name);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }

        private void DrawOptions()
        {
            EditorGUILayout.BeginVertical("Box");            
            EditorGUILayout.BeginVertical();
            foreach (DialogueOptions item in t.optionsEntries)
            {
                GUILayout.Label(t.optionsEntries.IndexOf(item) + ". " + item.name);
                GUILayout.Label("\t" + item.options.Count);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }
    }
}