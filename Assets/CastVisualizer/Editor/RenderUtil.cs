using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BgTools.CastVisualizer
{
    public class RenderUtil
    {
        public static void DrawCross(Vector3 position, Vector3 normal)
        {
            Color oldColor = Gizmos.color;
            Gizmos.color = CastVisualizerManager.Instance.HitMarkerColor;

            Quaternion rotation = Quaternion.LookRotation(normal);

            Gizmos.DrawLine(
                position + rotation * new Vector3(-0.1f, -0.1f, 0.0f),
                position + rotation * new Vector3( 0.1f,  0.1f, 0.0f)
                );

            Gizmos.DrawLine(
                position + rotation * new Vector3(-0.1f,  0.1f, 0.0f),
                position + rotation * new Vector3( 0.1f, -0.1f, 0.0f)
                );

            Gizmos.color = oldColor;
        }

        public static void DrawWireCube(Vector3 centerPos, Vector3 size)
        {
            Handles.DrawWireCube(centerPos, size);
        }

        public static Vector3[] GetWireCubeConnectingPoints(Vector3 halfExtents, Vector3 direction, Quaternion cubeRotation)
        {
            Vector3 rotation = Quaternion.Inverse(cubeRotation) * direction;
            List<Vector3> pointsList = new List<Vector3>();
            int dimensions = 0;

            Vector3[] points = new Vector3[8];
            points[0] = new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
            points[1] = new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
            points[2] = new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
            points[3] = new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);

            points[4] = new Vector3(-halfExtents.x, -halfExtents.y, halfExtents.z);
            points[5] = new Vector3(-halfExtents.x, halfExtents.y, halfExtents.z);
            points[6] = new Vector3(halfExtents.x, halfExtents.y, halfExtents.z);
            points[7] = new Vector3(halfExtents.x, -halfExtents.y, halfExtents.z);

            if (rotation.x != 0.0f)
            {
                switch (Mathf.Sign(rotation.x))
                {
                    case (1.0f):
                        pointsList.Add(points[2]);
                        pointsList.Add(points[3]);
                        pointsList.Add(points[6]);
                        pointsList.Add(points[7]);
                        dimensions++;
                        break;
                    case (-1.0f):
                        pointsList.Add(points[0]);
                        pointsList.Add(points[1]);
                        pointsList.Add(points[4]);
                        pointsList.Add(points[5]);
                        dimensions++;
                        break;
                }
            }

            if (rotation.y != 0.0f)
            {
                switch (Mathf.Sign(rotation.y))
                {
                    case (1.0f):
                        pointsList.Add(points[1]);
                        pointsList.Add(points[2]);
                        pointsList.Add(points[5]);
                        pointsList.Add(points[6]);
                        dimensions++;
                        break;
                    case (-1.0f):
                        pointsList.Add(points[0]);
                        pointsList.Add(points[3]);
                        pointsList.Add(points[4]);
                        pointsList.Add(points[7]);
                        dimensions++;
                        break;
                }
            }

            if (rotation.z != 0.0f)
            {
                switch (Mathf.Sign(rotation.z))
                {
                    case (1.0f):
                        pointsList.Add(points[4]);
                        pointsList.Add(points[5]);
                        pointsList.Add(points[6]);
                        pointsList.Add(points[7]);
                        dimensions++;
                        break;
                    case (-1.0f):
                        pointsList.Add(points[0]);
                        pointsList.Add(points[1]);
                        pointsList.Add(points[2]);
                        pointsList.Add(points[3]);
                        dimensions++;
                        break;
                }
            }

            dimensions = System.Math.Max(dimensions - 1, 1);

            return pointsList.GroupBy((element) => { return element; }).Where((group) => { return group.Count() <= dimensions; }).Select( (entry) => { return entry.Key; }).ToArray();
        }

        public static void DrawWireSphere(Vector3 centerPos, float radius)
        {
            // SphereBoundsHandle
            Handles.DrawWireArc(centerPos, Vector3.forward, Vector3.up, 360.0f, radius);
            Handles.DrawWireArc(centerPos, Vector3.up, Vector3.right, 360.0f, radius);
            Handles.DrawWireArc(centerPos, Vector3.right, Vector3.forward, 360.0f, radius);
        }

        public static Vector3[] GetWireSphereConnectingPoints(Vector3 direction)
        {
            Vector3 forward = direction.normalized;
            Vector3 left = Vector3.Slerp(forward, -forward, 0.5f);
            Vector3 right = -left;
            Vector2 down = Vector3.Cross(forward, left).normalized;
            Vector2 up = -down;

            Vector3[] points = new Vector3[4];
            points[0] = up;
            points[1] = down;
            points[2] = left;
            points[3] = right;

            return points;
        }

        public static void DrawWireCapsule(Vector3 topPos, Vector3 bottomPos, float radius)
        {
            Vector3 up = (bottomPos - topPos).normalized * radius;
            Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
            Vector3 right = Vector3.Cross(up, forward).normalized * radius;

            Vector3 top = topPos - up;
            Vector3 bottom = bottomPos + up;

            float height = (top - bottom).magnitude;
            float sideLength = Mathf.Max(0.0f, (height * 0.5f) - radius);
            Vector3 middle = (bottom + top) * 0.5f;

            top = middle + ((top - middle).normalized * sideLength);
            bottom = middle + ((bottom - middle).normalized * sideLength);

            //Radial circles
            Handles.DrawWireArc(top, up, right, 360f, radius);
            Handles.DrawWireArc(bottom, -up, right, 360f, radius);

            //Side lines
            Handles.DrawLine(top + right, bottom + right);
            Handles.DrawLine(top - right, bottom - right);

            Handles.DrawLine(top + forward, bottom + forward);
            Handles.DrawLine(top - forward, bottom - forward);

            //Topcap
            Handles.DrawWireArc(top, forward, right, -180f, radius);
            Handles.DrawWireArc(top, right, forward, 180f, radius);

            //Bottomcap
            Handles.DrawWireArc(bottom, forward, right, 180f, radius);
            Handles.DrawWireArc(bottom, right, forward, -180f, radius);
        }

        public static Vector3[] GetWireCapsuleConnectingPoints(Vector3 topPos, Vector3 bottomPos, float radius, Vector3 direction)
        {
            Vector3 forward = direction.normalized;
            Vector3 left = Vector3.Cross(forward, (forward == Vector3.up) ? Vector3.forward : Vector3.up).normalized;
            Vector3 up = Vector3.Cross(left, forward).normalized;

            Vector3 top = topPos;
            Vector3 bottom = bottomPos;
            if (bottomPos.y > topPos.y)
            {
                top = bottomPos;
                bottom = topPos;
            }

            Vector3[] points = new Vector3[6];
            points[0] = top + up * radius;
            points[1] = top + left * radius;
            points[2] = top - left * radius;

            points[3] = bottom - up * radius;
            points[4] = bottom + left * radius;
            points[5] = bottom - left * radius;

            return points;
        }

        public static void DrawCapsule2D(Vector3 topPos, Vector3 bottomPos, float radius)
        {
            Vector3 up = (bottomPos - topPos).normalized * radius;
            Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
            Vector3 right = Vector3.Cross(up, forward).normalized * radius;

            Vector3 top = topPos - up;
            Vector3 bottom = bottomPos + up;

            float height = (top - bottom).magnitude;
            float sideLength = Mathf.Max(0, (height * 0.5f) - radius);
            Vector3 middle = (bottom + top) * 0.5f;

            top = middle + ((top - middle).normalized * sideLength);
            bottom = middle + ((bottom - middle).normalized * sideLength);

            //Side lines
            Handles.DrawLine(top + right, bottom + right);
            Handles.DrawLine(top - right, bottom - right);

            //Caps
            Handles.DrawWireArc(top, Vector3.back, right, -180f, radius);
            Handles.DrawWireArc(bottom, Vector3.back, right, 180f, radius);
        }

        public static void DrawCapsule2D(Vector2 offset, float height, float radius, CapsuleDirection2D direction)
        {
            Vector2 hgtAx, radAx1, top, bottom;

            if (direction == CapsuleDirection2D.Horizontal)
            {
                hgtAx = Vector3.right;
                radAx1 = Vector3.up;
            }else
            {
                hgtAx = Vector3.up;
                radAx1 = Vector3.right;
            }

            top = offset + hgtAx * (height * 0.5f - radius);
            bottom = offset - hgtAx * (height * 0.5f - radius);

            DrawCapsule2D(top, bottom, radius, direction);
        }

        public static void DrawCapsule2D(Vector2 top, Vector2 bottom, float radius, CapsuleDirection2D direction)
        {
            Vector2 hgtAx, radAx1;
            float arcDegree = 180.0f;

            if (direction == CapsuleDirection2D.Horizontal)
            {
                hgtAx = Vector3.right;
                radAx1 = Vector3.up;
            }
            else
            {
                hgtAx = Vector3.up;
                radAx1 = Vector3.right;
                arcDegree *= -1.0f;
            }

            // draw caps
            Handles.DrawWireArc(top, Vector3.back, radAx1, arcDegree, radius);
            Handles.DrawWireArc(bottom, Vector3.back, radAx1, -arcDegree, radius);

            // cross-section body
            Handles.DrawLine(top + radAx1 * radius, bottom + radAx1 * radius);
            Handles.DrawLine(top - radAx1 * radius, bottom - radAx1 * radius);
        }

        public static Vector3[] GetCapsule2DConnectingPoints(Vector2 offset, float height, float radius, CapsuleDirection2D capDir, Vector3 direction, Quaternion rotation)
        {
            Vector2 hgtAx, radAx1, top, bottom;

            if (capDir == CapsuleDirection2D.Horizontal)
            {
                hgtAx = Vector3.right;
                radAx1 = Vector3.up;
            }
            else
            {
                hgtAx = Vector3.up;
                radAx1 = Vector3.right;
            }

            top = offset + hgtAx * (height * 0.5f - radius);
            bottom = offset - hgtAx * (height * 0.5f - radius);

            return GetCapsule2DConnectingPoints(top, bottom, radius, direction, rotation);
        }

        public static Vector3[] GetCapsule2DConnectingPoints(Vector2 top, Vector2 bottom, float radius, Vector3 direction, Quaternion rotation)
        {
            Vector3 up = direction.normalized * radius;
            Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
            Vector2 right = Vector3.Cross(up, forward).normalized * radius;
            Vector2 left = -right;

            Vector2 rotTop = rotation * top;
            Vector2 rotBottom = rotation * bottom;

            Vector3[] points = new Vector3[2];
            if (rotTop.y > rotBottom.y)
            {
                points[0] = rotTop + right;
                points[1] = rotBottom + left;
            }
            else
            {
                points[0] = rotTop + left;
                points[1] = rotBottom + right;
            }

            return points;
        }

        public static void DrawCircle2D(Vector2 centerPos, float radius)
        {
            Handles.DrawWireArc(centerPos, Vector3.back, Vector3.up, 360.0f, radius);
        }

        public static Vector3[] GetCicleConnectingPoints()
        {
            Vector3[] points = new Vector3[2];
            points[0] = Vector3.left;
            points[1] = Vector3.right;

            return points;
        }

        public static void DrawCube2D(Vector3 offset, Vector3 size, float edgeRadius = 0.0f)
        {
            Vector3 halfsize = (size * 0.5f);

            Vector3[] points = new Vector3[8];
            points[0] = offset + new Vector3(-halfsize.x, halfsize.y + edgeRadius);
            points[1] = offset + new Vector3(halfsize.x, halfsize.y + edgeRadius);

            points[2] = offset + new Vector3(halfsize.x + edgeRadius, halfsize.y);
            points[3] = offset + new Vector3(halfsize.x + edgeRadius, -halfsize.y);

            points[4] = offset + new Vector3(halfsize.x, -halfsize.y - edgeRadius);
            points[5] = offset + new Vector3(-halfsize.x, -halfsize.y - edgeRadius);

            points[6] = offset + new Vector3(-halfsize.x - edgeRadius, -halfsize.y);
            points[7] = offset + new Vector3(-halfsize.x - edgeRadius, halfsize.y);

            Handles.DrawLines(points);

            Handles.DrawWireArc(offset + new Vector3(halfsize.x, halfsize.y), Vector3.back, Vector2.up, 90, edgeRadius);
            Handles.DrawWireArc(offset + new Vector3(halfsize.x, -halfsize.y), Vector3.back, Vector2.right, 90, edgeRadius);
            Handles.DrawWireArc(offset + new Vector3(-halfsize.x, -halfsize.y), Vector3.back, Vector2.down, 90, edgeRadius);
            Handles.DrawWireArc(offset + new Vector3(-halfsize.x, halfsize.y), Vector3.back, Vector2.left, 90, edgeRadius);
        }

        public static Vector3[] GetCube2DConnectingPoints(Vector3 halfExtents, Vector3 direction, Quaternion rotation)
        {
            Vector3 up = direction.normalized;
            up.Scale(halfExtents);
            Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
            Vector3 right = Vector3.Cross(up, forward).normalized;
            right.Scale(halfExtents);
            Vector3 left = -right;

            Vector3[] points = new Vector3[8];
            points[0] = new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
            points[1] = new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
            points[2] = new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
            points[3] = new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);

            points[4] = new Vector3(-halfExtents.x, -halfExtents.y, halfExtents.z);
            points[5] = new Vector3(-halfExtents.x, halfExtents.y, halfExtents.z);
            points[6] = new Vector3(halfExtents.x, halfExtents.y, halfExtents.z);
            points[7] = new Vector3(halfExtents.x, -halfExtents.y, halfExtents.z);

            Vector3[] result = new Vector3[2];

            float currLeftLength = float.MaxValue;
            float currRightLength = float.MaxValue;
            float length = 0;

            foreach(Vector3 pos in points)
            {
                length = Vector3.Distance(rotation * pos, left);
                if (length < currLeftLength)
                {
                    currLeftLength = length;
                    result[0] = pos;
                }
                length = Vector3.Distance(rotation * pos, right);
                if (length < currRightLength)
                {
                    currRightLength = length;
                    result[1] = pos;
                }
            }

            return result;
        }
    }
}