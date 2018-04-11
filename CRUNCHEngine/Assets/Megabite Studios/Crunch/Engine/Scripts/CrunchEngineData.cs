using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Preset", menuName = "Crunch/New Engine Preset", order = 1)]
public class CrunchEngineData : ScriptableObject {

    public string presetName { get { return this.name; } set { this.name = value; } }

    [HideInInspector] public AudioClip UISoundEvent_MouseEnter;
    [HideInInspector] public AudioClip UISoundEvent_MouseDown;
    [HideInInspector] public AudioClip UISoundEvent_MouseUp;
    [HideInInspector] public AudioClip UISoundEvent_MouseExit;
}
