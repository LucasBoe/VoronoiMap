using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static VoronoiMap.Voronoi2DTo3D;

namespace VoronoiMap
{
    public class VoronoiMapGizmoDrawer : MonoBehaviour
    {
        private VoronoiMapData mapData;

        public VoronoiMapData Map { get => mapData; set => mapData = value; }

        private void OnDrawGizmos()
        {
            if (mapData == null)
                return;

            for (int i = 0; i < mapData.Cells.Count; i++)
            {
                VoronoiCellData cell = mapData.Cells[i];
                Gizmos.color = Color.HSVToRGB((float)i / mapData.Cells.Count, 1, 1);
                for (int j = 1; j < cell.Edges3D.Length; j++)
                {
                    Gizmos.DrawLine(cell.Edges3D[j - 1], cell.Edges3D[j]);
                }
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(0, 0, mapData.Size.y));
            Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(mapData.Size.x, 0, 0));
            Gizmos.DrawLine(new Vector3(mapData.Size.x, 0, 0), new Vector3(mapData.Size.x, 0, mapData.Size.y));
            Gizmos.DrawLine(new Vector3(0, 0, mapData.Size.y), new Vector3(mapData.Size.x, 0, mapData.Size.y));

            Gizmos.color = Color.red;
            foreach (var item in mapData.Fails)
            {
                Gizmos.DrawWireSphere(ToV3(item), .2f);
            }
        }
    }
}