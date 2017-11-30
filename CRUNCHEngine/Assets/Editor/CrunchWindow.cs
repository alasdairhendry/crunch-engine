using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace Crunch.Engine
{
    public class CrunchWindow : EditorWindow
    {
        private bool sceneTab;
        private bool assetsTab;
        private bool utilitiesTab;
        private bool consoleTab;

        [MenuItem("Crunch/Open Engine", false, 0)]
        private static void Open()
        {
            CrunchWindow window = GetWindow<CrunchWindow>();
            window.Show();
            window.Initialize();
        }

        private void OnEnable()
        {
            titleContent.text = "Crunch Engine";
        }

        private void Initialize()
        {
            GetWindow<CrunchWindow>().position = new Rect(Screen.width - (Screen.width / 2) - 256, Screen.height + (Screen.height / 2) + 256, 512, 512);
        }

        private void Draw()
        {
            Draw_Toolbar();
            Draw_TabOptions();

            Draw_SceneTab();
            Draw_AssetsTab();            
        }

        private void Draw_Toolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(position.width));
            if (GUILayout.Button("Clear Console", EditorStyles.toolbarButton)) { Debug.Log(Utilities.Log.Clear()); }
            if (GUILayout.Button("Close Window", EditorStyles.toolbarButton)) { CrunchWindow window = GetWindow<CrunchWindow>(); window.Close(); }
            GUILayout.EndHorizontal();
        }

        private void Draw_TabOptions()
        {
            GUILayout.BeginVertical(EditorStyles.toolbar);            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Scene", EditorStyles.toolbarButton))
            {
                sceneTab = true;
                assetsTab = false;
                utilitiesTab = false;
                consoleTab = false;
            }

            if (GUILayout.Button("Assets", EditorStyles.toolbarButton))
            {
                sceneTab = false;
                assetsTab = true;
                utilitiesTab = false;
                consoleTab = false;
            }

            if (GUILayout.Button("Utilities", EditorStyles.toolbarButton))
            {
                sceneTab = false;
                assetsTab = false;
                utilitiesTab = true;
                consoleTab = false;
            }

            if (GUILayout.Button("Console", EditorStyles.toolbarButton))
            {
                sceneTab = false;
                assetsTab = false;
                utilitiesTab = false;
                consoleTab = true;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        bool foldoutGameObjects = false;
        bool foldoutUIElements = false;
        bool foldoutTexts = false;
        bool foldoutImages = false;
        bool foldoutButtons = false;
        bool foldoutInputFields = false;
        bool foldoutAudioSource = false;
        private void Draw_SceneTab()
        {
            if (!sceneTab)
                return;

            GUILayout.BeginVertical("Box");
            GUILayout.Label("Scene", EditorStyles.boldLabel);
            GUILayout.EndVertical();

            GameObject[] go = GameObject.FindObjectsOfType<GameObject>();
            Animator[] animators = GameObject.FindObjectsOfType<Animator>();
            MeshRenderer[] meshRenderers = GameObject.FindObjectsOfType<MeshRenderer>();
            SkinnedMeshRenderer[] skinnedMeshRenderers = GameObject.FindObjectsOfType<SkinnedMeshRenderer>();
            MaskableGraphic[] ui = GameObject.FindObjectsOfType<MaskableGraphic>();
            Text[] texts = GameObject.FindObjectsOfType<Text>();
            Image[] images = GameObject.FindObjectsOfType<Image>();
            Button[] buttons = GameObject.FindObjectsOfType<Button>();
            InputField[] inputFields = GameObject.FindObjectsOfType<InputField>();
            AudioSource[] audioSource = GameObject.FindObjectsOfType<AudioSource>();            
            foldoutGameObjects = EditorGUILayout.Foldout(foldoutGameObjects, go.Length + " - GameObjects", true);
            if (foldoutGameObjects)
            {
                EditorGUI.indentLevel++;

                foldoutUIElements = EditorGUILayout.Foldout(foldoutUIElements, ui.Length + " - UI Elements", true);
                if (foldoutUIElements)
                {
                    EditorGUI.indentLevel++;
                    foldoutTexts = EditorGUILayout.Foldout(foldoutTexts, texts.Length + " - Text", true);
                    if (foldoutTexts)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUI.indentLevel++;
                        foreach (Text item in texts)
                        {
                            GUILayout.BeginHorizontal();
                            if(Selection.activeGameObject == item.gameObject)
                            {
                                EditorGUILayout.ToggleLeft("", true, GUILayout.MaxWidth(16));
                            }

                            item.name = EditorGUILayout.TextField(item.name);
                            if (GUILayout.Button("Select", GUILayout.MaxWidth(64)))
                            {
                                Selection.activeGameObject = item.gameObject;
                            }
                            if (GUILayout.Button("Focus", GUILayout.MaxWidth(64)))
                            {
                                Selection.activeGameObject = item.gameObject;
                                EditorApplication.ExecuteMenuItem("Edit/Frame Selected");                                
                            }
                            if (GUILayout.Button("Delete", GUILayout.MaxWidth(64)))
                            {
                                Destroy(item.gameObject);
                            }
                            GUILayout.EndHorizontal();
                        }
                        EditorGUI.indentLevel--;
                        EditorGUI.indentLevel--;
                    }

                    foldoutImages = EditorGUILayout.Foldout(foldoutImages, images.Length + " - Images", true);
                    if (foldoutImages)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUI.indentLevel++;
                        foreach (Image item in images)
                        {
                            GUILayout.BeginHorizontal();
                            if (Selection.activeGameObject == item.gameObject)
                            {
                                EditorGUILayout.ToggleLeft("", true, GUILayout.MaxWidth(16));
                            }

                            item.name = EditorGUILayout.TextField(item.name);
                            if (GUILayout.Button("Select", GUILayout.MaxWidth(64)))
                            {
                                Selection.activeGameObject = item.gameObject;
                            }
                            if (GUILayout.Button("Focus", GUILayout.MaxWidth(64)))
                            {
                                Selection.activeGameObject = item.gameObject;
                                EditorApplication.ExecuteMenuItem("Edit/Frame Selected");
                            }
                            if (GUILayout.Button("Delete", GUILayout.MaxWidth(64)))
                            {
                                Destroy(item.gameObject);
                            }
                            GUILayout.EndHorizontal();
                        }
                        EditorGUI.indentLevel--;
                        EditorGUI.indentLevel--;
                    }
                    EditorGUI.indentLevel--;
                }

                foldoutAudioSource = EditorGUILayout.Foldout(foldoutAudioSource, audioSource.Length + " - Audio Sources", true);
                if (foldoutAudioSource)
                {

                }

                EditorGUI.indentLevel--;
            }
        }

        private void Draw_AssetsTab()
        {
            if (!assetsTab)
                return;

            GUILayout.BeginVertical("Box");
            GUILayout.Label("Assets", EditorStyles.boldLabel);
            GUILayout.EndVertical();
        }

        private void OnGUI()
        {
            Draw();
        }      
    }
}
