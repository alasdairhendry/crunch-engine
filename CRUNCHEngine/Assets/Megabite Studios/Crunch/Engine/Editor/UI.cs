using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Crunch.Engine.Editor
{
    public class UI : UnityEditor.Editor
    {
        [MenuItem("GameObject/Crunch/UI/Window", false, 0)]
        public static void CreateNewWindow()
        {
            if (Selection.activeGameObject.GetComponent<RectTransform>() != null)
            {
                GameObject parent = new GameObject("New Window", typeof(RectTransform), typeof(CanvasRenderer), typeof(Crunch.UI.Window));
                parent.transform.SetParent(Selection.activeTransform);

                GameObject child = new GameObject("Main_Panel", typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image));
                child.transform.SetParent(parent.transform);

                parent.GetComponent<Crunch.UI.Window>().Reset();

                Selection.activeGameObject = parent.gameObject;
                EditorApplication.ExecuteMenuItem("Edit/Frame Selected");
            }
        }
    }
}
