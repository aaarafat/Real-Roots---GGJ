using HarmonyLib;
using UnityEditor;
using UnityEngine;

namespace BgTools.CastVisualizer
{
    public enum PhysicsMeshShape
    {
        Box = 0,
        Capsule,
        Sphere
    }

    public sealed class PhysicsVisualizer : BaseVisulizer<PhysicsMeshShape, Collider>
    {
        // Use DrawGizmo annotation to avoid the pickable Gizmos in OnDrawGizmos()
        [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
        static void DrawGizmos(PhysicsVisualizer script, GizmoType gizmoType)
        {
            BaseVisulizer<PhysicsMeshShape, Collider>.DrawGizmos(script, gizmoType);
        }

        protected override bool DrawCondition() { return CastVisualizerManager.Instance.ShowPhysicsCasts; }
        protected override Color RayColor() { return CastVisualizerManager.Instance.PhysicsRayColor; }

        protected override void DrawMeshes((PhysicsMeshShape, Matrix4x4, Vector3) meshData)
        {
            switch (meshData.Item1)
            {
                case PhysicsMeshShape.Box:
                    using (new Handles.DrawingScope(meshData.Item2))
                    {
                        RenderUtil.DrawWireCube(Vector3.zero, Vector3.one);

                        if (meshData.Item3 != Vector3.zero)
                        {
                            Matrix4x4 invMat = meshData.Item2.inverse;

                            RenderUtil.DrawWireCube(invMat * meshData.Item3, Vector3.one);

                            switch (CastVisualizerManager.Instance.CastBodyVisualization)
                            {
                                case CastVisualizerManager.CastBodyVisuType.Line:
                                    Handles.DrawDottedLine(Vector3.zero, invMat * meshData.Item3, 5.0f);
                                    break;
                                case CastVisualizerManager.CastBodyVisuType.WiredBody:
                                    Vector3[] conectionPoints = RenderUtil.GetWireCubeConnectingPoints(meshData.Item2.lossyScale * 0.25f, meshData.Item3, meshData.Item2.rotation);
                                    foreach (Vector3 pos in conectionPoints)
                                    {
                                        Handles.DrawDottedLine(pos, (Vector3)(invMat * meshData.Item3) + pos, 5.0f);
                                    }
                                    break;
                            }
                        }
                    }
                    break;
                case PhysicsMeshShape.Sphere:
                    using (new Handles.DrawingScope(meshData.Item2))
                    {
                        RenderUtil.DrawWireSphere(Vector3.zero, 1.0f);

                        if (meshData.Item3 != Vector3.zero)
                        {
                            Matrix4x4 invMat = meshData.Item2.inverse;

                            RenderUtil.DrawWireSphere(invMat * meshData.Item3, 1.0f);

                            switch (CastVisualizerManager.Instance.CastBodyVisualization)
                            {
                                case CastVisualizerManager.CastBodyVisuType.Line:
                                    Handles.DrawDottedLine(Vector3.zero, invMat * meshData.Item3, 5.0f);
                                    break;
                                case CastVisualizerManager.CastBodyVisuType.WiredBody:
                                    Vector3[] conectionPoints = RenderUtil.GetWireSphereConnectingPoints(meshData.Item3);
                                    foreach (Vector3 pos in conectionPoints)
                                    {
                                        Handles.DrawDottedLine(pos, (Vector3)(invMat * meshData.Item3) + pos, 5.0f);
                                    }
                                    break;
                            }
                        }
                    }
                    break;
                case PhysicsMeshShape.Capsule:
                    //(point0, point1, Vector4(radius), Vector4.zero);
                    Matrix4x4 dataMatrix = meshData.Item2;

                    Vector3 point0 = dataMatrix.GetColumn(0);
                    Vector3 point1 = dataMatrix.GetColumn(1);
                    float radius = dataMatrix.GetColumn(2).x;

                    RenderUtil.DrawWireCapsule(point0, point1, radius);

                    if (meshData.Item3 != Vector3.zero)
                    {
                        RenderUtil.DrawWireCapsule(point0 + meshData.Item3, point1 + meshData.Item3, radius);

                        switch (CastVisualizerManager.Instance.CastBodyVisualization)
                        {
                            case CastVisualizerManager.CastBodyVisuType.Line:
                                Vector3 center = (point0 + point1) * 0.5f;
                                Handles.DrawDottedLine(center, center + meshData.Item3, 5.0f);
                                break;
                            case CastVisualizerManager.CastBodyVisuType.WiredBody:
                                Vector3[] conectionPoints = RenderUtil.GetWireCapsuleConnectingPoints(point0, point1, radius, meshData.Item3);
                                foreach (Vector3 pos in conectionPoints)
                                {
                                    Handles.DrawDottedLine(pos, meshData.Item3 + pos, 5.0f);
                                }
                                break;
                        }
                    }
                    break;
            }
        }

        protected override void DrawHitMeshes((PhysicsMeshShape, Matrix4x4) meshData)
        {
            switch (meshData.Item1)
            {
                case PhysicsMeshShape.Box:
                    using (new Handles.DrawingScope(meshData.Item2))
                    {
                        RenderUtil.DrawWireCube(Vector3.zero, Vector3.one);
                    }
                    break;
                case PhysicsMeshShape.Sphere:
                    using (new Handles.DrawingScope(meshData.Item2))
                    {
                        RenderUtil.DrawWireSphere(Vector3.zero, 1.0f);
                    }
                    break;
                case PhysicsMeshShape.Capsule:
                    Matrix4x4 dataMatrix = meshData.Item2;

                    Vector3 point0 = dataMatrix.GetColumn(0);
                    Vector3 point1 = dataMatrix.GetColumn(1);
                    float radius = dataMatrix.GetColumn(2).x;

                    RenderUtil.DrawWireCapsule(point0, point1, radius);
                    break;
            }
        }

        protected override void DrawColliders(Collider collider)
        {
            switch (collider.GetType().Name)
            {
                case "BoxCollider":
                    {
                        BoxCollider boxCollider = collider as BoxCollider;
                        Matrix4x4 matrix = Matrix4x4.TRS(boxCollider.transform.TransformPoint(boxCollider.center), boxCollider.transform.rotation, boxCollider.transform.lossyScale);

                        using (new Handles.DrawingScope(matrix))
                        {
                            RenderUtil.DrawWireCube(Vector3.zero, boxCollider.size);
                        }
                    }
                    break;
                case "SphereCollider":
                    {
                        SphereCollider sphereCollider = collider as SphereCollider;
                        Vector3 lScale = sphereCollider.transform.lossyScale;
                        float maxScaleValue = Mathf.Max(new float[] { Mathf.Abs(lScale.x), Mathf.Abs(lScale.y), Mathf.Abs(lScale.z) });
                        Matrix4x4 matrix = Matrix4x4.TRS(sphereCollider.transform.TransformPoint(sphereCollider.center), sphereCollider.transform.rotation, new Vector3(maxScaleValue, maxScaleValue, maxScaleValue));

                        using (new Handles.DrawingScope(matrix))
                        {
                            RenderUtil.DrawWireSphere(Vector3.zero, sphereCollider.radius);
                        }
                    }
                    break;
                case "CapsuleCollider":
                    {
                        CapsuleCollider capsuleCollider = collider as CapsuleCollider;
                        Vector3 lScale = capsuleCollider.transform.lossyScale;
                        Matrix4x4 matrix = Matrix4x4.TRS(capsuleCollider.transform.position, capsuleCollider.transform.rotation, Vector3.one);
                        Vector3 centerHalfScale = new Vector3(capsuleCollider.center.x * lScale.x, capsuleCollider.center.y * lScale.y, capsuleCollider.center.z * lScale.z);
                        float radiusScale = capsuleCollider.radius * Mathf.Max(Mathf.Abs(lScale.x), Mathf.Abs(lScale.z));
                        float halfHeightToSphereCenter = (capsuleCollider.height - (2.0f * radiusScale)) * Mathf.Abs(lScale.y) * 0.5f;

                        Vector3 directionVector = Vector3.up;
                        // X-axis
                        if (capsuleCollider.direction == 0)
                            directionVector = (Quaternion.AngleAxis(90.0f, Vector3.forward) * Vector3.up);
                        // Y-axis
                        else if (capsuleCollider.direction == 1)
                            directionVector = (Quaternion.AngleAxis(90.0f, Vector3.up) * Vector3.up);
                        // Z-axis
                        else if (capsuleCollider.direction == 2)
                            directionVector = (Quaternion.AngleAxis(90.0f, Vector3.right) * Vector3.up);

                        using (new Handles.DrawingScope(matrix))
                        {
                            Vector3 top = centerHalfScale + directionVector * halfHeightToSphereCenter;
                            Vector3 button = centerHalfScale - directionVector * halfHeightToSphereCenter;
                            RenderUtil.DrawWireCapsule(top, button, radiusScale);
                        }
                    }
                    break;
                case "MeshCollider":
                    {
                        MeshCollider meshCollider = collider as MeshCollider;
                        Color orgGizmosColor = Gizmos.color;
                        Matrix4x4 orgMatrix = Gizmos.matrix;

                        Gizmos.color = CastVisualizerManager.Instance.HitMarkerColor;
                        Gizmos.matrix = Matrix4x4.TRS(meshCollider.transform.position, meshCollider.transform.rotation, meshCollider.transform.lossyScale);

                        Gizmos.DrawWireMesh(meshCollider.sharedMesh);

                        Gizmos.color = orgGizmosColor;
                        Gizmos.matrix = orgMatrix;
                    }
                    break;
                //case "TerrainCollider":
                //    {
                //        TerrainCollider terrainCollider = collHit as TerrainCollider;
                //        // Not supported
                //    }
                //    break;
                //case "WheelCollider":
                //    {
                //        WheelCollider wheelCollider = collHit as WheelCollider;
                //        // Not supported
                //    }
                //    break;
            }
        }

        #region Raycast
        [HarmonyPatch(typeof(UnityEngine.PhysicsScene))]
        partial class PhysicsScenePatches
        {
            // extern private static bool Internal_RaycastTest(PhysicsScene physicsScene, Ray ray, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
            [HarmonyPrefix]
            [HarmonyPatch("Internal_RaycastTest")]
            static void Internal_RaycastTest_Prefix(Ray ray, float maxDistance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_1))   // Line/Raycasts
                {
                    RaycastHit hitInfo;
                    Physics.Raycast(ray.origin, ray.direction, out hitInfo, maxDistance);
                }
            }

            // extern private static bool Internal_Raycast(PhysicsScene physicsScene, Ray ray, float maxDistance, ref RaycastHit hit, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
            [HarmonyPostfix]
            [HarmonyPatch("Internal_Raycast")]
            static void Internal_Raycast_Postfix(Ray ray, float maxDistance, ref RaycastHit hit)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_1))   // Line/Raycasts
                {
                    float distance = (float.IsInfinity(maxDistance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : maxDistance;

                    if (hit.collider != null)
                    {
                        distance = hit.distance;
                        AddHitToRender((hit.point, hit.normal));
                    }
                    AddRayToRender((ray, distance));
                }
            }

            // extern private static int Internal_RaycastNonAlloc(PhysicsScene physicsScene, Ray ray, RaycastHit[] raycastHits, float maxDistance, int mask, QueryTriggerInteraction queryTriggerInteraction)
            [HarmonyPrefix]
            [HarmonyPatch("Internal_RaycastNonAlloc")]
            static void Internal_RaycastNonAlloc_Prefix(Ray ray, float maxDistance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_1))   // Line/Raycasts
                {
                    float distance = (float.IsInfinity(maxDistance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : maxDistance;

                    AddRayToRender((ray, distance));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("Internal_RaycastNonAlloc")]
            static void Internal_RaycastNonAlloc_Postfix(RaycastHit[] raycastHits)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_1))   // Line/Raycasts
                {
                    foreach (RaycastHit hit in raycastHits)
                    {
                        if (hit.collider != null)
                        {
                            AddHitToRender((hit.point, hit.normal));
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(UnityEngine.Physics))]
        partial class PhysicsPatches
        {
            //extern static RaycastHit[] Internal_RaycastAll(PhysicsScene physicsScene, Ray ray, float maxDistance, int mask, QueryTriggerInteraction queryTriggerInteraction)
            [HarmonyPrefix]
            [HarmonyPatch("Internal_RaycastAll")]
            static void Internal_RaycastAll_Prefix(Ray ray, float maxDistance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_1))   // Line/Raycasts
                {
                    float distance = (float.IsInfinity(maxDistance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : maxDistance;

                    AddRayToRender((ray, distance));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("Internal_RaycastAll")]
            static void Internal_RaycastAll_Postfix(RaycastHit[] __result)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_1))   // Line/Raycasts
                {
                    foreach (RaycastHit hit in __result)
                    {
                        if (hit.collider != null)
                        {
                            AddHitToRender((hit.point, hit.normal));
                        }
                    }
                }
            }
        }
        #endregion

        #region Boxcast

        [HarmonyPatch(typeof(UnityEngine.PhysicsScene))]
        partial class PhysicsScenePatches
        {
            //extern static private bool Query_BoxCast(PhysicsScene physicsScene, Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, float maxDistance, ref RaycastHit outHit, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
            [HarmonyPostfix]
            [HarmonyPatch("Query_BoxCast")]
            static void Query_BoxCast_Postfix(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, float maxDistance, ref RaycastHit outHit)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_2))   // Boxcasts
                {
                    float distance = (float.IsInfinity(maxDistance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : maxDistance;

                    Vector3 extends = halfExtents * 2.0f;

                    if (outHit.collider != null)
                    {
                        distance = outHit.distance;
                        AddHitToRender((outHit.point, outHit.normal));

                        AddMeshHitToRender((PhysicsMeshShape.Box, Matrix4x4.TRS(center + direction * distance, orientation, extends)));
                    }

                    AddMeshToRender((PhysicsMeshShape.Box, Matrix4x4.TRS(center, orientation, extends), direction * distance));
                }
            }

            // private static extern int Internal_BoxCastNonAlloc(PhysicsScene physicsScene, Vector3 center, Vector3 halfExtents, Vector3 direction, RaycastHit[] raycastHits, Quaternion orientation, float maxDistance, int mask, QueryTriggerInteraction queryTriggerInteraction)
            [HarmonyPrefix]
            [HarmonyPatch("Internal_BoxCastNonAlloc")]
            static void Internal_BoxCastNonAlloc_Prefix(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, float maxDistance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_2))   // Boxcasts
                {
                    float distance = (float.IsInfinity(maxDistance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : maxDistance;

                    Vector3 extends = halfExtents * 2.0f;
                    AddMeshToRender((PhysicsMeshShape.Box, Matrix4x4.TRS(center, orientation, extends), direction * distance));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("Internal_BoxCastNonAlloc")]
            static void Internal_BoxCastNonAlloc_Postfix(RaycastHit[] raycastHits, Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_2))   // Boxcasts
                {
                    Vector3 extends = halfExtents * 2.0f;

                    foreach (RaycastHit hit in raycastHits)
                    {
                        if (hit.collider != null)
                        {
                            AddHitToRender((hit.point, hit.normal));
                            AddMeshHitToRender((PhysicsMeshShape.Box, Matrix4x4.TRS(center + direction * hit.distance, orientation, extends)));
                        }
                    }
                }
            }

            // extern private static int OverlapBoxNonAlloc_Internal(PhysicsScene physicsScene, Vector3 center, Vector3 halfExtents, Collider[] results, Quaternion orientation, int mask, QueryTriggerInteraction queryTriggerInteraction)
            [HarmonyPrefix]
            [HarmonyPatch("OverlapBoxNonAlloc_Internal")]
            static void OverlapBoxNonAlloc_Internal_Prefix(Vector3 center, Vector3 halfExtents, Quaternion orientation)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_5))   // OverlapBoxes
                {
                    Vector3 extends = halfExtents * 2.0f;
                    AddMeshToRender((PhysicsMeshShape.Box, Matrix4x4.TRS(center, orientation, extends), Vector3.zero));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("OverlapBoxNonAlloc_Internal")]
            static void OverlapBoxNonAlloc_Internal_Postfix(Collider[] results)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_5))   // OverlapBoxes
                {
                    foreach (Collider collider in results)
                    {
                        if (collider != null)
                        {
                            AddColliderHitToRender(collider);
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(UnityEngine.Physics))]
        partial class PhysicsPatches
        {
            // private static extern RaycastHit[] Internal_BoxCastAll(PhysicsScene physicsScene, Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
            [HarmonyPrefix]
            [HarmonyPatch("Internal_BoxCastAll")]
            static void Internal_BoxCastAll_Prefix(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, float maxDistance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_2))   // Boxcasts
                {
                    float distance = (float.IsInfinity(maxDistance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : maxDistance;

                    Vector3 extends = halfExtents * 2.0f;
                    AddMeshToRender((PhysicsMeshShape.Box, Matrix4x4.TRS(center, orientation, extends), direction * distance));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("Internal_BoxCastAll")]
            static void Internal_BoxCastAll_Postfix(RaycastHit[] __result, Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_2))   // Boxcasts
                {
                    Vector3 extends = halfExtents * 2.0f;

                    foreach (RaycastHit hit in __result)
                    {
                        if (hit.collider != null)
                        {
                            AddHitToRender((hit.point, hit.normal));
                            AddMeshHitToRender((PhysicsMeshShape.Box, Matrix4x4.TRS(center + direction * hit.distance, orientation, extends)));
                        }
                    }
                }
            }

            // extern private static Collider[] OverlapBox_Internal(PhysicsScene physicsScene, Vector3 center, Vector3 halfExtents, Quaternion orientation, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
            [HarmonyPrefix]
            [HarmonyPatch("OverlapBox_Internal")]
            static void OverlapBox_Internal_Prefix(Vector3 center, Vector3 halfExtents, Quaternion orientation)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_5))   // OverlapBoxes
                {
                    Vector3 extends = halfExtents * 2.0f;
                    AddMeshToRender((PhysicsMeshShape.Box, Matrix4x4.TRS(center, orientation, extends), Vector3.zero));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("OverlapBox_Internal")]
            static void OverlapBox_Internal_Postfix(Collider[] __result)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_5))   // OverlapBoxes
                {
                    foreach (Collider collider in __result)
                    {
                        if (collider.gameObject != null)
                        {
                            AddColliderHitToRender(collider);
                        }
                    }
                }
            }
        }
        #endregion

        #region Spherecast

        [HarmonyPatch(typeof(UnityEngine.PhysicsScene))]
        partial class PhysicsScenePatches
        {
            // extern private static bool Query_SphereCast(PhysicsScene physicsScene, Vector3 origin, float radius, Vector3 direction, float maxDistance, ref RaycastHit hitInfo, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
            [HarmonyPostfix]
            [HarmonyPatch("Query_SphereCast")]
            static void Query_SphereCast_Postfix(Vector3 origin, float radius, Vector3 direction, float maxDistance, ref RaycastHit hitInfo)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_4))   // SphereCasts
                {
                    float distance = (float.IsInfinity(maxDistance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : maxDistance;

                    Vector3 extends = new Vector3(radius, radius, radius);

                    if (hitInfo.collider != null)
                    {
                        distance = hitInfo.distance;
                        AddHitToRender((hitInfo.point, hitInfo.normal));

                        AddMeshHitToRender((PhysicsMeshShape.Sphere, Matrix4x4.TRS(origin + direction * distance, Quaternion.identity, extends)));
                    }

                    AddMeshToRender((PhysicsMeshShape.Sphere, Matrix4x4.TRS(origin, Quaternion.identity, extends), direction * distance));
                }
            }

            // extern private static int Internal_SphereCastNonAlloc(PhysicsScene physicsScene, Vector3 origin, float radius, Vector3 direction, RaycastHit[] raycastHits, float maxDistance, int mask, QueryTriggerInteraction queryTriggerInteraction)
            [HarmonyPrefix]
            [HarmonyPatch("Internal_SphereCastNonAlloc")]
            static void Internal_SphereCastNonAlloc_Prefix(Vector3 origin, float radius, Vector3 direction, float maxDistance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_4))   // SphereCasts
                {
                    float distance = (float.IsInfinity(maxDistance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : maxDistance;

                    Vector3 extends = new Vector3(radius, radius, radius);
                    AddMeshToRender((PhysicsMeshShape.Sphere, Matrix4x4.TRS(origin, Quaternion.identity, extends), direction * distance));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("Internal_SphereCastNonAlloc")]
            static void Internal_SphereCastNonAlloc_Postfix(RaycastHit[] raycastHits, Vector3 origin, float radius, Vector3 direction)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_4))   // SphereCasts
                {
                    Vector3 extends = new Vector3(radius, radius, radius);

                    foreach (RaycastHit hit in raycastHits)
                    {
                        if (hit.collider != null)
                        {
                            AddHitToRender((hit.point, hit.normal));
                            AddMeshHitToRender((PhysicsMeshShape.Sphere, Matrix4x4.TRS(origin + direction * hit.distance, Quaternion.identity, extends)));
                        }
                    }
                }
            }

            // extern private static int OverlapSphereNonAlloc_Internal(PhysicsScene physicsScene, Vector3 position, float radius, Collider[] results, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
            [HarmonyPrefix]
            [HarmonyPatch("OverlapSphereNonAlloc_Internal")]
            static void OverlapSphereNonAlloc_Internal_Prefix(Vector3 position, float radius)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_6))   // OverlapSpheres
                {
                    Vector3 extends = new Vector3(radius, radius, radius);
                    AddMeshToRender((PhysicsMeshShape.Sphere, Matrix4x4.TRS(position, Quaternion.identity, extends), Vector3.zero));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("OverlapSphereNonAlloc_Internal")]
            static void OverlapSphereNonAlloc_Internal_Postfix(Collider[] results)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_6))   // OverlapSpheres
                {
                    foreach (Collider collider in results)
                    {
                        if (collider != null)
                        {
                            AddColliderHitToRender(collider);
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(UnityEngine.Physics))]
        partial class PhysicsPatches
        {
            // extern private static RaycastHit[] Query_SphereCastAll(PhysicsScene physicsScene, Vector3 origin, float radius, Vector3 direction, float maxDistance, int mask, QueryTriggerInteraction queryTriggerInteraction)
            [HarmonyPrefix]
            [HarmonyPatch("Query_SphereCastAll")]
            static void Query_SphereCastAll_Prefix(Vector3 origin, float radius, Vector3 direction, float maxDistance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_4))   // SphereCasts
                {
                    float distance = (float.IsInfinity(maxDistance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : maxDistance;

                    Vector3 extends = new Vector3(radius, radius, radius);
                    AddMeshToRender((PhysicsMeshShape.Sphere, Matrix4x4.TRS(origin, Quaternion.identity, extends), direction * distance));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("Query_SphereCastAll")]
            static void Query_SphereCastAll_Postfix(RaycastHit[] __result, Vector3 origin, float radius, Vector3 direction)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_4))   // SphereCasts
                {
                    Vector3 extends = new Vector3(radius, radius, radius);

                    foreach (RaycastHit hit in __result)
                    {
                        if (hit.collider != null)
                        {
                            AddHitToRender((hit.point, hit.normal));
                            AddMeshHitToRender((PhysicsMeshShape.Sphere, Matrix4x4.TRS(origin + direction * hit.distance, Quaternion.identity, extends)));
                        }
                    }
                }
            }

            // extern private static Collider[] OverlapSphere_Internal(PhysicsScene physicsScene, Vector3 position, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
            [HarmonyPrefix]
            [HarmonyPatch("OverlapSphere_Internal")]
            static void OverlapSphere_Internal_Prefix(Vector3 position, float radius)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_6))   // OverlapSpheres
                {
                    Vector3 extends = new Vector3(radius, radius, radius);
                    AddMeshToRender((PhysicsMeshShape.Sphere, Matrix4x4.TRS(position, Quaternion.identity, extends), Vector3.zero));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("OverlapSphere_Internal")]
            static void OverlapSphere_Internal_Postfix(Collider[] __result)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_6))   // OverlapSpheres
                {
                    foreach (Collider collider in __result)
                    {
                        if (collider != null)
                        {
                            AddColliderHitToRender(collider);
                        }
                    }
                }
            }
        }
        #endregion

        #region Capsulecast

        [HarmonyPatch(typeof(UnityEngine.PhysicsScene))]
        partial class PhysicsScenePatches
        {
            // extern private static bool Query_CapsuleCast(PhysicsScene physicsScene, Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance, ref RaycastHit hitInfo, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
            [HarmonyPostfix]
            [HarmonyPatch("Query_CapsuleCast")]
            static void Query_CapsuleCast_Postfix(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance, ref RaycastHit hitInfo)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_3))   // CapsuleCasts
                {
                    float distance = (float.IsInfinity(maxDistance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : maxDistance;

                    Vector3 extends = new Vector3(radius, radius, radius);

                    if (hitInfo.collider != null)
                    {
                        distance = hitInfo.distance;
                        AddHitToRender((hitInfo.point, hitInfo.normal));

                        AddMeshHitToRender((PhysicsMeshShape.Capsule, new Matrix4x4(point1 + direction * distance, point2 + direction * distance, extends, Vector4.zero)));
                    }

                    AddMeshToRender((PhysicsMeshShape.Capsule, new Matrix4x4(point1, point2, extends, Vector4.zero), direction * distance));
                }
            }

            // extern private static int Internal_CapsuleCastNonAlloc(PhysicsScene physicsScene, Vector3 p0, Vector3 p1, float radius, Vector3 direction, RaycastHit[] raycastHits, float maxDistance, int mask, QueryTriggerInteraction queryTriggerInteraction)
            [HarmonyPrefix]
            [HarmonyPatch("Internal_CapsuleCastNonAlloc")]
            static void Internal_CapsuleCastNonAlloc_Prefix(Vector3 p0, Vector3 p1, float radius, Vector3 direction, float maxDistance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_3))   // CapsuleCasts
                {
                    float distance = (float.IsInfinity(maxDistance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : maxDistance;

                    Vector3 extends = new Vector3(radius, radius, radius);
                    Matrix4x4 mat = new Matrix4x4(p0, p1, extends, Vector4.zero);

                    AddMeshToRender((PhysicsMeshShape.Capsule, mat, direction * distance));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("Internal_CapsuleCastNonAlloc")]
            static void Internal_CapsuleCastNonAlloc_Postfix(RaycastHit[] raycastHits, Vector3 p0, Vector3 p1, float radius, Vector3 direction)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_3))   // CapsuleCasts
                {
                    Vector3 extends = new Vector3(radius, radius, radius);

                    foreach (RaycastHit hit in raycastHits)
                    {
                        if (hit.collider != null)
                        {
                            Matrix4x4 mat = new Matrix4x4(p0 + direction * hit.distance, p1 + direction * hit.distance, extends, Vector4.zero);

                            AddHitToRender((hit.point, hit.normal));
                            AddMeshHitToRender((PhysicsMeshShape.Capsule, mat));
                        }
                    }
                }
            }

            // extern private static int OverlapCapsuleNonAlloc_Internal(PhysicsScene physicsScene, Vector3 point0, Vector3 point1, float radius, Collider[] results, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
            [HarmonyPrefix]
            [HarmonyPatch("OverlapCapsuleNonAlloc_Internal")]
            static void OverlapCapsuleNonAlloc_Internal_Prefix(Vector3 point0, Vector3 point1, float radius)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_7))   // OverlapCapsules
                {
                    Vector3 extends = new Vector3(radius, radius, radius);
                    Matrix4x4 mat = new Matrix4x4(point0, point1, extends, Vector4.zero);

                    AddMeshToRender((PhysicsMeshShape.Capsule, mat, Vector3.zero));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("OverlapCapsuleNonAlloc_Internal")]
            static void OverlapCapsuleNonAlloc_Internal_Postfix(Collider[] results)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_7))   // OverlapCapsules
                {
                    foreach (Collider collider in results)
                    {
                        if (collider != null)
                        {
                            AddColliderHitToRender(collider);
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(UnityEngine.Physics))]
        partial class PhysicsPatches
        {
            // extern private static RaycastHit[] Query_CapsuleCastAll(PhysicsScene physicsScene, Vector3 p0, Vector3 p1, float radius, Vector3 direction, float maxDistance, int mask, QueryTriggerInteraction queryTriggerInteraction)
            [HarmonyPrefix]
            [HarmonyPatch("Query_CapsuleCastAll")]
            static void Query_CapsuleCastAll_Prefix(Vector3 p0, Vector3 p1, float radius, Vector3 direction, float maxDistance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_3))   // CapsuleCasts
                {
                    float distance = (float.IsInfinity(maxDistance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : maxDistance;

                    Vector3 extends = new Vector3(radius, radius, radius);
                    Matrix4x4 mat = new Matrix4x4(p0, p1, extends, Vector4.zero);

                    AddMeshToRender((PhysicsMeshShape.Capsule, mat, direction * distance));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("Query_CapsuleCastAll")]
            static void Query_CapsuleCastAll_Postfix(RaycastHit[] __result, Vector3 p0, Vector3 p1, float radius, Vector3 direction)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_3))   // CapsuleCasts
                {
                    Vector3 extends = new Vector3(radius, radius, radius);

                    foreach (RaycastHit hit in __result)
                    {
                        if (hit.collider != null)
                        {
                            Matrix4x4 mat = new Matrix4x4(p0 + direction * hit.distance, p1 + direction * hit.distance, extends, Vector4.zero);

                            AddHitToRender((hit.point, hit.normal));
                            AddMeshHitToRender((PhysicsMeshShape.Capsule, mat));
                        }
                    }
                }
            }

            // extern private static Collider[] OverlapCapsule_Internal(PhysicsScene physicsScene, Vector3 point0, Vector3 point1, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
            [HarmonyPrefix]
            [HarmonyPatch("OverlapCapsule_Internal")]
            static void OverlapCapsule_Internal_Prefix(Vector3 point0, Vector3 point1, float radius)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_7))   // OverlapCapsules
                {
                    Vector3 extends = new Vector3(radius, radius, radius);
                    Matrix4x4 mat = new Matrix4x4(point0, point1, extends, Vector4.zero);

                    AddMeshToRender((PhysicsMeshShape.Capsule, mat, Vector3.zero));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("OverlapCapsule_Internal")]
            static void OverlapCapsule_Internal_Postfix(Collider[] __result)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysicsFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_7))   // OverlapCapsules
                {
                    foreach (Collider collider in __result)
                    {
                        if (collider != null)
                        {
                            AddColliderHitToRender(collider);
                        }
                    }
                }
            }
        }
        #endregion
    }
}