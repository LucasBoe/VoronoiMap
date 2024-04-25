using Delaunay;
using Delaunay.Geo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

namespace VoronoiMap
{

    [System.Serializable]
    public class VoronoiMapData
    {
        public Vector2 Size { get; private set; }

        public List<VoronoiCellData> Cells = new();
        public List<Vector2> Fails = new();
        public VoronoiMapData() { }
        public VoronoiMapData(Vector2 size, float cellSizeInSquareUnity = 4f, int smoothStepCount = 5, bool generateCellsAutomatically = true)
        {
            Size = size;

            float sizeX = size.x;
            float sizeY = size.y;

            int pointCount = Mathf.RoundToInt((sizeX * sizeY) / cellSizeInSquareUnity);

            MapGenerationData generationData = new(pointCount, size);
            Voronoi voronoi = VoronoiUtil.GenerateSmoothedVoronoi(smoothStepCount, generationData);
            List<Vector2Pair> edges = VoronoiUtil.ExtractEdgesFrom(voronoi);

            if (generateCellsAutomatically)
            {
                foreach (Vector2 point in generationData.Points)
                {
                    var proc = new CellBindProcedure(point, edges);
                    proc.ExecuteFull();

                    if (proc.HadSuccess)
                        Cells.Add(proc.Cell);
                    else
                        Fails.Add(point);
                }
            }
        }
        private class CellBindProcedure
        {
            private Vector2 center;
            private List<Vector2Pair> allEdges;
            private Vector2 startingPoint;

            private Vector2 currentPoint, nextPoint;
            private int tryCount = 0;

            private List<Vector2> ownEdgePoints = new();
            public CellBindProcedure(Vector2 center, List<Vector2Pair> allEdges)
            {
                this.center = center;
                this.allEdges = allEdges;
                this.startingPoint = allEdges.OrderBy(p => Vector2.Distance(center, p.A)).FirstOrDefault().A;
                this.ownEdgePoints = new List<Vector2> { startingPoint };
            }
            public bool HadSuccess { get; private set; }
            public VoronoiCellData Cell { get; private set; }
            internal void ExecuteFull()
            {
                HadSuccess = true;

                while (allEdges.Count < 4 || !SameDistance(nextPoint, ownEdgePoints.First()))
                {
                    currentPoint = ownEdgePoints.Last();

                    //try to find next point
                    if (tryCount > 100 || !FindNext(currentPoint, allEdges, center, ref ownEdgePoints, out var _next))
                    {
                        HadSuccess = false;
                        break;
                    }

                    nextPoint = _next;

                    if (!SameDistance(nextPoint, ownEdgePoints.First()) && ContainsDistance(ownEdgePoints, nextPoint))
                    {
                        HadSuccess = false;
                        break;
                    }

                    ownEdgePoints.Add(nextPoint);
                    tryCount++;
                }

                Cell = new VoronoiCellData(center, ownEdgePoints.ToArray());
            }
            private bool FindNext(Vector2 current, List<Vector2Pair> edges, Vector2 center, ref List<Vector2> exclude, out Vector2 next)
            {
                var previous = exclude.ToArray();
                var filtered = CreatePoolFromDistanceComparison(current, edges, exclude);

                if (filtered.Count() == 0 || filtered.Count() < 2 && SameDistance(filtered[0], previous.First()))
                {
                    next = Vector2.zero;
                    return false;
                }

                next = filtered.OrderBy(point => AngleDelta(current, point, center)).Last();
                return true;
            }
            private Vector2[] CreatePoolFromDistanceComparison(Vector2 current, List<Vector2Pair> edges, List<Vector2> filter)
            {
                List<Vector2> pool = new List<Vector2>();

                foreach (var edge in edges)
                {
                    var points = new Vector2[] { edge.A, edge.B };

                    foreach (var point in points)
                    {
                        if (Vector2.Distance(point, current) < 0.02f)
                            pool.Add(edge.Other(point));
                    }
                }

                var first = filter.First();
                return pool.Where(p => SameDistance(p, first) || !ContainsDistance(filter.ToList(), p)).ToArray();
            }
            private bool ContainsDistance(List<Vector2> list, Vector2 item)
            {
                foreach (var point in list)
                {
                    if (SameDistance(point, item))
                        return true;
                }
                return false;
            }
            private bool SameDistance(Vector2 a, Vector2 b)
            {
                return (Vector2.Distance(a, b) < 0.02f);
            }
            private float AngleDelta(Vector2 a, Vector2 b, Vector2 center)
            {
                float angleA = Mathf.Atan2(a.x - center.x, a.y - center.y) * Mathf.Rad2Deg;
                float angleB = Mathf.Atan2(b.x - center.x, b.y - center.y) * Mathf.Rad2Deg;

                return Mathf.DeltaAngle(angleA, angleB);
            }
        }
    }
}
