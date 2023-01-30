using HarmonyLib;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace BgTools.CastVisualizer
{
    public enum Physics2DMeshShape
    {
        Box = 0,
        Circle,
        Capsule,
        Point
    }

    public sealed class Physics2DVisualizer : BaseVisulizer<Physics2DMeshShape, Collider2D>
    {
        // Use DrawGizmo annotation to avoid the pickable Gizmos in OnDrawGizmos()
        [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
        static void DrawGizmos(Physics2DVisualizer script, GizmoType gizmoType)
        {
            BaseVisulizer<Physics2DMeshShape, Collider2D>.DrawGizmos(script, gizmoType);
        }

        protected override bool DrawCondition() { return CastVisualizerManager.Instance.ShowPhysics2DCasts; }
        protected override Color RayColor() { return CastVisualizerManager.Instance.Physics2dRayColor; }

        protected override void DrawMeshes((Physics2DMeshShape, Matrix4x4, Vector3) meshData)
        {
            switch (meshData.Item1)
            {
                case Physics2DMeshShape.Box:
                    using (new Handles.DrawingScope(meshData.Item2))
                    {
                        RenderUtil.DrawWireCube(Vector3.zero, Vector3.one);

                        if (meshData.Item3 != Vector3.zero)
                        {
                            Quaternion rotationCorection = Quaternion.Inverse(meshData.Item2.rotation);
                            RenderUtil.DrawWireCube(rotationCorection * meshData.Item3, Vector3.one);

                            switch (CastVisualizerManager.Instance.CastBodyVisualization)
                            {
                                case CastVisualizerManager.CastBodyVisuType.Line:
                                    Handles.DrawDottedLine(Vector3.zero, rotationCorection * meshData.Item3, 5.0f);
                                    break;
                                case CastVisualizerManager.CastBodyVisuType.WiredBody:
                                    Vector3[] conectionPoints = RenderUtil.GetCube2DConnectingPoints(meshData.Item2.lossyScale * 0.5f, meshData.Item3, meshData.Item2.rotation);
                                    foreach (Vector3 pos in conectionPoints)
                                    {
                                        Handles.DrawDottedLine(pos, rotationCorection * (meshData.Item3) + pos, 5.0f);
                                    }
                                    break;
                            }
                        }
                    }
                    break;
                case Physics2DMeshShape.Circle:
                    using (new Handles.DrawingScope(meshData.Item2))
                    {
                        RenderUtil.DrawCircle2D(Vector2.zero, 1.0f);
                        if (meshData.Item3 != Vector3.zero)
                        {
                            Quaternion rotationCorection = Quaternion.LookRotation(meshData.Item3, Vector3.forward);
                            RenderUtil.DrawCircle2D(meshData.Item3, 1.0f);

                            switch (CastVisualizerManager.Instance.CastBodyVisualization)
                            {
                                case CastVisualizerManager.CastBodyVisuType.Line:
                                    Handles.DrawDottedLine(Vector3.zero, meshData.Item3, 5.0f);
                                    break;
                                case CastVisualizerManager.CastBodyVisuType.WiredBody:
                                    Vector3[] conectionPoints = RenderUtil.GetCicleConnectingPoints();
                                    foreach (Vector3 pos in conectionPoints)
                                    {
                                        Handles.DrawDottedLine(rotationCorection * pos, meshData.Item3 + rotationCorection * pos, 5.0f);
                                    }
                                    break;
                            }
                        }
                    }
                    break;
                case Physics2DMeshShape.Capsule:
                    //(center, Vector4(size.x, size.y, angle, 0.0f), Vector4(direction))
                    Matrix4x4 dataMatrix = meshData.Item2;

                    Vector2 center = dataMatrix.GetColumn(0);
                    Vector2 size = dataMatrix.GetColumn(1);
                    float angle = dataMatrix.GetColumn(1).z;
                    CapsuleDirection2D capsuleDir = (CapsuleDirection2D)dataMatrix.GetColumn(2).x;

                    Matrix4x4 matrix = Matrix4x4.TRS(center, Quaternion.Euler(0.0f, 0.0f, angle), Vector3.one);

                    Vector3 handleHeightAxis, handleRadiusAxis;
                    Vector3 x = Vector3.right * size.x;
                    Vector3 y = Vector3.up * size.y;
                    if (capsuleDir == CapsuleDirection2D.Horizontal)
                    {
                        handleRadiusAxis = y;
                        handleHeightAxis = x;
                    }
                    else
                    {
                        handleRadiusAxis = x;
                        handleHeightAxis = y;
                    }

                    float height, radius = 0.0f;
                    height = handleHeightAxis.magnitude;
                    radius = handleRadiusAxis.magnitude * 0.5f;

                    using (new Handles.DrawingScope(matrix))
                    {
                        RenderUtil.DrawCapsule2D(Vector2.zero, height, radius, capsuleDir);
                        if (meshData.Item3 != Vector3.zero)
                        {
                            Quaternion rotationCorection = Quaternion.Inverse(matrix.rotation);

                            RenderUtil.DrawCapsule2D(rotationCorection * meshData.Item3, height, radius, capsuleDir);

                            switch (CastVisualizerManager.Instance.CastBodyVisualization)
                            {
                                case CastVisualizerManager.CastBodyVisuType.Line:
                                    Handles.DrawDottedLine(Vector3.zero, meshData.Item3, 5.0f);
                                    break;
                                case CastVisualizerManager.CastBodyVisuType.WiredBody:
                                    Vector3[] conectionPoints = RenderUtil.GetCapsule2DConnectingPoints(Vector2.zero, height, radius, capsuleDir, meshData.Item3, matrix.rotation);
                                    foreach (Vector3 pos in conectionPoints)
                                    {
                                        Handles.DrawDottedLine(pos, rotationCorection * meshData.Item3 + pos, 5.0f);
                                    }
                                    break;
                            }
                        }
                    }
                    break;
                case Physics2DMeshShape.Point:
                    using (new Handles.DrawingScope(meshData.Item2))
                    {
                        RenderUtil.DrawCircle2D(Vector2.zero, 0.02f);
                    }
                    break;
            }
        }

        protected override void DrawHitMeshes((Physics2DMeshShape, Matrix4x4) meshData)
        {
            switch (meshData.Item1)
            {
                case Physics2DMeshShape.Box:
                    using (new Handles.DrawingScope(meshData.Item2))
                    {
                        RenderUtil.DrawWireCube(Vector3.zero, Vector3.one);
                    }
                    break;
                case Physics2DMeshShape.Circle:
                    using (new Handles.DrawingScope(meshData.Item2))
                    {
                        RenderUtil.DrawCircle2D(Vector2.zero, 1.0f);
                    }
                    break;
                case Physics2DMeshShape.Capsule:
                    Matrix4x4 dataMatrix = meshData.Item2;

                    Vector2 center = dataMatrix.GetColumn(0);
                    Vector2 size = dataMatrix.GetColumn(1);
                    float angle = dataMatrix.GetColumn(1).z;
                    CapsuleDirection2D capsuleDir = (CapsuleDirection2D)dataMatrix.GetColumn(2).x;

                    Matrix4x4 matrix = Matrix4x4.TRS(center, Quaternion.Euler(0.0f, 0.0f, angle), Vector3.one);

                    Vector3 handleHeightAxis, handleRadiusAxis;
                    Vector3 x = Vector3.right * size.x;
                    Vector3 y = Vector3.up * size.y;
                    if (capsuleDir == CapsuleDirection2D.Horizontal)
                    {
                        handleRadiusAxis = y;
                        handleHeightAxis = x;
                    }
                    else
                    {
                        handleRadiusAxis = x;
                        handleHeightAxis = y;
                    }

                    float height, radius = 0.0f;
                    height = handleHeightAxis.magnitude;
                    radius = handleRadiusAxis.magnitude * 0.5f;

                    using (new Handles.DrawingScope(matrix))
                    {
                        RenderUtil.DrawCapsule2D(Vector2.zero, height, radius, capsuleDir);
                    }
                    break;
            }
        }

        protected override void DrawColliders(Collider2D collider)
        {
            // Skip invallid colliders; fix to early deleted objects from overlap collider
            if (collider == null)
                return;

            switch (collider.GetType().Name)
            {
                case "CompositeCollider2D":
                    {
                        CompositeCollider2D compositeCollider2D = collider as CompositeCollider2D;
                        Matrix4x4 matrix = Matrix4x4.TRS(compositeCollider2D.transform.TransformPoint(compositeCollider2D.offset), compositeCollider2D.transform.rotation, Vector3.one);

                        Vector2[] points;
                        Vector3[] pointsV3;

                        using (new Handles.DrawingScope(matrix))
                        {
                            for (int i = 0; i < compositeCollider2D.pathCount; ++i)
                            {
                                points = new Vector2[compositeCollider2D.GetPathPointCount(i)];
                                compositeCollider2D.GetPath(i, points);

                                pointsV3 = new Vector3[points.Length + 1];
                                for (int j = 0; j < points.Length; ++j)
                                {
                                    pointsV3[j] = points[j];
                                }
                                pointsV3[points.Length] = points[0];

                                Handles.DrawAAPolyLine(pointsV3);

                                if (compositeCollider2D.geometryType == CompositeCollider2D.GeometryType.Outlines && compositeCollider2D.edgeRadius > 0.0f)
                                {
                                    for (int k = 0; k < pointsV3.Length - 1; ++k)
                                    {
                                        RenderUtil.DrawCapsule2D(pointsV3[k], pointsV3[k + 1], compositeCollider2D.edgeRadius);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "BoxCollider2D":
                    {
                        BoxCollider2D boxCollider2D = collider as BoxCollider2D;
                        Matrix4x4 matrix = Matrix4x4.TRS(boxCollider2D.transform.TransformPoint(boxCollider2D.offset), boxCollider2D.transform.rotation, Vector3.one);
                        using (new Handles.DrawingScope(matrix))
                        {
                            Vector3 scaledSize = Vector3.Scale(boxCollider2D.size, boxCollider2D.transform.lossyScale);

                            RenderUtil.DrawCube2D(Vector3.zero, scaledSize);

                            if (boxCollider2D.edgeRadius != 0.0f)
                            {
                                RenderUtil.DrawCube2D(Vector3.zero, scaledSize, boxCollider2D.edgeRadius);
                            }
                        }
                    }
                    break;
                case "CircleCollider2D":
                    {
                        CircleCollider2D circleCollider2D = collider as CircleCollider2D;
                        float maxScaleValue = Mathf.Max(new float[] { Mathf.Abs(circleCollider2D.transform.lossyScale.x), Mathf.Abs(circleCollider2D.transform.lossyScale.y), Mathf.Abs(circleCollider2D.transform.lossyScale.z) });
                        Matrix4x4 matrix = Matrix4x4.TRS(circleCollider2D.transform.TransformPoint(circleCollider2D.offset), circleCollider2D.transform.rotation, new Vector3(maxScaleValue, maxScaleValue, maxScaleValue));

                        using (new Handles.DrawingScope(matrix))
                        {
                            RenderUtil.DrawCircle2D(Vector3.zero, circleCollider2D.radius);
                        }
                    }
                    break;
                case "CapsuleCollider2D":
                    {
                        CapsuleCollider2D capsuleCollider2D = collider as CapsuleCollider2D;
                        Matrix4x4 matrix = Matrix4x4.TRS(capsuleCollider2D.transform.TransformPoint(capsuleCollider2D.offset), capsuleCollider2D.transform.rotation, Vector3.one);

                        Vector3 handleHeightAxis, handleRadiusAxis;
                        Matrix4x4 colliderTransformMatrix = capsuleCollider2D.transform.localToWorldMatrix;
                        Vector3 x = ProjectOntoWorldPlane(colliderTransformMatrix * (Vector3.right * capsuleCollider2D.size.x));
                        Vector3 y = ProjectOntoWorldPlane(colliderTransformMatrix * (Vector3.up * capsuleCollider2D.size.y));
                        if (capsuleCollider2D.direction == CapsuleDirection2D.Horizontal)
                        {
                            handleRadiusAxis = y;
                            handleHeightAxis = x;
                        }
                        else
                        {
                            handleRadiusAxis = x;
                            handleHeightAxis = y;
                        }

                        float height, radius = 0.0f;
                        height = handleHeightAxis.magnitude;
                        radius = handleRadiusAxis.magnitude * 0.5f;

                        using (new Handles.DrawingScope(matrix))
                        {
                            RenderUtil.DrawCapsule2D(Vector2.zero, height, radius, capsuleCollider2D.direction);
                        }
                    }
                    break;
                case "EdgeCollider2D":
                    {
                        EdgeCollider2D edgeCollider2D = collider as EdgeCollider2D;
                        Matrix4x4 matrix = Matrix4x4.TRS(edgeCollider2D.transform.TransformPoint(edgeCollider2D.offset), edgeCollider2D.transform.rotation, Vector3.one);

                        Vector3[] points = new Vector3[edgeCollider2D.points.Length];
                        for (int i = 0; i < edgeCollider2D.points.Length; ++i)
                        {
                            Vector2 vec = edgeCollider2D.points[i];
                            points[i] = Vector3.Scale(vec, edgeCollider2D.transform.lossyScale);
                        }
                        using (new Handles.DrawingScope(matrix))
                        {
                            Handles.DrawAAPolyLine(points);

                            for (int i = 0; i < points.Length - 1; ++i)
                            {
                                RenderUtil.DrawCapsule2D(points[i], points[i + 1], edgeCollider2D.edgeRadius);
                            }
                        }
                    }
                    break;
                case "PolygonCollider2D":
                    {
                        PolygonCollider2D polygonCollider2D = collider as PolygonCollider2D;
                        Matrix4x4 matrix = Matrix4x4.TRS(polygonCollider2D.transform.TransformPoint(polygonCollider2D.offset), polygonCollider2D.transform.rotation, polygonCollider2D.transform.lossyScale);

                        Vector3[] points = new Vector3[polygonCollider2D.points.Length + 1];
                        for (int i = 0; i < polygonCollider2D.points.Length; ++i)
                        {
                            Vector2 vec = polygonCollider2D.points[i];
                            points[i] = vec;
                        }
                        points[polygonCollider2D.points.Length] = polygonCollider2D.points[0];

                        using (new Handles.DrawingScope(matrix))
                        {
                            Handles.DrawAAPolyLine(points);
                        }
                    }
                    break;
            }
        }

        // return specified world vector projected onto world x/y plane
        private Vector3 ProjectOntoWorldPlane(Vector3 worldVector)
        {
            worldVector.z = 0f;
            return worldVector;
        }

        #region Linecast
        [HarmonyPatch(typeof(UnityEngine.PhysicsScene2D))]
        partial class PhysicsScene2DPatches
        {
            // extern private static RaycastHit2D Linecast_Internal(PhysicsScene2D physicsScene, Vector2 start, Vector2 end, ContactFilter2D contactFilter)
            [HarmonyPostfix]
            [HarmonyPatch("Linecast_Internal")]
            static void Linecast_Internal_Postfix(Vector2 start, Vector2 end, RaycastHit2D __result)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_1))   // Line/Raycasts
                {
                    Vector2 dir = end - start;
                    float distance = dir.magnitude;

                    if (__result.collider != null)
                    {
                        distance = __result.distance;
                        AddHitToRender((__result.point, Vector3.back));
                    }

                    AddRayToRender((new Ray(start, dir), distance));
                }
            }

            // extern private static int LinecastNonAllocList_Internal(PhysicsScene2D physicsScene, Vector2 start, Vector2 end, ContactFilter2D contactFilter, [NotNull] List<RaycastHit2D> results)
            [HarmonyPrefix]
            [HarmonyPatch("LinecastNonAllocList_Internal")]
            static void LinecastNonAllocList_Internal_Prefix(Vector2 start, Vector2 end)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_1))   // Line/Raycasts
                {
                    Vector2 dir = end - start;
                    AddRayToRender((new Ray(start, dir), dir.magnitude));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("LinecastNonAllocList_Internal")]
            static void LinecastNonAllocList_Internal_Postfix(List<RaycastHit2D> results)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_1))   // Line/Raycasts
                {
                    foreach (RaycastHit2D hit in results)
                    {
                        if (hit.collider != null)
                        {
                            AddHitToRender((hit.point, Vector3.back));
                        }
                    }
                }
            }

            // extern private static int LinecastArray_Internal(PhysicsScene2D physicsScene, Vector2 start, Vector2 end, ContactFilter2D contactFilter, [NotNull] RaycastHit2D[] results)
            [HarmonyPrefix]
            [HarmonyPatch("LinecastArray_Internal")]
            static void LinecastArray_Internal_Prefix(Vector2 start, Vector2 end)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_1))   // Line/Raycasts
                {
                    Vector2 dir = end - start;
                    AddRayToRender((new Ray(start, dir), dir.magnitude));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("LinecastArray_Internal")]
            static void LinecastArray_Internal_Postfix(RaycastHit2D[] results)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_1))   // Line/Raycasts
                {
                    foreach (RaycastHit2D hit in results)
                    {
                        if (hit.collider != null)
                        {
                            AddHitToRender((hit.point, Vector3.back));
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(UnityEngine.Physics2D))]
        partial class Physics2DPatches
        {
            // extern private static RaycastHit2D[] LinecastAll_Internal(PhysicsScene2D physicsScene, Vector2 start, Vector2 end, ContactFilter2D contactFilter)
            [HarmonyPrefix]
            [HarmonyPatch("LinecastAll_Internal")]
            static void LinecastAll_Internal_Prefix(Vector2 start, Vector2 end)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_1))   // Line/Raycasts
                {
                    Vector2 dir = end - start;
                    AddRayToRender((new Ray(start, dir), dir.magnitude));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("LinecastAll_Internal")]
            static void LinecastAll_Internal_Postfix(RaycastHit2D[] __result)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_1))   // Line/Raycasts
                {
                    foreach (RaycastHit2D hit in __result)
                    {
                        if (hit.collider != null)
                        {
                            AddHitToRender((hit.point, Vector3.back));
                        }
                    }
                }
            }
        }
        #endregion

        #region Raycast
        [HarmonyPatch(typeof(UnityEngine.PhysicsScene2D))]
        partial class PhysicsScene2DPatches
        {
            // extern private static RaycastHit2D Raycast_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 direction, float distance, ContactFilter2D contactFilter)
            [HarmonyPostfix]
            [HarmonyPatch("Raycast_Internal")]
            static void Raycast_Internal_Postfix(RaycastHit2D __result, Vector2 origin, Vector2 direction, float distance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_1))   // Line/Raycasts
                {
                    float safeDistance = (float.IsInfinity(distance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : distance;

                    if (__result.collider != null)
                    {
                        safeDistance = __result.distance;
                        AddHitToRender((__result.point, Vector3.back));
                    }

                    AddRayToRender((new Ray(origin, direction), safeDistance));
                }
            }

            // extern private static int RaycastArray_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 direction, float distance, ContactFilter2D contactFilter, [NotNull] RaycastHit2D[] results)
            [HarmonyPrefix]
            [HarmonyPatch("RaycastArray_Internal")]
            static void RaycastArray_Internal_Prefix(Vector2 origin, Vector2 direction, float distance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_1))   // Line/Raycasts
                {
                    float safeDistance = (float.IsInfinity(distance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : distance;

                    AddRayToRender((new Ray(origin, direction), safeDistance));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("RaycastArray_Internal")]
            static void RaycastArray_Internal_Postfix(RaycastHit2D[] results)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_1))   // Line/Raycasts
                {
                    foreach (RaycastHit2D hit in results)
                    {
                        if (hit.collider != null)
                        {
                            AddHitToRender((hit.point, Vector3.back));
                        }
                    }
                }
            }

            // extern private static int RaycastList_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 direction, float distance, ContactFilter2D contactFilter, [NotNull] List<RaycastHit2D> results)
            [HarmonyPrefix]
            [HarmonyPatch("RaycastList_Internal")]
            static void RaycastList_Internal_Prefix(Vector2 origin, Vector2 direction, float distance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_1))   // Line/Raycasts
                {
                    float safeDistance = (float.IsInfinity(distance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : distance;

                    AddRayToRender((new Ray(origin, direction), safeDistance));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("RaycastList_Internal")]
            static void RaycastList_Internal_Postfix(List<RaycastHit2D> results)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_1))   // Line/Raycasts
                {
                    foreach (RaycastHit2D hit in results)
                    {
                        if (hit.collider != null)
                        {
                            AddHitToRender((hit.point, Vector3.back));
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(UnityEngine.Physics2D))]
        partial class Physics2DPatches
        {
            // extern private static RaycastHit2D[] RaycastAll_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 direction, float distance, ContactFilter2D contactFilter)
            [HarmonyPrefix]
            [HarmonyPatch("RaycastAll_Internal")]
            static void RaycastAll_Internal_Prefix(Vector2 origin, Vector2 direction, float distance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_1))   // Line/Raycasts
                {
                    float safeDistance = (float.IsInfinity(distance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : distance;

                    AddRayToRender((new Ray(origin, direction), safeDistance));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("RaycastAll_Internal")]
            static void Raycast_Internal_Postfix(ref RaycastHit2D[] __result)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_1))   // Line/Raycasts
                {
                    foreach (RaycastHit2D hit in __result)
                    {
                        if (hit.collider != null)
                        {
                            AddHitToRender((hit.point, Vector3.back));
                        }
                    }
                }
            }
        }
        #endregion

        #region Boxcast
        [HarmonyPatch(typeof(UnityEngine.PhysicsScene2D))]
        partial class PhysicsScene2DPatches
        {
            // extern private static RaycastHit2D BoxCast_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter)
            [HarmonyPostfix]
            [HarmonyPatch("BoxCast_Internal")]
            static void BoxCast_Internal_Postfix(RaycastHit2D __result, Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_2))   // Boxcasts
                {
                    float safeDistance = (float.IsInfinity(distance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : distance;
                    Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, angle);

                    if (__result.collider != null)
                    {
                        safeDistance = __result.distance;
                        AddHitToRender((__result.point, Vector3.back));

                        AddMeshHitToRender((Physics2DMeshShape.Box, Matrix4x4.TRS(origin + direction * safeDistance, rotation, size)));
                    }

                    AddMeshToRender((Physics2DMeshShape.Box, Matrix4x4.TRS(origin, rotation, size), direction * safeDistance));
                }
            }

            // extern private static int BoxCastArray_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter, [NotNull] RaycastHit2D[] results)
            [HarmonyPrefix]
            [HarmonyPatch("BoxCastArray_Internal")]
            static void BoxCastArray_Internal_Prefix(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_2))   // Boxcasts
                {
                    float safeDistance = (float.IsInfinity(distance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : distance;

                    AddMeshToRender((Physics2DMeshShape.Box, Matrix4x4.TRS(origin, Quaternion.Euler(0.0f, 0.0f, angle), size), direction * safeDistance));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("BoxCastArray_Internal")]
            static void BoxCastArray_Internal_Postfix(RaycastHit2D[] results, Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_2))   // Boxcasts
                {
                    foreach (RaycastHit2D hit in results)
                    {
                        float safeDistance = (float.IsInfinity(distance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : distance;
                        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, angle);

                        if (hit.collider != null)
                        {
                            AddHitToRender((hit.point, Vector3.back));
                            AddMeshHitToRender((Physics2DMeshShape.Box, Matrix4x4.TRS(origin + direction * hit.distance, rotation, size)));
                        }
                    }
                }
            }

            // extern private static int BoxCastList_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter, [NotNull] List<RaycastHit2D> results)
            [HarmonyPrefix]
            [HarmonyPatch("BoxCastList_Internal")]
            static void BoxCastList_Internal_Prefix(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_2))   // Boxcasts
                {
                    float safeDistance = (float.IsInfinity(distance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : distance;

                    AddMeshToRender((Physics2DMeshShape.Box, Matrix4x4.TRS(origin, Quaternion.Euler(0.0f, 0.0f, angle), size), direction * safeDistance));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("BoxCastList_Internal")]
            static void BoxCastList_Internal_Postfix(List<RaycastHit2D> results, Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_2))   // Boxcasts
                {
                    foreach (RaycastHit2D hit in results)
                    {
                        float safeDistance = (float.IsInfinity(distance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : distance;
                        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, angle);

                        if (hit.collider != null)
                        {
                            AddHitToRender((hit.point, Vector3.back));
                            AddMeshHitToRender((Physics2DMeshShape.Box, Matrix4x4.TRS(origin + direction * hit.distance, rotation, size)));
                        }
                    }
                }
            }

            // extern private static Collider2D OverlapBox_Internal(PhysicsScene2D physicsScene, Vector2 point, Vector2 size, float angle, ContactFilter2D contactFilter)
            [HarmonyPrefix]
            [HarmonyPatch("OverlapBox_Internal")]
            static void OverlapBox_Internal_Prefix(Vector2 point, Vector2 size, float angle)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_5))   // OverlapBoxes
                {
                    AddMeshToRender((Physics2DMeshShape.Box, Matrix4x4.TRS(point, Quaternion.Euler(0.0f, 0.0f, angle), size), Vector3.zero));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("OverlapBox_Internal")]
            static void OverlapBox_Internal_Postfix(Collider2D __result)
            {
                if (__result != null && CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_5))   // OverlapBoxes
                {
                    AddColliderHitToRender(__result);
                }
            }

            // extern private static int OverlapBoxArray_Internal(PhysicsScene2D physicsScene, Vector2 point, Vector2 size, float angle, ContactFilter2D contactFilter, [NotNull] Collider2D[] results)
            [HarmonyPrefix]
            [HarmonyPatch("OverlapBoxArray_Internal")]
            static void OverlapBoxArray_Internal_Prefix(Vector2 point, Vector2 size, float angle)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_5))   // OverlapBoxes
                {
                    AddMeshToRender((Physics2DMeshShape.Box, Matrix4x4.TRS(point, Quaternion.Euler(0.0f, 0.0f, angle), size), Vector3.zero));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("OverlapBoxArray_Internal")]
            static void OverlapBoxArray_Internal_Postfix(Collider2D[] results)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_5))   // OverlapBoxes
                {
                    foreach (Collider2D collider in results)
                    {
                        if (collider != null)
                        {
                            AddColliderHitToRender(collider);
                        }
                    }
                }
            }

            // extern private static int OverlapBoxList_Internal(PhysicsScene2D physicsScene, Vector2 point, Vector2 size, float angle, ContactFilter2D contactFilter, [NotNull] List<Collider2D> results)
            [HarmonyPrefix]
            [HarmonyPatch("OverlapBoxList_Internal")]
            static void OverlapBoxList_Internal_Prefix(Vector2 point, Vector2 size, float angle)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_5))   // OverlapBoxes
                {
                    AddMeshToRender((Physics2DMeshShape.Box, Matrix4x4.TRS(point, Quaternion.Euler(0.0f, 0.0f, angle), size), Vector3.zero));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("OverlapBoxList_Internal")]
            static void OverlapBoxList_Internal_Postfix(List<Collider2D> results)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_5))   // OverlapBoxes
                {
                    foreach (Collider2D collider in results)
                    {
                        if (collider != null)
                        {
                            AddColliderHitToRender(collider);
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(UnityEngine.Physics2D))]
        partial class Physics2DPatches
        {
            // extern private static RaycastHit2D[] BoxCastAll_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter)
            [HarmonyPrefix]
            [HarmonyPatch("BoxCastAll_Internal")]
            static void BoxCastAll_Internal_Prefix(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_2))   // Boxcasts
                {
                    float safeDistance = (float.IsInfinity(distance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : distance;

                    AddMeshToRender((Physics2DMeshShape.Box, Matrix4x4.TRS(origin, Quaternion.Euler(0.0f, 0.0f, angle), size), direction * safeDistance));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("BoxCastAll_Internal")]
            static void BoxCastAll_Internal_Postfix(RaycastHit2D[] __result, Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_2))   // Boxcasts
                {
                    foreach (RaycastHit2D hit in __result)
                    {
                        float safeDistance = (float.IsInfinity(distance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : distance;
                        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, angle);

                        if (hit.collider != null)
                        {
                            AddHitToRender((hit.point, Vector3.back));
                            AddMeshHitToRender((Physics2DMeshShape.Box, Matrix4x4.TRS(origin + direction * hit.distance, rotation, size)));
                        }
                    }
                }
            }

            // extern private static Collider2D[] OverlapBoxAll_Internal(PhysicsScene2D physicsScene, Vector2 point, Vector2 size, float angle, ContactFilter2D contactFilter)
            [HarmonyPrefix]
            [HarmonyPatch("OverlapBoxAll_Internal")]
            static void OverlapBoxAll_Internal_Prefix(Vector2 point, Vector2 size, float angle)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_5))   // OverlapBoxes
                {
                    AddMeshToRender((Physics2DMeshShape.Box, Matrix4x4.TRS(point, Quaternion.Euler(0.0f, 0.0f, angle), size), Vector3.zero));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("OverlapBoxAll_Internal")]
            static void OverlapBoxAll_Internal_Postfix(Collider2D[] __result)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_5))   // OverlapBoxes
                {
                    foreach (Collider2D collider in __result)
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

        #region Circlecast
        [HarmonyPatch(typeof(UnityEngine.PhysicsScene2D))]
        partial class PhysicsScene2DPatches
        {
            // extern private static RaycastHit2D CircleCast_Internal(PhysicsScene2D physicsScene, Vector2 origin, float radius, Vector2 direction, float distance, ContactFilter2D contactFilter)
            [HarmonyPostfix]
            [HarmonyPatch("CircleCast_Internal")]
            static void CircleCast_Internal_Postfix(RaycastHit2D __result, Vector2 origin, float radius, Vector2 direction, float distance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_4))   // CircleCasts
                {
                    float safeDistance = (float.IsInfinity(distance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : distance;
                    Vector3 size = new Vector3(radius, radius, radius);

                    if (__result.collider != null)
                    {
                        safeDistance = __result.distance;
                        AddHitToRender((__result.point, Vector3.back));

                        AddMeshHitToRender((Physics2DMeshShape.Circle, Matrix4x4.TRS(origin + direction * safeDistance, Quaternion.identity, size)));
                    }

                    AddMeshToRender((Physics2DMeshShape.Circle, Matrix4x4.TRS(origin, Quaternion.identity, size), direction * safeDistance));
                }
            }

            // extern private static int CircleCastArray_Internal(PhysicsScene2D physicsScene, Vector2 origin, float radius, Vector2 direction, float distance, ContactFilter2D contactFilter, [NotNull] RaycastHit2D[] results)
            [HarmonyPrefix]
            [HarmonyPatch("CircleCastArray_Internal")]
            static void CircleCastArray_Internal_Prefix(Vector2 origin, float radius, Vector2 direction, float distance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_4))   // CircleCasts
                {
                    float safeDistance = (float.IsInfinity(distance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : distance;
                    Vector3 size = new Vector3(radius, radius, radius);

                    AddMeshToRender((Physics2DMeshShape.Circle, Matrix4x4.TRS(origin, Quaternion.identity, size), direction * safeDistance));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("CircleCastArray_Internal")]
            static void CircleCastArray_Internal_Postfix(RaycastHit2D[] results, Vector2 origin, float radius, Vector2 direction, float distance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_4))   // CircleCasts
                {
                    foreach (RaycastHit2D hit in results)
                    {
                        float safeDistance = (float.IsInfinity(distance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : distance;
                        Vector3 size = new Vector3(radius, radius, radius);

                        if (hit.collider != null)
                        {
                            AddHitToRender((hit.point, Vector3.back));
                            AddMeshHitToRender((Physics2DMeshShape.Circle, Matrix4x4.TRS(origin + direction * hit.distance, Quaternion.identity, size)));
                        }
                    }
                }
            }

            // extern private static int CircleCastList_Internal(PhysicsScene2D physicsScene, Vector2 origin, float radius, Vector2 direction, float distance, ContactFilter2D contactFilter, [NotNull] List<RaycastHit2D> results)
            [HarmonyPrefix]
            [HarmonyPatch("CircleCastList_Internal")]
            static void CircleCastList_Internal_Prefix(Vector2 origin, float radius, Vector2 direction, float distance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_4))   // CircleCasts
                {
                    float safeDistance = (float.IsInfinity(distance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : distance;
                    Vector3 size = new Vector3(radius, radius, radius);

                    AddMeshToRender((Physics2DMeshShape.Circle, Matrix4x4.TRS(origin, Quaternion.identity, size), direction * safeDistance));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("CircleCastList_Internal")]
            static void CircleCastList_Internal_Postfix(List<RaycastHit2D> results, Vector2 origin, float radius, Vector2 direction, float distance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_4))   // CircleCasts
                {
                    foreach (RaycastHit2D hit in results)
                    {
                        float safeDistance = (float.IsInfinity(distance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : distance;
                        Vector3 size = new Vector3(radius, radius, radius);

                        if (hit.collider != null)
                        {
                            AddHitToRender((hit.point, Vector3.back));
                            AddMeshHitToRender((Physics2DMeshShape.Circle, Matrix4x4.TRS(origin + direction * hit.distance, Quaternion.identity, size)));
                        }
                    }
                }
            }

            // extern private static Collider2D OverlapCircle_Internal(PhysicsScene2D physicsScene, Vector2 point, float radius, ContactFilter2D contactFilter)
            [HarmonyPrefix]
            [HarmonyPatch("OverlapCircle_Internal")]
            static void OverlapCircle_Internal_Prefix(Vector2 point, float radius)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_6))   // OverlapCircles
                {
                    Vector3 size = new Vector3(radius, radius, radius);

                    AddMeshToRender((Physics2DMeshShape.Circle, Matrix4x4.TRS(point, Quaternion.identity, size), Vector3.zero));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("OverlapCircle_Internal")]
            static void OverlapCircle_Internal_Postfix(Collider2D __result)
            {
                if (__result != null && CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_6))   // OverlapCircles
                {
                    AddColliderHitToRender(__result);
                }
            }

            // extern private static int OverlapCircleArray_Internal(PhysicsScene2D physicsScene, Vector2 point, float radius, ContactFilter2D contactFilter, [NotNull] Collider2D[] results)
            [HarmonyPrefix]
            [HarmonyPatch("OverlapCircleArray_Internal")]
            static void OverlapCircleArray_Internal_Prefix(Vector2 point, float radius)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_6))   // OverlapCircles
                {
                    Vector3 size = new Vector3(radius, radius, radius);

                    AddMeshToRender((Physics2DMeshShape.Circle, Matrix4x4.TRS(point, Quaternion.identity, size), Vector3.zero));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("OverlapCircleArray_Internal")]
            static void OverlapCircleArray_Internal_Postfix(Collider2D[] results)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_6))   // OverlapCircles
                {
                    foreach (Collider2D hit in results)
                    {
                        if (hit != null)
                        {
                            AddColliderHitToRender(hit);
                        }
                    }
                }
            }

            // extern private static int OverlapCircleList_Internal(PhysicsScene2D physicsScene, Vector2 point, float radius, ContactFilter2D contactFilter, [NotNull] List<Collider2D> results)
            [HarmonyPrefix]
            [HarmonyPatch("OverlapCircleList_Internal")]
            static void OverlapCircleList_Internal_Prefix(Vector2 point, float radius)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_6))   // OverlapCircles
                {
                    Vector3 size = new Vector3(radius, radius, radius);

                    AddMeshToRender((Physics2DMeshShape.Circle, Matrix4x4.TRS(point, Quaternion.identity, size), Vector3.zero));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("OverlapCircleList_Internal")]
            static void OverlapCircleList_Internal_Postfix(List<Collider2D> results)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_6))   // OverlapCircles
                {
                    foreach (Collider2D hit in results)
                    {
                        if (hit != null)
                        {
                            AddColliderHitToRender(hit);
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(UnityEngine.Physics2D))]
        partial class Physics2DPatches
        {
            // extern private static RaycastHit2D[] CircleCastAll_Internal(PhysicsScene2D physicsScene, Vector2 origin, float radius, Vector2 direction, float distance, ContactFilter2D contactFilter)
            [HarmonyPrefix]
            [HarmonyPatch("CircleCastAll_Internal")]
            static void CircleCastAll_Internal_Prefix(Vector2 origin, float radius, Vector2 direction, float distance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_4))   // CircleCasts
                {
                    float safeDistance = (float.IsInfinity(distance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : distance;
                    Vector3 size = new Vector3(radius, radius, radius);

                    AddMeshToRender((Physics2DMeshShape.Circle, Matrix4x4.TRS(origin, Quaternion.identity, size), direction * safeDistance));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("CircleCastAll_Internal")]
            static void CircleCastAll_Internal_Postfix(RaycastHit2D[] __result, Vector2 origin, float radius, Vector2 direction, float distance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_4))   // CircleCasts
                {
                    foreach (RaycastHit2D hit in __result)
                    {
                        float safeDistance = (float.IsInfinity(distance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : distance;
                        Vector3 size = new Vector3(radius, radius, radius);

                        if (hit.collider != null)
                        {
                            AddHitToRender((hit.point, Vector3.back));
                            AddMeshHitToRender((Physics2DMeshShape.Circle, Matrix4x4.TRS(origin + direction * hit.distance, Quaternion.identity, size)));
                        }
                    }
                }
            }

            // extern private static Collider2D[] OverlapCircleAll_Internal(PhysicsScene2D physicsScene, Vector2 point, float radius, ContactFilter2D contactFilter)
            [HarmonyPrefix]
            [HarmonyPatch("OverlapCircleAll_Internal")]
            static void OverlapCircleAll_Internal_Prefix(Vector2 point, float radius)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_6))   // OverlapCircles
                {
                    Vector3 size = new Vector3(radius, radius, radius);

                    AddMeshToRender((Physics2DMeshShape.Circle, Matrix4x4.TRS(point, Quaternion.identity, size), Vector3.zero));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("OverlapCircleAll_Internal")]
            static void OverlapCircleAll_Internal_Postfix(Collider2D[] __result)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_6))   // OverlapCircles
                {
                    foreach (Collider2D collider in __result)
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
        [HarmonyPatch(typeof(UnityEngine.PhysicsScene2D))]
        partial class PhysicsScene2DPatches
        {
            // extern private static RaycastHit2D CapsuleCast_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter)
            [HarmonyPostfix]
            [HarmonyPatch("CapsuleCast_Internal")]
            static void CapsuleCast_Internal_Postfix(RaycastHit2D __result, Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_3))   // CapsuleCasts
                {
                    float safeDistance = (float.IsInfinity(distance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : distance;

                    Vector3 dataVec = new Vector3(size.x, size.y, angle);
                    Vector3 dataVec2 = new Vector3((float)capsuleDirection, (float)capsuleDirection, (float)capsuleDirection);

                    if (__result.collider != null)
                    {
                        safeDistance = __result.distance;
                        AddHitToRender((__result.point, Vector3.back));

                        AddMeshHitToRender((Physics2DMeshShape.Capsule, new Matrix4x4(origin + direction * safeDistance, dataVec, dataVec2, Vector3.zero)));
                    }

                    AddMeshToRender((Physics2DMeshShape.Capsule, new Matrix4x4(origin, dataVec, dataVec2, Vector3.zero), direction * safeDistance));
                }
            }

            // extern private static int CapsuleCastArray_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter, [NotNull] RaycastHit2D[] results)
            [HarmonyPrefix]
            [HarmonyPatch("CapsuleCastArray_Internal")]
            static void CapsuleCastArray_Internal_Prefix(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_3))   // CapsuleCasts
                {
                    float safeDistance = (float.IsInfinity(distance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : distance;

                    Vector3 dataVec = new Vector3(size.x, size.y, angle);
                    Vector3 dataVec2 = new Vector3((float)capsuleDirection, (float)capsuleDirection, (float)capsuleDirection);

                    AddMeshToRender((Physics2DMeshShape.Capsule, new Matrix4x4(origin, dataVec, dataVec2, Vector3.zero), direction * safeDistance));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("CapsuleCastArray_Internal")]
            static void CapsuleCastArray_Internal_Postfix(RaycastHit2D[] results, Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_3))   // CapsuleCasts
                {
                    foreach (RaycastHit2D hit in results)
                    {
                        if (hit.collider != null)
                        {
                            Vector3 dataVec = new Vector3(size.x, size.y, angle);
                            Vector3 dataVec2 = new Vector3((float)capsuleDirection, (float)capsuleDirection, (float)capsuleDirection);

                            AddHitToRender((hit.point, Vector3.back));
                            AddMeshHitToRender((Physics2DMeshShape.Capsule, new Matrix4x4(origin + direction * hit.distance, dataVec, dataVec2, Vector3.zero)));
                        }
                    }
                }
            }

            // extern private static int CapsuleCastList_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter, [NotNull] List<RaycastHit2D> results)
            [HarmonyPrefix]
            [HarmonyPatch("CapsuleCastList_Internal")]
            static void CapsuleCastList_Internal_Prefix(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_3))   // CapsuleCasts
                {
                    float safeDistance = (float.IsInfinity(distance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : distance;

                    Vector3 dataVec = new Vector3(size.x, size.y, angle);
                    Vector3 dataVec2 = new Vector3((float)capsuleDirection, (float)capsuleDirection, (float)capsuleDirection);

                    AddMeshToRender((Physics2DMeshShape.Capsule, new Matrix4x4(origin, dataVec, dataVec2, Vector3.zero), direction * safeDistance));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("CapsuleCastList_Internal")]
            static void CapsuleCastList_Internal_Postfix(List<RaycastHit2D> results, Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_3))   // CapsuleCasts
                {
                    foreach (RaycastHit2D hit in results)
                    {
                        if (hit.collider != null)
                        {
                            Vector3 dataVec = new Vector3(size.x, size.y, angle);
                            Vector3 dataVec2 = new Vector3((float)capsuleDirection, (float)capsuleDirection, (float)capsuleDirection);

                            AddHitToRender((hit.point, Vector3.back));
                            AddMeshHitToRender((Physics2DMeshShape.Capsule, new Matrix4x4(origin + direction * hit.distance, dataVec, dataVec2, Vector3.zero)));
                        }
                    }
                }
            }

            // extern private static Collider2D OverlapCapsule_Internal(PhysicsScene2D physicsScene, Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, ContactFilter2D contactFilter)
            [HarmonyPrefix]
            [HarmonyPatch("OverlapCapsule_Internal")]
            static void OverlapCapsule_Internal_Prefix(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_7))   // OverlapCapsules
                {
                    Vector3 dataVec = new Vector3(size.x, size.y, angle);
                    Vector3 dataVec2 = new Vector3((float)direction, (float)direction, (float)direction);

                    AddMeshToRender((Physics2DMeshShape.Capsule, new Matrix4x4(point, dataVec, dataVec2, Vector3.zero), Vector3.zero));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("OverlapCapsule_Internal")]
            static void OverlapCapsule_Internal_Postfix(Collider2D __result)
            {
                if (__result != null && CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_7))   // OverlapCapsules
                {
                    AddColliderHitToRender(__result);
                }
            }

            // extern private static int OverlapCapsuleArray_Internal(PhysicsScene2D physicsScene, Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, ContactFilter2D contactFilter, [NotNull] Collider2D[] results)
            [HarmonyPrefix]
            [HarmonyPatch("OverlapCapsuleArray_Internal")]
            static void OverlapCapsuleArray_Internal_Prefix(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_7))   // OverlapCapsules
                {
                    Vector3 dataVec = new Vector3(size.x, size.y, angle);
                    Vector3 dataVec2 = new Vector3((float)direction, (float)direction, (float)direction);

                    AddMeshToRender((Physics2DMeshShape.Capsule, new Matrix4x4(point, dataVec, dataVec2, Vector3.zero), Vector3.zero));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("OverlapCapsuleArray_Internal")]
            static void OverlapCapsuleArray_Internal_Postfix(Collider2D[] results)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_7))   // OverlapCapsules
                {
                    foreach (Collider2D collider in results)
                    {
                        if (collider != null)
                        {
                            AddColliderHitToRender(collider);
                        }
                    }
                }
            }

            // extern private static int OverlapCapsuleList_Internal(PhysicsScene2D physicsScene, Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, ContactFilter2D contactFilter, [NotNull] List<Collider2D> results)
            [HarmonyPrefix]
            [HarmonyPatch("OverlapCapsuleList_Internal")]
            static void OverlapCapsuleList_Internal_Prefix(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_7))   // OverlapCapsules
                {
                    Vector3 dataVec = new Vector3(size.x, size.y, angle);
                    Vector3 dataVec2 = new Vector3((float)direction, (float)direction, (float)direction);

                    AddMeshToRender((Physics2DMeshShape.Capsule, new Matrix4x4(point, dataVec, dataVec2, Vector3.zero), Vector3.zero));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("OverlapCapsuleList_Internal")]
            static void OverlapCapsuleList_Internal_Postfix(List<Collider2D> results)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_7))   // OverlapCapsules
                {
                    foreach (Collider2D collider in results)
                    {
                        if (collider != null)
                        {
                            AddColliderHitToRender(collider);
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(UnityEngine.Physics2D))]
        partial class Physics2DPatches
        {
            // extern private static RaycastHit2D[] CapsuleCastAll_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter)
            [HarmonyPrefix]
            [HarmonyPatch("CapsuleCastAll_Internal")]
            static void CapsuleCastAll_Internal_Prefix(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_3))   // CapsuleCasts
                {
                    float safeDistance = (float.IsInfinity(distance)) ? CastVisualizerManager.INF_RAY_DRAW_LENGHT : distance;

                    Vector3 dataVec = new Vector3(size.x, size.y, angle);
                    Vector3 dataVec2 = new Vector3((float)capsuleDirection, (float)capsuleDirection, (float)capsuleDirection);

                    AddMeshToRender((Physics2DMeshShape.Capsule, new Matrix4x4(origin, dataVec, dataVec2, Vector3.zero), direction * safeDistance));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("CapsuleCastAll_Internal")]
            static void CapsuleCastAll_Internal_Postfix(RaycastHit2D[] __result, Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_3))   // CapsuleCasts
                {
                    foreach (RaycastHit2D hit in __result)
                    {
                        if (hit.collider != null)
                        {
                            Vector3 dataVec = new Vector3(size.x, size.y, angle);
                            Vector3 dataVec2 = new Vector3((float)capsuleDirection, (float)capsuleDirection, (float)capsuleDirection);

                            AddHitToRender((hit.point, Vector3.back));
                            AddMeshHitToRender((Physics2DMeshShape.Capsule, new Matrix4x4(origin + direction * hit.distance, dataVec, dataVec2, Vector3.zero)));
                        }
                    }
                }
            }

            // extern private static Collider2D[] OverlapCapsuleAll_Internal(PhysicsScene2D physicsScene, Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, ContactFilter2D contactFilter)
            [HarmonyPrefix]
            [HarmonyPatch("OverlapCapsuleAll_Internal")]
            static void OverlapCapsuleAll_Internal_Prefix(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_7))   // OverlapCapsules
                {
                    Vector3 dataVec = new Vector3(size.x, size.y, angle);
                    Vector3 dataVec2 = new Vector3((float)direction, (float)direction, (float)direction);

                    AddMeshToRender((Physics2DMeshShape.Capsule, new Matrix4x4(point, dataVec, dataVec2, Vector3.zero), Vector3.zero));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("OverlapCapsuleAll_Internal")]
            static void OverlapCapsuleAll_Internal_Postfix(Collider2D[] __result)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_7))   // OverlapCapsules
                {
                    foreach (Collider2D collider in __result)
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

        #region Pointcast
        [HarmonyPatch(typeof(UnityEngine.PhysicsScene2D))]
        partial class PhysicsScene2DPatches
        {
            // extern private static Collider2D OverlapPoint_Internal(PhysicsScene2D physicsScene, Vector2 point, ContactFilter2D contactFilter)
            [HarmonyPrefix]
            [HarmonyPatch("OverlapPoint_Internal")]
            static void OverlapPoint_Internal_Prefix(Vector2 point)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_9))   // OverlapPoints
                {
                    AddMeshToRender((Physics2DMeshShape.Point, Matrix4x4.TRS(point, Quaternion.identity, Vector3.one), Vector3.zero));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("OverlapPoint_Internal")]
            static void OverlapPoint_Internal_Postfix(Collider2D __result)
            {
                if (__result != null && CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_9))   // OverlapPoints
                {
                    AddColliderHitToRender(__result);
                }
            }

            // extern private static int OverlapPointArray_Internal(PhysicsScene2D physicsScene, Vector2 point, ContactFilter2D contactFilter, [NotNull] Collider2D[] results)
            [HarmonyPrefix]
            [HarmonyPatch("OverlapPointArray_Internal")]
            static void OverlapPointArray_Internal_Prefix(Vector2 point)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_9))   // OverlapPoints
                {
                    AddMeshToRender((Physics2DMeshShape.Point, Matrix4x4.TRS(point, Quaternion.identity, Vector3.one), Vector3.zero));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("OverlapPointArray_Internal")]
            static void OverlapPointArray_Internal_Postfix(Collider2D[] results)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_9))   // OverlapPoints
                {
                    foreach (Collider2D collider in results)
                    {
                        if (collider != null)
                        {
                            AddColliderHitToRender(collider);
                        }
                    }
                }
            }

            // extern private static int OverlapPointList_Internal(PhysicsScene2D physicsScene, Vector2 point, ContactFilter2D contactFilter, [NotNull] List<Collider2D> results)
            [HarmonyPrefix]
            [HarmonyPatch("OverlapPointList_Internal")]
            static void OverlapPointList_Internal_Prefix(Vector2 point)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_9))   // OverlapPoints
                {
                    AddMeshToRender((Physics2DMeshShape.Point, Matrix4x4.TRS(point, Quaternion.identity, Vector3.one), Vector3.zero));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("OverlapPointList_Internal")]
            static void OverlapPointList_Internal_Postfix(List<Collider2D> results)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_9))   // OverlapPoints
                {
                    foreach (Collider2D collider in results)
                    {
                        if (collider != null)
                        {
                            AddColliderHitToRender(collider);
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(UnityEngine.Physics2D))]
        partial class Physics2DPatches
        {
            // extern private static Collider2D[] OverlapPointAll_Internal(PhysicsScene2D physicsScene, Vector2 point, ContactFilter2D contactFilter)
            [HarmonyPrefix]
            [HarmonyPatch("OverlapPointAll_Internal")]
            static void OverlapPointAll_Internal_Prefix(Vector2 point)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_9))   // OverlapPoints
                {
                    AddMeshToRender((Physics2DMeshShape.Point, Matrix4x4.TRS(point, Quaternion.identity, Vector3.one), Vector3.zero));
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("OverlapPointAll_Internal")]
            static void OverlapPointAll_Internal_Postfix(Collider2D[] __result)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_9))   // OverlapPoints
                {
                    foreach (Collider2D collider in __result)
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

        #region Collidercast
        [HarmonyPatch(typeof(UnityEngine.PhysicsScene2D))]
        partial class PhysicsScene2DPatches
        {
            // extern private static int OverlapColliderArray_Internal([NotNull] Collider2D collider, ContactFilter2D contactFilter, [NotNull] Collider2D[] results)
            [HarmonyPrefix]
            [HarmonyPatch("OverlapColliderArray_Internal")]
            static void OverlapColliderArray_Internal_Prefix(Collider2D collider)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_8))   // OverlapColliders
                {
                    AddColliderCastToRender(collider);
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("OverlapColliderArray_Internal")]
            static void OverlapColliderArray_Internal_Postfix(Collider2D[] results, Collider2D collider)
            {
                foreach (Collider2D coll in results)
                {
                    if (coll != null && CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_8))   // OverlapColliders
                    {
                        AddColliderHitToRender(coll);
                    }
                }
            }

            // extern private static int OverlapColliderList_Internal([NotNull] Collider2D collider, ContactFilter2D contactFilter, [NotNull] List<Collider2D> results)
            [HarmonyPrefix]
            [HarmonyPatch("OverlapColliderList_Internal")]
            static void OverlapColliderList_Internal_Prefix(Collider2D collider)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_8))   // OverlapColliders
                {
                    AddColliderCastToRender(collider);
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("OverlapColliderList_Internal")]
            static void OverlapColliderList_Internal_Postfix(List<Collider2D> results, Collider2D collider)
            {
                if (CastVisualizerManager.Instance.ViewStatePhysics2DFlag.HasFlag(CastVisualizerManager.ViewStateFlags.ViewStateElement_8))   // OverlapColliders
                {
                    foreach (Collider2D coll in results)
                    {
                        if (coll != null)
                        {
                            AddColliderHitToRender(coll);
                        }
                    }
                }
            }
        }
        #endregion
    }
}