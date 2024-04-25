using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static VoronoiMap.Voronoi2DTo3D;

public class CellOutline : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;

    // Update is called once per frame
    void Update()
    {
        if (!MapHolder.HasMap)
            return;

        var closest = MapHolder.Map.Cells.OrderBy(c => Vector2.Distance(c.Center, ToV2(transform.position))).First();
        lineRenderer.positionCount = closest.Edges.Length;
        for (int i = 0; i < closest.Edges.Length; i++)
        {
            lineRenderer.SetPosition(i, ToV3(closest.Edges[i], transform.position.y));
        }
    }
}