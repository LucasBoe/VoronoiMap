using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoronoiMap;

public class Boot : MonoBehaviour
{
    [SerializeField] VoronoiMeshCreator meshCreator;
    [SerializeField] VoronoiMapGizmoDrawer gizmoDrawer;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BootRoutine());
    }

    private IEnumerator BootRoutine()
    {
        while (true)
        {
            VoronoiMapData map = new VoronoiMapData(new Vector2(25, 25), generateCellsAutomatically: false);
            gizmoDrawer.Map = map;

            yield return map.GenerateCellsRoutine();

            meshCreator.GenerateMesh(map);

            yield return null;

            while (!Input.GetKeyUp(KeyCode.R))
                yield return null;

            meshCreator.DeleteLastMesh();

            yield return null;
        }
    }
}
