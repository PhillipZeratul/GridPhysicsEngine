using Unity.Entities;
using Unity.Collections;
using UnityEngine;


public class PlayerControlSystem : ComponentSystem
{
    private ComponentGroup playerGroup;


    protected override void OnCreateManager()
    {
        playerGroup = GetComponentGroup(typeof(Player), typeof(PhysicsObject));
    }

    protected override void OnUpdate()
    {
        using (var playerEntities = playerGroup.ToEntityArray(Allocator.TempJob))
            foreach (var player in playerEntities)
            {
                // ~TODO: Get Input
            }
    }
}
