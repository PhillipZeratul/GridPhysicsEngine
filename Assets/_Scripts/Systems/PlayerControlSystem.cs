using Unity.Entities;
using Unity.Collections;
using UnityEngine;


[UpdateAfter(typeof(ControlSystem))]
public class PlayerControlSystem : ComponentSystem
{
    private ComponentGroup playerGroup;
    private ComponentGroup controlGroup;


    protected override void OnCreateManager()
    {
        playerGroup = GetComponentGroup(typeof(Player), typeof(PhysicsObject));
        controlGroup = GetComponentGroup(typeof(Control));
    }

    protected override void OnUpdate()
    {
        if (playerGroup.CalculateLength() != 1)
            throw new System.Exception("Number of Player is not 1!");
        if (controlGroup.CalculateLength() != 1)
            throw new System.Exception("Number of Control is not 1!");

        float dT = Time.deltaTime;

        var playerEntities = playerGroup.ToEntityArray(Allocator.TempJob);
        var controlEntities = controlGroup.ToEntityArray(Allocator.TempJob);

        var physicsObject = EntityManager.GetComponentData<PhysicsObject>(playerEntities[0]);
        var control = EntityManager.GetComponentData<Control>(controlEntities[0]);

        physicsObject.dx += physicsObject.moveSpeed * dT * control.move.x;
        physicsObject.dy += physicsObject.moveSpeed * dT * control.move.y;

        if (physicsObject.dx > physicsObject.maxSpeed) physicsObject.dx = physicsObject.maxSpeed;
        if (physicsObject.dx < -physicsObject.maxSpeed) physicsObject.dx = -physicsObject.maxSpeed;
        if (physicsObject.dy > physicsObject.maxSpeed) physicsObject.dy = physicsObject.maxSpeed;
        if (physicsObject.dy < -physicsObject.maxSpeed) physicsObject.dy = -physicsObject.maxSpeed;

        EntityManager.SetComponentData<PhysicsObject>(playerEntities[0], physicsObject);



        playerEntities.Dispose();
        controlEntities.Dispose();
    }
}
