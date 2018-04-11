using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.IO;

namespace Crunch.Engine.Editor
{
    public class CrunchWindow : EditorWindow
    {
        private bool hasCurrentData = false;
        private bool hasCrunchEngine = false;
        //private CrunchEngineData currentData;
        private CrunchEngine crunchEngine;

        private bool sceneTab;
        private bool assetsTab;
        private bool utilitiesTab;
        private bool consoleTab;
        private int iconDimensions = 24;
        private Vector2 mainScrollPosition;
        private Vector2 searchScrollPosition;

        // Scene Tab 
        private bool scene_FoldoutGameObjects, scene_FoldoutUIElements, scene_FoldoutTexts, scene_FoldoutImages, scene_FoldoutButtons, scene_FoldoutInputFields, scene_FoldoutAudioSource = false;
        private enum SceneDisplayObjectTypes { DontDisplay, DisplayTypes }
        private SceneDisplayObjectTypes sceneDisplayObjectTypes = SceneDisplayObjectTypes.DisplayTypes;
        private bool Scene_ShowTypes { get { if (sceneDisplayObjectTypes == SceneDisplayObjectTypes.DisplayTypes) return true; else return false; } }
        private string scene_SearchParameter = "";
        private List<int> scene_FavouriteObjects = new List<int>();

        // Utilities Tab
        private AudioClip UISoundEvent_MouseEnter;
        private AudioClip UISoundEvent_MouseDown;
        private AudioClip UISoundEvent_MouseUp;
        private AudioClip UISoundEvent_MouseExit;

        [MenuItem("Crunch/Open Engine", false, 0)]
        private static void Open()
        {
            CrunchWindow window = GetWindow<CrunchWindow>();
            window.Show();
            window.Initialize();
        }

        private void OnEnable()
        {
            CrunchEngine ce = GameObject.FindObjectOfType<CrunchEngine>();
            if(ce == null)
            {
                hasCrunchEngine = false;
                hasCurrentData = false;
                //currentData = null;
                crunchEngine = null;
            }
            else
            {
                crunchEngine = ce;
                hasCrunchEngine = true;

                if(crunchEngine.CurrentData != null)
                {
                    hasCurrentData = true;
                    //currentData = ce.CurrentData;
                }
            }
            titleContent.text = "Crunch Engine";            
        }

        private void Initialize()
        {
            position = new Rect(Screen.width - (Screen.width / 2) - 256, Screen.height + (Screen.height / 2) + 256, 512, 512);
            minSize = new Vector2(512, 256);
            sceneTab = true;
            ReadFavourites();
        }

        private void Draw()
        {
            Draw_TabOptions();

            if (hasCurrentData)
            {
                Draw_Toolbar();
                Draw_SceneTab();
                Draw_AssetsTab();
                Draw_UtilitiesTab();
            }
            else
            {
                Draw_Initialization();
            }
        }

        private void Draw_Initialization()
        {
            GUILayout.BeginVertical("Box");
            GUILayout.Label("Crunch Engine", EditorStyles.boldLabel);
            GUILayout.EndVertical();

            mainScrollPosition = GUILayout.BeginScrollView(mainScrollPosition, false, false);
            GUILayout.BeginVertical("Box");

            GUILayout.Label("Initialize", EditorStyles.boldLabel);

            if (hasCrunchEngine)
            {
                if (!hasCurrentData)
                {                                        
                    EditorGUILayout.LabelField("You haven't initialized your current Preset yet. \\nIf you have not created one yet, please do so using the button. If you already have a Preset, assign it in the field.");
                    EditorStyles.label.wordWrap = true;

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label("Create New Preset: ");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Create New"))
                    {
                        CrunchEngineData data = ScriptableObject.CreateInstance<CrunchEngineData>();
                        AssetDatabase.CreateAsset(data, "Assets/Megabite Studios/Crunch/Presets/New Preset.asset");
                        AssetDatabase.SaveAssets();
                        EditorUtility.FocusProjectWindow();
                        Selection.activeObject = data;

                        crunchEngine.AssignEngineData(data);
                        //currentData = data;
                        //titleContent.text = "Crunch Engine - " + currentData.presetName;
                        hasCurrentData = true;
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label("Or Assign Existing: ");
                    GUILayout.FlexibleSpace();
                    EditorGUI.BeginChangeCheck();
                    crunchEngine.CurrentData = (CrunchEngineData)EditorGUILayout.ObjectField(crunchEngine.CurrentData, typeof(CrunchEngineData), false);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (crunchEngine.CurrentData != null)
                        {
                            hasCurrentData = true;

                            if (hasCrunchEngine)
                            {
                                //crunchEngine.AssignEngineData(currentData);
                                //currentData = crunchEngine.CurrentData;
                                //titleContent.text = "Crunch Engine - " + currentData.presetName;
                            }
                            else
                            {
                                GameObject go = new GameObject();
                                go.transform.position = Vector3.zero;
                                go.transform.localScale = Vector3.one;
                                go.transform.rotation = Quaternion.identity;
                                go.name = "CrunchEngine";
                                crunchEngine = go.AddComponent<CrunchEngine>();
                                //crunchEngine.AssignEngineData(currentData);
                                //currentData = crunchEngine.CurrentData;
                                //titleContent.text = "Crunch Engine - " + currentData.presetName;
                                hasCrunchEngine = true;
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                GUILayout.Label("You haven't initialized Crunch Engine yet. Use the button below to begin.");

                GUILayout.BeginHorizontal();  
                if (GUILayout.Button("Begin"))
                {
                    GameObject go = new GameObject();
                    go.transform.position = Vector3.zero;
                    go.transform.localScale = Vector3.one;
                    go.transform.rotation = Quaternion.identity;
                    go.name = "CrunchEngine";
                    go.AddComponent<CrunchEngine>();
                    crunchEngine = go.GetComponent<CrunchEngine>();
                    hasCrunchEngine = true;
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        private void Draw_Toolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(position.width));
            if (sceneTab)
            {
                sceneDisplayObjectTypes = (SceneDisplayObjectTypes)EditorGUILayout.EnumPopup(sceneDisplayObjectTypes);
                if (GUILayout.Button("Clear Console", EditorStyles.toolbarButton)) { Debug.Log(Utilities.Log.Clear()); }
            }
            else if (assetsTab)
            {

            }
            GUILayout.EndHorizontal();
        }

        private void Draw_TabOptions()
        {
            GUILayout.BeginVertical(EditorStyles.toolbar);
            if (GUILayout.Button("Close Window", EditorStyles.toolbarButton)) { CrunchWindow window = GetWindow<CrunchWindow>(); window.Close(); }
            GUILayout.EndVertical();

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

        private void Draw_SceneTab()
        {
            if (!sceneTab)
                return;

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
            Light[] lights = GameObject.FindObjectsOfType<Light>();

            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label("Scene", EditorStyles.boldLabel);
            GUILayout.Space(12);
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            scene_SearchParameter = EditorGUILayout.TextField(scene_SearchParameter);
            GUILayout.Label(Resources.Load("Textures/search") as Texture, GUILayout.MaxWidth(iconDimensions - 4), GUILayout.MaxHeight(iconDimensions - 4));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();            
            GUILayout.EndVertical();

            if (!string.IsNullOrEmpty(scene_SearchParameter))
            {                
                GUILayout.BeginVertical("Box");
                GUILayout.Label("Search Results", EditorStyles.boldLabel);
                searchScrollPosition = GUILayout.BeginScrollView(searchScrollPosition, false, false, GUILayout.MaxHeight(78));                

                bool returnedResult = false;
                int i = 0;
                foreach (GameObject item in go)
                {
                    if(item.name.ToLower().Contains(scene_SearchParameter.ToString().ToLower()))
                    {
                        i++;
                        GUILayout.BeginHorizontal();                        
                        GUILayout.Label(i.ToString() + ".", GUILayout.MaxWidth(16));                        
                        DrawList(item, () => { });
                        GUILayout.EndHorizontal();
                        returnedResult = true;
                    }
                }

                if(!returnedResult)
                {
                    GUILayout.Label("No results.");
                }

                GUILayout.EndScrollView();
                GUILayout.EndVertical();               
            }            

            mainScrollPosition = GUILayout.BeginScrollView(mainScrollPosition, false, false);
            GUILayout.BeginVertical("Box");
            GUILayout.Label("Overview", EditorStyles.boldLabel);
            GUILayout.Label(go.Length + " - GameObjects");
            GUILayout.Label(meshRenderers.Length + " - Mesh Renderers");
            GUILayout.Label(skinnedMeshRenderers.Length + " - Skinned Mesh Renderers");
            GUILayout.Label(lights.Length + " - Lights");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            GUILayout.Label("Inspection", EditorStyles.boldLabel);
            GUILayout.BeginVertical();

            scene_FoldoutUIElements = EditorGUILayout.Foldout(scene_FoldoutUIElements, ui.Length + " - UI Elements", true);
            if (scene_FoldoutUIElements)
            {
                EditorGUI.indentLevel++;
                scene_FoldoutTexts = EditorGUILayout.Foldout(scene_FoldoutTexts, texts.Length + " - Text", true);
                if (scene_FoldoutTexts)
                {
                    EditorGUI.indentLevel++;
                    EditorGUI.indentLevel++;
                    foreach (Text item in texts)
                    {
                        DrawList(item.gameObject, () => { GUILayout.BeginVertical(); item.text = EditorGUILayout.TextField(item.text); item.color = EditorGUILayout.ColorField(item.color); GUILayout.EndVertical(); }, 3);
                    }
                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;
                }

                scene_FoldoutImages = EditorGUILayout.Foldout(scene_FoldoutImages, images.Length + " - Images", true);
                if (scene_FoldoutImages)
                {
                    EditorGUI.indentLevel++;
                    EditorGUI.indentLevel++;
                    foreach (Image item in images)
                    {
                        DrawList(item.gameObject, () => { }, 3);
                    }
                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;
                }

                scene_FoldoutButtons = EditorGUILayout.Foldout(scene_FoldoutButtons, buttons.Length + " - Buttons", true);
                if (scene_FoldoutButtons)
                {
                    EditorGUI.indentLevel++;
                    EditorGUI.indentLevel++;
                    foreach (Button item in buttons)
                    {
                        DrawList(item.gameObject, () => { }, 3);
                    }
                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;
                }

                scene_FoldoutInputFields = EditorGUILayout.Foldout(scene_FoldoutInputFields, inputFields.Length + " - Input Fields", true);
                if (scene_FoldoutInputFields)
                {
                    EditorGUI.indentLevel++;
                    EditorGUI.indentLevel++;
                    foreach (InputField item in inputFields)
                    {
                        DrawList(item.gameObject, () => { }, 3);
                    }
                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }

            scene_FoldoutAudioSource = EditorGUILayout.Foldout(scene_FoldoutAudioSource, audioSource.Length + " - Audio Sources", true);
            if (scene_FoldoutAudioSource)
            {

            }


            GUILayout.EndVertical();
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            GUILayout.Label("Favourites", EditorStyles.boldLabel);

            if (scene_FavouriteObjects.Count > 0)
            {
                foreach (int item in scene_FavouriteObjects)
                {
                    GameObject _go = EditorUtility.InstanceIDToObject(item) as GameObject;

                    if (_go == null)
                        continue;

                    DrawList(_go, () => { });
                }
            }
            else
            {
                GUILayout.Label("No favourites.");
            }


            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        private void Draw_AssetsTab()
        {
            if (!assetsTab)
                return;

            GUILayout.BeginVertical("Box");
            GUILayout.Label("Assets", EditorStyles.boldLabel);
            GUILayout.EndVertical();

            mainScrollPosition = GUILayout.BeginScrollView(mainScrollPosition, false, false);

            #region Engine

            GUILayout.BeginVertical("Box");

            GUILayout.Label("Engine", EditorStyles.boldLabel);            

            //GUILayout.BeginHorizontal();
            //GUILayout.Space(20);
            //GUILayout.Label("Crunch Engine");
            //GUILayout.FlexibleSpace();
            //CrunchEngine boop = (CrunchEngine)EditorGUILayout.ObjectField(crunchEngine, typeof(CrunchEngine), false);
            //GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("Current Preset");
            GUILayout.FlexibleSpace();
            crunchEngine.CurrentData = (CrunchEngineData)EditorGUILayout.ObjectField(crunchEngine.CurrentData, typeof(CrunchEngineData), false);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            #endregion

            #region UI
            GUILayout.BeginVertical("Box");

            GUILayout.Label("UI", EditorStyles.boldLabel);

            GUILayout.Label("Default Sound Events");

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("On Mouse Enter");
            GUILayout.FlexibleSpace();
            crunchEngine.CurrentData.UISoundEvent_MouseEnter = (AudioClip)EditorGUILayout.ObjectField(crunchEngine.CurrentData.UISoundEvent_MouseEnter, typeof(AudioClip), false);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("On Mouse Down");
            GUILayout.FlexibleSpace();
            crunchEngine.CurrentData.UISoundEvent_MouseDown = (AudioClip)EditorGUILayout.ObjectField(crunchEngine.CurrentData.UISoundEvent_MouseDown, typeof(AudioClip), false);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("On Mouse Up");
            GUILayout.FlexibleSpace();
            crunchEngine.CurrentData.UISoundEvent_MouseUp = (AudioClip)EditorGUILayout.ObjectField(crunchEngine.CurrentData.UISoundEvent_MouseUp, typeof(AudioClip), false);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("On Mouse Exit");
            GUILayout.FlexibleSpace();
            crunchEngine.CurrentData.UISoundEvent_MouseExit = (AudioClip)EditorGUILayout.ObjectField(crunchEngine.CurrentData.UISoundEvent_MouseExit, typeof(AudioClip), false);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
#endregion
            GUILayout.EndScrollView();
        }

        private void Draw_UtilitiesTab()
        {
            if (!utilitiesTab)
                return;

            GUILayout.BeginVertical("Box");
            GUILayout.Label("Utilities", EditorStyles.boldLabel);
            GUILayout.EndVertical();

            mainScrollPosition = GUILayout.BeginScrollView(mainScrollPosition, false, false);
            GUILayout.BeginVertical("Box");
            GUILayout.Label("UI", EditorStyles.boldLabel);
            if (GUILayout.Button("Create Window"))
            {
                Crunch.Engine.Editor.UI.CreateNewWindow();
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        private void OnGUI()
        {
            CrunchEngine ce = GameObject.FindObjectOfType<CrunchEngine>();
            if (ce == null)
            {
                hasCrunchEngine = false;
                hasCurrentData = false;
                //currentData = null;
                crunchEngine = null;
            }
            else
            {
                crunchEngine = ce;
                hasCrunchEngine = true;

                if (crunchEngine.CurrentData != null)
                {
                    hasCurrentData = true;
                    //currentData = ce.CurrentData;
                }
                else
                {
                    hasCurrentData = false;
                    //currentData = null;
                }
            }

            Draw();

            //if (GameObject.FindObjectOfType<CrunchEngine>() == null) hasPreset = false;
            //else hasPreset = true;
        }

        private void DrawList(GameObject item, Action action)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            if (Selection.activeGameObject == item.gameObject)
            {
                EditorGUILayout.ToggleLeft("", true, GUILayout.MaxWidth(16));
            }

            item.name = EditorGUILayout.TextField(item.name);
            if (Selection.activeGameObject != item.gameObject)
            {
                if (GUILayout.Button(Resources.Load("Textures/select01") as Texture, GUILayout.MaxWidth(iconDimensions), GUILayout.MaxHeight(iconDimensions)))
                {
                    Selection.activeGameObject = item.gameObject;
                }
            }
            else
            {
                if (GUILayout.Button(Resources.Load("Textures/focus01") as Texture, GUILayout.MaxWidth(iconDimensions), GUILayout.MaxHeight(iconDimensions)))
                {
                    Selection.activeGameObject = item.gameObject;
                    EditorApplication.ExecuteMenuItem("Edit/Frame Selected");
                }
            }
            if (!scene_FavouriteObjects.Contains(item.gameObject.GetInstanceID()))
            {
                if (GUILayout.Button(Resources.Load("Textures/favourite") as Texture, GUILayout.MaxWidth(iconDimensions), GUILayout.MaxHeight(iconDimensions)))
                {
                    scene_FavouriteObjects.Add(item.gameObject.GetInstanceID());
                    SaveFavourites();
                }
            }
            else
            {
                if (GUILayout.Button(Resources.Load("Textures/unfavourite") as Texture, GUILayout.MaxWidth(iconDimensions), GUILayout.MaxHeight(iconDimensions)))
                {
                    scene_FavouriteObjects.Remove(item.gameObject.GetInstanceID());
                    SaveFavourites();
                }
            }
            if (GUILayout.Button(Resources.Load("Textures/delete") as Texture, GUILayout.MaxWidth(iconDimensions), GUILayout.MaxHeight(iconDimensions)))
            {
                Undo.RecordObject(this, "Delete " + item.name);
                DestroyImmediate(item.gameObject);
            }

            if (Scene_ShowTypes) action();

            GUILayout.EndHorizontal();
            GUILayout.Space(4);            
            GUILayout.EndVertical();
        }

        private void DrawList(GameObject item, Action action, int indent)
        {            
            if (Scene_ShowTypes)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20 * indent);
                GUILayout.BeginHorizontal("Box");
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }
            else
            {
                GUILayout.BeginHorizontal();
            }

            if (Selection.activeGameObject == item.gameObject)
            {
                Color def = GUI.color;
                GUI.color = Color.green;
                item.name = EditorGUILayout.TextField(item.name);
                GUI.color = def;
            }
            else
            {
                item.name = EditorGUILayout.TextField(item.name);
            }
            if (Scene_ShowTypes) action();
            if (Selection.activeGameObject != item.gameObject)
            {
                if (GUILayout.Button(Resources.Load("Textures/select01") as Texture, GUILayout.MaxWidth(iconDimensions), GUILayout.MaxHeight(iconDimensions)))
                {
                    Selection.activeGameObject = item.gameObject;
                }
            }
            else
            {
                if (GUILayout.Button(Resources.Load("Textures/focus01") as Texture, GUILayout.MaxWidth(iconDimensions), GUILayout.MaxHeight(iconDimensions)))
                {
                    Selection.activeGameObject = item.gameObject;
                    EditorApplication.ExecuteMenuItem("Edit/Frame Selected");
                }
            }
            if (item.activeSelf)
            {
                if (GUILayout.Button(Resources.Load("Textures/enabled") as Texture, GUILayout.MaxWidth(iconDimensions), GUILayout.MaxHeight(iconDimensions)))
                {
                    item.SetActive(false);
                }
            }
            else
            {
                if (GUILayout.Button(Resources.Load("Textures/disabled") as Texture, GUILayout.MaxWidth(iconDimensions), GUILayout.MaxHeight(iconDimensions)))
                {
                    item.SetActive(true);
                }
            }
            if (!scene_FavouriteObjects.Contains(item.gameObject.GetInstanceID()))
            {
                if (GUILayout.Button(Resources.Load("Textures/favourite") as Texture, GUILayout.MaxWidth(iconDimensions), GUILayout.MaxHeight(iconDimensions)))
                {
                    scene_FavouriteObjects.Add(item.gameObject.GetInstanceID());
                    SaveFavourites();
                }
            }
            else
            {
                if (GUILayout.Button(Resources.Load("Textures/unfavourite") as Texture, GUILayout.MaxWidth(iconDimensions), GUILayout.MaxHeight(iconDimensions)))
                {
                    scene_FavouriteObjects.Remove(item.gameObject.GetInstanceID());
                    SaveFavourites();
                }
            }
            if (GUILayout.Button(Resources.Load("Textures/delete") as Texture, GUILayout.MaxWidth(iconDimensions), GUILayout.MaxHeight(iconDimensions)))
            {
                Undo.RecordObject(this, "Delete " + item.name);
                DestroyImmediate(item.gameObject);
            }

            if (Scene_ShowTypes)
            {
                GUILayout.EndHorizontal();
                GUILayout.Space(20);
                GUILayout.EndHorizontal();

                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;
            }
            else
            {
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(4);
        }

        private void ReadFavourites()
        {
            string text = "";
            if (File.Exists(Application.dataPath + "/Megabite Studios/Crunch/Resources/Data/crunch_favourites.txt"))
            {
                text = File.ReadAllText(Application.dataPath + "/Megabite Studios/Crunch/Resources/Data/crunch_favourites.txt");

                string[] parse = text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string stri in parse)
                {
                    scene_FavouriteObjects.Add(int.Parse(stri));
                }
            }
        }

        private void SaveFavourites()
        {
            string text = "";
            foreach (int item in scene_FavouriteObjects)
            {
                text += item.ToString() + ";";
            }
            File.WriteAllText(Application.dataPath + "/Megabite Studios/Crunch/Resources/Data/crunch_favourites.txt", text);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }     

        //private void HorizontalIndent(int indent, Action action)
        //{
        //    GUILayout.BeginHorizontal();
        //    GUILayout.Space(20 * indent);
        //    GUILayout.BeginHorizontal("Box");
        //    action();
        //    GUILayout.EndHorizontal();
        //    GUILayout.Space(20 * indent);
        //    GUILayout.EndHorizontal();

        //}
    }
}
