using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class UIDrawCallOverview : EditorWindow
{

    [MenuItem("NGUI/Open/Draw Call Overview")]
    static public void OpenPanelOverview()
    {
        EditorWindow.GetWindow<UIDrawCallOverview>(false, "Draw Calls", true);
    }

    //TODO make fold out all draw calls button when they're not.
    Vector2 mScroll = Vector2.zero;

    readonly Dictionary<string, int> _depths = new Dictionary<string, int>();
    private bool _depthsDirty = false;


    readonly Color32[] _allColors = new Color32[]
    {
        new Color32(124,252,0,255),        //lawn green
        new Color32(30,144,255,255),         //dodger blue
        new Color32(255,215,32,255),         //gold
        new Color32(255,0,255,255),          //magenta
        new Color32(244,164,96,255),            //sandy brown
        new Color32(0,191,255,255),        //sky blue
        new Color32(0,128,0,255),            //green
        new Color32(186,85,211,255),         //medium orchid
        
        
        
        
        
        
        
        //add more colors for more differentiation
        //see http://www.rapidtables.com/web/color/RGB_Color.htm#rgb-format
    };
    private readonly Dictionary<UIPanel, Color32> _activeColors = new Dictionary<UIPanel, Color32>();

    

    void OnGUI()
    {
        
        if (_depthsDirty)
        {
            _depths.Clear();
            _depthsDirty = false;
        }
        GUILayout.BeginVertical();
        mScroll = GUILayout.BeginScrollView(mScroll);
        
        GUI.backgroundColor = new Color32(30, 144, 255, 255);
        for (int i = 0; i < UIDrawCall.list.size; ++i)
        {
            UIDrawCall dc = UIDrawCall.list[i];
            string key = dc.keyName;
            

            Color32 backgroundColor;
          
            if (_activeColors.ContainsKey(dc.panel))
            {
                backgroundColor = _activeColors[dc.panel];
            }
            else
            {
                backgroundColor = _allColors[_activeColors.Count % _allColors.Length];

                _activeColors.Add(dc.panel, backgroundColor);
            }

            if (!dc.isActive) backgroundColor = Color.red;

            GUI.backgroundColor = backgroundColor;
            GUI.color = Color.white;

            string name = "<b>" + key + " of " + UIDrawCall.list.size + " (" + dc.panel.name + ")</b>";
            if (!dc.isActive) name = name + " <b>(HIDDEN)</b>";
            bool shouldBeUnfolded = DrawHeader(name, key);
            if (!shouldBeUnfolded) continue;

            NGUIEditorTools.BeginContents();
            GUI.backgroundColor = Color.white;
            EditorGUILayout.ObjectField("Material", dc.baseMaterial, typeof(Material), false);

            GUILayout.BeginHorizontal();
           

            string panelKey = key + "panel";

            int currentPanelDepth = dc.panel.depth;
            int panelDepth = currentPanelDepth;
            
            if (_depths.ContainsKey(panelKey))
            {
                panelDepth = _depths[panelKey];
            }


            GUILayout.Label("Panel depth", GUILayout.Width(80f));
            _depths[panelKey] = EditorGUILayout.IntField(panelDepth, GUILayout.Width(50f));

            

            var alignBefore = GUI.skin.button.alignment;
            GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            if (GUILayout.Button("Select panel: <b>" + dc.panel.name + "</b>", new GUILayoutOption[] { GUILayout.ExpandWidth(false), GUILayout.MinWidth(250f)}))
            {
                Selection.activeGameObject = dc.panel.gameObject;
            }
            GUI.skin.button.alignment = alignBefore;
            GUILayout.Space(18f);

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Hide draw call", GUILayout.Width(90f));
            
            bool draw = !EditorGUILayout.Toggle(!dc.isActive);

            if (dc.isActive != draw)
            {
                dc.isActive = draw;
                EditorUtility.SetDirty(dc.panel);
            }
            GUILayout.EndHorizontal();
            // Apply panel depth buttons

            if (panelDepth != currentPanelDepth)
            {
                GUILayout.BeginHorizontal();
                Color oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(0.4f, 1f, 0.4f);
                if (GUILayout.Button("Apply Depth to panel", GUILayout.Width(150f)))
                {   
                    dc.panel.depth = panelDepth;
                    _depths.Clear();
                    _depthsDirty = true;
                    GUI.FocusControl(null);
                    EditorUtility.SetDirty(dc.panel);

                }
                GUI.backgroundColor = new Color(1f, 0.8f, 0.8f);

                if (GUILayout.Button("Reset Depth", GUILayout.Width(150f)))
                {
                    _depths[panelKey] = currentPanelDepth;
                    GUI.FocusControl(null);
                    EditorUtility.SetDirty(dc.panel);

                }
                GUI.backgroundColor = oldColor;
                GUILayout.EndHorizontal();
            }




            var depths = new List<List<UIWidget>>();

            int initial = NGUITools.GetHierarchy(dc.panel.cachedGameObject).Length + 1;
            int masterListIndex = -1;
            int currentDepth = int.MinValue;

            for (int b = 0; b < UIWidget.list.size; ++b)
            {
                UIWidget w = UIWidget.list[b];
                int depth = w.depth;
                if (currentDepth == int.MinValue || currentDepth != depth)
                {
                    currentDepth = depth;
                    masterListIndex++;
                    depths.Add(new List<UIWidget>());
                }

                if (w.drawCall == dc)
                { 
                    depths[masterListIndex].Add(w);
                }
            }



            for (int d = 0; d < depths.Count; d++)
            {
                int widgetCount = depths[d].Count;
                if (widgetCount < 1) continue;
                currentDepth = depths[d][0].depth;

                string foldoutTitle = widgetCount + " Widget" + (widgetCount == 1 ? "" : "s") + " - Depth: " + currentDepth.ToString();

                string depthKey = key + "depth" + currentDepth.ToString();

                bool wasFoldedOut = EditorPrefs.GetBool(depthKey, false);

                int massDepth = currentDepth;
                if (_depths.ContainsKey(depthKey))
                {
                    massDepth = _depths[depthKey];
                }


                GUILayout.BeginHorizontal();


                GUILayout.Label("Depth", GUILayout.Width(40f));
                _depths[depthKey] = EditorGUILayout.IntField(massDepth, GUILayout.Width(50f));

                bool foldedOut = false;
                foldedOut = DrawDepthCollapser("<b>" + foldoutTitle + "</b> - Click to " + (wasFoldedOut ? "collapse" : "expand"), depthKey, wasFoldedOut);

                GUILayout.EndHorizontal();

                if (massDepth != currentDepth)
                {
                    GUILayout.BeginHorizontal();
                    Color oldColor = GUI.backgroundColor;
                    GUI.backgroundColor = new Color(0.4f, 1f, 0.4f);
                    if (GUILayout.Button("Apply Depth to widgets", GUILayout.Width(150f)))
                    {
                        for (int iW = 0; iW < depths[d].Count; iW++)
                        {
                            depths[d][iW].depth = massDepth;
                        }
                        EditorPrefs.DeleteKey(depthKey);
                        //_depths.Clear();
                        _depthsDirty = true;
                        GUI.FocusControl(null);
                        EditorUtility.SetDirty(dc.panel);
                    }
                    GUI.backgroundColor = new Color(1f, 0.8f, 0.8f);

                    if (GUILayout.Button("Reset Depth", GUILayout.Width(150f)))
                    {
                        _depths[depthKey] = currentDepth;
                        GUI.FocusControl(null);
                        EditorUtility.SetDirty(dc.panel);

                    }
                    GUI.backgroundColor = oldColor;
                    GUILayout.EndHorizontal();
                }

                

                if (foldedOut)
                {
                    for (int iW = 0; iW < depths[d].Count; iW++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(10f);
                        alignBefore = GUI.skin.button.alignment;
                        GUI.skin.button.alignment = TextAnchor.MiddleLeft;
                        if (GUILayout.Button(NGUITools.GetHierarchy(depths[d][iW].cachedGameObject).Remove(0, initial), GUILayout.ExpandWidth(false)))
                        {
                            Selection.activeGameObject = depths[d][iW].gameObject;
                        }
                        GUI.skin.button.alignment = alignBefore;
                       
                        GUILayout.EndHorizontal();
                    }
                }
            }
            NGUIEditorTools.EndContents();
            GUI.color = Color.white;
        }
        GUI.backgroundColor = Color.white;
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }


    public bool DrawDepthCollapser(string text, string key, bool forceOn)
    {
        bool state = EditorPrefs.GetBool(key, forceOn);

        GUILayout.Space(3f);
        Color oldColor = GUI.backgroundColor;
        if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
        //GUILayout.BeginHorizontal();
        GUILayout.Space(3f);

        GUI.changed = false;

        if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.ExpandWidth(false))) state = !state;
        if (GUI.changed) EditorPrefs.SetBool(key, state);

        GUILayout.Space(2f);
        //GUILayout.EndHorizontal();
        GUI.backgroundColor = oldColor;
        if (!forceOn && !state) GUILayout.Space(3f);
        return state;
    }

    static public bool DrawHeader(string text, string key)
    {
        bool state = EditorPrefs.GetBool(key, true);
        
        GUILayout.Space(3f);
        
      
        GUILayout.BeginHorizontal();
        GUILayout.Space(3f);

        GUI.changed = false;

        if (!GUILayout.Toggle(true, text, "dragtab")) state = !state;
        if (GUI.changed) EditorPrefs.SetBool(key, state);

        GUILayout.Space(2f);
        GUILayout.EndHorizontal();
        if (!state) GUILayout.Space(3f);
        return state;
    }
}
