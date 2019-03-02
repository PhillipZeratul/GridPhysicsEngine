using Unity.Entities;
using System;


[Serializable]
public struct Camera : IComponentData
{}

[UnityEngine.DisallowMultipleComponent]
public class CameraProxy : ComponentDataProxy<Camera> { }
