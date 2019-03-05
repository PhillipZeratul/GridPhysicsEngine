using System;
using Unity.Entities;
using UnityEngine;


// Serializable attribute is for editor support.
[Serializable]
public struct Obstacle : IComponentData
{
    [Range(0f, 10f)]
    public float maxRndMoveSpeed; 
}

// ComponentDataProxy is for creating a MonoBehaviour representation of this component (for editor support).
[DisallowMultipleComponent]
public class ObstacleProxy : ComponentDataProxy<Obstacle> { }