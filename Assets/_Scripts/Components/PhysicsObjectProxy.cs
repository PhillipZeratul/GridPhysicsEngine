using System;
using Unity.Entities;


// Serializable attribute is for editor support.
[Serializable]
public struct PhysicsObject : IComponentData
{
    // Base coordinates
    public int cx;
    public int cy;
    public float rx;
    public float ry;

    // Resulting coordinates
    public float xx;
    public float yy;

    // Movements
    public float dx;
    public float dy;

    // Collision raduis
    public float radius;

    public float moveSpeed;
    public float maxSpeed;
}

// ComponentDataProxy is for creating a MonoBehaviour representation of this component (for editor support).
[UnityEngine.DisallowMultipleComponent]
public class PhysicsObjectProxy : ComponentDataProxy<PhysicsObject> { }