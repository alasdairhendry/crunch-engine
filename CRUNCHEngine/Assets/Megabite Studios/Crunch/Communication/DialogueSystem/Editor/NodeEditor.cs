using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Crunch.Communication
{
	public class NodeEditor : EditorWindow 
	{
        private Dialogue dialogue;
        private Rect parentRect = new Rect(-512, -512, 1024, 1024);
        private List<NodeBase> windows = new List<NodeBase>();

        private bool isConnecting = false;
        private int connectingFrom = -1;
        private int connectingFromOption = -1;
        
		private void OnEnable()
        {
            titleContent.text = "Dialogue Editor";
        }

        public void SetDialogue(Dialogue dialogue)
        {
            this.dialogue = dialogue;
            titleContent.text = "Dialogue - " + dialogue.gameObject.name;
            CreateInitialWindows();
        }

        private void CreateInitialWindows()
        {
            NodeBase start = CreateInitialWindow(NodeBase.Type.Start);
            start.start = dialogue.start;

            NodeBase end = CreateInitialWindow(NodeBase.Type.End);
            end.end = dialogue.end;

            foreach (DialogueSpeech item in dialogue.speechEntries)
            {
                NodeBase speech = CreateInitialWindow(NodeBase.Type.Speech);
                speech.speech = dialogue.speechEntries[dialogue.speechEntries.IndexOf(item)];
            }

            foreach (DialogueOptions item in dialogue.optionsEntries)
            {
                NodeBase option = CreateInitialWindow(NodeBase.Type.Option);
                option.options = dialogue.optionsEntries[dialogue.optionsEntries.IndexOf(item)];
            }

            LayoutInitialWindows();
        }

        private void LayoutInitialWindows()
        {
            int windowCount = windows.Count;
            int y = 32;
            int x = 0;

            for (int i = 0; i < windowCount; i++)
            {                
                if(i % 3 == 0 && i !=0)
                {
                    y += 288;
                    x = 0;
                }

                windows[i].position = new Vector2(x * 256 + 32, y);
                x++;
            }
        }

        private void OnGUI()
        {
            if (GUILayout.Button("^"))
            {
                foreach (NodeBase item in windows)
                {
                    item.size += new Vector2(item.rect.size.x / 10, item.rect.size.y / 10);
                }
            }
            if (GUILayout.Button("v"))
            {
                foreach (NodeBase item in windows)
                {
                    item.size -= new Vector2(item.rect.size.x / 10, item.rect.size.y / 10);
                }
            }
            Event e = Event.current;
            foreach (NodeBase item in windows)
            {
                item.DrawConnections(windows);
            }

            if(e.type == EventType.KeyDown)
            {
                if(e.keyCode == KeyCode.Escape)
                {
                    isConnecting = false;
                }
            }

            if (e.button == 1)
            {
                if (e.type == EventType.MouseDown)
                {
                    Generic();
                }
            }

            DrawNodes();
            DragWindows();
        }

        private void DrawNodes()
        {
            BeginWindows();

            foreach (NodeBase item in windows)
            {
                int index = windows.IndexOf(item);
                GUI.Window(index, item.rect, DrawNodeWindow, windows[index].windowName);
            }

            EndWindows();
        }

        private void DragWindows()
        {
            Event e = Event.current;

            if (e.isMouse)
            {
                if (e.type == EventType.MouseDrag)
                {
                    if (e.button == 0)
                    {
                        foreach (NodeBase item in windows)
                        {
                            if (item.rect.Contains(e.mousePosition))
                            {
                                item.position += e.delta;
                                Repaint();
                                return;
                            }
                        }

                        foreach (NodeBase item in windows)
                        {
                            item.position += e.delta;
                        }
                        Repaint();
                    }
                }
            }
        }

        private void DrawNodeWindow(int index)
        {
            //GUI.DragWindow();

            DrawGUI_StartNode(index);
            DrawGUI_SpeechNode(index);
            DrawGUI_OptionsNode(index);
            DrawGUI_EndNode(index);
        }

        private void DrawGUI_StartNode(int index)
        {
            NodeBase window = windows[index];

            if (window.type == NodeBase.Type.Start)
            {
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Output - ");
                GUILayout.EndHorizontal();

                window.outputPositions[0] = window.position + new Vector2(window.rect.width, window.rect.height - 10);

                if (!isConnecting)
                {
                    if (GUILayout.Button("Change Output"))
                    {
                        isConnecting = true;
                        connectingFrom = index;
                    }
                }
                else
                {
                    if(GUILayout.Button("X"))
                    {
                        isConnecting = false;
                        window.DeleteConnection(connectingFromOption);
                        Repaint();
                    }
                }
            }
        }

        private void DrawGUI_SpeechNode(int index)
        {
            NodeBase window = windows[index];

            if (window.type == NodeBase.Type.Speech)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Name", EditorStyles.boldLabel);
                window.speech.name = EditorGUILayout.TextField(window.speech.name);
                EditorGUILayout.EndHorizontal();

                GUILayout.Label("Speech", EditorStyles.boldLabel);
                window.speech.speech = EditorGUILayout.TextArea(window.speech.speech);

                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Output - ");
                GUILayout.EndHorizontal();

                window.outputPositions[0] = window.position + new Vector2(window.rect.width, window.rect.height - 10);

                if (!isConnecting)
                {
                    if (GUILayout.Button("O -"))
                    {
                        isConnecting = true;
                        connectingFrom = index;
                    }
                }

                if (isConnecting && connectingFrom != index)
                {
                    if(GUILayout.Button("Connect to me!"))
                    {
                        windows[connectingFrom].ConnectTo(index, windows, connectingFromOption);
                        isConnecting = false;
                        connectingFrom = -1;
                    }
                }
                else if(isConnecting && connectingFrom == index)
                {
                    if(GUILayout.Button("X"))
                    {
                        isConnecting = false;
                        window.DeleteConnection(connectingFromOption);
                        Repaint();
                    }
                }
            }
        }

        private void DrawGUI_OptionsNode(int index)
        {
            NodeBase window = windows[index];

            if (window.type == NodeBase.Type.Option)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Name", EditorStyles.boldLabel);
                window.options.name = EditorGUILayout.TextField(window.options.name);
                EditorGUILayout.EndHorizontal();

                GUILayout.Label("Options", EditorStyles.boldLabel);
                List<Vector2> locations = new List<Vector2>();
                foreach (string item in window.options.GetOptions)
                {
                    EditorGUILayout.BeginHorizontal();
                    window.options.options[window.options.GetOptions.IndexOf(item)].option = EditorGUILayout.TextField(window.options.GetOptions[window.options.GetOptions.IndexOf(item)]);
                    if (!isConnecting)
                    {
                        if (GUILayout.Button("O -"))
                        {
                            isConnecting = true;
                            connectingFrom = index;
                            connectingFromOption = window.options.GetOptions.IndexOf(item);
                            Debug.Log(connectingFromOption);
                        }
                    }
                    else
                    {
                        if(connectingFrom == index)
                        {
                            if(connectingFromOption == window.options.GetOptions.IndexOf(item))
                            {
                                if(GUILayout.Button("X"))
                                {
                                    window.DeleteConnection(connectingFromOption);
                                    isConnecting = false;
                                    Repaint();
                                }
                            }
                        }
                    }
                    locations.Add(window.position + new Vector2(window.rect.width, 15));
                    EditorGUILayout.EndHorizontal();
                }
                if (window.options.options.Count < 6)
                {
                    if (GUILayout.Button("Add Option"))
                    {
                        window.options.options.Add(new Backend.DialogueOption("New Option"));
                        Repaint();
                    }
                }
                //Debug.Log(locations.Count);
                window.outputPositions = locations;
                //Debug.Log(window.outputPositions.Count);

                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Output - ");
                GUILayout.EndHorizontal();

                if (isConnecting && connectingFrom != index)
                {
                    if (GUILayout.Button("Connect to me!"))
                    {
                        windows[connectingFrom].ConnectTo(index, windows, connectingFromOption);
                        isConnecting = false;
                        connectingFrom = -1;
                    }
                }
            }
        }

        private void DrawGUI_EndNode(int index)
        {
            NodeBase window = windows[index];

            if (window.type == NodeBase.Type.End)
            {
                if (isConnecting && connectingFrom != index)
                {
                    if (GUILayout.Button("Connect to me!"))
                    {
                        windows[connectingFrom].ConnectTo(index, windows, connectingFromOption);
                        isConnecting = false;
                        connectingFrom = -1;
                    }
                }
            }
        }

        private NodeBase CreateInitialWindow(NodeBase.Type type)
        {
            NodeBase window = new NodeBase(type, this);
            windows.Add(window);
            return window;
        }

        private NodeBase CreateWindow(NodeBase.Type type)
        {
            NodeBase window = new NodeBase(type, this);
            windows.Add(window);

            if(type == NodeBase.Type.Speech)
            {
                DialogueSpeech speech = dialogue.AddSpeech("New Speech");
                window.speech = speech;
            }
            else if(type == NodeBase.Type.Option)
            {
                DialogueOptions option = dialogue.AddOptions("New Option");
                window.options = option;
            }

            return window;
        }

        private void Generic()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add Speech Node"), false, () => { CreateWindow(NodeBase.Type.Speech); });
            menu.AddItem(new GUIContent("Add Options Node"), false, () => { CreateWindow(NodeBase.Type.Option); });
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Clear All"), false, () => { dialogue.speechEntries.Clear(); dialogue.optionsEntries.Clear(); windows.Clear(); CreateInitialWindows(); });
            menu.ShowAsContext();
        }
	}

    public class NodeBase
    {
        private Rect m_Rect = new Rect(0, 0, 96, 256);
        public Rect rect { get { return m_Rect; } set { m_Rect = value; } }
        public Vector2 position { get { return m_Rect.position; } set { m_Rect.position = value; } }
        public Vector2 size { get { return m_Rect.size; } set { m_Rect.size = value; } }

        public enum Type { Start, Speech, Option, End }
        private Type m_Type = Type.Start;
        public Type type { get { return m_Type; } }

        private string m_WindowName = "New Window";
        public string windowName { get { return m_WindowName; } }

        public List<Vector2> outputPositions = new List<Vector2>();

        public DialogueStart start;
        public DialogueSpeech speech;
        public DialogueOptions options;
        public List<Rect> optionButtons = new List<Rect>();
        public DialogueEnd end;

        public NodeBase(Type _type, NodeEditor window)
        {
            m_Type = _type;

            if (_type == Type.Start)
            {
                Vector2 midpoint = new Vector2(window.position.size.x / 2, window.position.size.y / 2);
                m_Rect = new Rect((int)(midpoint.x - 64), (int)(midpoint.y - 32), 128, 64);
                m_WindowName = "Start Node";
                outputPositions.Add(Vector2.zero);
            }

            if (_type == Type.Speech)
            {
                Vector2 midpoint = new Vector2(window.position.size.x / 2, window.position.size.y / 2);
                m_Rect = new Rect((int)(midpoint.x + 128), (int)(midpoint.y + 64), 196, 256);
                m_WindowName = "Speech Node";
                outputPositions.Add(Vector2.zero);
            }

            if (_type == Type.Option)
            {
                Vector2 midpoint = new Vector2(window.position.size.x / 2, window.position.size.y / 2);
                m_Rect = new Rect((int)(midpoint.x + 128), (int)(midpoint.y + 64), 196, 256);
                m_WindowName = "Options Node";
                outputPositions.Add(Vector2.zero);
            }

            if (_type == Type.End)
            {
                Vector2 midpoint = new Vector2(window.position.size.x / 2, window.position.size.y / 2);
                m_Rect = new Rect((int)(midpoint.x - 64), (int)(midpoint.y - 32), 128, 64);
                m_WindowName = "End Node";
                outputPositions.Add(Vector2.zero);
            }
        }

        public bool CheckIsTarget(string name)
        {
            switch(m_Type)
            {
                case Type.Start:
                    if (name == start.name)
                        return true;
                    else return false;

                case Type.Speech:
                    if (name == speech.name)
                        return true;
                    else return false;

                case Type.Option:
                    if (name == options.name)
                        return true;
                    else return false;

                case Type.End:
                    if (name == end.name)
                        return true;
                    else return false;

                default:
                    return false;
            }
        }

        public void DrawConnections(List<NodeBase> windows)
        {
            if (m_Type == Type.Start)
            {
                foreach (NodeBase item in windows)
                {
                    if (item.CheckIsTarget(start.target.name))
                    {
                        Handles.DrawLine(outputPositions[0], item.position + new Vector2(item.rect.size.x / 2, (item.rect.size.y / 2)));
                    }
                }
            }

            if (m_Type == Type.Speech)
            {
                foreach (NodeBase item in windows)
                {
                    if (item.CheckIsTarget(speech.target.name))
                    {
                        Handles.DrawLine(outputPositions[0], item.position + new Vector2(item.rect.size.x / 2, (item.rect.size.y / 2)));
                    }
                }
            }

            if (m_Type == Type.Option)
            {
                //Debug.Log("A" + outputPositions.Count);
                int yOffset = 16;
                for (int i = 0; i < options.options.Count; i++)
                {
                  
                    int yPos = (int)position.y + 64;

                    foreach (NodeBase item in windows)
                    {
                        if(item.CheckIsTarget(options.options[i].target.name))
                        {
                            Handles.DrawLine(new Vector2(position.x + size.x, yPos + yOffset), item.position + new Vector2(item.rect.size.x / 2, (item.rect.size.y / 2)));
                        }
                    }

                    yOffset += 16;
                }        
            }
        }

        public void DeleteConnection(int connectingOption)
        {
            if (m_Type == Type.Start)
            {
                start.target = null;
            }

            if (m_Type == Type.Speech)
            {
                speech.target = null;
            }

            if (m_Type == Type.Option)
            {
                options.options[connectingOption].target = null;
            }
        }

        public void ConnectTo(int index, List<NodeBase> windows, int optionIndex)
        {
            if (m_Type == Type.Start)
            {
                if (windows[index].type == Type.Speech)
                {
                    start.target = windows[index].speech;
                }
                else if (windows[index].type == Type.Option)
                {
                    start.target = windows[index].options;
                }
                else if (windows[index].type == Type.End)
                {
                    start.target = windows[index].end;
                }
            }

            if (m_Type == Type.Speech)
            {
                if (windows[index].type == Type.Speech)
                {
                    speech.target = windows[index].speech;
                }
                else if (windows[index].type == Type.Option)
                {
                    speech.target = windows[index].options;
                }
                else if (windows[index].type == Type.End)
                {
                    speech.target = windows[index].end;
                }
            }

            if (m_Type == Type.Option)
            {
                if (windows[index].type == Type.Speech)
                {
                    options.options[optionIndex].target = windows[index].speech;
                }
                else if (windows[index].type == Type.Option)
                {
                    options.options[optionIndex].target = windows[index].options;
                }
                else if (windows[index].type == Type.End)
                {
                    options.options[optionIndex].target = windows[index].end;
                }
            }
        }
    }
}