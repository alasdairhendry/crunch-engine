using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Crunch.Engine
{
    public class CrunchEngine : MonoBehaviour
    {
        public static CrunchEngine singleton;

        private void Awake()
        {
            if (singleton == null)
                singleton = this;
            else if (singleton != this)
            {
                Debug.LogError("You have more than one CrunchEngine active. Please delete one before continuing.");
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        [SerializeField] private CrunchEngineData currentData;
        public CrunchEngineData CurrentData { get { return currentData; } set { currentData = value; } }

        private void OnDisable()
        {            
            if (singleton == this)
                Debug.LogError("You are disabling your Crunch Engine Reference", this);
        }

        private void OnDestroy()
        {
            if (singleton == this)
                Debug.LogError("You are destroying your Crunch Engine Reference", this);            
        }
        
        public void AssignEngineData(CrunchEngineData newData)
        {
            currentData = newData;
        }
    }
}