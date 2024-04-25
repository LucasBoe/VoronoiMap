using System.Linq;
using UnityEngine;
using static VoronoiMap.Voronoi2DTo3D;

namespace VoronoiMap
{
    [System.Serializable]
    public class VoronoiCellData
    {
        public Vector2 Center;
        public Vector2[] Edges;
        public Vector3[] Edges3D;
        public VoronoiCellData() { }
        public VoronoiCellData(Vector2 point, Vector2[] edges)
        {
            this.Center = point;
            this.Edges = edges;
            this.Edges3D = edges.Select(e => ToV3(e)).ToArray();
        }
    }
}