using UnityEditor;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BgTools.CastVisualizer
{
    [InitializeOnLoad]
    public sealed class CastVisualizerManager
    {
        public static readonly float INF_RAY_DRAW_LENGHT = 100000.0f;

        private Harmony harmony;

        private GameObject goParent;
        private GameObject go;

        private static CastVisualizerManager instance = null; 
        public static CastVisualizerManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new CastVisualizerManager();
                return instance;
            }
        }

        public enum CastBodyVisuType
        {
            WiredBody = 0,
            Line
        }

        [System.Flags]
        public enum ViewStateFlags
        {
            None = 0,
            ViewStateElement_1 = 1 << 0,
            ViewStateElement_2 = 1 << 1,
            ViewStateElement_3 = 1 << 2,
            ViewStateElement_4 = 1 << 3,
            ViewStateElement_5 = 1 << 4,
            ViewStateElement_6 = 1 << 5,
            ViewStateElement_7 = 1 << 6,
            ViewStateElement_8 = 1 << 7,
            ViewStateElement_9 = 1 << 8,
            ViewStateElement_10 = 1 << 9,
            ViewStateElement_11 = 1 << 10,
            ViewStateElement_12 = 1 << 11,
            All = ~None
        }

        #region Fields
        public bool ShowPhysicsCasts { get; set; } = true;
        public bool ShowPhysics2DCasts { get; set; } = true;
        public ViewStateFlags ViewStatePhysicsFlag { get; set; } = ViewStateFlags.All;
        public ViewStateFlags ViewStatePhysics2DFlag { get; set; } = ViewStateFlags.All;
        public bool ShowHits { get; set; } = true;
        public CastBodyVisuType CastBodyVisualization { get; set; } = CastBodyVisuType.WiredBody;
        public float DrawTime { get; set; } = 0.0f;
        
        private Color physicsRayColor = Color.blue;
        public Color PhysicsRayColor
        {
            get
            {
                return physicsRayColor;
            }
            set
            {
                physicsRayColor = value;
            }
        }

        private Color physics2dRayColor = Color.yellow;
        public Color Physics2dRayColor
        {
            get
            {
                return physics2dRayColor;
            }
            set
            {
                physics2dRayColor = value;
            }
        }

        private Color hitMarkerColor = Color.red;
        public Color HitMarkerColor
        {
            get
            {
                return hitMarkerColor;
            }
            set
            {
                hitMarkerColor = value;
            }
        }
        #endregion

        static CastVisualizerManager()
        {
            instance = new CastVisualizerManager();
        }

        private CastVisualizerManager()
        {
            LoadConfig();

            //Harmony.DEBUG = true;

            harmony = new Harmony("com.bgtools.castvisualizer");
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;

            EditorApplication.playModeStateChanged -= PlaymodeChanged;
            EditorApplication.playModeStateChanged += PlaymodeChanged;

            if (EditorPrefs.GetBool("BGTools.CastVisualizer.ActiveState", false))
            {
                StartVisualizer();
            }
        }

        private void LoadConfig()
        {
            ShowPhysicsCasts = EditorPrefs.GetBool("BGTools.CastVisualizer.ShowPhysicsCasts", ShowPhysicsCasts);
            ShowPhysics2DCasts = EditorPrefs.GetBool("BGTools.CastVisualizer.ShowPhysics2DCasts", ShowPhysics2DCasts);
            ShowHits = EditorPrefs.GetBool("BGTools.CastVisualizer.ShowHits", ShowHits);

            string htmlColor = $"#{EditorPrefs.GetString("BGTools.CastVisualizer.PhysicsCastColor", ColorUtility.ToHtmlStringRGBA(PhysicsRayColor))}";
            ColorUtility.TryParseHtmlString(htmlColor, out physicsRayColor);
            htmlColor = $"#{EditorPrefs.GetString("BGTools.CastVisualizer.Physics2DCastColor", ColorUtility.ToHtmlStringRGBA(Physics2dRayColor))}";
            ColorUtility.TryParseHtmlString(htmlColor, out physics2dRayColor);
            htmlColor = $"#{EditorPrefs.GetString("BGTools.CastVisualizer.HitColor", ColorUtility.ToHtmlStringRGBA(HitMarkerColor))}";
            ColorUtility.TryParseHtmlString(htmlColor, out hitMarkerColor);

            DrawTime = EditorPrefs.GetFloat("BGTools.CastVisualizer.DrawTime", DrawTime);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            InstantiateDrawerObjects();
        }

        private void PlaymodeChanged(PlayModeStateChange state)
        {
            if(state == PlayModeStateChange.EnteredPlayMode)
            {
                //Debug.Log("[RV] Create visulizers");
                InstantiateDrawerObjects();
            }
            if(state == PlayModeStateChange.ExitingPlayMode)
            {
                //Debug.Log("[RV] Distroy visulizers");

                GameObject.DestroyImmediate(goParent);
                GameObject.DestroyImmediate(go);
            }
        }

        private void InstantiateDrawerObjects()
        {
            GameObject parent = GameObject.Find("bgtools_rayvisualizer_root");
            if (parent == null)
            {
                goParent = new GameObject("bgtools_rayvisualizer_root");
                go = new GameObject("bgtools_rayvisualizer_drawer");
                go.transform.SetParent(goParent.transform);
                go.AddComponent<PhysicsVisualizer>();
                go.AddComponent<Physics2DVisualizer>();
                //goParent.hideFlags = HideFlags.HideAndDontSave | HideFlags.NotEditable;
                //go.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
            }
        }

        public void StartVisualizer()
        {
            if (harmony != null)
            {
                harmony.PatchAll();
            }
        }

        public void StopVisualizer()
        {
            if (harmony != null)
            {
                harmony.UnpatchAll();
            }
        }
    }
}