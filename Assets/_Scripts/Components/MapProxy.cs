using Unity.Entities;
using UnityEngine;
using System;


// Serializable attribute is for editor support.
[Serializable]
public struct Map : ISharedComponentData
{
    // int array of length rows x cols
    // 0 for road
    // 1 for wall

    [Range(5, 131)]
    public int rows;
    [Range(5, 131)]
    public int cols;

    public GameObject wallPrefab;

    [HideInInspector]
    public int[] mapArray;
}

// ComponentDataProxy is for creating a MonoBehaviour representation of this component (for editor support).
[DisallowMultipleComponent]
public class MapProxy : SharedComponentDataProxy<Map> { }
