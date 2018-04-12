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
        private CrunchEngine crunchEngine;
        private bool hasCurrentData, hasCrunchEngine = false;        

        private bool sceneTab, engineTab, utilitiesTab, consoleTab;        
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

        [MenuItem("Crunch/Open Engine", false, 0)]
        private static void Open()
        {
            CrunchWindow window = GetWindow<CrunchWindow>();
            window.Show();
            window.InitializeWindow();
        }

        // Called when the window is opened
        private void InitializeWindow()
        {            
            position = new Rect(Screen.width - (Screen.width / 2) - 256, Screen.height + (Screen.height / 2) + 256, 512, 512);
            minSize = new Vector2(384, 256);
            sceneTab = true;
            ReadFavourites();
        }

        private void OnEnable()
        {            
            titleContent.text = "Crunch Engine";
            CheckEngineInitialization();
        }

        // Check if we have "enabled" CrunchEngine (i.e, Created the instance object and assigned an Engine Preset)
        private void CheckEngineInitialization()
        {
            CrunchEngine engine = GameObject.FindObjectOfType<CrunchEngine>();
            if (engine == null)
            {
                hasCrunchEngine = false;
                hasCurrentData = false;                
                crunchEngine = null;                
            }
            else
            {
                crunchEngine = engine;
                hasCrunchEngine = true;

                if (crunchEngine.CurrentData != null)
                {
                    hasCurrentData = true;                    
                }
                else
                {
                    hasCurrentData = false;
                }
            }
        }

        // Called each Unity Editor GUI Update (i.e, every time an event happens on this editor window)
        private void OnGUI()
        {
            // Check the engine initialization each time the window updates
            CheckEngineInitialization();           

            // Draw all content to the window, depending on the values selected
            Draw();
        }

        // Draw all content to the window, depending on the values selected
        private void Draw()
        {
            Draw_Toolbar();

            if (hasCurrentData)
            {
                Draw_SecondaryToolbar();
                Draw_SceneTab();
                Draw_EngineTab();
                Draw_UtilitiesTab();
            }
            else
            {
                Draw_Initialization();
            }
        }

        // Draw the Initialization screen if the engine hasn't yet been "enabled"
        private void Draw_Initialization()
        {
            VerticalBox(() =>
            {
                Label("Crunch Engine", true);
            });

            ScrollBox(() =>
            {
                VerticalBox(() =>
                {
                    Label("Initialize", true);

                    if (hasCrunchEngine)
                    {
                        if (!hasCurrentData)
                        {
                            EditorGUILayout.LabelField("You haven't initialized your current Preset yet. If you have not created one yet, please do so using the button. If you already have a Preset, assign it in the field.");
                            EditorStyles.label.wordWrap = true;

                            Horizontal(() =>
                            {
                                GUILayout.Space(20);
                                Label("Create New Preset: ", false);
                                GUILayout.FlexibleSpace();
                                if (GUILayout.Button("Create New"))
                                {
                                    CrunchEngineData data = ScriptableObject.CreateInstance<CrunchEngineData>();
                                    AssetDatabase.CreateAsset(data, "Assets/Megabite Studios/Crunch/Presets/New Preset.asset");
                                    AssetDatabase.SaveAssets();
                                    EditorUtility.FocusProjectWindow();
                                    Selection.activeObject = data;

                                    crunchEngine.AssignEngineData(data);
                                    hasCurrentData = true;
                                }
                            });

                            Horizontal(() =>
                            {
                                GUILayout.Space(20);
                                Label("Or Assign Existing: ", false);
                                GUILayout.FlexibleSpace();
                                EditorGUI.BeginChangeCheck();
                                crunchEngine.CurrentData = (CrunchEngineData)EditorGUILayout.ObjectField(crunchEngine.CurrentData, typeof(CrunchEngineData), false);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    if (crunchEngine.CurrentData != null)
                                    {
                                        hasCurrentData = true;

                                        if (!hasCrunchEngine)
                                        {
                                            GameObject go = new GameObject();
                                            go.transform.position = Vector3.zero;
                                            go.transform.localScale = Vector3.one;
                                            go.transform.rotation = Quaternion.identity;
                                            go.name = "CrunchEngine";
                                            crunchEngine = go.AddComponent<CrunchEngine>();
                                            hasCrunchEngine = true;
                                        }
                                    }
                                }
                            });
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField("You haven't initialized Crunch Engine yet. Use the button below to begin.");
                        EditorStyles.label.wordWrap = true;
                        EditorGUILayout.LabelField("NOTE: You will receive this message if the CrunchEngine GameObject is inactive.");
                        EditorStyles.label.wordWrap = true;                        

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

                });
            });          
        }

        // Draws the toolbar options at the top of the window (Scene, Assets, Utilities etc...)
        private void Draw_Toolbar()
        {
            Vertical(() =>
            {
                if (GUILayout.Button("Close Window", EditorStyles.toolbarButton)) { CrunchWindow window = GetWindow<CrunchWindow>(); window.Close(); }
            }, EditorStyles.toolbar);

            Vertical(() =>
            {
                Horizontal(() =>
                {
                    if (GUILayout.Button("Scene", EditorStyles.toolbarButton))
                    {
                        sceneTab = true;
                        engineTab = false;
                        utilitiesTab = false;
                        consoleTab = false;
                    }

                    if (GUILayout.Button("Engine", EditorStyles.toolbarButton))
                    {
                        sceneTab = false;
                        engineTab = true;
                        utilitiesTab = false;
                        consoleTab = false;
                    }

                    if (GUILayout.Button("Utilities", EditorStyles.toolbarButton))
                    {
                        sceneTab = false;
                        engineTab = false;
                        utilitiesTab = true;
                        consoleTab = false;
                    }

                    //if (GUILayout.Button("Console", EditorStyles.toolbarButton))
                    //{
                    //    sceneTab = false;
                    //    assetsTab = false;
                    //    utilitiesTab = false;
                    //    consoleTab = true;
                    //}
                });
            }, EditorStyles.toolbar);
        }

        // Draws secondary toolbar options for each individual tab
        private void Draw_SecondaryToolbar()
        {
            if (sceneTab)
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(position.width));
                sceneDisplayObjectTypes = (SceneDisplayObjectTypes)EditorGUILayout.EnumPopup(sceneDisplayObjectTypes);
                if (GUILayout.Button("Clear Console", EditorStyles.toolbarButton)) { Debug.Log(Utilities.Log.Clear()); }
                GUILayout.EndHorizontal();
            }
        }

        #region Tabs

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

            VerticalBox(() =>
            {
                Horizontal(() =>
                {
                    Vertical(() =>
                    {
                        GUILayout.Label("Scene", EditorStyles.boldLabel);
                        GUILayout.Space(12);
                    });

                    Vertical(() =>
                    {
                        GUILayout.Space(6);
                        Horizontal(() =>
                        {
                            scene_SearchParameter = EditorGUILayout.TextField(scene_SearchParameter);
                            GUILayout.Label(Resources.Load("Textures/search") as Texture, GUILayout.MaxWidth(iconDimensions - 4), GUILayout.MaxHeight(iconDimensions - 4));
                        });
                    });
                });
            });          

            if (!string.IsNullOrEmpty(scene_SearchParameter))
            {
                VerticalBox(() =>
                {                
                    Label("Search Results", true);
                    searchScrollPosition = GUILayout.BeginScrollView(searchScrollPosition, false, false, GUILayout.MaxHeight(78));

                    bool returnedResult = false;
                    int i = 0;
                    foreach (GameObject item in go)
                    {
                        if (item.name.ToLower().Contains(scene_SearchParameter.ToString().ToLower()))
                        {
                            i++;                            
                            Horizontal(() =>
                            {
                                GUILayout.Label(i.ToString() + ".", GUILayout.MaxWidth(16));
                                DrawList(item, () => { });
                            });                            
                            returnedResult = true;
                        }
                    }

                    if (!returnedResult)
                    {
                        Label("No results.", false);
                    }

                    GUILayout.EndScrollView();
                });                            
            }        
            
            ScrollBox(() =>
            {
                VerticalBox(() =>
                {
                    GUILayout.Label("Overview", EditorStyles.boldLabel);
                    GUILayout.Label(go.Length + " - GameObjects");
                    GUILayout.Label(meshRenderers.Length + " - Mesh Renderers");
                    GUILayout.Label(skinnedMeshRenderers.Length + " - Skinned Mesh Renderers");
                    GUILayout.Label(lights.Length + " - Lights");
                });

                VerticalBox(() =>
                {
                    Label("Inspection", true);
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
                });

                VerticalBox(() =>
                {
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
                });
            });                       
        }

        private void Draw_EngineTab()
        {            
            if (!engineTab)
                return;
            
            VerticalBox(() =>
            {
                Label("Engine", true);
            });            
            
            ScrollBox(() =>
            {
                #region Engine                
                VerticalBox(() =>
                {
                    Label("Preset", true);

                    Horizontal(() =>
                    {                        
                        Label("Current Preset", false);
                        GUILayout.FlexibleSpace();
                        crunchEngine.CurrentData = (CrunchEngineData)EditorGUILayout.ObjectField(crunchEngine.CurrentData, typeof(CrunchEngineData), false);
                    });              
                });
                #endregion

                VerticalBox(() =>
                {
                    Label("Systems", true);
                    crunchEngine.CurrentData.usingLoadingSystem = GUILayout.Toggle(crunchEngine.CurrentData.usingLoadingSystem, "Enable Loading System");
                    crunchEngine.CurrentData.usingMusicSystem = GUILayout.Toggle(crunchEngine.CurrentData.usingMusicSystem, "Enable Music System");
                    crunchEngine.CurrentData.usingSoundEffectSystem = GUILayout.Toggle(crunchEngine.CurrentData.usingSoundEffectSystem, "Enable Sound Effect System");
                    crunchEngine.CurrentData.usingPoolingSystem = GUILayout.Toggle(crunchEngine.CurrentData.usingPoolingSystem, "Enable Pooling System");
                });

                #region UI                
                VerticalBox(() =>
                {
                    Label("UI", true);

                    Label("Default Sound Events", false);

                    Horizontal(() =>
                    {
                        GUILayout.Space(20);
                        Label("On Mouse Enter", false);
                        GUILayout.FlexibleSpace();
                        crunchEngine.CurrentData.UISoundEvent_MouseEnter = (AudioClip)EditorGUILayout.ObjectField(crunchEngine.CurrentData.UISoundEvent_MouseEnter, typeof(AudioClip), false);
                    });

                    Horizontal(() =>
                    {
                        GUILayout.Space(20);
                        Label("On Mouse Down", false);
                        GUILayout.FlexibleSpace();
                        crunchEngine.CurrentData.UISoundEvent_MouseDown = (AudioClip)EditorGUILayout.ObjectField(crunchEngine.CurrentData.UISoundEvent_MouseDown, typeof(AudioClip), false);
                    });

                    Horizontal(() =>
                    {
                        GUILayout.Space(20);
                        Label("On Mouse Up", false);
                        GUILayout.FlexibleSpace();
                        crunchEngine.CurrentData.UISoundEvent_MouseUp = (AudioClip)EditorGUILayout.ObjectField(crunchEngine.CurrentData.UISoundEvent_MouseUp, typeof(AudioClip), false);
                    });

                    Horizontal(() =>
                    {
                        GUILayout.Space(20);
                        Label("On Mouse Exit", false);
                        GUILayout.FlexibleSpace();
                        crunchEngine.CurrentData.UISoundEvent_MouseExit = (AudioClip)EditorGUILayout.ObjectField(crunchEngine.CurrentData.UISoundEvent_MouseExit, typeof(AudioClip), false);
                    });
                });         
                #endregion
            });            
        }

        private void Draw_UtilitiesTab()
        {
            if (!utilitiesTab)
                return;

            VerticalBox(() =>
            {
                Label("Utilities", true);
            });
            
            ScrollBox(() =>
            {
                VerticalBox(() =>
                {
                    Label("UI", true);
                    if (GUILayout.Button("Create Window"))
                    {
                        Crunch.Engine.Editor.UI.CreateNewWindow();
                    }
                });
            });

        }

        #endregion

        #region Shorthand Layout Method

        // Displays a label in the window, with the option to make it bold
        private void Label(string content, bool bold)
        {
            if (bold)
                GUILayout.Label(content, EditorStyles.boldLabel);
            else
                GUILayout.Label(content);
        }

        // Displays all content within in a horizontal layout group
        private void Horizontal(Action content)
        {
            GUILayout.BeginHorizontal();
            content();
            GUILayout.EndHorizontal();
        }

        // Displays all content within in a horizontal layout group, surrounded by a box
        private void HorizontalBox(Action content)
        {
            GUILayout.BeginHorizontal("Box");
            content();
            GUILayout.EndHorizontal();
        }

        // Displays all content within in a vertical layout group
        private void Vertical(Action content)
        {
            GUILayout.BeginVertical();
            content();
            GUILayout.EndVertical();
        }

        // Displays all content within in a vertical layout group, surrounded by a box
        private void VerticalBox(Action content)
        {
            GUILayout.BeginVertical("Box");
            content();
            GUILayout.EndVertical();
        }

        // Displays all content within in a horizontal layout group, with layout option parameters
        private void Vertical(Action content, params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(options);
            content();
            GUILayout.EndVertical();
        }

        // Displays all content within in a horizontal layout group, with a specified GUIStyle
        private void Vertical(Action content, GUIStyle style)
        {
            GUILayout.BeginVertical(style);
            content();
            GUILayout.EndVertical();
        }

        // Surrounds the content within inside a scroll box. NOTE: This is used for main tabs only.
        private void ScrollBox(Action content)
        {
            mainScrollPosition = GUILayout.BeginScrollView(mainScrollPosition, false, false);
            content();
            GUILayout.EndScrollView();
        }

        #endregion

        #region Favourites

        // Reads the favourite GameObjects the developer has chosen. 
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

        // Saves the favourite GameObjects the developer has chosen.
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

        #endregion

        #region DrawList Methods

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

        #endregion
    }
}
