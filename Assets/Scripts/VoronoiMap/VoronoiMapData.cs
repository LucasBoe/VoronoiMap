using Delaunay;
using Delaunay.Geo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace VoronoiMap
{

    [System.Serializable]
    public partial class VoronoiMapData
    {
        public Vector2 Size { get; private set; }
        public List<VoronoiCellData> Cells = new();
        public List<CellBindProcedure> Fails = new();
        private List<Vector2Pair> allEdges;
        private MapGenerationData generationData;
        public VoronoiMapData(Vector2 size, float cellSizeInSquareUnity = 4f, int smoothStepCount = 5, bool generateCellsAutomatically = true)
        {
            Size = size;

            float sizeX = size.x;
            float sizeY = size.y;

            int pointCount = Mathf.RoundToInt((sizeX * sizeY) / cellSizeInSquareUnity);

            generationData = new(pointCount, size);
            Voronoi voronoi = VoronoiUtil.GenerateSmoothedVoronoi(smoothStepCount, generationData);
            allEdges = VoronoiUtil.ExtractEdgesFrom(voronoi);

            if (generateCellsAutomatically)
            {
                foreach (Vector2 point in generationData.Points)
                {
                    var proc = new CellBindProcedure(point, allEdges);                   

                    if (proc.TryExecuteFull())
                        Cells.Add(proc.Cell);
                    else
                        Fails.Add(proc);
                }
            }
        }

        internal IEnumerator GenerateCellsRoutine()
        {
            foreach (Vector2 point in generationData.Points)
            {
                var proc = new CellBindProcedure(point, allEdges);

                while (proc.State == ProcedureState.PENDING)
                    proc.TryExecuteSingleStep();

                if (proc.State == ProcedureState.SUCCESS)
                    Cells.Add(proc.Cell);

                if (proc.State == ProcedureState.FAILURE && proc.TryCount > 50)
                {
                    Fails.Add(proc);
                    yield return new WaitForSeconds(1f);
                }
            }
        }
    }
}
