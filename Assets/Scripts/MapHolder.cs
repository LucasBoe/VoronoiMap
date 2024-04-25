using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoronoiMap;

[SingletonSettings(_lifetime: SingletonLifetime.Scene, _canBeGenerated: true)]
public class MapHolder : SingletonBehaviour<MapHolder>
{
    private bool hasMap = false;
    private VoronoiMapData mapData;
    public static VoronoiMapData Map => Instance.mapData;
    public static bool HasMap => Instance.hasMap;
    internal void FeedMap(VoronoiMapData mapData)
    {
        this.mapData = mapData;
        hasMap = true;
    }
}
