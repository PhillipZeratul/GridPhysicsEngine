using System;
using Unity.Entities;


// Serializable attribute is for editor support.
[Serializable]
public struct Initializer : IComponentData
{}

// ComponentDataProxy is for creating a MonoBehaviour representation of this component (for editor support).
[UnityEngine.DisallowMultipleComponent]
public class InitializerProxy : ComponentDataProxy<Initializer> { }
