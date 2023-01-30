using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace BgTools.CastVisualizer
{
    [ExecuteInEditMode]
    public class Caster : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField]
        private CastModule castModule;
        [SerializeField]
        private CastForm castForm;
        [SerializeField]
        private CastShape castShape;
        [SerializeField]
        private CastType castType;
        [SerializeField]
        private CastRate castRate;
        [SerializeField]
        private float castDistance = Mathf.Infinity;
        [SerializeField]
        private Vector3 castDirection = Vector3.zero;
        #pragma warning restore 0649

        public List<(GameObject, Vector3)> hitsList = new List<(GameObject, Vector3)>();

        private Dictionary<(CastModule, CastForm, CastShape, CastType), Func<object[], object>> methodCalls = new Dictionary<(CastModule, CastForm, CastShape, CastType), Func<object[], object>>();

        private readonly float RADIUS = 0.5f;
        private readonly Vector3 DIMENSIONS = Vector3.one;

        private bool FirstVisualizationDone = false;

        protected enum CastModule
        {
            Physics = 0,
            Physics2D
        }

        protected enum CastForm
        {
            Cast = 0,
            Overlap
        }

        protected enum CastType
        {
            Check = 0,
            Standard,
            All,
            NonAlloc
        }

        protected enum CastShape
        {
            Ray = 0,
            Line,
            Box,
            Capsule,
            Sphere,
            Circle,
            Area,
            Point,
            Collider
        }

        protected enum CastRate
        {
            EveryTick = 0,
            Once
        }

        public string[] GetModules() { return methodCalls.Keys.GroupBy((tuple) => tuple.Item1).Select((group) => group.First().Item1.ToString()).ToArray(); }

        public string[] GetForms()   { return methodCalls.Keys.Where((tuple) => tuple.Item1 == castModule).GroupBy((tuple) => tuple.Item2).Select((group) => group.First().Item2.ToString()).ToArray(); }

        public string[] GetShapes()  { return methodCalls.Keys.Where((tuple) => tuple.Item1 == castModule && tuple.Item2 == castForm).GroupBy((tuple) => tuple.Item3).Select((group) => group.First().Item3.ToString()).ToArray(); }

        public string[] GetTypes()   { return methodCalls.Keys.Where((tuple) => tuple.Item1 == castModule && tuple.Item2 == castForm && tuple.Item3 == castShape).GroupBy((tuple) => tuple.Item4).Select((group) => group.First().Item4.ToString()).ToArray(); }

        public string[] GetRates()   { return new string[] { CastRate.EveryTick.ToString(), CastRate.Once.ToString() }; }

        private void Awake()
        {
            FirstVisualizationDone = false;
            
            if (castDirection == Vector3.zero)
            {
                castDirection = transform.forward;
            }

            #region Registry
            /****** PHISICS ******/
            // Linecast
            methodCalls.Add((CastModule.Physics, CastForm.Cast, CastShape.Line, CastType.Check), (parameters) => {
                if (Physics.Linecast((Vector3)parameters[0], (Vector3)parameters[1]))
                    hitsList.Add((null, Vector3.negativeInfinity));
                return 0;
                });
            methodCalls.Add((CastModule.Physics, CastForm.Cast, CastShape.Line, CastType.Standard), (parameters) => {
                RaycastHit hitInfo;
                Physics.Linecast((Vector3)parameters[0], (Vector3)parameters[1], out hitInfo);
                if (hitInfo.collider != null)
                    hitsList.Add((hitInfo.collider.gameObject, hitInfo.point));
                return 0;
                });

            // Raycasts
            methodCalls.Add((CastModule.Physics, CastForm.Cast, CastShape.Ray, CastType.Check), (parameters) => {
                if (Physics.Raycast(new Ray((Vector3)parameters[0], (Vector3)parameters[1]), castDistance))
                    hitsList.Add((null, Vector3.negativeInfinity));
                return 0;
                });
            methodCalls.Add((CastModule.Physics, CastForm.Cast, CastShape.Ray, CastType.Standard), (parameters) => {
                RaycastHit hitInfo;
                Physics.Raycast(new Ray((Vector3)parameters[0], (Vector3)parameters[1]), out hitInfo, castDistance);
                if(hitInfo.collider != null)
                    hitsList.Add((hitInfo.collider.gameObject, hitInfo.point));
                return 0;
                });
            methodCalls.Add((CastModule.Physics, CastForm.Cast, CastShape.Ray, CastType.All), (parameters) => {
                RaycastHit[] hitInfos = Physics.RaycastAll(new Ray((Vector3)parameters[0], (Vector3)parameters[1]), castDistance);
                hitsList.AddRange(hitInfos.Where((hit) => hit.collider != null).Select((hit) => (hit.collider.gameObject, hit.point)));
                return 0;
                });
            methodCalls.Add((CastModule.Physics, CastForm.Cast, CastShape.Ray, CastType.NonAlloc), (parameters) => {
                RaycastHit[] hitInfos = new RaycastHit[10];
                Physics.RaycastNonAlloc(new Ray((Vector3)parameters[0], (Vector3)parameters[1]), hitInfos, castDistance);
                hitsList.AddRange(hitInfos.Where((hit) => hit.collider != null).Select((hit) => (hit.collider.gameObject, hit.point)));
                return 0;
                });

            // Boxcast
            methodCalls.Add((CastModule.Physics, CastForm.Cast, CastShape.Box, CastType.Check), (parameters) => {
                if(Physics.BoxCast((Vector3)parameters[0], DIMENSIONS, (Vector3)parameters[1], (Quaternion)parameters[2], castDistance))
                    hitsList.Add((null, Vector3.negativeInfinity));
                return 0;
                });
            methodCalls.Add((CastModule.Physics, CastForm.Cast, CastShape.Box, CastType.Standard), (parameters) => {
                RaycastHit hitInfo;
                Physics.BoxCast((Vector3)parameters[0], DIMENSIONS, (Vector3)parameters[1], out hitInfo, (Quaternion)parameters[2], castDistance);
                if (hitInfo.collider != null)
                    hitsList.Add((hitInfo.collider.gameObject, hitInfo.point));
                return 0;
                });
            methodCalls.Add((CastModule.Physics, CastForm.Cast, CastShape.Box, CastType.All), (parameters) => {
                RaycastHit[] hitInfos = Physics.BoxCastAll((Vector3)parameters[0], DIMENSIONS, (Vector3)parameters[1], (Quaternion)parameters[2], castDistance);
                hitsList.AddRange(hitInfos.Where((hit) => hit.collider != null).Select((hit) => (hit.collider.gameObject, hit.point)));
                return 0;
            });
            methodCalls.Add((CastModule.Physics, CastForm.Cast, CastShape.Box, CastType.NonAlloc), (parameters) => {
                RaycastHit[] hitInfos = new RaycastHit[10];
                Physics.BoxCastNonAlloc((Vector3)parameters[0], DIMENSIONS, (Vector3)parameters[1], hitInfos, (Quaternion)parameters[2], castDistance);
                hitsList.AddRange(hitInfos.Where((hit) => hit.collider != null).Select((hit) => (hit.collider.gameObject, hit.point)));
                return 0;
            });

            methodCalls.Add((CastModule.Physics, CastForm.Overlap, CastShape.Box, CastType.Standard), (parameters) => {
                Collider[] colliders = Physics.OverlapBox((Vector3)parameters[0], DIMENSIONS, (Quaternion)parameters[2]);
                hitsList.AddRange(colliders.Where((collider) => collider != null).Select((collider) => (collider.gameObject, collider.transform.position)));
                return 0;
                });
            methodCalls.Add((CastModule.Physics, CastForm.Overlap, CastShape.Box, CastType.NonAlloc), (parameters) => {
                Collider[] colliders = new Collider[10];
                Physics.OverlapBoxNonAlloc((Vector3)parameters[0], DIMENSIONS, colliders, (Quaternion)parameters[2]);
                hitsList.AddRange(colliders.Where((collider) => collider != null).Select((collider) => (collider.gameObject, collider.transform.position)));
                return 0;
                });

            // Capsule
            methodCalls.Add((CastModule.Physics, CastForm.Cast, CastShape.Capsule, CastType.Check), (parameters) => {
                if(Physics.CapsuleCast((Vector3)parameters[0], (Vector3)parameters[0] + (Quaternion)parameters[2] * Vector3.up, RADIUS, (Vector3)parameters[1], castDistance))
                    hitsList.Add((null, Vector3.negativeInfinity));
                return 0;
                });
            methodCalls.Add((CastModule.Physics, CastForm.Cast, CastShape.Capsule, CastType.Standard), (parameters) => {
                RaycastHit hitInfo;
                Physics.CapsuleCast((Vector3)parameters[0], (Vector3)parameters[0] + (Quaternion)parameters[2] * Vector3.up, RADIUS, (Vector3)parameters[1], out hitInfo, castDistance);
                if (hitInfo.collider != null)
                    hitsList.Add((hitInfo.collider.gameObject, hitInfo.point));
                return 0;
                });
            methodCalls.Add((CastModule.Physics, CastForm.Cast, CastShape.Capsule, CastType.All), (parameters) => {
                RaycastHit[] hitInfos = Physics.CapsuleCastAll((Vector3)parameters[0], (Vector3)parameters[0] + (Quaternion)parameters[2] * Vector3.up, RADIUS, (Vector3)parameters[1], castDistance);
                hitsList.AddRange(hitInfos.Where((hit) => hit.collider != null).Select((hit) => (hit.collider.gameObject, hit.point)));
                return 0;
            });
            methodCalls.Add((CastModule.Physics, CastForm.Cast, CastShape.Capsule, CastType.NonAlloc), (parameters) => {
                RaycastHit[] hitInfos = new RaycastHit[10];
                Physics.CapsuleCastNonAlloc((Vector3)parameters[0], (Vector3)parameters[0] + (Quaternion)parameters[2] * Vector3.up, RADIUS, (Vector3)parameters[1], hitInfos, castDistance);
                hitsList.AddRange(hitInfos.Where((hit) => hit.collider != null).Select((hit) => (hit.collider.gameObject, hit.point)));
                return 0;
            });

            methodCalls.Add((CastModule.Physics, CastForm.Overlap, CastShape.Capsule, CastType.Standard), (parameters) => {
                Collider[] colliders = Physics.OverlapCapsule((Vector3)parameters[0], (Vector3)parameters[0] + (Quaternion)parameters[2] * Vector3.up, RADIUS);
                hitsList.AddRange(colliders.Where((collider) => collider != null).Select((collider) => (collider.gameObject, collider.transform.position)));
                return 0;
            });
            methodCalls.Add((CastModule.Physics, CastForm.Overlap, CastShape.Capsule, CastType.NonAlloc), (parameters) => {
                Collider[] colliders = new Collider[10];
                Physics.OverlapCapsuleNonAlloc((Vector3)parameters[0], (Vector3)parameters[0] + (Quaternion)parameters[2] * Vector3.up, RADIUS, colliders);
                hitsList.AddRange(colliders.Where((collider) => collider != null).Select((collider) => (collider.gameObject, collider.transform.position)));
                return 0;
            });

            // Sphere
            methodCalls.Add((CastModule.Physics, CastForm.Cast, CastShape.Sphere, CastType.Check), (parameters) => {
                if(Physics.SphereCast(new Ray((Vector3)parameters[0], (Vector3)parameters[1]), RADIUS, castDistance))
                    hitsList.Add((null, Vector3.negativeInfinity));
                return 0;
            });
            methodCalls.Add((CastModule.Physics, CastForm.Cast, CastShape.Sphere, CastType.Standard), (parameters) => {
                RaycastHit hitInfo;
                Physics.SphereCast(new Ray((Vector3)parameters[0], (Vector3)parameters[1]), RADIUS, out hitInfo, castDistance);
                if (hitInfo.collider != null)
                    hitsList.Add((hitInfo.collider.gameObject, hitInfo.point));
                return 0;
            });
            methodCalls.Add((CastModule.Physics, CastForm.Cast, CastShape.Sphere, CastType.All), (parameters) => {
                RaycastHit[] hitInfos = Physics.SphereCastAll(new Ray((Vector3)parameters[0], (Vector3)parameters[1]), RADIUS, castDistance);
                hitsList.AddRange(hitInfos.Where((hit) => hit.collider != null).Select((hit) => (hit.collider.gameObject, hit.point)));
                return 0;
            });
            methodCalls.Add((CastModule.Physics, CastForm.Cast, CastShape.Sphere, CastType.NonAlloc), (parameters) => {
                RaycastHit[] hitInfos = new RaycastHit[10];
                Physics.SphereCastNonAlloc(new Ray((Vector3)parameters[0], (Vector3)parameters[1]), RADIUS, hitInfos, castDistance);
                hitsList.AddRange(hitInfos.Where((hit) => hit.collider != null).Select((hit) => (hit.collider.gameObject, hit.point)));
                return 0;
            });

            methodCalls.Add((CastModule.Physics, CastForm.Overlap, CastShape.Sphere, CastType.Standard), (parameters) => {
                Collider[] colliders = Physics.OverlapSphere((Vector3)parameters[0], RADIUS);
                hitsList.AddRange(colliders.Where((collider) => collider != null).Select((collider) => (collider.gameObject, collider.transform.position)));
                return 0;
            });
            methodCalls.Add((CastModule.Physics, CastForm.Overlap, CastShape.Sphere, CastType.NonAlloc), (parameters) => {
                Collider[] colliders = new Collider[10];
                Physics.OverlapSphereNonAlloc((Vector3)parameters[0], RADIUS, colliders);
                hitsList.AddRange(colliders.Where((collider) => collider != null).Select((collider) => (collider.gameObject, collider.transform.position)));
                return 0;
            });

            /****** PHISICS2D ******/
            // Linecast
            methodCalls.Add((CastModule.Physics2D, CastForm.Cast, CastShape.Line, CastType.Standard), (parameters) => {
                RaycastHit2D hitInfo2D = Physics2D.Linecast((Vector3)parameters[0], (Vector3)parameters[1]);
                if (hitInfo2D.collider != null)
                    hitsList.Add((hitInfo2D.collider.gameObject, hitInfo2D.point));
                return 0;
            });
            methodCalls.Add((CastModule.Physics2D, CastForm.Cast, CastShape.Line, CastType.All), (parameters) => {
                RaycastHit2D[] hitInfos2D = Physics2D.LinecastAll((Vector3)parameters[0], (Vector3)parameters[1]);
                hitsList.AddRange(hitInfos2D.Where((hit) => hit.collider != null).Select((hit) => (hit.collider.gameObject, (Vector3) hit.point)));
                return 0;
            });
            methodCalls.Add((CastModule.Physics2D, CastForm.Cast, CastShape.Line, CastType.NonAlloc), (parameters) => {
                RaycastHit2D[] hitInfos2D = new RaycastHit2D[10];
                Physics2D.LinecastNonAlloc((Vector3)parameters[0], (Vector3)parameters[1], hitInfos2D);
                hitsList.AddRange(hitInfos2D.Where((hit) => hit.collider != null).Select((hit) => (hit.collider.gameObject, (Vector3)hit.point)));
                return 0;
            });

            //Raycast
            methodCalls.Add((CastModule.Physics2D, CastForm.Cast, CastShape.Ray, CastType.Standard), (parameters) => {
                RaycastHit2D hitInfo2D = Physics2D.Raycast((Vector3)parameters[0], (Vector3)parameters[1], castDistance);
                if (hitInfo2D.collider != null)
                    hitsList.Add((hitInfo2D.collider.gameObject, hitInfo2D.point));
                return 0;
            });
            methodCalls.Add((CastModule.Physics2D, CastForm.Cast, CastShape.Ray, CastType.All), (parameters) => {
                RaycastHit2D[] hitInfos2D = Physics2D.RaycastAll((Vector3)parameters[0], (Vector3)parameters[1], castDistance);
                hitsList.AddRange(hitInfos2D.Where((hit) => hit.collider != null).Select((hit) => (hit.collider.gameObject, (Vector3)hit.point)));
                return 0;
            });
            methodCalls.Add((CastModule.Physics2D, CastForm.Cast, CastShape.Ray, CastType.NonAlloc), (parameters) => {
                RaycastHit2D[] hitInfos2D = new RaycastHit2D[10];
                Physics2D.RaycastNonAlloc((Vector3)parameters[0], (Vector3)parameters[1], hitInfos2D, castDistance);
                hitsList.AddRange(hitInfos2D.Where((hit) => hit.collider != null).Select((hit) => (hit.collider.gameObject, (Vector3)hit.point)));
                return 0;
            });

            //Boxcast
            methodCalls.Add((CastModule.Physics2D, CastForm.Cast, CastShape.Box, CastType.Standard), (parameters) => {
                RaycastHit2D hitInfo2D = Physics2D.BoxCast((Vector3)parameters[0], DIMENSIONS, ((Quaternion)parameters[2]).eulerAngles.z, (Vector3)parameters[1], castDistance);
                if (hitInfo2D.collider != null)
                    hitsList.Add((hitInfo2D.collider.gameObject, hitInfo2D.point));
                return 0;
            });
            methodCalls.Add((CastModule.Physics2D, CastForm.Cast, CastShape.Box, CastType.All), (parameters) => {
                RaycastHit2D[] hitInfos2D = Physics2D.BoxCastAll((Vector3)parameters[0], DIMENSIONS, ((Quaternion)parameters[2]).eulerAngles.z, (Vector3)parameters[1], castDistance);
                hitsList.AddRange(hitInfos2D.Where((hit) => hit.collider != null).Select((hit) => (hit.collider.gameObject, (Vector3)hit.point)));
                return 0;
            });
            methodCalls.Add((CastModule.Physics2D, CastForm.Cast, CastShape.Box, CastType.NonAlloc), (parameters) => {
                RaycastHit2D[] hitInfos2D = new RaycastHit2D[10];
                Physics2D.BoxCastNonAlloc((Vector3)parameters[0], DIMENSIONS, ((Quaternion)parameters[2]).eulerAngles.z, (Vector3)parameters[1], hitInfos2D, castDistance);
                hitsList.AddRange(hitInfos2D.Where((hit) => hit.collider != null).Select((hit) => (hit.collider.gameObject, (Vector3)hit.point)));
                return 0;
            });

            methodCalls.Add((CastModule.Physics2D, CastForm.Overlap, CastShape.Box, CastType.Standard), (parameters) => {
                Collider2D collider2D = Physics2D.OverlapBox((Vector3)parameters[0], DIMENSIONS, ((Quaternion)parameters[2]).eulerAngles.z);
                if(collider2D != null)
                    hitsList.Add((collider2D.gameObject, collider2D.transform.position));
                return 0;
            });
            methodCalls.Add((CastModule.Physics2D, CastForm.Overlap, CastShape.Box, CastType.All), (parameters) => {
                Collider2D[] colliders2D = Physics2D.OverlapBoxAll((Vector3)parameters[0], DIMENSIONS, ((Quaternion)parameters[2]).eulerAngles.z);
                hitsList.AddRange(colliders2D.Where((collider) => collider != null).Select((collider) => (collider.gameObject, collider.transform.position)));
                return 0;
            });
            methodCalls.Add((CastModule.Physics2D, CastForm.Overlap, CastShape.Box, CastType.NonAlloc), (parameters) => {
                Collider2D[] colliders2D = new Collider2D[10];
                Physics2D.OverlapBoxNonAlloc((Vector3)parameters[0], DIMENSIONS, ((Quaternion)parameters[2]).eulerAngles.z, colliders2D);
                hitsList.AddRange(colliders2D.Where((collider) => collider != null).Select((collider) => (collider.gameObject, collider.transform.position)));
                return 0;
            });

            //Capsule
            methodCalls.Add((CastModule.Physics2D, CastForm.Cast, CastShape.Capsule, CastType.Standard), (parameters) => {
                RaycastHit2D hitInfo2D = Physics2D.CapsuleCast((Vector3)parameters[0], new Vector2(DIMENSIONS.x, DIMENSIONS.y * 2.0f), CapsuleDirection2D.Vertical, ((Quaternion)parameters[2]).eulerAngles.z, (Vector3)parameters[1], castDistance);
                if (hitInfo2D.collider != null)
                    hitsList.Add((hitInfo2D.collider.gameObject, hitInfo2D.point));
                return 0;
            });
            methodCalls.Add((CastModule.Physics2D, CastForm.Cast, CastShape.Capsule, CastType.All), (parameters) => {
                RaycastHit2D[] hitInfos2D = Physics2D.CapsuleCastAll((Vector3)parameters[0], new Vector2(DIMENSIONS.x, DIMENSIONS.y * 2.0f), CapsuleDirection2D.Vertical, ((Quaternion)parameters[2]).eulerAngles.z, (Vector3)parameters[1], castDistance);
                hitsList.AddRange(hitInfos2D.Where((hit) => hit.collider != null).Select((hit) => (hit.collider.gameObject, (Vector3)hit.point)));
                return 0;
            });
            methodCalls.Add((CastModule.Physics2D, CastForm.Cast, CastShape.Capsule, CastType.NonAlloc), (parameters) => {
                RaycastHit2D[] hitInfos2D = new RaycastHit2D[10];
                Physics2D.CapsuleCastNonAlloc((Vector3)parameters[0], new Vector2(DIMENSIONS.x * 2.0f, DIMENSIONS.y), CapsuleDirection2D.Horizontal, ((Quaternion)parameters[2]).eulerAngles.z, (Vector3)parameters[1], hitInfos2D, castDistance);
                hitsList.AddRange(hitInfos2D.Where((hit) => hit.collider != null).Select((hit) => (hit.collider.gameObject, (Vector3)hit.point)));
                return 0;
            });

            methodCalls.Add((CastModule.Physics2D, CastForm.Overlap, CastShape.Capsule, CastType.Standard), (parameters) => {
                Collider2D collider2D = Physics2D.OverlapCapsule((Vector3)parameters[0], new Vector2(DIMENSIONS.x, DIMENSIONS.y * 2.0f), CapsuleDirection2D.Vertical, ((Quaternion)parameters[2]).eulerAngles.z);
                if (collider2D != null)
                    hitsList.Add((collider2D.gameObject, collider2D.transform.position));
                return 0;
            });
            methodCalls.Add((CastModule.Physics2D, CastForm.Overlap, CastShape.Capsule, CastType.All), (parameters) => {
                Collider2D[] colliders2D = Physics2D.OverlapCapsuleAll((Vector3)parameters[0], new Vector2(DIMENSIONS.x, DIMENSIONS.y * 2.0f), CapsuleDirection2D.Vertical, ((Quaternion)parameters[2]).eulerAngles.z);
                hitsList.AddRange(colliders2D.Where((collider) => collider != null).Select((collider) => (collider.gameObject, collider.transform.position)));
                return 0;
            });
            methodCalls.Add((CastModule.Physics2D, CastForm.Overlap, CastShape.Capsule, CastType.NonAlloc), (parameters) => {
                Collider2D[] colliders2D = new Collider2D[10];
                Physics2D.OverlapCapsuleNonAlloc((Vector3)parameters[0], new Vector2(DIMENSIONS.x * 2.0f, DIMENSIONS.y), CapsuleDirection2D.Horizontal, ((Quaternion)parameters[2]).eulerAngles.z, colliders2D);
                hitsList.AddRange(colliders2D.Where((collider) => collider != null).Select((collider) => (collider.gameObject, collider.transform.position)));
                return 0;
            });

            //Circle
            methodCalls.Add((CastModule.Physics2D, CastForm.Cast, CastShape.Circle, CastType.Standard), (parameters) => {
                RaycastHit2D hitInfo2D = Physics2D.CircleCast((Vector3)parameters[0], RADIUS, (Vector3)parameters[1], castDistance);
                if (hitInfo2D.collider != null)
                    hitsList.Add((hitInfo2D.collider.gameObject, hitInfo2D.point));
                return 0;
            });
            methodCalls.Add((CastModule.Physics2D, CastForm.Cast, CastShape.Circle, CastType.All), (parameters) => {
                RaycastHit2D[] hitInfos2D = Physics2D.CircleCastAll((Vector3)parameters[0], RADIUS, (Vector3)parameters[1], castDistance);
                hitsList.AddRange(hitInfos2D.Where((hit) => hit.collider != null).Select((hit) => (hit.collider.gameObject, (Vector3)hit.point)));
                return 0;
            });
            methodCalls.Add((CastModule.Physics2D, CastForm.Cast, CastShape.Circle, CastType.NonAlloc), (parameters) => {
                RaycastHit2D[] hitInfos2D = new RaycastHit2D[10];
                Physics2D.CircleCastNonAlloc((Vector3)parameters[0], RADIUS, (Vector3)parameters[1], hitInfos2D, castDistance);
                hitsList.AddRange(hitInfos2D.Where((hit) => hit.collider != null).Select((hit) => (hit.collider.gameObject, (Vector3)hit.point)));
                return 0;
            });

            methodCalls.Add((CastModule.Physics2D, CastForm.Overlap, CastShape.Circle, CastType.Standard), (parameters) => {
                Collider2D collider2D = Physics2D.OverlapCircle((Vector3)parameters[0], RADIUS);
                if (collider2D != null)
                    hitsList.Add((collider2D.gameObject, collider2D.transform.position));
                return 0;
            });
            methodCalls.Add((CastModule.Physics2D, CastForm.Overlap, CastShape.Circle, CastType.All), (parameters) => {
                Collider2D[] colliders2D = Physics2D.OverlapCircleAll((Vector3)parameters[0], RADIUS);
                hitsList.AddRange(colliders2D.Where((collider) => collider != null).Select((collider) => (collider.gameObject, collider.transform.position)));
                return 0;
            });
            methodCalls.Add((CastModule.Physics2D, CastForm.Overlap, CastShape.Circle, CastType.NonAlloc), (parameters) => {
                Collider2D[] colliders2D = new Collider2D[10];
                Physics2D.OverlapCircleNonAlloc((Vector3)parameters[0], RADIUS, colliders2D);
                hitsList.AddRange(colliders2D.Where((collider) => collider != null).Select((collider) => (collider.gameObject, collider.transform.position)));
                return 0;
            });

            //Area
            methodCalls.Add((CastModule.Physics2D, CastForm.Overlap, CastShape.Area, CastType.Standard), (parameters) => {
                Collider2D collider2D = Physics2D.OverlapArea((Vector3)parameters[0], (Vector3)parameters[0] + DIMENSIONS);
                if (collider2D != null)
                    hitsList.Add((collider2D.gameObject, collider2D.transform.position));
                return 0;
            });
            methodCalls.Add((CastModule.Physics2D, CastForm.Overlap, CastShape.Area, CastType.All), (parameters) => {
                Collider2D[] colliders2D = Physics2D.OverlapAreaAll((Vector3)parameters[0], (Vector3)parameters[0] + DIMENSIONS);
                hitsList.AddRange(colliders2D.Where((collider) => collider != null).Select((collider) => (collider.gameObject, collider.transform.position)));
                return 0;
            });
            methodCalls.Add((CastModule.Physics2D, CastForm.Overlap, CastShape.Area, CastType.NonAlloc), (parameters) => {
                Collider2D[] colliders2D = new Collider2D[10];
                Physics2D.OverlapAreaNonAlloc((Vector3)parameters[0], (Vector3)parameters[0] + DIMENSIONS, colliders2D);
                hitsList.AddRange(colliders2D.Where((collider) => collider != null).Select((collider) => (collider.gameObject, collider.transform.position)));
                return 0;
            });

            //Point
            methodCalls.Add((CastModule.Physics2D, CastForm.Overlap, CastShape.Point, CastType.Standard), (parameters) => {
                Collider2D collider2D = Physics2D.OverlapPoint((Vector3)parameters[0]);
                if (collider2D != null)
                    hitsList.Add((collider2D.gameObject, collider2D.transform.position));
                return 0;
            });
            methodCalls.Add((CastModule.Physics2D, CastForm.Overlap, CastShape.Point, CastType.All), (parameters) => {
                Collider2D[] colliders2D = Physics2D.OverlapPointAll((Vector3)parameters[0]);
                hitsList.AddRange(colliders2D.Where((collider) => collider != null).Select((collider) => (collider.gameObject, collider.transform.position)));
                return 0;
            });
            methodCalls.Add((CastModule.Physics2D, CastForm.Overlap, CastShape.Point, CastType.NonAlloc), (parameters) => {
                Collider2D[] colliders2D = new Collider2D[10];
                Physics2D.OverlapPointNonAlloc((Vector3)parameters[0], colliders2D);
                hitsList.AddRange(colliders2D.Where((collider) => collider != null).Select((collider) => (collider.gameObject, collider.transform.position)));
                return 0;
            });

            // Collider
            methodCalls.Add((CastModule.Physics2D, CastForm.Overlap, CastShape.Collider, CastType.Standard), (parameters) => {

                Collider2D testCollider = gameObject.GetComponent<Collider2D>();
                if (testCollider == null)
                {
                    testCollider = gameObject.AddComponent<BoxCollider2D>();
                    ((BoxCollider2D)testCollider).size = DIMENSIONS;
                }

                Collider2D[] colliders2D = new Collider2D[10];
                ContactFilter2D filter = new ContactFilter2D();
                filter.NoFilter();

                Physics2D.OverlapCollider(testCollider, filter, colliders2D);
                hitsList.AddRange(colliders2D.Where((collider) => collider != null).Select((collider) => (collider.gameObject, collider.transform.position)));
                return 0;
            });
            #endregion //Registry
        }

        private void Start()
        {
            if(methodCalls.Count == 0)
            {
                Awake();
            }
        }

        private void FixedUpdate()
        {
            if (castRate == CastRate.Once && FirstVisualizationDone)
            {
                return;
            }
            
            FirstVisualizationDone = true;

            hitsList.Clear();

            try
            {
                Func<object[], object> function = methodCalls[(castModule, castForm, castShape, castType)];
                // position, direction, rotation
                function(new object[] { transform.position, castDirection, transform.rotation});
            }
            catch (KeyNotFoundException)
            { }
        }

        private void OnDrawGizmos()
        {
            Color defaultColor = Gizmos.color;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.1f);
            Gizmos.color = defaultColor;

            Handles.Label(transform.position + new Vector3(0.15f, 0.06f), GetCallString());
        }

        private void OnDrawGizmosSelected()
        {
            if (castForm != CastForm.Overlap)
            {
                Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(castDirection), 1.0f, EventType.Repaint);
            }
        }

        private string GetCallString()
        {
            if (castForm == CastForm.Cast)
                return $"{castModule.ToString()}.{castShape.ToString()}Cast{castType.ToString()}";
            else
                return $"{castModule.ToString()}.Overlap{castShape.ToString()}{castType.ToString()}";
        }

        [CustomEditor(typeof(Caster))]
        public class RaycasterEditor : Editor
        {
            private Caster raycaster;

            private SerializedProperty castModule;
            private SerializedProperty castForm;
            private SerializedProperty castShape;
            private SerializedProperty castType;
            private SerializedProperty castRate;
            private SerializedProperty castDistance;
            private SerializedProperty castDirection;

            private string[] avaiableModules;
            private string[] avaiableForms;
            private string[] avaiableShapes;
            private string[] avaiableTypes;
            private string[] avaiableRates;

            private ReorderableList reordableList;
            private int enumIndex;

            private void OnEnable()
            {
                if (raycaster == null)
                    raycaster = (Caster)target;

                GUIStyle rightAlignedTextStyle = new GUIStyle();
                rightAlignedTextStyle.alignment = TextAnchor.UpperRight;

                reordableList = new ReorderableList(raycaster.hitsList, typeof((GameObject, Vector3)), false, true, false, false);
                reordableList.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, $"Hits {raycaster.hitsList.Count()}");
                };
                reordableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    (GameObject, Vector3) element = raycaster.hitsList[index];

                    if (element.Item1 == null && element.Item2.Equals(Vector3.negativeInfinity))
                    {
                        EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width * 0.5f, EditorGUIUtility.singleLineHeight), "Hit some Object");
                        return;
                    }

                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width * 0.5f, EditorGUIUtility.singleLineHeight), element.Item1.name);
                    EditorGUI.LabelField(new Rect(rect.x+ rect.width * 0.5f, rect.y, rect.width * 0.5f, EditorGUIUtility.singleLineHeight), element.Item2.ToString(), rightAlignedTextStyle);
                };
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                castModule = serializedObject.FindProperty("castModule");
                castForm = serializedObject.FindProperty("castForm");
                castShape = serializedObject.FindProperty("castShape");
                castType = serializedObject.FindProperty("castType");
                castRate = serializedObject.FindProperty("castRate");
                castDistance = serializedObject.FindProperty("castDistance");
                castDirection = serializedObject.FindProperty("castDirection");

                avaiableModules = raycaster.GetModules();
                avaiableForms = raycaster.GetForms();
                avaiableShapes = raycaster.GetShapes();
                avaiableTypes = raycaster.GetTypes();
                avaiableRates = raycaster.GetRates();

                EditorGUI.BeginChangeCheck();
                enumIndex = avaiableModules.ToList().IndexOf(castModule.enumNames[castModule.enumValueIndex]);
                enumIndex = EditorGUILayout.Popup("Cast Module", enumIndex, avaiableModules);
                if (EditorGUI.EndChangeCheck())
                {
                    castModule.enumValueIndex = (int)Enum.Parse(typeof(CastModule), avaiableModules[enumIndex]);
                }
                EditorGUI.BeginChangeCheck();
                enumIndex = avaiableForms.ToList().IndexOf(castForm.enumNames[castForm.enumValueIndex]);
                enumIndex = EditorGUILayout.Popup("Cast Form", enumIndex, avaiableForms);
                if (EditorGUI.EndChangeCheck())
                {
                    castForm.enumValueIndex = (int)Enum.Parse(typeof(CastForm), avaiableForms[enumIndex]);
                }
                EditorGUI.BeginChangeCheck();
                enumIndex = avaiableShapes.ToList().IndexOf(castShape.enumNames[castShape.enumValueIndex]);
                enumIndex = EditorGUILayout.Popup("Cast Shape", enumIndex, avaiableShapes);
                if (EditorGUI.EndChangeCheck())
                {
                    castShape.enumValueIndex = (int)Enum.Parse(typeof(CastShape), avaiableShapes[enumIndex]);
                }
                EditorGUI.BeginChangeCheck();
                enumIndex = avaiableTypes.ToList().IndexOf(castType.enumNames[castType.enumValueIndex]);
                enumIndex = EditorGUILayout.Popup("Cast Type", enumIndex, avaiableTypes);
                if (EditorGUI.EndChangeCheck())
                {
                    castType.enumValueIndex = (int)Enum.Parse(typeof(CastType), avaiableTypes[enumIndex]);
                }
                EditorGUI.BeginChangeCheck();
                enumIndex = avaiableRates.ToList().IndexOf(castRate.enumNames[castRate.enumValueIndex]);
                enumIndex = EditorGUILayout.Popup("Cast Rate", enumIndex, avaiableRates);
                if (EditorGUI.EndChangeCheck())
                {
                    castRate.enumValueIndex = (int)Enum.Parse(typeof(CastRate), avaiableRates[enumIndex]);
                }

                if ((CastForm)castForm.enumValueIndex != CastForm.Overlap)
                {
                    EditorGUILayout.Space();

                    GUILayout.Label("Cast options");
                    EditorGUI.indentLevel++;

                    EditorGUI.BeginChangeCheck();
                    castDistance.floatValue = EditorGUILayout.Slider(new GUIContent("Distance", "Cast distance; max means Inf."), castDistance.floatValue, 1.0f, 100.0f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (castDistance.floatValue == 100.0f)
                            castDistance.floatValue = Mathf.Infinity;
                    }

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(castDirection, new GUIContent("Direction"));
                    if (EditorGUI.EndChangeCheck())
                    {
                        if ((CastModule)castModule.enumValueIndex == CastModule.Physics2D)
                            castDirection.vector3Value = new Vector2(castDirection.vector3Value.x, castDirection.vector3Value.y).normalized;
                        else
                            castDirection.vector3Value = castDirection.vector3Value.normalized;
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.Space();

                reordableList.DoLayoutList();

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}