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

            RunEngine();
        }

        [SerializeField] [HideInInspector] private CrunchEngineData currentData;
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

        private void RunEngine()
        {
            if (currentData.usingLoadingSystem) CreateLoadingSystem();
            if (currentData.usingMusicSystem) CreateMusicSystem();
            if (currentData.usingSoundEffectSystem) CreateSoundEffectSystem();
            if (currentData.usingPoolingSystem) CreatePoolingSystem();
        }

        private void CreateLoadingSystem()
        {
            Debug.Log("CreateLoadingSystem");
        }

        private void CreateMusicSystem()
        {
            Debug.Log("CreateMusicSystem");
        }

        private void CreateSoundEffectSystem()
        {
            Debug.Log("CreateSoundEffectSystem");
        }

        private void CreatePoolingSystem()
        {
            Debug.Log("CreatePoolingSystem");
        }
    }
}