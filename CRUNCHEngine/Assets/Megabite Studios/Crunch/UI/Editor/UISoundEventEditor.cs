using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Crunch.UI.Editor
{
    [UnityEditor.CustomEditor(typeof(UISoundEvent))]
    public class UISoundEventEditor : UnityEditor.Editor
    {
        UISoundEvent targ;

        public void OnEnable()
        {
            targ = (UISoundEvent)target;
        }

        public override void OnInspectorGUI()
        {
            if(targ.UseDefaultSounds)
            {
                GUILayout.Space(20);
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);                
                targ.UseDefaultSounds = GUILayout.Toggle(targ.UseDefaultSounds, "Use Default Sounds");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Space(20);
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);                
                targ.UseDefaultSounds = GUILayout.Toggle(targ.UseDefaultSounds, "Use Default Sounds");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginVertical("Box");                
                GUILayout.Label("Event Sounds", EditorStyles.boldLabel);

                GUILayout.BeginHorizontal();
                GUILayout.Label("On Mouse Enter");
                GUILayout.FlexibleSpace();
                targ.OnMouseEnter = (AudioClip)EditorGUILayout.ObjectField(targ.OnMouseEnter, typeof(AudioClip), false);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("On Mouse Down");
                GUILayout.FlexibleSpace();
                targ.OnMouseDown = (AudioClip)EditorGUILayout.ObjectField(targ.OnMouseDown, typeof(AudioClip), false);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("On Mouse Up");
                GUILayout.FlexibleSpace();
                targ.OnMouseUp = (AudioClip)EditorGUILayout.ObjectField(targ.OnMouseUp, typeof(AudioClip), false);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("On Mouse Enter");
                GUILayout.FlexibleSpace();
                targ.OnMouseExit = (AudioClip)EditorGUILayout.ObjectField(targ.OnMouseExit, typeof(AudioClip), false);
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }
        }
    } 
}
