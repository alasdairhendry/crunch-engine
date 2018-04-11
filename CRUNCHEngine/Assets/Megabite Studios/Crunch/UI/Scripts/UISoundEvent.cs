using Crunch.Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Crunch.UI
{
    public class UISoundEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private bool useDefaultSounds;
        public bool UseDefaultSounds { get { return useDefaultSounds; } set { useDefaultSounds = value; } }

        private AudioClip onMouseEnter;
        public AudioClip OnMouseEnter { get { return onMouseEnter; } set { onMouseEnter = value; } }

        private AudioClip onMouseDown;
        public AudioClip OnMouseDown { get { return onMouseDown; } set { onMouseDown = value; } }

        private AudioClip onMouseUp;
        public AudioClip OnMouseUp { get { return onMouseUp; } set { onMouseUp = value; } }

        private AudioClip onMouseExit;
        public AudioClip OnMouseExit { get { return onMouseExit; } set { onMouseExit = value; } }

        private void Start()
        {
            if (useDefaultSounds)
            {                
                onMouseEnter = CrunchEngine.singleton.CurrentData.UISoundEvent_MouseEnter;
                onMouseDown = CrunchEngine.singleton.CurrentData.UISoundEvent_MouseDown;
                onMouseUp = CrunchEngine.singleton.CurrentData.UISoundEvent_MouseUp;
                onMouseExit = CrunchEngine.singleton.CurrentData.UISoundEvent_MouseExit;
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {

        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {

        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {

        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {

        }

    } 
}
