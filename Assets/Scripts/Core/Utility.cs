using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Venice
{
    /// <summary>
    /// Quick Vectorial Utility scripts Courtesy of Strix
    /// </summary>
    public static class VectorSwizzle
    {
        public static Vector3 xyz(this Vector3 v) => new Vector3(v.x, v.y, v.z);
        public static Vector3 xzy(this Vector3 v) => new Vector3(v.x, v.z, v.y);
        public static Vector3 yxz(this Vector3 v) => new Vector3(v.y, v.x, v.z);
        public static Vector3 yzx(this Vector3 v) => new Vector3(v.y, v.z, v.x);

        public static Vector3 xyz(this Vector2 v) => new Vector3(v.x, v.y, 0);
        public static Vector3 xzy(this Vector2 v) => new Vector3(v.x, 0, v.y);
        public static Vector3 yxz(this Vector2 v) => new Vector3(v.y, v.x, 0);
        public static Vector3 yzx(this Vector2 v) => new Vector3(v.y, 0, v.x);
    }

    public static class MathUtility
    {
        public static void Split(this Vector3 v, Vector3 normal, out Vector3 lat, out Vector3 ver)
        {
            lat = Vector3.ProjectOnPlane(v, normal);
            ver = v - lat;
        }

        public static Vector3 ProjectOnUp(this Vector3 v) => Vector3.ProjectOnPlane(v, Vector3.up);

        public static Vector3 Bezier(Vector3 start, Vector3 end, Vector3 pivot, float time)
        {
            var v1 = Vector3.Lerp(start, pivot, time);
            var v2 = Vector3.Lerp(pivot, end, time);
            return Vector3.Lerp(v1, v2, time);
        }

        public static Vector3 Bezier2(Vector3 start, Vector3 end, Vector3 pivot1, Vector3 pivot2, float time)
        {
            var v1 = Vector3.Lerp(start, pivot1, time);
            var v2 = Vector3.Lerp(pivot1, pivot2, time);
            var v3 = Vector3.Lerp(pivot2, end, time);
            return Vector3.Lerp(Vector3.Lerp(v1, v2, time), Vector3.Lerp(v2, v3, time), time);
        }

        public static Vector3 LerpAngle(Vector3 a, Vector3 b, float t)
        {
            return new Vector3(Mathf.LerpAngle(a.x, b.x, t),
                Mathf.LerpAngle(a.y, b.y, t),
                Mathf.LerpAngle(a.z, b.z, t));
        }
    }
}
