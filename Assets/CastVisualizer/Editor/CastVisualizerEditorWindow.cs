using UnityEditor;
using UnityEngine;

namespace BgTools.CastVisualizer
{
    using static BgTools.CastVisualizer.CastVisualizerManager;

    public class CastVisualizerEditorWindow : EditorWindow
    {
        private ActicationState actvationState = ActicationState.Off;
        private enum ActicationState
        {
            Off = 0,
            On = 1
        }

        private CastVisualizerManager castVisualizer;

        private Color physicsRayColor;
        private Color physics2dRayColor;
        private Color hitMarkerColor;

        bool PhysicsCastsFoldout = false;
        bool PhysicsOverlapsFoldout = false;
        bool Physics2DCastsFoldout = false;
        bool Physics2DOverlapsFoldout = false;

        [MenuItem("Tools/BG Tools/CastVisualizer", false, 1)]
        static void ShowWindow()
        {
            CastVisualizerEditorWindow window = EditorWindow.GetWindow<CastVisualizerEditorWindow>(false, "CastVisualizer");
            window.minSize = new Vector2(270.0f, 300.0f);
            window.name = "CastVisualizer";

            window.Show();
        }

        private void OnEnable()
        {
            castVisualizer = CastVisualizerManager.Instance;

            actvationState = (ActicationState) EditorPrefs.GetInt("BGTools.CastVisualizer.ActiveState", 0);

            castVisualizer.ShowPhysicsCasts = EditorPrefs.GetBool("BGTools.CastVisualizer.ShowPhysicsCasts", castVisualizer.ShowPhysicsCasts);
            castVisualizer.ShowPhysics2DCasts = EditorPrefs.GetBool("BGTools.CastVisualizer.ShowPhysics2DCasts", castVisualizer.ShowPhysics2DCasts);

            castVisualizer.ViewStatePhysicsFlag = (ViewStateFlags) EditorPrefs.GetInt("BGTools.CastVisualizer.ViewStatePhysicsFlag", (int)castVisualizer.ViewStatePhysicsFlag);
            castVisualizer.ViewStatePhysics2DFlag = (ViewStateFlags)EditorPrefs.GetInt("BGTools.CastVisualizer.ViewStatePhysics2DFlag", (int)castVisualizer.ViewStatePhysics2DFlag);

            castVisualizer.ShowHits = EditorPrefs.GetBool("BGTools.CastVisualizer.ShowHits", castVisualizer.ShowHits);
            castVisualizer.CastBodyVisualization = (CastBodyVisuType) EditorPrefs.GetInt("BGTools.CastVisualizer.CastBodyVisuType", 0);
            castVisualizer.DrawTime = EditorPrefs.GetFloat("BGTools.CastVisualizer.DrawTime", castVisualizer.DrawTime);

            string htmlColor = $"#{EditorPrefs.GetString("BGTools.CastVisualizer.PhysicsCastColor", ColorUtility.ToHtmlStringRGBA(castVisualizer.PhysicsRayColor))}";
            ColorUtility.TryParseHtmlString(htmlColor, out physicsRayColor);
            htmlColor = $"#{EditorPrefs.GetString("BGTools.CastVisualizer.Physics2DCastColor", ColorUtility.ToHtmlStringRGBA(castVisualizer.Physics2dRayColor))}";
            ColorUtility.TryParseHtmlString(htmlColor, out physics2dRayColor);
            htmlColor = $"#{EditorPrefs.GetString("BGTools.CastVisualizer.HitColor", ColorUtility.ToHtmlStringRGBA(castVisualizer.HitMarkerColor))}";
            ColorUtility.TryParseHtmlString(htmlColor, out hitMarkerColor);
        }

        void OnGUI()
        {
            GUILayout.BeginVertical();

            GUIStyle gs = new GUIStyle();
            gs.margin.left = 100;
            gs.margin.right = 2;

            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            if (GUILayout.Toggle(actvationState == ActicationState.Off, "Off", EditorStyles.toolbarButton))
                actvationState = ActicationState.Off;

            if (GUILayout.Toggle(actvationState == ActicationState.On, "On", EditorStyles.toolbarButton))
                actvationState = ActicationState.On;

            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetInt("BGTools.CastVisualizer.ActiveState", (int) actvationState);
                switch(actvationState)
                {
                    case ActicationState.On:
                        castVisualizer.StartVisualizer();
                        break;
                    case ActicationState.Off:
                        castVisualizer.StopVisualizer();
                        break;
                }
            }
            GUILayout.EndHorizontal();

            GUI.enabled = (actvationState == ActicationState.On);

            GUILayout.Label("Show");
            EditorGUI.indentLevel++;

            EditorGUI.BeginChangeCheck();
            castVisualizer.ShowPhysicsCasts = EditorGUILayout.BeginToggleGroup("Physics", castVisualizer.ShowPhysicsCasts);
            if (castVisualizer.ShowPhysicsCasts)
            {
                EditorGUI.indentLevel++;
                PhysicsCastsFoldout = EditorGUILayout.Foldout(PhysicsCastsFoldout, new GUIContent("Casts"));
                if(PhysicsCastsFoldout)
                {
                    EditorGUI.indentLevel++;
                    if (EditorGUILayout.Toggle("Line/Raycasts",     castVisualizer.ViewStatePhysicsFlag.HasFlag(ViewStateFlags.ViewStateElement_1))) { castVisualizer.ViewStatePhysicsFlag |= ViewStateFlags.ViewStateElement_1; } else { castVisualizer.ViewStatePhysicsFlag &= ~ViewStateFlags.ViewStateElement_1; }
                    if (EditorGUILayout.Toggle("BoxCasts",          castVisualizer.ViewStatePhysicsFlag.HasFlag(ViewStateFlags.ViewStateElement_2))) { castVisualizer.ViewStatePhysicsFlag |= ViewStateFlags.ViewStateElement_2; } else { castVisualizer.ViewStatePhysicsFlag &= ~ViewStateFlags.ViewStateElement_2; }
                    if (EditorGUILayout.Toggle("SphereCasts",       castVisualizer.ViewStatePhysicsFlag.HasFlag(ViewStateFlags.ViewStateElement_4))) { castVisualizer.ViewStatePhysicsFlag |= ViewStateFlags.ViewStateElement_4; } else { castVisualizer.ViewStatePhysicsFlag &= ~ViewStateFlags.ViewStateElement_4; }
                    if (EditorGUILayout.Toggle("CapsuleCasts",      castVisualizer.ViewStatePhysicsFlag.HasFlag(ViewStateFlags.ViewStateElement_3))) { castVisualizer.ViewStatePhysicsFlag |= ViewStateFlags.ViewStateElement_3; } else { castVisualizer.ViewStatePhysicsFlag &= ~ViewStateFlags.ViewStateElement_3; }
                    EditorGUI.indentLevel--;
                }
                PhysicsOverlapsFoldout = EditorGUILayout.Foldout(PhysicsOverlapsFoldout, new GUIContent("Overlaps"));
                if(PhysicsOverlapsFoldout)
                {
                    EditorGUI.indentLevel++;
                    if (EditorGUILayout.Toggle("OverlapBoxes",      castVisualizer.ViewStatePhysicsFlag.HasFlag(ViewStateFlags.ViewStateElement_5))) { castVisualizer.ViewStatePhysicsFlag |= ViewStateFlags.ViewStateElement_5; } else { castVisualizer.ViewStatePhysicsFlag &= ~ViewStateFlags.ViewStateElement_5; }
                    if (EditorGUILayout.Toggle("OverlapSpheres",    castVisualizer.ViewStatePhysicsFlag.HasFlag(ViewStateFlags.ViewStateElement_6))) { castVisualizer.ViewStatePhysicsFlag |= ViewStateFlags.ViewStateElement_6; } else { castVisualizer.ViewStatePhysicsFlag &= ~ViewStateFlags.ViewStateElement_6; }
                    if (EditorGUILayout.Toggle("OverlapCapsules",   castVisualizer.ViewStatePhysicsFlag.HasFlag(ViewStateFlags.ViewStateElement_7))) { castVisualizer.ViewStatePhysicsFlag |= ViewStateFlags.ViewStateElement_7; } else { castVisualizer.ViewStatePhysicsFlag &= ~ViewStateFlags.ViewStateElement_7; }
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool("BGTools.CastVisualizer.ShowPhysicsCasts", castVisualizer.ShowPhysicsCasts);
                EditorPrefs.SetInt("BGTools.CastVisualizer.ViewStatePhysicsFlag", (int)castVisualizer.ViewStatePhysicsFlag);
            }

            EditorGUI.BeginChangeCheck();
            castVisualizer.ShowPhysics2DCasts = EditorGUILayout.BeginToggleGroup("Physics2D", castVisualizer.ShowPhysics2DCasts);
            if (castVisualizer.ShowPhysics2DCasts)
            {
                EditorGUI.indentLevel++;
                Physics2DCastsFoldout = EditorGUILayout.Foldout(Physics2DCastsFoldout, new GUIContent("Casts"));
                if(Physics2DCastsFoldout)
                {
                    EditorGUI.indentLevel++;
                    if (EditorGUILayout.Toggle("Line/Raycasts",    castVisualizer.ViewStatePhysics2DFlag.HasFlag(ViewStateFlags.ViewStateElement_1)))  { castVisualizer.ViewStatePhysics2DFlag |= ViewStateFlags.ViewStateElement_1;  } else { castVisualizer.ViewStatePhysics2DFlag &= ~ViewStateFlags.ViewStateElement_1; }
                    if (EditorGUILayout.Toggle("BoxCasts",         castVisualizer.ViewStatePhysics2DFlag.HasFlag(ViewStateFlags.ViewStateElement_2)))  { castVisualizer.ViewStatePhysics2DFlag |= ViewStateFlags.ViewStateElement_2;  } else { castVisualizer.ViewStatePhysics2DFlag &= ~ViewStateFlags.ViewStateElement_2; }
                    if (EditorGUILayout.Toggle("CircleCasts",      castVisualizer.ViewStatePhysics2DFlag.HasFlag(ViewStateFlags.ViewStateElement_4)))  { castVisualizer.ViewStatePhysics2DFlag |= ViewStateFlags.ViewStateElement_4;  } else { castVisualizer.ViewStatePhysics2DFlag &= ~ViewStateFlags.ViewStateElement_4; }
                    if (EditorGUILayout.Toggle("CapsuleCasts",     castVisualizer.ViewStatePhysics2DFlag.HasFlag(ViewStateFlags.ViewStateElement_3)))  { castVisualizer.ViewStatePhysics2DFlag |= ViewStateFlags.ViewStateElement_3;  } else { castVisualizer.ViewStatePhysics2DFlag &= ~ViewStateFlags.ViewStateElement_3; }
                    EditorGUI.indentLevel--;
                }
                Physics2DOverlapsFoldout = EditorGUILayout.Foldout(Physics2DOverlapsFoldout, new GUIContent("Overlaps"));
                if(Physics2DOverlapsFoldout)
                {
                    EditorGUI.indentLevel++;
                    if (EditorGUILayout.Toggle("OverlapBox/Areas", castVisualizer.ViewStatePhysics2DFlag.HasFlag(ViewStateFlags.ViewStateElement_5)))  { castVisualizer.ViewStatePhysics2DFlag |= ViewStateFlags.ViewStateElement_5;  } else { castVisualizer.ViewStatePhysics2DFlag &= ~ViewStateFlags.ViewStateElement_5; }
                    if (EditorGUILayout.Toggle("OverlapCircles",   castVisualizer.ViewStatePhysics2DFlag.HasFlag(ViewStateFlags.ViewStateElement_6)))  { castVisualizer.ViewStatePhysics2DFlag |= ViewStateFlags.ViewStateElement_6;  } else { castVisualizer.ViewStatePhysics2DFlag &= ~ViewStateFlags.ViewStateElement_6; }
                    if (EditorGUILayout.Toggle("OverlapCapsules",  castVisualizer.ViewStatePhysics2DFlag.HasFlag(ViewStateFlags.ViewStateElement_7)))  { castVisualizer.ViewStatePhysics2DFlag |= ViewStateFlags.ViewStateElement_7;  } else { castVisualizer.ViewStatePhysics2DFlag &= ~ViewStateFlags.ViewStateElement_7; }
                    if (EditorGUILayout.Toggle("OverlapColliders", castVisualizer.ViewStatePhysics2DFlag.HasFlag(ViewStateFlags.ViewStateElement_8)))  { castVisualizer.ViewStatePhysics2DFlag |= ViewStateFlags.ViewStateElement_8;  } else { castVisualizer.ViewStatePhysics2DFlag &= ~ViewStateFlags.ViewStateElement_8; }
                    if (EditorGUILayout.Toggle("OverlapPoints",    castVisualizer.ViewStatePhysics2DFlag.HasFlag(ViewStateFlags.ViewStateElement_9)))  { castVisualizer.ViewStatePhysics2DFlag |= ViewStateFlags.ViewStateElement_9;  } else { castVisualizer.ViewStatePhysics2DFlag &= ~ViewStateFlags.ViewStateElement_9; }
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool("BGTools.CastVisualizer.ShowPhysics2DCasts", castVisualizer.ShowPhysics2DCasts);
                EditorPrefs.SetInt("BGTools.CastVisualizer.ViewStatePhysics2DFlag", (int)castVisualizer.ViewStatePhysics2DFlag);
            }
            GUILayout.Space(EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();
            castVisualizer.ShowHits = EditorGUILayout.Toggle("Hits", castVisualizer.ShowHits);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool("BGTools.CastVisualizer.ShowHits", castVisualizer.ShowHits);
            }

            EditorGUI.indentLevel--;

            GUILayout.Space(EditorGUIUtility.singleLineHeight);

            GUILayout.Label("Settings");
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            physicsRayColor = EditorGUILayout.ColorField("Physics cast color", physicsRayColor);
            physics2dRayColor = EditorGUILayout.ColorField("Physics2D cast color", physics2dRayColor);
            hitMarkerColor = EditorGUILayout.ColorField("Hit marker color", hitMarkerColor);
            if (EditorGUI.EndChangeCheck())
            {
                castVisualizer.PhysicsRayColor = physicsRayColor;
                castVisualizer.Physics2dRayColor = physics2dRayColor;
                castVisualizer.HitMarkerColor = hitMarkerColor;

                EditorPrefs.SetString("BGTools.CastVisualizer.PhysicsCastColor", ColorUtility.ToHtmlStringRGBA(castVisualizer.PhysicsRayColor));
                EditorPrefs.SetString("BGTools.CastVisualizer.Physics2DCastColor", ColorUtility.ToHtmlStringRGBA(castVisualizer.Physics2dRayColor));
                EditorPrefs.SetString("BGTools.CastVisualizer.HitColor", ColorUtility.ToHtmlStringRGBA(castVisualizer.HitMarkerColor));
            }

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            castVisualizer.CastBodyVisualization = (CastVisualizerManager.CastBodyVisuType) EditorGUILayout.EnumPopup("Body visualization", castVisualizer.CastBodyVisualization);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetInt("BGTools.CastVisualizer.CastBodyVisuType", (int) castVisualizer.CastBodyVisualization);
            }
            
            EditorGUI.BeginChangeCheck();
            castVisualizer.DrawTime = (int) EditorGUILayout.Slider(new GUIContent("Draw time", "Time to show visualisations in seconds; Min means one frame"), (int) castVisualizer.DrawTime, 0.0f, 10.0f);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetFloat("BGTools.CastVisualizer.DrawTime", castVisualizer.DrawTime);
            }

            EditorGUI.indentLevel--;
            GUILayout.EndVertical();
        }
    }
}