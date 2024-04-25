using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoronoiMap
{
    public static class Voronoi2DTo3D
    {
        public static Vector3 ToV3(Vector2 v2, float height = 0f)
        {
            return new Vector3(v2.x, height, v2.y);
        }
        public static Vector2 ToV2(Vector3 v3)
        {
            return new Vector2(v3.x, v3.z);
        }
    }
}
