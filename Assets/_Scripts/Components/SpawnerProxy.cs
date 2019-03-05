using UnityEngine;
using Unity.Entities;
using System;


[Serializable]
public struct Spawner : ISharedComponentData
{
    public GameObject playerPrefab;
    public GameObject obstaclePrefab;
}

[DisallowMultipleComponent]
public class SpawnerProxy : SharedComponentDataProxy<Spawner> { }
