using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BgTools.CastVisualizer
{
    [DefaultExecutionOrder(-32000)]
    public abstract class BaseVisulizer<T, U> : MonoBehaviour where T : System.Enum
    {
        // Must be a class than Tuple and structs are immutable
        private class RenderData<TTuple>
        {
            public float Lifetime;
            public readonly TTuple Data;

            public RenderData(float time, TTuple tupleData)
            {
                Lifetime = time;
                Data = tupleData;
            }
        }
        
        private List<RenderData<(Vector3, Vector3)>> hitsToRender = new List<RenderData<(Vector3, Vector3)>>();
        private List<RenderData<(Ray, float)>> raysToRender = new List<RenderData<(Ray, float)>>();
        private List<RenderData<(T, Matrix4x4, Vector3)>> meshesToRender = new List<RenderData<(T, Matrix4x4, Vector3)>>();
        private List<RenderData<(T, Matrix4x4)>> meshHitsToRender = new List<RenderData<(T, Matrix4x4)>>();
        private List<RenderData<(U, bool)>> colliderHitsToRender = new List<RenderData<(U, bool)>>();
        private List<RenderData<(U, bool)>> colliderCastsToRender = new List<RenderData<(U, bool)>>();

        private int lastframe;

        #region Add Render Data Functions
        internal static bool AddHitToRender((Vector3, Vector3) hitData)
        {
            if(Instance != null)
            {
                Instance.hitsToRender.Add(new RenderData<(Vector3, Vector3)>(CastVisualizerManager.Instance.DrawTime, hitData));
                return true;
            }
            return false;
        }

        internal static bool AddRayToRender((Ray, float) rayData)
        {
            if (Instance != null)
            {
                Instance.raysToRender.Add(new RenderData<(Ray, float)>(CastVisualizerManager.Instance.DrawTime, rayData));
                return true;
            }
            return false;
        }

        internal static bool AddMeshToRender((T, Matrix4x4, Vector3) meshData)
        {
            if (Instance != null)
            {
                Instance.meshesToRender.Add(new RenderData<(T, Matrix4x4, Vector3)>(CastVisualizerManager.Instance.DrawTime, meshData));
                return true;
            }
            return false;
        }

        internal static bool AddMeshHitToRender((T, Matrix4x4) meshHitData)
        {
            if (Instance != null)
            {
                Instance.meshHitsToRender.Add(new RenderData<(T, Matrix4x4)>(CastVisualizerManager.Instance.DrawTime, meshHitData));
                return true;
            }
            return false;
        }

        internal static bool AddColliderHitToRender(U colliderData)
        {
            if (Instance != null)
            {
                Instance.colliderHitsToRender.Add(new RenderData<(U, bool)>(CastVisualizerManager.Instance.DrawTime, (colliderData, false)));
                return true;
            }
            return false;
        }

        internal static bool AddColliderCastToRender(U colliderData)
        {
            if (Instance != null)
            {
                Instance.colliderCastsToRender.Add(new RenderData<(U, bool)>(CastVisualizerManager.Instance.DrawTime, (colliderData, false)));
                return true;
            }
            return false;
        }
        #endregion

        protected static BaseVisulizer<T, U> Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

            //Debug.Log($"[RV] Init {GetType().Name}");
        }

        private void FixedUpdate()
        {
            if (lastframe == Time.frameCount)
                return;
            
            lastframe = Time.frameCount;
                
            hitsToRender.RemoveAll(data => data.Lifetime < 0.0f);
            raysToRender.RemoveAll(data => data.Lifetime < 0.0f);
            meshesToRender.RemoveAll(data => data.Lifetime < 0.0f);
            meshHitsToRender.RemoveAll(data => data.Lifetime < 0.0f);
            colliderHitsToRender.RemoveAll(data => data.Lifetime < 0.0f);
            colliderCastsToRender.RemoveAll(data => data.Lifetime < 0.0f);

            hitsToRender.ForEach(renderData => { renderData.Lifetime -= Time.deltaTime; });
            raysToRender.ForEach(renderData => renderData.Lifetime -= Time.deltaTime);
            meshesToRender.ForEach(renderData => renderData.Lifetime -= Time.deltaTime);
            meshHitsToRender.ForEach(renderData => renderData.Lifetime -= Time.deltaTime);
            colliderHitsToRender.ForEach(renderData => renderData.Lifetime -= Time.deltaTime);
            colliderCastsToRender.ForEach(renderData => renderData.Lifetime -= Time.deltaTime);
        }

        protected virtual Color RayColor() { return Color.magenta; }
        protected virtual bool DrawCondition() { return true; }
        protected abstract void DrawMeshes((T, Matrix4x4, Vector3) meshData);
        protected abstract void DrawHitMeshes((T, Matrix4x4) meshData);
        protected abstract void DrawColliders(U collider);

        // Necessary for Gizmos Menu entry
        //private void OnDrawGizmos()
        //{
        //    // Code in DrawGizmos() fucntion
        //}

        // Use DrawGizmo annotation to avoid the pickable Gizmos in OnDrawGizmos()
        //[DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
        protected static void DrawGizmos(BaseVisulizer<T, U> script, GizmoType gizmoType)
        {
            if (!script.DrawCondition())
                return;

            // Casts
            Color orgGizmosColor = Gizmos.color;
            Gizmos.color = Instance.RayColor();

            foreach (RenderData<(Ray, float)> rayValues in script.raysToRender)
            {
                Gizmos.DrawRay(rayValues.Data.Item1.origin, rayValues.Data.Item1.direction * rayValues.Data.Item2);
            }

            Gizmos.color = orgGizmosColor;

            using (new Handles.DrawingScope(Instance.RayColor()))
            {
                foreach (RenderData<(T, Matrix4x4, Vector3)> meshData in script.meshesToRender)
                {
                    script.DrawMeshes((meshData.Data.Item1, meshData.Data.Item2, meshData.Data.Item3));
                }

                foreach (RenderData<(U, bool)> collHit in script.colliderCastsToRender)
                {
                    if(collHit.Data.Item1 != null)
                        script.DrawColliders(collHit.Data.Item1);
                }
            }
            // Hits
            if (!CastVisualizerManager.Instance.ShowHits)
                return;

            using (new Handles.DrawingScope(CastVisualizerManager.Instance.HitMarkerColor))
            {
                // HitPoints
                foreach (RenderData<(Vector3, Vector3)> hitData in script.hitsToRender)
                {
                    RenderUtil.DrawCross(hitData.Data.Item1, hitData.Data.Item2);
                }

                // Meshes
                foreach (RenderData<(T, Matrix4x4)> meshData in script.meshHitsToRender)
                {
                    script.DrawHitMeshes((meshData.Data.Item1, meshData.Data.Item2));
                }

                // Collider
                foreach (RenderData<(U, bool)> collHit in script.colliderHitsToRender)
                {
                    script.DrawColliders(collHit.Data.Item1);
                }
            }
        }
    }
}