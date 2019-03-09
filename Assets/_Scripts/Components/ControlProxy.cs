using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using System;


[Serializable]
public struct Control : IComponentData
{
    public float2 move;
}

[DisallowMultipleComponent]
public class ControlProxy : ComponentDataProxy<Control>
{
}
