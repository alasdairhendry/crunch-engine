using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Crunch.UI
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasRenderer))]
    public class Window : MonoBehaviour
    {
        private bool isOpen = false;
        public bool IsOpen { get { return isOpen; } }

        [SerializeField] protected Image TargetGraphic;
        [SerializeField] [Tooltip("The Button which will close this Window. Note, you do not need to add any listeners. They are added automatically.")] protected Image CloseGraphic;
        [SerializeField] private bool activeOnAwake = false;

        protected virtual void Awake()
        {
            if (activeOnAwake)
            {
                isOpen = true;
                TargetGraphic.gameObject.SetActive(isOpen);
            }
            else
            {
                isOpen = false;
                TargetGraphic.gameObject.SetActive(isOpen);
            }

            if(CloseGraphic != null)
            {
                CloseGraphic.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Close();
                });
            }   
        }

        // Use this for initialization
        protected virtual void Start()
        {

        }

        // Update is called once per frame
        protected virtual void Update()
        {

        }

        public virtual void Open()
        {
            if (isOpen) return;

            isOpen = true;
            TargetGraphic.gameObject.SetActive(isOpen);
        }

        public virtual void Trigger()
        {
            isOpen = !isOpen;
            TargetGraphic.gameObject.SetActive(isOpen);
        }

        public virtual void Close()
        {
            if (!isOpen) return;

            isOpen = false;
            TargetGraphic.gameObject.SetActive(isOpen);
        }

        public void Reset()
        {
            RectTransform rect = GetComponent<RectTransform>();
            rect.localScale = Vector3.one;
            rect.anchorMin = new Vector2(0.0f, 0.0f);
            rect.anchorMax = new Vector2(1.0f, 1.0f);
            rect.offsetMin = new Vector2(0.0f, 0.0f);
            rect.offsetMax = new Vector2(0.0f, 0.0f);

            if (transform.childCount >= 1)
            {
                if(transform.GetChild(0).GetComponent<Image>() != null)
                {
                    TargetGraphic = transform.GetChild(0).GetComponent<Image>();
                    RectTransform targetGraphicRect = TargetGraphic.GetComponent<RectTransform>();

                    targetGraphicRect.localScale = Vector3.one;
                    targetGraphicRect.anchorMin = new Vector2(0.0f, 0.0f);
                    targetGraphicRect.anchorMax = new Vector2(1.0f, 1.0f);
                    targetGraphicRect.offsetMin = new Vector2(0.0f, 0.0f);
                    targetGraphicRect.offsetMax = new Vector2(0.0f, 0.0f);
                }
            }
        }

        public void OnValidate()
        {
            if (TargetGraphic == this.gameObject)
            {
                Debug.LogError("Target Graphic cannot be the root GameObject. Consider assigning a child of this object. Target Graphic Reset.", this);
                TargetGraphic = null;
            }
        }
    }
}
